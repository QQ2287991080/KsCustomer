using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using Jusoft.YiFang.Api.Help;
using Jusoft.YiFang.Db;
using Newtonsoft.Json;
using Demo.Models;

namespace Jusoft.YiFang.Api
{
    /// <summary>
    /// DdApproval 的摘要说明
    /// 钉钉审批回调
    /// </summary>
    public class DdApproval : IHttpHandler
    {
        public YiFang_CustomerComplaintEntities dbContext = new YiFang_CustomerComplaintEntities();
        public void ProcessRequest(HttpContext context)
        {
            string mToken = Db.Models.Allocation.Token;
            string mSuiteKey = Db.Models.Allocation.CorpId;
            string mEncodingAesKey = Db.Models.Allocation.EncodingAESKey;
            //mSuiteKey = "suite4xxxxxxxxxxxxxxx";
            #region 获取回调URL里面的参数
            //url中的签名
            string msgSignature = context.Request["signature"];
            //url中的时间戳
            string timeStamp = context.Request["timestamp"];
            //url中的随机字符串
            string nonce = context.Request["nonce"];
            //post数据包数据中的加密数据
            string encryptStr = GetPostParam(context);
            #endregion
            //string sEchoStr = "";
            DingTalkCrypt dingTalk = new DingTalkCrypt(mToken, mEncodingAesKey, mSuiteKey);
            string plainText = "";
            int bools = dingTalk.DecryptMsg(msgSignature, timeStamp, nonce, encryptStr, ref plainText);
            string res = "success";
            #region 处理钉钉回调返回的数据 
            try
            {
                LogHelper.WriteLog("钉钉审批回调返回数据格式" + plainText);
                //获取token
                var DinToken = GetToken();
                LogHelper.WriteLog("回调token" + DinToken);
                Callback cb = Newtonsoft.Json.JsonConvert.DeserializeObject<Callback>(plainText);
                switch (cb.EventType)
                {
                    case "bpms_instance_change":
                        #region 审批实列
                        LogHelper.WriteLog("审批实列");
                        var KsCusmoterReal = dbContext.KS_Customer.FirstOrDefault(p => p.DingTalkApproval == cb.processInstanceId);
                        if (cb.result == "refuse")
                        {
                            //客诉审批状态为拒绝
                            KsCusmoterReal.State = 4;
                        }
                        else if (cb.result == "agree")
                        {
                            //客诉审批状态为完成
                            KsCusmoterReal.State = 2;
                        }
                        #endregion
                        break;
                    case "bpms_task_change":
                        #region 审批任务
                        if (cb.type== "finish")
                        {
                            var KsCusmoter = dbContext.KS_Customer.FirstOrDefault(p => p.DingTalkApproval == cb.processInstanceId);
                            if (KsCusmoter != null)
                            {
                                //获取审批实列详情
                                var ApprovalDetail = Db.ThirdSystem.AccessToken.ApprovalDetails(cb.processInstanceId, DinToken);
                                if (ApprovalDetail.Errcode != 0)
                                {
                                    LogHelper.WriteLog("获取用户详情失败：:" + ApprovalDetail.Errmsg);
                                }
                                //把json格式的审批实列详情转换为实体类
                                var data = Newtonsoft.Json.JsonConvert.DeserializeObject<Help.Root>(ApprovalDetail.Body);
                                LogHelper.WriteLog("【审批任务】" + ApprovalDetail.Body);
                                //实例化审批记录表
                                KS_Customer_Approval KsCusApp = new KS_Customer_Approval();
                                var OrPerson = dbContext.OR_Person.FirstOrDefault(p => p.PsnNum == cb.staffId);
                                //审批人
                                LogHelper.WriteLog("审批人" + KsCusApp.IdPerson);
                                if (OrPerson != null)
                                {
                                    KsCusApp.IdPerson = OrPerson.Id;
                                    KsCusApp.Name = OrPerson.Name;
                                }
                                KsCusApp.IdCustomer = KsCusmoter.Id;
                                var FinishTime = Helper.DateTimeToStamp(cb.finishTime);
                                KsCusApp.FinishTime = FinishTime;
                                foreach (var item in data.process_instance.tasks)
                                {
                                    if (KsCusApp.FinishTime == item.create_time)
                                    {
                                        //获取tasks集合中第一个用户id，就是下个审批人的用户id
                                        var PersonApproval = dbContext.OR_Person.FirstOrDefault(p => p.PsnNum == item.userid);
                                        if (PersonApproval != null)
                                        {
                                            KsCusmoter.IdPersonApproval = PersonApproval.Id;
                                        }
                                    }
                                }
                                //type="finish"并且result== "redirect为审批任务转交
                                if (data.process_instance.status == "RUNNING")
                                {
                                    //客诉审批记录为同意
                                    KsCusApp.state = 1;
                                }
                                //type="finish"并且result== "refuse"为审批任务拒绝
                                else if (data.process_instance.result == "refuse")
                                {
                                    //status: "COMPLETED"
                                    //客诉审批记录为拒绝
                                    KsCusApp.state = 2;
                                    //拒绝理由
                                }
                                //type="finish"为审批完成
                                else if (data.process_instance.status == "COMPLETED")
                                {
                                    LogHelper.WriteLog("审批任务完成");
                                    //客诉审批记录为同意
                                    KsCusApp.state = 1;
                                    KsCusApp.FinishTime = DateTime.Now;
                                }
                                KsCusApp.Remark = cb.remark;
                                dbContext.KS_Customer_Approval.Add(KsCusApp);
                            }
                        }
                        else if (cb.type=="start")
                        {
                            var KsCusmoter = dbContext.KS_Customer.FirstOrDefault(p => p.DingTalkApproval == cb.processInstanceId);
                            //获取审批实列详情
                            var ApprovalDetail = Db.ThirdSystem.AccessToken.ApprovalDetails(cb.processInstanceId, DinToken);
                            if (ApprovalDetail.Errcode != 0)
                            {
                                LogHelper.WriteLog("获取用户详情失败：:" + ApprovalDetail.Errmsg);
                            }
                            LogHelper.WriteLog("【发起审批】" + ApprovalDetail.Body);
                            //把json格式的审批实列详情转换为实体类
                            var data = Newtonsoft.Json.JsonConvert.DeserializeObject<Help.Root>(ApprovalDetail.Body);
                            foreach (var item in data.process_instance.tasks)
                            {
                                if (data.process_instance.create_time == item.create_time)
                                {
                                    //获取tasks集合中第一个用户id，就是下个审批人的用户id
                                    var PersonApproval = dbContext.OR_Person.FirstOrDefault(p => p.PsnNum == item.userid);
                                    if (PersonApproval != null)
                                    {
                                        KsCusmoter.IdPersonApproval = PersonApproval.Id;
                                    }
                                }
                            }
                        }
                        #endregion
                        break;
                    case "user_add_org":
                    case "user_modify_org":
                    case "user_leave_org":
                        #region 用户回调
                        foreach (var item in cb.UserId)
                        {
                            LogHelper.WriteLog("用户Id：:" + item);
                            var DdPersonInfo = Db.ThirdSystem.AccessToken.CallbackUserInfo(item, DinToken);
                            if (DdPersonInfo.Errcode != 0)
                            {
                                LogHelper.WriteLog("获取用户详情失败：:" + DdPersonInfo.Errmsg);
                            }
                           var PersonInfo = dbContext.OR_Person.FirstOrDefault(p => p.PsnNum == item);
                            //找到回调的用户
                            if (PersonInfo != null)
                            {
                                //判断为user_modify_org是更改用户信息
                                //否则就是用户离职
                                if (cb.EventType == "user_modify_org")
                                {
                                    //修改用户
                                    PersonInfo.Name = DdPersonInfo.Name;
                                    PersonInfo.PsnMobilePhone = DdPersonInfo.Mobile;
                                    PersonInfo.PsnEmail = DdPersonInfo.Email;
                                    PersonInfo.HeadUrl = DdPersonInfo.Extattr;
                                    PersonInfo.CodeDepartment = DdPersonInfo.Department[0].ToString();
                                }
                                else
                                {
                                    //删除用户
                                    PersonInfo.LeaveDate = DateTime.Now;
                                    PersonInfo.CodeDepartment = "1";
                                }
                            }
                            else
                            {
                                //新增用户
                                OR_Person OrPerson = new OR_Person();
                                OrPerson.PsnNum = item;
                                OrPerson.LoginName = item;
                                PersonInfo.Name = DdPersonInfo.Name;
                                PersonInfo.PsnMobilePhone = DdPersonInfo.Mobile;
                                PersonInfo.PsnEmail = DdPersonInfo.Email;
                                PersonInfo.HeadUrl = DdPersonInfo.Extattr;
                                PersonInfo.CodeDepartment = DdPersonInfo.Department[0].ToString();
                                PersonInfo.CreateTime = DateTime.Now;
                                PersonInfo.Sex = 0;
                                dbContext.OR_Person.Add(OrPerson);
                            }
                        }
                        #endregion
                        break;
                    case "org_dept_create":
                    case "org_dept_modify":
                    case "org_dept_remove":
                        #region 部门回调
                        foreach (var item in cb.DeptId)
                        {
                            var DdDepInfo = Db.ThirdSystem.AccessToken.CallbackDepInfo(item, DinToken);
                            if (DdDepInfo.Errcode != 0)
                            {
                                LogHelper.WriteLog("获取用户详情失败：:" + DdDepInfo.Errmsg);
                            }
                            var DelDepInfo = dbContext.OR_Department.FirstOrDefault(p => p.Code == item);
                            //判断是否找到该部门
                            //否则就是创建部门
                            if (DelDepInfo != null)
                            {
                                //找到判断是更改部门信息
                                //还是删除部门
                                if (cb.EventType == "org_dept_modify")
                                {
                                    //修改部门
                                    DelDepInfo.Name = DdDepInfo.Name;
                                    DelDepInfo.CodeDepartment = DdDepInfo.Parentid.ToString();
                                }
                                else
                                {
                                    //删除部门
                                    dbContext.OR_Department.Remove(DelDepInfo);
                                }
                            }
                            else
                            {
                                //新增部门
                                OR_Department OrDepInfo = new OR_Department();
                                OrDepInfo.Name = DdDepInfo.Name;
                                OrDepInfo.Code = DdDepInfo.Id.ToString();
                                OrDepInfo.CodeDepartment = DdDepInfo.Parentid.ToString();
                                OrDepInfo.CreateTime = DateTime.Now;
                                dbContext.OR_Department.Add(OrDepInfo);
                            }
                        }
                        #endregion
                        break;
                }
                LogHelper.WriteLog("回调执行完成，");
                dbContext.SaveChanges();
                LogHelper.WriteLog("回调成功，保存成功");
            }
            catch (Exception ex)
            {
                LogHelper.WriteLog("回调异常信息："+ex.Message);
            }
            #endregion
            //DingTalk_call_back_tag.call_back_tag(bools, plainText);
            timeStamp = Help.DingTalkCrypt.GetTimeStamp().ToString();
            string encrypt = "";
            string signature = "";
            dingTalk = new DingTalkCrypt(mToken, mEncodingAesKey, mSuiteKey);
            dingTalk.EncryptMsg(res, timeStamp, nonce, ref encrypt, ref signature);
            Hashtable jsonMap = new Hashtable
                {
                    {"msg_signature", signature},
                    {"encrypt", encrypt},
                    {"timeStamp", timeStamp},
                    {"nonce", nonce}
                };
            string result = JsonConvert.SerializeObject(jsonMap);
            context.Response.Write(result);
        }
        private string GetPostParam(HttpContext context)
        {
            if ("POST" == context.Request.RequestType)
            {
                Stream sm = context.Request.InputStream;//获取post正文
                int len = (int)sm.Length;//post数据长度
                byte[] inputByts = new byte[len];//字节数据,用于存储post数据
                sm.Read(inputByts, 0, len);//将post数据写入byte数组中
                sm.Close();//关闭IO流

                //**********下面是把字节数组类型转换成字符串**********

                string data = Encoding.UTF8.GetString(inputByts);//转为String
                data = data.Replace("{\"encrypt\":\"", "").Replace("\"}", "");
                return data;
            }
            return "get方法";
        }
        /// <summary>
        ///获取token
        /// </summary>
        /// <returns></returns>
        public string GetToken()
        {
            string key = ConfigurationManager.AppSettings["appkey"];
            string secret = ConfigurationManager.AppSettings["appsecret"];
            var jsonstr = Db.ThirdSystem.AccessToken.GetAccessToken();
            //if (jsonstr.Errcode != 0)
            //{
            //    //myways.WriteLog($"获取token失败【{jsonstr.Errmsg}】");
            //}
            //Help.Root root = Newtonsoft.Json.JsonConvert.DeserializeObject<Help.Root>(jsonstr.Body);
            if (!string.IsNullOrEmpty(jsonstr))
            {
                return jsonstr;
            }
            else
            {
                return "";
            }
        }
        public bool IsReusable
        {
            get
            {
                return false;
            }
        }
    }
}
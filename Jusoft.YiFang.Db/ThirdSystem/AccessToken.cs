using DingTalk.Api;
using DingTalk.Api.Request;
using DingTalk.Api.Response;
using Jusoft.YiFang.Db.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Caching;

namespace Jusoft.YiFang.Db.ThirdSystem
{
    public class AccessToken
    {
        #region 数据结构相关
        private struct Access_Token
        {
            public int errcode { get; set; }
            public string errmsg { get; set; }
            public int expires_in { get; set; }
            public string access_token { get; set; }
        }
        #endregion

        #region 获取AccessToken
        public static string GetAccessToken()
        {
            // 将AccessToken存储至缓存
            SetAccessToken(new CacheItemRemovedReason());
            // 输出缓存中存储的AccessToken
            return HttpRuntime.Cache.Get("Dingtalk_AccessToken").ToString();
        }
        #endregion

        #region 设置AccessToken(企业内部开发)
        public static void SetAccessToken(CacheItemRemovedReason reason)
        {
            //企业内部开发
            var accessToken = HttpRuntime.Cache.Get("Dingtalk_AccessToken");
            if (accessToken == null)
            {
                DefaultDingTalkClient client = new DefaultDingTalkClient("https://oapi.dingtalk.com/gettoken");
                OapiGettokenRequest request = new OapiGettokenRequest();
                request.Appkey = Allocation.Appkey;
                request.Appsecret = Allocation.Appsecret;
                request.SetHttpMethod("GET");
                OapiGettokenResponse response = client.Execute(request);
                HttpRuntime.Cache.Insert("Dingtalk_AccessToken", response.AccessToken, null, DateTime.Now.AddSeconds(3600), Cache.NoSlidingExpiration);
            }
            //定制服务商
            //var accessToken = HttpRuntime.Cache.Get("Dingtalk_AccessToken");
            //if (accessToken == null)
            //{
            //    DefaultDingTalkClient client = new DefaultDingTalkClient(Config.ServerUrl);
            //    OapiServiceGetCorpTokenRequest req = new OapiServiceGetCorpTokenRequest();
            //    req.AuthCorpid = "ding0f2ae4da768e493235c2f4657eb6378f";
            //    OapiServiceGetCorpTokenResponse execute = client.Execute(req, "suitev6ajx5rbjrfqph0m", "y26DwWRab7il6nEe_oE9ib0xjwzPsG83KWyZBuS2ONVxTHZlk1spWyyec27m-Avw", Config.SuiteTicket);

            //    Access_Token access_Token = JsonConvert.DeserializeObject<Access_Token>(execute.Body);

            //    HttpRuntime.Cache.Insert("Dingtalk_AccessToken", access_Token.access_token, null, DateTime.Now.AddSeconds(3600), Cache.NoSlidingExpiration);
            //}

        }
        #endregion
        #region 获取用户userID
        public static OapiUserGetuserinfoResponse GetUserId(string Code, string Token)
        {
            IDingTalkClient client = new DefaultDingTalkClient("https://oapi.dingtalk.com/user/getuserinfo");
            OapiUserGetuserinfoRequest request = new OapiUserGetuserinfoRequest();
            request.Code = Code;
            request.SetHttpMethod("GET");
            OapiUserGetuserinfoResponse response = client.Execute(request, Token);
            return response;
        }
        #endregion

        #region 发送审批实列
        /// <summary>
        /// 发送审批实列
        /// </summary>
        /// <param name="kSCustomer">客诉</param>
        /// <param name="Token"></param>
        /// <param name="DbContext"></param>
        /// <returns></returns>
        public static OapiProcessinstanceCreateResponse SendTemplate(KS_Customer kSCustomer, int DepCode, string Userid, string Token, YiFang_CustomerComplaintEntities DbContext)
        {
            DefaultDingTalkClient client = new DefaultDingTalkClient("https://oapi.dingtalk.com/topapi/processinstance/create");
            OapiProcessinstanceCreateRequest request = new OapiProcessinstanceCreateRequest();
            request.AgentId = Allocation.AgentId;
            request.ProcessCode = Allocation.ProcessCode;
            request.DeptId = DepCode;
            request.OriginatorUserId = Userid;
            List<OapiProcessinstanceCreateRequest.FormComponentValueVoDomain> formComponentValues = new List<OapiProcessinstanceCreateRequest.FormComponentValueVoDomain>();
            //客诉门店
            var Person = DbContext.OR_Person.FirstOrDefault(p => p.Id == kSCustomer.IdPerson);
            //一方督导
            var PersonSupervision = DbContext.OR_Person.FirstOrDefault(p => p.Id == kSCustomer.IdPersonSupervision);
            //异常归类
            var Abnormal = DbContext.BA_SysEnType.FirstOrDefault(p => p.Id == kSCustomer.AbnormalId)?.Name;
            //客诉小类名称
            var customername = DbContext.BA_SysEnType.FirstOrDefault(p => p.Id == kSCustomer.SubclassId).Name;
            //所属大区
            //var regionid = DbContext.AC_SysUsers.FirstOrDefault(k => k.UserName == Person.LoginName).ST_Store.FirstOrDefault().RegionId;
            var regionid = 1;
            var RegionName = DbContext.BA_SysEnType.FirstOrDefault(p => p.Id == regionid)?.Name; ;
            switch (kSCustomer.StateId)
            {
                case 1:
                    #region 创建客诉类型为原物料的审批
                    OapiProcessinstanceCreateRequest.FormComponentValueVoDomain from1 = new OapiProcessinstanceCreateRequest.FormComponentValueVoDomain()
                    { Name = "客诉类型", Value = customername };
                    formComponentValues.Add(from1);
                    var from2 = new OapiProcessinstanceCreateRequest.FormComponentValueVoDomain();
                    from2.Name = "客诉内容";
                    from2.Value = kSCustomer.Remark + "";
                    formComponentValues.Add(from2);
                    var from3 = new OapiProcessinstanceCreateRequest.FormComponentValueVoDomain();
                    from3.Name = "客诉单号";
                    from3.Value = kSCustomer.Id + "";
                    formComponentValues.Add(from3);
                    var from4 = new OapiProcessinstanceCreateRequest.FormComponentValueVoDomain();
                    from4.Name = "提交时间";
                    from4.Value = kSCustomer.CreateTime + "";
                    formComponentValues.Add(from4);
                    var from5 = new OapiProcessinstanceCreateRequest.FormComponentValueVoDomain();
                    from5.Name = "异常产品";
                    from5.Value = kSCustomer.ProductIdNames + "";
                    formComponentValues.Add(from5);
                    var from6 = new OapiProcessinstanceCreateRequest.FormComponentValueVoDomain();
                    from6.Name = "异常数量";
                    from6.Value = kSCustomer.Number + "";
                    formComponentValues.Add(from6);
                    var from7 = new OapiProcessinstanceCreateRequest.FormComponentValueVoDomain();
                    from7.Name = "异常归类";
                    from7.Value = Abnormal + "";
                    formComponentValues.Add(from7);
                    var from8 = new OapiProcessinstanceCreateRequest.FormComponentValueVoDomain();
                    from8.Name = "到货日期";
                    from8.Value = kSCustomer.DeliveryDate + "";
                    formComponentValues.Add(from8);
                    var from9 = new OapiProcessinstanceCreateRequest.FormComponentValueVoDomain();
                    from9.Name = "生产批次";
                    from9.Value = kSCustomer.ProductionBatch + "";
                    formComponentValues.Add(from9);
                    var from10 = new OapiProcessinstanceCreateRequest.FormComponentValueVoDomain();
                    from10.Name = "一言为订订单号";
                    from10.Value = kSCustomer.OrderNumber + "";
                    formComponentValues.Add(from10);
                    var from11 = new OapiProcessinstanceCreateRequest.FormComponentValueVoDomain();
                    from11.Name = "一芳督导";
                    from11.Value = PersonSupervision?.Name + "";
                    formComponentValues.Add(from11);
                    var from12 = new OapiProcessinstanceCreateRequest.FormComponentValueVoDomain();
                    from12.Name = "客诉门店";
                    from12.Value = Person?.Id + "";
                    formComponentValues.Add(from12);
                    var from13 = new OapiProcessinstanceCreateRequest.FormComponentValueVoDomain();
                    from13.Name = "门店联系人";
                    from13.Value = kSCustomer.StoreContact + "";
                    formComponentValues.Add(from13);
                    var from14 = new OapiProcessinstanceCreateRequest.FormComponentValueVoDomain();
                    from14.Name = "详情";
                    from14.Value = "http://47.103.125.208:6071/#/detail?id=" + kSCustomer.Id + "&type=" + kSCustomer.StateId + "&state=" + kSCustomer.State + "";
                    formComponentValues.Add(from14);
                    var from15 = new OapiProcessinstanceCreateRequest.FormComponentValueVoDomain();
                    from15.Name = "所属大区";
                    from15.Value = RegionName;
                    formComponentValues.Add(from15);
                    #endregion
                    break;
                case 2:
                    #region 创建客诉类型为设备报修的审批
                    var Repair1 = new OapiProcessinstanceCreateRequest.FormComponentValueVoDomain();
                    Repair1.Name = "客诉类型";
                    Repair1.Value = customername;
                    formComponentValues.Add(Repair1);
                    var Repair2 = new OapiProcessinstanceCreateRequest.FormComponentValueVoDomain();
                    Repair2.Name = "客诉内容";
                    Repair2.Value = kSCustomer.Remark + "";
                    formComponentValues.Add(Repair2);
                    var Repair3 = new OapiProcessinstanceCreateRequest.FormComponentValueVoDomain();
                    Repair3.Name = "客诉单号";
                    Repair3.Value = kSCustomer.Id + "";
                    formComponentValues.Add(Repair3);
                    var Repair4 = new OapiProcessinstanceCreateRequest.FormComponentValueVoDomain();
                    Repair4.Name = "提交时间";
                    Repair4.Value = "" + kSCustomer.CreateTime;
                    formComponentValues.Add(Repair4);
                    var Repair5 = new OapiProcessinstanceCreateRequest.FormComponentValueVoDomain();
                    Repair5.Name = "异常产品";
                    Repair5.Value = "" + kSCustomer.ProductIdNames;
                    formComponentValues.Add(Repair5);
                    var Repair6 = new OapiProcessinstanceCreateRequest.FormComponentValueVoDomain();
                    Repair6.Name = "门店联系人";
                    Repair6.Value = "" + kSCustomer.StoreContact;
                    formComponentValues.Add(Repair6);
                    var Repair7 = new OapiProcessinstanceCreateRequest.FormComponentValueVoDomain();
                    Repair7.Name = "门店联系人电话";
                    Repair7.Value = "" + kSCustomer.StoreTel;
                    formComponentValues.Add(Repair7);
                    var Repair8 = new OapiProcessinstanceCreateRequest.FormComponentValueVoDomain();
                    Repair8.Name = "门店地址";
                    Repair8.Value = "" + kSCustomer.StoreAddress;
                    formComponentValues.Add(Repair8);
                    var Repair9 = new OapiProcessinstanceCreateRequest.FormComponentValueVoDomain();
                    Repair9.Name = "一芳督导";
                    Repair9.Value = "" + PersonSupervision?.Name;
                    formComponentValues.Add(Repair9);
                    var Repair10 = new OapiProcessinstanceCreateRequest.FormComponentValueVoDomain();
                    Repair10.Name = "详情";
                    Repair10.Value = "http://47.103.125.208:6071/#/detail?id=" + kSCustomer.Id + "&type=" + kSCustomer.StateId + "&state=" + kSCustomer.State + "";
                    formComponentValues.Add(Repair10);
                    var Repair11 = new OapiProcessinstanceCreateRequest.FormComponentValueVoDomain();
                    Repair11.Name = "所属大区";
                    Repair11.Value = RegionName;
                    formComponentValues.Add(Repair11);
                    #endregion
                    break;
                case 3:
                    #region 创建客诉类型为运营反馈的审批
                    var Operate1 = new OapiProcessinstanceCreateRequest.FormComponentValueVoDomain();
                    Operate1.Name = "客诉类型";
                    Operate1.Value = customername;
                    formComponentValues.Add(Operate1);
                    var Operate2 = new OapiProcessinstanceCreateRequest.FormComponentValueVoDomain();
                    Operate2.Name = "客诉内容";
                    Operate2.Value = "" + kSCustomer.Remark;
                    formComponentValues.Add(Operate2);
                    var Operate3 = new OapiProcessinstanceCreateRequest.FormComponentValueVoDomain();
                    Operate3.Name = "客诉单号";
                    Operate3.Value = "" + kSCustomer.Id;
                    formComponentValues.Add(Operate3);
                    var Operate4 = new OapiProcessinstanceCreateRequest.FormComponentValueVoDomain();
                    Operate4.Name = "提交时间";
                    Operate4.Value = "" + kSCustomer.CreateTime;
                    formComponentValues.Add(Operate4);
                    var Operate5 = new OapiProcessinstanceCreateRequest.FormComponentValueVoDomain();
                    Operate5.Name = "异常产品";
                    Operate5.Value = "" + kSCustomer.ProductIdNames;
                    formComponentValues.Add(Operate5);
                    var Operate6 = new OapiProcessinstanceCreateRequest.FormComponentValueVoDomain();
                    Operate6.Name = "联系人";
                    Operate6.Value = "" + kSCustomer.StoreContact;
                    formComponentValues.Add(Operate6);
                    var Operate7 = new OapiProcessinstanceCreateRequest.FormComponentValueVoDomain();
                    Operate7.Name = "联系电话";
                    Operate7.Value = "" + kSCustomer.StoreTel;
                    formComponentValues.Add(Operate7);
                    var Operate8 = new OapiProcessinstanceCreateRequest.FormComponentValueVoDomain();
                    Operate8.Name = "邮箱";
                    Operate8.Value = "" + kSCustomer.StoreEmail;
                    formComponentValues.Add(Operate8);
                    var Operate9 = new OapiProcessinstanceCreateRequest.FormComponentValueVoDomain();
                    Operate9.Name = "客诉门店";
                    Operate9.Value = "" + kSCustomer.Id;
                    formComponentValues.Add(Operate9);
                    var Operate10 = new OapiProcessinstanceCreateRequest.FormComponentValueVoDomain();
                    Operate10.Name = "门店联系人";
                    Operate10.Value = "" + Person?.Name;
                    formComponentValues.Add(Operate10);
                    var Operate11 = new OapiProcessinstanceCreateRequest.FormComponentValueVoDomain();
                    Operate11.Name = "门店联系人电话";
                    Operate11.Value = "" + Person?.PsnMobilePhone;
                    formComponentValues.Add(Operate11);
                    var Operate12 = new OapiProcessinstanceCreateRequest.FormComponentValueVoDomain();
                    Operate12.Name = "详情";
                    Operate12.Value = "http://47.103.125.208:6071/#/detail?id=" + kSCustomer.Id + "&type=" + kSCustomer.StateId + "&state=" + kSCustomer.State + "";
                    formComponentValues.Add(Operate12);
                    var Operate13 = new OapiProcessinstanceCreateRequest.FormComponentValueVoDomain();
                    Operate13.Name = "所属大区";
                    Operate13.Value = RegionName;
                    formComponentValues.Add(Operate13);
                    #endregion
                    break;
                case 4:
                    #region 创建客诉类型为其他反馈的审批
                    var Other1 = new OapiProcessinstanceCreateRequest.FormComponentValueVoDomain();
                    Other1.Name = "客诉类型";
                    Other1.Value = customername;
                    formComponentValues.Add(Other1);
                    var Other2 = new OapiProcessinstanceCreateRequest.FormComponentValueVoDomain();
                    Other2.Name = "客诉内容";
                    Other2.Value = "" + kSCustomer.Remark;
                    formComponentValues.Add(Other2);
                    var Other3 = new OapiProcessinstanceCreateRequest.FormComponentValueVoDomain();
                    Other3.Name = "客诉单号";
                    Other3.Value = "" + kSCustomer.Id;
                    formComponentValues.Add(Other3);
                    var Other4 = new OapiProcessinstanceCreateRequest.FormComponentValueVoDomain();
                    Other4.Name = "提交时间";
                    Other4.Value = "" + kSCustomer.CreateTime;
                    formComponentValues.Add(Other4);
                    var Other5 = new OapiProcessinstanceCreateRequest.FormComponentValueVoDomain();
                    Other5.Name = "联系人";
                    Other5.Value = "" + kSCustomer.StoreContact;
                    formComponentValues.Add(Other5);
                    var Other6 = new OapiProcessinstanceCreateRequest.FormComponentValueVoDomain();
                    Other6.Name = "联系电话";
                    Other6.Value = "" + kSCustomer.StoreTel;
                    formComponentValues.Add(Other6);
                    var Other7 = new OapiProcessinstanceCreateRequest.FormComponentValueVoDomain();
                    Other7.Name = "邮箱";
                    Other7.Value = "" + kSCustomer.StoreEmail;
                    formComponentValues.Add(Other7);
                    var Other8 = new OapiProcessinstanceCreateRequest.FormComponentValueVoDomain();
                    Other8.Name = "一芳督导";
                    Other8.Value = "" + PersonSupervision?.Name;
                    formComponentValues.Add(Other8);
                    var Other9 = new OapiProcessinstanceCreateRequest.FormComponentValueVoDomain();
                    Other9.Name = "客诉门店";
                    Other9.Value = "" + Person?.Id;
                    formComponentValues.Add(Other9);
                    var Other10 = new OapiProcessinstanceCreateRequest.FormComponentValueVoDomain();
                    Other10.Name = "门店联系电话";
                    Other10.Value = "" + Person?.PsnMobilePhone;
                    formComponentValues.Add(Other10);
                    var Other11 = new OapiProcessinstanceCreateRequest.FormComponentValueVoDomain();
                    Other11.Name = "门店联系人";
                    Other11.Value = "" + Person?.Name;
                    formComponentValues.Add(Other11);
                    var Other12 = new OapiProcessinstanceCreateRequest.FormComponentValueVoDomain();
                    Other12.Name = "详情";
                    Other12.Value = "http://47.103.125.208:6071/#/detail?id=" + kSCustomer.Id + "&type=" + kSCustomer.StateId + "&state=" + kSCustomer.State + "";
                    formComponentValues.Add(Other12);
                    var Other13 = new OapiProcessinstanceCreateRequest.FormComponentValueVoDomain();
                    Other13.Name = "所属大区";
                    Other13.Value = RegionName;
                    formComponentValues.Add(Other13);
                    #endregion
                    break;
            }
            //生成图片至钉钉
            //获取当前访问ip信息
            string url = HttpContext.Current.Request.Url.ToString().Replace(HttpContext.Current.Request.Url.PathAndQuery, ""); //服务器协议+域名+端口
            var attachments = DbContext.BA_Attachment.Where(k => k.SourceId == kSCustomer.Id && k.CodeBusinessType == "KS01").Select(k => "\"" + url + k.FileAccess + "\"").ToList();
            if (attachments.Count() > 0)
            {
                formComponentValues.Add(new OapiProcessinstanceCreateRequest.FormComponentValueVoDomain() { Name = "图片", Value = $"[{string.Join(",", attachments)}]" });
            }

            request.FormComponentValues_ = formComponentValues;
            OapiProcessinstanceCreateResponse response = client.Execute(request, Token);
            return response;
        }
        #endregion

        #region 获取审批实列详情
        public static OapiProcessinstanceGetResponse ApprovalDetails(string ProcessId, string Token)
        {
            IDingTalkClient client = new DefaultDingTalkClient("https://oapi.dingtalk.com/topapi/processinstance/get");
            OapiProcessinstanceGetRequest request = new OapiProcessinstanceGetRequest();
            request.ProcessInstanceId = ProcessId;
            OapiProcessinstanceGetResponse response = client.Execute(request, Token);
            return response;
        }
        #endregion

        #region 获取用户详情
        public static OapiUserGetResponse CallbackUserInfo(string UserId, string Token)
        {
            IDingTalkClient client = new DefaultDingTalkClient("https://oapi.dingtalk.com/user/get");
            OapiUserGetRequest request = new OapiUserGetRequest();
            request.Userid = UserId;
            request.SetHttpMethod("GET");
            OapiUserGetResponse response = client.Execute(request, Token);
            return response;
        }
        #endregion

        #region 获取部门详情
        public static OapiDepartmentGetResponse CallbackDepInfo(string DepCode, string Token)
        {
            IDingTalkClient client = new DefaultDingTalkClient("https://oapi.dingtalk.com/department/get");
            OapiDepartmentGetRequest request = new OapiDepartmentGetRequest();
            request.Id = "2";
            request.SetHttpMethod("GET");
            OapiDepartmentGetResponse response = client.Execute(request, Token);
            return response;
        }
        #endregion

    }
    public class LogHelper
    {
        #region 写入日志
        /// <summary>
        /// 写入日志
        /// </summary>
        /// <param name="sLog">内容</param>
        /// <param name="sOption">标题</param>
        /// <param name="sSrc">路径</param>
        public static void WriteLog(string sLog, string sOption = "OP", string sSrc = "/LogData/Logs/")
        {
            var now = DateTime.Now;
            StreamWriter sr = null;

            var filePath = AppDomain.CurrentDomain.BaseDirectory + sSrc; // 文件路径
            var file = filePath + "Log_" + DateTime.Now.ToString("yyyy_MM_dd") + ".log"; // 文件

            if (!Directory.Exists(filePath)) Directory.CreateDirectory(filePath); // 判断是否存在目录,不存在则创建

            if (!File.Exists(file))
            {
                sr = File.CreateText(file);//创建日志文件
            }
            else
            {
                sr = File.AppendText(file);//追加日志文件
            }
            sr.WriteLine($"{DateTime.Now}:【{sOption}】:{sLog}");//日志格式
            if (sr != null) sr.Close();     //关闭文件流
        }
        #endregion
    }
}

using Compression.Services;
using DingTalk.Api;
using DingTalk.Api.Request;
using DingTalk.Api.Response;
using Jusoft.Auxiliary;
using Jusoft.DingTalk.Core;
using Jusoft.YiFang.Api.Help;
using Jusoft.YiFang.Api.Models;
using Jusoft.YiFang.Db;
using Jusoft.YiFang.Db.Extensions;
using Jusoft.YiFang.Db.Models;
using Jusoft.YiFang.Db.ThirdSystem;
using Jusoft.YiFang.Dto.CustomerDetails;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Linq.Expressions;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using X.PagedList;

namespace Jusoft.YiFang.Api.Controllers
{
    [Authorize]
    public class CustomerController : Controller
    {
        YiFang_CustomerComplaintEntities dbContext = new YiFang_CustomerComplaintEntities();

        #region 新增客诉
        [HttpPost]
        public object AddCustomerComplaint(Customer models)
        {
            try
            {
                //获取当前登录者信息
                var IdPerson = dbContext.OR_Person.FirstOrDefault(k => k.LoginName == User.Identity.Name);
                if (IdPerson == null)
                {
                    return this.JsonRespData(1000, "登录者不存在，请重新登录");
                }
               
                KS_Customer customer = new KS_Customer();
                var sub = dbContext.BA_SysEnType.FirstOrDefault(p => p.Id == models.SubclassId);
                if (sub==null)
                {
                    return this.JsonRespData(1000, "该客诉类型不存在，请重新选择");
                }
                //客服id
                //customer.IdPersonSupervision = super.Id;
                customer.Remark = models.Remark;
                customer.IdPerson = IdPerson.Id;
                customer.State = 0;
                customer.StateId = models.SubclassId;
                customer.CreateTime = DateTime.Now;
                customer.StoreName = IdPerson.OR_Department==null?"": IdPerson.OR_Department.Name;
                customer.StoreCode = IdPerson.OR_Department == null ? "" : IdPerson.OR_Department.Code;
                //customer.Number = 0;
                //var info = dbContext.OR_Person.FirstOrDefault(p => p.Id == store.IdPersonRegion);
                //customer.StoreRegionName = info == null ? "" : info.Name;
                dbContext.KS_Customer.Add(customer);
                dbContext.SaveChanges();
                #region 上传附件
                //上传附件
                List<BA_Attachment> attachments = new List<BA_Attachment>();
                if (models.Audios != null)
                {
                    foreach (var item in models.Audios)
                    {
                        //LogHelper.WriteLog("这是Url"+item.Url);
                        if (string.IsNullOrWhiteSpace(item.Url))
                        {
                            return this.JsonRespData(1000, "语音不能为空哦~");
                        }
                        BA_Attachment bA_ = new BA_Attachment
                        {
                            CodeBusinessType = Config.KS02,
                            FileAccess = item.Url,
                            FileSize = item.Duration,
                            SourceId = customer.Id,
                            FileName = item.Remark
                        };
                        dbContext.BA_Attachment.Add(bA_);
                        dbContext.SaveChanges();
                    }
                }
                var httpFile = HttpContext.Request.Files;
                for (int i = 0; i < httpFile.Count; i++)
                {
                    //图片
                    HttpPostedFileBase file = httpFile["FilesImg[" + i + "]"];

                    if (file != null)
                    {
                        string[] Upload = UploadFile.Upload(file, Config.KS01);
                        Guid guid = new Guid(UploadFile.GetMD5HashFromFile(Upload[1]));//文件唯一标识
                        BA_Attachment a_Attachment = new BA_Attachment()
                        {
                            CodeBusinessType = Config.KS01,
                            SourceId = customer.Id,
                            FileAccess = Upload[0],
                            FileName = file.FileName,
                            FileHash = guid

                        };
                        var attachment = dbContext.BA_Attachment.FirstOrDefault(me => me.FileHash == guid);//如果这个图片已经存在
                        if (attachment != null)
                        {
                            System.IO.FileInfo fileInfo = new System.IO.FileInfo(Upload[1]);//通过物理删除
                            fileInfo.Delete();
                            a_Attachment.FileAccess = attachment.FileAccess;
                        }
                        attachments.Add(a_Attachment);
                    }
                   
                    //视频
                    file = httpFile["FilesVideo[" + i + "]"];
                    if (file != null)
                    {
                        string[] Upload = UploadFile.Upload(file, Config.KS03);
                        Guid guid = new Guid(UploadFile.GetMD5HashFromFile(Upload[1]));

                        BA_Attachment a_Attachment = new BA_Attachment()
                        {
                            CodeBusinessType = Config.KS03,
                            SourceId = customer.Id,
                            FileAccess = Upload[0],
                            FileName = file.FileName,
                            FileHash = guid
                        };
                        var attachment = dbContext.BA_Attachment.FirstOrDefault(me => me.FileHash == guid);
                        if (attachment != null)
                        {
                            System.IO.FileInfo fileInfo = new System.IO.FileInfo(Upload[1]);
                            fileInfo.Delete();
                            a_Attachment.FileAccess = attachment.FileAccess;
                        }
                        attachments.Add(a_Attachment);
                    }
                }
                dbContext.BA_Attachment.AddRange(attachments);
                dbContext.SaveChanges();
                var cf = dbContext.KS_Confirm.Where(p => p.TypeId == models.SubclassId).Select(p=>p.IdPerson).ToList();
                if (cf==null)
                {
                    return this.JsonRespData(1000, "推送钉钉消息失败,请联系管理员！");
                }
                var kf= dbContext.OR_Person.Where(p => cf.Contains(p.Id)).Select(p=>p.PsnNum).ToArray();
                SendCreate(string.Join(",",kf), sub.Name, IdPerson.Name, customer.Remark);
                #endregion
                return this.JsonRespData("新增成功");
            }
            catch (Exception ex)
            {
                LogHelper.WriteLog(ex.Message);
                return this.JsonRespData(1000, ex.ToString());
            }
        }
        #endregion
        public void SendCreate(string id,string type,string name,string remark)
        {
            DingTalkMessage dingTalk = new DingTalkMessage();
            long agentId = Allocation.AgentId;
            string token = AccessToken.GetAccessToken();
            Dictionary<string, string> dic = new Dictionary<string, string>();
            dic.Add("标题：", $"您有一个客诉单需受理，请及时受理！");
            dic.Add("问题类型：",type );
            dic.Add("提交人：", name);
            dic.Add("提交时间：", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
            dic.Add("问题描述：", remark);
            dingTalk.OAMessage(agentId, id, token, "", dic);
        }
        #region 客诉详情
        [HttpPost]
        [AllowAnonymous]
        public object CustomerComplaintDeails(decimal Id)
        {
            try
            {
                //根据客诉Id和处理状态获取对应的单据
                var customer = dbContext.KS_Customer.FirstOrDefault(k => k.Id == Id);
                if (customer == null)
                {
                    return this.JsonRespData(1001, "选择单据不存在，请重新刷新界面");
                }

                var test = dbContext.KS_Confirm.Where(p => p.IdPerson == 1);
                //获取当前访问ip信息
                string url = HttpContext.Request.Url.ToString().Replace(HttpContext.Request.Url.PathAndQuery, ""); //服务器协议+域名+端口
                //备注信息特殊处理
                string FinishRemark = null;
                DateTime? EndTime = null;
                if (customer.State != 0 || customer.State != 1)
                {
                    FinishRemark = dbContext.KS_Customer_Approval.OrderByDescending(k => k.Id).FirstOrDefault(k => k.IdCustomer == Id)?.Remark;
                    EndTime = dbContext.KS_Customer_Approval.OrderByDescending(k => k.Id).FirstOrDefault(k => k.IdCustomer == Id)?.FinishTime;
                }

                var xx = dbContext.BA_SysEnType.FirstOrDefault(p => p.Id == customer.StateId);
                var data = new
                {
                    Id,//id
                       //门店信息
                    CreateTime = customer.CreateTime.ToString("yyyy-MM-dd HH:mm:ss"),//创建时间
                    customer.Remark,//备注
                    customer.State,//客诉状态
                    customer.StateId,//客诉类型
                    SubclassName =xx==null?"":xx.Name ,//客诉类型的名称
                    DeliveryDate = customer.DeliveryDate == null ? "" : customer.DeliveryDate.ToString(),
                    customer.Number,
                    customer.Evaluation,
                    EvaluationTime = customer.EvaluationTime == null ? "" : customer.EvaluationTime.ToString(),
                    customer.DealGrade,
                    customer.ResultGrade,
                    StoreName=customer.StoreName??"",
                    Supplementary = customer.KS_Customer_Approval.Select(k => new
                    {
                        k.Id,
                        k.Remark,
                        FinishTime = k.FinishTime.Value.ToString("yyyy-MM-dd HH:mm:ss"),
                        Name = dbContext.OR_Person.FirstOrDefault(s => s.Id == k.IdPerson)==null?"": dbContext.OR_Person.FirstOrDefault(s => s.Id == k.IdPerson).Name,
                        KFId = k.IdPerson,
                        State = k.state,
                        Files = GetKSFileList(k.Id,1)
                    }).OrderBy(p=>p.FinishTime).ToList(),
                    Files = GetKSFileList(Id)
                };
                return this.JsonRespData(data);
            }
            catch (Exception ex)
            {
                LogHelper.WriteLog(ex.ToString());
                return this.JsonRespData(1000, ex.ToString());
            }
        }
        #endregion

      


        #region 评价
        public object Evaluation(decimal Id, string Evaluation, int ResultGrade, int DealGrade)
        {

            try
            {
                //获取当前登录者信息
                var IdPerson = dbContext.OR_Person.FirstOrDefault(k => k.LoginName == User.Identity.Name)?.Id;
                if (IdPerson == null)
                {
                    return this.JsonRespData(1000, "登录者不存在，请重新登录");
                }

                var customer = dbContext.KS_Customer.FirstOrDefault(k => k.Id == Id && k.IdPerson == IdPerson);
                if (customer == null)
                {
                    return this.JsonRespData(1001, "选择单据不存在，请重新刷新界面");
                }
                if (customer.State != (byte)Enumerate.CustomerState.Completed)
                {
                    return this.JsonRespData(1001, "评价失败，只有在已完成的情况才能评价");
                }
                customer.Evaluation = Evaluation;
                customer.ResultGrade = ResultGrade;
                customer.DealGrade = DealGrade;
                customer.EvaluationTime = DateTime.Now;
                //成功评价回写状态
                //if (customer.State == (byte)Enumerate.CustomerState.Completed)
                //{
                //    customer.State = (byte)Enumerate.CustomerState.HasScore;
                //}
                dbContext.SaveChanges();
                return this.JsonRespData("评价成功");

            }
            catch (Exception ex)
            {

                return this.JsonRespData(1000, ex.ToString());
            }
        }
        #endregion


        public object GetKSFileList(decimal Id, int X=0)
        {
            string image = "";
            string audio = "";
            string video = "";
            if (X == 0)
            {
                image = "KS01";
                audio = "KS02";
                video = "KS03";
            }
            else
            {
                image = Config.KS_BC01;
                audio = Config.KS_BC02;
                video = Config.KS_BC03;
            }
            //获取当前访问ip信息
            string url = HttpContext.Request.Url.ToString().Replace(HttpContext.Request.Url.PathAndQuery, ""); //服务器协议+域名+端口
            var filelist = dbContext.BA_Attachment.Where(p => p.SourceId == Id).ToList();
            var Files = new
            {
                FilesImg = filelist.Where(p => p.CodeBusinessType == image).Select(k => new
                {
                    k.Id,
                    k.FileName,
                    FileAccess = url + k.FileAccess
                }),
                FilesAudio = filelist.Where(p => p.CodeBusinessType == audio).Select(k => new
                {
                    k.Id,
                    duration = k.FileSize,
                    url = k.FileAccess,
                    remark=k.FileName==null?"":k.FileName
                }),
                FilesVideo = filelist.Where(p => p.CodeBusinessType == video).Select(k => new
                {
                    k.Id,
                    k.FileName,
                    FileAccess = url + k.FileAccess
                }),
            };
            return Files;
        }

        #region 标准流程
        /// <summary>
        /// 客诉列表
        /// </summary>
        /// <param name="State"></param>
        /// <param name="PageSize"></param>
        /// <param name="PageIndex"></param>
        /// <returns></returns>
        [HttpPost]
        public object NewCustomerComplaintList(decimal? State, int PageSize, int PageIndex, DateTime? startDate, DateTime? endDate,int? number)
        {
            try
            {
                //获取当前登录者信息
                var IdPerson = dbContext.OR_Person.FirstOrDefault(k => k.LoginName == User.Identity.Name)?.Id;
                if (IdPerson == null)
                {
                    return this.JsonRespData(1000, "登录者不存在，请重新登录");
                }
                var dbdata = dbContext.KS_Customer.Where(k => k.IdPerson == IdPerson).ToList();
                
                if (startDate.HasValue)
                {
                    dbdata = dbdata.Where(p => p.CreateTime >= startDate && p.CreateTime <= endDate.Value.AddDays(1)).ToList();
                }
                //if (number!=0)
                //{
                //    dbdata = dbdata.Where(p => p.Number!=0).ToList();
                //}
                if (State.HasValue)
                {
                    dbdata = dbdata.Where(k => k.State == State).ToList();
                }
                //是否有下一页
                bool hasNext = false;
                //数据总条数
                int count = dbdata.Count();
                //总数/页数
                int page = count / PageSize;

                if (count % PageSize == 0 || (count - (PageSize * PageIndex)) <= 0)
                {
                    hasNext = false;
                }
                else
                {
                    hasNext = true;
                }

                if ((count - (PageSize * PageIndex)) < PageIndex)
                {
                    page = page + 1;
                }
                string url = HttpContext.Request.Url.ToString().Replace(HttpContext.Request.Url.PathAndQuery, ""); //服务器协议+域名+端口
                string path = "/Store/head.png";
                string fullpath = url + path;
                var orpersonlist = dbContext.OR_Person.AsQueryable();
                var tname = dbContext.BA_SysEnType.ToList(); ;
                var data = dbdata.Select(k =>
                 new
                {
                    k.Id,
                    k.StoreName,
                    k.StoreCode,
                    k.StateId,
                    SubclassName= tname.FirstOrDefault(fd=>fd.Id==k.StateId)==null?"": tname.FirstOrDefault(fd => fd.Id == k.StateId).Name,
                    k.Remark,
                    k.CreateTime,
                    k.State,
                    Nmae= orpersonlist.FirstOrDefault(p=>p.Id==k.IdPerson)==null?"": orpersonlist.FirstOrDefault(p => p.Id == k.IdPerson).Name,
                    HeadUrl= orpersonlist.FirstOrDefault(p => p.Id == k.IdPerson) == null ? fullpath : orpersonlist.FirstOrDefault(p => p.Id == k.IdPerson).HeadUrl,
                    Attachment =GetKSFileList(k.Id),
                    k.Number,//审批状态
                    DeliveryDate= k.DeliveryDate==null?"":k.DeliveryDate.ToString()//审批完成时间
                }).OrderByDescending(k => k.CreateTime).Skip((PageIndex - 1) * PageSize).Take(PageSize).ToList(); ;
                PageDataResult model = new PageDataResult();
                model.ListData = data;
                model.PageIndex = PageIndex;
                model.PageCount = page;
                model.TotalItemCount = count;
                model.HasNextPage = hasNext;
                return this.JsonRespData(model);
            }
            catch (Exception ex)
            {
                LogHelper.WriteLog(ex.Message);
                return this.JsonRespData(1000, ex.ToString());
            }
        }
        [HttpPost]
        public object CustServiceList(CustomerCondition condition)
        {
            //当前人
            var Person = dbContext.OR_Person.FirstOrDefault(p => p.LoginName == User.Identity.Name);
            if (!dbContext.OR_Person.IsExistencePerson(User.Identity.Name))
            {
                return this.JsonRespData(1000, "登录者不存在，请重新登录");
            }
            //var dbdata = dbContext.v_Jusoft_CustomerList.Where(k => k.IdPersonSupervision == Person.Id);
            Expression<Func<v_Kf_DataList, bool>> where = PredicateExtensions.True<v_Kf_DataList>();
            
            if (condition.StartTime.HasValue)//时间
            {
                where = where.And(p => p.CreateTime > condition.StartTime && p.CreateTime <= condition.EndTime);
            }
            if (condition.CustomerType.HasValue)//客诉类型
            {
                where = where.And(p => p.StateId == condition.CustomerType);
            }
            //if (condition.OwnedStores != null)//所属门店
            //{
            //    where = where.And(p => p.IdStore == condition.OwnedStores);
            //}
            //if (!string.IsNullOrWhiteSpace(condition.StoreName))
            //{
            //    where = where.And(p => p.StoreName.Contains(condition.StoreName));
            //}
            if (condition.Number.HasValue)
            {
                where = where.And(p => p.Number==condition.Number);
            }
            Expression<Func<v_Kf_DataList, bool>> where2 = null;
            if (!string.IsNullOrWhiteSpace(condition.SearchKey))
            {
                var ss = int.TryParse(condition.SearchKey, out int aa);
                if (ss)
                {
                    //decimal ksId = Convert.ToDecimal(condition.SearchKey);
                    where = where.And(p => p.KS_ID == aa);//
                    //where = where.And(p => p.Code == aa.ToString());
                }
                else
                {
                    where2 = p => (string.IsNullOrEmpty(condition.SearchKey)  || p.Name.Contains(condition.SearchKey)
                || p.ManagerName.Contains(condition.SearchKey) || p.EnTypeName.Contains(condition.SearchKey) );
                }
            }
            var confirm = dbContext.KS_Confirm.Where(p => p.IdPerson == Person.Id).Select(p => p.TypeId).Distinct().ToList();
            var personList = dbContext.KS_Confirm.Where(p => confirm.Contains(p.TypeId)).Select(p => p.IdPerson).Distinct().ToList();
            /*where = where.And(p => p.IdPersonSupervision == Person.Id || personList.Contains(p.IdPersonSupervision));*///督导Id 为当前用户
            where = where.And(p => p.ManagerId == Person.Id);
            var data = dbContext.v_Kf_DataList.Where(where);
            //var list2= dbContext.v_Jusoft_CustomerList.Where(p => p.IdPersonSupervision == Person.Id).ToList();

            var Untreated = data.Where(k => k.State == 0).Count();//未处理的数量
            var Processing = data.Where(k => k.State == 1).Count();//同意的数量
            var Endover = data.Where(k => k.State == 2).Count();//驳回的数量
            var AllCount = data.Where(k => true).Count();//所有的数量
            if (condition.State.HasValue)//客诉状态
            {
                data = data.Where(k => condition.State==k.State);
            }
            //if (!condition.Number.HasValue)
            //{
            //    data = data.Where(k => condition.State==k.State);
            //}
            List<v_Kf_DataList> list = new List<v_Kf_DataList>();
            if (where2!=null)
            {
                list = data.Where(where2).ToList();
            }
            else
            {
                list = data.ToList();
            }
            string url = HttpContext.Request.Url.ToString().Replace(HttpContext.Request.Url.PathAndQuery, ""); //服务器协议+域名+端口
            string path = "/Store/head.png";
            string fullpath = url + path;
            var CusDatas = list.Select(s => new {
                Person = s.Name,//提交人名称
                PersonHead = s.HeadUrl ?? fullpath,
                //s.Code,
                //s.Name,
                s.KS_ID,//客诉id
                s.StateId,//客诉类型id
                CreateTime = s.CreateTime.ToString("yyyy-MM-dd HH:mm:ss"),//创建时间
                s.State,//状态id 0未处理，1完成，2驳回
                s.Remark,//备注
                s.Number,
                s.StoreName,
                s.StoreCode,
                DeliveryDate = s.DeliveryDate == null ? "" : s.DeliveryDate.ToString(),//审批完成时间
                //s.SubclassId,//客诉小类id
                SubclassName = s.EnTypeName,
                //Abnormal = dbContext.BA_SysEnType.FirstOrDefault(p => p.Id == s.AbnormalId)?.Name,//产品分类Id，Name
                PersonName = s.ManagerName,//督导人名称
                /*HeadUrl = s.HeadUrl,*///投降
                FinishTime = s.FinishTime == null ? "" : s.FinishTime.ToString(),//完成时间
                Attachment = GetKSFileList(s.KS_ID)
            }).Distinct().OrderByDescending(p => p.CreateTime).ToPagedList(condition.PageIndex, condition.PageSize);
            //返回类型
            DdTypeCount Pdrs = new DdTypeCount();
            Pdrs.ListData = CusDatas;//返回数据体
            Pdrs.PageCount = CusDatas.PageCount;//数据总数
            Pdrs.PageIndex = condition.PageIndex;//数据页数
            Pdrs.HasNextPage = CusDatas.HasNextPage;//是否有下一页
            Pdrs.TotalItemCount = CusDatas.TotalItemCount;//数据总数
            Pdrs.Untreated = Untreated;//未处理的数量
            Pdrs.Processing = Processing;//同意的数量
            Pdrs.Endover = Endover;//驳回的数量
            Pdrs.AllCount = AllCount;//所有的数量
            return this.JsonRespData(Pdrs);
        }

        [HttpPost]
        public object SunCustomerDetails(decimal Id)
        {
            try
            {
                var KsCustomer = dbContext.KS_Customer.FirstOrDefault(p => p.Id == Id);
                if (KsCustomer != null)
                {
                    //客诉门店
                    var Person = dbContext.OR_Person.FirstOrDefault(p => p.Id == KsCustomer.IdPerson);
                    //一方督导
                    //var PersonSupervision = dbContext.OR_Person.FirstOrDefault(p => p.Id == KsCustomer.IdPersonSupervision);
                    //异常归类
                    //var Abnormal = dbContext.BA_SysEnType.FirstOrDefault(p => p.Id == KsCustomer.AbnormalId)?.Name;
                    //查询审批流程
                    // 获取当前访问ip信息
                    string url = HttpContext.Request.Url.ToString().Replace(HttpContext.Request.Url.PathAndQuery, ""); //服务器协议+域名+端口

                    var info = dbContext.v_Kf_DataList.Where(p => p.KS_ID == Id).FirstOrDefault();
                    ST_Store store = null;
                    if (info != null)
                    {
                        store = dbContext.ST_Store.FirstOrDefault(p => p.Id == info.KS_ID);
                    }
                    var tname = dbContext.BA_SysEnType.FirstOrDefault(p => p.Id == KsCustomer.StateId);
                    var Repair = new
                    {
                        Attachment = GetKSFileList(Id),
                        Remark = KsCustomer.Remark,//备注
                        Id = KsCustomer.Id,//客诉Id
                        SubclassName = tname == null ? "" : tname.Name,//客诉小类名称
                        KsCustomer.CreateTime,//创建时间,
                        //反馈类型
                        //FeedbackName = dbContext.BA_SysEnType.FirstOrDefault(p => p.Id == KsCustomer.RepairTypeId)?.Name,
                        //报修品项
                        //RepairsName = KsCustomer.BA_SysEnType1?.Name,
                        StoreContact = Person == null ? "" : Person.Name,//联系人
                        StoreTel = Person.PsnMobilePhone == null ? "" : Person.PsnMobilePhone,//联系电话
                        //StoreAddress = store == null ? "" : store.Address,//联系地址
                        PersonSupervisionName = info.ManagerName == null ? "" : info.ManagerName,//督导人名称
                        //门店信息
                        StoreCode = KsCustomer.StoreCode??"",
                        StoreName = KsCustomer.StoreName??"",//门店名称
                        //客诉审批状态
                        Number = KsCustomer.Number,
                        DeliveryDate = KsCustomer.DeliveryDate == null ? "" : KsCustomer.DeliveryDate.ToString(),//审批时间
                        State = KsCustomer.State,
                        KsCustomer.Evaluation,      //评价内容
                        KsCustomer.ResultGrade,//结果评分
                        KsCustomer.DealGrade,       //时效评价
                        EvaluationTime=KsCustomer.EvaluationTime==null?"":KsCustomer.EvaluationTime.ToString(),//评价时间
                                                  //PersonId = dbContext.ST_Store.FirstOrDefault(p=>p.)?.Name,//门店名称
                        CustomerApproval = CustomerApproval(KsCustomer.Id),//处理情况
                                                                           //ReplenishBit = KsCustomer.ReplenishBit,//是否可补充信息
                    };
                    return this.JsonRespData(Repair);
                }
                else
                {
                    return this.JsonRespData(1001, "未找到选择的客诉单据");
                }
            }
            catch (Exception ex)
            {
                return this.JsonRespData(1000, ex.Message);
            }
        }


        [HttpPost]
        public object AgreeOrReject(UpdateCustmoer model)
        {
            try
            {
                //当前人
                var Person = dbContext.OR_Person.FirstOrDefault(p => p.LoginName == User.Identity.Name);
                if (!dbContext.OR_Person.IsExistencePerson(User.Identity.Name))
                {
                    return this.JsonRespData(1000, "登录者不存在，请重新登录");
                }
                //获取对应客诉单
                var info = dbContext.KS_Customer.FirstOrDefault(p => p.Id == model.Id);
                if (info == null)
                {
                    return this.JsonRespData(1000, "客诉单已不存在");
                }
                //审批提交人=客诉单提交人
                var pp = dbContext.OR_Person.FirstOrDefault(p => p.Id == info.IdPerson);
                KS_Customer_Approval approval = new KS_Customer_Approval();
                if (model.IsAgree == 1)
                {
                    info.State = 1;//改变客诉状态
                    approval = new KS_Customer_Approval()
                    {
                        FinishTime = DateTime.Now,
                        IdCustomer = info.Id,
                        IdPerson = Person.Id,
                        Remark = model.Remark,
                        Name = Person.Name,
                        state = 1
                    };
                    info.Number = 4;
                    //附件
                    var fujian = dbContext.BA_Attachment.Where(p => p.CodeBusinessType == "KS01" && p.SourceId == info.Id);
                    Dictionary<string, List<string>> image = new Dictionary<string, List<string>>();//照片
                    List<string> src = new List<string>();//照片路径
                    var url = this.Request.Url.OriginalString.Replace(this.Request.Url.PathAndQuery, "");
                    //类型名称
                    var typeName = dbContext.BA_SysEnType.FirstOrDefault(p => p.Id == info.StateId);
                    foreach (var item in fujian)
                    {
                        src.Add("\"" + url + item.FileAccess + "\"");
                        LogHelper.WriteLog(url + item.FileAccess);
                    }
                    image.Add("图片", src);
                    foreach (var item in image)
                    {
                        LogHelper.WriteLog($"[{string.Join(",", item.Value)}]");
                    }
                    string shen = ShenPi(Convert.ToInt32(Person.CodeDepartment), pp.LoginName, typeName == null ? "暂无" : typeName.Name, info.Remark, image,model.Id);
                    if (!string.IsNullOrWhiteSpace(shen))
                    {
                        info.DingTalkApproval = shen;
                        info.IdPersonApproval = Person.Id;
                    }
                }
                else
                {
                    info.State = 2;//改变客诉状态
                    approval = new KS_Customer_Approval()
                    {
                        FinishTime = DateTime.Now,
                        IdCustomer = info.Id,
                        IdPerson = Person.Id,
                        Remark = model.Remark,
                        Name = Person.Name,
                        state = 2
                    };
                    info.Number = 3;
                }
                info.FinishTime = DateTime.Now;
                dbContext.KS_Customer_Approval.Add(approval);
                dbContext.SaveChanges();


                #region 上传附件
                //上传附件
                List<BA_Attachment> attachments = new List<BA_Attachment>();
                if (model.Audios != null)
                {
                    foreach (var item in model.Audios)
                    {
                        if (string.IsNullOrWhiteSpace(item.Url))
                        {
                            return this.JsonRespData(1000, "语音不能为空哦~");
                        }
                        BA_Attachment bA_ = new BA_Attachment
                        {
                            CodeBusinessType =Config.KS_BC02,
                            FileAccess = item.Url,
                            FileSize = item.Duration,
                            SourceId = approval.Id,
                            FileName = item.Remark
                        };
                        dbContext.BA_Attachment.Add(bA_);
                        dbContext.SaveChanges();
                    }
                }
                var httpFile = HttpContext.Request.Files;
                for (int i = 0; i < httpFile.Count; i++)
                {
                    //图片
                    HttpPostedFileBase file = httpFile["FilesImg[" + i + "]"];
                    if (file != null)
                    {
                        string[] Upload = UploadFile.Upload(file, Config.KS01);
                        Guid guid = new Guid(UploadFile.GetMD5HashFromFile(Upload[1]));

                        BA_Attachment a_Attachment = new BA_Attachment()
                        {
                            CodeBusinessType = Config.KS_BC01,
                            SourceId = approval.Id,
                            FileAccess = Upload[0],
                            FileName = file.FileName,
                            FileHash = guid
                        };
                        var attachment = dbContext.BA_Attachment.FirstOrDefault(me => me.FileHash == guid);
                        if (attachment != null)
                        {
                            System.IO.FileInfo fileInfo = new System.IO.FileInfo(Upload[1]);
                            fileInfo.Delete();
                            a_Attachment.FileAccess = attachment.FileAccess;
                        }
                        attachments.Add(a_Attachment);

                    }
                    //视频
                    file = httpFile["FilesVideo[" + i + "]"];
                    if (file != null)
                    {
                        string[] Upload = UploadFile.Upload(file, Config.KS03);
                        Guid guid = new Guid(UploadFile.GetMD5HashFromFile(Upload[1]));

                        BA_Attachment a_Attachment = new BA_Attachment()
                        {
                            CodeBusinessType = Config.KS_BC03,
                            SourceId = approval.Id,
                            FileAccess = Upload[0],
                            FileName = file.FileName,
                            FileHash = guid
                        };
                        var attachment = dbContext.BA_Attachment.FirstOrDefault(me => me.FileHash == guid);
                        if (attachment != null)
                        {
                            System.IO.FileInfo fileInfo = new System.IO.FileInfo(Upload[1]);
                            fileInfo.Delete();
                            a_Attachment.FileAccess = attachment.FileAccess;
                        }
                        attachments.Add(a_Attachment);
                    }
                }
                dbContext.BA_Attachment.AddRange(attachments);
                dbContext.SaveChanges();
                #endregion
                Send(model.IsAgree, pp.LoginName, pp.Name);
                return this.JsonRespData("操作成功");
            }
            catch (Exception ex)
            {
                return this.JsonRespData(1000, ex.Message);
            }
        }
        [AllowAnonymous]
        [HttpPost]
        public object KSEvaluation(decimal Id, string Evaluation,int ResultGrade,int DealGrade )
        {
            var info = dbContext.KS_Customer.FirstOrDefault(p => p.Id == Id);
            if (info==null)
            {
                return this.JsonRespData("该信息已不存在，请刷新！");
            }
            else
            {
                info.Evaluation = Evaluation;      //评价内容
                info.ResultGrade = ResultGrade;  //结果评分
                info.DealGrade = DealGrade;       //时效评价
                info.EvaluationTime = DateTime.Now;//评价时间
                dbContext.SaveChanges();
                return this.JsonRespData(0, "评价成功");
            }
        }
        public static void Send(int agree,string id, string name)
        {
            string str = agree == 1 ? "受理" : "驳回";
            DingTalkMessage dingTalk = new DingTalkMessage();
            long agentId = Allocation.AgentId;
            string token = AccessToken.GetAccessToken();
            Dictionary<string, string> dic = new Dictionary<string, string>();
            dic.Add("标题：", $"您的客诉单已{str},请知晓！");
            dic.Add($"客服：", name);
            dic.Add("处理时间：", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
            dingTalk.OAMessage(agentId, id, token, "", dic);
        }
        public string ShenPi(long dCode, string usrId, string name, string desc, Dictionary<string, List<string>> image,decimal id)
        {
            DingTalkApproval approval = new DingTalkApproval();
            long agentId = Allocation.AgentId;
            string processCode = Allocation.ProcessCode;
            long depId = dCode;
            Dictionary<string, string> dic = new Dictionary<string, string>();
            dic.Add("客诉类型", name);
            dic.Add("问题描述", desc);
            dic.Add("详情", $"http://47.103.68.172:6078/#/store/view-demo?id={id}");//http://47.103.68.172:6076/  http://121.199.49.237:12705/
            //dic.Add("提交时间", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
            string token = AccessToken.GetAccessToken();
            string shenpi = approval.Initiate2(agentId, processCode, dCode,usrId,token , dic, image,out string str);
            LogHelper.WriteLog("审批id"+ str);
            return shenpi;
        }

        public class GetSP
        {
            public string ErrCode { get; set; }
            public string ProcessInstanceId { get; set; }
        }
        private object CustomerApproval(decimal id)
        {
            
            var info = dbContext.KS_Customer_Approval.FirstOrDefault(p => p.IdCustomer == id&&(p.state == 1||p.state==2));
            if (info==null)
            {
                return null;
            }
            var list = GetKSFileList(info.Id,1);
            var obj = new
            {
                info.Id,
                info.state,
                PersonName = dbContext.OR_Person.FirstOrDefault(p => p.Id == info.IdPerson).Name,
                DepName = dbContext.OR_Department
                   .FirstOrDefault(p => p.Code ==
                   dbContext.OR_Person.FirstOrDefault(k => k.Id == info.IdPerson).CodeDepartment).Name,
                info.FinishTime,
                Remark=info.Remark==null?"":info.Remark,
                Files = list
            };
            return obj;
        }

        #endregion


        [HttpPost]
        public object CustServiceList2(CustomerCondition condition)
        {
            //当前人
            var Person = dbContext.OR_Person.FirstOrDefault(p => p.LoginName == User.Identity.Name);
            if (!dbContext.OR_Person.IsExistencePerson(User.Identity.Name))
            {
                return this.JsonRespData(1000, "登录者不存在，请重新登录");
            }

            string sql = "SELECT b.Name AS EnTypeName, a.Id, a.StateId,a.SubclassId,a.ProductIds, a.ProductIdNames, a.Number,a.UnitId, a.AbnormalId, a.DeliveryDate, a.ProductionBatch, a.OrderNumber,a.RepairTypeId, a.RepairProductId, a.StoreContact, a.StoreAddress,a.StoreTel,a.StoreEmail,a.Remark,a.IdPersonSupervision, a.PersonSupervision,a.IdPerson, a.StoreCode,a.StoreName,a.StoreRegionName,a.CreateTime, a.FinishTime,a.Evaluation,a.ResultGrade,a.DealGrade,a.State,a.ReplenishBit,a.DingTalkApproval, a.IdPersonApproval,a.ProductName, a.EvaluationTime, b.Id, b.IdSysEnType, b.IdParent,b.Name, b.IsSys, b.IsEnd,b.Level, b.RefSource,b.Memo FROM dbo.KS_Customer a LEFT JOIN dbo.BA_SysEnType b ON b.Id = a.SubclassId";
            //var dbdata = dbContext.v_Jusoft_CustomerList.Where(k => k.IdPersonSupervision == Person.Id);
            var all = dbContext.Database.SqlQuery<CustomerDto>(sql).AsQueryable();
            Expression<Func<CustomerDto, bool>> where = PredicateExtensions.True<CustomerDto>();

            if (condition.StartTime.HasValue)//时间
            {
                where = where.And(p => p.CreateTime > condition.StartTime && p.CreateTime <= condition.EndTime);
            }
            if (condition.CustomerType.HasValue)//客诉类型
            {
                where = where.And(p => p.StateId == condition.CustomerType);
            }
            //if (condition.OwnedStores != null)//所属门店
            //{
            //    where = where.And(p => p.IdStore == condition.OwnedStores);
            //}
            //if (!string.IsNullOrWhiteSpace(condition.StoreName))
            //{
            //    where = where.And(p => p.StoreName.Contains(condition.StoreName));
            //}
            if (condition.Number.HasValue)
            {
                where = where.And(p => p.Number == condition.Number);
            }
            Expression<Func<CustomerDto, bool>> where2 = null;
            if (!string.IsNullOrWhiteSpace(condition.SearchKey))
            {
                var ss = int.TryParse(condition.SearchKey, out int aa);
                if (ss)
                {
                    //decimal ksId = Convert.ToDecimal(condition.SearchKey);
                    where = where.And(p => p.Id == aa);//
                    //where = where.And(p => p.Code == aa.ToString());
                }
                else
                {
                    where2 = p => (string.IsNullOrEmpty(condition.SearchKey) ||p.EnTypeName.Contains(condition.SearchKey));
                }
            }
            //var confirm = dbContext.KS_Confirm.Where(p => p.IdPerson == Person.Id).Select(p => p.TypeId).Distinct().ToList();
            //var personList = dbContext.KS_Confirm.Where(p => confirm.Contains(p.TypeId)).Select(p => p.IdPerson).Distinct().ToList();
            ///*where = where.And(p => p.IdPersonSupervision == Person.Id || personList.Contains(p.IdPersonSupervision));*///督导Id 为当前用户
            //where = where.And(p => p.ManagerId == Person.Id);
            var data = all.Where(where);
            //var list2= dbContext.v_Jusoft_CustomerList.Where(p => p.IdPersonSupervision == Person.Id).ToList();

            var Untreated = data.Where(k => k.State == 0).Count();//未处理的数量
            var Processing = data.Where(k => k.State == 1).Count();//同意的数量
            var Endover = data.Where(k => k.State == 2).Count();//驳回的数量
            var AllCount = data.Where(k => true).Count();//所有的数量
            if (condition.State.HasValue)//客诉状态
            {
                data = data.Where(k => condition.State == k.State);
            }
            //if (!condition.Number.HasValue)
            //{
            //    data = data.Where(k => condition.State==k.State);
            //}
            List<CustomerDto> list = new List<CustomerDto>();
            if (where2 != null)
            {
                list = data.Where(where2).ToList();
            }
            else
            {
                list = data.ToList();
            }
            string url = HttpContext.Request.Url.ToString().Replace(HttpContext.Request.Url.PathAndQuery, ""); //服务器协议+域名+端口
            string path = "/Store/head.png";
            string fullpath = url + path;
            var CusDatas = list.Select(s => new {
                //Person = s.Name,//提交人名称
                //PersonHead = s.HeadUrl ?? fullpath,
                //s.Name,
                KS_ID=s.Id ,//客诉id
                s.StateId,//客诉类型id
                CreateTime = s.CreateTime.ToString("yyyy-MM-dd HH:mm:ss"),//创建时间
                s.State,//状态id 0未处理，1完成，2驳回
                s.Remark,//备注
                s.StoreName,
                s.StoreCode,
                //s.SubclassId,//客诉小类id
                SubclassName = s.EnTypeName,
                s.Number,
                DeliveryDate = s.DeliveryDate == null ? "" : s.DeliveryDate.ToString(),//审批完成时间
                //Abnormal = dbContext.BA_SysEnType.FirstOrDefault(p => p.Id == s.AbnormalId)?.Name,//产品分类Id，Name
                PersonName = Manage(s.Id),//督导人名称
                //HeadUrl = s.HeadUrl,//投降
                FinishTime = s.FinishTime == null ? "" : s.FinishTime.ToString(),//完成时间
                Attachment = GetKSFileList(s.Id)
            }).DistinctBy(p=>p.KS_ID).OrderByDescending(p => p.CreateTime).ToPagedList(condition.PageIndex, condition.PageSize);
            //返回类型
            DdTypeCount Pdrs = new DdTypeCount();
            Pdrs.ListData = CusDatas;//返回数据体
            Pdrs.PageCount = CusDatas.PageCount;//数据总数
            Pdrs.PageIndex = condition.PageIndex;//数据页数
            Pdrs.HasNextPage = CusDatas.HasNextPage;//是否有下一页
            Pdrs.TotalItemCount = CusDatas.TotalItemCount;//数据总数
            Pdrs.Untreated = Untreated;//未处理的数量
            Pdrs.Processing = Processing;//同意的数量
            Pdrs.Endover = Endover;//驳回的数量
            Pdrs.AllCount = AllCount;//所有的数量
            return this.JsonRespData(Pdrs);
        }

        public string Manage(decimal Id)
        {
            var list = dbContext.KS_Customer_Approval.Where(p => p.IdCustomer == Id).Select(p => p.IdPerson).ToArray() ;
            var xx= dbContext.OR_Person.Where(p => list.Contains(p.Id)).Select(p => p.Name).ToArray();
            return string.Join(",", xx);
        }
        #region 备份
        /// <summary>
        /// 同意或驳回
        /// </summary>
        /// <param name="Id"></param>
        /// <param name="agree">1 agree,2 reject</param>
        /// <returns></returns>
        //[HttpPost]
        //public object AgreeOrReject(UpdateCustmoer model)
        //{
        //    try
        //    {

        //        //当前人
        //        var Person = dbContext.OR_Person.FirstOrDefault(p => p.LoginName == User.Identity.Name);
        //        if (!dbContext.OR_Person.IsExistencePerson(User.Identity.Name))
        //        {
        //            return this.JsonRespData(1000, "登录者不存在，请重新登录");
        //        }
        //        //获取对应客诉单
        //        var info = dbContext.KS_Customer.FirstOrDefault(p => p.Id == model.Id);
        //        if (info == null)
        //        {
        //            return this.JsonRespData(1000, "客诉单已不存在");
        //        }
        //        KS_Customer_Approval approval = new KS_Customer_Approval();
        //        if (model.IsAgree == 1)
        //        {
        //            info.State = 1;//改变客诉状态
        //            approval = new KS_Customer_Approval()
        //            {
        //                FinishTime = DateTime.Now,
        //                IdCustomer = info.Id,
        //                IdPerson = Person.Id,
        //                Remark = model.Remark,
        //                Name = Person.Name,
        //                state = 1
        //            };
        //            //附件
        //            var fujian= dbContext.BA_Attachment.Where(p => p.CodeBusinessType == "KS01" && p.SourceId == info.Id);
        //            Dictionary<string, List<string>> image = new Dictionary<string, List<string>>();//照片
        //            List<string> src = new List<string>();//照片路径
        //            var url= this.Request.Url.OriginalString.Replace(this.Request.Url.PathAndQuery, "");
        //            //类型名称
        //            var typeName= dbContext.BA_SysEnType.FirstOrDefault(p => p.Id == info.StateId);
        //            foreach (var item in fujian)
        //            {
        //                src.Add(url + item.FileAccess);
        //            }
        //            image.Add("图片", src);
        //            string shen = ShenPi(Convert.ToInt32(Person.CodeDepartment), Person.LoginName,typeName==null?"暂无":typeName.Name,info.Remark, image);
        //            if (!string.IsNullOrWhiteSpace(shen))
        //            {
        //                info.DingTalkApproval = shen;
        //                info.IdPersonApproval = Person.Id;
        //            }
        //        }
        //        else
        //        {
        //            info.State = 2;//改变客诉状态
        //            approval = new KS_Customer_Approval()
        //            {
        //                FinishTime = DateTime.Now,
        //                IdCustomer = info.Id,
        //                IdPerson = Person.Id,
        //                Remark = model.Remark,
        //                Name = Person.Name,
        //                state = 2
        //            };
        //        }
        //        info.FinishTime = DateTime.Now;
        //        dbContext.KS_Customer_Approval.Add(approval);
        //        dbContext.SaveChanges();

        //        #region 上传附件
        //        //上传附件
        //        List<BA_Attachment> attachments = new List<BA_Attachment>();
        //        if (model.Audios != null)
        //        {
        //            foreach (var item in model.Audios)
        //            {
        //                BA_Attachment bA_ = new BA_Attachment
        //                {
        //                    CodeBusinessType = Config.KS02,
        //                    FileAccess = item.Url,
        //                    FileSize = item.Duration,
        //                    SourceId = approval.Id
        //                };
        //                dbContext.BA_Attachment.Add(bA_);
        //                dbContext.SaveChanges();
        //            }
        //        }
        //        var httpFile = HttpContext.Request.Files;
        //        for (int i = 0; i < httpFile.Count; i++)
        //        {
        //            //图片
        //            HttpPostedFileBase file = httpFile["FilesImg[" + i + "]"];
        //            if (file != null)
        //            {
        //                string[] Upload = UploadFile.Upload(file, Config.KS01);
        //                Guid guid = new Guid(UploadFile.GetMD5HashFromFile(Upload[1]));

        //                BA_Attachment a_Attachment = new BA_Attachment()
        //                {
        //                    CodeBusinessType = Config.KS01,
        //                    SourceId = approval.Id,
        //                    FileAccess = Upload[0],
        //                    FileName = file.FileName,
        //                    FileHash = guid
        //                };
        //                var attachment = dbContext.BA_Attachment.FirstOrDefault(me => me.FileHash == guid);
        //                if (attachment != null)
        //                {
        //                    System.IO.FileInfo fileInfo = new System.IO.FileInfo(Upload[1]);
        //                    fileInfo.Delete();
        //                    a_Attachment.FileAccess = attachment.FileAccess;
        //                }
        //                attachments.Add(a_Attachment);

        //            }
        //            //视频
        //            file = httpFile["FilesVideo[" + i + "]"];
        //            if (file != null)
        //            {
        //                string[] Upload = UploadFile.Upload(file, Config.KS03);
        //                Guid guid = new Guid(UploadFile.GetMD5HashFromFile(Upload[1]));

        //                BA_Attachment a_Attachment = new BA_Attachment()
        //                {
        //                    CodeBusinessType = Config.KS03,
        //                    SourceId = approval.Id,
        //                    FileAccess = Upload[0],
        //                    FileName = file.FileName,
        //                    FileHash = guid
        //                };
        //                var attachment = dbContext.BA_Attachment.FirstOrDefault(me => me.FileHash == guid);
        //                if (attachment != null)
        //                {
        //                    System.IO.FileInfo fileInfo = new System.IO.FileInfo(Upload[1]);
        //                    fileInfo.Delete();
        //                    a_Attachment.FileAccess = attachment.FileAccess;
        //                }
        //                attachments.Add(a_Attachment);
        //            }
        //        }
        //        dbContext.BA_Attachment.AddRange(attachments);
        //        dbContext.SaveChanges();
        //        Send(model.IsAgree, Person.LoginName,Person.Name);

        //        #endregion
        //        return this.JsonRespData("新增成功");
        //    }
        //    catch (Exception ex)
        //    {
        //        return this.JsonRespData(1000, ex.Message);
        //    }
        //}
        #endregion
    }


}
using Demo.Models;
using Jusoft.Auxiliary;
using Jusoft.DingTalk.Core;
using Jusoft.YiFang.Api.Models;
using Jusoft.YiFang.Db;
using Jusoft.YiFang.Db.Extensions;
using Jusoft.YiFang.Db.Models;
using Jusoft.YiFang.Db.ThirdSystem;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Web;
using System.Web.Mvc;
using X.PagedList;

namespace Jusoft.YiFang.Api.Controllers
{

    [Authorize]
    public class DdCustomerController : Controller
    {
        YiFang_CustomerComplaintEntities dbContext = new YiFang_CustomerComplaintEntities();

        #region 钉钉客诉列表
        /// <summary>
        /// 钉钉客诉列表
        /// </summary>
        /// <param name="Type">查案类型</param>
        /// <param name="PageIndex">当前页</param>
        /// <param name="PageSize">每页显示的条数</param>
        /// <returns></returns>
        [HttpPost]
        public object CustomerComplaintList(DdCustomerListModel model)
        {
            try
            {
                //获取当前登录者信息
                var Person = dbContext.OR_Person.FirstOrDefault(p=>p.LoginName== User.Identity.Name);
                if (Person == null)
                {
                    return this.JsonRespData(1000, "登录者不存在，请重新登录");
                }
                //校验审批人可查看单据（下一个审批人员-审批记录人员）
               var dbdata = dbContext.v_Jusoft_CustomerList.Where(k=>k.IdPersonApproval== Person.Id|| k.ApprovalIdPerson== Person.Id||k.ConfirmIdPerson==Person.Id);

               var Customerslist = dbdata.Select(s => new
                {
                    s.Id,
                    s.StateId,
                    s.CreateTime,
                    s.State,
                    s.Remark,
                    s.SubclassId,
                    s.AbnormalId,
                    s.IdPersonApproval,
                    s.IdPerson,
                    s.RepairTypeId,
                    s.seachkey
               }).Distinct();//.Where(k => k.IdPerson == Person || k.IdPersonSupervision == Person);
                #region 判断条件
                #region 判断关键字搜索是否为null
                if (!string.IsNullOrEmpty(model.Seachkey))
                {
                    if (model.Seachkey.IndexOf('|')>0)
                    {
                        return this.JsonRespData(1001,"搜索不能存在特殊字符");
                    }
                    Customerslist = Customerslist.Where(k=>k.seachkey.Contains(model.Seachkey));
                }
                #endregion
                #region 判断筛选的开始时间是否为null
                if (model.StartTime != null)
                {
                    Customerslist = Customerslist.Where(k => k.CreateTime >= model.StartTime);
                }
                #endregion
                #region 判断筛选的结束时间是否为null
                if (model.EndTime != null)
                {
                    Customerslist = Customerslist.Where(k => k.CreateTime <= model.EndTime);
                }
                #endregion
                #region 判断筛选的客诉类型是否为null
                if (model.CustomerType != null)
                {
                    Customerslist = Customerslist.Where(k => k.StateId == model.CustomerType);
                }
                #endregion
                #region 判断筛选的所属门店是否为null
                if (model.OwnedStores != null)
                {
                    Customerslist = Customerslist.Where(k => k.IdPerson <= model.OwnedStores);
                }
                #endregion
                #region 判断筛选的异常类型是否为null
                if (model.AbnormalType != null)
                {
                    Customerslist = Customerslist.Where(k => k.RepairTypeId <= model.AbnormalType);
                }
                #endregion
                #endregion
                var Untreated =Customerslist.Where(k =>k.State==0).Count();
                var Processing=Customerslist.Where(k => k.State == 1).Count();
                var Endover = Customerslist.Where(k => k.State == 2 || k.State == 3 || k.State == 4).Count();
                var AllCount = Customerslist.Where(k => true).Count();
                if ((model.Type??new List<int?>()).Count() > 0)
                {
                    Customerslist = Customerslist.Where(k => model.Type.Contains(k.State));
                }
                var list = Customerslist.ToList();
                string url = HttpContext.Request.Url.ToString().Replace(HttpContext.Request.Url.PathAndQuery, ""); //服务器协议+域名+端口
                var CusDatas = list.Select(s=>new {
                    s.Id,
                    s.StateId,
                    s.CreateTime,
                    s.State,
                    s.Remark,
                    s.SubclassId,
                    SubclassName=dbContext.BA_SysEnType.FirstOrDefault(p=>p.Id==s.SubclassId).Name,
                    Abnormal = dbContext.BA_SysEnType.FirstOrDefault(p => p.Id == s.AbnormalId)?.Name,
                    PersonName = dbContext.OR_Person.FirstOrDefault(p => p.Id == s.IdPersonApproval)?.Name,
                    HeadUrl = dbContext.OR_Person.FirstOrDefault(p => p.Id == s.IdPersonApproval)?.HeadUrl,
                    Attachment = dbContext.BA_Attachment.Where(p => p.SourceId == s.Id && p.CodeBusinessType == "KS01")
                                .Select(w => new
                                {
                                    w.CodeBusinessType,
                                    w.Id,
                                    w.FileName,
                                    FileAccess = url + w.FileAccess
                                }).ToList(),
                }).Distinct().OrderByDescending(p=>p.Id).ToPagedList(model.PageIndex, model.PageSize);
                DdTypeCount Pdrs = new DdTypeCount();
                Pdrs.ListData = CusDatas;
                Pdrs.PageCount = CusDatas.PageCount;
                Pdrs.PageIndex = model.PageIndex;
                Pdrs.HasNextPage = CusDatas.HasNextPage;
                Pdrs.TotalItemCount = CusDatas.TotalItemCount;
                Pdrs.Untreated = Untreated;
                Pdrs.Processing = Processing;
                Pdrs.Endover = Endover;
                Pdrs.AllCount = AllCount;
                return this.JsonRespData(Pdrs);
            }
            catch (Exception ex)
            {
                return this.JsonRespData(1000,ex.Message);
            }
        }
        #endregion

        #region 客诉列表筛选绑定下拉
        //所属门店列表
        [HttpPost]
        public object OwnedStoresList()
        {
            try
            {
                var PersonList = dbContext.ST_Store.Where(p => true);
                var Data = PersonList.Select(p => new
                {
                    p.Id,
                    p.Name
                }).ToList();
                return this.JsonRespData(Data);
            }
            catch (Exception ex)
            {
                return this.JsonRespData(1000,ex.Message);
            }
        }
        //异常类型
        [HttpPost]
        public object AbnormalTypeList()
        {
            try
            {
                var PersonList = dbContext.KS_Customer_Replenish.Where(p => true);
                var Data = PersonList.Select(p => new
                {
                    p.Id,
                    p.Remark
                }).ToList();
                return this.JsonRespData(Data);
            }
            catch (Exception ex)
            {
                return this.JsonRespData(1000, ex.Message);
            }
        }
        #endregion

       

       
        #region 是否需要补充资料
        /// <summary>
        /// 是否补充信息
        /// </summary>
        /// <param name="Id"></param>
        /// <param name="ReplenishBit"></param>
        /// <param name="Remark"></param>
        /// <returns></returns>
        [HttpPost]
        public object Supplement(int Id,bool ReplenishBit,string Remark)
        {
            try
            {
                string userid = User.Identity.Name;
                var KsCusmoter = dbContext.KS_Customer.FirstOrDefault(p=>p.Id==Id);
                if (KsCusmoter!=null)
                {
                   KsCusmoter.ReplenishBit = ReplenishBit;
                   KS_Customer_Replenish KsCusmoterRep = new KS_Customer_Replenish();
                   KsCusmoterRep.IdCustomer = KsCusmoter.Id;
                   KsCusmoterRep.DingCreateTime = DateTime.Now;
                   KsCusmoterRep.DingPerson =dbContext.OR_Person.FirstOrDefault(k=>k.LoginName==userid).Id;
                   KsCusmoterRep.DingRemark = Remark;
                   KsCusmoterRep.SupplemeBit = false;
                   dbContext.KS_Customer_Replenish.Add(KsCusmoterRep);
                }
                dbContext.Configuration.ValidateOnSaveEnabled = false;
                dbContext.SaveChanges();
                dbContext.Configuration.ValidateOnSaveEnabled = true;
                return this.JsonRespData("Ok");
            }
            catch (Exception ex)
            {
                return this.JsonRespData(1000,ex.Message);
            }
        }
        #endregion


   

        //#region 查看是否需要补充信息资料
        //[HttpPost]
        //public object SeeSupplement(int Id)
        //{
        //    try
        //    {
        //        var KsCusmoterRep = dbContext.KS_Customer_Replenish.Where(p => p.IdCustomer == Id);
        //        if (KsCusmoterRep == null)
        //        {
        //            return this.JsonRespData(1001,"未找到客诉！");
        //        }
        //        var data = KsCusmoterRep.Select(w => new
        //        {
        //            Id = w.IdCustomer,
        //            ReplenishBit = dbContext.KS_Customer.FirstOrDefault(p => p.Id == Id).ReplenishBit,
        //            Remark = "",
        //        }).ToList();
        //        return this.JsonRespData(data);
        //    }
        //    catch (Exception ex)
        //    {
        //        return this.JsonRespData(1000, ex.Message);
        //    }
        //}
        //#endregion
    }
}
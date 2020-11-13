using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using System.Web.Mvc;
using Jusoft.YiFang.Api.Help;
using Jusoft.YiFang.Api.Models;
using Jusoft.YiFang.Api.Models.StoreArchives;
using Jusoft.YiFang.Api.PC.Models;
using Jusoft.YiFang.Db;
using X.PagedList;

namespace Jusoft.YiFang.Api.Controllers
{
    [Authorize]
    public class BasisController : Controller
    {
        YiFang_CustomerComplaintEntities dbContext = new YiFang_CustomerComplaintEntities();

        #region 五大客诉反馈类型列表
        [HttpPost]
        public object CustomerTypeList(decimal IdParent)
        {
            var Data = dbContext.BA_SysEnType.Where(k=>k.IdParent==IdParent).Select(s=>new {
                s.Id,
                s.IdSysEnType,
                s.Name
            }).ToList();

            return this.JsonRespData(Data);
        }
        #endregion

        #region 基础档案
        [HttpPost]
        public object TypeList(decimal IdParent)
        {
            var Data = dbContext.BA_SysEnType.Where(k => k.IdParent == IdParent).Select(s => new {
                s.Id,
                s.Name
            }).ToList();

            return this.JsonRespData(Data);
        }
        #endregion
        /// <summary>
        /// 客诉类型列表
        /// </summary>
        /// <param name="SearchKey"></param>
        /// <param name="IdParent"></param>
        /// <returns></returns>
        [HttpPost]
        public object CustomerList(string SearchKey,decimal IdParent=1)
        {
            var list = dbContext.BA_SysEnType.AsNoTracking().ToList();
            var data = list.Where(s => s.IdParent == IdParent && (string.IsNullOrEmpty(SearchKey) || s.Name.Contains(SearchKey))).Select(p => new
            {
                p.Id,
                p.Name,
                IsLevel = list.Any(x => x.IdParent == p.Id)
            }).OrderByDescending(p=>p.IsLevel);
            return this.JsonRespData(data);
        }
        [HttpPost]
        public object TypeDetails(decimal Id)
        {
            var info = dbContext.BA_SysEnType.FirstOrDefault(p => p.Id == Id);
            if (info==null)
            {
                return this.JsonRespData(1000, "该类型不存在，请刷新");
            }
            else
            {
                var obj = new
                {
                    Memo = info.Memo
                };
                return this.JsonRespData(0,"成功",obj);
            }
        }
        public object CustomerList2(string SearchKey, decimal IdParent = 1)
        {
            var list = dbContext.BA_SysEnType.AsNoTracking().ToList();
            var data = list.Where(s => s.IdParent == IdParent && (string.IsNullOrEmpty(SearchKey)||s.Name.Contains(SearchKey))).Select(p => new
            {
                value=p.Id,
                label = p.Name,
                IsLevel = list.Any(x => x.IdParent == p.Id),
                children=GetChildren(p.Id)
            }).OrderByDescending(p => p.IsLevel);
            return this.JsonRespData(data);
        }

        public object GetChildren(decimal Id)
        {
            var obj = dbContext.BA_SysEnType.Where(p => p.IdParent == Id).Select(p => new
            {
                value = p.Id,
                label = p.Name,
            });
            return obj;
        }
        [HttpPost]
        public object StoreList(string name,int pagesize,int pageindex)
        {
            var person = dbContext.OR_Person.FirstOrDefault(p => p.LoginName == User.Identity.Name);
            if (person==null)
            {
                return this.JsonRespData(1000, "当前登录者不存在");
            }
            var list = dbContext.ST_Store.Where(p => p.IdSupervisor == person.Id||p.Name.Contains(name)).Select(s=>new { 
            s.Name,
            s.Id
            }).ToPagedList(pageindex,pagesize);
            DdTypeCount Pdrs = new DdTypeCount();
            Pdrs.ListData = list;//返回数据体
            Pdrs.PageCount = list.PageCount;//数据总数
            Pdrs.PageIndex = pageindex;//数据页数
            Pdrs.HasNextPage = list.HasNextPage;//是否有下一页
            Pdrs.TotalItemCount = list.TotalItemCount;//数据总数
            return this.JsonRespData(Pdrs);
        }
        /// <summary>
        /// PC端
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public object PCStoreList()
        {
          
            var list = dbContext.ST_Store.Select(s => new {
                s.Name,
                s.Id
            });
            return this.JsonRespData(list);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="Name"></param>
        /// <returns></returns>
        public object PersonList(string Name)
        {
            var data= dbContext.OR_Person.Where(p => p.Name.Contains(Name)).Select(p => new
            {
                p.Name,
                p.Id
            }).ToList();
            return this.JsonRespData(data);
        }
        /// <summary>
        /// 督导人
        /// </summary>
        /// <param name="name"></param>
        /// <param name="pagesize"></param>
        /// <param name="pageindex"></param>
        /// <returns></returns>
        [HttpPost]
        public object SuperList(string name, int pagesize, int pageindex)
        {
            var id = dbContext.OR_Person.Where(p => p.Name.Contains(name)).Select(p => p.Id).ToList();
            var data = dbContext.CS_CustomerService.AsQueryable();
            if (id.Count()!=0)
            {
                data = dbContext.CS_CustomerService.Where(p => id.Contains(p.PersonId));
            }
            var list= data.Select(p => new
            {
                p.PersonId,
                Name=p.OR_Person.Name
            }).ToPagedList(pageindex,pagesize);
            DdTypeCount Pdrs = new DdTypeCount();
            Pdrs.ListData = list;//返回数据体
            Pdrs.PageCount = list.PageCount;//数据总数
            Pdrs.PageIndex = pageindex;//数据页数
            Pdrs.HasNextPage = list.HasNextPage;//是否有下一页
            Pdrs.TotalItemCount = list.TotalItemCount;//数据总数
            return this.JsonRespData(Pdrs);
        }
        [HttpPost]
        public object SuperList2(string name)
        {
            var id = dbContext.OR_Person.Where(p => p.Name.Contains(name)).Select(p => p.Id).ToList();
            var data = dbContext.CS_CustomerService.AsQueryable();
            if (id.Count() != 0)
            {
                data = dbContext.CS_CustomerService.Where(p => id.Contains(p.PersonId));
            }
            var list = data.Select(p => new
            {
                p.PersonId,
                Name = p.OR_Person.Name
            }).ToList();
            return this.JsonRespData(list);
        }
        /// <summary>
        /// 地区
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public object AreaList()
        {
            var list = dbContext.BA_SysEnType.Where(p => p.IdParent == 2).Select(p => new
            {
                p.Id,
                p.Name
            }).ToList();
            return this.JsonRespData(list);
        }
        /// <summary>
        /// 地区负责人
        /// </summary>
        /// <param name="name"></param>
        /// <param name="pagesize"></param>
        /// <param name="pageindex"></param>
        /// <returns></returns>
       [HttpPost]
       
        public object RegionList(string name, int pagesize=10, int pageindex=1)
        {
            Expression<Func<OR_Person, bool>> where = p => (string.IsNullOrEmpty(name) || p.Name.Contains(name));
            var list = dbContext.OR_Person.Where(where).Select(p => new
            {
                p.Id,
                p.Name
            }).OrderByDescending(p=>p.Id).ToPagedList(pageindex, pagesize);
            DdTypeCount Pdrs = new DdTypeCount();
            Pdrs.ListData = list;//返回数据体
            Pdrs.PageCount = list.PageCount;//数据总数
            Pdrs.PageIndex = pageindex;//数据页数
            Pdrs.HasNextPage = list.HasNextPage;//是否有下一页
            Pdrs.TotalItemCount = list.TotalItemCount;//数据总数
            return this.JsonRespData(Pdrs);
        }


        /// <summary>
        /// 市
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        [HttpPost]
        public object City(BasicDetailsModel model)
        {
            ResultData rs = new ResultData();
            try
            {
                var List = dbContext.v_Jusoft_PcBASysArea.Where(p => true);
                if (model.Id != null)
                {
                    List = List.Where(p => p.idSysArea == model.Id);
                }
                else
                {
                    List = List.Where(p => p.LevelType == 1);
                }
                rs.Data = List.Select(p => new
                {
                    p.Id,
                    p.Name,
                    p.MergerName,
                    p.PinYinName
                });
                return Json(rs);
            }
            catch (Exception ex)
            {
                rs.Code = 1000;
                rs.strResult = ex.Message;
                return Json(rs);
            }
        }
        public class OutputModel :Page
        {

            public string Name { get; set; }
        }
    }
}
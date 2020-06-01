using Jusoft.DingTalk.Core.Logs;
using Jusoft.YiFang.Api.Help;
using Jusoft.YiFang.Api.Models.StoreArchives;
using Jusoft.YiFang.Db;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using X.PagedList;

namespace Jusoft.YiFang.Api.Controllers
{
    [Authorize]
    public class StoreArchivesController : ApiController
    {
        YiFang_CustomerComplaintEntities dbContext = new YiFang_CustomerComplaintEntities();
        [HttpGet]
        public IHttpActionResult GetDataList([FromUri]OutputStoreArchiveModel model)
        {
            try
            {
                string name = model.SearchKey == null ? "" : model.SearchKey.ToString();

                Expression<Func<v_StoreArchives, bool>> where = p => (string.IsNullOrEmpty(name) || p.Name.Contains(name)
                ||p.PersonRegion.Contains(name)||p.RegionArea.Contains(name)||p.Address.Contains(name)||p.Supervisor.Contains(name)||p.Code.Contains(name));
                var data = dbContext.v_StoreArchives.Where(where);
                var list = data.ToList().Select(p => new
                {
                    p.Id,
                    p.Name,//门店mingcheng
                    p.Supervisor,//督导人
                    p.Address,//门店详情地址
                    p.RegionArea,//所属大区
                    p.PersonRegion,//所属区域负责人
                    p.Code,//门店代码
                }).OrderByDescending(p=>p.Id).ToPagedList(model.PageIndex, model.PageSize);
                return JsonResultHelper.JsonResult(list);
            }
            catch (Exception ex)
            {
                LogHelper.WriteLog(ex.ToString());
                return JsonResultHelper.JsonResult(1000, ex.Message);
            }
        }

        [HttpPost]
        public IHttpActionResult GetDataList2(OutputStoreArchiveModel model)
        {
            try
            {

                Expression<Func<ST_Store, bool>> where = p => (string.IsNullOrEmpty(model.Name) || p.Name.Contains(model.Name));
                var data = dbContext.ST_Store.Where(where);
                //类型
                var enType = dbContext.BA_SysEnType.ToList();
                //人员
                var person = dbContext.OR_Person.ToList();
                var list = data.ToList().Select(p => new
                {
                    p.Id,
                    p.Name,//门店mingcheng
                    Supervisor = person.FirstOrDefault(x => x.Id == p.IdSupervisor) == null ? "" : person.FirstOrDefault(x => x.Id == p.IdSupervisor).Name,//督导人
                    p.Address,//门店详情地址
                    RegionArea = enType.FirstOrDefault(s => s.Id == p.RegionId) == null ? "" : enType.FirstOrDefault(s => s.Id == p.RegionId).Name,//所属大区
                    PersonRegion = person.FirstOrDefault(s => s.Id == p.IdPersonRegion) == null ? "" : person.FirstOrDefault(s => s.Id == p.IdPersonRegion).Name//所属区域负责人
                }).ToPagedList(model.PageIndex, model.PageSize);
                return JsonResultHelper.JsonResult(list);
            }
            catch (Exception ex)
            {
                LogHelper.WriteLog(ex.ToString());
                return JsonResultHelper.JsonResult(1000, ex.Message);
            }
        }
        [HttpPost]
        public IHttpActionResult Add(InputStoreArchiveModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    //返回错误
                    return JsonResultHelper.JsonResult(1000, ModelState.Values.SelectMany(p => p.Errors.Select(s => s.ErrorMessage)));
                }
                else
                {
                    if (dbContext.ST_Store.Any(p=>p.Name==model.Name))
                    {
                        return JsonResultHelper.JsonResult(1000, "该门店已存在");
                    }
                    if (dbContext.ST_Store.Any(p => p.Code == model.Code))
                    {
                        return JsonResultHelper.JsonResult(1000, "该门店编码已存在");
                    }
                    ST_Store store = new ST_Store()
                    {
                        Address = model.Address,
                        Code = model.Code,
                        IdPersonRegion = model.IdPersonRegion,
                        IdSupervisor = model.IdSupervisor,
                        RegionId = model.RegionId,
                        Remark = model.Remark,
                        Name = model.Name,
                        IdArea = model.IdArea
                    };
                    dbContext.ST_Store.Add(store);
                    dbContext.SaveChanges();
                    string sql = string.Format("	INSERT  REF_STORE_PERSON  VALUES ({0},{1})", store.IdSupervisor, store.Id);
                    dbContext.Database.ExecuteSqlCommand(sql);
                    return JsonResultHelper.JsonResult(store.Id);
                }
            }
            catch (Exception ex)
            {
                return JsonResultHelper.JsonResult(1000, ex.Message);
            }
        }
        [HttpPost]
        public IHttpActionResult Update(InputStoreArchiveModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    //返回错误
                    return JsonResultHelper.JsonResult(1000, ModelState.Values.SelectMany(p => p.Errors.Select(s => s.ErrorMessage)));
                }
                else
                {
                    //if (dbContext.ST_Store.Any(p => p.Name == model.Name))
                    //{
                    //    return JsonResultHelper.JsonResult(1000, "该门店已存在");
                    //}
                    //if (dbContext.ST_Store.Any(p => p.Code == model.Code))
                    //{
                    //    return JsonResultHelper.JsonResult(1000, "该门店编码已存在");
                    //}
                    var info = dbContext.ST_Store.FirstOrDefault(p => p.Id == model.Id);
                    info.IdPersonRegion = model.IdPersonRegion;
                    info.IdSupervisor = model.IdSupervisor;
                    info.Address = model.Address;
                    info.Code = model.Code;
                    info.RegionId = model.RegionId;
                    info.Remark = model.Remark;
                    info.Name = model.Name;
                    info.IdArea = model.IdArea;
                    dbContext.SaveChanges();
                    return JsonResultHelper.JsonResult(0, "OK", model);
                }
            }
            catch (Exception ex)
            {
                return JsonResultHelper.JsonResult(1000, ex.Message);
            }
        }
        [HttpGet]
        public IHttpActionResult Details(decimal? Id)
        {
            var info = dbContext.ST_Store.FirstOrDefault(p => p.Id == Id);
            if (info == null)
            {
                return JsonResultHelper.JsonResult(1000, "该信息已不存在");
            }
            else
            {
                var ar = dbContext.v_Jusoft_StortArea.FirstOrDefault(p => p.Id == info.Id);
                object data = new
                {
                    info.IdPersonRegion,
                    info.IdSupervisor,
                    info.Address,
                    ar.Area1,
                    ar.Area2,
                    ar.Area3,
                    info.Code,
                    info.RegionId,
                    info.Remark,
                    info.Name,
                    Id=info.Id
                };
                return JsonResultHelper.JsonResult(data);
            }
        }
        
        [HttpGet]
        public IHttpActionResult Delete(decimal? Id)
        {
            var info = dbContext.ST_Store.FirstOrDefault(p => p.Id == Id);
            if (info == null)
            {
                return JsonResultHelper.JsonResult(1000, "该信息已不存在");
            }
            else
            {
                string sql = string.Format("DELETE FROM  dbo.REF_STORE_PERSON WHERE IdStore={0}", Id);
                dbContext.Database.ExecuteSqlCommand(sql);
                dbContext.ST_Store.Remove(info);
                dbContext.SaveChanges();
                return JsonResultHelper.JsonResult(0, "删除成功");
            }

        }
    }
}

using Jusoft.YiFang.Api.Help;
using Jusoft.YiFang.Api.Models.CustomerService;
using Jusoft.YiFang.Api.Models.StorePerson;
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
    public class StorePersonController : ApiController
    {
        YiFang_CustomerComplaintEntities DbContext = new YiFang_CustomerComplaintEntities();
        // GET: CustomerService
        /// <summary>
        /// 获取客服列表
        /// </summary>
        /// <param name="output"></param>
        /// <returns></returns>
        [HttpGet]
        public IHttpActionResult GetDatalist([FromUri]OutputCustomerServiceModel output)
        {
            try
            {
                string name = output.SearchKey==null?"": output.SearchKey.ToString();

                //查询条件
                Expression<Func<v_Store_Person, bool>> where = p => (string.IsNullOrEmpty(name) || p.StoreName.Contains(name)) 
                ||(string.IsNullOrEmpty(name) || p.StoreName.Contains(name)) ||
                (string.IsNullOrEmpty(name) || p.Code.Contains(name))||(string.IsNullOrEmpty(name)||p.PsnMobilePhone.Contains(name)) ;
                //列表展示 人员名称，手机号码，所属门店
                var list = DbContext.v_Store_Person.Where(where);
                var data = list.Select(s => new
                {
                    s.Name,
                    s.StoreName,
                    s.Code,
                    s.IdPerson,
                    Phone = s.PsnMobilePhone
                }).ToPagedList(output.PageIndex, output.PageSize);
                return JsonResultHelper.JsonResult(data);
            }
            catch (Exception ex)
            {
                return JsonResultHelper.JsonResult(1000, ex.Message);
            }
        }
        /// <summary>
        /// 新增
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPost]
        public IHttpActionResult Add(InputStorePersonModel input)
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
                    string sql = string.Format("SELECT IdPerson FROM REF_STORE_PERSON WHERE IdPerson={0}", input.IdPerson);
                    //获取中间表中的人员id
                    var personlist= DbContext.Database.SqlQuery<decimal>(sql).ToList();
                    if (personlist.Count()>0)
                    {
                        return JsonResultHelper.JsonResult(1000, "该人员已有门店");
                    }
                    else
                    {
                        string insert = string.Format("INSERT REF_STORE_PERSON(IdPerson,IdStore)VALUES({0},{1})", input.IdPerson, input.IdStore);
                        DbContext.Database.ExecuteSqlCommand(insert);
                        return JsonResultHelper.JsonResult(0, "添加成功");
                    }
                }
            }
            catch (Exception ex)
            {
                return JsonResultHelper.JsonResult(1000, ex.Message);
            }
        }
        /// <summary>
        /// 修改
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPost]
        public IHttpActionResult Update(InputStorePersonModel input)
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
                    string update = string.Format("UPDATE REF_STORE_PERSON SET IdStore={0} WHERE IdPerson={1}", input.IdStore, input.IdPerson);
                    DbContext.Database.ExecuteSqlCommand(update);
                    return JsonResultHelper.JsonResult(0, "添加成功");
                }
            }
            catch (Exception ex)
            {
                return JsonResultHelper.JsonResult(1000, ex.Message);
            }
        }
        /// <summary>
        /// 详情
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        [HttpGet]
        public IHttpActionResult Details(decimal? Id)
        {
            var info = DbContext.v_Store_Person.FirstOrDefault(p => p.IdPerson == Id);
            if (info == null)
            {
                return JsonResultHelper.JsonResult(1000, "该客服已不存在");
            }
            else
            {
                var data = new
                {
                    info.Name,
                    info.StoreName,
                    info.Code,
                    info.IdPerson,
                    Phone = info.PsnMobilePhone,
                    info.IdStore
                };
                return JsonResultHelper.JsonResult(data);
            }
        }
        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        [HttpGet]
        public IHttpActionResult Delete(decimal? Id)
        {
            var delete = string.Format("DELETE FROM  REF_STORE_PERSON  WHERE IdPerson={0}", Id);
            DbContext.Database.ExecuteSqlCommand(delete);
            return JsonResultHelper.JsonResult(0, "删除成功");
        }
    }
}

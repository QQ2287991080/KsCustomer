using Demo.Models;
using Jusoft.YiFang.Api.Help;
using Jusoft.YiFang.Api.Models.CustomerService;
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
    public class CustomerServiceController : ApiController
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
                string name = output.SearchKey == null ? "" : output.SearchKey.ToString();
                //LogHelper.WriteLog(name+output.PageIndex+output.PageSize);
                //查询条件
                Expression<Func<v_CustomerService, bool>> where = p => (string.IsNullOrEmpty(name) || p.Name.Contains(name))|| (string.IsNullOrEmpty(name)
                || p.DepName.Contains(name))|| (string.IsNullOrEmpty(name)||p.Phone.Contains(name))|| (string.IsNullOrEmpty(name)||p.Remark.Contains(name));
                var list = DbContext.v_CustomerService.Where(where);
                var data = list.Select(s => new
                {
                    s.Name,//客服名称
                    s.Phone,//手机号
                    s.DepName,//部门名称
                    s.Id,
                    s.Remark,
                    s.CreateTime
                }).OrderByDescending(p => p.CreateTime).ToPagedList(output.PageIndex, output.PageSize);
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
        public IHttpActionResult Add(InputCustomerServiceModel input)
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
                    CS_CustomerService cs = new CS_CustomerService();
                    cs.PersonId = input.IdPerson;
                    cs.Remark = input.Remark;
                    cs.CreateTime = DateTime.Now;
                    DbContext.CS_CustomerService.Add(cs);
                    DbContext.SaveChanges();
                    return JsonResultHelper.JsonResult(0, "添加成功", cs.Id);
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
        public IHttpActionResult Update(InputCustomerServiceModel input)
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
                    var info = DbContext.CS_CustomerService.FirstOrDefault(p => p.Id == input.Id);
                    if (info==null)
                    {
                        return JsonResultHelper.JsonResult(1000, "该客服已不存在");
                    }
                    if (DbContext.CS_CustomerService.Any(p=>p.PersonId==input.IdPerson))
                    {
                        return JsonResultHelper.JsonResult(1000, "该客服存在");
                    }
                    info.Remark = input.Remark;
                    info.PersonId = input.IdPerson;
                    DbContext.SaveChanges();
                    return JsonResultHelper.JsonResult(0, "修改成功");
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
            var info = DbContext.CS_CustomerService.FirstOrDefault(p => p.Id == Id);
            if (info == null)
            {
                return JsonResultHelper.JsonResult(1000, "该客服已不存在");
            }
            else
            {
                var data = new
                {
                    Name = info.OR_Person == null ? "" : info.OR_Person.Name,//客服名称
                    Phone = info.OR_Person == null ? "" : info.OR_Person.PsnMobilePhone,//手机号
                    Department = info.OR_Person == null ? "" : info.OR_Person.OR_Department.Name,//部门名称
                    info.Id,
                    info.Remark,
                    info.PersonId
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
            var info = DbContext.CS_CustomerService.FirstOrDefault(p => p.Id == Id);
            if (info == null)
            {
                return JsonResultHelper.JsonResult(1000, "该客服已不存在");
            }
            else
            {
                DbContext.CS_CustomerService.Remove(info);
                DbContext.SaveChanges();
                return JsonResultHelper.JsonResult(0, "删除成功");
            }
        }
    }

}

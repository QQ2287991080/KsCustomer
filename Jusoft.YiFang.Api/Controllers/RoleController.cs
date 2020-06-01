using Jusoft.YiFang.Api.Help;
using Jusoft.YiFang.Api.PC.Models;
using Jusoft.YiFang.Db;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using X.PagedList;

namespace Jusoft.YiFang.Api.Controllers
{
    [Authorize]
    public class RoleController : ApiController
    {

        YiFang_CustomerComplaintEntities dbContext = new YiFang_CustomerComplaintEntities();
        #region 角色信息维护
        /// <summary>
        /// 列表
        /// </summary>
        /// <param name="SeachKey"></param>
        /// <param name="PageIndex"></param>
        /// <param name="PageSize"></param>
        /// <returns></returns>
        [HttpGet]
        public IHttpActionResult RoleMaintain([FromUri]RoleMaintainModel model)
        {
            ResultPageData rs = new ResultPageData();
            try
            {
                var List = dbContext.v_Jusoft_PcSysRoles.Where(p => true);
                if (!string.IsNullOrEmpty(model.SeachKey))
                {
                    List = List.Where(p => p.Name.Contains(model.SeachKey));
                }
                var Data = List.Select(p => new
                {
                    p.Id,
                    p.Code,
                    p.Name,
                }).OrderByDescending(p => p.Id).ToPagedList(model.PageIndex, model.PageSize);
                return JsonResultHelper.JsonResult(Data);
            }
            catch (Exception ex)
            {
                return JsonResultHelper.JsonResult(1000, ex.Message);
            }
        }
        /// <summary>
        /// 添加、编辑
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public IHttpActionResult RoleInfo(RoleInfoModel model)
        {
            try
            {

                var SysRole = dbContext.AC_SysRoles.FirstOrDefault(p => p.Id == model.Id);
                if (SysRole != null)
                {
                    var SysRoleCode = dbContext.AC_SysRoles.Where(p => p.Code == model.Code).ToList();
                    if (SysRoleCode.Count() > 0)
                    {

                        return JsonResultHelper.JsonResult(1000, "角色编码已存在，请重新输入！");
                    }
                    SysRole.Name = model.RoleName;
                }
                else
                {
                    AC_SysRoles Role = new AC_SysRoles();
                    Role.Code = model.Code;
                    Role.Name = model.RoleName;
                    dbContext.AC_SysRoles.Add(Role);
                }
                dbContext.Configuration.ValidateOnSaveEnabled = false;
                dbContext.SaveChanges();
                dbContext.Configuration.ValidateOnSaveEnabled = true;

                return JsonResultHelper.JsonResult(0, "操作成功");
            }
            catch (Exception ex)
            {
                return JsonResultHelper.JsonResult(1000, ex.Message);
            }
        }
        #endregion
        #region 角色分配菜单-功能
        [HttpPost]
        public IHttpActionResult EditRole_MenuFunction(EditRoleMenu model)
        {
            try
            {
                var sysRoles = dbContext.AC_SysRoles.FirstOrDefault(k => k.Id == model.Id);
                if (sysRoles == null)
                {
                    return JsonResultHelper.JsonResult(1000, "角色Id【" + model.Id + "】不存在,请刷新界面");
                }
                //菜单分配
                sysRoles.BA_SysMenu.Clear();
                foreach (var item in model.Menu ?? new List<string>())
                {
                    var sysMenu = dbContext.BA_SysMenu.FirstOrDefault(k => k.Code == item);
                    if (sysMenu == null)
                    {
                        return JsonResultHelper.JsonResult(1000, "菜单编码【" + item + "】不存在,请刷新界面");
                    }
                    sysRoles.BA_SysMenu.Add(sysMenu);
                }
                //功能分配
                sysRoles.BA_SysCommand.Clear();
                foreach (var item in model.Function ?? new List<string>())
                {
                    var sysCommand = dbContext.BA_SysCommand.FirstOrDefault(k => k.Code == item);
                    if (sysCommand == null)
                    {
                        return JsonResultHelper.JsonResult(1000, "功能编码【" + item + "】不存在,请刷新界面");
                    }
                    sysRoles.BA_SysCommand.Add(sysCommand);
                }
                dbContext.SaveChanges();
                return JsonResultHelper.JsonResult(0, "操作成功");
            }
            catch (Exception ex)
            {
                return JsonResultHelper.JsonResult(1000, ex.Message);
            }
        }
        #endregion

        #region 角色菜单-功能详情
        [HttpPost]
        public IHttpActionResult Role_MenuFunctionDetails(BasicDetailsModel model)
        {
            try
            {
                if (!dbContext.AC_SysRoles.Any(k => k.Id == model.Id))
                {
                    return JsonResultHelper.JsonResult(1000, "请求失败!角色Id【" + model.Id + "】已不存在, 请刷新界面");
                }
                //菜单-功能递归
                //List<MenuFunctionModels> data = RecursiveMenuFunction(null);

                //读取角色菜单
                var datamenucheck = dbContext.AC_SysRoles.FirstOrDefault(k => k.Id == model.Id).BA_SysMenu.Select(k => k.Code).ToList();

                //读取角色功能
                var datafunctioncheck = dbContext.AC_SysRoles.FirstOrDefault(k => k.Id == model.Id).BA_SysCommand.Select(k => k.Code).ToList();

                var data = new
                {
                    datamenucheck,
                    datafunctioncheck
                };
                return JsonResultHelper.JsonResult(data);
            }
            catch (Exception ex)
            {
                return JsonResultHelper.JsonResult(1000, ex.Message);
            }
        }


        [HttpPost]
        public IHttpActionResult BasicDetails(BasicDetailsModel model)
        {
            if (model.type == 4)
            {
                var SysUsers = dbContext.AC_SysUsers.FirstOrDefault(p => p.Id == model.Id);
                if (SysUsers == null)
                {
                    return JsonResultHelper.JsonResult(1000, "该人员不存在");
                }
                var SysRole = SysUsers.AC_SysRoles.Select(p => p.Id).ToList();
                return JsonResultHelper.JsonResult(SysRole);
            }
            else
            {
                return JsonResultHelper.JsonResult(1000, "类型不对");
            }
        }

        #region 菜单-功能递归
        [HttpPost]
        public List<MenuFunctionModels> RecursiveMenuFunction(RecursiveMenu model)
        {
            List<MenuFunctionModels> menuFunctions = new List<MenuFunctionModels>();
            foreach (var item in dbContext.v_jusoft_SysMenu_Command.Where(k => k.IdSysMenu == model.Id).ToList())
            {
                menuFunctions.Add(new MenuFunctionModels
                {
                    Code = item.Code,
                    Name = item.Name,
                    FunctionState = item.FunctionState,
                    children = RecursiveMenuFunction(new RecursiveMenu { Id = item.Id }),
                });
            }
            return menuFunctions;
        }
        #endregion
        #endregion
        //[HttpPost]
        //public object BasicDetails(BasicDetailsModel model)
        //{
        //    ResultData rs = new ResultData();
        //    try
        //    {
        //        if (model.type == 1)
        //        {
        //            var Data = dbContext.BA_SysEnType.FirstOrDefault(p => p.Id == model.Id);
        //            if (Data == null)
        //            {
        //                rs.Code = 1000;
        //                rs.strResult = "未找到产品型号！";
        //                return Json(rs);
        //            }
        //            decimal? IdSysEnType = null;
        //            decimal? IdParent = dbContext.BA_SysEnType.FirstOrDefault(p => p.Id == Data.IdParent && p.IdParent != null)?.Id;
        //            if (Data.IdSysEnType != null)
        //            {
        //                IdSysEnType = dbContext.BA_SysEnType.FirstOrDefault(p => p.Id == Data.IdParent)?.IdParent;
        //            }
        //            rs.Data = new
        //            {
        //                Data.Id,
        //                IdParent = IdSysEnType,
        //                IdSysEnType = IdParent,
        //                Data.Name,
        //                Data.Memo
        //            };
        //        }
        //        else if (model.type == 2)
        //        {
        //            var Data = dbContext.v_Jusoft_StortDetails.FirstOrDefault(p => p.Id == model.Id);
        //            if (Data == null)
        //            {
        //                rs.Code = 1000;
        //                rs.strResult = "未找到门店信息！";
        //                return Json(rs);
        //            }
        //            rs.Data = new
        //            {
        //                Data.Id,
        //                Data.Code,
        //                Data.Name,
        //                Data.JoinPerson,
        //                Data.Address,
        //                Data.Phone,
        //                Data.IdSupervisor,
        //                Data.RegionId,
        //                Data.IdPersonRegion,
        //                Data.Remark,
        //                Data.UserName,
        //                Data.PasswordHash,
        //                Data.Area1,
        //                Data.Area2,
        //                Data.Area3,
        //            };
        //        }
        //        else if (model.type == 3)
        //        {
        //            var data = dbContext.AC_SysRoles.FirstOrDefault(p => p.Id == model.Id);
        //            if (data == null)
        //            {
        //                rs.Code = 1000;
        //                rs.strResult = "未找到角色信息！";
        //                return Json(rs);
        //            }
        //            rs.Data = new
        //            {
        //                data.Id,
        //                data.Code,
        //                data.Name,
        //            };
        //        }
        //        else if (model.type == 4)
        //        {
        //            var SysUsers = dbContext.AC_SysUsers.FirstOrDefault(p => p.Id == model.Id);
        //            if (SysUsers == null)
        //            {
        //                rs.Code = 1000;
        //                rs.strResult = "未找到用户信息！";
        //                return Json(rs);
        //            }
        //            var SysRole = SysUsers.AC_SysRoles.Select(p => p.Id).ToList();
        //            rs.Data = SysRole;
        //        }
        //        else if (model.type == 5)
        //        {
        //            var KsConfirm = dbContext.KS_Confirm.Where(p => p.TypeId == model.Id).Select(w => new {
        //                w.Id,
        //                w.TypeId,
        //                TypeName = dbContext.BA_SysEnType.FirstOrDefault(s => s.Id == w.TypeId).Name,
        //                w.IdPerson,
        //                PersoName = dbContext.OR_Person.FirstOrDefault(s => s.Id == w.IdPerson).Name,
        //            });
        //            rs.Data = KsConfirm;
        //        }
        //        rs.Code = 0;
        //        rs.strResult = "ok";
        //        return Json(rs);
        //    }
        //    catch (Exception ex)
        //    {
        //        rs.Code = 1001;
        //        rs.strResult = ex.Message;
        //        return Json(rs);
        //    }
        //}

    }
}

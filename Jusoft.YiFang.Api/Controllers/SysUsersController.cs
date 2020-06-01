using Jusoft.DingTalk.Core.Logs;
using Jusoft.YiFang.Api.Help;
using Jusoft.YiFang.Api.PC.Models;
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
    public class SysUsersController : ApiController
    {
        YiFang_CustomerComplaintEntities dbContext = new YiFang_CustomerComplaintEntities();

        #region 用户信息维护
        /// <summary>
        /// 用户列表
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpGet]
        public IHttpActionResult UserMaintain([FromUri]UserMaintainModel model)
        {
            try
            {
                string name = model.SearchKey == null ? "" : model.SearchKey.ToString();
                Expression<Func<v_Jusoft_PcSysUser, bool>> where = w => (string.IsNullOrEmpty(name) || w.Name.Contains(name)) ||
                (string.IsNullOrEmpty(name) || w.PsnMobilePhone.Contains(name)) ||(string.IsNullOrEmpty(name) || w.DepName.Contains(name));
                var List = dbContext.v_Jusoft_PcSysUser.Where(where);
                var Data = List.Select(p => new
                {
                    p.id,
                    p.Name,
                    p.DepName,
                    p.PsnMobilePhone,
                    p.LockoutEnabled,
                    Roles = dbContext.AC_SysUsers.Where(o => o.Id == p.id).Select(k => new
                    {
                        roles = k.AC_SysRoles.Select(s => new
                        {
                            s.Name
                        }).ToList()
                    }),
                }).OrderBy(p => p.id).ToList().ToPagedList(model.PageIndex, model.PageSize);
                return JsonResultHelper.JsonResult(Data);
            }
            catch (Exception ex)
            {
                return JsonResultHelper.JsonResult(1000, ex.Message);
            }
        }
        /// <summary>
        /// 用户分配角色
        /// </summary> 
        /// <returns></returns>
        [HttpPost]
        public IHttpActionResult UserDistriRole(UserDistriModel model)
        {
            try
            {
                var SysUsers = dbContext.AC_SysUsers.FirstOrDefault(p => p.Id == model.UserId);
                if (SysUsers == null)
                {
                  
                    return JsonResultHelper.JsonResult(1000, "未找到用户！");
                }
                SysUsers.AC_SysRoles.Clear();
                foreach (var item in model.Roles ?? new List<int>())
                {
                    var SysRole = dbContext.AC_SysRoles.FirstOrDefault(p => p.Id == item);
                    if (SysRole != null)
                    {
                        SysUsers.AC_SysRoles.Add(SysRole);
                    }
                }
                
                dbContext.SaveChanges();
                return JsonResultHelper.JsonResult(0, "操作成功");
            }
            catch (Exception ex)
            {
                return JsonResultHelper.JsonResult(1000, ex.Message);
            }
        }


        #region   用户权限
        [HttpGet]
        public IHttpActionResult UserPermissionsDetails(decimal Id)
        {
            var info = dbContext.AC_SysUsers.FirstOrDefault(p => p.Id == Id);
            if (info==null)
            {
                return JsonResultHelper.JsonResult(1000, "该用户已不存在");
            }
            else
            {
                var Menu= info.AUTH_User_Menu.Select(p=>p.IdSysMenu).ToList();
                var Command = info.AUTH_User_Command.Select(p => p.CodeSysCommand).ToList();
                var data = new
                {
                    Menu,
                    Command
                };
                return JsonResultHelper.JsonResult(data);
            }
        
        }
        [HttpPost]
        public IHttpActionResult UserEidtPermissions(EditUsers model)
        {
            var info = dbContext.AC_SysUsers.FirstOrDefault(p => p.Id == model.Id);
            if (info == null)
            {
                return JsonResultHelper.JsonResult(1000, "该用户已不存在");
            }
            else
            {
                //所有的菜单权限
                var menuList = dbContext.BA_SysMenu.Select(p => p.Id).ToList();
                var oldMenu = info.AUTH_User_Menu.Where(p => menuList.Contains(p.IdSysMenu)).ToList();//当前用户存在的菜单权限
                oldMenu.ForEach(old => { dbContext.AUTH_User_Menu.Remove(old); });//删除所有的用户权限
                model.Menu.ForEach(news => dbContext.AUTH_User_Menu.Add(new AUTH_User_Menu { IdSysMenu = news, IdSysUsers = info.Id }));//新增当前用户的权限

                //所有功能权限
                var commandList = dbContext.BA_SysCommand.Select(p => p.Code).ToList();
                var oldCommand = info.AUTH_User_Command.Where(p => commandList.Contains(p.CodeSysCommand)).ToList();
                oldCommand.ForEach(old => { dbContext.AUTH_User_Command.Remove(old); }) ;
                model.Command.ForEach(news => dbContext.AUTH_User_Command.Add(new AUTH_User_Command { IdSysUsers = info.Id, CodeSysCommand = news }));
                dbContext.SaveChanges();
                return JsonResultHelper.JsonResult(0, "操作成功");
            }
           
        }
        [HttpPost]
        public IHttpActionResult User_Information()
        {
            try
            {
                //获取登入者的信息
                var users = dbContext.AC_SysUsers.FirstOrDefault(k => k.UserName == User.Identity.Name);
                LogHelper.WriteLog(User.Identity.Name);
                if (users == null)
                {
                    return JsonResultHelper.JsonResult(1000, "该用户已不存在，请刷新！");
                }

                //读取用户可查看菜单
                List<decimal> user_Menus = users.AUTH_User_Menu.Select(k => k.IdSysMenu).ToList();

                List<string> code = new List<string>();
                //读取用户角色可查看菜单
                var Roles = users.AC_SysRoles.Select(k => new { k.Id, k.Code }).ToList();
                foreach (var item in Roles)
                {

                    List<string> role_menu = dbContext.AC_SysRoles.FirstOrDefault(k => k.Id == item.Id).BA_SysMenu.Select(w => w.Code).ToList();

                    code = code.Union(role_menu).ToList();
                }
                var Menus = code;

                //读取用户可查看菜单-功能
                var user_Functions = users.AUTH_User_Command.Select(k => k.CodeSysCommand).ToList();

                //读取用户角色可查看菜单-功能
                foreach (var item in users.AC_SysRoles.Select(k => k.Id).ToList())
                {
                    var role_function = dbContext.AC_SysRoles.FirstOrDefault(k => k.Id == item).BA_SysCommand.Select(w => w.Code).ToList();

                    user_Functions = user_Functions.Union(role_function).ToList();
                }
                var person = dbContext.OR_Person.FirstOrDefault(p => p.PsnNum == users.UserName);
                //读取用户可查看菜单功能
                var Funs = user_Functions;
                var data = new { Id = users.Id, Avatar = "", UserName = person.Name, UserId = users.Id, Menus = Menus, Roles = Roles.Select(k => k.Code).ToList(), Funs = Funs };
                return JsonResultHelper.JsonResult(data);
            }
            catch (Exception ex)
            {

                return JsonResultHelper.JsonResult(1000, $"错误原因{ex.Message}");
            }
        }
        #endregion
        #endregion


        //public IHttpActionResult UserCommand(decimal Id)
        //{
        //    var info = dbContext.AC_SysUsers.FirstOrDefault(p => p.Id == Id);
        //    info.AUTH_User_Menu;
        //    info.AUTH_User_Command
        //}

        #region 产品型号信息维护
        /// <summary>
        /// 列表
        /// </summary>
        /// <param name="SeachKey">关键字搜索</param>
        /// <returns></returns>
        [HttpPost]
        public IHttpActionResult BasicMaintain(BasicMaintainModel model)
        {
            ResultPageData rs = new ResultPageData();
            try
            {
                var List = dbContext.v_Jusoft_PcSysEnType.Where(p => true);
                if (model.IdParent != null)
                {
                    List = List.Where(p => p.IdSysEnType == model.IdParent);
                }
                if (model.IdSysEnType != null)
                {
                    List = List.Where(p => p.IdParent == model.IdSysEnType);
                }
                if (!string.IsNullOrEmpty(model.SeachKey))
                {
                    List = List.Where(p => p.Name.Contains(model.SeachKey));
                }
                List<decimal?> Ids = dbContext.f_getEnTypeList(5).Select(p => p.Id).ToList();
                List = List.Where(p => Ids.Contains(p.Id));
                var Data = List.Select(p => new {
                    p.Id,
                    p.ParentName,
                    p.SysEnTypeName,
                    p.Name,
                    p.Memo
                }).OrderByDescending(p => p.Id).ToPagedList(model.PageIndex, model.PageSize);
                return JsonResultHelper.JsonResult(Data);
            }
            catch (Exception ex)
            {
                return JsonResultHelper.JsonResult(1000, ex.Message);
            }
        }
        /// <summary>
        /// 产品型号信息维护添加、编辑
        /// </summary>
        /// <param name="Id"></param>
        /// <param name="IdParent"></param>
        /// <param name="IdSysEnType"></param>
        /// <param name="ProducName"></param>
        /// <param name="Memo"></param>
        /// <returns></returns>

        public object ProductModel(ProductModel model)
        {
            try
            {

                if (model.Id == null)
                {
                    if (dbContext.BA_SysEnType.Any(p => p.Name == model.ProducName))
                    {
                        return Json(new { strResult = $"添加的产品型号【{model.ProducName}】已存在", Code = 1000 });
                    }
                    BA_SysEnType EnType = new BA_SysEnType()
                    {
                        Name = model.ProducName,
                        Memo = model.Memo
                    };
                    decimal? IdParent = model.IdParent;
                    //校验大类
                    if (model.IdParent == null)
                    {
                        IdParent = 5;//默认最大上级
                    }
                    //校验是否在产品大类中
                    if (!dbContext.f_getEnTypeList(5).Any(k => k.Id == model.IdParent))
                    {
                        return Json(new { strResult = $"添加的大类【{model.IdParent}】不存在，请联系管理员", Code = 1000 });
                    }
                    //检验是否是第三级
                    if (!dbContext.BA_SysEnType.Any(k => k.Id == model.IdParent && k.IdParent == 5))
                    {
                        EnType.IdSysEnType = 5;//第三级默认值
                    }
                    EnType.IdParent = IdParent;
                    dbContext.BA_SysEnType.Add(EnType);
                    dbContext.SaveChanges();
                    return Json(new { strResult = "添加成功", Code = 0 });
                }
                else
                {
                    var BASysEnType = dbContext.BA_SysEnType.FirstOrDefault(p => p.Id == model.Id);
                    if (BASysEnType == null)
                    {
                        return Json(new { strResult = "选择产品不存在，请刷新界面", Code = 10000 });
                    }
                    if (dbContext.BA_SysEnType.Any(p => p.Name == model.ProducName && p.Id != model.Id))
                    {
                        return Json(new { strResult = $"产品型号【{model.ProducName}】已存在,修改失败", Code = 1000 });
                    }
                    BASysEnType.IdSysEnType = null;
                    BASysEnType.IdParent = model.IdParent;
                    //校验大类
                    if (model.IdParent == null)
                    {
                        BASysEnType.IdParent = 5;//默认最大上级
                    }
                    //校验是否在产品大类中
                    if (!dbContext.f_getEnTypeList(5).Any(k => k.Id == model.IdParent))
                    {
                        return Json(new { strResult = $"添加的大类【{model.IdParent}】不存在，请联系管理员", Code = 1000 });
                    }
                    //检验是否是第三级
                    if (!dbContext.BA_SysEnType.Any(k => k.Id == model.IdParent && k.IdParent == 5))
                    {
                        BASysEnType.IdSysEnType = 5;//第三级默认值
                    }
                    BASysEnType.Name = model.ProducName;
                    BASysEnType.Memo = model.Memo;

                    dbContext.SaveChanges();
                    return Json(new { strResult = "修改成功", Code = 0 });
                }
            }
            catch (Exception ex)
            {
                return Json(new { strResult = ex.ToString(), Code = 1001 });
            }
        }
        #endregion
    }

    public class EditUsers
    {
        public int Id { get; set; }
        public List<decimal> Menu { get; set; }
        public List<string> Command { get; set; }
    }
}

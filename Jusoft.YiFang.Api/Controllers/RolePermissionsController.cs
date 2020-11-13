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
    public class RolePermissionsController : ApiController
    {
        YiFang_CustomerComplaintEntities dbContext = new YiFang_CustomerComplaintEntities();

        #region 基础信息详情、删除
        /// <summary>
        /// 详情
        /// </summary>
        /// <param name="Id"></param>
        ///<param name="type">类型为1产品型号详情，类型为2门店详情,
        ///类型为3角色详情，类型为4用户分配角色详情,类型为5操作人详情</param>
        /// <returns></returns>
        [HttpPost]
        public object BasicDetails(BasicDetailsModel model)
        {
            ResultData rs = new ResultData();
            try
            {
                if (model.type == 1)
                {
                    var Data = dbContext.BA_SysEnType.FirstOrDefault(p => p.Id == model.Id);
                    if (Data == null)
                    {
                        rs.Code = 1000;
                        rs.strResult = "未找到产品型号！";
                        return Json(rs);
                    }
                    decimal? IdSysEnType = null;
                    decimal? IdParent = dbContext.BA_SysEnType.FirstOrDefault(p => p.Id == Data.IdParent && p.IdParent != null)?.Id;
                    if (Data.IdSysEnType != null)
                    {
                        IdSysEnType = dbContext.BA_SysEnType.FirstOrDefault(p => p.Id == Data.IdParent)?.IdParent;
                    }
                    rs.Data = new
                    {
                        Data.Id,
                        IdParent = IdSysEnType,
                        IdSysEnType = IdParent,
                        Data.Name,
                        Data.Memo
                    };
                }
                else if (model.type == 2)
                {
                    var Data = dbContext.v_Jusoft_StortDetails.FirstOrDefault(p => p.Id == model.Id);
                    if (Data == null)
                    {
                        rs.Code = 1000;
                        rs.strResult = "未找到门店信息！";
                        return Json(rs);
                    }
                    rs.Data = new
                    {
                        Data.Id,
                        Data.Code,
                        Data.Name,
                        Data.JoinPerson,
                        Data.Address,
                        Data.Phone,
                        Data.IdSupervisor,
                        Data.RegionId,
                        Data.IdPersonRegion,
                        Data.Remark,
                        Data.UserName,
                        Data.PasswordHash,
                        Data.Area1,
                        Data.Area2,
                        Data.Area3,
                    };
                }
                else if (model.type == 3)
                {
                    var data = dbContext.AC_SysRoles.FirstOrDefault(p => p.Id == model.Id);
                    if (data == null)
                    {
                        rs.Code = 1000;
                        rs.strResult = "未找到角色信息！";
                        return Json(rs);
                    }
                    rs.Data = new
                    {
                        data.Id,
                        data.Code,
                        data.Name,
                    };
                }
                else if (model.type == 4)
                {
                    var SysUsers = dbContext.AC_SysUsers.FirstOrDefault(p => p.Id == model.Id);
                    if (SysUsers == null)
                    {
                        rs.Code = 1000;
                        rs.strResult = "未找到用户信息！";
                        return Json(rs);
                    }
                    var SysRole = SysUsers.AC_SysRoles.Select(p => p.Id).ToList();
                    rs.Data = SysRole;
                }
                else if (model.type == 5)
                {
                    var KsConfirm = dbContext.KS_Confirm.Where(p => p.TypeId == model.Id).Select(w => new {
                        w.Id,
                        w.TypeId,
                        TypeName = dbContext.BA_SysEnType.FirstOrDefault(s => s.Id == w.TypeId).Name,
                        w.IdPerson,
                        PersoName = dbContext.OR_Person.FirstOrDefault(s => s.Id == w.IdPerson).Name,
                    });
                    rs.Data = KsConfirm;
                }
                rs.Code = 0;
                rs.strResult = "ok";
                return Json(rs);
            }
            catch (Exception ex)
            {
                rs.Code = 1001;
                rs.strResult = ex.Message;
                return Json(rs);
            }
        }
        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="Type">类型为1产品型号删除，类型为2门店删除，
        /// 类型为3角色删除，类型为4计量单位删除,类型为5操作人删除</param>
        /// <param name="Id"></param>
        /// <returns></returns>
        [HttpPost]
        public object BasicMaintainDel(BasicDetailsModel model)
        {
            Result rs = new Result();
            try
            {
                if (model.Id == null)
                {
                    return Json(new { Code = 1000, strResult = "Id不能为null" });
                }
                if (model.type == 1)
                {
                    var BASysEnType = dbContext.BA_SysEnType.Where(p => p.IdParent == model.Id || p.IdSysEnType == model.Id);
                    if (BASysEnType.Count() > 0)
                    {
                        rs.Code = 0;
                        rs.strResult = "你删除的的产品型号下有小类或者有产品，请先删除小类和产品！";
                        return Json(rs);
                    }
                    var SysEnTypeDel = dbContext.BA_SysEnType.FirstOrDefault(p => p.Id == model.Id);
                    if (SysEnTypeDel != null)
                    {
                        dbContext.BA_SysEnType.Remove(SysEnTypeDel);
                    }
                }
                else if (model.type == 2)
                {
                    var Ststort = dbContext.ST_Store.FirstOrDefault(p => p.Id == model.Id);
                    if (Ststort != null)
                    {
                        //var SysUsers = dbContext.AC_SysUsers.FirstOrDefault(p => p.Id == Ststort.IdSysUsers);
                        AC_SysUsers SysUsers = null;
                        if (SysUsers != null)
                        {
                            var OrPerson = dbContext.OR_Person.FirstOrDefault(p => p.LoginName == SysUsers.UserName);
                            if (OrPerson != null)
                            {
                                OrPerson.LeaveDate = DateTime.Now;
                                OrPerson.WeChatOpenId = null;
                                OrPerson.LoginName = null;
                            }
                            dbContext.AC_SysUsers.Remove(SysUsers);
                        }
                        dbContext.ST_Store.Remove(Ststort);
                    }
                }
                else if (model.type == 3)
                {
                    var SysRoles = dbContext.AC_SysRoles.FirstOrDefault(p => p.Id == model.Id);
                    if (SysRoles != null)
                    {
                        if (SysRoles.BA_SysMenu.Count() > 0)
                        {
                            return Json(new { Code = 1000, strResult = "该角色已分配菜单！" });
                        }
                        if (SysRoles.BA_SysCommand.Count() > 0)
                        {
                            return Json(new { Code = 1000, strResult = "该角色已分配功能！" });
                        }
                        if (SysRoles.AC_SysUsers.Count() > 0)
                        {
                            return Json(new { Code = 1000, strResult = "该角色已被用户使用，请先给用户清除需要删除的角色！" });
                        }
                    }
                    dbContext.AC_SysRoles.Remove(SysRoles);
                }
                else if (model.type == 4)
                {
                    var SysEntype = dbContext.BA_SysEnType.FirstOrDefault(p => p.Id == model.Id);
                    if (SysEntype != null)
                    {
                        dbContext.BA_SysEnType.Remove(SysEntype);
                    }
                }
                else if (model.type == 5)
                {
                    var KsConfirm = dbContext.KS_Confirm.FirstOrDefault(p => p.Id == model.Id && p.IdPerson == model.IdPerson);
                    if (KsConfirm != null)
                    {
                        dbContext.KS_Confirm.Remove(KsConfirm);
                    }
                }
                dbContext.SaveChanges();
                rs.Code = 0;
                rs.strResult = "删除成功！";
                return Json(rs);
            }
            catch (Exception ex)
            {
                rs.Code = 1001;
                rs.strResult = ex.Message;
                return Json(rs);
            }
        }
        #endregion

        #region 产品型号信息维护
        /// <summary>
        /// 列表
        /// </summary>
        /// <param name="SeachKey">关键字搜索</param>
        /// <returns></returns>
        [HttpPost]
        public object BasicMaintain(BasicMaintainModel model)
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
                rs.Data = Data;
                rs.PageIndex = model.PageIndex;
                rs.HasNextPage = Data.HasNextPage;
                rs.PageCount = Data.PageCount;
                rs.TotalItemCount = Data.TotalItemCount;
                rs.Code = 0;
                rs.strResult = "ok";
                return Json(rs);
            }
            catch (Exception ex)
            {
                rs.Code = 1001;
                rs.strResult = ex.Message;
                return Json(rs);
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

        #region 异常类别信息维护
        /// <summary>
        /// 列表
        /// </summary>
        /// <returns></returns>
        public object CateMaintain()
        {
            ResultPageData rs = new ResultPageData();
            try
            {
                return Json(rs);
            }
            catch (Exception ex)
            {
                rs.Code = 1001;
                rs.strResult = ex.Message;
                return Json(rs);
            }
        }
        /// <summary>
        /// 添加、编辑
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public object Category()
        {
            Result rs = new Result();
            try
            {
                return Json(rs);
            }
            catch (Exception ex)
            {
                rs.Code = 1001;
                rs.strResult = ex.Message;
                return Json(rs);
            }
        }
        #endregion

        #region 计量单位维护
        /// <summary>
        /// 列表
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public object CompanyMaintain(CompanyMaintainModel model)
        {
            ResultPageData rs = new ResultPageData();
            try
            {
                if (model.Id != null)
                {
                    var List = dbContext.v_Jusoft_PcSysEnType.Where(p => p.IdParent == model.Id);
                    rs.Data = List.Select(p => new
                    {
                        p.Id,
                        p.Name,
                        p.Memo,
                    }).ToPagedList(model.PageIndex, model.PageSize);
                }
                rs.strResult = "ok";
                rs.Code = 0;
                return Json(rs);
            }
            catch (Exception ex)
            {
                rs.Code = 1001;
                rs.strResult = ex.Message;
                return Json(rs);
            }
        }
        /// <summary>
        /// 添加、编辑
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public object UnitCompany(UnitCompanyModel model)
        {
            Result rs = new Result();
            try
            {
                var SysEnType = dbContext.BA_SysEnType.FirstOrDefault(p => p.Id == model.Id);
                if (SysEnType == null)
                {
                    BA_SysEnType EbType = new BA_SysEnType();
                    EbType.IdParent = Convert.ToInt32(Region.Unit);
                    EbType.Name = model.Name;
                    EbType.Memo = model.Memo;
                    dbContext.BA_SysEnType.Add(EbType);
                }
                else
                {
                    SysEnType.Name = model.Name;
                    SysEnType.Memo = model.Memo;
                }
                dbContext.SaveChanges();
                return Json(rs);
            }
            catch (Exception ex)
            {
                rs.Code = 1001;
                rs.strResult = ex.Message;
                return Json(rs);
            }
        }
        #endregion

        #region 角色信息维护
        /// <summary>
        /// 列表
        /// </summary>
        /// <param name="SeachKey"></param>
        /// <param name="PageIndex"></param>
        /// <param name="PageSize"></param>
        /// <returns></returns>
        [HttpPost]
        public object RoleMaintain(RoleMaintainModel model)
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
                rs.Data = Data;
                rs.PageIndex = model.PageIndex;
                rs.PageCount = Data.PageCount;
                rs.HasNextPage = Data.HasNextPage;
                rs.TotalItemCount = Data.TotalItemCount;
                rs.Code = 0;
                rs.strResult = "ok";
                return Json(rs);
            }
            catch (Exception ex)
            {
                rs.Code = 1001;
                rs.strResult = ex.Message;
                return Json(rs);
            }
        }
        /// <summary>
        /// 添加、编辑
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public object RoleInfo(RoleInfoModel model)
        {
            Result rs = new Result();
            try
            {

                var SysRole = dbContext.AC_SysRoles.FirstOrDefault(p => p.Id == model.Id);
                if (SysRole != null)
                {
                    var SysRoleCode = dbContext.AC_SysRoles.Where(p => p.Code == model.Code&&p.Id!=model.Id).ToList();
                    if (SysRoleCode.Count() > 0)
                    {
                        rs.strResult = "角色编码已存在，请重新输入！";
                        rs.Code = 1001;
                        return Json(rs);
                    }
                    SysRole.Name = model.RoleName;
                    rs.strResult = "编辑成功";
                }
                else
                {
                    AC_SysRoles Role = new AC_SysRoles();
                    Role.Code = model.Code;
                    Role.Name = model.RoleName;
                    dbContext.AC_SysRoles.Add(Role);
                    rs.strResult = "添加成功";
                }
                dbContext.Configuration.ValidateOnSaveEnabled = false;
                dbContext.SaveChanges();
                dbContext.Configuration.ValidateOnSaveEnabled = true;
                rs.Code = 0;
                return Json(rs);
            }
            catch (Exception ex)
            {
                rs.Code = 1001;
                rs.strResult = ex.Message;
                return Json(rs);
            }
        }
        #endregion

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

        #region 角色分配菜单-功能
        [HttpPost]
        public object EditRole_MenuFunction(EditRoleMenu model)
        {
            Result rs = new Result();
            try
            {
                var sysRoles = dbContext.AC_SysRoles.FirstOrDefault(k => k.Id == model.Id);
                if (sysRoles == null)
                {
                    rs.Code = 1001;
                    rs.strResult = "角色Id【" + model.Id + "】不存在,请刷新界面";
                    return Json(rs);
                }
                //菜单分配
                sysRoles.BA_SysMenu.Clear();
                foreach (var item in model.Menu ?? new List<string>())
                {
                    var sysMenu = dbContext.BA_SysMenu.FirstOrDefault(k => k.Code == item);
                    if (sysMenu == null)
                    {
                        rs.Code = 1001;
                        rs.strResult = "菜单编码【" + item + "】不存在,请刷新界面";
                        return Json(rs);
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
                        rs.Code = 1001;
                        rs.strResult = "功能编码【" + item + "】不存在,请刷新界面";
                        return Json(rs);
                    }
                    sysRoles.BA_SysCommand.Add(sysCommand);
                }

                dbContext.SaveChanges();
                rs.Code = 0;
                rs.strResult = "分配成功";
                return Json(rs);
            }
            catch (Exception ex)
            {
                rs.Code = 0;
                rs.strResult = ex.Message;
                return Json(rs);
            }
        }
        #endregion

        #region 角色菜单-功能详情
        [HttpPost]
        public object Role_MenuFunctionDetails(BasicDetailsModel model)
        {
            try
            {
                if (!dbContext.AC_SysRoles.Any(k => k.Id == model.Id))
                {
                    return Json(new { Code = 10001, strResult = "请求失败!角色Id【" + model.Id + "】已不存在,请刷新界面" });
                }
                //菜单-功能递归
                //List<MenuFunctionModels> data = RecursiveMenuFunction(null);

                //读取角色菜单
                var datamenucheck = dbContext.AC_SysRoles.FirstOrDefault(k => k.Id == model.Id).BA_SysMenu.Select(k => k.Code).ToList();

                //读取角色功能
                var datafunctioncheck = dbContext.AC_SysRoles.FirstOrDefault(k => k.Id == model.Id).BA_SysCommand.Select(k => k.Code).ToList();

                return Json(new { datamenucheck = datamenucheck, datafunctioncheck = datafunctioncheck });
            }
            catch (Exception ex)
            {
                return Json(new { Code = 10000, strResult = "请求失败,失败原因:" + ex.ToString() });
            }
        }
        #endregion

        #region 登入者可查看菜单(有可能不需要)
        public object User_MenuView()
        {
            ResultData rs = new ResultData();
            try
            {
                //获取登入者的信息
                var users = dbContext.AC_SysUsers.FirstOrDefault(k => k.UserName == User.Identity.Name);
                if (users == null)
                {
                    rs.Code = 1001;
                    rs.strResult = "该用户不存在在系统中，请刷新界面";
                    return Json(rs);
                }
                //读取用户可查看菜单
                List<string> user_Menu = new List<string>();
                List<string> user_Comm = new List<string>();
                //读取用户角色可查看
                foreach (var item in users.AC_SysRoles.Select(k => k.Id).ToList())
                {
                    user_Menu = dbContext.AC_SysRoles.FirstOrDefault(k => k.Id == item).BA_SysMenu.Select(w => w.Code).ToList();
                    user_Comm = dbContext.AC_SysRoles.FirstOrDefault(k => k.Id == item).BA_SysCommand.Select(w => w.Code).ToList();
                }
                //读取菜单信息
                //List<MenuModels> data = RecursiveMenu(null);
                //读取用户选中菜单(需要传上级菜单id)--暂时不需要
                //List<decimal> Menus = new List<decimal>();
                //foreach (var item in user_Menus)
                //{
                //    Menus.AddRange(RecursiveMenuSuperior(item));
                //}
                //var datacheck = user_Menus.Union(Menus).ToList();
                var Orperson = dbContext.OR_Person.FirstOrDefault(p => p.LoginName == users.UserName);
                rs.Data = new { UserName = Orperson?.Name, HeadUrl = Orperson?.HeadUrl, userMenu = user_Menu, userComm = user_Comm };
                rs.Code = 0;
                rs.strResult = "ok";
                return Json(rs);
            }
            catch (Exception ex)
            {
                rs.Code = 1001;
                rs.strResult = ex.Message;
                return Json(rs);
            }
        }
        #endregion

        #region 菜单递归
        public List<MenuModels> RecursiveMenu(decimal? Id)
        {
            List<MenuModels> menus = new List<MenuModels>();
            foreach (var item in dbContext.BA_SysMenu.Where(k => k.IdSysMenu == Id).ToList())
            {
                menus.Add(new MenuModels
                {
                    Id = item.Id,
                    label = item.Name,
                    children = RecursiveMenu(item.Id)
                });
            }
            return menus;
        }
        #endregion

        #region 用户信息维护
        /// <summary>
        /// 用户列表
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public object UserMaintain(UserMaintainModel model)
        {
            ResultPageData rs = new ResultPageData();
            try
            {
                var List = dbContext.v_Jusoft_PcSysUser.Where(p => true);
                if (!string.IsNullOrEmpty(model.Name))
                {
                    List = List.Where(p => p.Name.Contains(model.Name));
                }
                if (!string.IsNullOrEmpty(model.Phone))
                {
                    List = List.Where(p => p.PsnMobilePhone.Contains(model.Phone));
                }
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
                }).OrderBy(p => p.id).ToPagedList(model.PageIndex, model.PageSize);
                rs.Data = Data;
                rs.PageIndex = model.PageIndex;
                rs.PageCount = Data.PageCount;
                rs.HasNextPage = Data.HasNextPage;
                rs.TotalItemCount = Data.TotalItemCount;
                return Json(rs);
            }
            catch (Exception ex)
            {
                rs.Code = 1001;
                rs.strResult = ex.Message;
                return Json(rs);
            }
        }
        /// <summary>
        /// 用户分配角色
        /// </summary> 
        /// <returns></returns>
        ////[HttpPost]
        public object UserDistriRole(UserDistriModel model)
        {
            Result rs = new Result();
            try
            {
                var SysUsers = dbContext.AC_SysUsers.FirstOrDefault(p => p.Id == model.UserId);
                if (SysUsers == null)
                {
                    rs.Code = 1001;
                    rs.strResult = "未找到用户！";
                    return Json(rs);
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
                rs.strResult = "分配角色成功！";
                dbContext.SaveChanges();
                return Json(rs);
            }
            catch (Exception ex)
            {
                rs.Code = 1000;
                rs.strResult = ex.Message;
                return Json(rs);
            }
        }
        #endregion

        #region 首页
        public object HomeIndex(HomeIndexModel model)
        {
            HomeResult hr = new HomeResult();
            try
            {
                var Data = dbContext.v_Jusoft_PcMissionCenter.Where(p => true);
                if (!string.IsNullOrEmpty(model.StStortCode))
                {
                    Data = Data.Where(p => p.CusStoreCode == model.StStortCode);
                }
                //if (model.SubclassId!=null)
                //{
                //    Data = Data.Where(p=>p.SubclassId==model.SubclassId);
                //}
                if (model.StateTime != null && model.EndTime != null)
                {
                    Data = Data.Where(p => p.CreateTime >= model.StateTime && p.CreateTime <= model.EndTime);
                }
                hr.ProcessedCount = Data.Where(p => p.State == HomeConfig.Processed).Count();
                hr.ProcessingCount = Data.Where(p => p.State == HomeConfig.Processing).Count();
                hr.OverCount = Data.Where(p => p.State == HomeConfig.Over && p.State == HomeConfig.Rejected && p.State == HomeConfig.Evaluated).Count();
                hr.MaterialCount = Data.Where(p => p.SubclassId == HomeConfig.Material).Count();
                hr.RepairCount = Data.Where(p => p.SubclassId == HomeConfig.Repair).Count();
                hr.LogisticsCount = Data.Where(p => p.SubclassId == HomeConfig.Logistics).Count();
                hr.DecorationCount = Data.Where(p => p.SubclassId == HomeConfig.Decoration).Count();
                hr.SupplyOtherCount = Data.Where(p => p.SubclassId == HomeConfig.SupplyOther).Count();
                hr.OperaCount = Data.Where(p => p.SubclassId == HomeConfig.Opera).Count();
                hr.PatrolCount = Data.Where(p => p.SubclassId == HomeConfig.Patrol).Count();
                hr.OperateOtherCount = Data.Where(p => p.SubclassId == HomeConfig.OperateOther).Count();
                hr.ElemerceCount = Data.Where(p => p.SubclassId == HomeConfig.Elemerce).Count();
                hr.OperationCount = Data.Where(p => p.SubclassId == HomeConfig.Operation).Count();
                hr.MarketingCount = Data.Where(p => p.SubclassId == HomeConfig.Marketing).Count();
                hr.AuxiliaryCount = Data.Where(p => p.SubclassId == HomeConfig.Auxiliary).Count();
                hr.ResearchCount = Data.Where(p => p.SubclassId == HomeConfig.Research).Count();
                hr.OtherCount = Data.Where(p => p.SubclassId == HomeConfig.Other).Count();
                hr.Code = 0;
                hr.Result = "ok";
                return Json(hr);
            }
            catch (Exception ex)
            {
                hr.Code = 1000;
                hr.Result = ex.Message;
                return Json(hr);
            }
        }
        #endregion

        #region 发起人信息维护
        [HttpPost]
        public object ConfirMaintain(ConfirMaintainModel model)
        {
            ResultData rs = new ResultData();
            try
            {
                var Data = dbContext.v_Jusoft_KSConfirm.Select(p => new
                {
                    p.TypeId,
                });
                if (model != null)
                {
                    if (model.Type != null)
                    {
                        Data = Data.Where(p => p.TypeId == model.Type);
                    }
                }
                List<ReturnConfirm> listRc = new List<ReturnConfirm>();
                var TypeIdList = Data.OrderBy(p => p.TypeId).GroupBy(p => p.TypeId).ToList();
                foreach (var item in TypeIdList)
                {
                    ReturnConfirm rc = new ReturnConfirm();
                    foreach (var item1 in item)
                    {
                        rc.TypeId = item1.TypeId;
                        rc.TypeName = dbContext.BA_SysEnType.FirstOrDefault(p => p.Id == item1.TypeId)?.Name;
                        var Names = dbContext.v_Jusoft_KSConfirm.Where(p => p.TypeId == item1.TypeId).Select(p => new
                        {
                            p.Name
                        });
                        rc.Name = new List<string>();
                        foreach (var PersonName in Names)
                        {
                            rc.Name.Add(PersonName.Name);
                        }
                    }
                    if (!listRc.Contains(rc))
                    {
                        listRc.Add(rc);
                    }
                }
                //var List = listRc.ToPagedList(model.PageIndex, model.PageSize);
                rs.Data = listRc;
                //var List = ListData;
                //rs.Data = List;
                //rs.HasNextPage = List.HasNextPage;
                //rs.PageCount = List.PageCount;
                //rs.TotalItemCount = List.TotalItemCount;
                //rs.PageIndex = model.PageIndex;
                return Json(rs);
            }
            catch (Exception ex)
            {
                return Json(new { Code = 10001, strResult = ex.Message });
            }
        }
        [HttpPost]
        public object ConfirInfo(ConfirInfoModel model)
        {
            Result rs = new Result();
            try
            {
                if (model.Type == null)
                {
                    return Json(new { Code = 1000, strResult = "请输入客诉类型！" });
                }
                var KsConfirmPerson = dbContext.KS_Confirm.Where(p => p.TypeId == model.Type).Select(p => p.IdPerson).ToList();
                //获取数据库中查询出来的人员和
                foreach (var item in KsConfirmPerson.Except(model.IdPerson))
                {
                    var Person = dbContext.KS_Confirm.FirstOrDefault(p => p.TypeId == model.Type && p.IdPerson == item);
                    if (Person != null)
                    {
                        dbContext.KS_Confirm.Remove(Person);
                    }
                    dbContext.SaveChanges();
                }
                foreach (var item in model.IdPerson.Except(KsConfirmPerson))
                {
                    var KsConfirm = dbContext.KS_Confirm.FirstOrDefault(p => p.TypeId == model.Type && p.IdPerson == item);
                    if (KsConfirm == null)
                    {
                        KS_Confirm Confirm = new KS_Confirm
                        {
                            IdPerson = Convert.ToInt32(item),
                            TypeId = Convert.ToInt32(model.Type)
                        };
                        dbContext.KS_Confirm.Add(Confirm);
                    }
                    dbContext.SaveChanges();
                }
                rs.strResult = "修改成功！";
                rs.Code = 0;
                return Json(rs);
            }
            catch (Exception ex)
            {
                return Json(new { Code = 1001, strResult = ex.Message });
            }
        }
        #endregion
    }
}

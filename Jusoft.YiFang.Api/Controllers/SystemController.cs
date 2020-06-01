using DingTalk.Api;
using DingTalk.Api.Request;
using DingTalk.Api.Response;
using Jusoft.YiFang.Api.Models;
using Jusoft.YiFang.Db;
using Jusoft.YiFang.Db.Extensions;
using Jusoft.YiFang.Db.ThirdSystem;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using System.Web.Mvc;
using X.PagedList;

namespace Jusoft.YiFang.Api.Controllers
{
    [Authorize]
    public class SystemController : Controller
    {
        YiFang_CustomerComplaintEntities dbContext = new YiFang_CustomerComplaintEntities();

        private readonly  string One = ConfigurationManager.AppSettings["One"];
        private readonly  string Two = ConfigurationManager.AppSettings["Two"];
        private readonly  string Three = ConfigurationManager.AppSettings["Three"];
        private readonly  string Four = ConfigurationManager.AppSettings["Four"];
        private readonly  string Five = ConfigurationManager.AppSettings["Five"];

        #region 微信解绑
        public object WeChatExit()
        {
            try
            {
                //获取当前登录者信息
                var person = dbContext.OR_Person.FirstOrDefault(k => k.LoginName == User.Identity.Name);
                if (person == null)
                {
                    return this.JsonRespData(1000, "登录者不存在，请重新登录");
                }
                person.WeChatOpenId = null;
                dbContext.SaveChanges();
                return this.JsonRespData("退出成功");
            }
            catch (Exception ex)
            {

                return this.JsonRespData(1000, ex.ToString());
            }
        }
        #endregion

        #region 登录门店信息
        [HttpPost]
        public object loginStore()
        {
            try
            {
                //获取当前登录者信息
                var person = dbContext.OR_Person.FirstOrDefault(k => k.LoginName == User.Identity.Name);
                if (person == null)
                {
                    return this.JsonRespData(1000, $"登录者不存在，请重新登录");
                }
                var dbdata = person.REF_STORE.FirstOrDefault();
                if (dbdata == null)
                {
                    return this.JsonRespData(1000, "当前用户不存在门店");
                }
                string url = HttpContext.Request.Url.ToString().Replace(HttpContext.Request.Url.PathAndQuery, ""); //服务器协议+域名+端口
                string path = "/Store/head.png";
                string fullpath = url + path;
                var data = new
                {
                    UserName = person.Name,
                    dbdata.Code,
                    dbdata.Name,
                    dbdata.Phone,
                    dbdata.JoinPerson,
                    dbdata.Address,
                    dbdata.RegionId,
                    RegionName = dbContext.BA_SysEnType.FirstOrDefault(k => k.Id == dbdata.RegionId)?.Name,
                    dbdata.IdSupervisor,
                    SupervisorName = dbContext.OR_Person.FirstOrDefault(k => k.Id == dbdata.IdSupervisor)?.Name,//督导名称
                    dbdata.IdPersonRegion,
                    PersonRegionName = dbContext.OR_Person.FirstOrDefault(k => k.Id == dbdata.IdPersonRegion)?.Name,//大区负责人名称
                    HeadPhoto = GetHead(person.Id) ?? fullpath
                };
                return this.JsonRespData(data);
            }
            catch (Exception ex)
            {
                return this.JsonRespData(1000, ex.ToString());
            }
        }
       /// <summary>
       /// 进入我的
       /// </summary>
       /// <returns></returns>
        [HttpPost]
        public object loginmine()
        {
            try
            {
                //获取当前登录者信息
                var person = dbContext.OR_Person.FirstOrDefault(k => k.LoginName == User.Identity.Name);
                if (person == null)
                {
                    return this.JsonRespData(1000, $"登录者不存在，请重新登录");
                }
                string url = HttpContext.Request.Url.ToString().Replace(HttpContext.Request.Url.PathAndQuery, ""); //服务器协议+域名+端口
                string path = "/Store/head.png";
                string fullpath = url + path;
                var data = new
                {
                    UserName = person.Name,
                    Phone=person.PsnMobilePhone,
                    HeadPhoto = GetHead(person.Id) ?? fullpath
                };
                return this.JsonRespData(data);
            }
            catch (Exception ex)
            {
                return this.JsonRespData(1000, ex.ToString());
            }
        }
        //[HttpPost]
        //public object LoginHome()
        //{
        //    var user = dbContext.AC_SysUsers.FirstOrDefault(p => p.UserName == User.Identity.Name);
        //    if (user == null)
        //    {
        //        return this.JsonRespData(1000, "登录者不存在，请重新登录");
        //    }
        //    string url = HttpContext.Request.Url.ToString().Replace(HttpContext.Request.Url.PathAndQuery, ""); //服务器协议+域名+端口
        //    string path = "/Store/";
        //    string fullpath = url + path;
        //    //获取该用户的首页菜单
        //    var ss = user.AUTH_User_Menu.Select(p => p.IdSysMenu).ToList();
        //    var person= dbContext.OR_Person.FirstOrDefault(p => p.LoginName == User.Identity.Name);
        //    //获取需要的信息
        //    var list = dbContext.BA_SysMenu.Where(p => ss.Contains(p.Id)).ToList().Select(s => new
        //    {
        //        s.Name,//菜单名称
        //        s.Url,//菜单的vue路径
        //        Img = fullpath + s.Url+".png",//首页图片
        //        Self= CustServiceList(s.Name,person.Id)
        //    });
        //    return this.JsonRespData(list);
        //}

        [HttpPost]
        public object LoginHome()
        {
            try
            {
                var user = dbContext.AC_SysUsers.FirstOrDefault(p => p.UserName == User.Identity.Name);
                if (user == null)
                {
                    return this.JsonRespData(1000, "登录者不存在，请重新登录");
                }
                if (string.IsNullOrWhiteSpace(One)|| string.IsNullOrWhiteSpace(Two)|| string.IsNullOrWhiteSpace(Three))
                {
                    return this.JsonRespData(1000, "登陆失败请联系管理员");
                }
                LogHelper.WriteLog(One);
                LogHelper.WriteLog(Two);
                LogHelper.WriteLog(Three);
                //获取该用户的首页菜单
                var ss = user.AUTH_User_Menu.Select(p => p.IdSysMenu).ToList();
                var person = dbContext.OR_Person.FirstOrDefault(p => p.LoginName == User.Identity.Name);

                string url = HttpContext.Request.Url.ToString().Replace(HttpContext.Request.Url.PathAndQuery, ""); //服务器协议+域名+端口
                string path = "/Store/";
                string fullpath = url + path;
                HashSet<object> hash = new HashSet<object>();
                //p => p.Name == One || p.Name == Two || p.Name == Three
                var role = user.AC_SysRoles.SelectMany(p => p.BA_SysMenu).Where(w=>w.Url!=null&&w.Url!="").Select(s => new
                {
                    s.Name,
                    s.Url,//菜单的vue路径
                    Img = fullpath + s.Url + ".png",//首页图片
                    //Self = CustServiceList(s.Name, person.Id)
                    Num=Num(s.Name)
                });
                foreach (var item in role)
                {
                    hash.Add(item);
                }
                return this.JsonRespData(hash);
            }
            catch (Exception ex)
            {

                return this.JsonRespData(1000, ex.Message);
            }

        }
        [HttpPost]
        public object PCLoginHome()
        {
            try
            {
                var user = dbContext.AC_SysUsers.FirstOrDefault(p => p.UserName == User.Identity.Name);
                if (user == null)
                {
                    return this.JsonRespData(1000, "登录者不存在，请重新登录");
                }
                

                //获取该用户的首页菜单
                var ss = user.AUTH_User_Menu.Select(p => p.IdSysMenu).ToList();
                var person = dbContext.OR_Person.FirstOrDefault(p => p.LoginName == User.Identity.Name);

                //var role = user.AC_SysRoles.SelectMany(p => p.BA_SysMenu).Select(s => new
                //{
                //    s.Name,
                //    Self = CustServiceList(s.Name, person.Id)
                //}) ;
                //获取需要的信息
                var list = dbContext.BA_SysMenu.Where(p => ss.Contains(p.Id)).ToList().Select(s => new
                {
                    s.Name,
                    Self = CustServiceList(s.Name, person.Id)
                });
                return this.JsonRespData(list);
            }
            catch (Exception ex)
            {

                return this.JsonRespData(1000, ex.Message);
            }

        }
        [HttpPost]
        public object PCLoginHome2()
        {
            try
            {
                var user = dbContext.AC_SysUsers.FirstOrDefault(p => p.UserName == User.Identity.Name);
                if (user == null)
                {
                    return this.JsonRespData(1000, "登录者不存在，请重新登录");
                }
                //获取该用户的首页菜单
                var ss = user.AUTH_User_Menu.Select(p => p.IdSysMenu).ToList();
                var person = dbContext.OR_Person.FirstOrDefault(p => p.LoginName == User.Identity.Name);

                var role = user.AC_SysRoles.SelectMany(p => p.BA_SysMenu).Where(p => p.Name == One || p.Name == Two || p.Name == Three).Select(s => new
                {
                    s.Name,
                    Self = CustServiceList(s.Name, person.Id)
                });
                return this.JsonRespData(role);
            }
            catch (Exception ex)
            {

                return this.JsonRespData(1000, ex.Message);
            }

        }
        [HttpPost]
        public object PCLookType()
        {
            try
            {
                //设置查询 语句
                string sql = "SELECT a.StateId,a.EnTypeName,COUNT(a.StateId) AS Number FROM  (SELECT a.StateId,ISNULL(b.Name,'默认类型') as EnTypeName FROM  Ks_Customer a inner join BA_SysEnType b  ON b.Id=a.StateId) AS a GROUP BY a.StateId,a.EnTypeName ORDER BY Number DESC  ";
                //转换为list
                var list = dbContext.Database.SqlQuery<NumberModel>(sql).ToList();
                //存在的类型id
                var idtos = list.Select(p => p.StateId).ToList();

                string sql2 = " SELECT Id FROM  dbo.BA_SysEnType WHERE IdParent=1";
                var list2 = dbContext.Database.SqlQuery<decimal?>(sql2).ToList();
                //查询一个id
                var typelist = dbContext.BA_SysEnType.Where(p => list2.Contains(p.IdParent) && !idtos.Contains(p.Id)).Select(p => new { p.Id, p.Name }).OrderBy(p => p.Id).ToList();
                //设定初始值
                var i = list.Count;
                foreach (var item in typelist)
                {
                    if (i == 9)//看板最多显示九个
                    {
                        break;
                    }
                    NumberModel number = new NumberModel
                    {
                        EnTypeName = item.Name,
                        StateId = item.Id,
                        Number = 0
                    };
                    list.Add(number);
                    i++;
                }
                return this.JsonRespData(list);
            }
            catch (Exception ex)
            {

                return this.JsonRespData(1000, ex.Message);
            }
        }
        public class NumberModel
        {
            public decimal? StateId { get; set; }
            public string EnTypeName { get; set; }
            public int Number { get; set; }
        }
        public int Num(string name)
        {
           
            if (name==One)
            {
                return 1;
            }
            if (name==Two)
            {
                return 2;
            }
            else
            {
                return 0;
            }
            //switch (name)
            //{
            //    case One:
            //        return 1;
            //    case "我要处理":
            //        return 2;
            //    case "我的":
            //        return 3;
            //    default:
            //        return 0;
            //}
        }
        public object CustServiceList(string name, decimal personId)
        {
            DateTime now = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd"));
            DateTime tomorrow = Convert.ToDateTime(DateTime.Now.AddDays(1).ToString("yyyy-MM-dd"));
            //zhuangtai 
            List<byte?> stateList = new List<byte?>();
            List<int?> numList = new List<int?>();
            //switch (name)
            //{
            //    case "我要提问":
            //        stateList = dbContext.KS_Customer.Where(p => p.IdPerson == personId).Where(p => p.CreateTime > now && p.CreateTime <= tomorrow).Select(p => p.State).ToList();
            //        //numList = dbContext.KS_Customer.Where(p => p.IdPerson == personId).Where(p => p.CreateTime > now && p.CreateTime <= tomorrow).Select(p => p.Number).ToList();
            //        break;
            //    case "我要处理":
            //        Expression<Func<v_Ks_DataList, bool>> where = PredicateExtensions.True<v_Ks_DataList>();
            //        //时间
            //        where = where.And(p => p.CreateTime > now && p.CreateTime <= tomorrow && p.IdPersonSupervision == personId);
            //        stateList = dbContext.v_Ks_DataList.Where(where).Select(p => p.State).ToList();
            //        //numList = dbContext.v_Ks_DataList.Where(where).Select(p => p.Number).ToList();
            //        break;
            //    case "我的":
            //        Expression<Func<v_Ks_DataList, bool>> where2 = PredicateExtensions.True<v_Ks_DataList>();
            //        //时间
            //        where2 = where2.And(p => p.CreateTime > now && p.CreateTime <= tomorrow && p.IdPersonSupervision == personId);
            //        stateList = dbContext.v_Ks_DataList.Where(where2).Select(p => p.State).ToList();
            //        //numList = dbContext.v_Ks_DataList.Where(where2).Select(p => p.Number).ToList();
            //        break;
            //}
            if (name==One)
            {
                stateList = dbContext.KS_Customer.Where(p => p.IdPerson == personId).Where(p => p.CreateTime > now && p.CreateTime <= tomorrow).Select(p => p.State).ToList();
            }
            if (name==Two)
            {
                Expression<Func<v_Ks_DataList, bool>> where = PredicateExtensions.True<v_Ks_DataList>();
                //时间
                where = where.And(p => p.CreateTime > now && p.CreateTime <= tomorrow && p.IdPersonSupervision == personId);
                stateList = dbContext.v_Ks_DataList.Where(where).Select(p => p.State).ToList();
            }
            if (name==Three)
            {
                Expression<Func<v_Ks_DataList, bool>> where2 = PredicateExtensions.True<v_Ks_DataList>();
                //时间
                where2 = where2.And(p => p.CreateTime > now && p.CreateTime <= tomorrow && p.IdPersonSupervision == personId);
                stateList = dbContext.v_Ks_DataList.Where(where2).Select(p => p.State).ToList();
            }

            var Untreated = stateList.Where(p => p == 0).Count();//未处理的数量
            var Processing = stateList.Where(p => p == 1).Count();//同意的数量
            var Endover = stateList.Where(p => p == 2).Count();//驳回的数量
            var obj = new
            {
                Untreated,
                Processing,
                Endover
            };
            return obj;
        }
        #endregion
        /// <summary>
        /// 获取用户头像
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public string GetHead(decimal id)
        {
            //OR_Person oR_Person = new OR_Person();

            //var s= dbContext.Entry(oR_Person).OriginalValues
            var info = dbContext.OR_Person.FirstOrDefault(p => p.Id == id);
            if (info == null) return null;
            else return info.HeadUrl;
        }
        /// <summary>
        /// 获取用户信息
        /// </summary>
        /// <returns></returns>
        public string GetUserInfo()
        {
            IDingTalkClient client = new DefaultDingTalkClient("https://oapi.dingtalk.com/user/get");
            OapiUserGetRequest request = new OapiUserGetRequest();
            request.Userid = User.Identity.Name;
            request.SetHttpMethod("GET");
            OapiUserGetResponse response = client.Execute(request, AccessToken.GetAccessToken());
            if (response.Errcode == 0)
            {
                return response.Avatar;

            }
            else
            {
                return null;
            }
        }

        public int GetName(string name)
        {
            switch (name)
            {
                case "普通用户":
                    return 1;
                case "客服":
                    return 2;
                default:
                    return 0;
            }
        }

        [HttpPost]
        public object CustServiceList(CustomerCondition2 condition)
        {
            //当前人
            var Person = dbContext.OR_Person.FirstOrDefault(p => p.LoginName == User.Identity.Name);
            if (!dbContext.OR_Person.IsExistencePerson(User.Identity.Name))
            {
                return this.JsonRespData(1000, "登录者不存在，请重新登录");
            }
            var user = dbContext.AC_SysUsers.FirstOrDefault(p => p.UserName == User.Identity.Name);
            if (user == null)
            {
                return this.JsonRespData(1000, "登录者不存在，请重新登录");
            }
            if (string.IsNullOrWhiteSpace(One) || string.IsNullOrWhiteSpace(Two) || string.IsNullOrWhiteSpace(Three))
            {
                return this.JsonRespData(1000, "登陆失败请联系管理员");
            }
            LogHelper.WriteLog(One);
            LogHelper.WriteLog(Two);
            LogHelper.WriteLog(Three);
            //获取该用户的首页菜单
            var ss = user.AUTH_User_Menu.Select(p => p.IdSysMenu).ToList();
            var person = dbContext.OR_Person.FirstOrDefault(p => p.LoginName == User.Identity.Name);


            //p => p.Name == One || p.Name == Two || p.Name == Three
            var role = user.AC_SysRoles.ToList().Select(p => new
            {
                Type= GetName(p.Name),
                Self= Many(person.Id, GetName(p.Name), condition.StartTime)
            }).OrderByDescending(p=>p.Type).FirstOrDefault();
            return this.JsonRespData(role);
            //var data = new List<byte?>();
            ////var dbdata = dbContext.v_Jusoft_CustomerList.Where(k => k.IdPersonSupervision == Person.Id);
            ////Expression<Func<v_Ks_DataList, bool>> where = p => string.IsNullOrEmpty(condition.SearchKey);
            //if (condition.Num == 0)
            //{
            //    //where = where.And(p => p.IdPerson == null);//督导Id 为当前用户
            //}
            //if (condition.Num == 1)
            //{
            //    data= dbContext.v_Kf_DataList.Where(p => p.IdPerson == Person.Id).Select(p=>p.State).ToList();//督导Id 为当前用户
            //}
            //if (condition.Num == 2)
            //{
            //    //where = where.And(p => p.IdPersonSupervision == Person.Id);//督导Id 为当前用户
            //    data = dbContext.v_Kf_DataList.Where(p => p.CreateTime.ToString("yyyy-MM") == condition.StartTime.ToString("yyyy-MM")&&p.ManagerId==Person.Id).Select(p=>p.State).ToList();
            //}
            ////var data = dbContext.v_Ks_DataList.Where(where).ToList();
            //////where = where.And(p => p.CreateTime.ToString("yyyy-MM").Contains(xx));
            ////data.Select(p => new
            ////{
            ////    p.State,
            ////    CreateTime = p.CreateTime.ToString("yyyy-MM")

            ////}).Where(p => p.CreateTime.Contains(condition.StartTime.ToString("yyyy-MM")));
            //LogHelper.WriteLog(condition.StartTime.ToString());
            //LogHelper.WriteLog(condition.Num.ToString());
            ////var list2= dbContext.v_Jusoft_CustomerList.Where(p => p.IdPersonSupervision == Person.Id).ToList();
            //var Untreated = data.Where(k => k== 0).Count();//待处理的数量
            //var Processing = data.Where(k => k== 1).Count();//处理中的数量
            //var Endover = data.Where(k => k== 2).Count();//结束的数量
            //var AllCount = data.Where(k => true).Count();//所有的数量
            ////返回类型
            //DdTypeCount Pdrs = new DdTypeCount();
            //Pdrs.Untreated = Untreated;//未处理的数量
            //Pdrs.Processing = Processing;//同意的数量
            //Pdrs.Endover = Endover;//驳回的数量
            //Pdrs.AllCount = AllCount;//所有的数量

        }
        public object Many(decimal Id,int Num,DateTime StartTime)
        {
            var data = new List<byte?>();
            //var dbdata = dbContext.v_Jusoft_CustomerList.Where(k => k.IdPersonSupervision == Person.Id);
            //Expression<Func<v_Ks_DataList, bool>> where = p => string.IsNullOrEmpty(condition.SearchKey);
            if (Num == 0)
            {
                //where = where.And(p => p.IdPerson == null);//督导Id 为当前用户
                data.Add(0);
            }
            if (Num == 1)
            {
                data = dbContext.v_Kf_DataList.Where(p => p.IdPerson == Id).Select(p => p.State).ToList();//督导Id 为当前用户
            }
            if (Num == 2)
            {
                //where = where.And(p => p.IdPersonSupervision == Person.Id);//督导Id 为当前用户
                data = dbContext.v_Kf_DataList.Where(p=> p.ManagerId == Id).ToList().Where(p => p.CreateTime.ToString("yyyy-MM") == StartTime.ToString("yyyy-MM") ).Select(p => p.State).ToList();
            }
            //var data = dbContext.v_Ks_DataList.Where(where).ToList();
            ////where = where.And(p => p.CreateTime.ToString("yyyy-MM").Contains(xx));
            //data.Select(p => new
            //{
            //    p.State,
            //    CreateTime = p.CreateTime.ToString("yyyy-MM")

            //}).Where(p => p.CreateTime.Contains(condition.StartTime.ToString("yyyy-MM")));
           
            //var list2= dbContext.v_Jusoft_CustomerList.Where(p => p.IdPersonSupervision == Person.Id).ToList();
            var Untreated = data.Where(k => k == 0).Count();//待处理的数量
            var Processing = data.Where(k => k == 1).Count();//处理中的数量
            var Endover = data.Where(k => k == 2).Count();//结束的数量
            var AllCount = data.Where(k => true).Count();//所有的数量
            //返回类型
            DdTypeCount Pdrs = new DdTypeCount();
            Pdrs.Untreated = Untreated;//未处理的数量
            Pdrs.Processing = Processing;//同意的数量
            Pdrs.Endover = Endover;//驳回的数量
            var result = new
            {
                Untreated = Untreated,
                Processing = Processing,
                Endover = Endover
            };
            return result;
        }
    }
}
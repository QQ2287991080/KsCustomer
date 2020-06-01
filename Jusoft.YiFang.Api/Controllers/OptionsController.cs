using Demo.Models;
using Jusoft.YiFang.Api.Help;
using Jusoft.YiFang.Api.Models;
using Jusoft.YiFang.Api.Models.StoreArchives;
using Jusoft.YiFang.Api.Models.SysType;
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
    public class OptionsController : ApiController
    {
        YiFang_CustomerComplaintEntities dbContext = new YiFang_CustomerComplaintEntities();
        /// 地区负责人
        /// </summary>
        /// <param name="name"></param>
        /// <param name="pagesize"></param>
        /// <param name="pageindex"></param>
        /// <returns></returns>
        //[HttpPost]
        //public IHttpActionResult RegionList(string name, int pagesize, int pageindex)
        //{
        //    var storageConnectionString = "XXXXXXX";
        //    var storageAccount = CloudStorageAccount.Parse(storageConnectionString);
        //    var blobClient = storageAccount.CreateCloudBlobClient();

        //    //Gets the folder with the MediaID
        //    CloudBlobContainer videoscontainer = blobClient.GetContainerReference("TestFolder");

        //    CloudBlockBlob videofile = videoscontainer.GetBlockBlobReference(("test" + ".mp4"));


        //    //Save blob contents to a temp video file.
        //    var videostream = new FileStream(@"XXXXXX.mp4", FileMode.Open, FileAccess.Read);
        //    videofile.UploadFromStream(videostream);
        //    videostream.Close();
        //    return JsonResultHelper.JsonResult(list);
        //}

        [HttpGet]
        public IHttpActionResult RegionList2([FromUri]OutputStoreArchiveModel model)
       {
            Expression<Func<OR_Person, bool>> where = p => (string.IsNullOrEmpty(model.Name) || p.Name.Contains(model.Name));
            var list = dbContext.OR_Person.Where(where).Select(p => new
            {
                p.Id,
                p.Name
            }).OrderBy(p => p.Id).ToPagedList(model.PageIndex, model.PageSize);

            return JsonResultHelper.JsonResult(list);
        }

        [HttpGet]
        public IHttpActionResult RegionList([FromUri]OutputStoreArchiveModel model)
        {
            Expression<Func<OR_Person, bool>> where = p => (string.IsNullOrEmpty(model.Name) || p.Name.Contains(model.Name));
            var list = dbContext.OR_Person.Where(where).Select(p => new
            {
                p.Id,
                p.Name
            }).OrderByDescending(p => p.Id).ToPagedList(model.PageIndex,model.PageSize);
            return JsonResultHelper.JsonResult(list);
        }
        [HttpPost]
        public IHttpActionResult ArrPerson(Options options)
        {
            try
            {
                if (options==null)
                {
                    return JsonResultHelper.JsonResult(null);
                }
                else
                {
                    var list = dbContext.OR_Person.Where(p => options.Arr.Contains(p.Id)).Select(p => new
                    {
                        p.Id,
                        p.Name
                    }).ToArray();
                    return JsonResultHelper.JsonResult(list);
                }
            }
            catch (Exception ex)
            {
                return JsonResultHelper.JsonResult(1000, ex.Message);
            }
        }
        [HttpGet]
        public IHttpActionResult GetDataList([FromUri]OutputStoreArchiveModel model)
        {
            string name =model.SearchKey==null?"": model.SearchKey.ToString();
            int id=0;
            if (model.SearchKey!=null)
            {
                int.TryParse(model.SearchKey.ToString(), out id);
            }
            Expression<Func<v_KsType, bool>> where = p => (string.IsNullOrEmpty(name)|| p.Name.Contains(name))||
            (string.IsNullOrEmpty(name) || p.ParentName.Contains(name))||
            (string.IsNullOrEmpty(name)||p.Memo.Contains(name))||
            (string.IsNullOrEmpty(name)||p.Id==id);
            
            var list = dbContext.v_KsType.Where(where).Where(where).ToList().Select(p => new
            {
                p.Id,
                p.Name,
                p.IdParent,
                p.ParentName,
                p.Memo
            }).OrderByDescending(p => p.Id).ToPagedList(model.PageIndex, model.PageSize);
            return JsonResultHelper.JsonResult(list);
        }

        public static List<decimal?> ToDecimal(List<decimal> xx)
        {
            List<decimal?> dd = new List<decimal?>();
            for (int i = 0; i < xx.Count; i++)
            {
                decimal? a = xx[i];
                dd.Add(a);
            }
            return dd;
        }
        [HttpPost]
        public IHttpActionResult Add(InputSysTypeModel model)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(model.Name))
                {
                    //返回错误
                    return JsonResultHelper.JsonResult(1000, "名字必填");
                }
                else
                {
                    if (dbContext.BA_SysEnType.Any(p => p.Name == model.Name))
                    {
                        return JsonResultHelper.JsonResult(1000, "该类型已存在");
                        LogHelper.WriteLog("该类型已存在");
                    }
                    else
                    {
                        //LogHelper.WriteLog("父级Id："+model.IdParent);
                        
                        decimal par = 1;
                        if (model.IdParent != 0)
                        {
                            par = model.IdParent;
                        }
                        string sql = string.Format("INSERT  dbo.BA_SysEnType(IdParent, Name, Memo )VALUES ({0},'{1}','{2}')", par, model.Name, model.Memo);


                        LogHelper.WriteLog("到底啥原因");
                        dbContext.Database.ExecuteSqlCommand(sql);
                        var info = dbContext.BA_SysEnType.FirstOrDefault(p => p.Name==model.Name);
                        //新增类型人员维护
                        if (model.PersonList!=null)
                        {
                            foreach (var item in model.PersonList)
                            {
                                dbContext.KS_Confirm.Add(new KS_Confirm
                                {
                                    IdPerson = item,
                                    TypeId = info.Id
                                });
                                dbContext.SaveChanges();
                            }
                        }
                        return JsonResultHelper.JsonResult(0, "添加成功");
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.WriteLog(ex.ToString());
                return JsonResultHelper.JsonResult(1000, ex.Message,ex);
            }
        }
        [HttpPost]
        public IHttpActionResult Update(InputSysTypeModel model)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(model.Name))
                {
                    //返回错误
                    return JsonResultHelper.JsonResult(1000,"名称不能为空");
                }
                else
                {

                    var info = dbContext.BA_SysEnType.FirstOrDefault(p => p.Id == model.Id);
                    if (info==null)
                    {
                        LogHelper.WriteLog("该类型已不存在,请刷新！");
                        return JsonResultHelper.JsonResult(1000, "该类型已不存在,请刷新！");
                    }
                    else
                    {
                        //首先删除中间表中这个类型数据
                        //var confirm = dbContext.KS_Confirm.Where(p => model.Id == p.TypeId);
                        //dbContext.KS_Confirm.RemoveRange(confirm);
                        //dbContext.SaveChanges();
                        LogHelper.WriteLog("父类");
                        decimal par = 1;
                        if (model.IdParent != 0)
                        {
                            par = model.IdParent;
                        }
                        string sql = $"DELETE FROM KS_Confirm WHERE   TypeId={model.Id}";
                        dbContext.Database.ExecuteSqlCommand(sql);
                        LogHelper.WriteLog("父类2");
                        info.IdParent = par;
                        info.Memo = model.Memo;
                        info.Name = model.Name;
                        dbContext.SaveChanges();
                        //新增类型人员维护
                        if (model.PersonList != null)
                        {
                            foreach (var item in model.PersonList)
                            {
                                dbContext.KS_Confirm.Add(new KS_Confirm
                                {
                                    IdPerson = item,
                                    TypeId = info.Id
                                });
                                dbContext.SaveChanges();
                            }
                        }
                        LogHelper.WriteLog("父类3");
                        return JsonResultHelper.JsonResult(0, "修改成功");
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.WriteLog(ex.Message);
                return JsonResultHelper.JsonResult(1000, ex.Message);
            }
        }
        [HttpGet]
        public IHttpActionResult Details(decimal Id)
        {
            var info = dbContext.BA_SysEnType.FirstOrDefault(p => p.Id == Id);
            if (info==null)
            {
                return JsonResultHelper.JsonResult(1000, "该信息已不存在");
            }
            else
            {
                var data = new
                {
                    info.Name,
                    info.Id,
                    ParentName = dbContext.BA_SysEnType.Where(p => p.Id == info.IdParent).FirstOrDefault().Name,
                    info.Memo,
                    info.IdParent,
                    PersonList = dbContext.KS_Confirm.Where(p => p.TypeId == info.Id).Select(p => p.IdPerson).ToList()
                };
                return JsonResultHelper.JsonResult(data);
            }
        }
        [HttpGet]
        public IHttpActionResult Delete(decimal Id)
        {
            var info = dbContext.BA_SysEnType.FirstOrDefault(p => p.Id == Id);
            if (info==null)
            {
                return JsonResultHelper.JsonResult(1000, "该信息已不存在");
            }
            else
            {
                List<BA_SysEnType> bA_SysEnTypes = new List<BA_SysEnType>();
                bA_SysEnTypes.Add(info);
                bA_SysEnTypes.AddRange(dbContext.BA_SysEnType.Where(p => p.IdParent == info.Id));
                dbContext.BA_SysEnType.RemoveRange(bA_SysEnTypes);
                //删除中间表中这个类型数据
                var confirm = dbContext.KS_Confirm.Where(p =>info.Id== p.TypeId);
                dbContext.KS_Confirm.RemoveRange(confirm);
                dbContext.SaveChanges();
                return JsonResultHelper.JsonResult(0, "删除成功");
            }
        }
        [HttpPost]
        public IHttpActionResult AllDataList()
        {
            try
            {
                var list= dbContext.v_KsType.Select(p=>new
                {
                    p.Id,
                    p.Name,
                    p.IdParent,
                    p.Memo
                }).ToList();
                return JsonResultHelper.JsonResult(0,"成功", list);
            }
            catch (Exception ex)
            {
                return JsonResultHelper.JsonResult(1000,ex.Message);
            }
        }


        /// <summary>
        /// 返回全部人员下拉列表
        /// </summary>
        /// <param name="SearchKey"></param>
        /// <returns></returns>
        [HttpGet]
        public IHttpActionResult NoPagePerson(string SearchKey)
        {
            var list = dbContext.OR_Person.Where(p => string.IsNullOrEmpty(SearchKey) || p.Name.Contains(SearchKey)).Select(p => new
            {
                p.Id,
                p.Name
            }).ToList();
            return JsonResultHelper.JsonResult(0, "OK", list);
        }
        /// <summary>
        /// 获取客服信息
        /// </summary>
        /// <param name="SearchKey"></param>
        /// <returns></returns>
        [HttpPost]
        public IHttpActionResult GetCustOption(PersonOptionModel model)
        {
            var list = (from p in this.dbContext.OR_Person
                        where string.IsNullOrEmpty(model.SearchKey) || p.Name.Contains(model.SearchKey)
                        select p).ToList<OR_Person>();
            var oldCust = dbContext.KS_Confirm.Where(p=>p.TypeId==model.Id).Select(p=>p.IdPerson);
            //拿到当前客服的现在存在的客户
            list = list.Where(p => oldCust.Contains(p.Id)).ToList();
            var data = list.Select(p => new
            {
                p.Id,
                p.Name
            }).OrderByDescending(p => p.Id).ToPagedList(model.PageIndex, model.PageSize);
            return JsonResultHelper.JsonResult(data);
        }

        /// <summary>
        /// 获取客户，如果选择全选的话显示当前用已存在的
        /// </summary>
        /// <param name="SearchKey"></param>
        /// <returns></returns>
        [HttpGet]
        public IHttpActionResult GetCustomerOption(string SearchKey, int PageSize, int PageIndex, decimal? PersonId)
        {
            var list = (from p in this.dbContext.v_KsType
                        //where string.IsNullOrEmpty(SearchKey) || p.TypeId.Contains(SearchKey) || p.CustName.Contains(SearchKey)
                        select p).ToList<v_KsType>();
            if (PersonId.HasValue)
            {
                var oldCust = dbContext.KS_Confirm.Where(p => p.IdPerson == PersonId).Select(p=>p.TypeId);
                //拿到当前客服的现在存在的客户
                list = list.Where(p => oldCust.Contains(p.Id)).ToList();
            }
            var data = list.Select(p => new {
                p.Id,
                p.Name
            }).OrderByDescending(p => p.Id).Where(p=>string.IsNullOrEmpty(SearchKey) || p.Name.Contains(SearchKey)).ToPagedList(PageIndex, PageSize);
            return JsonResultHelper.JsonResult(data);
        }

        /// <summary>
        /// 单选或取消
        /// </summary>
        /// <param name="PersonId"></param>
        /// <param name="CustomerId"></param>
        /// <param name="IsCheck"></param>
        /// <returns></returns>
        [HttpGet]
        public IHttpActionResult CheckBox(decimal PersonId, decimal TypeId, bool IsCheck)
        {

            try
            {

                OR_Person oR_Person = dbContext.OR_Person.FirstOrDefault(p => p.Id == PersonId);
                //bool flag = oR_Person == null;
                if (oR_Person == null)
                {
                    return JsonResultHelper.JsonResult(1000, "该客服不存在，请刷新！", null);
                }

                var _sysType = dbContext.BA_SysEnType.FirstOrDefault(p => p.Id == TypeId);
                if (_sysType == null)
                {
                    return JsonResultHelper.JsonResult(1000, "该客服不存在，请刷新！", null);
                }
                if (IsCheck)
                {
                    if (dbContext.KS_Confirm.Any(p=>p.TypeId==TypeId&&p.IdPerson==PersonId))
                    {
                        return JsonResultHelper.JsonResult(1000, "该客服已绑定改类型，请刷新！", null);
                    }
                    dbContext.KS_Confirm.Add(new KS_Confirm
                    {
                        TypeId = TypeId,
                        IdPerson = PersonId
                    });//表中新增一条数据
                    dbContext.SaveChanges();
                }
                else
                {
                    var info = dbContext.KS_Confirm.FirstOrDefault(p => p.IdPerson == PersonId && p.TypeId == TypeId);//删除表中数据
                    dbContext.KS_Confirm.Remove(info);
                    dbContext.SaveChanges();
                }
                return JsonResultHelper.JsonResult(0, "操作成功！", null);
            }
            catch (Exception ex)
            {
                return JsonResultHelper.JsonResult(1000, ex.Message, null);
            }
        }

        /// <summary>
        /// 全选
        /// </summary>
        /// <param name="PersonId"></param>
        /// <param name="All"></param>
        /// <returns></returns>
        [HttpGet]
        public IHttpActionResult AllNewCustomer(decimal PersonId, bool All)
        {

            try
            {
                OR_Person oR_Person = dbContext.OR_Person.FirstOrDefault(p => p.Id == PersonId);
                if (oR_Person == null)
                {
                    return JsonResultHelper.JsonResult(1000, "该客服不存在，请刷新！", null);
                }
               
                if (All)
                {
                    //拿到当前客服的现在存在的类型
                    var oldCust = dbContext.KS_Confirm.Where(p => p.IdPerson == PersonId).Select(p => p.TypeId).ToList();
                    //过滤不需要新增的类型
                    var CustomerLis = dbContext.v_KsType.Select(p => p.Id).ToList();
                    var newCust = CustomerLis.Where(p => !oldCust.Contains(p)).ToList();
                    if (newCust.Count() == 0) //如果没有数据就不走下面了。
                    {
                        return JsonResultHelper.JsonResult(0, "操作成功");
                    }                      
                    string add = "";
                    newCust.ForEach(item =>
                    {
                        add += string.Format(@"INSERT dbo.KS_Confirm(IdPerson,TypeId) VALUES ({0}, {1})", PersonId, item);
                    });
                    dbContext.Database.ExecuteSqlCommand(add);
                    return JsonResultHelper.JsonResult(0, "操作成功");
                }
                else
                {
                    string sql = string.Format(@"DELETE  FROM  KS_Confirm WHERE IdPerson={0}", PersonId);
                    dbContext.Database.ExecuteSqlCommand(sql);
                    return JsonResultHelper.JsonResult(0, "操作成功");
                }
            }
            catch (Exception ex)
            {
                return JsonResultHelper.JsonResult(1000, ex.Message, null);
            }
        }
        /// <summary>
        /// 全选新增之新增客服
        /// </summary>
        /// <param name="CustomerId"></param>
        /// <param name="All"></param>
        /// <returns></returns>
        [HttpGet]
        public IHttpActionResult AllNewPerson(decimal TypeId, bool All)
        {
            try
            {
                if (All)
                {
                    //拿到当前客服的现在存在的客户
                    var oldCust = dbContext.KS_Confirm.Where(p => p.TypeId == TypeId).Select(p => p.IdPerson).ToList();
                    //过滤不需要新增的客户
                    var newCust = dbContext.OR_Person.Select(p => p.Id).Where(p => !oldCust.Contains(p)).ToList();
                    if (newCust.Count == 0) ;//如果没有数据就不走下面了。
                                             //新增SQL
                    string add = "";
                    newCust.ForEach(item =>
                    {
                        add += string.Format(@"INSERT dbo.KS_Confirm(IdPerson,TypeId) VALUES ({0}, {1})", item, TypeId);
                    });
                    dbContext.Database.ExecuteSqlCommand(add);
                    return JsonResultHelper.JsonResult(0, "操作成功");
                }
                else
                {
                    string sql = string.Format(@"DELETE  FROM  KS_Confirm WHERE TypeId={0}", TypeId);
                    dbContext.Database.ExecuteSqlCommand(sql);
                    return JsonResultHelper.JsonResult(0, "操作成功");
                }
            }
            catch (Exception ex)
            {
                return JsonResultHelper.JsonResult(1000, ex.Message, null);
            }
        }

        /// <summary>
        /// 客服管理的客户
        /// </summary>
        /// <param name="PersonId"></param>
        /// <returns></returns>
        [HttpGet]
        public IHttpActionResult ExistCustomer(decimal PersonId)
        {

            try
            {
                OR_Person oR_Person = dbContext.OR_Person.FirstOrDefault(p => p.Id == PersonId);
                if (oR_Person == null)
                {
                    return JsonResultHelper.JsonResult(1000, "该客服不存在，请刷新！", null);
                }

                var arr= dbContext.KS_Confirm.Where(p => p.IdPerson == PersonId).Select(p => p.TypeId).ToArray();
                return JsonResultHelper.JsonResult(0, "操作成功", arr);
            }
            catch (Exception ex)
            {
                return JsonResultHelper.JsonResult(1000, "该客服不存在，请刷新！", null);
            }

        }
        /// <summary>
        /// 客户对应的客服
        /// </summary>
        /// <param name="CustomerId"></param>
        /// <returns></returns>
        [HttpGet]
        public IHttpActionResult ExistPerson(decimal TypeId)
        {
            try
            {
                var arr = dbContext.KS_Confirm.Where(p => p.TypeId == TypeId).Select(p => p.IdPerson).ToArray();
                return JsonResultHelper.JsonResult(0, "操作成功", arr);
            }
            catch (Exception ex)
            {
                return JsonResultHelper.JsonResult(1000, "该客服不存在，请刷新！", null);
            }
        }
    }
}

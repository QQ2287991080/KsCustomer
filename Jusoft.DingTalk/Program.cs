using DingTalk.Api;
using DingTalk.Api.Request;
using DingTalk.Api.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Jusoft.DingTalk.Models;
using System.Xml.Linq;
using Newtonsoft.Json;
using static DingTalk.Api.Response.OapiDepartmentListResponse;
using Jusoft.YiFang.Db;
using Jusoft.YiFang.Db.ThirdSystem;

namespace Jusoft.DingTalk
{
    class Program
    {
       
        static void Main(string[] args)
        {
            try
            {
                YiFang_CustomerComplaintEntities dbContext = new YiFang_CustomerComplaintEntities();
                //同步部门信息
                string accessToken =AccessToken.GetAccessToken();
                IDingTalkClient client = new DefaultDingTalkClient("https://oapi.dingtalk.com/department/list");
                OapiDepartmentListRequest request = new OapiDepartmentListRequest();
                //request.Id
                request.SetHttpMethod("GET");
                OapiDepartmentListResponse response = client.Execute(request, accessToken);
                if (response.Errcode!=0)
                {
                    Console.WriteLine("错误原因：【" + response.Errmsg + "】");
                }
                //移除部门-人员信息
                dbContext.Database.ExecuteSqlCommand("delete REF_Person_Department");
                //获取钉钉部门和人员集合
                List<long> DepartmentList = new List<long>();
                List<string> PersonList = new List<string>();
                //获取普通用户
                var nomal= dbContext.AC_SysRoles.FirstOrDefault(p => p.Name == "普通用户");
                if (nomal==null)
                {
                    Console.WriteLine("请维护角色");
                    return;
                }
                 DingTalkDepartmentList(dbContext, 0, response.Department);
                foreach (var item in response.Department.OrderBy(k => k.Id))
                {
                    //部门人员
                    //添加读取该部门所有人员信息
                    var personlist = JsonConvert.DeserializeObject<Jusoft_DepartmentPerson>(HttpRequestHelper.HttpGet("https://oapi.dingtalk.com/user/list?access_token=" + accessToken + "&department_id=" + item.Id));
                    foreach (var user in personlist.userlist)
                    {
                        var person = dbContext.OR_Person.FirstOrDefault(k => k.PsnNum == user.userid);
                        //person.REF_Person_Department.Add(new REF_Person_Department { });
                        if (person == null)
                        {
                            person = new OR_Person { PsnNum = user.userid, Sex = 0 };
                            dbContext.OR_Person.Add(person);
                        }
                        person.Name = user.name;
                        person.CodeDepartment = user.department[0].ToString();
                        person.PsnMobilePhone = user.mobile;
                        person.HeadUrl = user.avatar;
                        person.LoginName = user.userid;
                        person.ExternalBit = false;
                        dbContext.SaveChanges();
                        Console.WriteLine("【" + user.name + "】人员-同步成功");
                        dbContext.REF_Person_Department.Add(new REF_Person_Department
                        {
                            CodeDepartment =item.Id.ToString(),
                            IdPerson=person.Id
                        });

                        //同步信息到账户表
                        if (!dbContext.AC_SysUsers.Any(k => k.UserName == user.userid))
                        {
                            var _newUser = new AC_SysUsers
                            {
                                UserName = user.userid,
                                PasswordType = 1,
                                PasswordHash = user.userid,
                            };
                            dbContext.AC_SysUsers.Add(_newUser);
                            string sql = string.Format("INSERT dbo.REF_User_Roles(IdSysUsers,IdSysRoles)VALUES({0},{1})", _newUser.Id, nomal.Id);
                            Console.WriteLine(sql);
                            dbContext.Database.ExecuteSqlCommand(sql);
                        }

                        dbContext.SaveChanges();
                        //部门及人员 赋值
                        PersonList.Add(user.userid);
                    }
                    DepartmentList.Add(item.Id);

                }
                //离职人员 -(我们系统存在钉钉不存在)
                var employee = dbContext.OR_Person.Where(k=>k.ExternalBit == false).Select(k => k.PsnNum).ToList();
                foreach (var item in employee.Except(PersonList))
                {
                    var yee= dbContext.OR_Person.FirstOrDefault(k => k.PsnNum ==item);
                    yee.CodeDepartment = "1";
                    yee.LeaveDate = DateTime.Now;
                    dbContext.REF_Person_Department.RemoveRange(dbContext.REF_Person_Department.Where(w => w.IdPerson == yee.Id));
                    dbContext.SaveChanges();
                    Console.WriteLine("【" + yee.Name + "】人员-离职成功");
                }
                //删除部门-(我们系统存在钉钉不存在)
                var Deletedepartment = dbContext.OR_Department.Select(k => k.Code).ToList().Select(k=>Convert.ToInt64(k)).ToList();
                foreach (var item in Deletedepartment.Except(DepartmentList).OrderByDescending(k=>k))
                {
                    var delete = dbContext.OR_Department.FirstOrDefault(k => k.Code == item.ToString());
                    dbContext.OR_Department.Remove(delete);
                    dbContext.SaveChanges();
                    Console.WriteLine("【" + delete.Name + "】部门-删除成功");
                }

                Console.WriteLine("同步完成");
            }
            catch (Exception ex)
            {

                Console.WriteLine("【异常】【" + ex.ToString() + "】");
            }
            Console.WriteLine();
        }

        #region 钉钉-部门递归
        public static void DingTalkDepartmentList(YiFang_CustomerComplaintEntities dbContext, long? Id, List<DepartmentDomain> DingTalkDepartment)
        {
            foreach (var item in DingTalkDepartment.Where(k => k.Parentid == Id).ToList())
            {
                var department = dbContext.OR_Department.FirstOrDefault(k => k.Code == item.Id.ToString());
                if (department == null)
                {
                    department = new OR_Department { Code = item.Id.ToString() };
                    dbContext.OR_Department.Add(department);
                }
                department.Name = item.Name;
                department.CodeDepartment = item.Parentid.ToString() == "0" ? null : item.Parentid.ToString();
                department.FullName = item.Name;
                department.CreateTime = DateTime.Now;
                //提交部门信息
                dbContext.SaveChanges();
                Console.WriteLine("【" + item.Name + "】部门-同步成功");


                DingTalkDepartmentList(dbContext, item.Id, DingTalkDepartment);
            }
        }
        #endregion
    }
}

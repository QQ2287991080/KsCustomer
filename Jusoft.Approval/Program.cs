using DingTalk.Api;
using DingTalk.Api.Request;
using DingTalk.Api.Response;
using Jusoft.YiFang.Db;
using Jusoft.YiFang.Db.ThirdSystem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jusoft.Approval
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                YiFang_CustomerComplaintEntities DbContext = new YiFang_CustomerComplaintEntities();
                var list = DbContext.KS_Customer.Where(p => p.DingTalkApproval != null && p.Number == 4).Select(p => new { p.DingTalkApproval, p.Id }).ToList();
                //list.ForEach(ks =>
                //{
                //    var info = DbContext.KS_Customer.FirstOrDefault(p => p.Id == ks.Id);
                //    var number = ApprovalDetails(ks.DingTalkApproval);
                //    info.Number = number;

                //    Console.WriteLine("同步客诉单【" + ks.Id + "】");
                //});
                var person = DbContext.OR_Person.Where(p => p.LeaveDate == null);
                var all = DbContext.KS_Customer_Approval.Where(p => p.state >2);
                foreach (var ks in list)
                {
                    Console.WriteLine("同步客诉单【" + ks.Id + "】");
                    IDingTalkClient client = new DefaultDingTalkClient("https://oapi.dingtalk.com/topapi/processinstance/get");
                    OapiProcessinstanceGetRequest request = new OapiProcessinstanceGetRequest();
                    request.ProcessInstanceId = ks.DingTalkApproval;
                    string token = AccessToken.GetAccessToken();
                    OapiProcessinstanceGetResponse response = client.Execute(request, token);
                    Console.WriteLine(response.Errmsg);
                    if (response.Errcode == 0)
                    {
                       
                        if (response.ProcessInstance.Status == "COMPLETED")
                        {
                            Console.WriteLine("审批单结束");
                            var info = DbContext.KS_Customer.FirstOrDefault(p => p.Id == ks.Id);
                            if (response.ProcessInstance.Result== "agree")
                            {
                                info.Number = 1;
                                info.DeliveryDate = DateTime.Now;
                                info.State = 2;
                            }
                            if (response.ProcessInstance.Result == "refuse")
                            {
                                info.Number = 2;
                                info.DeliveryDate = DateTime.Now;
                                info.State = 2;
                            }
                            DbContext.SaveChanges();
                            Console.WriteLine("审批完成");
                        }
                        //新增审批详情
                        foreach (var u in response.ProcessInstance.Tasks)
                        {

                            if (u.Userid==null)
                            {
                                continue;
                            }
                            Console.WriteLine(u.Userid);
                            var ren = person.FirstOrDefault(p => p.PsnNum == u.Userid);
                            if (ren==null)
                            {
                                continue;
                            }
                            Console.WriteLine(ren.Id.ToString());
                            if (all.Any(p => p.IdCustomer == ks.Id && p.IdPerson == ren.Id))
                            {
                                continue;
                            }

                            if (u.TaskResult == "AGREE")
                            {

                                KS_Customer_Approval kS_Customer = new KS_Customer_Approval
                                {
                                    FinishTime = DateTime.Now,
                                    IdCustomer = ks.Id,
                                    IdPerson = ren.Id,
                                    Name = ren.Name,
                                    state = 3
                                };
                                Console.WriteLine("审批同意");
                                DbContext.KS_Customer_Approval.Add(kS_Customer);
                            }
                            if(u.TaskResult == "REFUSE")
                            {
                                KS_Customer_Approval kS_Customer = new KS_Customer_Approval
                                {
                                    FinishTime = DateTime.Now,
                                    IdCustomer = ks.Id,
                                    IdPerson = ren.Id,
                                    Name = ren.Name,
                                    state = 4
                                };
                                Console.WriteLine("审批拒绝");
                                DbContext.KS_Customer_Approval.Add(kS_Customer);
                            }
                        }
                        Console.WriteLine("成功");
                    }
                }
                DbContext.SaveChanges();
                Console.WriteLine("同步完成");
                //Environment.Exit(0);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
            System.Environment.Exit(0);
        }

        static int ApprovalDetails(string Id)
        {
            IDingTalkClient client = new DefaultDingTalkClient("https://oapi.dingtalk.com/topapi/processinstance/get");
            OapiProcessinstanceGetRequest request = new OapiProcessinstanceGetRequest();
            request.ProcessInstanceId = Id;
            string token = AccessToken.GetAccessToken();
            OapiProcessinstanceGetResponse response = client.Execute(request, token);
            if (response.Errcode==0)
            {
                if (response.ProcessInstance.Status == "COMPLETED") return 1;
                else return 0;
            }
            else return 2;

        }
    }
}

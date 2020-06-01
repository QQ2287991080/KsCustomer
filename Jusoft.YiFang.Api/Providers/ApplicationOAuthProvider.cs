using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.OAuth;
using System.IO;
using System.Web;
using System.Xml;
using Jusoft.YiFang.Db;
using Demo.Models;
using DingTalk.Api;
using DingTalk.Api.Request;
using DingTalk.Api.Response;
using Jusoft.YiFang.Db.ThirdSystem;

namespace Jusoft.YiFang.Api.Providers
{
    public class ApplicationOAuthProvider : OAuthAuthorizationServerProvider
    {
        public override Task ValidateClientAuthentication(OAuthValidateClientAuthenticationContext context)
        {
            context.Validated();
            return base.ValidateClientAuthentication(context);
        }
        public override Task GrantClientCredentials(OAuthGrantClientCredentialsContext context)
        {

            YiFang_CustomerComplaintEntities dbContext = new YiFang_CustomerComplaintEntities();
            try
            {
                //var data = await context.Request.ReadFormAsync();
                var formData = context.Request.ReadFormAsync();
                string Code = formData.Result["Code"];
                string CS = formData.Result["CS"];
                //ClaimsIdentity oAuthIdentity = new ClaimsIdentity(OAuthDefaults.AuthenticationType);
                ////用户名
                //oAuthIdentity.AddClaim(new Claim(ClaimsIdentity.DefaultNameClaimType, "19423657671291041"));
                ////设置授权凭据
                //AuthenticationProperties properties = CreateProperties("19423657671291041");
                //AuthenticationTicket ticket = new AuthenticationTicket(oAuthIdentity, properties);
                //context.Validated(ticket);
                //return base.GrantClientCredentials(context);
                //Code临时授权码为null执行微信登录,不为null执行钉钉登录
                if (!string.IsNullOrEmpty(Code))
                {
                    if (CS=="CS")
                    {
                        ClaimsIdentity oAuthIdentity = new ClaimsIdentity(OAuthDefaults.AuthenticationType);
                        //用户名
                        oAuthIdentity.AddClaim(new Claim(ClaimsIdentity.DefaultNameClaimType, Code));
                        //设置授权凭据
                        AuthenticationProperties properties = CreateProperties(Code);
                        AuthenticationTicket ticket = new AuthenticationTicket(oAuthIdentity, properties);
                        context.Validated(ticket);
                    }
                    else
                    {
                        DefaultDingTalkClient defaultDingTalk = new DefaultDingTalkClient("https://oapi.dingtalk.com/user/getuserinfo");
                        OapiUserGetuserinfoRequest req = new OapiUserGetuserinfoRequest();
                        req.Code = Code;
                        req.SetHttpMethod("GET");
                        OapiUserGetuserinfoResponse execute = defaultDingTalk.Execute(req, AccessToken.GetAccessToken());
                        if (execute.Errcode != 0)
                        {
                            DingTalk.Core.Logs.LogHelper.WriteLog(execute.Body);
                            context.SetError("授权码出错啦或配置错误");
                            return base.GrantClientCredentials(context);
                        }
                        string userid = execute.Userid;
                        var Person = dbContext.OR_Person.FirstOrDefault(p => p.LoginName == userid);
                        if (Person == null)
                        {
                            context.SetError("该人员不在组织中");
                            return base.GrantClientCredentials(context);
                        }
                        ClaimsIdentity oAuthIdentity = new ClaimsIdentity(OAuthDefaults.AuthenticationType);
                        //用户名
                        oAuthIdentity.AddClaim(new Claim(ClaimsIdentity.DefaultNameClaimType, userid));
                        //设置授权凭据
                        AuthenticationProperties properties = CreateProperties(userid);
                        AuthenticationTicket ticket = new AuthenticationTicket(oAuthIdentity, properties);
                        context.Validated(ticket);
                    }
                    //return base.GrantClientCredentials(context);
                    #region 钉钉登录
                    //if (Code=="123")
                    //{
                    //    ClaimsIdentity oAuthIdentity = new ClaimsIdentity(OAuthDefaults.AuthenticationType);
                    //                                                                         //010742350933650042
                    //    oAuthIdentity.AddClaim(new Claim(ClaimsIdentity.DefaultNameClaimType, "010742350933650042"));
                    //    AuthenticationProperties properties = CreateProperties("010742350933650042");
                    //    AuthenticationTicket ticket = new AuthenticationTicket(oAuthIdentity, properties);
                    //    context.Validated(ticket);
                    //    return base.GrantClientCredentials(context);
                    //}

                    //var AccessToken= Jusoft.YiFang.Db.ThirdSystem.AccessToken.GetAccessToken();
                    //if (string.IsNullOrEmpty(AccessToken))
                    //{
                    //    context.SetError("AccessToken", $"Code【{Code}】获取token失败");
                    //    return base.GrantClientCredentials(context);
                    //}
                    //var resUserId= Jusoft.YiFang.Db.ThirdSystem.AccessToken.GetUserId(Code, AccessToken);
                    //if (resUserId.Errcode!=0)
                    //{
                    //    context.SetError("resUserId", $"Code【{Code}】"+resUserId.Errmsg);
                    //    return base.GrantClientCredentials(context);
                    //}
                    //var Person = dbContext.OR_Person.FirstOrDefault(p=>p.LoginName == resUserId.Userid);
                    //if (Person != null)
                    //{
                    //    ClaimsIdentity oAuthIdentity = new ClaimsIdentity(OAuthDefaults.AuthenticationType);
                    //    oAuthIdentity.AddClaim(new Claim(ClaimsIdentity.DefaultNameClaimType, Person.LoginName));
                    //    AuthenticationProperties properties = CreateProperties(Person.LoginName);
                    //    AuthenticationTicket ticket = new AuthenticationTicket(oAuthIdentity, properties);
                    //    context.Validated(ticket);
                    //}
                    //else
                    //{
                    //    context.SetError("Person", $"Code【{Code}】未找到人员,请联系管理员");
                    //    return base.GrantClientCredentials(context);
                    //}
                    #endregion
                }
                else
                {
                    #region 微信登录
                    //string userid = formData.Result["username"];//用户名
                    //string password = formData.Result["password"];//密码
                    //string openid = formData.Result["openid"];//微信openid
                    //                                          //优先校验openid
                    //if (string.IsNullOrEmpty(openid))
                    //{
                    //    context.SetError("invalid_grant", "openid不合法");
                    //    return base.GrantClientCredentials(context);
                    //}
                    //if (!string.IsNullOrEmpty(userid))
                    //{
                    //    var person = dbContext.OR_Person.FirstOrDefault(k => k.LoginName == userid);
                    //    if (person == null)
                    //    {
                    //        context.SetError("1001", "门店账号信息不对，请重新输入");
                    //        return base.GrantClientCredentials(context);
                    //    }
                    //    else if (!string.IsNullOrEmpty(person.WeChatOpenId))
                    //    {
                    //        context.SetError("1001", $"门店账号信息已绑定用户，请联系管理员操作");
                    //        return base.GrantClientCredentials(context);
                    //    }
                    //    if (!dbContext.AC_SysUsers.Any(k => k.UserName == userid && k.PasswordHash == password))
                    //    {
                    //        context.SetError("1001", "门店密码信息不对，请重新输入");
                    //        return base.GrantClientCredentials(context);
                    //    }
                    //    person.WeChatOpenId = openid;
                    //    dbContext.SaveChanges();
                    //}
                    //else
                    //{
                    //    var person = dbContext.OR_Person.FirstOrDefault(k => k.WeChatOpenId == openid);
                    //    if (person == null)
                    //    {
                    //        context.SetError("1002", "用户还未绑定账号，请先绑定");
                    //        return base.GrantClientCredentials(context);
                    //    }
                    //    userid = person.LoginName;
                    //}

                    ////TODO: 校验该用户是否存在与我们自身的系统之中，若存在，则正常加入凭据信息
                    //var oAuthIdentity = new ClaimsIdentity(context.Options.AuthenticationType);
                    //oAuthIdentity.AddClaim(new Claim(ClaimTypes.Name, userid));
                    //var ticket = new AuthenticationTicket(oAuthIdentity, new AuthenticationProperties());
                    //context.Validated(ticket);
                    #endregion
                }
            }
            catch (Exception ex)
            {
                context.SetError("invalid_grant", ex.ToString());
            }
            return base.GrantClientCredentials(context);
        }

        public static AuthenticationProperties CreateProperties(string userName)
        {
            IDictionary<string, string> data = new Dictionary<string, string>
            {
                { "userName", userName }
            };
            return new AuthenticationProperties(data);
        }
    }
}
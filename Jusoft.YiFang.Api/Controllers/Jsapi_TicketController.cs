using DingTalk.Api;
using DingTalk.Api.Request;
using DingTalk.Api.Response;
using Jusoft.YiFang.Api.Help;
using Jusoft.YiFang.Db.Models;
using Jusoft.YiFang.Db.ThirdSystem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Caching;
using System.Web.Http;

namespace Jusoft.YiFang.Api.Controllers
{
    //[Authorize]
    public class Jsapi_TicketController : ApiController
    {
        [HttpGet]
        public IHttpActionResult GetJsapiTicket(string url)
        {
            try
            {
                string accessToken = AccessToken.GetAccessToken();
                //获取授权jsapi_ticket
                DefaultDingTalkClient client = new DefaultDingTalkClient("https://oapi.dingtalk.com/get_jsapi_ticket");
                OapiGetJsapiTicketRequest req = new OapiGetJsapiTicketRequest();
                req.SetHttpMethod("GET");
                OapiGetJsapiTicketResponse execute = client.Execute(req, accessToken);
                //初始化配置类
                Ticket ticket = new Ticket();
                ticket.AgentId = Allocation.AgentId.ToString();
                ticket.CorpId = Allocation.CorpId;
                //时间戳
                string timeStamp = ConvertDateTimeToInt().ToString();
                ticket.TimeX = timeStamp;
                //生成签名随机字符串
                string nonceStr = "123456";
                ticket.NonceStr = nonceStr;
                //计算鉴权签名
                string str = "jsapi_ticket=" + execute.Ticket + "&noncestr=" + nonceStr + "&timestamp=" + timeStamp + "&url=" + url;
                
                var buffer = Encoding.UTF8.GetBytes(str);
                var data = SHA1.Create().ComputeHash(buffer);
                //BitConverter.ToString(data)
                StringBuilder sub = new StringBuilder();
                foreach (var t in data)
                {
                    sub.Append(t.ToString("x2"));
                }
                //var sha = SHA1(str);
                ticket.Signature = sub.ToString();
                return JsonResultHelper.JsonResult(0, "请求成功",ticket);
            }
            catch (Exception ex)
            {
                return JsonResultHelper.JsonResult(10000,"请求失败："+ex.ToString());
            }
        }

        public static long ConvertDateTimeToInt()
        {
            TimeSpan ts = DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, 0);
            return (long)ts.TotalSeconds;

        }

        public class Ticket
        {
            public string AgentId { get; set; }
            public string TimeX { get; set; }
            public string NonceStr { get; set; }
            public string CorpId { get; set; }
            public string Signature { get; set; }
            public string Ticket_Type { get; set; }
        }
        public static string SHA(string str)
        {
            byte[] StrRes = Encoding.Default.GetBytes(str);
            HashAlgorithm iSHA = new SHA1CryptoServiceProvider();
            StrRes = iSHA.ComputeHash(StrRes);
            StringBuilder EnText = new StringBuilder();
            foreach (byte iByte in StrRes)
            {
                EnText.AppendFormat("{0:x2}", iByte);
            }
            return EnText.ToString().ToUpper();
        }
        
        public static long GetToUniversalTime()
        {
            var time = (DateTime.Now.ToUniversalTime().Ticks - 621355968000000000) / 10000000;
            return time;
        }
        public void InsertCache()
        {
            try
            {
                string accessToken = AccessToken.GetAccessToken();
                //获取授权jsapi_ticket
                DefaultDingTalkClient client = new DefaultDingTalkClient("https://oapi.dingtalk.com/get_jsapi_ticket");
                OapiGetJsapiTicketRequest req = new OapiGetJsapiTicketRequest();
                req.SetHttpMethod("GET");
                OapiGetJsapiTicketResponse execute = client.Execute(req, accessToken);
                HttpRuntime.Cache.Insert("Dingtalk_Ticket", execute.Ticket, null, DateTime.Now.AddSeconds(3600), Cache.NoSlidingExpiration);
            }
            catch (Exception ex)
            {
                throw ex;
            }
           
        }
        //public class Ticket
        //{
        //    public string AgentId { get; set; }
        //    public string TimeX { get; set; }
        //    public string NonceStr { get; set; }
        //    public string CorpId { get; set; }
        //    public string Signature { get; set; }
        //    public string Ticket_Type { get; set; }
        //}
    }
}

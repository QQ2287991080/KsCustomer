using DingTalk.Api;
using DingTalk.Api.Request;
using DingTalk.Api.Response;
using Jusoft.DingTalk.Core.Callback.DingTalk;
using Jusoft.DingTalk.Core.Callbck;
using Jusoft.YiFang.Db.ThirdSystem;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace Jusoft.YiFang.Api.Controllers
{
    [AllowAnonymous]
    public class DingTalkCoreController : Controller
    {

        private static string token = "";
        private static string encodingAesKey = "";
        private static string suiteKey = "";

        public object ReceiveCallBack(string EventType)
        {
            // 获取钉钉加密解密引擎实例
            //var dingTalk = new DingTalkCrypt(token,encodingAesKey,suiteKey);

            //var strEncrypt = JObject.Parse(data)["encrypt"]?.ToString();
            //// 解密后的postbody
            //string refPostBody = "";
            //if (dingTalk.DecryptMsg(signature, timestamp, nonce, strEncrypt, ref refPostBody) != 0)
            //    throw new Exception("钉钉解密回调消息体错误【signature = " + signature + " | timestamp = " + timestamp + " | nonce = " + nonce + " | PostBody = " + strEncrypt + "】");

            // 钉钉回调事件分流
            CallbackFactory callbackFactory = new CallbackFactory();
            // 新建线程执行具体业务逻辑
            Task task = new Task(() => callbackFactory.DingTalkEventShunt(EventType, "", AccessToken.GetAccessToken()));
            task.Start();
            //Task task = new Task(() => callbackFactory.DingTalkEventShunt(JObject.Parse(refPostBody)["EventType"]?.ToString(), refPostBody, AccessToken.GetAccessToken()));
            //task.Start();
            // 接收完参数后立即按要求返回加密success对象，避免因业务逻辑执行时间过长导致钉钉认为接口调用失败
            //string encrypt = "";
            //string re_signature = "";
            //string re_timeStamp = (DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, 0)).Ticks.ToString();
            //// 生成随机串
            //string strSource = "0123456789abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ";
            //StringBuilder strRd = new StringBuilder();
            //Random rd = new Random();
            //for (int i = 0; i < 6; i++)
            //{
            //    strRd.Append(strSource.Substring(rd.Next(0, strSource.Length), 1));
            //}
            //string re_nonce = strRd.ToString();
            //dingTalk.EncryptMsg("success", re_timeStamp, re_nonce, ref encrypt, ref re_signature);
            //Hashtable jsonMap = new Hashtable
            //    {
            //        {"encrypt", encrypt},
            //        {"nonce", re_nonce},
            //        {"timeStamp", re_timeStamp},
            //        {"msg_signature", re_signature}
            //    };
            //return jsonMap;
            return "";
        }

        [HttpPost]
        public void demo()
        {
            CallbackFactory callbackFactory = new CallbackFactory();
            callbackFactory.DingTalkEventShunt("123", "456", AccessToken.GetAccessToken());
            //return "2222";
        }
      
    }
}
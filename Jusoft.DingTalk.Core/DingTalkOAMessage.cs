using DingTalk.Api;
using DingTalk.Api.Request;
using DingTalk.Api.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jusoft.DingTalk.Core
{
    public class DingTalkOAMessage
    {
        /// <summary>
        /// 钉钉发送OA模板消息
        /// </summary>
        /// <param name="agentid">微应用id</param>
        /// <param name="userlist">推送指定人员多个用,分隔</param>
        /// <param name="accessToken">token</param>
        /// <param name="content"></param>
        /// <param name="keys">OA内容body字典集合</param>
        /// <returns></returns>
        public string Message(long agentid,string userlist,string accessToken,string content,Dictionary<string,string> keys)
        {
            IDingTalkClient client = new DefaultDingTalkClient("https://oapi.dingtalk.com/topapi/message/corpconversation/asyncsend_v2");

            OapiMessageCorpconversationAsyncsendV2Request request = new OapiMessageCorpconversationAsyncsendV2Request();
            request.UseridList= userlist;
            request.AgentId= agentid;
            request.ToAllUser = false;

            OapiMessageCorpconversationAsyncsendV2Request.MsgDomain msg = new OapiMessageCorpconversationAsyncsendV2Request.MsgDomain();

            msg.Msgtype = "oa";
         
            var domains = new List<OapiMessageCorpconversationAsyncsendV2Request.FormDomain>();
            //body
            foreach (var item in keys)
            {
                domains.Add(new OapiMessageCorpconversationAsyncsendV2Request.FormDomain { Key=item.Key,Value=item.Value});
            }
            //头部默认值
            msg.Oa = new OapiMessageCorpconversationAsyncsendV2Request.OADomain
            {
                MessageUrl = "www.baidu.com",
                Head = new OapiMessageCorpconversationAsyncsendV2Request.HeadDomain(),
                Body=new OapiMessageCorpconversationAsyncsendV2Request.BodyDomain {Form=domains }
            };
            request.Msg_ = msg;
            OapiMessageCorpconversationAsyncsendV2Response response = client.Execute(request, accessToken);

            return response.Body;
        }
    }
}

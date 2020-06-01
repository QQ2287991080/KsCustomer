using DingTalk.Api;
using DingTalk.Api.Request;
using DingTalk.Api.Response;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jusoft.DingTalk.Core
{
    public class DingTalkMessage
    {
        /// <summary>
        /// 钉钉OA模板发送推送消息
        /// </summary>
        /// <param name="agentid">微应用Id</param>
        /// <param name="Userid">发送人员id-多个用,分隔</param>
        /// <param name="Token">token</param>
        /// <param name="MessageUrl">跳转url</param>
        /// <param name="keys">字典集合</param>
        public string OAMessage(long agentid,string Userid,string Token,string MessageUrl, Dictionary<string, string> keys)
        {
            Root root = new Root();
            //默认头部
            root.head = new Head();
            //消息表体
            Body body = new Body();
            List <FormItem> forms= new List<FormItem>();
            foreach (var item in keys)
            {
                forms.Add(new FormItem { key=item.Key,value=item.Value});
            }
            body.form = forms;
            root.body = body;
            //跳转链接
            root.message_url = MessageUrl;
            
            IDingTalkClient client = new DefaultDingTalkClient("https://eco.taobao.com/router/rest");
            CorpMessageCorpconversationAsyncsendRequest req = new CorpMessageCorpconversationAsyncsendRequest();
            req.Msgtype = "oa";
            req.AgentId = agentid;//微应用id
            req.UseridList = Userid;//接收者Userid列表
            //req.DeptIdList = "";//接收部门列表(可不填)
            req.ToAllUser = false;//是否发送给企业全部用户
            req.Msgcontent = JsonConvert.SerializeObject(root);
            CorpMessageCorpconversationAsyncsendResponse rsp = client.Execute(req, Token);
            return JsonConvert.SerializeObject(rsp).ToString();
        }
    }
    public class Root
    {
        /// <summary>
        /// 跳转连接
        /// </summary>
        public string message_url { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public Head head { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public Body body { get; set; }
    }
    public class Head
    {
        /// <summary>
        /// 
        /// </summary>
        public string bgcolor { get; set; }
        /// <summary>
        /// 头部标题
        /// </summary>
        public string text { get; set; }
    }

    public class FormItem
    {
        /// <summary>
        /// 姓名:
        /// </summary>
        public string key { get; set; }
        /// <summary>
        /// 张三
        /// </summary>
        public string value { get; set; }
    }

    public class Rich
    {
        /// <summary>
        /// 
        /// </summary>
        public string num { get; set; }
        /// <summary>
        /// 元
        /// </summary>
        public string unit { get; set; }
    }

    public class Body
    {
        /// <summary>
        /// 正文标题
        /// </summary>
        public string title { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public List<FormItem> form { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public Rich rich { get; set; }
        /// <summary>
        /// 大段文本大段文本大段文本大段文本大段文本大段文本大段文本大段文本大段文本大段文本大段文本大段文本
        /// </summary>
        public string content { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string image { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string file_count { get; set; }
        /// <summary>
        /// 李四 
        /// </summary>
        public string author { get; set; }
    }
}

using DingTalk.Api;
using DingTalk.Api.Request;
using DingTalk.Api.Response;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jusoft.Auxiliary
{
    public class DingTalkApproval
    {
        /// <summary>
        /// 发起钉钉审批
        /// </summary>
        /// <param name="agentid">微应用id</param>
        /// <param name="processcode">审批表单processcode</param>
        /// <param name="deptid">发起人部门id</param>
        /// <param name="userid">发起人userid</param>
        /// <param name="token">token</param>
        /// <param name="keys">表单字典内容</param>
        /// <param name="Imgkeys">表单图片字典内容</param>
        /// <returns></returns>
        public string Initiate(long agentid,string processcode,long deptid,string userid,string token,Dictionary<string,string> keys, Dictionary<string, List<string>> Imgkeys)
        {
            DefaultDingTalkClient client = new DefaultDingTalkClient("https://oapi.dingtalk.com/topapi/processinstance/create");
            OapiProcessinstanceCreateRequest request = new OapiProcessinstanceCreateRequest();
            request.AgentId = agentid;
            request.ProcessCode = processcode;
            request.DeptId = deptid;
            request.OriginatorUserId = userid;

            List<OapiProcessinstanceCreateRequest.FormComponentValueVoDomain> formComponentValues = new List<OapiProcessinstanceCreateRequest.FormComponentValueVoDomain>();

            //表单内容
            foreach (var item in keys)
            {
                formComponentValues.Add(new OapiProcessinstanceCreateRequest.FormComponentValueVoDomain {
                    Name = item.Key,
                    Value =item.Value
                });
            }
            //表单图片
            foreach (var item in Imgkeys)
            {
                formComponentValues.Add(new OapiProcessinstanceCreateRequest.FormComponentValueVoDomain() { Name = item.Key, Value = $"[{string.Join(",", item.Value)}]" });
            }

            request.FormComponentValues_ = formComponentValues;
            OapiProcessinstanceCreateResponse response = client.Execute(request, token);

            return JsonConvert.SerializeObject(response).ToString();
        }


        public string Initiate2(long agentid, string processcode, long deptid, string userid, string token, Dictionary<string, string> keys, Dictionary<string, List<string>> Imgkeys, out string str) 
        {
            DefaultDingTalkClient client = new DefaultDingTalkClient("https://oapi.dingtalk.com/topapi/processinstance/create");
            OapiProcessinstanceCreateRequest request = new OapiProcessinstanceCreateRequest();
            request.AgentId = agentid;
            request.ProcessCode = processcode;
            request.DeptId = deptid;
            request.OriginatorUserId = userid;

            List<OapiProcessinstanceCreateRequest.FormComponentValueVoDomain> formComponentValues = new List<OapiProcessinstanceCreateRequest.FormComponentValueVoDomain>();

            //表单内容
            foreach (var item in keys)
            {
                formComponentValues.Add(new OapiProcessinstanceCreateRequest.FormComponentValueVoDomain
                {
                    Name = item.Key,
                    Value = item.Value
                });
            }
             //  表单图片
            foreach (var item in Imgkeys)
            {
                formComponentValues.Add(new OapiProcessinstanceCreateRequest.FormComponentValueVoDomain() { Name = item.Key, Value = $"[{string.Join(",", item.Value)}]" });
            }

            request.FormComponentValues_ = formComponentValues;
            OapiProcessinstanceCreateResponse response = client.Execute(request, token);
            str = response.Errmsg+response.Errcode+response.Body;
            return response.ProcessInstanceId;
        }
    }
}

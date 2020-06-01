using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Jusoft.DingTalk.Core.Logs;
using Newtonsoft.Json;

namespace Jusoft.DingTalk.Core.Callbck
{
    public class bpms_instance_change:CallbackFace
    {
        /// <summary>
        /// 审批实例开始/结束/中止
        /// </summary>
        /// <param name="data"></param>
        public void invoke(string data)
        {
            try
            {
                bpms_instance_change instance_Change = JsonConvert.DeserializeObject<bpms_instance_change>(data);

                //区分审批单据开始/结束/终止
                switch (instance_Change.type)
                {
                    case "start"://开始
                        LogHelper.WriteLog("", "【审批实例开始】");
                        break;
                    case "finish"://结束
                        if (instance_Change.result == "agree")//审批实例-同意
                        {
                            LogHelper.WriteLog("", "【审批实例结束-同意】");
                        }
                        else if (instance_Change.result == "refuse")//审批实例-结束
                        {
                            LogHelper.WriteLog("", "【审批实例结束-拒绝】");
                        }
                        break;
                    case "terminate"://终止
                        LogHelper.WriteLog("", "【审批实例终止】");
                        break;
                    default:
                        LogHelper.WriteLog("", "【审批实例-未知的状态】");
                        LogHelper.WriteLog(data, "【审批实例-未知状态数据】");
                        break;
                }
            }
            catch (Exception ex)
            {
                LogHelper.WriteLog(ex.ToString(), "【审批实例失败】");
                LogHelper.WriteLog(data, "【审批实例-失败数据】");
            }
        }


        public string EventType { get; set; }

        public string processInstanceId { get; set; }

        public long? finishTime { get; set; }

        public string corpId { get; set; }

        public string title { get; set; }

        public string type { get; set; }

        public string url { get; set; }

        public string result { get; set; }

        public long createTime { get; set; }

        public string staffId { get; set; }

        public string processCode { get; set; }

       
    }
}

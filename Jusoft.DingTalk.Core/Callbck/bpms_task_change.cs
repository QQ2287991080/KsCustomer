using Jusoft.DingTalk.Core.Logs;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jusoft.DingTalk.Core.Callbck
{
    public class bpms_task_change:CallbackFace
    {
        /// <summary>
        /// 审批任务开始/结束/转交
        /// </summary>
        /// <param name="data">钉钉返回数据</param>
        public void invoke(string data)
        {
            try
            {
                bpms_task_change task_Change = JsonConvert.DeserializeObject<bpms_task_change>(data);

                //区分审批单据任务开始/结束/转交
                switch (task_Change.type)
                {
                    case "start"://开始
                        LogHelper.WriteLog("", "【审批任务开始】");
                        break;
                    case "finish"://结束
                        if (task_Change.result == "agree")//审批任务-同意
                        {
                            LogHelper.WriteLog("", "【审批任务-同意】");
                        }
                        else if (task_Change.result == "refuse")//审批任务-结束
                        {
                            LogHelper.WriteLog("", "【审批任务-拒绝】");
                        }
                        else if (task_Change.result == "redirect")//审批任务-转交
                        {
                            LogHelper.WriteLog("", "【审批任务-转交】");
                        }
                        break;
                    default:
                        LogHelper.WriteLog("", "【审批任务-未知的状态】");
                        LogHelper.WriteLog(data, "【审批任务-未知状态数据】");
                        break;
                }
            }
            catch (Exception ex)
            {
                LogHelper.WriteLog(ex.ToString(), "【审批任务失败】");
                LogHelper.WriteLog(data, "【审批任务-失败数据】");
            }
        }

        public string EventType { get; set; }

        public string processInstanceId { get; set; }

        public int finishTime { get; set; }

        public string corpId { get; set; }

        public string title { get; set; }

        public string type { get; set; }

        public string result { get; set; }

        public string remark { get; set; }

        public int createTime { get; set; }

        public string staffId { get; set; }

        public string processCode { get; set; }

        
    }
}

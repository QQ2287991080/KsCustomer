using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Jusoft.YiFang.Api.Help
{
    public class Callback
    {
        /// <summary>
        /// 时间类型
        /// </summary>
        public string EventType { get; set; }
        /// <summary>
        /// 审批实列id
        /// </summary>
        public string processInstanceId { get; set; }
        /// <summary>
        /// 审批结束时间
        /// </summary>
        public long finishTime { get; set; }
        /// <summary>
        /// 发什么审批任务变更的企业
        /// </summary>
        public string corpId { get; set; }
        /// <summary>
        /// 实列标题
        /// </summary>
        public string title { get; set; }
        /// <summary>
        /// 类型
        /// </summary>
        public string type { get; set; }
        /// <summary>
        /// 结果
        /// </summary>
        public string result { get; set; }
        /// <summary>
        /// 拒绝理由
        /// </summary>
        public string remark { get; set; }
        /// <summary>
        /// 实列创建时间
        /// </summary>
        public long createTime { get; set; }
        /// <summary>
        /// 审批人
        /// </summary>
        public string staffId { get; set; }
        /// <summary>
        /// 审批模板唯一编码
        /// </summary>
        public string processCode { get; set; }
        /// <summary>
        /// 审批实列网址
        /// </summary>
        public string url { get; set; }

        public string CorpId { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public List<string> UserId { get; set; }

        public List<string> DeptId { get; set; }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Jusoft.YiFang.Api.PC.Models
{
    public class HomeResult
    {
        public int Code { get; set; }
        public string Result { get; set; }
        //待处理
        public int ProcessedCount { get; set; }
        //处理中
        public int ProcessingCount { get; set; }
        //已结束
        public int OverCount { get; set; }
        //原材料品质
        public int MaterialCount { get; set; }
        //设备品质
        public int RepairCount { get; set; }
        //物流服务
        public int LogisticsCount{ get; set; }
        //装修品质
        public int DecorationCount{ get; set; }
        //供应链其他
        public int SupplyOtherCount { get; set; }
        //营运管理
        public int OperaCount { get; set; }
        //巡店服务质量
        public int PatrolCount{ get; set; }
        //运营其他
        public int OperateOtherCount { get; set; }
        //电子商务
        public int ElemerceCount { get; set; }
        //营运
        public int OperationCount { get; set; }
        //行销反馈
        public int MarketingCount { get; set; }
        //辅助反馈
        public int AuxiliaryCount { get; set; }
        //研发反馈
        public int ResearchCount{ get; set; }
        //其他反馈
        public int OtherCount { get; set; }
    }
}
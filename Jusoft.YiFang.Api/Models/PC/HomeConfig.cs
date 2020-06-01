using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Jusoft.YiFang.Api.PC.Models
{
    public static class HomeConfig
    {
        //未处理
        public const int Processed = 0;
        //处理中
        public const int Processing = 1;
        //已结束
        public const int Over = 2;
        public const int Rejected = 3;
        public const int Evaluated = 4;
        //原材料品质
        public const int Material = 12;
        //设备品质
        public const int Repair = 13;
        //物流服务
        public const int Logistics = 14;
        //装修品质
        public const int Decoration = 15;
        //供应链其他
        public const int SupplyOther = 16;
        //营运管理
        public const int Opera = 39;
        //巡店服务质量
        public const int Patrol = 40;
        //运营其他
        public const int OperateOther = 155;
        //电子商务
        public const int Elemerce = 20;
        //营运
        public const int Operation = 21;
        //行销反馈
        public const int Marketing = 22;
        //辅助反馈
        public const int Auxiliary = 23;
        //研发反馈
        public const int Research = 24;
        //其他反馈
        public const int Other = 26;
    }
}
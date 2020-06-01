using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Jusoft.YiFang.Api.PC.Models
{
    public class MissionModel
    {
        //原物料1，设备客诉2，运营反馈3，其他反馈4
        public int Id { get; set; }
        //所属门店
        public string OwnedStores { get; set; }
        //异常归类
        public int AbnormalType { get; set; }
        //客诉类型
        public int CusmoterType { get; set; }
        //门店编号
        public string StoreCode { get; set; }
        //客诉单号
        public int? CusmoterCode { get; set; }
        //一言为订订单号
        public string OrderCode { get; set; }
        //当前状态
        public List<int?> State { get; set; }
        //时间段开始时间
        public DateTime? StateTime { get; set; }
        //时间段结束时间
        public DateTime? EndTime { get; set; }
        public int PageIndex { get; set; }
        public int PageSize { get; set; }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Jusoft.YiFang.Api.Models
{
    public static class Enumerate
    {
        public enum CustomerState
        {
            Untreated,//未处理
            Processing,//处理中
            Completed,//已完成
            HasScore,//已评分
            Refused//拒绝
        }
    }
}
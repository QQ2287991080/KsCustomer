using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Jusoft.YiFang.Api.PC.Models
{
    public class BasicMaintainModel
    {
        public string SeachKey { get; set; }
        public int? IdSysEnType { get; set; }
        public int? IdParent { get; set; }
        public int PageIndex { get; set; }
        public int PageSize { get; set; }
    }
}
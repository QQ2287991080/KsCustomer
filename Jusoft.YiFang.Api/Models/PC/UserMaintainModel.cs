using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Jusoft.YiFang.Api.PC.Models
{
    public class UserMaintainModel
    {
        public string Name { get; set; }
        public string Phone { get; set; }
        public int PageIndex { get; set; }
        public int PageSize { get; set; }

        public object SearchKey { get; set; }
    }
}
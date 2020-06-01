using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Jusoft.YiFang.Api.PC.Models
{
    public class HomeIndexModel
    {
        public string StStortCode { get; set; }
        //public int? SubclassId { get; set; }
        public DateTime? StateTime { get; set; }
        public DateTime? EndTime { get; set; }
    }
}
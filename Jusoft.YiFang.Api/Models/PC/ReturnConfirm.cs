using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Jusoft.YiFang.Api.PC.Models
{
    public class ReturnConfirm
    {
        public decimal TypeId { get; set; }
        public string TypeName { get; set; }
        public List<string> Name { get; set; }
    }
}
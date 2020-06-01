using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Jusoft.YiFang.Api.PC.Models
{
    public class ProductModel
    {
        public int? Id { get; set; }
        public int? IdParent { get; set; }
        public string ProducName { get; set; }
        public string Memo { get; set; }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Jusoft.YiFang.Api.PC.Models
{
    public class MenuModels
    {
        public decimal Id { get; set; }
        public string label { get; set; }
        public List<MenuModels> children { get; set; }
    }
}
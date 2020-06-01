using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Jusoft.YiFang.Api.PC.Models
{
    public class MenuFunctionModels
    {
        public string Code { get; set; }
        public string Name { get; set; }
        public int FunctionState { get; set; }
        public List<MenuFunctionModels> children { get; set; }
    }
}
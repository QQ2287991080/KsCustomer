using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Jusoft.YiFang.Api.PC.Models
{
    public class EditRoleMenu
    {
        public int Id { get; set; }
        public List<string> Menu { get; set; }
        public List<string> Function { get; set; }

    }
}
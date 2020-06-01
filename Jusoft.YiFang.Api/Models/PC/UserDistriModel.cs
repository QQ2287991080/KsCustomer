using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Jusoft.YiFang.Api.PC.Models
{
    public class UserDistriModel
    {
        public int UserId { get; set; }
        public List<int> Roles { get; set; }
    }
}
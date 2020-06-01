using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Jusoft.YiFang.Api.PC.Models
{
    public class BasicDetailsModel
    {
        //id
        public int? Id { get; set; }
        public int type { get; set; }
        //类型Id
        public int? IdPerson { get; set; }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Jusoft.YiFang.Api.PC.Models
{
    public class StoreModel
    {
        public int? Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public int? IdSysArea { get; set; }
        public string DetailedAddress { get; set; }
        public string JoinPerson { get; set; }
        public string Phone { get; set; }
        public string Remark { get; set; }
        public int? IdPerson { get; set; }
        public int? SubclassId { get; set; }
        public int? IdPersonRegion { get; set; }
        public string PasswordHash { get; set; }
        public string UserName { get; set; }

    }
}
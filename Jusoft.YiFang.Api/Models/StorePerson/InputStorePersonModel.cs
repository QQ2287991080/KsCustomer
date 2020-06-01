using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Jusoft.YiFang.Api.Models.StorePerson
{
    public class InputStorePersonModel
    {
        /// <summary>
        /// 门店id
        /// </summary>
        public decimal IdStore { get; set; }
        /// <summary>
        /// 人员id
        /// </summary>
        public decimal IdPerson { get; set; }
    }
}
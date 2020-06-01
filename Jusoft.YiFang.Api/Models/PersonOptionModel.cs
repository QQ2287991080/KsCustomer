using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Jusoft.YiFang.Api.Models
{
    public class PersonOptionModel : Page
    {
        /// <summary>
        /// 状态
        /// </summary>
        public int State { get; set; }
        /// <summary>
        /// 客户id或者驻点id
        /// </summary>
        public decimal Id { get; set; }

        public string SearchKey { get; set; }
    }
}
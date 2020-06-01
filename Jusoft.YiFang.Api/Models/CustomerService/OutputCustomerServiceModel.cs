using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Jusoft.YiFang.Api.Models.CustomerService
{
    public class OutputCustomerServiceModel:Page
    {
        /// <summary>
        /// 客服名称
        /// </summary>
        public string Name { get; set; }

        public object SearchKey { get; set; }
    }

  
}
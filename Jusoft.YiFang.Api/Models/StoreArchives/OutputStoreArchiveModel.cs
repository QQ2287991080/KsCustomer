using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Jusoft.YiFang.Api.Models.StoreArchives
{
    public class OutputStoreArchiveModel:Page
    {
        /// <summary>
        /// 门店名称
        /// </summary>
        public string Name { get; set; }

        public object SearchKey { get; set; }
        
    }
}
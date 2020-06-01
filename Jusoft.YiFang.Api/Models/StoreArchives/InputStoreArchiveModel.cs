using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Jusoft.YiFang.Api.Models.StoreArchives
{
    public class InputStoreArchiveModel
    {
        public decimal Id { get; set; }
        /// <summary>
        /// 门店名称
        /// </summary>
        [Required]
        public string Name { get; set; }
        /// <summary>
        /// 详细地址
        /// </summary>
        public string Address { get; set; }
        /// <summary>
        /// 门店代码
        /// </summary>
        public string Code { get; set; }
        /// <summary>
        /// 客服
        /// </summary>
        public decimal IdSupervisor { get; set; }
        /// <summary>
        /// 所属大区
        /// </summary>
        public decimal RegionId { get; set; }
        /// <summary>
        /// 大区负责人
        /// </summary>
        public decimal IdPersonRegion { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        public string Remark { get; set; }
        /// <summary>
        /// 地区id
        /// </summary>
        public decimal? IdArea { get; set; }
    }
}
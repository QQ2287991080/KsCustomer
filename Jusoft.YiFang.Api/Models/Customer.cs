using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Jusoft.YiFang.Api.Models
{
    public class Customer
    {
        /// <summary>
        /// 客诉类型Id
        /// </summary>
        public int SubclassId { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        public string Remark { get; set; }

        public List<Audio> Audios { get; set; }

    }
    public class Audio
    {
        /// <summary>
        /// 录音mediaid
        /// </summary>
        public string Url { get; set; }
        /// <summary>
        ///录音时间
        /// </summary>
        public decimal? Duration { get; set; }

        public string Remark { get; set; }
    }
}
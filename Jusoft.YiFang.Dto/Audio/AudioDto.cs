using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jusoft.YiFang.Dto.Audio
{
  public  class AudioDto
    {
        /// <summary>
        /// MP3的路径
        /// </summary>
        public string Url { get; set; }
        /// <summary>
        ///录音时间
        /// </summary>
        public decimal? Duration { get; set; }
        /// <summary>
        /// Arm文件的路径
        /// </summary>
        public string OriginUrl { get; set; }
        /// <summary>
        /// 音频资源的MediaID
        /// </summary>
        public string MediaId { get; set; }
        /// <summary>
        /// 文字描述
        /// </summary>
        public string Remark { get; set; }
    }
}

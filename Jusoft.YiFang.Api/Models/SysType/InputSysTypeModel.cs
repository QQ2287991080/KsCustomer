using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
namespace Jusoft.YiFang.Api.Models.SysType
{
    public class InputSysTypeModel
    {
        //[Required(ErrorMessage ="{0}不能为空啦")]
        /// <summary>
        /// 类型名称
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 父级id
        /// </summary>
        public decimal IdParent { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        public string Memo { get; set; }
        public decimal Id { get; set; }

        public List<decimal> PersonList { get; set; }


    }

}
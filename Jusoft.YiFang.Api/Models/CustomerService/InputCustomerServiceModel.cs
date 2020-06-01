using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Jusoft.YiFang.Api.Models.CustomerService
{
    public class InputCustomerServiceModel
    {
        public decimal Id { get; set; }
        [Required]
        [DisplayName("客服人员")]
        public decimal IdPerson { get; set; }
        [DisplayName("备注")]
        public string Remark { get; set; }
    }
}
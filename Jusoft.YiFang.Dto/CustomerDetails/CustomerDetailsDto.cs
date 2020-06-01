using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jusoft.YiFang.Dto.CustomerDetails
{
    public class CustomerDetailsDto
    {
        /// <summary>
        /// Id
        /// </summary>
        public decimal Id { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        public string CreateTime { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        public string Remark { get; set; }
        /// <summary>
        /// 状态
        /// </summary>
        public byte? State { get; set; }
        /// <summary>
        /// 客诉类型
        /// </summary>
        public decimal? StateId { get; set; }
        /// <summary>
        /// 客诉类型名称
        /// </summary>
        public string SubclassName { get; set; }
        /// <summary>
        /// 审批时间
        /// </summary>
        public string DeliveryDate { get; set; }
        /// <summary>
        /// 审批字段
        /// </summary>
        public int? Number { get; set; }
        /// <summary>
        /// 评价内容
        /// </summary>
        public string Evaluation { get; set; }
        /// <summary>
        /// 评价时间
        /// </summary>
        public string EvaluationTime { get; set; }
        /// <summary>
        /// 处理评分
        /// </summary>
        public int? DealGrade { get; set; }
        /// <summary>
        /// 结果评分
        /// </summary>
        public int? ResultGrade { get; set; }
    }
    public class Supplementary
    {
        /// <summary>
        /// 客服id
        /// </summary>
        public decimal Id { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        public string Remark { get; set; }
        /// <summary>
        /// 完成时间
        /// </summary>
        public string FinishTime { get; set; }
        /// <summary>
        /// 完成人名称
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 完成人id
        /// </summary>
        public decimal KFId { get; set; }
        /// <summary>
        /// 状态
        /// </summary>
        public byte State { get; set; }

    }
}

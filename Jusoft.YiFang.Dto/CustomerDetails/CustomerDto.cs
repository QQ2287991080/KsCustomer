using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jusoft.YiFang.Dto.CustomerDetails
{
   public class CustomerDto
    {
        public string EnTypeName { get; set; }
        public decimal Id { get; set; }
        public Nullable<decimal> StateId { get; set; }
        public Nullable<decimal> SubclassId { get; set; }
        public string ProductIds { get; set; }
        public string ProductIdNames { get; set; }
        public Nullable<int> Number { get; set; }
        public Nullable<decimal> UnitId { get; set; }
        public Nullable<decimal> AbnormalId { get; set; }
        public Nullable<System.DateTime> DeliveryDate { get; set; }
        public string ProductionBatch { get; set; }
        public string OrderNumber { get; set; }
        public Nullable<decimal> RepairTypeId { get; set; }
        public Nullable<decimal> RepairProductId { get; set; }
        public string StoreContact { get; set; }
        public string StoreAddress { get; set; }
        public string StoreTel { get; set; }
        public string StoreEmail { get; set; }
        public string Remark { get; set; }
        public decimal IdPersonSupervision { get; set; }
        public decimal IdPerson { get; set; }
        public System.DateTime CreateTime { get; set; }
        public Nullable<System.DateTime> FinishTime { get; set; }
        public string Evaluation { get; set; }
        public Nullable<int> ResultGrade { get; set; }
        public Nullable<int> DealGrade { get; set; }
        public Nullable<byte> State { get; set; }
        public Nullable<bool> ReplenishBit { get; set; }
        public string DingTalkApproval { get; set; }
        public Nullable<decimal> IdPersonApproval { get; set; }
        public string ProductName { get; set; }
        public string PersonSupervision { get; set; }
        public string StoreCode { get; set; }
        public string StoreName { get; set; }
        public string StoreRegionName { get; set; }
        public Nullable<System.DateTime> EvaluationTime { get; set; }
    }
}

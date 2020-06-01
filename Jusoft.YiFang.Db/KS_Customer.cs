//------------------------------------------------------------------------------
// <auto-generated>
//     此代码已从模板生成。
//
//     手动更改此文件可能导致应用程序出现意外的行为。
//     如果重新生成代码，将覆盖对此文件的手动更改。
// </auto-generated>
//------------------------------------------------------------------------------

namespace Jusoft.YiFang.Db
{
    using System;
    using System.Collections.Generic;
    
    public partial class KS_Customer
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public KS_Customer()
        {
            this.KS_Customer_Approval = new HashSet<KS_Customer_Approval>();
            this.KS_Customer_Replenish = new HashSet<KS_Customer_Replenish>();
        }
    
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
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<KS_Customer_Approval> KS_Customer_Approval { get; set; }
        public virtual OR_Person OR_Person { get; set; }
        public virtual OR_Person OR_Person1 { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<KS_Customer_Replenish> KS_Customer_Replenish { get; set; }
        public virtual BA_SysEnType BA_SysEnType { get; set; }
        public virtual BA_SysEnType BA_SysEnType1 { get; set; }
        public virtual BA_SysEnType BA_SysEnType2 { get; set; }
        public virtual BA_SysEnType BA_SysEnType3 { get; set; }
        public virtual BA_SysEnType BA_SysEnType4 { get; set; }
    }
}

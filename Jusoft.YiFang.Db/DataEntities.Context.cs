﻿//------------------------------------------------------------------------------
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
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    using System.Data.Entity.Core.Objects;
    using System.Linq;
    
    public partial class YiFang_CustomerComplaintEntities : DbContext
    {
        public YiFang_CustomerComplaintEntities()
            : base("name=YiFang_CustomerComplaintEntities")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<OR_Person> OR_Person { get; set; }
        public virtual DbSet<BA_BusinessType> BA_BusinessType { get; set; }
        public virtual DbSet<OR_Department> OR_Department { get; set; }
        public virtual DbSet<REF_Person_Department> REF_Person_Department { get; set; }
        public virtual DbSet<KS_Customer_Replenish> KS_Customer_Replenish { get; set; }
        public virtual DbSet<KS_Customer_Approval> KS_Customer_Approval { get; set; }
        public virtual DbSet<BA_Attachment> BA_Attachment { get; set; }
        public virtual DbSet<KS_Confirm> KS_Confirm { get; set; }
        public virtual DbSet<v_Jusoft_CustomerList> v_Jusoft_CustomerList { get; set; }
        public virtual DbSet<KS_Customer> KS_Customer { get; set; }
        public virtual DbSet<v_Jusoft_PcBASysArea> v_Jusoft_PcBASysArea { get; set; }
        public virtual DbSet<v_Jusoft_PcSysEnType> v_Jusoft_PcSysEnType { get; set; }
        public virtual DbSet<v_Jusoft_PcSysRoles> v_Jusoft_PcSysRoles { get; set; }
        public virtual DbSet<v_Jusoft_PcOrPerson> v_Jusoft_PcOrPerson { get; set; }
        public virtual DbSet<v_Jusoft_StortDetails> v_Jusoft_StortDetails { get; set; }
        public virtual DbSet<v_Jusoft_PcStStore> v_Jusoft_PcStStore { get; set; }
        public virtual DbSet<v_Jusoft_PcSysUser> v_Jusoft_PcSysUser { get; set; }
        public virtual DbSet<BA_SysEnType> BA_SysEnType { get; set; }
        public virtual DbSet<AC_SysUsers> AC_SysUsers { get; set; }
        public virtual DbSet<AUTH_User_Menu> AUTH_User_Menu { get; set; }
        public virtual DbSet<BA_SysCommand> BA_SysCommand { get; set; }
        public virtual DbSet<AUTH_User_Command> AUTH_User_Command { get; set; }
        public virtual DbSet<v_Jusoft_KSConfirm> v_Jusoft_KSConfirm { get; set; }
        public virtual DbSet<v_Jusoft_PcMissionCenter> v_Jusoft_PcMissionCenter { get; set; }
        public virtual DbSet<BA_SysMenu> BA_SysMenu { get; set; }
        public virtual DbSet<v_jusoft_SysMenu_Command> v_jusoft_SysMenu_Command { get; set; }
        public virtual DbSet<CS_CustomerService> CS_CustomerService { get; set; }
        public virtual DbSet<AC_SysRoles> AC_SysRoles { get; set; }
        public virtual DbSet<ST_Store> ST_Store { get; set; }
        public virtual DbSet<v_Store_Person> v_Store_Person { get; set; }
        public virtual DbSet<v_Jusoft_StortArea> v_Jusoft_StortArea { get; set; }
        public virtual DbSet<v_StoreArchives> v_StoreArchives { get; set; }
        public virtual DbSet<v_CustomerService> v_CustomerService { get; set; }
        public virtual DbSet<v_KsType> v_KsType { get; set; }
        public virtual DbSet<v_Ks_DataList> v_Ks_DataList { get; set; }
        public virtual DbSet<v_Customer> v_Customer { get; set; }
        public virtual DbSet<v_Kf_DataList> v_Kf_DataList { get; set; }
    
        [DbFunction("YiFang_CustomerComplaintEntities", "f_getEnTypeList")]
        public virtual IQueryable<f_getEnTypeList_Result> f_getEnTypeList(Nullable<decimal> id)
        {
            var idParameter = id.HasValue ?
                new ObjectParameter("Id", id) :
                new ObjectParameter("Id", typeof(decimal));
    
            return ((IObjectContextAdapter)this).ObjectContext.CreateQuery<f_getEnTypeList_Result>("[YiFang_CustomerComplaintEntities].[f_getEnTypeList](@Id)", idParameter);
        }
    }
}

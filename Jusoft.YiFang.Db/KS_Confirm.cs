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
    
    public partial class KS_Confirm
    {
        public decimal Id { get; set; }
        public decimal TypeId { get; set; }
        public decimal IdPerson { get; set; }
    
        public virtual OR_Person OR_Person { get; set; }
        public virtual BA_SysEnType BA_SysEnType { get; set; }
    }
}

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
    
    public partial class REF_Person_Department
    {
        public decimal IdPerson { get; set; }
        public string CodeDepartment { get; set; }
        public bool IsDefine { get; set; }
    
        public virtual OR_Department OR_Department { get; set; }
        public virtual OR_Person OR_Person { get; set; }
    }
}
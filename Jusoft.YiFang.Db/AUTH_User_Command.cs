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
    
    public partial class AUTH_User_Command
    {
        public string CodeSysCommand { get; set; }
        public decimal IdSysUsers { get; set; }
        public byte AUTH { get; set; }
    
        public virtual AC_SysUsers AC_SysUsers { get; set; }
        public virtual BA_SysCommand BA_SysCommand { get; set; }
    }
}

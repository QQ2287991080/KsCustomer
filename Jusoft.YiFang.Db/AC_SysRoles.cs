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
    
    public partial class AC_SysRoles
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public AC_SysRoles()
        {
            this.BA_SysCommand = new HashSet<BA_SysCommand>();
            this.BA_SysMenu = new HashSet<BA_SysMenu>();
            this.AC_SysUsers = new HashSet<AC_SysUsers>();
        }
    
        public decimal Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<BA_SysCommand> BA_SysCommand { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<BA_SysMenu> BA_SysMenu { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<AC_SysUsers> AC_SysUsers { get; set; }
    }
}

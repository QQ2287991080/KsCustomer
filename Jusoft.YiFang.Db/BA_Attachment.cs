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
    
    public partial class BA_Attachment
    {
        public decimal Id { get; set; }
        public string CodeBusinessType { get; set; }
        public decimal SourceId { get; set; }
        public string FileAccess { get; set; }
        public string FileName { get; set; }
        public Nullable<decimal> FileSize { get; set; }
        public byte[] FileSource { get; set; }
        public string FileType { get; set; }
        public System.Guid FileHash { get; set; }
    
        public virtual BA_BusinessType BA_BusinessType { get; set; }
    }
}

//------------------------------------------------------------------------------
// <auto-generated>
//    此代码是根据模板生成的。
//
//    手动更改此文件可能会导致应用程序中发生异常行为。
//    如果重新生成代码，则将覆盖对此文件的手动更改。
// </auto-generated>
//------------------------------------------------------------------------------

namespace Se.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class BonusLog
    {
        public int Id { get; set; }
        public int BonusType { get; set; }
        public int UserId { get; set; }
        public int ToUserId { get; set; }
        public int OldCash { get; set; }
        public int Cash { get; set; }
        public int NewCash { get; set; }
        public System.DateTime BonusTime { get; set; }
    }
}

//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace DtDc_Billing.Entity_FR
{
    using System;
    using System.Collections.Generic;
    
    public partial class Notification
    {
        public int N_ID { get; set; }
        public string Message { get; set; }
        public Nullable<System.DateTime> dateN { get; set; }
        public Nullable<bool> Status { get; set; }
        public string url_path { get; set; }
        public string description { get; set; }
    }
}

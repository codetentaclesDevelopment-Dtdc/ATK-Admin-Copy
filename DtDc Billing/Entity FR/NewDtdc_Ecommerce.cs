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
    
    public partial class NewDtdc_Ecommerce
    {
        public int Ecom_id { get; set; }
        public Nullable<double> EcomPslab1 { get; set; }
        public Nullable<double> EcomPslab2 { get; set; }
        public Nullable<double> EcomPslab3 { get; set; }
        public Nullable<double> EcomPslab4 { get; set; }
        public Nullable<double> EcomGEslab1 { get; set; }
        public Nullable<double> EcomGEslab2 { get; set; }
        public string Company_id { get; set; }
        public Nullable<int> Sector_Id { get; set; }
        public Nullable<double> EcomGEslab3 { get; set; }
        public Nullable<double> EcomGEslab4 { get; set; }
        public Nullable<double> EcomPupto1 { get; set; }
        public Nullable<double> EcomPupto2 { get; set; }
        public Nullable<double> EcomPupto3 { get; set; }
        public Nullable<double> EcomPupto4 { get; set; }
        public Nullable<double> EcomGEupto1 { get; set; }
        public Nullable<double> EcomGEupto2 { get; set; }
        public Nullable<double> EcomGEupto3 { get; set; }
        public Nullable<double> EcomGEupto4 { get; set; }
        public Nullable<int> NoOfSlabN { get; set; }
        public Nullable<int> NoOfSlabS { get; set; }
    
        public virtual Company Company { get; set; }
        public virtual Sector Sector { get; set; }
    }
}

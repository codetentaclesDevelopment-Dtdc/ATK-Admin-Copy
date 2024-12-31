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
    
    public partial class Company
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Company()
        {
            this.Dtdc_Ecommerce = new HashSet<Dtdc_Ecommerce>();
            this.Dtdc_Ecommerce1 = new HashSet<Dtdc_Ecommerce>();
            this.dtdcPlus = new HashSet<dtdcPlu>();
            this.GECrates = new HashSet<GECrate>();
            this.NewDtdc_Ecommerce = new HashSet<NewDtdc_Ecommerce>();
            this.Nondoxes = new HashSet<Nondox>();
            this.Nondoxes1 = new HashSet<Nondox>();
            this.Priorities = new HashSet<Priority>();
            this.RateLaptops = new HashSet<RateLaptop>();
            this.RateLaptops1 = new HashSet<RateLaptop>();
            this.Ratems = new HashSet<Ratem>();
            this.Ratems1 = new HashSet<Ratem>();
            this.RateRevLaptops = new HashSet<RateRevLaptop>();
            this.RateRevLaptops1 = new HashSet<RateRevLaptop>();
            this.dtdcPlus1 = new HashSet<dtdcPlu>();
            this.Dtdc_Ptp = new HashSet<Dtdc_Ptp>();
        }
    
        public string Company_Id { get; set; }
        public int c_id { get; set; }
        public Nullable<long> Phone { get; set; }
        public string Email { get; set; }
        public Nullable<double> Insurance { get; set; }
        public Nullable<double> Minimum_Risk_Charge { get; set; }
        public string Other_Details { get; set; }
        public Nullable<double> Fuel_Sur_Charge { get; set; }
        public Nullable<double> Topay_Charge { get; set; }
        public Nullable<double> Cod_Charge { get; set; }
        public Nullable<double> Gec_Fuel_Sur_Charge { get; set; }
        public string Pf_code { get; set; }
        public string Company_Address { get; set; }
        public string Company_Name { get; set; }
        public Nullable<System.DateTime> Datetime_Comp { get; set; }
        public string Gst_No { get; set; }
        public string Pan_No { get; set; }
        public Nullable<double> Royalty_Charges { get; set; }
        public Nullable<double> D_Docket { get; set; }
        public Nullable<double> P_Docket { get; set; }
        public Nullable<double> E_Docket { get; set; }
        public Nullable<double> V_Docket { get; set; }
        public Nullable<double> I_Docket { get; set; }
        public Nullable<double> N_Docket { get; set; }
        public string Password { get; set; }
        public string Username { get; set; }
        public Nullable<double> covidtax { get; set; }
        public string Isregister { get; set; }
        public Nullable<System.DateTime> StartDate { get; set; }
        public Nullable<System.DateTime> EndDate { get; set; }
        public Nullable<double> InsuranceperAboveonelakh { get; set; }
        public Nullable<int> IsAgreementoption { get; set; }
        public string DocumentFilepath { get; set; }
        public string Remark { get; set; }
        public string ProductType { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Dtdc_Ecommerce> Dtdc_Ecommerce { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Dtdc_Ecommerce> Dtdc_Ecommerce1 { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<dtdcPlu> dtdcPlus { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<GECrate> GECrates { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<NewDtdc_Ecommerce> NewDtdc_Ecommerce { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Nondox> Nondoxes { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Nondox> Nondoxes1 { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Priority> Priorities { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<RateLaptop> RateLaptops { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<RateLaptop> RateLaptops1 { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Ratem> Ratems { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Ratem> Ratems1 { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<RateRevLaptop> RateRevLaptops { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<RateRevLaptop> RateRevLaptops1 { get; set; }
        public virtual Franchisee Franchisee { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<dtdcPlu> dtdcPlus1 { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Dtdc_Ptp> Dtdc_Ptp { get; set; }
    }
}
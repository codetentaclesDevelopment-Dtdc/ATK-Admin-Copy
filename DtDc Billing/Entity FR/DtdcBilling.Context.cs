﻿//------------------------------------------------------------------------------
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
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    using System.Data.Entity.Core.Objects;
    using System.Linq;
    
    public partial class DtdcBillingEntities : DbContext
    {
        public DtdcBillingEntities()
            : base("name=DtdcBillingEntities")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<ActivityLog> ActivityLogs { get; set; }
        public virtual DbSet<Admin> Admins { get; set; }
        public virtual DbSet<Destination> Destinations { get; set; }
        public virtual DbSet<Dtdc_Ptp> Dtdc_Ptp { get; set; }
        public virtual DbSet<dtdcPlu> dtdcPlus { get; set; }
        public virtual DbSet<Employee> Employees { get; set; }
        public virtual DbSet<Expense> Expenses { get; set; }
        public virtual DbSet<express_cargo> express_cargo { get; set; }
        public virtual DbSet<Nondox> Nondoxes { get; set; }
        public virtual DbSet<Other_Service> Other_Service { get; set; }
        public virtual DbSet<Payment> Payments { get; set; }
        public virtual DbSet<Ratem> Ratems { get; set; }
        public virtual DbSet<Saving> Savings { get; set; }
        public virtual DbSet<sectorName> sectorNames { get; set; }
        public virtual DbSet<Service_list> Service_list { get; set; }
        public virtual DbSet<User> Users { get; set; }
        public virtual DbSet<WalletPoint> WalletPoints { get; set; }
        public virtual DbSet<RedeemOtp> RedeemOtps { get; set; }
        public virtual DbSet<Sendmessage> Sendmessages { get; set; }
        public virtual DbSet<wallet_History> wallet_History { get; set; }
        public virtual DbSet<Receipt_details> Receipt_details { get; set; }
        public virtual DbSet<deliverydata> deliverydatas { get; set; }
        public virtual DbSet<Franchisee> Franchisees { get; set; }
        public virtual DbSet<addcodamount> addcodamounts { get; set; }
        public virtual DbSet<addtopayamount> addtopayamounts { get; set; }
        public virtual DbSet<Transaction> Transactions { get; set; }
        public virtual DbSet<TransactionView> TransactionViews { get; set; }
        public virtual DbSet<Cheque> Cheques { get; set; }
        public virtual DbSet<Issue> Issues { get; set; }
        public virtual DbSet<Sector> Sectors { get; set; }
        public virtual DbSet<Company> Companies { get; set; }
        public virtual DbSet<Notification> Notifications { get; set; }
        public virtual DbSet<Complaint> Complaints { get; set; }
        public virtual DbSet<Holiday> Holidays { get; set; }
        public virtual DbSet<popupimage> popupimages { get; set; }
        public virtual DbSet<ReplyAdmin> ReplyAdmins { get; set; }
        public virtual DbSet<Invoice> Invoices { get; set; }
        public virtual DbSet<singleinvoiceconsignment> singleinvoiceconsignments { get; set; }
        public virtual DbSet<Cash> Cashes { get; set; }
        public virtual DbSet<NEFT> NEFTs { get; set; }
        public virtual DbSet<CreditNote> CreditNotes { get; set; }
        public virtual DbSet<Stationary> Stationaries { get; set; }
        public virtual DbSet<ExpiredStationary> ExpiredStationaries { get; set; }
        public virtual DbSet<Holiday1> Holidays1 { get; set; }
        public virtual DbSet<Issuedstationary> Issuedstationaries { get; set; }
    
        public virtual ObjectResult<Sp_GetSingleConsignment_Result> Sp_GetSingleConsignment(string con_no)
        {
            var con_noParameter = con_no != null ?
                new ObjectParameter("Con_no", con_no) :
                new ObjectParameter("Con_no", typeof(string));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<Sp_GetSingleConsignment_Result>("Sp_GetSingleConsignment", con_noParameter);
        }
    }
}

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
    
    public partial class DB_A43B74_wingsgrowdbEntities : DbContext
    {
        public DB_A43B74_wingsgrowdbEntities()
            : base("name=DB_A43B74_wingsgrowdbEntities")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<ActivityLog> ActivityLogs { get; set; }
        public virtual DbSet<addcodamount> addcodamounts { get; set; }
        public virtual DbSet<addtopayamount> addtopayamounts { get; set; }
        public virtual DbSet<Admin> Admins { get; set; }
        public virtual DbSet<BoxEnvelopePacking> BoxEnvelopePackings { get; set; }
        public virtual DbSet<Cash> Cashes { get; set; }
        public virtual DbSet<Cheque> Cheques { get; set; }
        public virtual DbSet<Company> Companies { get; set; }
        public virtual DbSet<Complaint> Complaints { get; set; }
        public virtual DbSet<CreditNote> CreditNotes { get; set; }
        public virtual DbSet<deliverydata> deliverydatas { get; set; }
        public virtual DbSet<Destination> Destinations { get; set; }
        public virtual DbSet<Dtdc_Ecommerce> Dtdc_Ecommerce { get; set; }
        public virtual DbSet<Dtdc_Ptp> Dtdc_Ptp { get; set; }
        public virtual DbSet<dtdcPlu> dtdcPlus { get; set; }
        public virtual DbSet<EmailPromotion> EmailPromotions { get; set; }
        public virtual DbSet<Employee> Employees { get; set; }
        public virtual DbSet<Expense> Expenses { get; set; }
        public virtual DbSet<ExpiredStationary> ExpiredStationaries { get; set; }
        public virtual DbSet<express_cargo> express_cargo { get; set; }
        public virtual DbSet<Franchisee> Franchisees { get; set; }
        public virtual DbSet<GECrate> GECrates { get; set; }
        public virtual DbSet<GECSector> GECSectors { get; set; }
        public virtual DbSet<Holiday> Holidays { get; set; }
        public virtual DbSet<Holiday1> Holidays1 { get; set; }
        public virtual DbSet<Invoice> Invoices { get; set; }
        public virtual DbSet<InvoiceRemark> InvoiceRemarks { get; set; }
        public virtual DbSet<Issue> Issues { get; set; }
        public virtual DbSet<Issuedstationary> Issuedstationaries { get; set; }
        public virtual DbSet<NEFT> NEFTs { get; set; }
        public virtual DbSet<NewDtdc_Ecommerce> NewDtdc_Ecommerce { get; set; }
        public virtual DbSet<Nondox> Nondoxes { get; set; }
        public virtual DbSet<Notification> Notifications { get; set; }
        public virtual DbSet<Other_Service> Other_Service { get; set; }
        public virtual DbSet<Payment> Payments { get; set; }
        public virtual DbSet<pincodeZone> pincodeZones { get; set; }
        public virtual DbSet<popupimage> popupimages { get; set; }
        public virtual DbSet<Priority> Priorities { get; set; }
        public virtual DbSet<RaiseIssue> RaiseIssues { get; set; }
        public virtual DbSet<RaiseQuery> RaiseQueries { get; set; }
        public virtual DbSet<RateLaptop> RateLaptops { get; set; }
        public virtual DbSet<Ratem> Ratems { get; set; }
        public virtual DbSet<RateRevLaptop> RateRevLaptops { get; set; }
        public virtual DbSet<Receipt_details> Receipt_details { get; set; }
        public virtual DbSet<RedeemOtp> RedeemOtps { get; set; }
        public virtual DbSet<ReplyAdmin> ReplyAdmins { get; set; }
        public virtual DbSet<Saving> Savings { get; set; }
        public virtual DbSet<sectorName> sectorNames { get; set; }
        public virtual DbSet<Sector> Sectors { get; set; }
        public virtual DbSet<Sendmessage> Sendmessages { get; set; }
        public virtual DbSet<Service_list> Service_list { get; set; }
        public virtual DbSet<singleinvoiceconsignment> singleinvoiceconsignments { get; set; }
        public virtual DbSet<Stationary> Stationaries { get; set; }
        public virtual DbSet<Transaction> Transactions { get; set; }
        public virtual DbSet<User> Users { get; set; }
        public virtual DbSet<wallet_History> wallet_History { get; set; }
        public virtual DbSet<WalletPoint> WalletPoints { get; set; }
        public virtual DbSet<TransactionView> TransactionViews { get; set; }
    
        public virtual ObjectResult<dashboardData_Result> dashboardData(Nullable<System.DateTime> currentDate, string pfcode)
        {
            var currentDateParameter = currentDate.HasValue ?
                new ObjectParameter("currentDate", currentDate) :
                new ObjectParameter("currentDate", typeof(System.DateTime));
    
            var pfcodeParameter = pfcode != null ?
                new ObjectParameter("pfcode", pfcode) :
                new ObjectParameter("pfcode", typeof(string));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<dashboardData_Result>("dashboardData", currentDateParameter, pfcodeParameter);
        }
    
        public virtual ObjectResult<dashboardDataAllPf_Result> dashboardDataAllPf(Nullable<System.DateTime> currentDate)
        {
            var currentDateParameter = currentDate.HasValue ?
                new ObjectParameter("currentDate", currentDate) :
                new ObjectParameter("currentDate", typeof(System.DateTime));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<dashboardDataAllPf_Result>("dashboardDataAllPf", currentDateParameter);
        }
    
        public virtual ObjectResult<getConsignmentNumberSeries_Result> getConsignmentNumberSeries(string consignmentNumber, string pf_Code)
        {
            var consignmentNumberParameter = consignmentNumber != null ?
                new ObjectParameter("ConsignmentNumber", consignmentNumber) :
                new ObjectParameter("ConsignmentNumber", typeof(string));
    
            var pf_CodeParameter = pf_Code != null ?
                new ObjectParameter("pf_Code", pf_Code) :
                new ObjectParameter("pf_Code", typeof(string));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<getConsignmentNumberSeries_Result>("getConsignmentNumberSeries", consignmentNumberParameter, pf_CodeParameter);
        }
    
        public virtual ObjectResult<getInvoice_Result> getInvoice(Nullable<System.DateTime> fromDate, Nullable<System.DateTime> toDate, string customerid, Nullable<int> size, Nullable<int> pageNo)
        {
            var fromDateParameter = fromDate.HasValue ?
                new ObjectParameter("fromDate", fromDate) :
                new ObjectParameter("fromDate", typeof(System.DateTime));
    
            var toDateParameter = toDate.HasValue ?
                new ObjectParameter("toDate", toDate) :
                new ObjectParameter("toDate", typeof(System.DateTime));
    
            var customeridParameter = customerid != null ?
                new ObjectParameter("Customerid", customerid) :
                new ObjectParameter("Customerid", typeof(string));
    
            var sizeParameter = size.HasValue ?
                new ObjectParameter("size", size) :
                new ObjectParameter("size", typeof(int));
    
            var pageNoParameter = pageNo.HasValue ?
                new ObjectParameter("pageNo", pageNo) :
                new ObjectParameter("pageNo", typeof(int));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<getInvoice_Result>("getInvoice", fromDateParameter, toDateParameter, customeridParameter, sizeParameter, pageNoParameter);
        }
    
        public virtual ObjectResult<getInvoiceWithoutcompany_Result> getInvoiceWithoutcompany(Nullable<System.DateTime> fromDate, Nullable<System.DateTime> toDate, Nullable<int> size, Nullable<int> pageNo)
        {
            var fromDateParameter = fromDate.HasValue ?
                new ObjectParameter("fromDate", fromDate) :
                new ObjectParameter("fromDate", typeof(System.DateTime));
    
            var toDateParameter = toDate.HasValue ?
                new ObjectParameter("toDate", toDate) :
                new ObjectParameter("toDate", typeof(System.DateTime));
    
            var sizeParameter = size.HasValue ?
                new ObjectParameter("size", size) :
                new ObjectParameter("size", typeof(int));
    
            var pageNoParameter = pageNo.HasValue ?
                new ObjectParameter("pageNo", pageNo) :
                new ObjectParameter("pageNo", typeof(int));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<getInvoiceWithoutcompany_Result>("getInvoiceWithoutcompany", fromDateParameter, toDateParameter, sizeParameter, pageNoParameter);
        }
    
        public virtual ObjectResult<getNotification_Result> getNotification()
        {
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<getNotification_Result>("getNotification");
        }
    
        public virtual ObjectResult<getRemaining_Result> getRemaining(string pfCode)
        {
            var pfCodeParameter = pfCode != null ?
                new ObjectParameter("PfCode", pfCode) :
                new ObjectParameter("PfCode", typeof(string));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<getRemaining_Result>("getRemaining", pfCodeParameter);
        }
    
        public virtual ObjectResult<Sp_GetSingleConsignment_Result> Sp_GetSingleConsignment(string con_no)
        {
            var con_noParameter = con_no != null ?
                new ObjectParameter("Con_no", con_no) :
                new ObjectParameter("Con_no", typeof(string));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<Sp_GetSingleConsignment_Result>("Sp_GetSingleConsignment", con_noParameter);
        }
    
        public virtual ObjectResult<TransactionFromAndToDateRecords_Result> TransactionFromAndToDateRecords(Nullable<System.DateTime> fromdate, Nullable<System.DateTime> todate, string cus_no)
        {
            var fromdateParameter = fromdate.HasValue ?
                new ObjectParameter("Fromdate", fromdate) :
                new ObjectParameter("Fromdate", typeof(System.DateTime));
    
            var todateParameter = todate.HasValue ?
                new ObjectParameter("Todate", todate) :
                new ObjectParameter("Todate", typeof(System.DateTime));
    
            var cus_noParameter = cus_no != null ?
                new ObjectParameter("Cus_no", cus_no) :
                new ObjectParameter("Cus_no", typeof(string));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<TransactionFromAndToDateRecords_Result>("TransactionFromAndToDateRecords", fromdateParameter, todateParameter, cus_noParameter);
        }
    }
}

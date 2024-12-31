using DtDc_Billing.Entity_FR;
using DtDc_Billing.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.SqlServer;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DtDc_Billing.Controllers
{
    [SessionSupport]
    public class CustomerSupportController : Controller
    {

        private DB_A43B74_wingsgrowdbEntities db = new DB_A43B74_wingsgrowdbEntities();
        private DB_A43B74_wingsgrowdbEntities dc = new DB_A43B74_wingsgrowdbEntities();
        public ActionResult Dashboard()
        {
            ViewBag.stationary = db.ExpiredStationaries.Count();
            ViewBag.openconcount = db.TransactionViews.Where(m => (m.Customer_Id == "" || m.Customer_Id == null) && (m.Pf_Code == "" || m.Pf_Code != null)).Count();
            ViewBag.unsignpinc = (from user in db.Transactions
                                  where !db.Destinations.Any(f => f.Pincode == user.Pincode)
                                  select user.Pincode).Distinct().ToList().Count();
            ViewBag.invalidcon = (from user in db.Transactions
                                  where !db.Companies.Any(f => f.Company_Id == user.Customer_Id) && user.Customer_Id != null
                                  select user).Count();

            DateTime serverTime = DateTime.Now; // gives you current Time in server timeZone
            DateTime utcTime = serverTime.ToUniversalTime(); // convert it to Utc using timezone setting of server computer

            TimeZoneInfo tzi = TimeZoneInfo.FindSystemTimeZoneById("India Standard Time");
            DateTime localTime = TimeZoneInfo.ConvertTimeFromUtc(utcTime, tzi);

            double d = ((from od in db.TransactionViews
                         where (od.booking_date.Value.Day == localTime.Day)
                       && (od.booking_date.Value.Month == localTime.Month)
                       && (od.booking_date.Value.Year == localTime.Year)
                       && od.Customer_Id != null && (!od.Customer_Id.StartsWith("Cash")) && od.Customer_Id != "BASIC_TS" && od.Pf_Code != null
                         select new { od.Amount, od.Risksurcharge, od.loadingcharge }).Sum(m => (m.Amount) + (m.Risksurcharge ?? 0) + (m.loadingcharge ?? 0))) ?? 0;

            //  ViewBag.sumofbilling = d.ToString("##");
            ViewBag.sumofbilling = Math.Round(d);
            ViewBag.countofbilling = ((from od in db.TransactionViews
                                       where (od.booking_date.Value.Day == localTime.Day)
                                     && (od.booking_date.Value.Month == localTime.Month)
                                     && (od.booking_date.Value.Year == localTime.Year)
                                     && od.Customer_Id != null && (!od.Customer_Id.StartsWith("Cash")) && od.Customer_Id != "BASIC_TS"
                                       select od.Amount).Count());

            double avgsum = db.TransactionViews.Select(m => new { m.Customer_Id, m.Amount, m.Risksurcharge, m.loadingcharge, m.booking_date, m.Pf_Code, month = SqlFunctions.DatePart("month", m.booking_date) + "-" + SqlFunctions.DatePart("year", m.booking_date) }).Where(m => m.Customer_Id != null && (!m.Customer_Id.StartsWith("cash")) && m.Customer_Id != "BASIC_TS" && m.Pf_Code != null).GroupBy(m => m.month).Average(m => m.Sum(x => (x.Amount + (x.loadingcharge ?? 0) + (x.Risksurcharge ?? 0)))) ?? 0;

            ViewBag.avgofbillingsum = avgsum.ToString("##");
            double avgofbillingcount = db.TransactionViews.Select(m => new { m.Customer_Id, m.Amount, m.Pf_Code, m.booking_date, month = SqlFunctions.DatePart("month", m.booking_date) + "-" + SqlFunctions.DatePart("year", m.booking_date) }).Where(m => m.Customer_Id != null && (!m.Customer_Id.StartsWith("cash")) && m.Customer_Id != "BASIC_TS" && m.Pf_Code != null).GroupBy(m => m.month).Average(m => m.Count());
            ViewBag.avgofbillingcount = avgofbillingcount.ToString("##");
            double sumofbillingcurrentmonthd = db.TransactionViews.Where(m => m.Customer_Id != null && (!m.Customer_Id.StartsWith("cash")) && m.Customer_Id != "BASIC_TS" && m.Pf_Code != null && SqlFunctions.DatePart("month", m.booking_date) == DateTime.Now.Month && SqlFunctions.DatePart("year", m.booking_date) == DateTime.Now.Year).Sum(m => (m.Amount + (m.loadingcharge ?? 0) + (m.Risksurcharge ?? 0))) ?? 0;
            ViewBag.sumofbillingcurrentmonth = sumofbillingcurrentmonthd.ToString("##");

            ViewBag.countofbillingcurrentmonth = db.TransactionViews.Where(m => m.Customer_Id != null && (!m.Customer_Id.StartsWith("cash")) && m.Customer_Id != "BASIC_TS" && m.Pf_Code != null && SqlFunctions.DatePart("month", m.booking_date) == DateTime.Now.Month && SqlFunctions.DatePart("year", m.booking_date) == DateTime.Now.Year).Count();

            DateTime date = DateTime.Now;
            ViewBag.firstDayOfMonth = new DateTime(date.Year, date.Month, 1).ToString("dd-MM-yyyy");
            ViewBag.currentday = DateTime.Now.ToString("dd-MM-yyyy");

            DateTime abc = DateTime.Now;

            string Pf = "abcd"; /*Session["PfID"].ToString();*/

            int PfCount = db.Sectors.Where(m => m.Pf_code == Pf).Count();

            if (PfCount < 6)
            {
                ViewBag.RedirectSector = true;
            }

            ViewBag.complaintcount = db.Complaints.Where(x => x.C_Status == "Not Resolved").Count();
            return View();
        }

        public ActionResult FranchiseeList()
        {
            return View(db.Franchisees.ToList());
        }

        public ActionResult RateMaster()
        {
            return View(db.Companies.ToList());
        }


        public ActionResult Index(string id)
        {

            ViewBag.companyid = Server.UrlDecode(Request.Url.Segments[3]);
            id = id.Replace("__", "&").Replace("xdotx", "."); ;
            var CompanyId = id;

            Company company = db.Companies.Where(m => m.Company_Id == CompanyId).FirstOrDefault();


            @ViewBag.Slabs = db.Ratems.Where(m => m.Company_id == CompanyId).FirstOrDefault();

            @ViewBag.Slabs1 = db.Nondoxes.Where(m => m.Company_id == CompanyId).FirstOrDefault();

            @ViewBag.Slabspri = db.Priorities.Where(m => m.Company_id == CompanyId).FirstOrDefault();

            ViewBag.Company = db.Companies.Where(m => m.Company_Id == CompanyId).FirstOrDefault();

            ViewBag.Dox = db.Ratems.Where(m => m.Company_id == CompanyId && m.Sector.BillD == true).OrderBy(m => m.Sector.Priority).ToList();

            ViewBag.NonDox = db.Nondoxes.Where(m => m.Company_id == CompanyId && m.Sector.BillN == true).OrderBy(m => m.Sector.Priority).ToList();

            ViewBag.Plus = db.dtdcPlus.Where(m => m.Company_id == CompanyId).ToList();

            ViewBag.Ptp = db.Dtdc_Ptp.Where(m => m.Company_id == CompanyId).ToList();

            ViewBag.Cargo = db.express_cargo.Where(m => m.Company_id == CompanyId && m.Sector.BillD == true).Include(e => e.Sector).OrderBy(m => m.Sector.Priority).ToList();

            ViewBag.Priority = db.Priorities.Where(m => m.Company_id == CompanyId && (m.Sector.BillD == true || m.Sector.BillN == true)).OrderBy(m => m.Sector.Priority).ToList();
            //<-------------risk surch charge dropdown--------------->
            double? selectedval = db.Companies.Where(m => m.Company_Id == CompanyId).Select(m => m.Minimum_Risk_Charge).FirstOrDefault();


            List<SelectListItem> items = new List<SelectListItem>();

            items.Add(new SelectListItem { Text = "0", Value = "0" });
            items.Add(new SelectListItem { Text = "50", Value = "50" });
            items.Add(new SelectListItem { Text = "100", Value = "100" });

            if (selectedval == null)
            {
                var selected = items.Where(x => x.Value == "0").First();
                selected.Selected = true;
            }
            else
            {


                var selected = items.Where(x => x.Value == selectedval.ToString()).First();
                selected.Selected = true;
            }

            ViewBag.Minimum_Risk_Charge = items;

            //<-------------risk surch charge dropdown--------------->

            ViewBag.Pf_code = new SelectList(db.Franchisees, "PF_Code", "PF_Code", company.Pf_code);

            return View();
        }


        public ActionResult Checkbookinglist()
        {
            List<TransactionView> list = new List<TransactionView>();

            return View(list);
        }

        [HttpPost]
        public ActionResult Checkbookinglist(string Fromdatetime, string ToDatetime, string Custid, string Submit)
        {
            string[] formats = {"dd/MM/yyyy", "dd-MMM-yyyy", "yyyy-MM-dd",
                   "dd-MM-yyyy", "M/d/yyyy", "dd MMM yyyy"};


            DateTime? fromdate;
            DateTime? todate;

            if (Fromdatetime != "")
            {
                string bdatefrom = DateTime.ParseExact(Fromdatetime, formats, CultureInfo.InvariantCulture, DateTimeStyles.None).ToString("MM/dd/yyyy");
                fromdate = Convert.ToDateTime(bdatefrom);

                ViewBag.fromdate = Fromdatetime;
            }
            else
            {
                fromdate = DateTime.Now;
            }

            if (ToDatetime != "")
            {
                string bdateto = DateTime.ParseExact(ToDatetime, formats, CultureInfo.InvariantCulture, DateTimeStyles.None).ToString("MM/dd/yyyy");
                todate = Convert.ToDateTime(bdateto);
                ViewBag.todate = ToDatetime;
            }
            else
            {
                todate = DateTime.Now;
            }

            if (Custid != "")
            {
                ViewBag.Custid = Custid;
            }



            List<TransactionView> transactions =
                db.TransactionViews.Where(m =>
               (m.Customer_Id == Custid)
                    ).ToList().Where(m => m.booking_date.Value.Date >= fromdate.Value.Date && m.booking_date.Value.Date <= todate.Value.Date).OrderBy(m => m.booking_date).ThenBy(n => n.Consignment_no)
                           .ToList();





            ViewBag.totalamt = transactions.Sum(b => b.Amount);

            if (Submit == "Export to Excel")
            {
                var import = db.TransactionViews.Where(m =>
                (m.Customer_Id == Custid)
                    ).ToList().Where(m => m.booking_date.Value.Date >= fromdate.Value.Date && m.booking_date.Value.Date <= todate.Value.Date).OrderBy(m => m.booking_date).ThenBy(n => n.Consignment_no).Select(x => new { x.Consignment_no, Weight = x.chargable_weight, x.Name, x.Pincode, x.Type_t, x.Mode, x.Amount, BookingDate = x.tembookingdate, x.Insurance, x.Claimamount, x.Percentage, Risksurcharge = x.calinsuranceamount, Total = (x.Amount + x.calinsuranceamount) }).ToList();
                ExportToExcelAll.ExportToExcelAdmin(import);
            }


            return View(transactions);
        }


        public ActionResult CustomerIdAutocomplete()
        {


            var entity = db.Companies.
Select(e => new
{
    e.Company_Id
}).Distinct().ToList();


            return Json(entity, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Nobookinglist()
        {
            List<TransactionModel> list = new List<TransactionModel>();
            ViewBag.PfCode = new SelectList(db.Franchisees, "PF_Code", "PF_Code");
            return View(list);
        }

        [HttpPost]
        public ActionResult Nobookinglist(string Fromdatetime, string ToDatetime, string PfCode, string Submit)
        {

            string[] formats = {"dd/MM/yyyy", "dd-MMM-yyyy", "yyyy-MM-dd",
                   "dd-MM-yyyy", "M/d/yyyy", "dd MMM yyyy"};

            ViewBag.PfCode = new SelectList(db.Franchisees, "PF_Code", "PF_Code", PfCode);

            DateTime? fromdate;
            DateTime? todate;

            if (Fromdatetime != "")
            {
                string bdatefrom = DateTime.ParseExact(Fromdatetime, formats, CultureInfo.InvariantCulture, DateTimeStyles.None).ToString("MM/dd/yyyy");
                fromdate = Convert.ToDateTime(bdatefrom);

                ViewBag.fromdate = Fromdatetime;
            }
            else
            {
                fromdate = DateTime.Now;
            }

            if (ToDatetime != "")
            {
                string bdateto = DateTime.ParseExact(ToDatetime, formats, CultureInfo.InvariantCulture, DateTimeStyles.None).ToString("MM/dd/yyyy");
                todate = Convert.ToDateTime(bdateto);
                ViewBag.todate = ToDatetime;
            }
            else
            {
                todate = DateTime.Now;
            }



            List<TransactionModel> transactions = (from order in db.Transactions
                                                   join u in db.Issuedstationaries on order.Consignment_no equals u.consignmentno into ou
                                                   where (order.Customer_Id == null || order.Customer_Id == "") && (order.Pf_Code == PfCode || PfCode == "") &&
                                                   (order.booking_date >= fromdate && order.booking_date <= todate
                                                   )
                                                   from Issuedstationary in ou.DefaultIfEmpty()
                                                   select new TransactionModel()
                                                   {
                                                       Pf_Code = order.Pf_Code,
                                                       Consignment_no = order.Consignment_no,
                                                       Weight_t = order.Weight_t,
                                                       Pincode = order.Pincode,
                                                       Mode = order.Mode,
                                                       booking_date = order.booking_date,
                                                       calinsuranceamount = ou.FirstOrDefault().employeename,
                                                   }).ToList();


            if (Submit == "Export to Excel")
            {
                var import = db.Transactions.Where(m =>
                (m.Customer_Id == null || m.Customer_Id == "") && (m.Pf_Code == PfCode || PfCode == "")
                    ).OrderBy(m => m.booking_date).ToList().Where(m => m.booking_date.Value.Date >= fromdate.Value.Date && m.booking_date.Value.Date <= todate.Value.Date).OrderBy(m => m.booking_date).ThenBy(n => n.Consignment_no).Select(x => new { x.Pf_Code, x.Consignment_no, Weight = x.Actual_weight, x.Pincode, x.Amount, x.tembookingdate }).ToList();
                ExportToExcelAll.ExportToExcelAdmin(import);
            }


            return View(transactions);
        }

        public ActionResult weightdifference()
        {
           
            List<TransactionView> transactions = new List<TransactionView>();
            
            ViewBag.totalamt = transactions.Sum(b => b.Amount);

            return View(transactions);
        }

        [HttpPost]
        public ActionResult weightdifference(string Fromdatetime, string ToDatetime)
        {
            string[] formats = {"dd/MM/yyyy", "dd-MMM-yyyy", "yyyy-MM-dd",
                   "dd-MM-yyyy", "M/d/yyyy", "dd MMM yyyy"};


            DateTime? fromdate;
            DateTime? todate;

            if (Fromdatetime != "")
            {
                string bdatefrom = DateTime.ParseExact(Fromdatetime, formats, CultureInfo.InvariantCulture, DateTimeStyles.None).ToString("MM/dd/yyyy");
                fromdate = Convert.ToDateTime(bdatefrom);

                ViewBag.fromdate = Fromdatetime;
            }
            else
            {
                fromdate = DateTime.Now;
            }

            if (ToDatetime != "")
            {
                string bdateto = DateTime.ParseExact(ToDatetime, formats, CultureInfo.InvariantCulture, DateTimeStyles.None).ToString("MM/dd/yyyy");
                todate = Convert.ToDateTime(bdateto);
                ViewBag.todate = ToDatetime;
            }
            else
            {
                todate = DateTime.Now;
            }





            List<TransactionView> transactions =
                db.TransactionViews.Where(m =>
               (m.chargable_weight < m.diff_weight)
                    ).ToList().Where(m => m.booking_date.Value.Date >= fromdate.Value.Date && m.booking_date.Value.Date <= todate.Value.Date).OrderBy(m => m.booking_date).ThenBy(n => n.Consignment_no)
                           .ToList();





            ViewBag.totalamt = transactions.Sum(b => b.Amount);

            return View(transactions);
        }


        public ActionResult ViewInvoice()
        {
            return View(db.Invoices.Where(m => m.Total_Lable == null || m.Total_Lable.Length == 0).ToList());
        }


        public ActionResult ViewDPInvoice()
        {
            return View(db.Invoices.Where(m => m.Total_Lable != null || m.Total_Lable.Length > 0).ToList());
        }


        public ActionResult ViewSingleInvoice()
        {
            var temp = dc.singleinvoiceconsignments.Select(m => m.Invoice_no).ToList();



            var a = (from member in db.Invoices
                     where temp.Contains(member.invoiceno)
                     select member).ToList();



            return View(a);

        }

    }
}
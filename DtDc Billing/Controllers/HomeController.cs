using DtDc_Billing.Entity_FR;
using DtDc_Billing.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.Entity.SqlServer;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DtDc_Billing.CustomModel;
using System.Web.Script.Serialization;
using Microsoft.SqlServer.Management.Smo;

namespace DtDc_Billing.Controllers
{
    [SessionAdmin]
    public class HomeController : Controller
    {
        private DB_A43B74_wingsgrowdbEntities db = new DB_A43B74_wingsgrowdbEntities();

        //public ActionResult Index()
        //{
        //    //P46298028


        //    ViewBag.stationary = db.ExpiredStationaries.Count();
        //    ViewBag.openconcount = db.TransactionViews.Where(m => (m.Customer_Id == "" || m.Customer_Id == null) && (m.Pf_Code != null)).Count();
        //    ViewBag.unsignpinc = (from user in db.Transactions
        //                          where !db.Destinations.Any(f => f.Pincode == user.Pincode)
        //                          select user.Pincode).Distinct().ToList().Count();
        //    ViewBag.invalidcon = (from user in db.Transactions
        //                          where !db.Companies.Any(f => f.Company_Id == user.Customer_Id) && user.Customer_Id != null
        //                          select user).Count();

        //    DateTime abc = DateTime.Now;

        //    string Pf = "abcd"; /*Session["PfID"].ToString();*/

        //    int PfCount = db.Sectors.Where(m => m.Pf_code == Pf).Count();

        //    if (PfCount < 6)
        //    {
        //        ViewBag.RedirectSector=true;
        //    }

        //    return View();
        //}

        public ActionResult Index(string Pfcode= "")
        {
            //ViewBag.stationary = db.ExpiredStationaries.Count();
            //ViewBag.openconcount = db.TransactionViews.Where(m => (m.Customer_Id == "" || m.Customer_Id == null) && (m.Pf_Code == "" || m.Pf_Code != null)).Count();
            //ViewBag.unsignpinc = (from user in db.Transactions
            //                      where !db.Destinations.Any(f => f.Pincode == user.Pincode)
            //                      select user.Pincode).Distinct().ToList().Count();
            //ViewBag.invalidcon = (from user in db.Transactions
            //                      where !db.Companies.Any(f => f.Company_Id == user.Customer_Id) && user.Customer_Id != null
            //                      select user).Count();

            DateTime serverTime = DateTime.Now; // gives you current Time in server timeZone
            DateTime utcTime = serverTime.ToUniversalTime(); // convert it to Utc using timezone setting of server computer

            TimeZoneInfo tzi = TimeZoneInfo.FindSystemTimeZoneById("India Standard Time");
            DateTime localTime = TimeZoneInfo.ConvertTimeFromUtc(utcTime, tzi);
            
            DateTime After30days = serverTime.AddDays(30);

            var ExpiryCompanyCount = db.Companies.Select(m => new { m.StartDate, m.EndDate,m.IsAgreementoption }).Where(m => m.EndDate <= After30days && m.IsAgreementoption != 1).Count();
            ViewBag.ExpiryCompCount = ExpiryCompanyCount;


            if (Pfcode != "")
            {

                var obj = db.dashboardData(localTime, Pfcode).Select(x => new dashboardDataModel
                {
                    expiredStationaryCount = x.expiredStationaryCount ?? 0,
                    openConCount = x.openConCount ?? 0,
                    unSignPincode = x.unSignPincode ?? 0,
                    invalidCon = x.invalidCon ?? 0,
                    complaintCount = x.complaintcount ?? 0,
                    sumOfBilling = x.sumOfBilling ?? 0,
                    countOfBilling = x.countOfBilling ?? 0,
                    avgOfBillingSum = x.avgOfBillingSum ?? 0,
                    sumOfBillingCurrentMonth = x.sumOfBillingCurrentMonth ?? 0,
                    countofbillingcurrentmonth = x.countofbillingcurrentmonth ?? 0,
                    todayExp = x.todayExp ?? 0,
                    monthexp = x.monthexp ?? 0

                }).FirstOrDefault();

                //double avgofbillingcount = db.TransactionViews.Select(m => new { m.Customer_Id, m.Amount, m.Pf_Code, m.booking_date, month = SqlFunctions.DatePart("month", m.booking_date) + "-" + SqlFunctions.DatePart("year", m.booking_date) }).Where(m => m.Customer_Id != null && (!m.Customer_Id.StartsWith("cash")) && m.Customer_Id != "BASIC_TS" && m.Pf_Code == Pfcode).GroupBy(m => m.month).Average(m => m.Count());
                //ViewBag.avgofbillingcount = avgofbillingcount.ToString("##");
                double avgOfBillingCount = 0;
                var groupedData = db.TransactionViews
                    .Where(m => m.Customer_Id != null &&
                                !m.Customer_Id.StartsWith("cash") &&
                                m.Customer_Id != "BASIC_TS" &&
                                m.Pf_Code == Pfcode
                              &&  SqlFunctions.DatePart("month", m.booking_date)==SqlFunctions.DatePart("month", localTime)
                              && SqlFunctions.DatePart("year", m.booking_date)==SqlFunctions.DatePart("year",localTime)
                                )
                    .Select(m => new
                    {
                        m.Customer_Id,
                        m.Amount,
                        m.Pf_Code,
                        m.booking_date,
                        Day = SqlFunctions.DatePart("day", m.booking_date)
                    })
                    .GroupBy(m => m.Day)
                    .Select(g => new { MonthYear = g.Key, Count = g.Count() })
                    .ToList();

                if (groupedData.Any())
                    avgOfBillingCount = groupedData.Average(g => g.Count);

                ViewBag.avgofbillingcount = Math.Round(avgOfBillingCount);

                ViewBag.firstDayOfMonth = new DateTime(serverTime.Year, serverTime.Month, 1).ToString("dd-MM-yyyy");
                ViewBag.currentday = DateTime.Now.ToString("dd-MM-yyyy");
                ViewBag.complaintcount = obj.complaintCount;
                ViewBag.sumofbilling = obj.sumOfBilling;
                ViewBag.sumofbillingcurrentmonth = obj.sumOfBillingCurrentMonth.ToString("##");
                ViewBag.avgofbillingsum = obj.avgOfBillingSum.ToString("##");
                ViewBag.countofbilling = obj.countOfBilling;
                ViewBag.countofbillingcurrentmonth = obj.countofbillingcurrentmonth;

                ViewBag.openconcount = obj.openConCount;
                ViewBag.unsignpinc = obj.unSignPincode;
                ViewBag.invalidcon = obj.invalidCon;
                ViewBag.stationary = obj.expiredStationaryCount;
                obj.notificationsList = db.getNotification().Select(x => new Notification
                {
                    N_ID = x.N_ID,
                    Message = x.Message,
                    description = x.description,
                    dateN = x.dateN ?? serverTime,
                    url_path = x.url_path,
                    Status = x.Status

                }).Where(m => m.Status == true && DateTime.Now.Date <= m.dateN).ToList();

                ViewBag.notification = db.Notifications.ToList().OrderByDescending(m => m.dateN).Take(4);

                ViewBag.Pf_code = new SelectList(db.Franchisees.Where(x=> !x.PF_Code.ToUpper().StartsWith("PR")), "PF_Code", "PF_Code", Pfcode);

                ViewBag.pfforchart = Pfcode;

                return View(obj);

            }
            else
            {
                var objAll = db.dashboardDataAllPf(localTime).Select(x => new dashboardDataModel
                {
                    expiredStationaryCount = x.expiredStationaryCount ?? 0,
                    openConCount = x.openConCount ?? 0,
                    unSignPincode = x.unSignPincode ?? 0,
                    invalidCon = x.invalidCon ?? 0,
                    complaintCount = x.complaintcount ?? 0,
                    sumOfBilling = x.sumOfBilling ?? 0,
                    countOfBilling = x.countOfBilling ?? 0,
                    avgOfBillingSum = x.avgOfBillingSum ?? 0,
                    sumOfBillingCurrentMonth = x.sumOfBillingCurrentMonth ?? 0,
                    countofbillingcurrentmonth = x.countofbillingcurrentmonth ?? 0,
                    todayExp = x.todayExp ?? 0,
                    monthexp = x.monthexp ?? 0

                }).FirstOrDefault();

                //double avgofbillingcount = db.TransactionViews.Select(m => new { m.Customer_Id, m.Amount, m.Pf_Code, m.booking_date, month = SqlFunctions.DatePart("month", m.booking_date) + "-" + SqlFunctions.DatePart("year", m.booking_date) }).Where(m => m.Customer_Id != null && (!m.Customer_Id.StartsWith("cash")) && m.Customer_Id != "BASIC_TS" && m.Pf_Code != null).GroupBy(m => m.month).Average(m => m.Count());
               // ViewBag.avgofbillingcount = avgofbillingcount.ToString("##");

                ViewBag.firstDayOfMonth = new DateTime(serverTime.Year, serverTime.Month, 1).ToString("dd-MM-yyyy");
                ViewBag.currentday = DateTime.Now.ToString("dd-MM-yyyy");
                ViewBag.complaintcount = objAll.complaintCount;
                ViewBag.sumofbilling = objAll.sumOfBilling;
                ViewBag.sumofbillingcurrentmonth = objAll.sumOfBillingCurrentMonth.ToString("##");
                ViewBag.avgofbillingsum = objAll.avgOfBillingSum.ToString("##");
                ViewBag.countofbilling = objAll.countOfBilling;
                ViewBag.countofbillingcurrentmonth = objAll.countofbillingcurrentmonth;

                ViewBag.openconcount = objAll.openConCount;
                ViewBag.unsignpinc = objAll.unSignPincode;
                ViewBag.invalidcon = objAll.invalidCon;
                ViewBag.stationary = objAll.expiredStationaryCount;
                objAll.notificationsList = db.getNotification().Select(x => new Notification
                {
                    N_ID = x.N_ID,
                    Message = x.Message,
                    description = x.description,
                    dateN = x.dateN ?? serverTime,
                    url_path = x.url_path,
                    Status = x.Status

                }).Where(m => m.Status == true && DateTime.Now.Date <= m.dateN).ToList();

                ViewBag.notification = db.Notifications.ToList().OrderByDescending(m => m.dateN).Take(4);

                ViewBag.Pf_code = new SelectList(db.Franchisees.Where(x => !x.PF_Code.ToUpper().StartsWith("PR")), "PF_Code", "PF_Code", Pfcode);

                ViewBag.pfforchart = Pfcode;
                double avgOfBillingCount = 0;
                var groupedData = db.TransactionViews
                    .Where(m => m.Customer_Id != null &&
                                !m.Customer_Id.StartsWith("cash") &&
                                m.Customer_Id != "BASIC_TS"
                                 && SqlFunctions.DatePart("month", m.booking_date) == SqlFunctions.DatePart("month", localTime)
                              && SqlFunctions.DatePart("year", m.booking_date) == SqlFunctions.DatePart("year", localTime)

                                )
                    .Select(m => new
                    {
                        m.Customer_Id,
                        m.Amount,
                        m.Pf_Code,
                        m.booking_date,
                        Day = SqlFunctions.DatePart("day", m.booking_date)
                    })
                    .GroupBy(m => m.Day)
                    .Select(g => new { MonthYear = g.Key, Count = g.Count() })
                    .ToList();

                if (groupedData.Any())
                    avgOfBillingCount = groupedData.Average(g => g.Count);

                ViewBag.avgofbillingcount = Math.Round(avgOfBillingCount);


                return View(objAll);


            }
            //double d = ((from od in db.TransactionViews
            //             where (od.booking_date.Value.Day == localTime.Day)
            //           && (od.booking_date.Value.Month == localTime.Month)
            //           && (od.booking_date.Value.Year == localTime.Year)
            //           && od.Customer_Id != null && (!od.Customer_Id.StartsWith("Cash")) && od.Customer_Id != "BASIC_TS" && od.Pf_Code != null
            //             select new { od.Amount, od.Risksurcharge, od.loadingcharge }).Sum(m => (m.Amount) + (m.Risksurcharge ?? 0) + (m.loadingcharge ?? 0))) ?? 0;

            ////  ViewBag.sumofbilling = d.ToString("##");
            //ViewBag.sumofbilling = Math.Round(d);
            //ViewBag.countofbilling = ((from od in db.TransactionViews
            //                           where (od.booking_date.Value.Day == localTime.Day)
            //                         && (od.booking_date.Value.Month == localTime.Month)
            //                         && (od.booking_date.Value.Year == localTime.Year)
            //                         && od.Customer_Id != null && (!od.Customer_Id.StartsWith("Cash")) && od.Customer_Id != "BASIC_TS"
            //                           select od.Amount).Count());

            //double avgsum = db.TransactionViews.Select(m => new { m.Customer_Id, m.Amount, m.Risksurcharge, m.loadingcharge, m.booking_date, m.Pf_Code, month = SqlFunctions.DatePart("month", m.booking_date) + "-" + SqlFunctions.DatePart("year", m.booking_date) }).Where(m => m.Customer_Id != null && (!m.Customer_Id.StartsWith("cash")) && m.Customer_Id != "BASIC_TS" && m.Pf_Code != null).GroupBy(m => m.month).Average(m => m.Sum(x => (x.Amount + (x.loadingcharge ?? 0) + (x.Risksurcharge ?? 0)))) ?? 0;

            //ViewBag.avgofbillingsum = avgsum.ToString("##");


            //DateTime abc = DateTime.Now;

            //string Pf = "abcd"; /*Session["PfID"].ToString();*/

            //int PfCount = db.Sectors.Where(m => m.Pf_code == Pf).Count();

            //if (PfCount < 6)
            //{
            //    ViewBag.RedirectSector = true;
            //}

            //ViewBag.complaintcount = db.Complaints.Where(x => x.C_Status == "Not Resolved").Count();
           
           

            //return View();
        }


        public ActionResult Company()
        {
           
            return View();
        }


        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        

        public ActionResult CreateCompanyPartial()
        {
         

            return PartialView();
        }

        

        [HttpPost]
        public ActionResult CreateCompanyPartial(Company company)
        {
            if (!ModelState.IsValid)
            {
                // prepare and populate required data for the input fields
                // . . .

                return PartialView("Createoredit");
            }
            else
            {
                return PartialView(company);
            }
        }



        public ActionResult ajax1()
        {
            return View();
        }


        public JsonResult GetNotifications()
        {

            db.Configuration.ProxyCreationEnabled = false;

            var notifications = db.Notifications.OrderByDescending(m=>m.N_ID).ToList();
            
            return Json(notifications,JsonRequestBehavior.AllowGet);

        }


        ////////////////////////////////////////////

        public ActionResult steplinechart(string pfcode)
        {
            DateTime today = DateTime.Now;
            DateTime sixMonthsBack = today.AddMonths(-6);
            Console.WriteLine(today.ToShortDateString());
            Console.WriteLine(sixMonthsBack.ToShortDateString());

            string Todayda = Convert.ToString(today.Date.ToString("MM/dd/yyyy"));
            string[] Todaydate = Todayda.Split('/');
            string TodayMonth = Todaydate[0];
            string TodayYear = Todaydate[2];

            string da = Convert.ToString(sixMonthsBack.Date.ToString("MM/dd/yyyy"));
            string[] SixMonthBackdate = da.Split('/');
            string SixMonthBackMonth = SixMonthBackdate[0];
            string SixMonthBackYear = SixMonthBackdate[2];

            List<steplinechart> dataPoints = new List<steplinechart>();

            //var inv = db.Invoices.Select(m => new { m.netamount, m.invoicedate, month = SqlFunctions.DatePart("month", m.invoicedate) + "-" + SqlFunctions.DatePart("year", m.invoicedate) }).GroupBy(m => m.month).Select(m => new { netamount = m.Sum(c => c.netamount), month = SqlFunctions.DatePart("month", m.FirstOrDefault().invoicedate), Year = SqlFunctions.DatePart("year", m.FirstOrDefault().invoicedate), invoicedate= m.FirstOrDefault().invoicedate }).OrderBy(m => m.invoicedate).Take(12).ToList();

            if (pfcode == null || pfcode == "")
            {
                var inv1 = db.Invoices.Select(m => new { m.netamount, m.invoicedate, month = SqlFunctions.DatePart("month", m.invoicedate) + "-" + SqlFunctions.DatePart("year", m.invoicedate) }).GroupBy(m => m.month).Select(m => new { netamount = m.Sum(c => c.netamount), month = SqlFunctions.DatePart("month", m.FirstOrDefault().invoicedate), Year = SqlFunctions.DatePart("year", m.FirstOrDefault().invoicedate), invoicedate = m.FirstOrDefault().invoicedate }).OrderBy(m => m.invoicedate).Where(m => m.invoicedate >= sixMonthsBack && m.invoicedate <= today).Take(6).ToList();

                foreach (var i in inv1)
                {
                    //steplinechart data = new steplinechart(i.netamount, i.month, i.Year);
                    steplinechart data = new steplinechart(i.netamount, i.month, i.Year);
                    dataPoints.Add(data);
                }

                ViewBag.DataPoints = JsonConvert.SerializeObject(dataPoints);
            }
            else
            {
                //var inv1 = db.Invoices.Select(m => new { m.netamount, m.invoicedate, month = SqlFunctions.DatePart("month", m.invoicedate) + "-" + SqlFunctions.DatePart("year", m.invoicedate) }).GroupBy(m => m.month).Select(m => new { netamount = m.Sum(c => c.netamount), month = SqlFunctions.DatePart("month", m.FirstOrDefault().invoicedate), Year = SqlFunctions.DatePart("year", m.FirstOrDefault().invoicedate), invoicedate = m.FirstOrDefault().invoicedate }).OrderBy(m => m.invoicedate).Where(m => m.invoicedate >= sixMonthsBack && m.invoicedate <= today).ToList();

                var inv1 = (from inv in db.Invoices
                            join c in db.Companies on inv.Customer_Id equals c.Company_Id
                            where c.Pf_code == pfcode
                            select (new { inv.netamount, inv.invoicedate, c.Pf_code, month = SqlFunctions.DatePart("month", inv.invoicedate) + "-" + SqlFunctions.DatePart("year", inv.invoicedate) }))
                           .GroupBy(m => m.month).Select(m => new
                           {
                               netamount = m.Sum(c => c.netamount),
                               month = SqlFunctions.DatePart("month", m.FirstOrDefault().invoicedate),
                               Year = SqlFunctions.DatePart("year", m.FirstOrDefault().invoicedate),
                               invoicedate = m.FirstOrDefault().invoicedate
                           }).OrderBy(m => m.invoicedate)
                               .Where(m => m.invoicedate >= sixMonthsBack && m.invoicedate <= today).Take(6).ToList();

                foreach (var i in inv1)
                {
                    //steplinechart data = new steplinechart(i.netamount, i.month, i.Year);
                    steplinechart data = new steplinechart(i.netamount, i.month, i.Year);
                    dataPoints.Add(data);
                }

                ViewBag.DataPoints = JsonConvert.SerializeObject(dataPoints);
            }
            return View();
        }

        public ActionResult Jschart(string pfcode)
        {
            DateTime today = DateTime.Now;
            DateTime sixMonthsBack = today.AddMonths(-6);
            Console.WriteLine(today.ToShortDateString());
            Console.WriteLine(sixMonthsBack.ToShortDateString());

            string Todayda = Convert.ToString(today.Date.ToString("MM-dd-yyyy"));
            string[] Todaydate = Todayda.Split('-');
            string TodayMonth = Todaydate[0];
            string TodayYear = Todaydate[2];

            string da = Convert.ToString(sixMonthsBack.Date.ToString("MM-dd-yyyy"));
            string[] SixMonthBackdate = da.Split('-');
            string SixMonthBackMonth = SixMonthBackdate[0];
            string SixMonthBackYear = SixMonthBackdate[2];

            List<steplinechart> dataPoints = new List<steplinechart>();

            //var inv = db.Invoices.Select(m => new { m.netamount, m.invoicedate, month = SqlFunctions.DatePart("month", m.invoicedate) + "-" + SqlFunctions.DatePart("year", m.invoicedate) }).GroupBy(m => m.month).Select(m => new { netamount = m.Sum(c => c.netamount), month = SqlFunctions.DatePart("month", m.FirstOrDefault().invoicedate), Year = SqlFunctions.DatePart("year", m.FirstOrDefault().invoicedate), invoicedate= m.FirstOrDefault().invoicedate }).OrderBy(m => m.invoicedate).Take(12).ToList();

            if (pfcode == null || pfcode == "")
            {
                var inv1 = db.Invoices.Select(m => new { m.netamount, m.invoicedate, month = SqlFunctions.DatePart("month", m.invoicedate) + "-" + SqlFunctions.DatePart("year", m.invoicedate) }).GroupBy(m => m.month).Select(m => new { netamount = m.Sum(c => c.netamount), month = SqlFunctions.DatePart("month", m.FirstOrDefault().invoicedate), Year = SqlFunctions.DatePart("year", m.FirstOrDefault().invoicedate), invoicedate = m.FirstOrDefault().invoicedate }).OrderBy(m => m.invoicedate).Where(m => m.invoicedate >= sixMonthsBack && m.invoicedate <= today).Take(6).ToList();

                foreach (var i in inv1)
                {
                    //steplinechart data = new steplinechart(i.netamount, i.month, i.Year);
                    steplinechart data = new steplinechart(i.netamount, i.month, i.Year);
                    dataPoints.Add(data);
                }

                ViewBag.DataPoints = JsonConvert.SerializeObject(dataPoints);
            }
            else
            {
                //var inv1 = db.Invoices.Select(m => new { m.netamount, m.invoicedate, month = SqlFunctions.DatePart("month", m.invoicedate) + "-" + SqlFunctions.DatePart("year", m.invoicedate) }).GroupBy(m => m.month).Select(m => new { netamount = m.Sum(c => c.netamount), month = SqlFunctions.DatePart("month", m.FirstOrDefault().invoicedate), Year = SqlFunctions.DatePart("year", m.FirstOrDefault().invoicedate), invoicedate = m.FirstOrDefault().invoicedate }).OrderBy(m => m.invoicedate).Where(m => m.invoicedate >= sixMonthsBack && m.invoicedate <= today).ToList();

                var inv1 = (from inv in db.Invoices
                            join c in db.Companies on inv.Customer_Id equals c.Company_Id
                            where c.Pf_code == pfcode
                            select (new { inv.netamount, inv.invoicedate, c.Pf_code, month = SqlFunctions.DatePart("month", inv.invoicedate) + "-" + SqlFunctions.DatePart("year", inv.invoicedate) }))
                           .GroupBy(m => m.month).Select(m => new
                           {
                               netamount = m.Sum(c => c.netamount),
                               month = SqlFunctions.DatePart("month", m.FirstOrDefault().invoicedate),
                               Year = SqlFunctions.DatePart("year", m.FirstOrDefault().invoicedate),
                               invoicedate = m.FirstOrDefault().invoicedate
                           }).OrderBy(m => m.invoicedate)
                               .Where(m => m.invoicedate >= sixMonthsBack && m.invoicedate <= today).Take(6).ToList();

                foreach (var i in inv1)
                {
                    //steplinechart data = new steplinechart(i.netamount, i.month, i.Year);
                    steplinechart data = new steplinechart(i.netamount, i.month, i.Year);
                    dataPoints.Add(data);
                }

                ViewBag.DataPoints = JsonConvert.SerializeObject(dataPoints);
            }
            return View();
        }

        /// ////////////////////////////////////////////////

        public ActionResult Salesstatistics()
        {
            ViewBag.PfCode = new SelectList(db.Franchisees.Where(x => !x.PF_Code.ToUpper().StartsWith("PR")), "PF_Code", "PF_Code");

            return View();
        }

        public JsonResult PivotData(string Pf_Code)
        {
            //var results = (from c in db.Invoices
            //               group c by new
            //               {
            //                   month = SqlFunctions.DatePart("month", c.invoicedate) + "-" + SqlFunctions.DatePart("year", c.invoicedate),
            //                   c.Customer_Id,
            //               } into gcs
            //               select new
            //               {
            //                   Customer_id = gcs.Key.Customer_Id,
            //                   month = gcs.Key.month,
            //                   NetAmount = gcs.Sum(c => c.netamount),
            //               }).ToList();

            //return Json(results, JsonRequestBehavior.AllowGet);

            // Get the date range for the previous two years
            DateTime currentDate = DateTime.Now;
            DateTime startDate = currentDate.AddYears(-2).AddDays(1 - currentDate.DayOfYear); // Start of 2 years ago

            var results = (from c in db.Invoices
                           where c.invoicedate >= startDate && c.invoicedate <= currentDate
                           group c by new
                           {
                               month = SqlFunctions.DatePart("month", c.invoicedate) + "-" + SqlFunctions.DatePart("year", c.invoicedate),
                               c.Customer_Id,
                           } into gcs
                           select new
                           {
                               Customer_id = gcs.Key.Customer_Id,
                               month = gcs.Key.month,
                               NetAmount = gcs.Sum(c => c.netamount),
                           }).ToList();
            var companies = db.Companies.Where(x => x.Pf_code == Pf_Code).Select(x => x.Company_Id).ToList();


            // Filter results for matching company data
            var filteredResults = results
                .Where(r => companies.Contains(r.Customer_id))
                .ToList();
            return Json(filteredResults, JsonRequestBehavior.AllowGet);
        }

        
    }
}
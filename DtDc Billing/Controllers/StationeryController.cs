using DtDc_Billing.Entity_FR;
using DtDc_Billing.Models;
using PagedList;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DtDc_Billing.Controllers
{
    [SessionAdmin]
    public class StationeryController : Controller
    {
        private DB_A43B74_wingsgrowdbEntities db = new DB_A43B74_wingsgrowdbEntities();
        // GET: Stationery
        public ActionResult Add()
        {
            ViewBag.Pf_code = new SelectList(db.Franchisees, "PF_Code", "PF_Code");
            return View();
        }


        [HttpPost]
        public ActionResult Add(Stationary stationary)
        {
            string[] formats = { "dd/MM/yyyy", "dd-MMM-yyyy", "yyyy-MM-dd", "dd-MM-yyyy", "M/d/yyyy", "dd MMM yyyy" };

           
                if (ModelState.IsValid)
                {

                    string invdate = DateTime.ParseExact(stationary.temprecdate, formats, CultureInfo.InvariantCulture, DateTimeStyles.None).ToString("MM/dd/yyyy");
                    stationary.Expiry_Date = Convert.ToDateTime(invdate);
                    stationary.Status = 0;
                    db.Stationaries.Add(stationary);

                     db.SaveChanges();


                    ViewBag.Pf_code = new SelectList(db.Franchisees, "PF_Code", "PF_Code", stationary.Pf_code);
                    ViewBag.Message = "Stationary Added SuccessFully";
                    ModelState.Clear();
                    return View();

                }
           
            ViewBag.Pf_code = new SelectList(db.Franchisees, "PF_Code", "PF_Code", stationary.Pf_code);
         
            return View(stationary);
        }





        public ActionResult Issue()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Issue(IssueStationaryModel issue)
        {

            if (ModelState.IsValid)
            {
                //issue.Pf_code = Session["PfID"].ToString();
                char stch = issue.startno[0];
                char Endch = issue.endno[0];

                long startConsignment = Convert.ToInt64(issue.startno.Substring(1));
                long EndConsignment = Convert.ToInt64(issue.endno.Substring(1));

                var diff = EndConsignment - startConsignment;
                int countconsigmnets = 0;

                if (diff < 100)
                {
                    if (stch == Endch)
                    {
                        for (long i = startConsignment; i <= EndConsignment; i++)
                        {
                            string updateconsignment = stch + i.ToString();


                            //Transaction transaction = db.Transactions.Where(m => m.Consignment_no == updateconsignment).FirstOrDefault();
                            Issuedstationary issuedstationary = db.Issuedstationaries.Where(m => m.consignmentno == updateconsignment).FirstOrDefault();

                            Issuedstationary issuedstationary1 = new Issuedstationary();

                            if (issuedstationary != null)
                            {



                                issuedstationary.employeename = issue.EmployeeName;


                                db.Entry(issuedstationary).State = EntityState.Modified;
                                db.SaveChanges();
                            }
                            else
                            {

                                issuedstationary1.consignmentno = updateconsignment;
                                issuedstationary1.employeename = issue.EmployeeName;
                                TimeZoneInfo INDIAN_ZONE = TimeZoneInfo.FindSystemTimeZoneById("India Standard Time");
                                issuedstationary1.date = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, INDIAN_ZONE);
                                db.Issuedstationaries.Add(issuedstationary1);
                                db.SaveChanges();
                            }

                            countconsigmnets++;

                            if (countconsigmnets >= 100)
                            {
                                break;
                            }

                        }
                        Issue isssta = new Issue();

                        isssta.startno = issue.startno;
                        isssta.endno = issue.endno;
                        isssta.noofleafs = issue.noofleafs;
                        isssta.Inssuedate = issue.Inssuedate;
                        isssta.Comapny_Id = issue.Comapny_Id;
                        isssta.EmployeeName = issue.EmployeeName;
                        //isssta.Pf_code = issue.Pf_code;


                        db.Issues.Add(isssta);
                        db.SaveChanges();
                        ViewBag.Message = "Issue Added SuccessFully";

                        ModelState.Clear();
                        return View(new IssueStationaryModel());
                    }
                }
                else
                {
                    ViewBag.Message1 = "You can only issue upto 100 consignments";
                    return View(issue);
                }


            }

            return View(issue);
        }


        [HttpGet]
        public ActionResult Remaining(string pfcode="" ,int page = 1, int pagsize = 10)
        {
            if(pfcode == "")
            pfcode = db.Franchisees.Select(m => m.PF_Code).First();
            List<Stationary> st = db.Stationaries.Where(m=>m.Pf_code == pfcode).OrderByDescending(x=>x.S_id).ToList();
            PagedList<Stationary> model = new PagedList<Stationary>(st, page, pagsize);



         //   var st = db.Stationaries.ToList();
             List<string> str = new List<string>();


            foreach (var j in model)
            {

                int counter = 0;

                char stch = j.startno[0];
                char Endch = j.endno[0];

                long startConsignment = Convert.ToInt64(j.startno.Substring(1));
                long EndConsignment = Convert.ToInt64(j.endno.Substring(1));


                List<string> str1 = new List<string>();

                for (long i = startConsignment; i <= EndConsignment; i++)
                {
                    string updateconsignment = stch + i.ToString();
                    str1.Add(updateconsignment);
                    
                    //Transaction transaction = db.Transactions.Where(m => m.Consignment_no == updateconsignment).FirstOrDefault();


                    //if (transaction != null && transaction.Customer_Id != null && transaction.Customer_Id.Length > 1)
                    //{
                    //    counter++;
                    //}


                }

                counter = db.Transactions.Where(m => str1.Contains(m.Consignment_no) && m.Customer_Id.Length >1).Count();

                str.Add(counter.ToString());
                counter = 0;




            }


            ViewBag.str = str.ToArray();
            ViewBag.pfcode1 = pfcode;
            ViewBag.PfCode = new SelectList(db.Franchisees, "PF_Code", "PF_Code", pfcode);
            return View(model);

        }

        

        public JsonResult RemainingConsignments(string startno, string endno)
        {





            List<string> Consignments = new List<string>();




            char stch = startno[0];
            char Endch = endno[0];

            long startConsignment = Convert.ToInt64(startno.Substring(1));
            long EndConsignment = Convert.ToInt64(endno.Substring(1));



            for (long i = startConsignment; i <= EndConsignment; i++)
            {
                string updateconsignment = stch + i.ToString();


                Transaction transaction = db.Transactions.Where(m => m.Consignment_no == updateconsignment).FirstOrDefault();



                if (transaction == null || transaction.Customer_Id == null || transaction.Customer_Id.Length == 0)
                {
                    Consignments.Add(updateconsignment);
                }








            }

            return Json(Consignments, JsonRequestBehavior.AllowGet);

        }


        public ActionResult IsseueRemaining()
        {
            var st = db.Issues.ToList();

            List<string> str = new List<string>();


            foreach (var j in st)
            {

                int counter = 0;

                char stch = j.startno[0];
                char Endch = j.endno[0];

                long startConsignment = Convert.ToInt64(j.startno.Substring(1));
                long EndConsignment = Convert.ToInt64(j.endno.Substring(1));



                for (long i = startConsignment; i <= EndConsignment; i++)
                {
                    string updateconsignment = stch + i.ToString();


                    Transaction transaction = db.Transactions.Where(m => m.Consignment_no == updateconsignment).FirstOrDefault();


                    if (transaction != null && transaction.Customer_Id != null && transaction.Customer_Id.Length > 1)
                    {
                        counter++;
                    }


                }


                str.Add(counter.ToString());
                counter = 0;




            }

            ViewBag.str = str.ToArray();

            ViewBag.Pf_code = new SelectList(db.Franchisees, "PF_Code", "PF_Code");
            return View(st);

        }

        public ActionResult Employeeautocomplete()
        {


            var entity = db.Issues.
Select(e => new
{
    e.EmployeeName
}).Distinct().ToList();


            return Json(entity, JsonRequestBehavior.AllowGet);
        }


        public ActionResult ExpiredStationary()
        {
             

            return View(db.ExpiredStationaries.ToList());


        }


        public ActionResult RemaningExpired()
        {

            var stationary = db.ExpiredStationaries.ToList().Where(m => DateTime.Now.Date >= m.Expiry_Date.Value.AddDays(60)).ToList();

            return View(stationary);


        }


        public ActionResult Stationary()
        {
            //Stationary stationary = db.Stationaries.ToList();

            //if (stationary == null)
            //{
            //    return HttpNotFound();
            //}
            return View(db.Stationaries);


        }


        public ActionResult EditStationary(long id)
        {
            Stationary stationary = db.Stationaries.Find(id);

            if (stationary == null)
            {
                return HttpNotFound();
            }
            ViewBag.Pf_code = new SelectList(db.Franchisees, "PF_Code", "PF_Code", stationary.Pf_code);
            return View(stationary);         


        }

        [HttpPost]
        public ActionResult EditStationary(Stationary stationary)
        {

            string[] formats = { "dd/MM/yyyy", "dd-MMM-yyyy", "yyyy-MM-dd", "dd-MM-yyyy", "M/d/yyyy", "dd MMM yyyy" };

            if (ModelState.IsValid)
            {

                string invdate = DateTime.ParseExact(stationary.temprecdate, formats, CultureInfo.InvariantCulture, DateTimeStyles.None).ToString("MM/dd/yyyy");
                stationary.Expiry_Date = Convert.ToDateTime(invdate);
                stationary.Status = 0;
                db.Entry(stationary).State=System.Data.Entity.EntityState.Modified;

                db.SaveChanges();


                ViewBag.Pf_code = new SelectList(db.Franchisees, "PF_Code", "PF_Code", stationary.Pf_code);
                ViewBag.Message = "Stationary Updated SuccessFully";
                ModelState.Clear();
                return View();

            }

            ViewBag.Pf_code = new SelectList(db.Franchisees, "PF_Code", "PF_Code", stationary.Pf_code);
            return View(stationary);


        }

     


    }
}
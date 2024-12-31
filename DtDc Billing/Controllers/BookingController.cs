using DtDc_Billing.Entity_FR;
using DtDc_Billing.Metadata_Classes;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DtDc_Billing.Models;
using System.Data.Entity.Validation;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Math;
using System.Diagnostics;
using OfficeOpenXml;
using System.IO;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Text;

namespace DtDc_Billing.Controllers
{
    [SessionAdmin]
    public class BookingController : Controller
    {
        private DB_A43B74_wingsgrowdbEntities db = new DB_A43B74_wingsgrowdbEntities();
        // GET: Booking
        public ActionResult ConsignMent()
        {
            return View();
        }

        public JsonResult Consignmentdetails(string Cosignmentno)
        {
            db.Configuration.ProxyCreationEnabled = false;

            //var suggestions = (from s in db.Transactions
            //                   where s.Consignment_no == Cosignmentno
            //                   select s).FirstOrDefault();

            var suggestions = db.Sp_GetSingleConsignment(Cosignmentno).FirstOrDefault();

            return Json(suggestions, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult SaveEditConsignment(TransactionMetadata transaction)
        {

            //if (transaction.Insurance != "yes")
            //{
            //    transaction.BillAmount = 0;                
            //    transaction.Risksurcharge = 0;
            //    transaction.Invoice_No = 0;
            //}

            if (transaction.topay != "yes")
            {
                transaction.Topaycharges = 0;
                transaction.consignee = "0";
                transaction.TopayAmount = 0;
            }
            if (transaction.cod != "yes")
            {
                transaction.codAmount = 0;
                transaction.codcharges = 0;
                transaction.consigner = "0";
                transaction.codtotalamount = 0;
            }


            ViewBag.Customerid = transaction.Customer_Id;

            if (ModelState.IsValid)
            {
                Transaction tr = db.Transactions.Where(m => m.Consignment_no == transaction.Consignment_no).FirstOrDefault();


                string[] formats = { "dd/MM/yyyy", "dd-MMM-yyyy", "yyyy-MM-dd", "dd-MM-yyyy", "M/d/yyyy", "dd MMM yyyy" };


                string bdate = DateTime.ParseExact(transaction.tembookingdate, formats, CultureInfo.InvariantCulture, DateTimeStyles.None).ToString("MM/dd/yyyy");

                transaction.booking_date = Convert.ToDateTime(bdate);

                if (tr != null)
                {

                    db.Entry(tr).State = EntityState.Detached;

                    //////////////////////////
                    Transaction tran = new Transaction();

                    tran.Customer_Id = transaction.Customer_Id;
                    tran.booking_date = transaction.booking_date;
                    tran.Consignment_no = transaction.Consignment_no.Trim();
                    tran.Pincode = transaction.Pincode;
                    tran.Mode = transaction.Mode;
                    tran.Weight_t = transaction.Weight_t;
                    tran.Amount = transaction.Amount;
                    tran.Company_id = transaction.Company_id;
                    tran.ReceiverName = transaction.ReceiverName;

                    tran.Quanntity = transaction.Quanntity;
                    tran.Type_t = transaction.Type_t;
                    tran.Insurance = transaction.Insurance;
                    tran.Claimamount = transaction.Claimamount;
                    tran.Percentage = transaction.Percentage;

                    tran.Claimamount = transaction.Claimamount;
                    tran.remark = transaction.remark;
                    tran.topay = transaction.topay;
                    tran.codAmount = transaction.codAmount;
                    tran.consignee = transaction.consigner;
                    tran.cod = transaction.cod;
                    tran.TopayAmount = transaction.TopayAmount;
                    tran.Topaycharges = transaction.Topaycharges;
                    tran.Actual_weight = transaction.Actual_weight;
                    tran.codcharges = transaction.codcharges;
                    tran.codAmount = transaction.codAmount;
                    tran.dtdcamount = transaction.dtdcamount;
                    tran.chargable_weight = transaction.chargable_weight;
                    tran.status_t = transaction.status_t;
                    tran.rateperkg = transaction.rateperkg;
                    tran.docketcharege = transaction.docketcharege;
                    tran.fovcharge = transaction.fovcharge;
                    tran.loadingcharge = transaction.loadingcharge;
                    tran.odocharge = transaction.odocharge;
                    tran.Risksurcharge = transaction.Risksurcharge;
                    tran.Invoice_No = transaction.Invoice_No;
                    tran.BillAmount = transaction.BillAmount;
                    tran.tembookingdate = transaction.tembookingdate;
                    tran.codtotalamount = transaction.codtotalamount;
                    tran.consigner = transaction.consigner;
                    tran.Packingcharges=transaction.Packingcharges;
                    tran.GEC3Additioncharges = transaction.GEC3Additioncharges;
                    tran.Handlingcharges = transaction.Handlingcharges;

                    tran.Pf_Code = db.Companies.Where(m => m.Company_Id == transaction.Customer_Id).Select(m => m.Pf_code).FirstOrDefault();
                    tran.AdminEmp = 000;

                    /////////////////////////////

                    tran.T_id = tr.T_id;

                    db.Entry(tran).State = EntityState.Modified;
                    db.SaveChanges();
                    ViewBag.Message = "Consignment Updated SuccessFully";
                }
                else
                {

                    Transaction tran1 = new Transaction();

                    tran1.Customer_Id = transaction.Customer_Id;
                    tran1.booking_date = transaction.booking_date;
                    tran1.Consignment_no = transaction.Consignment_no.Trim();
                    tran1.Pincode = transaction.Pincode;
                    tran1.Mode = transaction.Mode;
                    tran1.Weight_t = transaction.Weight_t;
                    tran1.Amount = transaction.Amount;
                    tran1.Company_id = transaction.Company_id;
                    tran1.Quanntity = transaction.Quanntity;
                    tran1.Type_t = transaction.Type_t;
                    tran1.Insurance = transaction.Insurance;
                    tran1.Claimamount = transaction.Claimamount;
                    tran1.Percentage = transaction.Percentage;

                    tran1.ReceiverName = transaction.ReceiverName;

                    tran1.Claimamount = transaction.Claimamount;
                    tran1.remark = transaction.remark;
                    tran1.topay = transaction.topay;
                    tran1.codAmount = transaction.codAmount;
                    tran1.consignee = transaction.consigner;
                    tran1.cod = transaction.cod;
                    tran1.TopayAmount = transaction.TopayAmount;
                    tran1.Topaycharges = transaction.Topaycharges;
                    tran1.Actual_weight = transaction.Actual_weight;
                    tran1.codcharges = transaction.codcharges;
                    tran1.codAmount = transaction.codAmount;
                    tran1.dtdcamount = transaction.dtdcamount;
                    tran1.chargable_weight = transaction.chargable_weight;
                    tran1.status_t = transaction.status_t;
                    tran1.rateperkg = transaction.rateperkg;
                    tran1.docketcharege = transaction.docketcharege;
                    tran1.fovcharge = transaction.fovcharge;
                    tran1.loadingcharge = transaction.loadingcharge;
                    tran1.odocharge = transaction.odocharge;
                    tran1.Risksurcharge = transaction.Risksurcharge;
                    tran1.Invoice_No = transaction.Invoice_No;
                    tran1.BillAmount = transaction.BillAmount;
                    tran1.tembookingdate = transaction.tembookingdate;
                    tran1.codtotalamount = transaction.codtotalamount;
                    tran1.consigner = transaction.consigner;
                    tran1.Packingcharges = transaction.Packingcharges;
                    tran1.GEC3Additioncharges = transaction.GEC3Additioncharges;
                    tran1.Handlingcharges = transaction.Handlingcharges;


                    tran1.Pf_Code = db.Companies.Where(m => m.Company_Id == transaction.Customer_Id).Select(m => m.Pf_code).FirstOrDefault();
                    tran1.AdminEmp = 000;
                    db.Transactions.Add(tran1);
                    db.SaveChanges();

                    Jobclass jobclass = new Jobclass();
                    jobclass.deletefromExpiry(tran1.Consignment_no);

                    ViewBag.Message = "Consignment Booked SuccessFully";
                }


                ModelState.Clear();

                ViewBag.success = true;

                //char ch = transaction.Consignment_no[0];

                //long consignnumber = Convert.ToInt64(transaction.Consignment_no.Substring(1));

                //consignnumber = consignnumber + 1;


                //string conno = Convert.ToString(consignnumber);

                //ViewBag.nextconsignment = ch + "" + conno.PadLeft(8, '0');


                if (transaction.Consignment_no.ToLower().StartsWith("7d"))
                {
                    string ch = transaction.Consignment_no.Substring(0, 2);
                    long consignnumber = Convert.ToInt64(transaction.Consignment_no.Substring(2));
                    long consignnumberadd = consignnumber + 1;
                    var lenght = transaction.Consignment_no.Substring(2).Length;

                    ViewBag.nextconsignment = ch + "" + (consignnumberadd.ToString().PadLeft(lenght, '1'));
                }

                else if (transaction.Consignment_no.ToLower().StartsWith("7x"))
                {
                    string ch = transaction.Consignment_no.Substring(0, 2);
                    long consignnumber = Convert.ToInt64(transaction.Consignment_no.Substring(2));
                    consignnumber = consignnumber + 1;
                    var lenght = transaction.Consignment_no.Substring(2).Length;

                    ViewBag.nextconsignment = ch + "" + (consignnumber.ToString().PadLeft(lenght, '1'));
                }

                else
                {
                    char ch = transaction.Consignment_no[0];

                    long consignnumber = Convert.ToInt64(transaction.Consignment_no.Substring(1));

                    consignnumber = consignnumber + 1;

                    var lenght = transaction.Consignment_no.Substring(1).Length;

                    ViewBag.nextconsignment = ch + "" + (consignnumber.ToString().PadLeft(lenght, '0'));

                }

                return PartialView("ConsignmentPartial");
            }

            return PartialView("ConsignmentPartial", transaction);
        }

        public ActionResult CustomerIdAutocomplete()
        {


            var entity = db.Companies.
Select(e => new
{
    e.Company_Id,
    e.IsAgreementoption

}).Where(e => e.IsAgreementoption != 1).Distinct().ToList();


            return Json(entity, JsonRequestBehavior.AllowGet);
        }

        public ActionResult CustomerIdReceipt(string id)
        {


            var entity = db.Companies.Where(m => m.Pf_code == id).
Select(e => new
{
    e.Company_Id,
    e.IsAgreementoption
}).Where(e => e.IsAgreementoption != 1).Distinct().ToList();


            return Json(entity, JsonRequestBehavior.AllowGet);
        }


        public ActionResult PincodeautocompleteSender()
        {


            var entity = db.Destinations.
Select(e => new
{
    e.Pincode
}).ToList();


            return Json(entity, JsonRequestBehavior.AllowGet);
        }

        public JsonResult CustomerDetails(string CustomerId)
        {
            db.Configuration.ProxyCreationEnabled = false;

            var suggestions = (from s in db.Companies
                               where s.Company_Id == CustomerId
                               select s).FirstOrDefault();

            return Json(suggestions, JsonRequestBehavior.AllowGet);
        }


        public ActionResult EditConsignment()
        {
            ViewBag.transaction = new TransactionMetadata();

            ViewBag.talist = new List<TransactionView>();

            return View();
        }


        [HttpPost]
        public ActionResult EditConsignment(TransactionMetadata transaction)
        {

            //if(transaction.Insurance != "yes")
            //{
            //    transaction.BillAmount = 0;                
            //    transaction.Risksurcharge = 0;
            //    transaction.Invoice_No = 0;
            //}

            if (transaction.topay != "yes")
            {
                transaction.Topaycharges = 0;
                transaction.consignee = "0";
                transaction.TopayAmount = 0;
            }
            if (transaction.cod != "yes")
            {
                transaction.codAmount = 0;
                transaction.codcharges = 0;
                transaction.consigner = "0";
                transaction.codtotalamount = 0;
            }


            if (ModelState.IsValid)
            {
                Transaction tr = db.Transactions.Where(m => m.Consignment_no == transaction.Consignment_no).FirstOrDefault();


                string[] formats = { "dd/MM/yyyy", "dd-MMM-yyyy", "yyyy-MM-dd", "dd-MM-yyyy", "M/d/yyyy", "dd MMM yyyy" };

                string bdate = DateTime.ParseExact(transaction.tembookingdate, formats, CultureInfo.InvariantCulture, DateTimeStyles.None).ToString("MM/dd/yyyy");

                transaction.booking_date = Convert.ToDateTime(bdate);


                if (tr != null)
                {

                    ; db.Entry(tr).State = EntityState.Detached;


                    transaction.T_id = tr.T_id;


                    Transaction tran = new Transaction();
                    tran.T_id = tr.T_id;
                    tran.Customer_Id = transaction.Customer_Id;
                    tran.booking_date = transaction.booking_date;
                    tran.Consignment_no = transaction.Consignment_no.Trim().PadLeft(8);
                    tran.Pincode = transaction.Pincode;
                    tran.Mode = transaction.Mode;
                    tran.Pf_Code = db.Companies.Where(m => m.Company_Id == transaction.Customer_Id).Select(m => m.Pf_code).FirstOrDefault();
                    tran.AdminEmp = 000;
                    tran.Weight_t = transaction.Weight_t;
                    tran.Amount = transaction.Amount;
                    tran.Company_id = transaction.Company_id;
                    tran.ReceiverName = transaction.ReceiverName;
                    tran.Quanntity = transaction.Quanntity;
                    tran.Type_t = transaction.Type_t;
                    tran.Insurance = transaction.Insurance;
                    tran.Claimamount = transaction.Claimamount;
                    tran.Percentage = transaction.Percentage;

                    tran.Claimamount = transaction.Claimamount;
                    tran.remark = transaction.remark;
                    tran.topay = transaction.topay;
                    tran.codAmount = transaction.codAmount;
                    tran.consignee = transaction.consigner;
                    tran.cod = transaction.cod;
                    tran.TopayAmount = transaction.TopayAmount;
                    tran.Topaycharges = transaction.Topaycharges;
                    tran.Actual_weight = transaction.Actual_weight;
                    tran.codcharges = transaction.codcharges;
                    tran.codAmount = transaction.codAmount;
                    tran.dtdcamount = transaction.dtdcamount;
                    tran.chargable_weight = transaction.chargable_weight;
                    tran.status_t = transaction.status_t;
                    tran.rateperkg = transaction.rateperkg;
                    tran.docketcharege = transaction.docketcharege;
                    tran.fovcharge = transaction.fovcharge;
                    tran.loadingcharge = transaction.loadingcharge;
                    tran.odocharge = transaction.odocharge;
                    tran.Risksurcharge = transaction.Risksurcharge;
                    tran.Invoice_No = transaction.Invoice_No;
                    tran.BillAmount = transaction.BillAmount;
                    tran.tembookingdate = transaction.tembookingdate;

                    tran.codtotalamount = transaction.codtotalamount;
                    tran.consigner = transaction.consigner;
                    tran.Packingcharges = transaction.Packingcharges;
                    tran.Handlingcharges = transaction.Handlingcharges; 
                    tran.GEC3Additioncharges = transaction.GEC3Additioncharges;

                    db.Entry(tran).State = EntityState.Modified;

                    //  db.SaveChanges();

                    try
                    {
                        // Your code...
                        // Could also be before try if you know the exception occurs in SaveChanges

                        db.SaveChanges();
                    }
                    catch (DbEntityValidationException e)
                    {
                        foreach (var eve in e.EntityValidationErrors)
                        {
                            Console.WriteLine("Entity of type \"{0}\" in state \"{1}\" has the following validation errors:",
                                eve.Entry.Entity.GetType().Name, eve.Entry.State);
                            foreach (var ve in eve.ValidationErrors)
                            {
                                Console.WriteLine("- Property: \"{0}\", Error: \"{1}\"",
                                    ve.PropertyName, ve.ErrorMessage);
                            }
                        }
                        throw;
                    }
                    ViewBag.Message = "Consignment Updated SuccessFully";
                }

                ModelState.Clear();

                ViewBag.success = true;

                char ch = transaction.Consignment_no[0];


                var isNumeric = int.TryParse(ch.ToString(), out int n);
                string nextconsignment = "";

                long consignnumber = 0;

                if (transaction.Consignment_no.ToLower().StartsWith("7d"))
                {
                 var    cha = transaction.Consignment_no.Substring(0, 2);
                     consignnumber = Convert.ToInt64(transaction.Consignment_no.Substring(2));
                    long consignnumberadd = consignnumber + 1;
                    var lenght = transaction.Consignment_no.Substring(2).Length;

                   nextconsignment = cha + "" + (consignnumberadd.ToString().PadLeft(lenght, '1'));
                }

                else if (transaction.Consignment_no.ToLower().StartsWith("7x"))
                {
                   var cha = transaction.Consignment_no.Substring(0, 2);
                     consignnumber = Convert.ToInt64(transaction.Consignment_no.Substring(2));
                    consignnumber = consignnumber + 1;
                    var lenght = transaction.Consignment_no.Substring(2).Length;
                  nextconsignment = cha + "" + (consignnumber.ToString().PadLeft(lenght, '1'));
                }
                else if (isNumeric)
                {
                    //var getNum = transaction.Consignment_no.Substring(0, transaction.Consignment_no.Length-1);

                    consignnumber = Convert.ToInt64(transaction.Consignment_no.Remove(0, 2));

                    consignnumber = consignnumber + 1;

                    nextconsignment = transaction.Consignment_no.Substring(0, 2) + "" + consignnumber;

                }
                else
                {

                    consignnumber = Convert.ToInt64(transaction.Consignment_no.Substring(1));

                    consignnumber = consignnumber + 1;



                    nextconsignment = ch + "" + consignnumber;
                }

                var con = db.Transactions.Where(m => m.Consignment_no == nextconsignment).FirstOrDefault();

                if (con != null && isNumeric != true)
                {

                    ViewBag.nextconsignment = ch + "" + consignnumber.ToString().PadLeft(8, '0');

                }
                else
                {
                    ViewBag.nextconsignment = nextconsignment;
                }



                return PartialView("EditConsignmentPartial");
            }

            return PartialView("EditConsignmentPartial", transaction);
        }





        [HttpPost]
        public ActionResult Trtableseacrh(string Fromdatetime, string ToDatetime, string Custid)
        {

            DateTime? fromdate = null;
            DateTime? todate = null;


            string[] formats = {"dd/MM/yyyy", "dd-MMM-yyyy", "yyyy-MM-dd",
                   "dd-MM-yyyy", "M/d/yyyy", "dd MMM yyyy"};







            if (Fromdatetime != "")
            {

                string bdatefrom = DateTime.ParseExact(Fromdatetime, formats, CultureInfo.InvariantCulture, DateTimeStyles.None).ToString("MM/dd/yyyy");
                fromdate = Convert.ToDateTime(bdatefrom);

                ViewBag.todate = ToDatetime;
            }
            else
            {
                todate = null;
            }

            if (ToDatetime != "")
            {
                string bdateto = DateTime.ParseExact(ToDatetime, formats, CultureInfo.InvariantCulture, DateTimeStyles.None).ToString("MM/dd/yyyy");
                todate = Convert.ToDateTime(bdateto);
                ViewBag.fromdate = Fromdatetime;
            }
            else
            {
                fromdate = null;
            }
            if (Custid != "")
            {
                ViewBag.Custid = Custid;
            }



            List<TransactionView> transactions =
                db.TransactionViews.Where(m =>

               (m.Customer_Id == Custid || Custid == "")

               && (m.Customer_Id != null)
                    ).ToList().Where(m => (fromdate == null || m.booking_date.Value.Date >= fromdate.Value.Date) && (todate == null || m.booking_date.Value.Date <= todate.Value.Date))
                           .ToList();



            ViewBag.totalamt = transactions.Sum(b => b.Amount);


            return PartialView("TrSearchTable", transactions);
        }


        public ActionResult MultipleBooking()
        {

            return View();
        }

        [HttpPost]
        public ActionResult MultipleBooking(string StartingCons, string EndingCons, string Companyid)
        {
            string startcons = StartingCons.Substring(0, 2);
            string endcons = EndingCons.Substring(0, 2);


            if (startcons.ToLower() == "7d")
            {

                long startConsignment = Convert.ToInt64(StartingCons.Substring(2));
                long EndConsignment = Convert.ToInt64(EndingCons.Substring(2));

                int countconsigmnets = 0;
                var pfcode = db.Companies.Where(m => m.Company_Id == Companyid).Select(m => m.Pf_code).FirstOrDefault();
                if (startcons == endcons)
                {
                    for (long i = startConsignment; i <= EndConsignment; i++)
                    {
                        string updateconsignment = startcons + i.ToString();


                        Transaction transaction = db.Transactions.Where(m => m.Consignment_no == updateconsignment).FirstOrDefault();

                        if (transaction != null)
                        {

                            CalculateAmount ca = new CalculateAmount();

                            double? amt = ca.CalulateAmt(transaction.Consignment_no, Companyid, transaction.Pincode, transaction.Mode, Convert.ToDouble(transaction.chargable_weight), transaction.Type_t);

                            transaction.Amount = amt;
                            transaction.Customer_Id = Companyid;
                            transaction.AdminEmp = 000;
                            transaction.Pf_Code = pfcode;
                            db.Entry(transaction).State = EntityState.Modified;
                            db.SaveChanges();
                        }


                        countconsigmnets++;

                        if (countconsigmnets >= 100)
                        {
                            break;
                        }

                    }
                }

            }
            else if (startcons.ToLower() == "7x")
            {

                long startConsignment = Convert.ToInt64(StartingCons.Substring(2));
                long EndConsignment = Convert.ToInt64(EndingCons.Substring(2));

                int countconsigmnets = 0;
                var pfcode = db.Companies.Where(m => m.Company_Id == Companyid).Select(m => m.Pf_code).FirstOrDefault();
                if (startcons == endcons)
                {
                    for (long i = startConsignment; i <= EndConsignment; i++)
                    {
                        string updateconsignment = startcons + i.ToString();


                        Transaction transaction = db.Transactions.Where(m => m.Consignment_no == updateconsignment).FirstOrDefault();

                        if (transaction != null)
                        {

                            CalculateAmount ca = new CalculateAmount();

                            double? amt = ca.CalulateAmt(transaction.Consignment_no, Companyid, transaction.Pincode, transaction.Mode, Convert.ToDouble(transaction.chargable_weight), transaction.Type_t);

                            transaction.Amount = amt;
                            transaction.Customer_Id = Companyid;
                            transaction.AdminEmp = 000;
                            transaction.Pf_Code = pfcode;
                            db.Entry(transaction).State = EntityState.Modified;
                            db.SaveChanges();
                        }


                        countconsigmnets++;

                        if (countconsigmnets >= 100)
                        {
                            break;
                        }

                    }
                }

            }


            else
            {
                char stch = StartingCons[0];
                char Endch = EndingCons[0];

                long startConsignment = Convert.ToInt64(StartingCons.Substring(1));
                long EndConsignment = Convert.ToInt64(EndingCons.Substring(1));

                int countconsigmnets = 0;
                var pfcode = db.Companies.Where(m => m.Company_Id == Companyid).Select(m => m.Pf_code).FirstOrDefault();
                if (stch == Endch)
                {
                    for (long i = startConsignment; i <= EndConsignment; i++)
                    {
                        string updateconsignment = stch + i.ToString();


                        Transaction transaction = db.Transactions.Where(m => m.Consignment_no == updateconsignment).FirstOrDefault();

                        if (transaction != null)
                        {

                            CalculateAmount ca = new CalculateAmount();

                            double? amt = ca.CalulateAmt(transaction.Consignment_no, Companyid, transaction.Pincode, transaction.Mode, Convert.ToDouble(transaction.chargable_weight), transaction.Type_t);

                            transaction.Amount = amt;
                            transaction.Customer_Id = Companyid;
                            transaction.AdminEmp = 000;
                            transaction.Pf_Code = pfcode;
                            db.Entry(transaction).State = EntityState.Modified;
                            db.SaveChanges();
                        }


                        countconsigmnets++;

                        if (countconsigmnets >= 100)
                        {
                            break;
                        }

                    }
                }


            }
            ViewBag.Message = "Booking Completed Successfully";

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


        public ActionResult Nobookinglist()
        {
            List<TransactionModel> list = new List<TransactionModel>();
            ViewBag.PfCode = new SelectList(db.Franchisees.Where(x => !x.PF_Code.ToUpper().StartsWith("PR")), "PF_Code", "PF_Code");
            return View(list);
        }

        [HttpPost]
        public ActionResult Nobookinglist(string Fromdatetime, string ToDatetime, string PfCode, string Submit)
        {

            string[] formats = {"dd/MM/yyyy", "dd-MMM-yyyy", "yyyy-MM-dd",
                   "dd-MM-yyyy", "M/d/yyyy", "dd MMM yyyy"};

            ViewBag.PfCode = new SelectList(db.Franchisees.Where(x => !x.PF_Code.ToUpper().StartsWith("PR")), "PF_Code", "PF_Code", PfCode);

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
                                                   select new TransactionModel() {
                                                       Pf_Code = order.Pf_Code,
                                                       Consignment_no = order.Consignment_no,
                                                       Actual_weight = order.Actual_weight,
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
            //string Fromdatetime="01/01/2015"; string ToDatetime="01/01/2199";
            //string[] formats = {"dd/MM/yyyy", "dd-MMM-yyyy", "yyyy-MM-dd",
            //       "dd-MM-yyyy", "M/d/yyyy", "dd MMM yyyy"};


            //DateTime? fromdate;
            //DateTime? todate;

            //if (Fromdatetime != "")
            //{
            //    string bdatefrom = DateTime.ParseExact(Fromdatetime, formats, CultureInfo.InvariantCulture, DateTimeStyles.None).ToString("MM/dd/yyyy");
            //    fromdate = Convert.ToDateTime(bdatefrom);

            //    ViewBag.fromdate = Fromdatetime;
            //}
            //else
            //{
            //    fromdate = DateTime.Now;
            //}

            //if (ToDatetime != "")
            //{
            //    string bdateto = DateTime.ParseExact(ToDatetime, formats, CultureInfo.InvariantCulture, DateTimeStyles.None).ToString("MM/dd/yyyy");
            //    todate = Convert.ToDateTime(bdateto);
            //    ViewBag.todate = ToDatetime;
            //}
            //else
            //{
            //    todate = DateTime.Now;
            //}





            //List<TransactionView> transactions =
            //    db.TransactionViews.Where(m =>
            //   (m.chargable_weight < m.diff_weight)
            //        ).ToList().Where(m => m.booking_date.Value.Date >= fromdate.Value.Date && m.booking_date.Value.Date <= todate.Value.Date)
            //               .ToList();

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

        public string Checkcompany(string Customerid)
        {
            db.Configuration.ProxyCreationEnabled = false;

            var suggestions = (from s in db.Companies
                               where s.Company_Id == Customerid
                               select s).FirstOrDefault();

            if (suggestions == null)
            {
                return "0";
            }
            else
            {
                return "1";
            }

        }



        public ActionResult MultipleBookingReceipt()
        {
            ViewBag.PfCode = new SelectList(db.Franchisees.Where(x => !x.PF_Code.ToUpper().StartsWith("PR")), "PF_Code", "PF_Code");

            ViewBag.Employees = new SelectList(db.Users.Take(0), "User_Id", "Name");

            return View();
        }

        [HttpPost]
        public ActionResult MultipleBookingReceipt(string PfCode, long Employees, string ToDatetime, string Fromdatetime, string Customer_Id)
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
                ViewBag.fromdate = Fromdatetime;
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
                ViewBag.fromdate = Fromdatetime;
            }


            ViewBag.Customer_Id = Customer_Id;




            ViewBag.PfCode = new SelectList(db.Franchisees.Where(x => !x.PF_Code.ToUpper().StartsWith("PR")), "PF_Code", "PF_Code", PfCode);

            ViewBag.Employees = new SelectList(db.Users, "User_Id", "Name", Employees);

            ViewBag.selectedemp = Employees;

            List<Receipt_details> rc = (from m in db.Receipt_details
                                        where m.Pf_Code == PfCode && m.User_Id == Employees && m.Datetime_Cons != null
                                        select m).ToList()
                             .Where(x => DateTime.Compare(x.Datetime_Cons.Value.Date, fromdate.Value.Date) >= 0 && DateTime.Compare(x.Datetime_Cons.Value.Date, todate.Value.Date) <= 0)
                                .ToList();
            int count = 0;
            var pfcode = db.Companies.Where(m => m.Company_Id == Customer_Id).Select(m => m.Pf_code).FirstOrDefault();
            foreach (var i in rc)
            {
                Transaction tr = new Transaction();

                tr = db.Transactions.Where(m => m.Consignment_no == i.Consignment_No).FirstOrDefault();

                if (tr != null)
                {
                    tr.Customer_Id = Customer_Id;
                    tr.Amount = i.Charges_Total;
                    tr.AdminEmp = 000;
                    tr.Pf_Code = pfcode;
                    db.Entry(tr).State = EntityState.Modified;
                    db.SaveChanges();
                    count++;
                }


            }

            ViewBag.success = count + "Records Updated SuccessFully";


            return View();
        }

        public ActionResult InternationalCity()
        {

            var entity = db.Destinations.Where(m => m.Pincode.StartsWith("111")).
            Select(e => new
            {
                e.Name
            }).Distinct().ToList();


            return Json(entity, JsonRequestBehavior.AllowGet);
        }

        public JsonResult FillPincode(string Name)
        {
            
            var suggestions = from s in db.Destinations
                              where s.Name == Name
                              && s.Pincode.StartsWith("111")
                              select s;

            return Json(suggestions, JsonRequestBehavior.AllowGet);
        }


        public string DeleteConsignment(string Consignment_No)
        {
            Transaction cash = db.Transactions.Where(m => m.Consignment_no == Consignment_No).FirstOrDefault();

            cash.AdminEmp = 000;

            cash.Customer_Id = null;

            db.Entry(cash).State = EntityState.Modified;

            db.SaveChanges();

            return "Consignment Deleted SuccessFully";


        }


        public ActionResult UpdateRate()
        {
            List<TransactionView> list = new List<TransactionView>();

            return View(list);
        }

        [HttpPost]
        public ActionResult UpdateRate(string Fromdatetime, string ToDatetime, string Custid, string submit)
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

            if (submit == "UpdateRate")
            {

                foreach (var i in transactions)
                {

                    Transaction transaction = db.Transactions.Where(m => m.Consignment_no == i.Consignment_no).FirstOrDefault();

                    if (transaction != null)
                    {

                        CalculateAmount ca = new CalculateAmount();

                        double? amt = ca.CalulateAmt(transaction.Consignment_no, i.Customer_Id, transaction.Pincode, transaction.Mode, Convert.ToDouble(transaction.chargable_weight), transaction.Type_t);

                        transaction.Amount = amt;
                        transaction.AdminEmp = 000;

                        db.Entry(transaction).State = EntityState.Modified;
                        db.SaveChanges();
                    }


                }



            }

            ViewBag.totalamt = transactions.Sum(b => b.Amount);

            List<TransactionView> transactions1 =
                  db.TransactionViews.Where(m =>
             (m.Customer_Id == Custid)
                  ).ToList().Where(m => m.booking_date.Value.Date >= fromdate.Value.Date && m.booking_date.Value.Date <= todate.Value.Date).OrderBy(m => m.booking_date).ThenBy(n => n.Consignment_no)
                         .ToList();

            ViewBag.Message = "Added Sucessfully";

            return View(transactions1);
        }

        public ActionResult AddBox_SizeRates()
        {
            ViewBag.Pf_code = new SelectList(db.Franchisees.Where(x => !x.PF_Code.ToUpper().StartsWith("PR")), "PF_Code", "PF_Code");
            ViewBag.Employees = new SelectList(db.Users.Take(0), "User_Id", "Name");

            List<SelectListItem> items = new List<SelectListItem>();

            items.Add(new SelectListItem { Text = "--Select--", Value = "--Select--" });
            items.Add(new SelectListItem { Text = "Box1", Value = "Box1" });
            items.Add(new SelectListItem { Text = "Box2", Value = "Box2" });
            items.Add(new SelectListItem { Text = "Box3", Value = "Box3" });
            items.Add(new SelectListItem { Text = "Box4", Value = "Box4" });
            items.Add(new SelectListItem { Text = "Box5", Value = "Box5" });
            items.Add(new SelectListItem { Text = "Envelope1", Value = "Envelope1" });
            items.Add(new SelectListItem { Text = "Envelope2", Value = "Envelope2" });
            items.Add(new SelectListItem { Text = "Envelope3", Value = "Envelope3" });
            items.Add(new SelectListItem { Text = "Envelope4", Value = "Envelope4" });
            items.Add(new SelectListItem { Text = "Envelope5", Value = "Envelope5" });
            ViewBag.Type = items;



            return View();
        }


        [HttpPost]
        public ActionResult AddBox_SizeRates(AddBox_SizesModel boxsize, string Type, string Pf_code, int Employees)
        {
            string[] formats = { "dd/MM/yyyy", "dd-MMM-yyyy", "yyyy-MM-dd", "dd-MM-yyyy", "M/d/yyyy", "dd MMM yyyy" };

            if (Type == "--Select--" || Pf_code == "--Select--" || Employees == 0)
            {
                ViewBag.Message = "PLease Select Pfcode or Type or Employee Name";
                ViewBag.Pf_code = new SelectList(db.Franchisees.Where(x => !x.PF_Code.ToUpper().StartsWith("PR")), "PF_Code", "PF_Code", Pf_code);
                ViewBag.Employees = new SelectList(db.Users.Take(0), "User_Id", "Name");

                List<SelectListItem> items2 = new List<SelectListItem>();

                items2.Add(new SelectListItem { Text = "--Select--", Value = "--Select--" });
                items2.Add(new SelectListItem { Text = "Box1", Value = "Box1" });
                items2.Add(new SelectListItem { Text = "Box2", Value = "Box2" });
                items2.Add(new SelectListItem { Text = "Box3", Value = "Box3" });
                items2.Add(new SelectListItem { Text = "Box4", Value = "Box4" });
                items2.Add(new SelectListItem { Text = "Box5", Value = "Box5" });
                items2.Add(new SelectListItem { Text = "Envelope1", Value = "Envelope1" });
                items2.Add(new SelectListItem { Text = "Envelope2", Value = "Envelope2" });
                items2.Add(new SelectListItem { Text = "Envelope3", Value = "Envelope3" });
                items2.Add(new SelectListItem { Text = "Envelope4", Value = "Envelope4" });
                items2.Add(new SelectListItem { Text = "Envelope5", Value = "Envelope5" });
                ViewBag.Type = items2;




                return View(boxsize);
            }
            else
            {
                if (ModelState.IsValid)
                {

                    string invdate = DateTime.ParseExact(boxsize.temprecdate, formats, CultureInfo.InvariantCulture, DateTimeStyles.None).ToString("MM/dd/yyyy");

                    BoxEnvelopePacking boxe = new BoxEnvelopePacking();

                    boxe.Entrydate = DateTime.Parse(invdate);
                    boxe.pfcode = Pf_code;
                    boxe.Userid = Employees;
                    boxe.Type = Type;
                    boxe.Quantity = boxsize.Quantity;
                    boxe.Amount = boxsize.amount;

                    db.BoxEnvelopePackings.Add(boxe);
                    db.SaveChanges();


                    ViewBag.Pf_code = new SelectList(db.Franchisees.Where(x => !x.PF_Code.ToUpper().StartsWith("PR")), "PF_Code", "PF_Code", Pf_code);
                    ViewBag.Employees = new SelectList(db.Users.Take(0), "User_Id", "Name");

                    List<SelectListItem> itemsst = new List<SelectListItem>();

                    itemsst.Add(new SelectListItem { Text = "--Select--", Value = "--Select--" });
                    itemsst.Add(new SelectListItem { Text = "Box1", Value = "Box1" });
                    itemsst.Add(new SelectListItem { Text = "Box2", Value = "Box2" });
                    itemsst.Add(new SelectListItem { Text = "Box3", Value = "Box3" });
                    itemsst.Add(new SelectListItem { Text = "Box4", Value = "Box4" });
                    itemsst.Add(new SelectListItem { Text = "Box5", Value = "Box5" });
                    itemsst.Add(new SelectListItem { Text = "Envelope1", Value = "Envelope1" });
                    itemsst.Add(new SelectListItem { Text = "Envelope2", Value = "Envelope2" });
                    itemsst.Add(new SelectListItem { Text = "Envelope3", Value = "Envelope3" });
                    itemsst.Add(new SelectListItem { Text = "Envelope4", Value = "Envelope4" });
                    itemsst.Add(new SelectListItem { Text = "Envelope5", Value = "Envelope5" });
                    ViewBag.Type = itemsst;


                    ViewBag.Message = "Box/Envelope Added SuccessFully";
                    ModelState.Clear();
                    return View();

                }
            }
            ViewBag.Pf_code = new SelectList(db.Franchisees.Where(x => !x.PF_Code.ToUpper().StartsWith("PR")), "PF_Code", "PF_Code",Pf_code);
            ViewBag.Employees = new SelectList(db.Users.Take(0), "User_Id", "Name");

            List<SelectListItem> items = new List<SelectListItem>();

            items.Add(new SelectListItem { Text = "--Select--", Value = "--Select--" });
            items.Add(new SelectListItem { Text = "Box1", Value = "Box1" });
            items.Add(new SelectListItem { Text = "Box2", Value = "Box2" });
            items.Add(new SelectListItem { Text = "Box3", Value = "Box3" });
            items.Add(new SelectListItem { Text = "Box4", Value = "Box4" });
            items.Add(new SelectListItem { Text = "Box5", Value = "Box5" });
            items.Add(new SelectListItem { Text = "Envelope1", Value = "Envelope1" });
            items.Add(new SelectListItem { Text = "Envelope2", Value = "Envelope2" });
            items.Add(new SelectListItem { Text = "Envelope3", Value = "Envelope3" });
            items.Add(new SelectListItem { Text = "Envelope4", Value = "Envelope4" });
            items.Add(new SelectListItem { Text = "Envelope5", Value = "Envelope5" });
            ViewBag.Type = items;

          
           

            return View(boxsize);
        }



        public ActionResult UpdateData()
        {
            return View();
        }

        [HttpPost]
        public ActionResult UpdateData(HttpPostedFileBase httpPostedFileBase, int type)
        {
            if (httpPostedFileBase != null && type == 1)
            {
                HttpPostedFileBase file = httpPostedFileBase;
                if ((file != null) && (file.ContentLength > 0) && !string.IsNullOrEmpty(file.FileName))
                {
                    string fileName = Path.GetFileName(file.FileName);
                    string fileExtension = Path.GetExtension(fileName);

                    if(fileExtension != ".xlsx")
                    {
                        TempData["error"] = "File extension should be xlsx";
                        return RedirectToAction("importFromExcel", "Admin");
                    }

                   // string fileName = file.FileName;
                    string fileContentType = file.ContentType;
                    byte[] fileBytes = new byte[file.ContentLength];
                    var data = file.InputStream.Read(fileBytes, 0, Convert.ToInt32(file.ContentLength));

                    using (var package = new ExcelPackage(file.InputStream))
                    {
                        var currentSheet = package.Workbook.Worksheets;
                        var workSheet = currentSheet.First();
                        var noOfCol = workSheet.Dimension.End.Column;
                        var noOfRow = workSheet.Dimension.End.Row;
                        for (int rowIterator = 2; rowIterator <= noOfRow; rowIterator++)
                        {
                            var Consignment_no = workSheet.Cells[rowIterator, 2].Value.ToString().Trim();
                            var newWeight = workSheet.Cells[rowIterator, 5].Value.ToString();

                            Transaction transaction = db.Transactions.Where(m => m.Consignment_no == Consignment_no).FirstOrDefault();

                            if (transaction != null)
                            {

                                CalculateAmount ca = new CalculateAmount();

                                transaction.chargable_weight = Convert.ToDouble(newWeight);

                                db.Entry(transaction).State = EntityState.Modified;
                                db.SaveChanges();
                            }

                        }
                    }


                }
                TempData["type"] = "1";
                TempData["SuccessMsg"] = "Uploaded Successfully";
            }
            if (httpPostedFileBase != null && type == 2)
            {
                HttpPostedFileBase file = httpPostedFileBase;
                if ((file != null) && (file.ContentLength > 0) && !string.IsNullOrEmpty(file.FileName))
                {
                    string fileName = Path.GetFileName(file.FileName);
                    string fileExtension = Path.GetExtension(fileName);

                    if (fileExtension != ".xlsx")
                    {
                        TempData["error"] = "File extension should be xlsx";
                        return RedirectToAction("importFromExcel", "Admin");
                    }
                    //string fileName = file.FileName;
                    string fileContentType = file.ContentType;
                    byte[] fileBytes = new byte[file.ContentLength];
                    var data = file.InputStream.Read(fileBytes, 0, Convert.ToInt32(file.ContentLength));

                    using (var package = new ExcelPackage(file.InputStream))
                    {
                        var currentSheet = package.Workbook.Worksheets;
                        var workSheet = currentSheet.First();
                        var noOfCol = workSheet.Dimension.End.Column;
                        var noOfRow = workSheet.Dimension.End.Row;
                        for (int rowIterator = 5; rowIterator <= noOfRow; rowIterator++)
                        {
                            var Consignment_no = workSheet.Cells[rowIterator, 2].Value.ToString().Trim();
                            var dtdcAmount = workSheet.Cells[rowIterator, 8].Value.ToString();

                            Transaction transaction = db.Transactions.Where(m => m.Consignment_no == Consignment_no).FirstOrDefault();

                            if (transaction != null)
                            {

                                CalculateAmount ca = new CalculateAmount();

                                transaction.dtdcamount = Convert.ToDouble(dtdcAmount);

                                db.Entry(transaction).State = EntityState.Modified;
                                db.SaveChanges();
                            }

                        }
                    }


                }
                TempData["type"] = "2";
                TempData["SuccessMsg"] = "Uploaded Successfully";
            }

            return RedirectToAction("importFromExcel", "Admin");
        }
    }
}
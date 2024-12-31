using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using DtDc_Billing.Entity_FR;
using DtDc_Billing.Models;

namespace DtDc_Billing.Controllers
{

    [SessionAdmin]
    public class PaymentController : Controller
    {
        private DB_A43B74_wingsgrowdbEntities db = new DB_A43B74_wingsgrowdbEntities();
        // GET: Payment
        public ActionResult InvoicePaymentList()
        {

            ViewBag.Cash = new Cash();
            ViewBag.Cheque = new Cheque();
            ViewBag.Neft = new NEFT();
            ViewBag.Credit = new CreditNote();

          //  var transactions = db.Invoices.Where(m=> !m.invoiceno.StartsWith("ATK/18-19/") && !m.invoiceno.StartsWith("ATK/17-18")).AsEnumerable();
            var transactions =new List<Invoice>();
            return View(transactions.ToList());
        }

        [HttpPost]
        public ActionResult InvoicePaymentList(string Fromdatetime, string ToDatetime, string Custid, string Submit)
        {
            DateTime? fromdate = null;
            DateTime? todate = null;



            string[] formats = {"dd/MM/yyyy", "dd-MMM-yyyy", "yyyy-MM-dd",
                   "dd-MM-yyyy", "M/d/yyyy", "dd MMM yyyy"};



            string bdatefrom = DateTime.ParseExact(Fromdatetime, formats, CultureInfo.InvariantCulture, DateTimeStyles.None).ToString("MM/dd/yyyy");
            fromdate = Convert.ToDateTime(bdatefrom);




            string bdateto = DateTime.ParseExact(ToDatetime, formats, CultureInfo.InvariantCulture, DateTimeStyles.None).ToString("MM/dd/yyyy");
            todate = Convert.ToDateTime(bdateto);
            ViewBag.fromdate = Fromdatetime;
            ViewBag.todate = ToDatetime;


            if (Custid != "")
            {
                ViewBag.Custid = Custid;
            }
            ViewBag.Cash = new Cash();
            ViewBag.Cheque = new Cheque();
            ViewBag.Neft = new NEFT();
            ViewBag.Credit = new CreditNote();

            //      ViewBag.Savingscount = db.Savings.Where(m => m.Datetime_Sav.Value.Day == localTime.Day
            //&& m.Datetime_Sav.Value.Month == localTime.Month
            //&& m.Datetime_Sav.Value.Year == localTime.Year
            //&& m.Pf_Code == pfcode

          //  var transactions = db.Invoices.Where(m => m.Customer_Id == Custid).ToList();
            var transactions = (from u in db.Invoices
                                where u.Customer_Id == Custid
                                select u).ToList()

                 .Where(x => DateTime.Compare(x.invoicedate.Value.Date, fromdate.Value.Date) >= 0 && DateTime.Compare(x.invoicedate.Value.Date, todate.Value.Date) <= 0).ToList();

            //var transactions = db.Invoices.Where(m => m.Customer_Id == Custid &&
            //m.invoicedate>= fromdate && m.invoicedate<= todate).ToList();

            return View(transactions.AsEnumerable());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Cash(Cash cash)
        {
            if (ModelState.IsValid)
            {
                var cashb = db.Invoices.Where(m => m.invoiceno == cash.Invoiceno).FirstOrDefault();

                double balance = Math.Round(Convert.ToDouble(cashb.netamount)) - Convert.ToDouble(cashb.paid);

                if (cash.C_Total_Amount > balance)
                {
                    ModelState.AddModelError("InvAmt", "Amount Is Greater Than Balance");
                }
                else
                {
                    cashb.paid = Convert.ToDouble(cashb.paid) + Convert.ToDouble(cash.C_Total_Amount);
                    db.Entry(cashb).State = EntityState.Modified;

                    db.Cashes.Add(cash);
                    db.SaveChanges();

                    return Json(new { RedirectUrl = Url.Action("InvoicePaymentList") });
                }
            }

            return PartialView("CashPartial", cash);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Cheque(Cheque cheque)
        {
            if (ModelState.IsValid)
            {

                var cashb = db.Invoices.Where(m => m.invoiceno == cheque.Invoiceno).FirstOrDefault();

                double balance = Math.Round(Convert.ToDouble(cashb.netamount)) - Convert.ToDouble(cashb.paid);

                if (cheque.totalAmount > balance)
                {
                    ModelState.AddModelError("InvAmt", "Amount Is Greater Than Balance");
                }
                else
                {
                    cashb.paid = Convert.ToDouble(cashb.paid) + Convert.ToDouble(cheque.totalAmount);
                    db.Entry(cashb).State = EntityState.Modified;


                    db.Cheques.Add(cheque);
                    db.SaveChanges();
                    return Json(new { RedirectUrl = Url.Action("InvoicePaymentList") });
                }
            }

            return PartialView("ChequePartial", cheque);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Neft(NEFT nEFT)
        {
            if (ModelState.IsValid)
            {
                var cashb = db.Invoices.Where(m => m.invoiceno == nEFT.Invoiceno).FirstOrDefault();

                double balance = Math.Round(Convert.ToDouble(cashb.netamount)) - Convert.ToDouble(cashb.paid);

                if (nEFT.N_Total_Amount > balance)
                {
                    ModelState.AddModelError("InvAmt", "Amount Is Greater Than Balance");
                }
                else
                {
                    cashb.paid = Convert.ToDouble(cashb.paid) + Convert.ToDouble(nEFT.NeftAmount);
                    db.Entry(cashb).State = EntityState.Modified;


                    db.NEFTs.Add(nEFT);
                    db.SaveChanges();
                    return Json(new { RedirectUrl = Url.Action("InvoicePaymentList") });
                }
            }

            return PartialView("NeftPartial", nEFT);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreditNote(CreditNote creditNote)
        {
            if (ModelState.IsValid)
            {
                var cashb = db.Invoices.Where(m => m.invoiceno == creditNote.Invoiceno).FirstOrDefault();

                double balance = Math.Round(Convert.ToDouble(cashb.netamount)) - Convert.ToDouble(cashb.paid);

                if (creditNote.Cr_Amount > balance)
                {
                    ModelState.AddModelError("InvAmt", "Amount Is Greater Than Balance");
                }
                else
                {
                    cashb.paid = Convert.ToDouble(cashb.paid) + Convert.ToDouble(creditNote.Cr_Amount);
                    db.Entry(cashb).State = EntityState.Modified;

                    db.CreditNotes.Add(creditNote);
                    db.SaveChanges();

                    return Json(new { RedirectUrl = Url.Action("InvoicePaymentList") });
                }
            }

            return PartialView("CreditNotePartial", creditNote);
        }


        public ActionResult AddCodPayment()
        {
            ViewBag.Cod = new addcodamount();

            ViewBag.Codlist = new List<TransactionView>();


            return View();
        }




        [HttpPost]
        public ActionResult CodSearch(string Custid)
        {
            ViewBag.Custid = Custid;

            List<TransactionView> transactions = (from u in db.TransactionViews
                                                  where u.cod=="yes" &&
                                                  !db.addcodamounts.Any(f => f.consinment_no == u.Consignment_no)
                                                  select u).ToList();

            return PartialView("CodSearchPartial", transactions);


        }

        [HttpPost]
        public ActionResult EditCod(addcodamount addcodamount)
        {



            if (ModelState.IsValid)
            {
                db.addcodamounts.Add(addcodamount);
                db.SaveChanges();

                ViewBag.Message = "Cod Payment Added SuccessFully";

                return PartialView("EditCodPartial");
            }

            return PartialView("EditCodPartial", addcodamount);
        }



        public ActionResult AddTopayPayment()
        {
            ViewBag.Topay = new addtopayamount();

            ViewBag.Codlist = new List<TransactionView>();


            return View();
        }



        [HttpPost]
        public ActionResult TopaySearch(string Custid)
        {
            ViewBag.Custid = Custid;

            List<TransactionView> transactions = (from u in db.TransactionViews
                                                  where u.cod == "yes" &&
                                                  !db.addtopayamounts.Any(f => f.consinmentno == u.Consignment_no)
                                                  select u).ToList();

            return PartialView("TopaySearchPartial", transactions);


        }

        [HttpPost]
        public ActionResult EditTopay(addtopayamount addtopayamount)
        {



            if (ModelState.IsValid)
            {
                db.addtopayamounts.Add(addtopayamount);
                db.SaveChanges();

                ViewBag.Message = "Topay Payment Added SuccessFully";

                return PartialView("EditTopayPartial");
            }

            return PartialView("EditTopayPartial", addtopayamount);
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
    }
}
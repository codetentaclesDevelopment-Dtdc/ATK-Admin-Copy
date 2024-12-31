using DtDc_Billing.Entity_FR;
using DtDc_Billing.Models;
using Ionic.Zip;
using Microsoft.Reporting.WebForms;
using OfficeOpenXml;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Math;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Security.Policy;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using WebGrease.Css.Ast;
using static DtDc_Billing.invo;

namespace DtDc_Billing.Controllers
{
    [SessionAdmin]
    public class InvoiceController : Controller
    {

        private DB_A43B74_wingsgrowdbEntities db = new DB_A43B74_wingsgrowdbEntities();

        private DB_A43B74_wingsgrowdbEntities dc = new DB_A43B74_wingsgrowdbEntities();

        //string invstart = "ATK/20-21/"; 
        //string invstart = "ATK/22-23/";
        string invstart = "ATK/23-24/";

        public ActionResult GenerateInvoice(string Invoiceno = null)
        {
            Invoice inv = db.Invoices.Where(m => m.invoiceno == Invoiceno).FirstOrDefault();      
            ViewBag.Gst = "18";
            if(inv!=null)
            { 
            ViewBag.invno = inv.invoiceno;
            }
            return View(inv);

        }

        public ActionResult DpInvoice(string Invoiceno = null)
        {
            Invoice inv = db.Invoices.Where(m => m.invoiceno == Invoiceno).FirstOrDefault();
            ViewBag.Gst = "18";
            if (inv != null)
            {
                ViewBag.invno = inv.invoiceno;
            }
            return View(inv);

        }


        // GET: Invoice
        //public ActionResult GenerateInvoice(string Invoiceno = null)
        //{



        //    string lastInvoiceno = db.Invoices.Where(m => m.invoiceno.StartsWith(invstart)).OrderByDescending(m => m.IN_Id).Take(1).Select(m => m.invoiceno).FirstOrDefault() ?? invstart + 0;

        //    int number = Convert.ToInt32(lastInvoiceno.Substring(10));

        //    Invoice inv = db.Invoices.Where(m => m.invoiceno == Invoiceno).FirstOrDefault();

        //    ViewBag.lastInvoiceno = invstart +""+ (number + 1);

        //    ViewBag.Gst = "18";
        //    return View(inv);
        //}



        //public ActionResult DpInvoice(string Invoiceno = null)
        //{


        //    string lastInvoiceno = db.Invoices.Where(m => m.invoiceno.StartsWith(invstart)).OrderByDescending(m => m.IN_Id).Take(1).Select(m => m.invoiceno).FirstOrDefault() ?? invstart + 0;
        //    int number = Convert.ToInt32(lastInvoiceno.Substring(10));

        //    ViewBag.lastInvoiceno = invstart +""+ (number + 1);


        //    Invoice inv = db.Invoices.Where(m => m.invoiceno == Invoiceno).FirstOrDefault();


        //    return View(inv);

        //}
        // GET: Invoice
        [HttpGet]
        public ActionResult ViewInvoice()
        {
            ViewBag.Cash = new AddInvoiceAckModel();

            List<InvoiceModel> list = new List<InvoiceModel>();
            
            ViewBag.PfCode = new SelectList(db.Franchisees.Where(x => !x.PF_Code.ToUpper().StartsWith("PR")), "PF_Code", "PF_Code");
            return View(list);


        }

        [HttpPost]
        public ActionResult ViewInvoice(string invfromdate, string Companydetails, string invtodate, int size, int pageNo = 1)
        {


            ViewBag.Cash = new AddInvoiceAckModel();

            ViewBag.PfCode = new SelectList(db.Franchisees.Where(x => !x.PF_Code.ToUpper().StartsWith("PR")), "PF_Code", "PF_Code");

         
            string[] formats = {"dd/MM/yyyy", "dd-MMM-yyyy", "yyyy-MM-dd",
                   "dd-MM-yyyy", "M/d/yyyy", "dd MMM yyyy"};

            string fromdate = "";

            string todate = "";

            if (invfromdate != null)
            {
                fromdate = DateTime.ParseExact(invfromdate, formats, CultureInfo.InvariantCulture, DateTimeStyles.None).ToString("yyyy-MM-dd");
                todate = DateTime.ParseExact(invtodate, formats, CultureInfo.InvariantCulture, DateTimeStyles.None).ToString("yyyy-MM-dd");
            }


            ViewBag.invfromdate = invfromdate;
            ViewBag.invtodate = invtodate;
            ViewBag.pageNo = pageNo;
            ViewBag.size = size;


            ViewBag.Companydetails = Companydetails;//new SelectList(db.Companies, "Company_Id", "Company_Name");

            var comp = db.Companies.Where(m => m.Company_Id == Companydetails).FirstOrDefault();
            if (comp != null)
            {
                ViewBag.Companyid = comp.Company_Id;
            }
            if (Companydetails == "")
            {
                var obj = db.getInvoiceWithoutcompany(DateTime.Parse(fromdate), DateTime.Parse(todate), size, pageNo).Select(x => new InvoiceModel
                {

                    IN_Id = x.IN_Id,
                    invoiceno = x.invoiceno,
                    invoicedate = x.invoicedate,
                    periodfrom = x.periodfrom,
                    periodto = x.periodto,
                    total = x.total,
                    fullsurchargetax = x.fullsurchargetax,
                    fullsurchargetaxtotal = x.fullsurchargetaxtotal,
                    servicetax = x.servicetax,
                    servicetaxtotal = x.servicetaxtotal,
                    othercharge = x.othercharge,
                    netamount = x.netamount,
                    Customer_Id = x.Customer_Id,
                    paid = x.paid,
                    discount = x.discount,
                    Royalty_charges = x.Royalty_charges,
                    Docket_charges = x.Docket_charges,
                    Tempdatefrom = x.Tempdatefrom,
                    TempdateTo = x.TempdateTo,
                    tempInvoicedate = x.tempInvoicedate,
                    Address = x.Address,
                    Invoice_Lable = x.Invoice_Lable,
                    totalCount = x.totalCount ?? 0,
                    Filepath=x.Filepath
                }).ToList();

                return View(obj);

            }
            else
            {
                var obj = db.getInvoice(DateTime.Parse(fromdate), DateTime.Parse(todate), comp.Company_Id, size, pageNo).Select(x => new InvoiceModel
                {

                    IN_Id = x.IN_Id,
                    invoiceno = x.invoiceno,
                    invoicedate = x.invoicedate,
                    periodfrom = x.periodfrom,
                    periodto = x.periodto,
                    total = x.total,
                    fullsurchargetax = x.fullsurchargetax,
                    fullsurchargetaxtotal = x.fullsurchargetaxtotal,
                    servicetax = x.servicetax,
                    servicetaxtotal = x.servicetaxtotal,
                    othercharge = x.othercharge,
                    netamount = x.netamount,
                    Customer_Id = x.Customer_Id,
                    paid = x.paid,
                    discount = x.discount,
                    Royalty_charges = x.Royalty_charges,
                    Docket_charges = x.Docket_charges,
                    Tempdatefrom = x.Tempdatefrom,
                    TempdateTo = x.TempdateTo,
                    tempInvoicedate = x.tempInvoicedate,
                    Address = x.Address,
                    Invoice_Lable = x.Invoice_Lable,
                    totalCount = x.totalCount ?? 0,
                    Filepath = x.Filepath
                }).ToList();

                return View(obj);


            }

            return View();
        }
        //[HttpPost]
        //public ActionResult ViewInvoice(string Customerid, string invfromdate, string invtodate)
        //{
        //    DateTime? fromdate = null;
        //    DateTime? todate = null;

        //    ViewBag.Customerid = Customerid;

        //    string[] formats = {"dd/MM/yyyy", "dd-MMM-yyyy", "yyyy-MM-dd",
        //           "dd-MM-yyyy", "M/d/yyyy", "dd MMM yyyy"};

        //    if ((invfromdate != null && invfromdate != "") || (invtodate != null && invtodate != ""))
        //    {
        //        string bdatefrom = DateTime.ParseExact(invfromdate, formats, CultureInfo.InvariantCulture, DateTimeStyles.None).ToString("MM/dd/yyyy");
        //        fromdate = Convert.ToDateTime(bdatefrom);

        //        string bdateto = DateTime.ParseExact(invtodate, formats, CultureInfo.InvariantCulture, DateTimeStyles.None).ToString("MM/dd/yyyy");
        //        todate = Convert.ToDateTime(bdateto);


        //        ViewBag.invfromdate = invfromdate;
        //        ViewBag.invtodate = invtodate;

        //        return View(db.Invoices.Where(m => (m.Total_Lable == null || m.Total_Lable.Length == 0) && m.Customer_Id.Contains(Customerid)).ToList().Where(m => m.invoicedate.Value.Date >= fromdate.Value.Date && m.invoicedate.Value.Date <= todate.Value.Date).ToList());

        //    }
        //    else
        //    {


        //        return View(db.Invoices.Where(m => (m.Total_Lable == null || m.Total_Lable.Length == 0) && m.Customer_Id.Contains(Customerid)).ToList());
        //    }
        //}

        public ActionResult ViewDPInvoice()
        {
            return View(db.Invoices.Where(m => m.Total_Lable != null || m.Total_Lable.Length > 0).ToList());
        }


        public ActionResult ViewAllRemarkInvoice()
        {
            return View(db.InvoiceRemarks.ToList());
        }
        public ActionResult ViewInvoiceAcknoledgment()
        {
            return View(db.Invoices.Where(d=>d.Filepath!=null).ToList());
        }

        public ActionResult ViewSingleInvoice()
        {
            var temp = dc.singleinvoiceconsignments.Select(m=>m.Invoice_no).ToList();



            var a =(from member in db.Invoices
                    where temp.Contains(member.invoiceno)
                    select member).ToList();



            return View(a);

        }


        public JsonResult InvoiceTable(string CustomerId, string Tempdatefrom, string TempdateTo)
        {

            string[] formats = {"dd/MM/yyyy", "dd-MMM-yyyy", "yyyy-MM-dd",
                   "dd-MM-yyyy", "M/d/yyyy", "dd MMM yyyy"};

            string bdatefrom = DateTime.ParseExact(Tempdatefrom, formats, CultureInfo.InvariantCulture, DateTimeStyles.None).ToString("MM/dd/yyyy");
            string bdateto = DateTime.ParseExact(TempdateTo, formats, CultureInfo.InvariantCulture, DateTimeStyles.None).ToString("MM/dd/yyyy");


            DateTime fromdate = Convert.ToDateTime(bdatefrom);
            DateTime todate = Convert.ToDateTime(bdateto);




            db.Configuration.ProxyCreationEnabled = false;

            var Companies = db.TransactionViews.Where(m => m.Customer_Id == CustomerId && !db.singleinvoiceconsignments.Select(b=>b.Consignment_no).Contains(m.Consignment_no)).ToList().
            Where(x => DateTime.Compare(x.booking_date.Value.Date, fromdate) >= 0 && DateTime.Compare(x.booking_date.Value.Date, todate) <= 0).OrderBy(m => m.booking_date).ThenBy(n => n.Consignment_no)
                              .ToList();




            return Json(Companies, JsonRequestBehavior.AllowGet);

        }

       

        public JsonResult Getvalues(string CustomerId, string Tempdatefrom, string TempdateTo)
        {

            string[] formats = {"dd/MM/yyyy", "dd-MMM-yyyy", "yyyy-MM-dd",
                   "dd-MM-yyyy", "M/d/yyyy", "dd MMM yyyy"};

            string bdatefrom = DateTime.ParseExact(Tempdatefrom, formats, CultureInfo.InvariantCulture, DateTimeStyles.None).ToString("MM/dd/yyyy");
            string bdateto = DateTime.ParseExact(TempdateTo, formats, CultureInfo.InvariantCulture, DateTimeStyles.None).ToString("MM/dd/yyyy");


            DateTime fromdate = Convert.ToDateTime(bdatefrom);
            DateTime todate = Convert.ToDateTime(bdateto);




            db.Configuration.ProxyCreationEnabled = false;

            var Companies = db.TransactionViews.Where(m => m.Customer_Id == CustomerId && !db.singleinvoiceconsignments.Select(b => b.Consignment_no).Contains(m.Consignment_no)).ToList().
            Where(x => DateTime.Compare(x.booking_date.Value.Date, fromdate) >= 0 && DateTime.Compare(x.booking_date.Value.Date, todate) <= 0).OrderBy(m => m.booking_date).ThenBy(n => n.Consignment_no)
                              .ToList();


            var totalsum = Companies.Sum(m => (m.Amount ?? 0) + (m.Risksurcharge ?? 0));

            return Json(totalsum, JsonRequestBehavior.AllowGet);
        }



     

        public JsonResult InvoiceDetails(string CustomerId, string invono)
        {
            db.Configuration.ProxyCreationEnabled = false;

            var Companies = db.Companies.Where(m => m.Company_Id == CustomerId).FirstOrDefault();
            
            var pfcode = (from d in db.Companies
                          where d.Company_Id == CustomerId
                          select new { d.Pf_code }).FirstOrDefault();

            Invoice inv = db.Invoices.Where(m => m.invoiceno == invono).FirstOrDefault();

                if (inv == null)
            {
                string lastInvoiceno = "";
                

                if (pfcode != null)
                {
                    var branchcode = (from d in db.Franchisees
                                      where d.PF_Code == pfcode.Pf_code
                                      select new { d.BranchCode }).FirstOrDefault();



                    string getOldInvoiceWith2Slash = "ATK/" + branchcode.BranchCode + "/24-25/";

                    
                    string no = "";
                        string finalstring = ""; 


                    lastInvoiceno = db.Invoices.Where(m => m.invoiceno.StartsWith(getOldInvoiceWith2Slash)).OrderByDescending(m => m.IN_Id).Take(1).Select(m => m.invoiceno).FirstOrDefault();


                    
                    if (lastInvoiceno == null)
                    {
                        //int number = Convert.ToInt32(lastInvoiceno1.Substring(13));
                        //no = lastInvoiceno.Substring(13);
                        string Invoiceno = getOldInvoiceWith2Slash + "" + (0 + 1);
                        Companies.Other_Details = Invoiceno;

                    }
                    
                    else
                    {
                        
                        string[] strarrinvno = lastInvoiceno.Split('/');
                        int newnumber = Convert.ToInt32(strarrinvno[3]) + 1;
                      
                        string Invoiceno = getOldInvoiceWith2Slash + "" + newnumber;
                        Companies.Other_Details = Invoiceno;
                    }

                    ViewBag.Gst = "18";

                }
            }
            else
            {
                if (pfcode != null)
                {
                    Companies.Other_Details = inv.invoiceno;
                }
            }
                return Json(Companies, JsonRequestBehavior.AllowGet);

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

        public ActionResult CustomerIdAutocompleteForViewInvocie(string PF_COde)
        {


            var entity = db.Companies.Where(m => m.Pf_code == PF_COde).
Select(e => new
{
    e.Company_Id,
    e.IsAgreementoption
}).Where(e => e.IsAgreementoption != 1).Distinct().ToList();


            return Json(entity, JsonRequestBehavior.AllowGet);
        }


        public JsonResult PfWiseCustomerIdAutocomplete(string pfcode)
        {
            //List<string> entity = ;

            if (pfcode == "")
            {
                var entity = db.Companies.Where(a => !(a.Company_Id.StartsWith("Cash_")) && !(a.Company_Id.StartsWith("BASIC_TS")) && a.IsAgreementoption!=1).Select(e => new
                {
                    e.Company_Id
                }).Distinct().ToList();
                return Json(entity, JsonRequestBehavior.AllowGet);
            }
            else
            {
                var entity = db.Companies.Where(a => !(a.Company_Id.StartsWith("Cash_")) && !(a.Company_Id.StartsWith("BASIC_TS")) && a.Pf_code == pfcode && a.IsAgreementoption != 1).Select(e => new
                {
                    e.Company_Id
                }).Distinct().ToList();
                return Json(entity, JsonRequestBehavior.AllowGet);
            }
            
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult SaveInvoice(Invoice invoice, string submit, string description, string Remark)
        {
            try
            {
                if (invoice.discount == "yes")
                {
                    ViewBag.disc = invoice.discount;
                }


                if (ModelState.IsValid)
                {

                    string[] formats = { "dd/MM/yyyy", "dd-MMM-yyyy", "yyyy-MM-dd", "dd-MM-yyyy", "M/d/yyyy", "dd MMM yyyy" };

                    Invoice inv = db.Invoices.Where(m => m.invoiceno == invoice.invoiceno).FirstOrDefault();


                    if (inv != null)
                    {

                        if (Remark == "")
                        {
                            ViewBag.success = "please Enter Remark";
                            return PartialView("GenerateInvoicePartial", invoice);
                        }
                        else
                        {
                            string bdatefrom = DateTime.ParseExact(invoice.Tempdatefrom, formats, CultureInfo.InvariantCulture, DateTimeStyles.None).ToString("MM/dd/yyyy");
                            string bdateto = DateTime.ParseExact(invoice.TempdateTo, formats, CultureInfo.InvariantCulture, DateTimeStyles.None).ToString("MM/dd/yyyy");

                            string invdate = DateTime.ParseExact(invoice.tempInvoicedate, formats, CultureInfo.InvariantCulture, DateTimeStyles.None).ToString("MM/dd/yyyy");

                            double netAmt = Convert.ToDouble(inv.netamount);

                            invoice.periodfrom = Convert.ToDateTime(bdatefrom);
                            invoice.periodto = Convert.ToDateTime(bdateto);
                            invoice.invoicedate = Convert.ToDateTime(invdate);




                            //ViewBag.nextinvoice = GetmaxInvoiceno(invstart);


                            invoice.IN_Id = inv.IN_Id;

                            invoice.invoiceno = invoice.invoiceno;

                            invoice.Invoice_Lable = AmountTowords.changeToWords(invoice.netamount.ToString());
                            db.Entry(inv).State = EntityState.Detached;
                            db.Entry(invoice).State = EntityState.Modified;
                            db.SaveChanges();


                            //----------------- For InvoiceRemark Save Date Added on 07/09/2022------------------------------

                            InvoiceRemark invremark = new InvoiceRemark();

                            invremark.InvoiceNo = invoice.invoiceno;
                            invremark.Remark = Remark;
                            invremark.NetAmount = netAmt;
                            invremark.UpdatedNetAmount = invoice.netamount;

                            db.InvoiceRemarks.Add(invremark);
                            db.SaveChanges();

                            //-----------------------------------------------------------

                            ViewBag.success = "Invoice Added SuccessFully";
                        }
                    }
                    else
                    {
                        string bdatefrom = DateTime.ParseExact(invoice.Tempdatefrom, formats, CultureInfo.InvariantCulture, DateTimeStyles.None).ToString("MM/dd/yyyy");
                        string bdateto = DateTime.ParseExact(invoice.TempdateTo, formats, CultureInfo.InvariantCulture, DateTimeStyles.None).ToString("MM/dd/yyyy");

                        string invdate = DateTime.ParseExact(invoice.tempInvoicedate, formats, CultureInfo.InvariantCulture, DateTimeStyles.None).ToString("MM/dd/yyyy");


                        invoice.periodfrom = Convert.ToDateTime(bdatefrom);
                        invoice.periodto = Convert.ToDateTime(bdateto);
                        invoice.invoicedate = Convert.ToDateTime(invdate);


                        invoice.Invoice_Lable = AmountTowords.changeToWords(invoice.netamount.ToString());

                        //ViewBag.nextinvoice = GetmaxInvoiceno(invstart);

                        invoice.invoiceno = invoice.invoiceno;

                        db.Invoices.Add(invoice);
                        db.SaveChanges();

                        ViewBag.success = "Invoice Added SuccessFully";

                    }

                    string Pfcode = db.Companies.Where(m => m.Company_Id == invoice.Customer_Id).Select(m => m.Pf_code).FirstOrDefault(); /// take dynamically


                    LocalReport lr = new LocalReport();

                    //var dataset = db.TransactionViews.Where(m => m.Customer_Id == invoice.Customer_Id && !db.singleinvoiceconsignments.Select(b => b.Consignment_no).Contains(m.Consignment_no))
                    //              .ToList().
                    //              Where(x => DateTime.Compare(x.booking_date.Value.Date, invoice.periodfrom.Value.Date) >= 0 && DateTime.Compare(x.booking_date.Value.Date, invoice.periodto.Value.Date) <= 0).OrderBy(m => m.booking_date).ThenBy(n => n.Consignment_no)
                    //         .ToList();

                    var dataset = db.TransactionViews.Where(m => m.Customer_Id == invoice.Customer_Id && !db.singleinvoiceconsignments.Select(b => b.Consignment_no).Contains(m.Consignment_no)).ToList().
                 Where(x => DateTime.Compare(x.booking_date.Value.Date, invoice.periodfrom.Value.Date) >= 0 && DateTime.Compare(x.booking_date.Value.Date, invoice.periodto.Value.Date) <= 0).OrderBy(m => m.booking_date).ThenBy(n => n.Consignment_no)
                               .ToList();

                    var dataset2 = db.Franchisees.Where(x => x.PF_Code == Pfcode);

                    var dataset3 = db.Invoices.OrderByDescending(m => m.invoiceno == invoice.invoiceno);

                    var dataset4 = db.Companies.Where(m => m.Company_Id == invoice.Customer_Id);

                    string clientGst = dataset4.FirstOrDefault().Gst_No;
                    string frgst = dataset2.FirstOrDefault().GstNo;
                    string discount = dataset3.FirstOrDefault().discount;

                    if (discount == "no")
                    {
                        if (clientGst != null && clientGst.Length > 4)
                        {
                            if(invoice.Customer_Id.ToUpper()== "TRESVISTA ANALYTICS LLP PUNE".ToUpper())
                            {
                                string path = Path.Combine(Server.MapPath("~/RdlcReport"), "PrintInvoiceIGST.rdlc");

                                if (System.IO.File.Exists(path))
                                {
                                    lr.ReportPath = path;
                                }
                            }
                            else if (frgst.Substring(0, 2) == clientGst.Substring(0, 2))
                            {
                                string path = Path.Combine(Server.MapPath("~/RdlcReport"), "PrintInvoice.rdlc");

                                if (System.IO.File.Exists(path))
                                {
                                    lr.ReportPath = path;
                                }

                            }
                            else
                            {
                                string path = Path.Combine(Server.MapPath("~/RdlcReport"), "PrintInvoiceIGST.rdlc");

                                if (System.IO.File.Exists(path))
                                {
                                    lr.ReportPath = path;
                                }
                            }
                        }
                        else
                        {
                            string path = Path.Combine(Server.MapPath("~/RdlcReport"), "PrintInvoice.rdlc");

                            if (System.IO.File.Exists(path))
                            {
                                lr.ReportPath = path;
                            }
                        }
                    }
                    else if (discount == "yes")
                    {
                        //string path = Path.Combine(Server.MapPath("~/RdlcReport"), "DiscountInvoice.rdlc");

                        //if (System.IO.File.Exists(path))
                        //{
                        //    lr.ReportPath = path;
                        //}

                        if (clientGst != null && clientGst.Length > 4)
                        {
                            if (frgst.Substring(0, 2) == clientGst.Substring(0, 2))
                            {
                                string path = Path.Combine(Server.MapPath("~/RdlcReport"), "DiscountInvoice.rdlc");

                                if (System.IO.File.Exists(path))
                                {
                                    lr.ReportPath = path;
                                }

                            }
                            else
                            {
                                string path = Path.Combine(Server.MapPath("~/RdlcReport"), "DiscountInvoiceIGST.rdlc");

                                if (System.IO.File.Exists(path))
                                {
                                    lr.ReportPath = path;
                                }
                            }
                        }
                        else
                        {
                            string path = Path.Combine(Server.MapPath("~/RdlcReport"), "DiscountInvoice.rdlc");

                            if (System.IO.File.Exists(path))
                            {
                                lr.ReportPath = path;
                            }
                        }
                    }



                    ReportDataSource rd = new ReportDataSource("PrintInvoice", dataset);
                    ReportDataSource rd1 = new ReportDataSource("franchisees", dataset2);
                    ReportDataSource rd2 = new ReportDataSource("invoice", dataset3);
                    ReportDataSource rd3 = new ReportDataSource("comp", dataset4);



                    lr.DataSources.Add(rd);
                    lr.DataSources.Add(rd1);
                    lr.DataSources.Add(rd2);
                    lr.DataSources.Add(rd3);

                    string reportType = "pdf";
                    string mimeType;
                    string encoding;
                    string fileNameExte;

                    string deviceInfo =
                        "<DeviceInfo>" +
                        "<OutputFormat>" + "pdf" + "</OutputFormat>" +
                        "<PageHeight>11in</PageHeight>" +
                       "<Margintop>0.1in</Margintop>" +
                         "<Marginleft>0.1in</Marginleft>" +
                          "<Marginright>0.1in</Marginright>" +
                           "<Marginbottom>0.5in</Marginbottom>" +
                           "</DeviceInfo>";

                    Warning[] warnings;
                    string[] streams;
                    byte[] renderByte;


                    renderByte = lr.Render
                  (reportType,
                  deviceInfo,
                  out mimeType,
                  out encoding,
                  out fileNameExte,
                  out streams,
                  out warnings
                  );


                    ViewBag.pdf = false;

                    //if (submit == "Generate")
                    //{
                    ViewBag.pdf = true;
                    ViewBag.invoiceno = invoice.invoiceno;
                    //}
                    if (submit == "Email")
                    {

                        MemoryStream memoryStream = new MemoryStream(renderByte);
                        var emailcontent = db.EmailPromotions.Where(a => a.message == "True").FirstOrDefault();
                        using (MailMessage mm = new MailMessage("billing@atkexp.com", dataset4.FirstOrDefault().Email))
                        {
                            mm.Subject = "Invoice";
                            string Bodytext = "";

                            if (emailcontent != null)
                            {
                                Bodytext = "<html><body>" + emailcontent.note + "</body></html>";
                            }
                            else
                            {
                                Bodytext = "<html><body>Please Find Attachment</body></html>";
                            }

                            Attachment attachment = new Attachment(memoryStream, "Invoice.pdf");

                            mm.IsBodyHtml = true;


                            mm.BodyEncoding = System.Text.Encoding.GetEncoding("utf-8");

                            AlternateView plainView = System.Net.Mail.AlternateView.CreateAlternateViewFromString(System.Text.RegularExpressions.Regex.Replace(Bodytext, @"<(.|\n)*?>", string.Empty), null, "text/plain");
                            // mm.Body = Bodytext;
                            mm.Body = Bodytext;

                            //Add Byte array as Attachment.

                            mm.Attachments.Add(attachment);

                            SmtpClient smtp = new SmtpClient();
                            smtp.Host = "smtp.gmail.com";
                            //smtp.Host = "smtp.gmail.com";
                            mm.ReplyToList.Add("acctsmfpune@dtdc.com");
                            smtp.EnableSsl = true;
                            System.Net.NetworkCredential credentials = new System.Net.NetworkCredential();
                            credentials.UserName = "billing@atkexp.com";
                            credentials.Password = "atk123##";
                            smtp.UseDefaultCredentials = true;
                            smtp.Credentials = credentials;
                            smtp.Port = 587;
                            smtp.Send(mm);
                        }

                    }

                    return PartialView("GenerateInvoicePartial", invoice);

                }
            }
            catch (Exception ex)
            {

            }
            
            return PartialView("GenerateInvoicePartial", invoice);
        }



        [HttpPost]
        public ActionResult SaveDpInvoice(Invoice invoice, string submit, string Remark, HttpPostedFileBase httpPostedFileBase)
        {


            if (invoice.Total_Lable == null)
            {
                ModelState.AddModelError("Total_Lable", "Label Required");
            }

            if (Remark == "")
            {
                ViewBag.success = "please Enter Remark";
                return PartialView("GenerateInvoicePartial", invoice);
            }
            else
            {

                if (ModelState.IsValid)
                {

                    string[] formats = {"dd/MM/yyyy", "dd-MMM-yyyy", "yyyy-MM-dd",
                   "dd-MM-yyyy", "M/d/yyyy", "dd MMM yyyy"};

                    Invoice inv = db.Invoices.Where(m => m.invoiceno == invoice.invoiceno).FirstOrDefault();


                    if (inv != null)
                    {
                        string bdatefrom = DateTime.ParseExact(invoice.Tempdatefrom, formats, CultureInfo.InvariantCulture, DateTimeStyles.None).ToString("MM/dd/yyyy");
                        string bdateto = DateTime.ParseExact(invoice.TempdateTo, formats, CultureInfo.InvariantCulture, DateTimeStyles.None).ToString("MM/dd/yyyy");

                        string invdate = DateTime.ParseExact(invoice.tempInvoicedate, formats, CultureInfo.InvariantCulture, DateTimeStyles.None).ToString("MM/dd/yyyy");

                        double netAmt = Convert.ToDouble(inv.netamount);

                        invoice.periodfrom = Convert.ToDateTime(bdatefrom);
                        invoice.periodto = Convert.ToDateTime(bdateto);
                        invoice.invoicedate = Convert.ToDateTime(invdate);


                        //ViewBag.nextinvoice = GetmaxInvoiceno(invstart);


                        invoice.IN_Id = inv.IN_Id;

                        invoice.invoiceno = invoice.invoiceno;

                        invoice.fullsurchargetaxtotal = 0;
                        invoice.fullsurchargetax = 0;
                        invoice.discountper = 0;
                        invoice.discountamount = 0;
                        invoice.discount = "no";
                        invoice.othercharge = 0;
                        invoice.Invoice_Lable = AmountTowords.changeToWords(invoice.netamount.ToString());

                        db.Entry(inv).State = EntityState.Detached;
                        db.Entry(invoice).State = EntityState.Modified;
                        db.SaveChanges();

                        //----------------- For InvoiceRemark Save Date Added on 08/09/2022------------------------------

                        InvoiceRemark invremark = new InvoiceRemark();

                        invremark.InvoiceNo = invoice.invoiceno;
                        invremark.Remark = Remark;
                        invremark.NetAmount = netAmt;
                        invremark.UpdatedNetAmount = invoice.netamount;

                        db.InvoiceRemarks.Add(invremark);
                        db.SaveChanges();

                        //-----------------------------------------------------------

                        ViewBag.success = "Invoice Added SuccessFully";
                    }
                    else
                    {
                        string bdatefrom = DateTime.ParseExact(invoice.Tempdatefrom, formats, CultureInfo.InvariantCulture, DateTimeStyles.None).ToString("MM/dd/yyyy");
                        string bdateto = DateTime.ParseExact(invoice.TempdateTo, formats, CultureInfo.InvariantCulture, DateTimeStyles.None).ToString("MM/dd/yyyy");

                        string invdate = DateTime.ParseExact(invoice.tempInvoicedate, formats, CultureInfo.InvariantCulture, DateTimeStyles.None).ToString("MM/dd/yyyy");


                        invoice.periodfrom = Convert.ToDateTime(bdatefrom);
                        invoice.periodto = Convert.ToDateTime(bdateto);
                        invoice.invoicedate = Convert.ToDateTime(invdate);

                        // ViewBag.nextinvoice = GetmaxInvoiceno(invstart);

                        invoice.invoiceno = invoice.invoiceno;

                        invoice.fullsurchargetaxtotal = 0;
                        invoice.fullsurchargetax = 0;
                        invoice.Invoice_Lable = AmountTowords.changeToWords(invoice.netamount.ToString());
                        db.Invoices.Add(invoice);
                        db.SaveChanges();

                        ViewBag.success = "Invoice Added SuccessFully";

                    }

                    string Pfcode = db.Companies.Where(m => m.Company_Id == invoice.Customer_Id).Select(m => m.Pf_code).FirstOrDefault(); /// take dynamically


                    LocalReport lr = new LocalReport();

                    var dataset = db.TransactionViews.Where(m => m.Customer_Id == invoice.Customer_Id && !db.singleinvoiceconsignments.Select(b => b.Consignment_no).Contains(m.Consignment_no)).ToList().
                                Where(x => DateTime.Compare(x.booking_date.Value.Date, invoice.periodfrom.Value.Date) >= 0 && DateTime.Compare(x.booking_date.Value.Date, invoice.periodto.Value.Date) <= 0).OrderBy(m => m.booking_date).ThenBy(n => n.Consignment_no)
                                              .ToList();


                    var dataset2 = db.Franchisees.Where(x => x.PF_Code == Pfcode);

                    var dataset3 = db.Invoices.OrderByDescending(m => m.invoiceno == invoice.invoiceno);

                    var dataset4 = db.Companies.Where(m => m.Company_Id == invoice.Customer_Id);

                    string clientGst = dataset4.FirstOrDefault().Gst_No;
                    string frgst = dataset2.FirstOrDefault().GstNo;


                    if (clientGst != null && clientGst.Length > 4)
                    {
                        if (frgst.Substring(0, 2) == clientGst.Substring(0, 2))
                        {
                            string path = Path.Combine(Server.MapPath("~/RdlcReport"), "DpPrintInvoice.rdlc");

                            if (System.IO.File.Exists(path))
                            {
                                lr.ReportPath = path;
                            }

                        }
                        else
                        {
                            string path = Path.Combine(Server.MapPath("~/RdlcReport"), "DpPrintInvoiceIGST.rdlc");

                            if (System.IO.File.Exists(path))
                            {
                                lr.ReportPath = path;
                            }
                        }
                    }
                    else
                    {
                        string path = Path.Combine(Server.MapPath("~/RdlcReport"), "DpPrintInvoice.rdlc");

                        if (System.IO.File.Exists(path))
                        {
                            lr.ReportPath = path;
                        }
                    }


                    ReportDataSource rd = new ReportDataSource("PrintInvoice", dataset);
                    ReportDataSource rd1 = new ReportDataSource("franchisees", dataset2);
                    ReportDataSource rd2 = new ReportDataSource("invoice", dataset3);
                    ReportDataSource rd3 = new ReportDataSource("comp", dataset4);

                    lr.DataSources.Add(rd);
                    lr.DataSources.Add(rd1);
                    lr.DataSources.Add(rd2);
                    lr.DataSources.Add(rd3);

                    string reportType = "pdf";
                    string mimeType;
                    string encoding;
                    string fileNameExte;

                    string deviceInfo =
                        "<DeviceInfo>" +
                        "<OutputFormat>" + "pdf" + "</OutputFormat>" +
                        "<PageHeight>11in</PageHeight>" +
                       "<Margintop>0.1in</Margintop>" +
                         "<Marginleft>0.1in</Marginleft>" +
                          "<Marginright>0.1in</Marginright>" +
                           "<Marginbottom>0.5in</Marginbottom>" +
                           "</DeviceInfo>";

                    Warning[] warnings;
                    string[] streams;
                    byte[] renderByte;


                    renderByte = lr.Render
                  (reportType,
                  deviceInfo,
                  out mimeType,
                  out encoding,
                  out fileNameExte,
                  out streams,
                  out warnings
                  );


                    //if (submit == "Generate")
                    //{
                    ViewBag.pdf = true;
                    ViewBag.invoiceno = invoice.invoiceno;
                    ViewBag.companyname = dataset4.FirstOrDefault();
                    // }
                    string savePath = Server.MapPath("~/PDF/" + dataset4.FirstOrDefault().Company_Name.Replace("/", "-") + " " + dataset3.FirstOrDefault().invoiceno.Replace("/", "-") + ".pdf");
                    ViewBag.path=savePath;
                    using (FileStream stream = new FileStream(savePath, FileMode.Create))
                    {
                        stream.Write(renderByte, 0, renderByte.Length);
                    }

                  //  return dataset4.FirstOrDefault().Company_Name + " " + dataset3.FirstOrDefault().invoiceno.Replace("/", "-") + ".pdf";

                    if (submit == "Email")
                    {

                        MemoryStream memoryStream = new MemoryStream(renderByte);



                        using (MailMessage mm = new MailMessage("billing@atkexp.com", dataset4.FirstOrDefault().Email))
                        {
                            mm.Subject = "Invoice";

                            string Bodytext = "<html><body>Please Find Attachment</body></html>";
                            Attachment attachment = new Attachment(memoryStream, "Invoice.pdf");

                            mm.IsBodyHtml = true;

                            mm.BodyEncoding = System.Text.Encoding.GetEncoding("utf-8");

                            AlternateView plainView = System.Net.Mail.AlternateView.CreateAlternateViewFromString(System.Text.RegularExpressions.Regex.Replace(Bodytext, @"<(.|\n)*?>", string.Empty), null, "text/plain");
                            // mm.Body = Bodytext;
                            mm.Body = Bodytext;

                            //Add Byte array as Attachment.

                            mm.Attachments.Add(attachment);

                            SmtpClient smtp = new SmtpClient();
                            smtp.Host = "smtp.gmail.com";
                            //smtp.Host = "smtp.gmail.com";
                            mm.ReplyToList.Add("acctsmfpune@dtdc.com");
                            smtp.EnableSsl = true;
                            System.Net.NetworkCredential credentials = new System.Net.NetworkCredential();
                            credentials.UserName = "billing@atkexp.com";
                            credentials.Password = "atk123##";
                            smtp.UseDefaultCredentials = true;
                            smtp.Credentials = credentials;
                            smtp.Port = 587;
                            smtp.Send(mm);
                        }


                    }

                    // importFromExcelWhole(httpPostedFileBase);

                    return PartialView("DpInvoicePartial", invoice);

                }
            }
            return PartialView("DpInvoicePartial", invoice);
        }



        [HttpPost]
        public ActionResult SaveInvoiceLastYear(Invoice invoice, string submit)
        {


            if (invoice.Total_Lable == null)
            {
                ModelState.AddModelError("Total_Lable", "Label Required");
            }





            if (ModelState.IsValid)
            {

                string[] formats = {"dd/MM/yyyy", "dd-MMM-yyyy", "yyyy-MM-dd",
                   "dd-MM-yyyy", "M/d/yyyy", "dd MMM yyyy"};

                Invoice inv = db.Invoices.Where(m => m.invoiceno == invstart + invoice.invoiceno).FirstOrDefault();


                if (inv != null)
                {
                    string bdatefrom = DateTime.ParseExact(invoice.Tempdatefrom, formats, CultureInfo.InvariantCulture, DateTimeStyles.None).ToString("MM/dd/yyyy");
                    string bdateto = DateTime.ParseExact(invoice.TempdateTo, formats, CultureInfo.InvariantCulture, DateTimeStyles.None).ToString("MM/dd/yyyy");

                    string invdate = DateTime.ParseExact(invoice.tempInvoicedate, formats, CultureInfo.InvariantCulture, DateTimeStyles.None).ToString("MM/dd/yyyy");


                    invoice.periodfrom = Convert.ToDateTime(bdatefrom);
                    invoice.periodto = Convert.ToDateTime(bdateto);
                    invoice.invoicedate = Convert.ToDateTime(invdate);




                    ViewBag.nextinvoice = GetmaxInvoiceno(invstart);


                    invoice.IN_Id = inv.IN_Id;

                    invoice.invoiceno = invstart + invoice.invoiceno;

                    invoice.fullsurchargetaxtotal = 0;
                    invoice.fullsurchargetax = 0;
                    invoice.discountper = 0;
                    invoice.discountamount = 0;
                    invoice.discount = "no";
                    invoice.othercharge = 0;
                    invoice.Invoice_Lable = AmountTowords.changeToWords(invoice.netamount.ToString());

                    db.Entry(inv).State = EntityState.Detached;
                    db.Entry(invoice).State = EntityState.Modified;
                    db.SaveChanges();
                    ViewBag.success = "Invoice Added SuccessFully";
                }
                else
                {
                    string bdatefrom = DateTime.ParseExact(invoice.Tempdatefrom, formats, CultureInfo.InvariantCulture, DateTimeStyles.None).ToString("MM/dd/yyyy");
                    string bdateto = DateTime.ParseExact(invoice.TempdateTo, formats, CultureInfo.InvariantCulture, DateTimeStyles.None).ToString("MM/dd/yyyy");

                    string invdate = DateTime.ParseExact(invoice.tempInvoicedate, formats, CultureInfo.InvariantCulture, DateTimeStyles.None).ToString("MM/dd/yyyy");


                    invoice.periodfrom = Convert.ToDateTime(bdatefrom);
                    invoice.periodto = Convert.ToDateTime(bdateto);
                    invoice.invoicedate = Convert.ToDateTime(invdate);




                    ViewBag.nextinvoice = GetmaxInvoiceno(invstart);

                    invoice.invoiceno = invstart + invoice.invoiceno;

                    invoice.fullsurchargetaxtotal = 0;
                    invoice.fullsurchargetax = 0;
                    invoice.Invoice_Lable = AmountTowords.changeToWords(invoice.netamount.ToString());
                    db.Invoices.Add(invoice);
                    db.SaveChanges();

                    ViewBag.success = "Invoice Added SuccessFully";

                }





                string Pfcode = db.Companies.Where(m => m.Company_Id == invoice.Customer_Id).Select(m => m.Pf_code).FirstOrDefault(); /// take dynamically


                LocalReport lr = new LocalReport();










                //if (submit == "Generate")
                //{
                ViewBag.pdf = true;
                ViewBag.invoiceno = invoice.invoiceno;
                // }


                








                return PartialView("GenerateInvoiceLastYearPartial", invoice);

            }
            return PartialView("GenerateInvoiceLastYearPartial", invoice);
        }




        [HttpGet]
        public ActionResult ReportPrinterMethod(string myParameter)
        {
            {

                LocalReport lr = new LocalReport();



                Invoice inc = db.Invoices.Where(m => m.invoiceno == myParameter).FirstOrDefault();

                string Pfcode = db.Companies.Where(m => m.Company_Id == inc.Customer_Id).Select(m => m.Pf_code).FirstOrDefault();

                //var dataset = db.TransactionViews.Where(m => m.Customer_Id == inc.Customer_Id && !db.singleinvoiceconsignments.Select(b => b.Consignment_no).Contains(m.Consignment_no))
                //           .ToList().
                //           Where(x => DateTime.Compare(x.booking_date.Value.Date, inc.periodfrom.Value.Date) >= 0 && DateTime.Compare(x.booking_date.Value.Date, inc.periodto.Value.Date) <= 0).OrderBy(m => m.booking_date).ThenBy(n => n.Consignment_no)
                //      .ToList();



                var dataset = db.TransactionViews.Where(m => m.Customer_Id == inc.Customer_Id && !db.singleinvoiceconsignments.Select(b => b.Consignment_no).Contains(m.Consignment_no)).ToList().
          Where(x => DateTime.Compare(x.booking_date.Value.Date, inc.periodfrom.Value.Date) >= 0 && DateTime.Compare(x.booking_date.Value.Date, inc.periodto.Value.Date) <= 0).OrderBy(m => m.booking_date).ThenBy(n => n.Consignment_no)
                        .ToList();


                var dataset2 = db.Franchisees.Where(x => x.PF_Code == Pfcode);

                var dataset3 = db.Invoices.OrderByDescending(m => m.invoiceno == inc.invoiceno);

                var dataset4 = db.Companies.Where(m => m.Company_Id == inc.Customer_Id);


                /////////////////Total//////////////

                /////////////////Total//////////////

                string clientGst = dataset4.FirstOrDefault().Gst_No;
                string frgst = dataset2.FirstOrDefault().GstNo;


                if (clientGst != null && clientGst.Length > 4)
                {
                    if (frgst.Substring(0, 2) == clientGst.Substring(0, 2))
                    {
                        string path = Path.Combine(Server.MapPath("~/RdlcReport"), "PrintInvoice.rdlc");

                        if (System.IO.File.Exists(path))
                        {
                            lr.ReportPath = path;
                        }

                    }
                    else
                    {
                        string path = Path.Combine(Server.MapPath("~/RdlcReport"), "PrintInvoiceIGST.rdlc");

                        if (System.IO.File.Exists(path))
                        {
                            lr.ReportPath = path;
                        }
                    }
                }
                else
                {
                    string path = Path.Combine(Server.MapPath("~/RdlcReport"), "PrintInvoice.rdlc");

                    if (System.IO.File.Exists(path))
                    {
                        lr.ReportPath = path;
                    }
                }





                ////////////////////////////////////
                ReportDataSource rd = new ReportDataSource("PrintInvoice", dataset);
                ReportDataSource rd1 = new ReportDataSource("franchisees", dataset2);
                ReportDataSource rd2 = new ReportDataSource("invoice", dataset3);
                ReportDataSource rd3 = new ReportDataSource("comp", dataset4);

                //  ReportParameter[] allPar = new ReportParameter[1]; // create parameters array
                //  ReportParameter parSum = new ReportParameter("Dcno", dcno);



                //  lr.SetParameters(new ReportParameter[] { parSum });

                lr.DataSources.Add(rd);
                lr.DataSources.Add(rd1);
                lr.DataSources.Add(rd2);
                lr.DataSources.Add(rd3);

                string reportType = "pdf";
                string mimeType;
                string encoding;
                string fileNameExte;

                string deviceInfo =
                    "<DeviceInfo>" +
                    "<OutputFormat>" + "pdf" + "</OutputFormat>" +
                    "<PageHeight>11in</PageHeight>" +
                   "<Margintop>0.1in</Margintop>" +
                     "<Marginleft>0.1in</Marginleft>" +
                      "<Marginright>0.1in</Marginright>" +
                       "<Marginbottom>0.5in</Marginbottom>" +
                       "</DeviceInfo>";

                Warning[] warnings;
                string[] streams;
                byte[] renderByte;


                renderByte = lr.Render
              (reportType,
              deviceInfo,
              out mimeType,
              out encoding,
              out fileNameExte,
              out streams,
              out warnings
              );

                return File(renderByte, mimeType);
            }

        }


        [HttpGet]
        public ActionResult DpReportPrinterMethod(string myParameter)
        {
            {

                LocalReport lr = new LocalReport();



                Invoice inc = db.Invoices.Where(m => m.invoiceno == myParameter).FirstOrDefault();

                string Pfcode = db.Companies.Where(m => m.Company_Id == inc.Customer_Id).Select(m => m.Pf_code).FirstOrDefault();

                var dataset = db.TransactionViews.Where(m => m.Customer_Id == inc.Customer_Id)
                           .ToList().
                           Where(x => DateTime.Compare(x.booking_date.Value.Date, inc.periodfrom.Value.Date) >= 0 && DateTime.Compare(x.booking_date.Value.Date, inc.periodto.Value.Date) <= 0)
                      .ToList();


                var dataset2 = db.Franchisees.Where(x => x.PF_Code == Pfcode);

                var dataset3 = db.Invoices.OrderByDescending(m => m.invoiceno == inc.invoiceno);

                var dataset4 = db.Companies.Where(m => m.Company_Id == inc.Customer_Id);


                /////////////////Total//////////////

                /////////////////Total//////////////

                string clientGst = dataset4.FirstOrDefault().Gst_No;
                string frgst = dataset2.FirstOrDefault().GstNo;


                if (clientGst != null && clientGst.Length > 4)
                {
                    if (frgst.Substring(0, 2) == clientGst.Substring(0, 2))
                    {
                        string path = Path.Combine(Server.MapPath("~/RdlcReport"), "DpPrintInvoice.rdlc");

                        if (System.IO.File.Exists(path))
                        {
                            lr.ReportPath = path;
                        }

                    }
                    else
                    {
                        string path = Path.Combine(Server.MapPath("~/RdlcReport"), "DpPrintInvoiceIGST.rdlc");

                        if (System.IO.File.Exists(path))
                        {
                            lr.ReportPath = path;
                        }
                    }
                }
                else
                {
                    string path = Path.Combine(Server.MapPath("~/RdlcReport"), "DpPrintInvoice.rdlc");

                    if (System.IO.File.Exists(path))
                    {
                        lr.ReportPath = path;
                    }
                }





                ////////////////////////////////////
                ReportDataSource rd = new ReportDataSource("PrintInvoice", dataset);
                ReportDataSource rd1 = new ReportDataSource("franchisees", dataset2);
                ReportDataSource rd2 = new ReportDataSource("invoice", dataset3);
                ReportDataSource rd3 = new ReportDataSource("comp", dataset4);

                //  ReportParameter[] allPar = new ReportParameter[1]; // create parameters array
                //  ReportParameter parSum = new ReportParameter("Dcno", dcno);



                //  lr.SetParameters(new ReportParameter[] { parSum });

                lr.DataSources.Add(rd);
                lr.DataSources.Add(rd1);
                lr.DataSources.Add(rd2);
                lr.DataSources.Add(rd3);

                string reportType = "pdf";
                string mimeType;
                string encoding;
                string fileNameExte;

                string deviceInfo =
                    "<DeviceInfo>" +
                    "<OutputFormat>" + "pdf" + "</OutputFormat>" +
                    "<PageHeight>11in</PageHeight>" +
                   "<Margintop>0.1in</Margintop>" +
                     "<Marginleft>0.1in</Marginleft>" +
                      "<Marginright>0.1in</Marginright>" +
                       "<Marginbottom>0.5in</Marginbottom>" +
                       "</DeviceInfo>";

                Warning[] warnings;
                string[] streams;
                byte[] renderByte;


                renderByte = lr.Render
              (reportType,
              deviceInfo,
              out mimeType,
              out encoding,
              out fileNameExte,
              out streams,
              out warnings
              );

                return File(renderByte, mimeType);
            }

        }


        public ActionResult MultipleInvoice()
        {
            ViewBag.PfCode = new SelectList(db.Franchisees.Where(x => !x.PF_Code.ToUpper().StartsWith("PR")), "PF_Code", "PF_Code");
            ViewBag.Complist = db.Companies.Where(m => !(m.Company_Id.StartsWith("Cash_")) && !(m.Company_Id.StartsWith("BASIC_TS")) && m.IsAgreementoption!=1).Select(m => m.Company_Id).ToList();
            return View();
        }
        [HttpPost]
        public async Task<ActionResult> MultipleInvoice(string[] Companies, Invoice invoice, string submit,string PfCode)
        {
            ViewBag.PfCode = new SelectList(db.Franchisees, "PF_Code", "PF_Code", PfCode);
            ViewBag.Complist = db.Companies.Where(m => !(m.Company_Id.StartsWith("Cash_")) && !(m.Company_Id.StartsWith("BASIC_TS")) && m.IsAgreementoption != 1).Select(m => m.Company_Id).ToList();




            if (ModelState.IsValid)
            {

                Task.Run(() => MultipleInvoiceAsyncMethod(Companies, invoice, submit));

                ViewBag.Success = "All Invoices Generated SuccessFully";
            }


            return View();
        }

        public void MultipleInvoiceAsyncMethod(string[] Companies, Invoice invoice, string submit)
        {
            string[] formats = {"dd/MM/yyyy", "dd-MMM-yyyy", "yyyy-MM-dd",
                   "dd-MM-yyyy", "M/d/yyyy", "dd MMM yyyy"};

            string bdatefrom = DateTime.ParseExact(invoice.Tempdatefrom, formats, CultureInfo.InvariantCulture, DateTimeStyles.None).ToString("MM/dd/yyyy");
            string bdateto = DateTime.ParseExact(invoice.TempdateTo, formats, CultureInfo.InvariantCulture, DateTimeStyles.None).ToString("MM/dd/yyyy");
            string invoicedate = DateTime.ParseExact(invoice.tempInvoicedate, formats, CultureInfo.InvariantCulture, DateTimeStyles.None).ToString("MM/dd/yyyy");


            DateTime fromdate = Convert.ToDateTime(bdatefrom);
            DateTime todate = Convert.ToDateTime(bdateto);
            DateTime invdate = Convert.ToDateTime(invoicedate);



            foreach (var i in Companies)
            {

                Company cm = db.Companies.Where(m => m.Company_Id == i).FirstOrDefault();

                var TrList = db.TransactionViews.Where(m => m.Customer_Id == i).ToList().
               Where(x => DateTime.Compare(x.booking_date.Value.Date, fromdate) >= 0 && DateTime.Compare(x.booking_date.Value.Date, todate) <= 0)
                                 .ToList();


                Invoice inv = new Invoice();



                double? AmountTotal = TrList.Sum(m => m.Amount ?? 0);

                double? RisksurchargeTotal = TrList.Sum(m => m.Risksurcharge ?? 0);


                inv.total = AmountTotal + RisksurchargeTotal;

                inv.fullsurchargetax = cm.Fuel_Sur_Charge ?? 0;

                inv.periodfrom = fromdate;
                inv.servicetax = invoice.servicetax;
                //
                inv.covidtax = invoice.covidtax ?? 0;
                //

                inv.periodto = todate;
                inv.invoicedate = invdate;
                inv.Tempdatefrom = invoice.Tempdatefrom;
                inv.TempdateTo = invoice.TempdateTo;
                inv.tempInvoicedate = invoice.tempInvoicedate;
                inv.Address = db.Companies.Where(m => m.Company_Id == i).Select(m => m.Company_Address).FirstOrDefault();
                inv.Customer_Id = i;

                inv.fullsurchargetaxtotal = ((inv.total * Convert.ToDouble(cm.Fuel_Sur_Charge)) / 100);
                //
                inv.covidtaxtotal = (inv.total * inv.covidtax) / 100;
                //
                //string invoiceno = db.Invoices.OrderByDescending(m => m.IN_Id).Select(m => m.invoiceno).FirstOrDefault();
                //int number = Convert.ToInt32(invoiceno.Substring(10))+1;

                var pfcode = (from d in db.Companies
                              where d.Company_Id == i
                              select new { d.Pf_code }).FirstOrDefault();

                //Invoice inv = db.Invoices.Where(m => m.invoiceno == invono).FirstOrDefault();

               
                    string lastInvoiceno = "";
                    
                  if (pfcode != null)
                  {
                        var branchcode = (from d in db.Franchisees
                                          where d.PF_Code == pfcode.Pf_code
                                          select new { d.BranchCode }).FirstOrDefault();

                    string getOldInvoiceWith2Slash = "ATK/" + branchcode.BranchCode + "/24-25/";
                    
                    lastInvoiceno = db.Invoices.Where(m => m.invoiceno.StartsWith(getOldInvoiceWith2Slash)).OrderByDescending(m => m.IN_Id).Take(1).Select(m => m.invoiceno).FirstOrDefault();

                    //string lastInvoiceno = db.Invoices.Where(m => m.invoiceno.StartsWith(invstart)).OrderByDescending(m => m.IN_Id).Take(1).Select(m => m.invoiceno).FirstOrDefault() ?? invstart + 0;

                    if (lastInvoiceno == null)
                    {
                        //int number = Convert.ToInt32(lastInvoiceno1.Substring(13));
                        //no = lastInvoiceno.Substring(13);
                        string Invoiceno = getOldInvoiceWith2Slash + "" + (0 + 1);
                      
                    }
                    else
                    {

                        string[] strarrinvno = lastInvoiceno.Split('/');
                        int newnumber = Convert.ToInt32(strarrinvno[3]) + 1;

                        string Invoiceno = getOldInvoiceWith2Slash + "" + newnumber;
                       
                    }


                }

                   
                inv.discount = "no";

                inv.Docket_charges = 0;

                foreach (var j in TrList)
                {
                    if (j.Consignment_no.ToLower().StartsWith("d"))
                    {
                        inv.Docket_charges = inv.Docket_charges + Convert.ToDouble(cm.D_Docket);
                    }
                    else if (j.Consignment_no.ToLower().StartsWith("p"))
                    {
                        inv.Docket_charges = inv.Docket_charges + Convert.ToDouble(cm.P_Docket);
                    }
                    else if (j.Consignment_no.ToLower().StartsWith("e"))
                    {
                        inv.Docket_charges = inv.Docket_charges + Convert.ToDouble(cm.E_Docket);
                    }
                    else if (j.Consignment_no.ToLower().StartsWith("v"))
                    {
                        inv.Docket_charges = inv.Docket_charges + Convert.ToDouble(cm.V_Docket);
                    }
                    else if (j.Consignment_no.ToLower().StartsWith("i"))
                    {
                        inv.Docket_charges = inv.Docket_charges + Convert.ToDouble(cm.I_Docket);
                    }
                    else if (j.Consignment_no.ToLower().StartsWith("n"))
                    {
                        inv.Docket_charges = inv.Docket_charges + Convert.ToDouble(cm.N_Docket);
                    }
                }

                inv.Royalty_charges = ((inv.total * Convert.ToDouble(cm.Royalty_Charges)) / 100);
                //
                inv.covidtaxtotal = (inv.total * inv.covidtax ?? 0) / 100;
                //
                inv.servicetaxtotal = (((inv.total + inv.fullsurchargetaxtotal + inv.Docket_charges + inv.Royalty_charges+inv.covidtaxtotal) * invoice.servicetax) / 100); //((gst_total * parseFloat("0" + gst)) / 100);
               
                inv.netamount = inv.total + inv.Docket_charges + inv.Royalty_charges + inv.servicetaxtotal + inv.fullsurchargetaxtotal+inv.covidtaxtotal;
                inv.Invoice_Lable = AmountTowords.changeToWords(invoice.netamount.ToString());

                if (inv.netamount > 0)
                {
                    db.Invoices.Add(inv);
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

                    //if (submit == "Email")
                    //{
                    SendMailInvoiceMultiple(inv, submit);
                    // }
                }
            }


            Notification nt = new Notification();

            TimeZoneInfo INDIAN_ZONE = TimeZoneInfo.FindSystemTimeZoneById("India Standard Time");
            nt.dateN = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, INDIAN_ZONE);
            nt.Message = "From Company Id" + Companies.FirstOrDefault() + "to" + Companies.LastOrDefault() + "Invoices Generated SuccessFully";
            nt.Status = false;

            db.Notifications.Add(nt);
            db.SaveChanges();


        }



        public void SendMailInvoiceMultiple(Invoice invoice, string submit)
        {
            LocalReport lr = new LocalReport();

            string Pfcode = db.Companies.Where(m => m.Company_Id == invoice.Customer_Id).Select(m => m.Pf_code).FirstOrDefault();

            //var dataset = db.TransactionViews.Where(m => m.Customer_Id == invoice.Customer_Id && !db.singleinvoiceconsignments.Select(b => b.Consignment_no).Contains(m.Consignment_no))
            //                 .ToList().
            //                 Where(x => DateTime.Compare(x.booking_date.Value.Date, invoice.periodfrom.Value.Date) >= 0 && DateTime.Compare(x.booking_date.Value.Date, invoice.periodto.Value.Date) <= 0).OrderBy(m => m.booking_date).ThenBy(n => n.Consignment_no)
            //            .ToList();


            var dataset = db.TransactionViews.Where(m => m.Customer_Id == invoice.Customer_Id && !db.singleinvoiceconsignments.Select(b => b.Consignment_no).Contains(m.Consignment_no)).ToList().
            Where(x => DateTime.Compare(x.booking_date.Value.Date, invoice.periodfrom.Value.Date) >= 0 && DateTime.Compare(x.booking_date.Value.Date, invoice.periodto.Value.Date) <= 0).OrderBy(m => m.booking_date).ThenBy(n => n.Consignment_no)
                          .ToList();


            var dataset2 = db.Franchisees.Where(x => x.PF_Code == Pfcode);

            var dataset3 = db.Invoices.OrderByDescending(m => m.invoiceno == invoice.invoiceno);

            var dataset4 = db.Companies.Where(m => m.Company_Id == invoice.Customer_Id);

            string clientGst = dataset4.FirstOrDefault().Gst_No;
            string frgst = dataset2.FirstOrDefault().GstNo;


            if (clientGst != null && clientGst.Length > 4)
            {
                if (frgst.Substring(0, 2) == clientGst.Substring(0, 2))
                {
                    string path = Path.Combine(Server.MapPath("~/RdlcReport"), "PrintInvoice.rdlc");

                    if (System.IO.File.Exists(path))
                    {
                        lr.ReportPath = path;
                    }

                }
                else
                {
                    string path = Path.Combine(Server.MapPath("~/RdlcReport"), "PrintInvoiceIGST.rdlc");

                    if (System.IO.File.Exists(path))
                    {
                        lr.ReportPath = path;
                    }
                }
            }
            else
            {
                string path = Path.Combine(Server.MapPath("~/RdlcReport"), "PrintInvoice.rdlc");

                if (System.IO.File.Exists(path))
                {
                    lr.ReportPath = path;
                }
            }




            ReportDataSource rd = new ReportDataSource("PrintInvoice", dataset);
            ReportDataSource rd1 = new ReportDataSource("franchisees", dataset2);
            ReportDataSource rd2 = new ReportDataSource("invoice", dataset3);
            ReportDataSource rd3 = new ReportDataSource("comp", dataset4);



            lr.DataSources.Add(rd);
            lr.DataSources.Add(rd1);
            lr.DataSources.Add(rd2);
            lr.DataSources.Add(rd3);

            string reportType = "pdf";
            string mimeType;
            string encoding;
            string fileNameExte;

            string deviceInfo =
                "<DeviceInfo>" +
                "<OutputFormat>" + "pdf" + "</OutputFormat>" +
                "<PageHeight>11in</PageHeight>" +
               "<Margintop>0.1in</Margintop>" +
                 "<Marginleft>0.1in</Marginleft>" +
                  "<Marginright>0.1in</Marginright>" +
                   "<Marginbottom>0.5in</Marginbottom>" +
                   "</DeviceInfo>";

            Warning[] warnings;
            string[] streams;
            byte[] renderByte;


            renderByte = lr.Render
          (reportType,
          deviceInfo,
          out mimeType,
          out encoding,
          out fileNameExte,
          out streams,
          out warnings
          );

            if (submit == "Email")
            {
                MemoryStream memoryStream = new MemoryStream(renderByte);

                
                var emailcontent = db.EmailPromotions.Where(a => a.message == "True").FirstOrDefault();
               
                using (MailMessage mm = new MailMessage("billing@atkexp.com", dataset4.FirstOrDefault().Email))
                {
                    mm.Subject = "Invoice";
                    string Bodytext = "";

                    if (emailcontent != null)
                    {
                        Bodytext = "<html><body>" + emailcontent.note + "</body></html>";
                    }
                    else
                    {
                        Bodytext = "<html><body>Please Find Attachment</body></html>";
                    }

                   // string Bodytext = "<html><body>Please Find Attachment</body></html>";
                    Attachment attachment = new Attachment(memoryStream, "Invoice.pdf");

                    mm.IsBodyHtml = true;



                    mm.BodyEncoding = System.Text.Encoding.GetEncoding("utf-8");

                    AlternateView plainView = System.Net.Mail.AlternateView.CreateAlternateViewFromString(System.Text.RegularExpressions.Regex.Replace(Bodytext, @"<(.|\n)*?>", string.Empty), null, "text/plain");
                    // mm.Body = Bodytext;
                    mm.Body = Bodytext;

                    //Add Byte array as Attachment.

                    mm.Attachments.Add(attachment);

                    SmtpClient smtp = new SmtpClient();
                    smtp.Host = "smtp.gmail.com";
                    //smtp.Host = "smtp.gmail.com";
                    mm.ReplyToList.Add("acctsmfpune@dtdc.com");
                    smtp.EnableSsl = true;
                    System.Net.NetworkCredential credentials = new System.Net.NetworkCredential();
                    credentials.UserName = "billing@atkexp.com";
                    credentials.Password = "atk123##";
                    smtp.UseDefaultCredentials = true;
                    smtp.Credentials = credentials;
                    smtp.Port = 587;
                    smtp.Send(mm);
                }


            }


            string savePath = Server.MapPath("~/PDF/" + dataset4.FirstOrDefault().Company_Name + " " + dataset3.FirstOrDefault().invoiceno.Replace("/", "-") + ".pdf");

            using (FileStream stream = new FileStream(savePath, FileMode.Create))
            {
                stream.Write(renderByte, 0, renderByte.Length);
            }



        }

        [HttpGet]
        public string SavepdInvoice(string myParameter)
        {
            {

                LocalReport lr = new LocalReport();



                Invoice inc = db.Invoices.Where(m => m.invoiceno == myParameter).FirstOrDefault();

                string Pfcode = db.Companies.Where(m => m.Company_Id == inc.Customer_Id).Select(m => m.Pf_code).FirstOrDefault();

                //var dataset = db.TransactionViews.Where(m => m.Customer_Id == inc.Customer_Id && !db.singleinvoiceconsignments.Select(b=>b.Consignment_no).Contains(m.Consignment_no))
                //           .ToList().
                //           Where(x => DateTime.Compare(x.booking_date.Value.Date, inc.periodfrom.Value.Date) >= 0 && DateTime.Compare(x.booking_date.Value.Date, inc.periodto.Value.Date) <= 0).OrderBy(m => m.booking_date).ThenBy(n => n.Consignment_no)
                //      .ToList();

                var dataset = db.TransactionViews.Where(m => m.Customer_Id == inc.Customer_Id && !db.singleinvoiceconsignments.Select(b => b.Consignment_no).Contains(m.Consignment_no)).ToList().
            Where(x => DateTime.Compare(x.booking_date.Value.Date, inc.periodfrom.Value.Date) >= 0 && DateTime.Compare(x.booking_date.Value.Date, inc.periodto.Value.Date) <= 0).OrderBy(m => m.booking_date).ThenBy(n => n.Consignment_no)
                          .ToList();


                var dataset2 = db.Franchisees.Where(x => x.PF_Code == Pfcode);

                var dataset3 = db.Invoices.OrderByDescending(m => m.invoiceno == inc.invoiceno);

                var dataset4 = db.Companies.Where(m => m.Company_Id == inc.Customer_Id);

                /////////////////Total//////////////

                string clientGst = dataset4.FirstOrDefault().Gst_No;
                string frgst = dataset2.FirstOrDefault().GstNo;
                string discount = dataset3.FirstOrDefault().discount;
                if(discount == "no")
                { 
                if (clientGst != null && clientGst.Length > 4)
                {
                        if (inc.Customer_Id.ToUpper() == "TRESVISTA ANALYTICS LLP PUNE".ToUpper())
                        {
                            string path = Path.Combine(Server.MapPath("~/RdlcReport"), "PrintInvoiceIGST.rdlc");

                            if (System.IO.File.Exists(path))
                            {
                                lr.ReportPath = path;
                            }
                        }
                    else    if (frgst.Substring(0, 2) == clientGst.Substring(0, 2))
                    {
                        string path = Path.Combine(Server.MapPath("~/RdlcReport"), "PrintInvoice.rdlc");

                        if (System.IO.File.Exists(path))
                        {
                            lr.ReportPath = path;
                        }

                    }
                    else
                    {
                        string path = Path.Combine(Server.MapPath("~/RdlcReport"), "PrintInvoiceIGST.rdlc");

                        if (System.IO.File.Exists(path))
                        {
                            lr.ReportPath = path;
                        }
                    }
                }
                else
                {
                    string path = Path.Combine(Server.MapPath("~/RdlcReport"), "PrintInvoice.rdlc");

                    if (System.IO.File.Exists(path))
                    {
                        lr.ReportPath = path;
                    }
                }
            }

                else if (discount == "yes")
                {
                    //string path = Path.Combine(Server.MapPath("~/RdlcReport"), "DiscountInvoice.rdlc");

                    //if (System.IO.File.Exists(path))
                    //{
                    //    lr.ReportPath = path;
                    //}

                    if (clientGst != null && clientGst.Length > 4)
                    {
                        if (frgst.Substring(0, 2) == clientGst.Substring(0, 2))
                        {
                            string path = Path.Combine(Server.MapPath("~/RdlcReport"), "DiscountInvoice.rdlc");

                            if (System.IO.File.Exists(path))
                            {
                                lr.ReportPath = path;
                            }

                        }
                        else
                        {
                            string path = Path.Combine(Server.MapPath("~/RdlcReport"), "DiscountInvoiceIGST.rdlc");

                            if (System.IO.File.Exists(path))
                            {
                                lr.ReportPath = path;
                            }
                        }
                    }
                    else
                    {
                        string path = Path.Combine(Server.MapPath("~/RdlcReport"), "DiscountInvoice.rdlc");

                        if (System.IO.File.Exists(path))
                        {
                            lr.ReportPath = path;
                        }
                    }
                }


                ////////////////////////////////////
                ReportDataSource rd = new ReportDataSource("PrintInvoice", dataset);
                ReportDataSource rd1 = new ReportDataSource("franchisees", dataset2);
                ReportDataSource rd2 = new ReportDataSource("invoice", dataset3);
                ReportDataSource rd3 = new ReportDataSource("comp", dataset4);

                //  ReportParameter[] allPar = new ReportParameter[1]; // create parameters array
                //  ReportParameter parSum = new ReportParameter("Dcno", dcno);



                //  lr.SetParameters(new ReportParameter[] { parSum });

                lr.DataSources.Add(rd);
                lr.DataSources.Add(rd1);
                lr.DataSources.Add(rd2);
                lr.DataSources.Add(rd3);

                string reportType = "pdf";
                string mimeType;
                string encoding;
                string fileNameExte;

                string deviceInfo =
                    "<DeviceInfo>" +
                    "<OutputFormat>" + "pdf" + "</OutputFormat>" +
                    "<PageHeight>11in</PageHeight>" +
                   "<Margintop>0.1in</Margintop>" +
                     "<Marginleft>0.1in</Marginleft>" +
                      "<Marginright>0.1in</Marginright>" +
                       "<Marginbottom>0.5in</Marginbottom>" +
                       "</DeviceInfo>";

                Warning[] warnings;
                string[] streams;
                byte[] renderByte;


                renderByte = lr.Render
              (reportType,
              deviceInfo,
              out mimeType,
              out encoding,
              out fileNameExte,
              out streams,
              out warnings
              );


                
                string savePath = Server.MapPath("~/PDF/" + dataset4.FirstOrDefault().Company_Name.Replace("/", "-") + " " + dataset3.FirstOrDefault().invoiceno.Replace("/", "-") + ".pdf");

                using (FileStream stream = new FileStream(savePath, FileMode.Create))
                {
                    stream.Write(renderByte, 0, renderByte.Length);
                }

                return dataset4.FirstOrDefault().Company_Name + " " + dataset3.FirstOrDefault().invoiceno.Replace("/", "-") + ".pdf";

            }

        }

        [HttpGet]
        public string SavepdDpInvoice(string myParameter)
        {
            {

                LocalReport lr = new LocalReport();

                Invoice inc = db.Invoices.Where(m => m.invoiceno == myParameter).FirstOrDefault();


                string Pfcode = db.Companies.Where(m => m.Company_Id == inc.Customer_Id).Select(m => m.Pf_code).FirstOrDefault();
               
                var dataset = db.TransactionViews.Where(m => m.Customer_Id == inc.Customer_Id && !db.singleinvoiceconsignments.Select(b => b.Consignment_no).Contains(m.Consignment_no)).ToList().
                         Where(x => DateTime.Compare(x.booking_date.Value.Date, inc.periodfrom.Value.Date) >= 0 && DateTime.Compare(x.booking_date.Value.Date, inc.periodto.Value.Date) <= 0).OrderBy(m => m.booking_date).ThenBy(n => n.Consignment_no)
                                       .ToList();

                var dataset2 = db.Franchisees.Where(x => x.PF_Code == Pfcode);

                var dataset3 = db.Invoices.OrderByDescending(m => m.invoiceno == inc.invoiceno);

                var dataset4 = db.Companies.Where(m => m.Company_Id == inc.Customer_Id).ToList();


                /////////////////Total//////////////

                string clientGst = dataset4.FirstOrDefault().Gst_No;
                string frgst = dataset2.FirstOrDefault().GstNo;


                if (clientGst != null && clientGst.Length > 4)
                {
                    if (frgst.Substring(0, 2) == clientGst.Substring(0, 2))
                    {
                        string path = Path.Combine(Server.MapPath("~/RdlcReport"), "DpPrintInvoice.rdlc");

                        if (System.IO.File.Exists(path))
                        {
                            lr.ReportPath = path;
                        }

                    }
                    else
                    {
                        string path = Path.Combine(Server.MapPath("~/RdlcReport"), "DpPrintInvoiceIGST.rdlc");

                        if (System.IO.File.Exists(path))
                        {
                            lr.ReportPath = path;
                        }
                    }
                }
                else
                {
                    string path = Path.Combine(Server.MapPath("~/RdlcReport"), "DpPrintInvoice.rdlc");

                    if (System.IO.File.Exists(path))
                    {
                        lr.ReportPath = path;
                    }
                }

                ////////////////////////////////////
                ReportDataSource rd = new ReportDataSource("PrintInvoice", dataset);
                ReportDataSource rd1 = new ReportDataSource("franchisees", dataset2);
                ReportDataSource rd2 = new ReportDataSource("invoice", dataset3);
                ReportDataSource rd3 = new ReportDataSource("comp", dataset4);

                //  ReportParameter[] allPar = new ReportParameter[1]; // create parameters array
                //  ReportParameter parSum = new ReportParameter("Dcno", dcno);

                //  lr.SetParameters(new ReportParameter[] { parSum });

                lr.DataSources.Add(rd);
                lr.DataSources.Add(rd1);
                lr.DataSources.Add(rd2);
                lr.DataSources.Add(rd3);

                string reportType = "pdf";
                string mimeType;
                string encoding;
                string fileNameExte;

                string deviceInfo =
                    "<DeviceInfo>" +
                    "<OutputFormat>" + "pdf" + "</OutputFormat>" +
                    "<PageHeight>11in</PageHeight>" +
                   "<Margintop>0.1in</Margintop>" +
                     "<Marginleft>0.1in</Marginleft>" +
                      "<Marginright>0.1in</Marginright>" +
                       "<Marginbottom>0.5in</Marginbottom>" +
                       "</DeviceInfo>";

                Warning[] warnings;
                string[] streams;
                byte[] renderByte;


                renderByte = lr.Render
              (reportType,
              deviceInfo,
              out mimeType,
              out encoding,
              out fileNameExte,
              out streams,
              out warnings
              );

                string savePath = Server.MapPath("~/PDF/" + dataset4.FirstOrDefault().Company_Name + " " + dataset3.FirstOrDefault().invoiceno.Replace("/", "-") + ".pdf");

                using (FileStream stream = new FileStream(savePath, FileMode.Create))
                {
                    stream.Write(renderByte, 0, renderByte.Length);
                }
                return dataset4.FirstOrDefault().Company_Name + " " + dataset3.FirstOrDefault().invoiceno.Replace("/", "-") + ".pdf";
            }
        }

        [HttpGet]
        public ActionResult InvoiceZip()
        {

            return View();
        }

        [HttpPost]
        public ActionResult InvoiceZip(int frominv, int toinv)
        {
            string fileType = "application/octet-stream";
            var outputStream = new MemoryStream();
            invstart = "ATK/24-25/";
            using (ZipFile zipFile = new ZipFile())
            {
                for (int i = frominv; i <= toinv; i++)
                {
                    string companyid = db.Invoices.Where(m => m.invoiceno == invstart + i).Select(m => m.Customer_Id).FirstOrDefault();
                    string companyname = db.Companies.Where(m => m.Company_Id == companyid).Select(m => m.Company_Name).FirstOrDefault();

                    string filePath = Server.MapPath("/PDF/" + companyname + " " + "ATK-24-25-" + i + ".pdf");

                    if (System.IO.File.Exists(filePath))
                    {
                        zipFile.AddFile(filePath, "Invoices");
                    }
                }

                Response.ClearContent();
                Response.ClearHeaders();

                //Set zip file name
                Response.AppendHeader("content-disposition", "attachment; filename=Invoices.zip");

                //Save the zip content in output stream
                zipFile.Save(outputStream);
            }

            //Set the cursor to start position
            outputStream.Position = 0;

            //Dispance the stream
            return new FileStreamResult(outputStream, fileType);
        }

        public ActionResult GenerateInvoiceSingle(string Invoiceno = null)
        {
            Invoice inv = db.Invoices.Where(m => m.invoiceno == Invoiceno).FirstOrDefault();
            ViewBag.Gst = "18";
            if (Invoiceno != null)
             {
                       ViewBag.consignmnts = string.Join(",", dc.singleinvoiceconsignments.Where(m => m.Invoice_no == Invoiceno).Select(m => m.Consignment_no).ToArray());
             }
                if (inv != null)
            {
                ViewBag.invno = inv.invoiceno;
            }
            return View(inv);

        }

        //public ActionResult GenerateInvoiceSingle(string Invoiceno = null)
        //{

        //    string lastInvoiceno = db.Invoices.Where(m => m.invoiceno.StartsWith(invstart)).OrderByDescending(m => m.IN_Id).Take(1).Select(m => m.invoiceno).FirstOrDefault() ?? invstart + 0;
        //    int number = Convert.ToInt32(lastInvoiceno.Substring(10));

        //    Invoice inv = db.Invoices.Where(m => m.invoiceno == Invoiceno).FirstOrDefault();

        //    if(Invoiceno != null)
        //    {
        //        ViewBag.consignmnts = string.Join(",", dc.singleinvoiceconsignments.Where(m => m.Invoice_no == Invoiceno).Select(m => m.Consignment_no).ToArray());
        //    }
        //    ViewBag.lastInvoiceno = invstart +""+ (number + 1);

        //    return View(inv);
        //}

        [HttpPost]
        public ActionResult SaveSingleInvoice(Invoice invoice, string submit,string consignments,string Remark)
        {

            ViewBag.consignmnts = consignments;

            if (invoice.discount == "yes")
            {
                ViewBag.disc = invoice.discount;
            }
            if (Remark == "")
            {
                ViewBag.success = "please Enter Remark";
                return PartialView("GenerateInvoiceSinglePartial", invoice);
            }
            else
            {

                if (ModelState.IsValid)
                {

                    string[] formats = { "dd/MM/yyyy", "dd-MMM-yyyy", "yyyy-MM-dd", "dd-MM-yyyy", "M/d/yyyy", "dd MMM yyyy" };

                    Invoice inv = db.Invoices.Where(m => m.invoiceno == invoice.invoiceno).FirstOrDefault();

                    if (inv != null)
                    {
                        string bdatefrom = DateTime.ParseExact(invoice.Tempdatefrom, formats, CultureInfo.InvariantCulture, DateTimeStyles.None).ToString("MM/dd/yyyy");
                        string bdateto = DateTime.ParseExact(invoice.TempdateTo, formats, CultureInfo.InvariantCulture, DateTimeStyles.None).ToString("MM/dd/yyyy");

                        string invdate = DateTime.ParseExact(invoice.tempInvoicedate, formats, CultureInfo.InvariantCulture, DateTimeStyles.None).ToString("MM/dd/yyyy");
                        invoice.periodfrom = Convert.ToDateTime(bdatefrom);
                        invoice.periodto = Convert.ToDateTime(bdateto);
                        invoice.invoicedate = Convert.ToDateTime(invdate);

                        double netAmt = Convert.ToDouble(inv.netamount);

                        //ViewBag.nextinvoice = GetmaxInvoiceno(invstart);
                        invoice.IN_Id = inv.IN_Id;
                        invoice.invoiceno = invoice.invoiceno;
                        invoice.Invoice_Lable = AmountTowords.changeToWords(invoice.netamount.ToString());
                        db.Entry(inv).State = EntityState.Detached;
                        db.Entry(invoice).State = EntityState.Modified;
                        db.SaveChanges();

                        //----------------- For InvoiceRemark Save Date Added on 08/09/2022------------------------------

                        InvoiceRemark invremark = new InvoiceRemark();

                        invremark.InvoiceNo = invoice.invoiceno;
                        invremark.Remark = Remark;
                        invremark.NetAmount = netAmt;
                        invremark.UpdatedNetAmount = invoice.netamount;

                        db.InvoiceRemarks.Add(invremark);
                        db.SaveChanges();

                        //-----------------------------------------------------------

                        ViewBag.success = "Invoice Added SuccessFully";
                    }
                    else
                    {
                        string bdatefrom = DateTime.ParseExact(invoice.Tempdatefrom, formats, CultureInfo.InvariantCulture, DateTimeStyles.None).ToString("MM/dd/yyyy");
                        string bdateto = DateTime.ParseExact(invoice.TempdateTo, formats, CultureInfo.InvariantCulture, DateTimeStyles.None).ToString("MM/dd/yyyy");

                        string invdate = DateTime.ParseExact(invoice.tempInvoicedate, formats, CultureInfo.InvariantCulture, DateTimeStyles.None).ToString("MM/dd/yyyy");

                        invoice.periodfrom = Convert.ToDateTime(bdatefrom);
                        invoice.periodto = Convert.ToDateTime(bdateto);
                        invoice.invoicedate = Convert.ToDateTime(invdate);

                        invoice.Invoice_Lable = AmountTowords.changeToWords(invoice.netamount.ToString());
                    
                        //ViewBag.nextinvoice = GetmaxInvoiceno(invstart);

                        invoice.invoiceno = invoice.invoiceno;

                        db.Invoices.Add(invoice);
                        db.SaveChanges();

                        ViewBag.success = "Invoice Added SuccessFully";

                    }

                    string[] cons = consignments.Split(',');

                    foreach (var i in cons)
                    {
                        singleinvoiceconsignment upsc = dc.singleinvoiceconsignments.Where(m => m.Consignment_no == i).FirstOrDefault();

                        if (upsc == null)
                        {
                            singleinvoiceconsignment sc = new singleinvoiceconsignment();

                            sc.Consignment_no = i.Trim();
                            sc.Invoice_no = invoice.invoiceno;
                            dc.singleinvoiceconsignments.Add(sc);
                            dc.SaveChanges();

                        }
                    }

                    string Pfcode = db.Companies.Where(m => m.Company_Id == invoice.Customer_Id).Select(m => m.Pf_code).FirstOrDefault(); /// take dynamically


                    LocalReport lr = new LocalReport();
                    List<TransactionView> dataset = new List<TransactionView>();

                    var consigmfromsingle = dc.singleinvoiceconsignments.Where(m => m.Invoice_no == invoice.invoiceno);

                    foreach (var c in consigmfromsingle)
                    {
                        TransactionView temp = db.TransactionViews.Where(m => m.Consignment_no == c.Consignment_no).FirstOrDefault();
                        dataset.Add(temp);
                    }

                    var dataset2 = db.Franchisees.Where(x => x.PF_Code == Pfcode);

                    var dataset3 = db.Invoices.OrderByDescending(m => m.invoiceno == invoice.invoiceno);

                    var dataset4 = db.Companies.Where(m => m.Company_Id == invoice.Customer_Id);

                    string clientGst = dataset4.FirstOrDefault().Gst_No;
                    string frgst = dataset2.FirstOrDefault().GstNo;


                    if (clientGst != null && clientGst.Length > 4)
                    {
                        if (frgst.Substring(0, 2) == clientGst.Substring(0, 2))
                        {
                            string path = Path.Combine(Server.MapPath("~/RdlcReport"), "PrintInvoice.rdlc");

                            if (System.IO.File.Exists(path))
                            {
                                lr.ReportPath = path;
                            }

                        }
                        else
                        {
                            string path = Path.Combine(Server.MapPath("~/RdlcReport"), "PrintInvoiceIGST.rdlc");

                            if (System.IO.File.Exists(path))
                            {
                                lr.ReportPath = path;
                            }
                        }
                    }
                    else
                    {
                        string path = Path.Combine(Server.MapPath("~/RdlcReport"), "PrintInvoice.rdlc");

                        if (System.IO.File.Exists(path))
                        {
                            lr.ReportPath = path;
                        }
                    }

                    ReportDataSource rd = new ReportDataSource("PrintInvoice", dataset);
                    ReportDataSource rd1 = new ReportDataSource("franchisees", dataset2);
                    ReportDataSource rd2 = new ReportDataSource("invoice", dataset3);
                    ReportDataSource rd3 = new ReportDataSource("comp", dataset4);

                    lr.DataSources.Add(rd);
                    lr.DataSources.Add(rd1);
                    lr.DataSources.Add(rd2);
                    lr.DataSources.Add(rd3);

                    string reportType = "pdf";
                    string mimeType;
                    string encoding;
                    string fileNameExte;

                    string deviceInfo =
                        "<DeviceInfo>" +
                        "<OutputFormat>" + "pdf" + "</OutputFormat>" +
                        "<PageHeight>11in</PageHeight>" +
                       "<Margintop>0.1in</Margintop>" +
                         "<Marginleft>0.1in</Marginleft>" +
                          "<Marginright>0.1in</Marginright>" +
                           "<Marginbottom>0.5in</Marginbottom>" +
                           "</DeviceInfo>";

                    Warning[] warnings;
                    string[] streams;
                    byte[] renderByte;


                    renderByte = lr.Render
                  (reportType,
                  deviceInfo,
                  out mimeType,
                  out encoding,
                  out fileNameExte,
                  out streams,
                  out warnings
                  );


                    ViewBag.pdf = false;

                    //if (submit == "Generate")
                    //{
                    ViewBag.pdf = true;
                    ViewBag.invoiceno = invoice.invoiceno;
                    //}
                    if (submit == "Email")
                    {

                        MemoryStream memoryStream = new MemoryStream(renderByte);

                        var emailcontent = db.EmailPromotions.Where(a => a.message == "True").FirstOrDefault();
                        using (MailMessage mm = new MailMessage("billing@atkexp.com", dataset4.FirstOrDefault().Email))
                        {
                            mm.Subject = "Invoice";
                            string Bodytext = "";

                            if (emailcontent != null)
                            {
                                Bodytext = "<html><body>" + emailcontent.note + "</body></html>";
                            }
                            else
                            {
                                Bodytext = "<html><body>Please Find Attachment</body></html>";
                            }
                            // mm.Subject = "Invoice";

                            //string Bodytext = "<html><body>Please Find Attachment</body></html>";
                            Attachment attachment = new Attachment(memoryStream, "Invoice.pdf");

                            mm.IsBodyHtml = true;

                            mm.BodyEncoding = System.Text.Encoding.GetEncoding("utf-8");

                            AlternateView plainView = System.Net.Mail.AlternateView.CreateAlternateViewFromString(System.Text.RegularExpressions.Regex.Replace(Bodytext, @"<(.|\n)*?>", string.Empty), null, "text/plain");
                            // mm.Body = Bodytext;
                            mm.Body = Bodytext;

                            //Add Byte array as Attachment.

                            mm.Attachments.Add(attachment);

                            SmtpClient smtp = new SmtpClient();
                            smtp.Host = "smtp.gmail.com";
                            //smtp.Host = "smtp.gmail.com";
                            mm.ReplyToList.Add("acctsmfpune@dtdc.com");
                            smtp.EnableSsl = true;
                            System.Net.NetworkCredential credentials = new System.Net.NetworkCredential();
                            credentials.UserName = "billing@atkexp.com";
                            credentials.Password = "atk123##";
                            smtp.UseDefaultCredentials = true;
                            smtp.Credentials = credentials;
                            smtp.Port = 587;
                            smtp.Send(mm);
                        }

                    }
                    return PartialView("GenerateInvoiceSinglePartial", invoice);

                }
            }
            return PartialView("GenerateInvoiceSinglePartial", invoice);
        }

        public JsonResult InvoiceTableSingle(string [] array,string Customerid)
        {
            List<Transaction> Companies = new List<Transaction>();
            db.Configuration.ProxyCreationEnabled = false;

            if (array != null)
            {
                var temp = array.Distinct().ToArray();

                foreach (var i in temp)
                {
                    Transaction tr = db.Transactions.Where(m => m.Consignment_no == i.Trim() && m.Customer_Id == Customerid).FirstOrDefault();
                    if (tr != null)
                    {
                        Companies.Add(tr);
                    }

                }
            }
            return Json(Companies, JsonRequestBehavior.AllowGet);
        }


        [HttpGet]
        public ActionResult ReportsinglePrinterMethod(string myParameter) //on view call thise method
        {
            {

                LocalReport lr = new LocalReport();
                Invoice inc = db.Invoices.Where(m => m.invoiceno == myParameter).FirstOrDefault();

                string Pfcode = db.Companies.Where(m => m.Company_Id == inc.Customer_Id).Select(m => m.Pf_code).FirstOrDefault();
                List<TransactionView> dataset = new List<TransactionView>();

                var consigmfromsingle = dc.singleinvoiceconsignments.Where(m => m.Invoice_no == myParameter);

                foreach (var c in consigmfromsingle)
                {
                    TransactionView temp = db.TransactionViews.Where(m => m.Consignment_no == c.Consignment_no).FirstOrDefault();
                    dataset.Add(temp);
                }

                var dataset2 = db.Franchisees.Where(x => x.PF_Code == Pfcode);

                var dataset3 = db.Invoices.OrderByDescending(m => m.invoiceno == inc.invoiceno);

                var dataset4 = db.Companies.Where(m => m.Company_Id == inc.Customer_Id);


                /////////////////Total//////////////

                /////////////////Total//////////////

                string clientGst = dataset4.FirstOrDefault().Gst_No;
                string frgst = dataset2.FirstOrDefault().GstNo;


                if (clientGst != null && clientGst.Length > 4)
                {
                    if (frgst.Substring(0, 2) == clientGst.Substring(0, 2))
                    {
                        string path = Path.Combine(Server.MapPath("~/RdlcReport"), "PrintInvoice.rdlc");

                        if (System.IO.File.Exists(path))
                        {
                            lr.ReportPath = path;
                        }

                    }
                    else
                    {
                        string path = Path.Combine(Server.MapPath("~/RdlcReport"), "PrintInvoiceIGST.rdlc");

                        if (System.IO.File.Exists(path))
                        {
                            lr.ReportPath = path;
                        }
                    }
                }
                else
                {
                    string path = Path.Combine(Server.MapPath("~/RdlcReport"), "PrintInvoice.rdlc");

                    if (System.IO.File.Exists(path))
                    {
                        lr.ReportPath = path;
                    }
                }

                ////////////////////////////////////
                ReportDataSource rd = new ReportDataSource("PrintInvoice", dataset);
                ReportDataSource rd1 = new ReportDataSource("franchisees", dataset2);
                ReportDataSource rd2 = new ReportDataSource("invoice", dataset3);
                ReportDataSource rd3 = new ReportDataSource("comp", dataset4);

                //  ReportParameter[] allPar = new ReportParameter[1]; // create parameters array
                //  ReportParameter parSum = new ReportParameter("Dcno", dcno);
                //  lr.SetParameters(new ReportParameter[] { parSum });

                lr.DataSources.Add(rd);
                lr.DataSources.Add(rd1);
                lr.DataSources.Add(rd2);
                lr.DataSources.Add(rd3);

                string reportType = "pdf";
                string mimeType;
                string encoding;
                string fileNameExte;

                string deviceInfo =
                    "<DeviceInfo>" +
                    "<OutputFormat>" + "pdf" + "</OutputFormat>" +
                    "<PageHeight>11in</PageHeight>" +
                   "<Margintop>0.1in</Margintop>" +
                     "<Marginleft>0.1in</Marginleft>" +
                      "<Marginright>0.1in</Marginright>" +
                       "<Marginbottom>0.5in</Marginbottom>" +
                       "</DeviceInfo>";

                Warning[] warnings;
                string[] streams;
                byte[] renderByte;


                renderByte = lr.Render
              (reportType,
              deviceInfo,
              out mimeType,
              out encoding,
              out fileNameExte,
              out streams,
              out warnings
              );

                return File(renderByte, mimeType);
            }

        }


        [HttpGet]
        public string SavesinglepdInvoice(string myParameter)
        {
            {
                LocalReport lr = new LocalReport();
                Invoice inc = db.Invoices.Where(m => m.invoiceno == myParameter).FirstOrDefault();

                string Pfcode = db.Companies.Where(m => m.Company_Id == inc.Customer_Id).Select(m => m.Pf_code).FirstOrDefault();


                List<TransactionView> dataset = new List<TransactionView>();

                var consigmfromsingle = dc.singleinvoiceconsignments.Where(m => m.Invoice_no == myParameter);

                foreach (var c in consigmfromsingle)
                {
                    TransactionView temp = db.TransactionViews.Where(m => m.Consignment_no == c.Consignment_no).FirstOrDefault();
                    dataset.Add(temp);
                }
                var dataset2 = db.Franchisees.Where(x => x.PF_Code == Pfcode);

                var dataset3 = db.Invoices.OrderByDescending(m => m.invoiceno == inc.invoiceno);

                var dataset4 = db.Companies.Where(m => m.Company_Id == inc.Customer_Id);


                /////////////////Total//////////////
                string clientGst = dataset4.FirstOrDefault().Gst_No;
                string frgst = dataset2.FirstOrDefault().GstNo;


                if (clientGst != null && clientGst.Length > 4)
                {
                    if (frgst.Substring(0, 2) == clientGst.Substring(0, 2))
                    {
                        string path = Path.Combine(Server.MapPath("~/RdlcReport"), "PrintInvoice.rdlc");

                        if (System.IO.File.Exists(path))
                        {
                            lr.ReportPath = path;
                        }

                    }
                    else
                    {
                        string path = Path.Combine(Server.MapPath("~/RdlcReport"), "PrintInvoiceIGST.rdlc");

                        if (System.IO.File.Exists(path))
                        {
                            lr.ReportPath = path;
                        }
                    }
                }
                else
                {
                    string path = Path.Combine(Server.MapPath("~/RdlcReport"), "PrintInvoice.rdlc");

                    if (System.IO.File.Exists(path))
                    {
                        lr.ReportPath = path;
                    }
                }
                ////////////////////////////////////
                ReportDataSource rd = new ReportDataSource("PrintInvoice", dataset);
                ReportDataSource rd1 = new ReportDataSource("franchisees", dataset2);
                ReportDataSource rd2 = new ReportDataSource("invoice", dataset3);
                ReportDataSource rd3 = new ReportDataSource("comp", dataset4);

                //  ReportParameter[] allPar = new ReportParameter[1]; // create parameters array
                //  ReportParameter parSum = new ReportParameter("Dcno", dcno);



                //  lr.SetParameters(new ReportParameter[] { parSum });

                lr.DataSources.Add(rd);
                lr.DataSources.Add(rd1);
                lr.DataSources.Add(rd2);
                lr.DataSources.Add(rd3);

                string reportType = "pdf";
                string mimeType;
                string encoding;
                string fileNameExte;

                string deviceInfo =
                    "<DeviceInfo>" +
                    "<OutputFormat>" + "pdf" + "</OutputFormat>" +
                    "<PageHeight>11in</PageHeight>" +
                   "<Margintop>0.1in</Margintop>" +
                     "<Marginleft>0.1in</Marginleft>" +
                      "<Marginright>0.1in</Marginright>" +
                       "<Marginbottom>0.5in</Marginbottom>" +
                       "</DeviceInfo>";

                Warning[] warnings;
                string[] streams;
                byte[] renderByte;


                renderByte = lr.Render
              (reportType,
              deviceInfo,
              out mimeType,
              out encoding,
              out fileNameExte,
              out streams,
              out warnings
              );
                string savePath = Server.MapPath("~/PDF/" + dataset4.FirstOrDefault().Company_Name + " " + dataset3.FirstOrDefault().invoiceno.Replace("/", "-") + ".pdf");
                using (FileStream stream = new FileStream(savePath, FileMode.Create))
                {
                    stream.Write(renderByte, 0, renderByte.Length);
                }
                return dataset4.FirstOrDefault().Company_Name + " " + dataset3.FirstOrDefault().invoiceno.Replace("/", "-") + ".pdf";
            }
        }

        public ActionResult GenerateInvoiceLastYear(string Invoiceno = null)
        {
            string invstart = "ATK/22-23/";
            string lastInvoiceno = db.Invoices.Where(m => m.invoiceno.StartsWith(invstart)).OrderByDescending(m => m.IN_Id).Take(1).Select(m => m.invoiceno).FirstOrDefault() ?? invstart + 0;
            int number = Convert.ToInt32(lastInvoiceno.Substring(10));
            Invoice inv = db.Invoices.Where(m => m.invoiceno == Invoiceno).FirstOrDefault();
            ViewBag.lastInvoiceno = number + 1;
            return View(inv);
        }

        public ActionResult GetmaxInvoiceno(string invstart)
        {
           
            string lastInvoiceno = db.Invoices.Where(m => m.invoiceno.StartsWith(invstart)).OrderByDescending(m => m.IN_Id).Take(1).Select(m => m.invoiceno).FirstOrDefault() ?? invstart + 0;
            int number = Convert.ToInt32(lastInvoiceno.Substring(10));
            ViewBag.lastInvoiceno = invstart +""+ (number + 1);
                return View();
        }

        public ActionResult Download(long id)
        {
            var invoice = db.Invoices.Where(m => m.IN_Id == id).FirstOrDefault();
            string companyname = db.Companies.Where(m => m.Company_Id == invoice.Customer_Id).Select(m=>m.Company_Name).FirstOrDefault().ToString();
            string savePath = "http://admin.atkxpress.com/PDF/" + companyname.Replace('/','-') + " " + invoice.invoiceno.Replace("/", "-") + ".pdf";
            return Redirect(savePath);         
        }
        public ActionResult DownloadAckcopy(long id)
        {
            var invoice = db.Invoices.Where(m => m.IN_Id == id).FirstOrDefault();
           string savePath = "http://admin.atkxpress.com/UploadedAckInvoiceCopy/" + invoice.Filepath;
            return Redirect(savePath);
        }
        public ActionResult ViewInvoiceRemark(string Invoiceno)
        {
           
           List<InvRemarkModel> invRemark = new List<InvRemarkModel>();
            invRemark = (from d in db.InvoiceRemarks
                         where d.InvoiceNo == Invoiceno
                         select new InvRemarkModel{
                         InvoiceNo=d.InvoiceNo,
                         Remark=d.Remark,
                         NetAmount=d.NetAmount.ToString(),
                         AfterUpdatedNetAmount=d.UpdatedNetAmount.ToString()
                         }).ToList();
            

            return View("ViewInvoiceRemark", invRemark);
        }

        public ActionResult ViewInvoiceDPRemark(string Invoiceno)
        {

            List<InvRemarkModel> invRemark = new List<InvRemarkModel>();
            invRemark = (from d in db.InvoiceRemarks
                         where d.InvoiceNo == Invoiceno
                         select new InvRemarkModel
                         {
                             InvoiceNo = d.InvoiceNo,
                             Remark = d.Remark,
                             NetAmount = d.NetAmount.ToString(),
                             AfterUpdatedNetAmount = d.UpdatedNetAmount.ToString()
                         }).ToList();


            return View("ViewInvoiceDPRemark", invRemark);
        }

        public ActionResult ViewSingleInvoiceRemark(string Invoiceno)
        {

            List<InvRemarkModel> invRemark = new List<InvRemarkModel>();
            invRemark = (from d in db.InvoiceRemarks
                         where d.InvoiceNo == Invoiceno
                         select new InvRemarkModel
                         {
                             InvoiceNo = d.InvoiceNo,
                             Remark = d.Remark,
                             NetAmount = d.NetAmount.ToString(),
                             AfterUpdatedNetAmount = d.UpdatedNetAmount.ToString()
                         }).ToList();


            return View("ViewSingleInvoiceRemark", invRemark);
        }


        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult AddAcknoledgmentPartial(AddInvoiceAckModel inv)
        //{
        //    inv.companyid = "";
        //    return Json(new { RedirectUrl = Url.Action("ViewInvoice", new { invfromdate=inv.invfromdate, Companydetails=inv.companyid, invtodate=inv.invtodate, size=inv.size, pageNo=1 })});
        //    //return (ViewInvoice(inv.invfromdate, inv.companyid = "", inv.invtodate, inv.size, 1));

        //   //return Json(true);
        //}


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddAcknoledgment(AddInvoiceAckModel inv)
        {
            ViewBag.PfCode = new SelectList(db.Franchisees.Where(x => !x.PF_Code.ToUpper().StartsWith("PR")), "PF_Code", "PF_Code");

           
                var r = new Regex(@"([a-zA-Z0-9\s_\\.\-:])+(.pdf)$");

                if (ModelState.IsValid)
                {

                if (!r.IsMatch(inv.file.FileName))
                {
                   
                    ModelState.AddModelError("Invfile", "Only PDF files allowed");

                }
                else
                {
                    string _FileName = "";
                    string _path = "";

                    if (inv.file.ContentLength > 0)
                    {
                        _FileName = Path.GetFileName(inv.file.FileName);
                        _path = Server.MapPath("~/UploadedAckInvoiceCopy/") + _FileName;
                        inv.file.SaveAs(_path);
                    }
                    var lo = (from d in db.Invoices
                              where d.invoiceno == inv.Invoiceno
                              select d).FirstOrDefault();


                    lo.Filepath = _FileName;

                    db.Entry(lo).State = EntityState.Modified;
                    db.SaveChanges();
                    TempData["Success1"] = "File Uploaded Successfully!";

                    ViewBag.invfromdate = inv.invfromdate;
                    ViewBag.invtodate = inv.invtodate;

                    //return Json(new { RedirectUrl = Url.Action("ViewInvoice")});
                    return PartialView("AddAcknoledgmentPartial",inv);

                   // return ViewInvoice(inv.invfromdate, "", inv.invtodate, inv.size,1);
                }
                   
                }
                else
                {
                    ModelState.AddModelError("Invfile1", "Please Select File.");
                }

            //return Json(new { success = false, responseText = "The attached file is not supported." }, JsonRequestBehavior.AllowGet);
            //return Json(new { success = false, message = "Please select a file !" }, JsonRequestBehavior.AllowGet);
            return PartialView("AddAcknoledgmentPartial", inv);
            // return RedirectToAction("ViewInvoice", new { invfromdate = invfromdate, Companydetails = companyid, invtodate = invtodate, size = size, pageNo = pageNo });

        }




    }
}
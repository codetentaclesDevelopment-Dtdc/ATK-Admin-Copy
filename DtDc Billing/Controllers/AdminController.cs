using DtDc_Billing.Entity_FR;
using DtDc_Billing.Models;

using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Data.SqlClient;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

using Microsoft.SqlServer.Management.Common;
using System.Configuration;
using PagedList;
using System.Net.Mail;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Math;

namespace DtDc_Billing.Controllers
{
    public class AdminController : Controller
    {
        private DB_A43B74_wingsgrowdbEntities db = new DB_A43B74_wingsgrowdbEntities();
        // GET: Admin
        public ActionResult AdminLogin(string ReturnUrl)
        {
            ViewBag.ReturnUrl = ReturnUrl;
            return View();
        }

        [HttpPost]
        public ActionResult AdminLogin(AdminLogin login, string ReturnUrl)
        {

            var obj = db.Admins.Where(a => a.Username.Equals(login.UserName) && a.A_Password.Equals(login.Password)).FirstOrDefault();

            if (obj != null)
            {
                Session["Admin"] = obj.A_Id.ToString();
                Session["UserName"] = obj.Username.ToString();

                //HttpCookie cookie = new HttpCookie("AdminValue", Session["Admin"].ToString());
                //cookie.Expires = DateTime.Now.AddDays(30);
                //Response.Cookies.Add(cookie);


                HttpCookie cookie = new HttpCookie("Cookies");
                cookie["AdminValue"] = obj.A_Id.ToString();

                cookie["UserValue"] = obj.Username.ToString();
                cookie.Expires = DateTime.Now.AddDays(1);
                Response.Cookies.Add(cookie);

                string decodedUrl = "";
                if (!string.IsNullOrEmpty(ReturnUrl))
                    decodedUrl = Server.UrlDecode(ReturnUrl);

                //Login logic...

                if (Url.IsLocalUrl(decodedUrl))
                {
                    return Redirect(decodedUrl);
                }
                else
                {
                    return RedirectToAction("Index", "Home");
                }


            }
            else
            {
                ModelState.AddModelError("LoginAuth", "Username or Password Is Incorrect");
            }
            return View();
        }


        //[HttpPost]
        //public ActionResult importFromExceldestination(HttpPostedFileBase httpPostedFileBase)
        //{
        //    if (httpPostedFileBase != null)
        //    {
        //        HttpPostedFileBase file = httpPostedFileBase;
        //        if ((file != null) && (file.ContentLength > 0) && !string.IsNullOrEmpty(file.FileName))
        //        {
        //            string fileName = file.FileName;
        //            string fileContentType = file.ContentType;
        //            byte[] fileBytes = new byte[file.ContentLength];
        //            var data = file.InputStream.Read(fileBytes, 0, Convert.ToInt32(file.ContentLength));



        //            using (var package = new ExcelPackage(file.InputStream))
        //            {
        //                var currentSheet = package.Workbook.Worksheets;
        //                var workSheet = currentSheet.First();
        //                var noOfCol = workSheet.Dimension.End.Column;
        //                var noOfRow = workSheet.Dimension.End.Row;
        //                for (int rowIterator = 2; rowIterator <= noOfRow; rowIterator++)
        //                {
        //                    var tran = new Transaction();

        //                    tran.Pincode = workSheet.Cells[rowIterator, 1].Value.ToString().Trim();

        //                    Destination transaction = db.Destinations.Where(m => m.Pincode == tran.Pincode).FirstOrDefault();

        //                    if (transaction != null)
        //                    {

        //                         transaction.ZONE = workSheet.Cells[rowIterator, 5].Value.ToString().Trim();
        //                        transaction.LANE = workSheet.Cells[rowIterator, 6].Value.ToString().Trim();

        //                        db.Entry(transaction).State = EntityState.Modified;
        //                        db.SaveChanges();

        //                    }



        //                }
        //            }

        //            ViewBag.Success = "Excel File Uploaded SuccessFully";
        //        }
        //    }


        //    return View();
        //}



        public async Task InsertGECDataAsync(HttpPostedFileBase httpPostedFileBase)
        {
            if (httpPostedFileBase != null)
            {
                HttpPostedFileBase file = httpPostedFileBase;
                if ((file != null) && (file.ContentLength > 0) && !string.IsNullOrEmpty(file.FileName))
                {
                    string fileName = file.FileName;
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

                            var pincode = workSheet.Cells[rowIterator, 1].Value.ToString().Trim();
                            var city = workSheet.Cells[rowIterator, 2].Value.ToString();
                            var sectorName = workSheet.Cells[rowIterator, 3].Value.ToString();
                            GECSector addnew = new GECSector();

                            addnew.Pincode = pincode;
                            addnew.City = city;
                            addnew.SectorName = sectorName;

                            db.GECSectors.Add(addnew);

                            db.SaveChanges();


                        }
                    }

                }
            }
        }

        [HttpGet]
        public ActionResult GECimportFromExcel()
        {
            return View();
        }

        [HttpPost]
        public ActionResult GECimportFromExcel(HttpPostedFileBase httpPostedFileBase)
        {

            var task = Task.Run(async () => await InsertGECDataAsync(httpPostedFileBase));
            return View();
        }


        [HttpGet]
        public ActionResult importFromExcel()
        {
            return View();
        }

        [HttpPost]
        public ActionResult importFromExcel(HttpPostedFileBase httpPostedFileBase)
        {
            if (httpPostedFileBase != null)
            {
                HttpPostedFileBase file = httpPostedFileBase;
                if ((file != null) && (file.ContentLength > 0) && !string.IsNullOrEmpty(file.FileName))
                {
                    string fileName = file.FileName;
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
                            var tran = new Transaction();

                            tran.Consignment_no = workSheet.Cells[rowIterator, 2].Value.ToString().Trim();
                            tran.Customer_Id = workSheet.Cells[rowIterator, 3].Value.ToString();



                            Transaction transaction = db.Transactions.Where(m => m.Consignment_no == tran.Consignment_no).FirstOrDefault();

                            if (transaction != null)
                            {

                                CalculateAmount ca = new CalculateAmount();

                                double? amt = ca.CalulateAmt(transaction.Consignment_no, tran.Customer_Id, transaction.Pincode, transaction.Mode, Convert.ToDouble(transaction.chargable_weight), transaction.Type_t);

                                transaction.Amount = amt;
                                transaction.Customer_Id = tran.Customer_Id;

                                transaction.Pf_Code = db.Companies.Where(m => m.Company_Id == transaction.Customer_Id).Select(m => m.Pf_code).FirstOrDefault();
                                transaction.AdminEmp = 000;

                                db.Entry(transaction).State = EntityState.Modified;
                                db.SaveChanges();
                            }

                        }
                    }

                    ViewBag.Success = "Excel File Uploaded SuccessFully";
                }
            }


            return View();
        }



        [HttpGet]
        public ActionResult importFromExcelWhole()
        {
            return View();
        }

        [HttpPost]
        public ActionResult importFromExcelWhole(HttpPostedFileBase httpPostedFileBase)
        {
            if (httpPostedFileBase != null)
            {
                HttpPostedFileBase file = httpPostedFileBase;
                if ((file != null) && (file.ContentLength > 0) && !string.IsNullOrEmpty(file.FileName))
                {
                    string fileName = file.FileName;
                    string fileContentType = file.ContentType;
                    byte[] fileBytes = new byte[file.ContentLength];
                    var data = file.InputStream.Read(fileBytes, 0, Convert.ToInt32(file.ContentLength));

                    string[] formats = {"dd/MM/yyyy", "dd-MMM-yyyy", "yyyy-MM-dd",
                   "dd-MM-yyyy", "M/d/yyyy","d/M/yyyy", "dd MMM yyyy"};
                    

                    using (var package = new ExcelPackage(file.InputStream))
                    {
                        var currentSheet = package.Workbook.Worksheets;
                        var workSheet = currentSheet.First();
                        var noOfCol = workSheet.Dimension.End.Column;
                        var noOfRow = workSheet.Dimension.End.Row;
                        for (int rowIterator = 2; rowIterator <= noOfRow; rowIterator++)
                        {
                            var tran = new Transaction();

                            tran.Consignment_no = workSheet.Cells[rowIterator, 2].Value.ToString().Trim();
                            tran.chargable_weight = Convert.ToDouble(workSheet.Cells[rowIterator, 3].Value);
                            tran.Mode= workSheet.Cells[rowIterator, 4].Value.ToString();
                            tran.Quanntity = Convert.ToInt16(workSheet.Cells[rowIterator, 6].Value);
                            tran.Pincode = workSheet.Cells[rowIterator, 7].Value.ToString();
                            string abc = Convert.ToDateTime(workSheet.Cells[rowIterator, 8].Value.ToString()).ToShortDateString();
                            
                            string bdate = DateTime.ParseExact(abc.ToString(), formats, CultureInfo.InvariantCulture, DateTimeStyles.None).ToString("MM/dd/yyyy");
                            tran.booking_date = Convert.ToDateTime(bdate);
                            tran.Type_t= workSheet.Cells[rowIterator, 10].Value.ToString();
                            tran.Customer_Id = workSheet.Cells[rowIterator, 12].Value.ToString();

                            Transaction transaction = db.Transactions.Where(m => m.Consignment_no == tran.Consignment_no).FirstOrDefault();

                            if (transaction != null)
                            {

                                CalculateAmount ca = new CalculateAmount();

                                double? amt = ca.CalulateAmt(tran.Consignment_no, tran.Customer_Id, tran.Pincode, tran.Mode, Convert.ToDouble(tran.chargable_weight), tran.Type_t);

                                transaction.Amount = amt;
                                transaction.Customer_Id = tran.Customer_Id;

                                transaction.Consignment_no = tran.Consignment_no.Trim();
                                transaction.chargable_weight = tran.chargable_weight;
                                transaction.Mode = tran.Mode;
                                transaction.Quanntity = tran.Quanntity;
                                transaction.Pincode = tran.Pincode;
                                transaction.booking_date = tran.booking_date;
                                transaction.Type_t = tran.Type_t;

                                transaction.Pf_Code = db.Companies.Where(m => m.Company_Id == transaction.Customer_Id).Select(m => m.Pf_code).FirstOrDefault();
                                transaction.AdminEmp = 000;



                                db.Entry(transaction).State = EntityState.Modified;
                                db.SaveChanges();
                            }
                            else
                            {
                                CalculateAmount ca = new CalculateAmount();

                                double? amt = ca.CalulateAmt(tran.Consignment_no, tran.Customer_Id, tran.Pincode, tran.Mode, Convert.ToDouble(tran.chargable_weight), tran.Type_t);

                                tran.Amount = amt;
                                tran.Customer_Id = tran.Customer_Id;

                                tran.Pf_Code = db.Companies.Where(m => m.Company_Id == tran.Customer_Id).Select(m => m.Pf_code).FirstOrDefault();
                                tran.AdminEmp = 000;



                                db.Transactions.Add(tran);
                                db.SaveChanges();
                            }

                        }
                    }

                   TempData["Success"] = "Excel File With Other PF Uploaded SuccessFully";
                }
            }


            return RedirectToAction("importFromExcel");
        }




        public ActionResult importTextFile()
        {

            return View();
        }

        [HttpPost]
        public ActionResult importTextFile(HttpPostedFileBase ImportText)
        {
            string filePath = string.Empty;

            if (ImportText != null)
            {
                //string path = Server.MapPath("~/UploadsText/");

                //if (!Directory.Exists(path))
                //{
                //    Directory.CreateDirectory(path);
                //}

                //filePath = path + DateTime.Now.ToString().Replace("/", "").Replace(" ", "").Replace(":", "") + Path.GetFileName(ImportText.FileName);
                //string extension = Path.GetExtension(ImportText.FileName);
                //ImportText.SaveAs(filePath);




                //Task.Run(() => InsertRecords(filePath, ImportText.FileName));

                //Read the contents of CSV file.


                #region new uploadtext

                System.IO.StreamReader myReader = new System.IO.StreamReader(ImportText.InputStream);
                string csvData = myReader.ReadToEnd();

              //  string csvData = System.IO.File.ReadAllText(ImportText.ToString());

                //Execute a loop over the rows.
                int i = 0;
                foreach (string row in csvData.Split('\n'))
                {
                    i++;
                    if (i <= 2)
                    {
                        continue;
                    }

                    if (!string.IsNullOrEmpty(row))
                    {

                        string[] values = row.Split('"');


                        Transaction tr = new Transaction();

                        string[] formats = {"dd/MM/yyyy", "dd-MMM-yyyy", "yyyy-MM-dd",
                   "dd-MM-yyyy", "M/d/yyyy", "dd MMM yyyy"};
                        string bdate = DateTime.ParseExact(values[10].Trim('\''), formats, CultureInfo.InvariantCulture, DateTimeStyles.None).ToString("MM/dd/yyyy");



                        tr.Consignment_no = values[1].Trim('\'').Trim();
                        tr.Pf_Code = values[3].Trim('\'');
                        tr.Actual_weight = Convert.ToDouble(values[4].Replace("~", "").Trim('\''));
                        tr.Mode = values[5].Trim('\'');
                        tr.Quanntity = Convert.ToInt16(values[8].Trim('\''));
                        tr.Pincode = values[9].Trim('\'');
                        tr.booking_date = Convert.ToDateTime(bdate);
                        tr.tembookingdate = values[10].Trim('\'');
                        tr.dtdcamount = Convert.ToDouble(values[11].Replace("~", "").Trim('\''));
                        tr.chargable_weight = Convert.ToDouble(values[4].Replace("~", "").Trim('\''));
                        tr.diff_weight = Convert.ToDouble(values[4].Replace("~", "").Trim('\''));
                        tr.topay = "no";
                        tr.cod = "no";
                        //tr.Insurance = "no";
                        tr.Type_t = values[16].Trim('\'');
                        tr.Invoice_No =values[19].Trim('\'');
                        tr.BillAmount= Convert.ToDouble(values[21].Replace("~", "").Trim('\''));

                        if(tr.BillAmount==0.00)
                        {
                            tr.Insurance = "nocoverage";
                        }
                        else
                        {
                            tr.Insurance = "ownerrisk";
                        }
                        if (tr.Consignment_no.Length > 8)
                        {

                            Transaction insertupdate = db.Transactions.Where(m => m.Consignment_no == tr.Consignment_no).FirstOrDefault();





                            if (insertupdate == null)
                            {
                                // db.Entry(insertupdate).State = EntityState.Detached;

                                db.Transactions.Add(tr);
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

                            }
                            else
                            {
                                insertupdate.Pf_Code = values[3].Trim('\'');
                                insertupdate.dtdcamount = Convert.ToDouble(values[11].Replace("~", "").Trim('\''));
                                insertupdate.diff_weight = Convert.ToDouble(values[4].Replace("~", "").Trim('\''));
                                insertupdate.Consignment_no = insertupdate.Consignment_no.Trim();
                                insertupdate.Actual_weight = Convert.ToDouble(values[4].Replace("~", "").Trim('\''));
                                insertupdate.chargable_weight = Convert.ToDouble(values[4].Replace("~", "").Trim('\''));
                                insertupdate.Invoice_No = values[19].Trim('\'');
                                insertupdate.BillAmount = Convert.ToDouble(values[21].Replace("~", "").Trim('\''));
                                insertupdate.Insurance = tr.Insurance;

                                db.Entry(insertupdate).State = EntityState.Modified;

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

                                // db.SaveChanges();

                            }

                        }



                    }


                }

              
            }





            TempData["Upload"] = "File Uploaded Successfully!";
            return View();

            #endregion


            //TempData["Upload"] = "File Uploaded Successfully!";

            //return RedirectToAction("ConsignMent", "Booking");
        }


        public void InsertRecords(string filePath, string Filename)
        {
            List<Transaction> Tranjaction = new List<Transaction>();



            //Read the contents of CSV file.
            string csvData = System.IO.File.ReadAllText(filePath);

            //Execute a loop over the rows.
            int i = 0;
            foreach (string row in csvData.Split('\n'))
            {
                i++;
                if (i <= 2)
                {
                    continue;
                }

                if (!string.IsNullOrEmpty(row))
                {

                    string[] values = row.Split('"');


                    Transaction tr = new Transaction();

                    string[] formats = {"dd/MM/yyyy", "dd-MMM-yyyy", "yyyy-MM-dd",
                   "dd-MM-yyyy", "M/d/yyyy", "dd MMM yyyy"};
                    string bdate = DateTime.ParseExact(values[10].Trim('\''), formats, CultureInfo.InvariantCulture, DateTimeStyles.None).ToString("MM/dd/yyyy");



                    tr.Consignment_no = values[1].Trim('\'').Trim(); 
                    tr.Pf_Code = values[3].Trim('\'');
                    tr.Actual_weight = Convert.ToDouble(values[4].Replace("~", "").Trim('\''));
                    tr.Mode = values[5].Trim('\'');
                    tr.Quanntity = Convert.ToInt16(values[8].Trim('\''));
                    tr.Pincode = values[9].Trim('\'');
                    tr.booking_date = Convert.ToDateTime(bdate);
                    tr.tembookingdate = values[10].Trim('\'');
                    tr.dtdcamount = Convert.ToDouble(values[11].Replace("~", "").Trim('\''));
                    tr.chargable_weight = Convert.ToDouble(values[4].Replace("~", "").Trim('\''));
                    tr.diff_weight = Convert.ToDouble(values[4].Replace("~", "").Trim('\''));
                    tr.topay = "no";
                    tr.cod = "no";
                    tr.Insurance = "no";
                    tr.Type_t = values[16].Trim('\'');


                    if (tr.Consignment_no.Length > 8)
                    {

                        Transaction insertupdate = db.Transactions.Where(m => m.Consignment_no == tr.Consignment_no).FirstOrDefault();



                        

                        if (insertupdate == null)
                        {
                            // db.Entry(insertupdate).State = EntityState.Detached;

                            db.Transactions.Add(tr);
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

                        }
                        else
                        {
                            insertupdate.Pf_Code = values[3].Trim('\'');
                            insertupdate.dtdcamount = Convert.ToDouble(values[11].Replace("~", "").Trim('\''));
                            insertupdate.diff_weight = Convert.ToDouble(values[4].Replace("~", "").Trim('\''));
                            insertupdate.Consignment_no = insertupdate.Consignment_no.Trim();

                            db.Entry(insertupdate).State = EntityState.Modified;

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

                            // db.SaveChanges();

                        }

                    }



                }


            }

            Notification nt = new Notification();

            TimeZoneInfo INDIAN_ZONE = TimeZoneInfo.FindSystemTimeZoneById("India Standard Time");
            nt.dateN = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, INDIAN_ZONE);
            nt.Message = Filename + "File Uploaded Successfully From Branch";
            nt.Status = false;

            db.Notifications.Add(nt);
            db.SaveChanges();

        }



        [HttpPost]
        public async Task<ActionResult> InternationalimportTextFile(HttpPostedFileBase ImportText)
        {

            string filePath = string.Empty;

            if (ImportText != null)
            {
                string path = Server.MapPath("~/UploadsText/");

                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }

                filePath = path + DateTime.Now.ToString().Replace("/", "").Replace(" ", "").Replace(":", "") + Path.GetFileName(ImportText.FileName);
                string extension = Path.GetExtension(ImportText.FileName);
                ImportText.SaveAs(filePath);




                Task.Run(() => InsertRecords(filePath, ImportText.FileName));

            }




            TempData["Upload"] = "File Uploaded Successfully!";

            return RedirectToAction("ConsignMent", "Booking");

        }



        public ActionResult DeliveryFile()
        {

            return View();
        }

        [HttpPost]
        public ActionResult DeliveryFile(HttpPostedFileBase ImportText)
        {


            List<deliverydata> Tranjaction = new List<deliverydata>();
            string filePath = string.Empty;

            if (ImportText != null)
            {
                string path = Server.MapPath("~/Uploadsdelivery/");

                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }

                filePath = path + Path.GetFileName(ImportText.FileName);
                string extension = Path.GetExtension(ImportText.FileName);
                ImportText.SaveAs(filePath);

                //Read the contents of CSV file.
                string csvData = System.IO.File.ReadAllText(filePath);

                //Execute a loop over the rows.
                int i = 0;
                foreach (string row in csvData.Split('\n'))
                {
                    i++;
                    if (i <= 2)
                    {
                        continue;
                    }

                    if (!string.IsNullOrEmpty(row))
                    {

                        string[] values = row.Split('"');


                        deliverydata tr = new deliverydata();

                        string[] formats = {"dd/MM/yyyy", "dd-MMM-yyyy", "yyyy-MM-dd",
                   "dd-MM-yyyy", "M/d/yyyy", "dd MMM yyyy"};
                        // string bdate = DateTime.ParseExact(values[10].Trim('\''), formats, CultureInfo.InvariantCulture, DateTimeStyles.None).ToString("MM/dd/yyyy");



                        tr.consinmentno = values[0].Trim('\'').Trim();
                        tr.tempdelivereddate = values[1].Trim('\'');
                        tr.tempdeliveredtime = values[2].Trim('\'');
                        tr.receivedby = values[3].Trim('\'');
                        tr.remarks = values[4].Trim('\'');


                        deliverydata dr = db.deliverydatas.Where(m => m.consinmentno == tr.consinmentno).FirstOrDefault();

                        if (dr == null)
                        {
                            db.deliverydatas.Add(tr);
                            db.SaveChanges();
                        }
                        else
                        {
                            db.Entry(dr).State = EntityState.Detached;

                            tr.d_id = dr.d_id;
                            db.Entry(tr).State = EntityState.Modified;
                            db.SaveChanges();
                        }






                    }


                }
            }

            ViewBag.Message = "File Uploaded SuccessFully";

            return View();
        }

        [SessionAdmin]
        public ActionResult CreateUser()
        {

            List<SelectListItem> items = new List<SelectListItem>();

            items.Add(new SelectListItem { Text = "CashCounter", Value = "CashCounter" });

            items.Add(new SelectListItem { Text = "Billing", Value = "Billing" });

            items.Add(new SelectListItem { Text = "Customer Support", Value = "Customer Support" });

            items.Add(new SelectListItem { Text = "Branch Manager", Value = "Branch Manager" });

            ViewBag.Usertype = items;



            ViewBag.PF_Code = new SelectList(db.Franchisees.Where(x => !x.PF_Code.ToUpper().StartsWith("PR")), "PF_Code", "PF_Code");
            return View();

        }
        [SessionAdmin]
        [HttpPost]
        public ActionResult CreateUser(User user, string[] Usertype)
        {
            List<SelectListItem> items = new List<SelectListItem>();

            items.Add(new SelectListItem { Text = "CashCounter", Value = "CashCounter" });

            items.Add(new SelectListItem { Text = "Billing", Value = "Billing" });

            items.Add(new SelectListItem { Text = "Customer Support", Value = "Customer Support" });

            items.Add(new SelectListItem { Text = "Branch Manager", Value = "Branch Manager" });

            if (ModelState.IsValid)
            {


                db.Users.Add(user);
                db.SaveChanges();




                //////////Alert Afte Success///
                ViewBag.Success = " Added Successfully...!!!";
                ////////////////////////////////////////
                ViewBag.PF_Code = new SelectList(db.Franchisees.Where(x => !x.PF_Code.ToUpper().StartsWith("PR")), "PF_Code", "PF_Code", user.PF_Code);
                ViewBag.Usertype = items;
                ModelState.Clear();

                return View(new  User());
            }

            ViewBag.PF_Code = new SelectList(db.Franchisees.Where(x => !x.PF_Code.ToUpper().StartsWith("PR")), "PF_Code", "PF_Code", user.PF_Code);
            ViewBag.Usertype = items;


            return View(user);

        }

        [SessionAdmin]
        public ActionResult EditUser(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            User user = db.Users.Find(id);
            if (user == null)
            {
                return HttpNotFound();
            }

            List<SelectListItem> items = new List<SelectListItem>();

            items.Add(new SelectListItem { Text = "CashCounter", Value = "CashCounter" });

            items.Add(new SelectListItem { Text = "Billing", Value = "Billing" });
			
			items.Add(new SelectListItem { Text = "Customer Support", Value = "Customer Support" });

            items.Add(new SelectListItem { Text = "Branch Manager", Value = "Branch Manager" });

            var types = db.Users.Where(m => m.User_Id == id).Select(m => m.Usertype).FirstOrDefault();
            string[] split = types.Split(',');

            foreach (var item in items)
            {
                if (split.Contains(item.Value))
                {
                    item.Selected = true;

                }
            }

            ViewBag.Usertype = items;



            ViewBag.PF_Code = new SelectList(db.Franchisees.Where(x => !x.PF_Code.ToUpper().StartsWith("PR")), "PF_Code", "PF_Code", user.PF_Code);
            return View(user);
        }

        // POST: demo/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditUser([Bind(Include = "User_Id,Name,Email,Contact_no,PF_Code,Password_U,Usertype,Datetime_User")] User user, string[] Usertype)
        {
            if (ModelState.IsValid)
            {
                var result = string.Join(",", Usertype);
                user.Usertype = result;
                db.Entry(user).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("UserList");
            }

            List<SelectListItem> items = new List<SelectListItem>();

            items.Add(new SelectListItem { Text = "CashCounter", Value = "CashCounter" });

            items.Add(new SelectListItem { Text = "Billing", Value = "Billing" });

			items.Add(new SelectListItem { Text = "Customer Support", Value = "Customer Support" });

            items.Add(new SelectListItem { Text = "Branch Manager", Value = "Branch Manager" });

            ViewBag.PF_Code = new SelectList(db.Franchisees.Where(x => !x.PF_Code.ToUpper().StartsWith("PR")), "PF_Code", "PF_Code", user.PF_Code);
            return View(user);
        }


        [SessionAdmin]
        public ActionResult AddFranchisee()
        {
            return View();
        }


        [SessionAdmin]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddFranchisee(Franchisee franchisee)
        {
            foreach (ModelState modelState in ViewData.ModelState.Values)
            {
                foreach (ModelError error in modelState.Errors)
                {
                    Console.WriteLine(error.ErrorMessage);
                }
            }


            if (ModelState.IsValid)
            {
                db.Franchisees.Add(franchisee);
                db.SaveChanges();



                //Adding Eantries To the Sector Table
                var sectornamelist = db.sectorNames.ToList();

                var pfcode = (from u in db.Franchisees
                              where u.PF_Code == franchisee.PF_Code
                              select u).FirstOrDefault();
                if (pfcode != null)
                {
                    foreach (var i in sectornamelist)
                    {
                        Sector sn = new Sector();

                        sn.Pf_code = pfcode.PF_Code;
                        sn.Sector_Name = i.sname;



                        sn.CashD = true;
                        sn.CashN = true;
                        sn.BillD = true;
                        sn.BillN = true;


                        if (sn.Sector_Name == "Within City")
                        {
                            sn.Priority = 1;
                            sn.Pincode_values = "411001 - 411100";
                        }
                        else if (sn.Sector_Name == "Within Zone")
                        {

                            sn.CashD = true;
                            sn.CashN = true;
                            sn.BillD = false;
                            sn.BillN = false;

                            sn.Priority = 3;
                            sn.Pincode_values = "360000-400000,450000-490000";
                        }
                        else if (sn.Sector_Name == "Within State")
                        {
                            sn.Priority = 4;
                            sn.Pincode_values = "400000-450000";
                        }
                        else if (sn.Sector_Name == "Metro")
                        {
                            sn.Priority = 5;
                            sn.Pincode_values = "110000-110505,500001-500873,560000-560099,600001-600099,700001-700099";
                        }
                        else if (sn.Sector_Name == "Estern And Non Estern")
                        {
                            sn.Priority = 6;
                            sn.Pincode_values = "780000-800000,170000-180000";
                        }
                        else if (sn.Sector_Name == "Jammu And Kashmir")
                        {
                            sn.Priority = 7;
                            sn.Pincode_values = "180000-200000";
                        }
                        else if (sn.Sector_Name == "Rest Of India")
                        {
                            sn.Priority = 8;
                            sn.Pincode_values = "000000";
                        }
                        else if (sn.Sector_Name == "Mumbai")
                        {

                            sn.CashD = false;
                            sn.CashN = false;
                            sn.BillD = true;
                            sn.BillN = true;

                            sn.Priority = 2;
                            sn.Pincode_values = "400001-400610,400615-400706,400710-401203,401205-402209";
                        }
                        else
                        {
                            sn.Pincode_values = null;
                        }




                        db.Sectors.Add(sn);

                        db.SaveChanges();

                    }
                }
                //////////////////////////////////////////////

                ///Adding Eantries To New Company For Cash Counter ///               




                var Companyid = "Cash_" + franchisee.PF_Code;


                var secotrs = db.Sectors.Where(m => m.Pf_code == franchisee.PF_Code).ToList();

                Company cm = new Company();
                cm.Company_Id = Companyid;
                cm.Pf_code = franchisee.PF_Code;
                cm.Phone = 9657570808;
                cm.Company_Address = franchisee.F_Address;
                cm.Company_Name = Companyid;
                cm.Email = Companyid + "@gmail.com";
                db.Companies.Add(cm);



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


                foreach (var i in secotrs)
                {
                    Ratem dox = new Ratem();
                    Nondox ndox = new Nondox();
                    express_cargo cs = new express_cargo();

                    dox.Company_id = Companyid;
                    dox.Sector_Id = i.Sector_Id;
                    dox.NoOfSlab = 2;
                    //dox.CashCounter = true;

                    ndox.Company_id = Companyid;
                    ndox.Sector_Id = i.Sector_Id;
                    ndox.NoOfSlabN = 2;
                    ndox.NoOfSlabS = 2;
                    // ndox.CashCounterNon = true;


                    cs.Company_id = Companyid;
                    cs.Sector_Id = i.Sector_Id;

                    // cs.CashCounterExpr = true;

                    db.Ratems.Add(dox);
                    db.Nondoxes.Add(ndox);
                    db.express_cargo.Add(cs);


                }

                for (int i = 0; i < 5; i++)
                {
                    dtdcPlu dtplu = new dtdcPlu();
                    Dtdc_Ptp stptp = new Dtdc_Ptp();

                    if (i == 0)
                    {
                        dtplu.destination = "City Plus";
                        stptp.dest = "City";
                    }
                    else if (i == 1)
                    {
                        dtplu.destination = "Zonal Plus/Blue";
                        stptp.dest = "Zonal";

                    }
                    else if (i == 2)
                    {
                        dtplu.destination = "Metro Plus/Blue";
                        stptp.dest = "Metro";
                    }
                    else if (i == 3)
                    {
                        dtplu.destination = "National Plus/Blue";
                        stptp.dest = "National";
                    }
                    else if (i == 4)
                    {
                        dtplu.destination = "Regional Plus";
                        stptp.dest = "Regional";
                    }

                    dtplu.Company_id = Companyid;
                    // dtplu.CashCounterPlus = true;
                    stptp.Company_id = Companyid;


                    db.dtdcPlus.Add(dtplu);
                    db.Dtdc_Ptp.Add(stptp);

                }

                db.SaveChanges();

                /////////////////////////////////////////////////////
                //////////Alert Afte Success///
                TempData["Success1"] = " Added Successfully...!!!";
                ////////////////////////////////////////
                ModelState.Clear();

                return RedirectToAction("Add_SectorPin", new { PfCode = franchisee.PF_Code });
            }

            return View(franchisee);

        }


        [SessionAdmin]
        public ActionResult Add_SectorPin(string PfCode)
        {
            string Pf = PfCode; /*Session["PfID"].ToString();*/



            List<Sector> st = (from u in db.Sectors
                               where u.Pf_code == Pf
                              &&   u.GEcreate==null
                               select u).ToList();
            ViewBag.pfcode = PfCode;//stored in hidden format on the view


            return View(st);
        }


        [SessionAdmin]
        [HttpPost]
        public ActionResult Add_SectorPin(FormCollection fc, string pfcode)
        {
            string Pf = pfcode;

            ViewBag.pfcode = pfcode;//stored in hidden format on the view if All fields not filled

            var sectoridarray = fc.GetValues("item.Sector_Id");
            var pincodearayy = fc.GetValues("item.Pincode_values");


            for (int i = 0; i < sectoridarray.Count(); i++)
            {

                Sector str = db.Sectors.Find(Convert.ToInt16(sectoridarray[i]));

                if (pincodearayy[i] == "")
                {
                    pincodearayy[i] = null;
                }


                str.Pincode_values = pincodearayy[i];
                db.Entry(str).State = EntityState.Modified;

            }

            int result = pincodearayy.Count(s => s == null);

            if (result > 0)
            {
                ModelState.AddModelError("PinError", "All Fields Are Compulsary");

                List<Sector> stt = (from u in db.Sectors
                                    where u.Pf_code == Pf
                                      && u.GEcreate == null
                                    && u.Pincode_values == null
                                    select u).ToList();
                return View(stt);
            }
            else
            {
                db.SaveChanges();
                TempData["Success"] = "Sectors Added Successfully!";
            }


            List<Sector> st = (from u in db.Sectors
                               where u.Pf_code == Pf
                                 && u.GEcreate == null
                               select u).ToList();

            return View(st);
        }


        [SessionAdmin]
        public ActionResult Add_SectorPinEdit(string PfCode)
        {
            string Pf = PfCode; /*Session["PfID"].ToString();*/



            List<Sector> st = (from u in db.Sectors
                               where u.Pf_code == Pf
                               && u.GEcreate == null
                               orderby u.Priority
                               select u).ToList();
            ViewBag.pfcode = PfCode;//stored in hidden format on the view

            return View("Add_SectorPin", st);
        }

        public ActionResult Edit(string PfCode)
        {
            if (PfCode == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Franchisee franchisee = db.Franchisees.Find(PfCode);
            if (franchisee == null)
            {
                return HttpNotFound();
            }
            return View(franchisee);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Franchisee franchisee)
        {
            if (ModelState.IsValid)
            {
                db.Entry(franchisee).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("FranchiseeList");
            }
            return View(franchisee);
        }


        public ActionResult ImportCsv()
        {
            return View();
        }

        public ActionResult FranchiseeList()
        {

            return View(db.Franchisees.Where(x => !x.PF_Code.ToUpper().StartsWith("PR")).ToList());
        }


        public ActionResult UserList()
        {
           
            return View(db.Users.ToList());
        }

        public ActionResult Destinationlist()
        {


            return View(db.Destinations.ToList());
        }

        [SessionAdmin]
        public ActionResult Consignmentlist(string id)
        {

            return View(db.Receipt_details.ToList());
        }




        #region Edit Consignments


        public ActionResult EditCons(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Receipt_details receipt_details = db.Receipt_details.Find(id);

            if (receipt_details == null)
            {
                return HttpNotFound();
            }

            ViewBag.Pf_Code = new SelectList(db.Franchisees.Where(x => !x.PF_Code.ToUpper().StartsWith("PR")), "PF_Code", "F_Address", receipt_details.Pf_Code);
            ViewBag.User_Id = new SelectList(db.Users, "User_Id", "Name", receipt_details.User_Id);

            return View(receipt_details);
        }

        // POST: Receipt_details/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditCons([Bind(Include = "Receipt_Id,Consignment_No,Destination,sender_phone,Sender_Email,Sender,SenderCompany,SenderAddress,SenderCity,SenderState,SenderPincode,Reciepents_phone,Reciepents_Email,Reciepents,ReciepentCompany,ReciepentsAddress,ReciepentsCity,ReciepentsState,ReciepentsPincode,Shipmenttype,Shipment_Length,Shipment_Quantity,Shipment_Breadth,Shipment_Heigth,DivideBy,TotalNo,Actual_Weight,volumetric_Weight,DescriptionContent1,DescriptionContent2,DescriptionContent3,Amount1,Amount2,Amount3,Total_Amount,Insurance,Insuance_Percentage,Insuance_Amount,Charges_Amount,Charges_Service,Risk_Surcharge,Service_Tax,Charges_Total,Cash,Credit,Credit_Amount,secure_Pack,Passport,OfficeSunday,Shipment_Mode,Addition_charge,Addition_Lable,Discount,Pf_Code,User_Id,Datetime_Cons,Paid_Amount")] Receipt_details receipt_details)
        {
            if (ModelState.IsValid)
            {
                db.Entry(receipt_details).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.Pf_Code = new SelectList(db.Franchisees.Where(x => !x.PF_Code.ToUpper().StartsWith("PR")), "PF_Code", "F_Address", receipt_details.Pf_Code);
            ViewBag.User_Id = new SelectList(db.Users, "User_Id", "Name", receipt_details.User_Id);
            return View(receipt_details);
        }

        #endregion


        //public ActionResult DeleteCons(string id)
        //{
        //    Receipt_details receipt_details = db.Receipt_details.Where(m => m.Consignment_No == id).FirstOrDefault();
        //    db.Receipt_details.Remove(receipt_details);
        //    //db.SaveChanges();
        //    return RedirectToAction("Consignmentlist");
        //}


        public ActionResult WalletHistory(string phone)
        {
            List<wallet_History> wallet_History = db.wallet_History.Where(m => m.mobile_no == phone).ToList();
            return View(wallet_History);
        }



        public ActionResult LogOut()
        {


            // Microsoft.SqlServer.Management.Smo.Backup backup = new Microsoft.SqlServer.Management.Smo.Backup();
            // //Set type of backup to be performed to database
            // backup.Action = Microsoft.SqlServer.Management.Smo.BackupActionType.Database;
            // backup.BackupSetDescription = "BackupDataBase description";
            // //Set the name used to identify a particular backup set.
            // backup.BackupSetName = "Backup";
            // //specify the name of the database to back up
            // backup.Database = "DtdcBilling";
            // backup.Initialize = true;
            // backup.Checksum = true;
            // //Set it to true to have the process continue even after checksum error.
            // backup.ContinueAfterError = true;
            // //Set the backup expiry date.
            // backup.ExpirationDate = DateTime.Now.AddDays(3);
            // //truncate the database log as part of the backup operation.
            // backup.LogTruncation = Microsoft.SqlServer.Management.Smo.BackupTruncateLogType.Truncate;



            // Microsoft.SqlServer.Management.Smo.BackupDeviceItem deviceItem = new Microsoft.SqlServer.Management.Smo.BackupDeviceItem(
            //                     "E:\\DtdcBilling1.Bak",
            //                     Microsoft.SqlServer.Management.Smo.DeviceType.File);
            // backup.Devices.Add(deviceItem);

            //     ServerConnection connection = new ServerConnection(@"43.255.152.26");

            // // Log in using SQL authentication
            // connection.LoginSecure = false;
            // connection.Login = "DtdcBilling";
            // connection.Password = "Billing@123";
            // Microsoft.SqlServer.Management.Smo.Server sqlServer = new Microsoft.SqlServer.Management.Smo.Server(connection);


            ////start the back up operation

            // backup.SqlBackup(sqlServer);


            //SqlConnection con = new SqlConnection();
            //SqlCommand sqlcmd = new SqlCommand();
            //SqlDataAdapter da = new SqlDataAdapter();


            //con.ConnectionString = @"Data Source=43.255.152.26;Initial Catalog=DtdcBilling;User id=DtdcBilling;Password=Billing@123";



            //string backupDIR = Server.MapPath("~/Content/");

            //if (!System.IO.Directory.Exists(Server.MapPath(backupDIR)))
            //{
            //    System.IO.Directory.CreateDirectory(Server.MapPath(backupDIR));
            //}
            //try
            //{
            //    con.Open();
            //    sqlcmd = new SqlCommand("backup database DtdcBilling to disk='" + backupDIR + "//" + DateTime.Now.ToString("ddMMyyyy_HHmmss") + ".Bak'", con);
            //    sqlcmd.ExecuteNonQuery();
            //    con.Close();

            //}
            //catch (Exception ex)
            //{
            //    con.Close();
            //}

            //SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["Data Source=43.255.152.26;Initial Catalog=DtdcBilling;User id=DtdcBilling;Password=Billing@123"].ConnectionString);
            //SqlCommand cmd = new SqlCommand();
            //cmd.Connection = con;
            //cmd.CommandText = "BACKUP DATABASE MyDB TO DISK = 'E:\\DB.bak'";
            //con.Open();
            //cmd.ExecuteNonQuery();
            //con.Close();


            if (Request.Cookies["Cookies"] != null)
            {
                var c = new HttpCookie("Cookies");
                c.Expires = DateTime.Now.AddDays(-1);
                Response.Cookies.Add(c);
            }

            FormsAuthentication.SignOut();
            Session.Abandon(); // it will clear the session at the end of request
            return RedirectToAction("Adminlogin", "Admin");

        }


        public ActionResult DeleteCons()
        {

            return View();
        }
        [HttpPost]
        public ActionResult DeleteCons(string id)
        {
            var validcons = db.Receipt_details.Where(p => p.Consignment_No == id).FirstOrDefault();
            if (validcons != null)
            {
                Receipt_details receipt = db.Receipt_details.Where(m => m.Consignment_No == id).FirstOrDefault();
                db.Receipt_details.Remove(receipt);
                db.SaveChanges();
            }
            else
            {
                TempData["fail"] = "Invalid Consignment";
                return View();
            }
            TempData["Success"] = "Consignment Deleted SuccessFully";
            return RedirectToAction("Consignmentlist");
        }
        public ActionResult DeleteCompapy(string id)
        {

            List<Dtdc_Ptp> dtdc_Ptps = db.Dtdc_Ptp.Where(m => m.Company_id == id).ToList();
            List<dtdcPlu> dtdcPlu = db.dtdcPlus.Where(m => m.Company_id == id).ToList();
            List<express_cargo> express_cargo = db.express_cargo.Where(m => m.Company_id == id).ToList();
            List<Nondox> Nondox = db.Nondoxes.Where(m => m.Company_id == id).ToList();
            List<Ratem> Ratem = db.Ratems.Where(m => m.Company_id == id).ToList();
            List<Priority> pra = db.Priorities.Where(m => m.Company_id == id).ToList();
            List<RateLaptop> lap = db.RateLaptops.Where(m => m.Company_id == id).ToList();
            List<RateRevLaptop> rlap = db.RateRevLaptops.Where(m => m.Company_id == id).ToList();
            List<NewDtdc_Ecommerce> ecom = db.NewDtdc_Ecommerce.Where(m => m.Company_id == id).ToList();
            List<GECrate> gec=db.GECrates.Where(m=>m.Company_id==id).ToList();
            Company tran = db.Companies.Where(m => m.Company_Id == id).FirstOrDefault();

            foreach (var i in dtdc_Ptps)
            {
                db.Dtdc_Ptp.Remove(i);
            }
            foreach (var i in dtdcPlu)
            {
                db.dtdcPlus.Remove(i);
            }
            foreach (var i in express_cargo)
            {
                db.express_cargo.Remove(i);
            }
            foreach (var i in Nondox)
            {
                db.Nondoxes.Remove(i);
            }
            foreach (var i in Ratem)
            {
                db.Ratems.Remove(i);
            }
            foreach (var i in pra)
            {
                db.Priorities.Remove(i);
            }
            foreach (var i in lap)
            {
                db.RateLaptops.Remove(i);
            }
            foreach (var i in rlap)
            {
                db.RateRevLaptops.Remove(i);
            }
            foreach (var i in ecom)
            {
                db.NewDtdc_Ecommerce.Remove(i);
            }
            foreach(var i in gec)
            {
                db.GECrates.Remove(i);
            }
            db.Companies.Remove(tran);

            db.SaveChanges();
            TempData["Success"] = "Company Deleted SuccessFully";
            return RedirectToAction("EditCompanyRateMaster", "RateMaster");
        }


        public string getMail()
        {
            //MemoryStream memoryStream = new MemoryStream(renderByte);
            //var emailcontent = db.EmailPromotions.Where(a => a.message == "True").FirstOrDefault();
            using (MailMessage mm = new MailMessage("billing@atkexp.com", "navlakheprajkta23@gmail.com"))
            {
                mm.Subject = "Invoice";
                string Bodytext = "";

               
                Bodytext = "<html><body>Please Find Attachment 132456789</body></html>";
                

               // Attachment attachment = new Attachment(memoryStream, "Invoice.pdf");

                mm.IsBodyHtml = true;


                mm.BodyEncoding = System.Text.Encoding.GetEncoding("utf-8");

                AlternateView plainView = System.Net.Mail.AlternateView.CreateAlternateViewFromString(System.Text.RegularExpressions.Regex.Replace(Bodytext, @"<(.|\n)*?>", string.Empty), null, "text/plain");
                // mm.Body = Bodytext;
                mm.Body = Bodytext;

                //Add Byte array as Attachment.

                //mm.Attachments.Add(attachment);

                SmtpClient smtp = new SmtpClient();
                smtp.Host = "smtp.gmail.com";
                //smtp.Host = "smtp.gmail.com";
                mm.ReplyToList.Add("acctsmfpune@dtdc.com");
                smtp.EnableSsl = true;
                System.Net.NetworkCredential credentials = new System.Net.NetworkCredential();
                credentials.UserName = "billing@atkexp.com";
                credentials.Password = "Billing@321";
                smtp.UseDefaultCredentials = true;
                smtp.Credentials = credentials;
                smtp.Port = 587;
                smtp.Send(mm);
            }
            return "";
        }

        [HttpGet]
        public ActionResult importFromExcelDpInvoice()
        {
            return View();
        }

        [HttpPost]
        public ActionResult importFromExcelDpInvoice(HttpPostedFileBase httpPostedFileBase)
        {
            if (httpPostedFileBase != null)
            {
                HttpPostedFileBase file = httpPostedFileBase;
                if ((file != null) && (file.ContentLength > 0) && !string.IsNullOrEmpty(file.FileName))
                {
                    string fileName = file.FileName;
                    string fileContentType = file.ContentType;
                    byte[] fileBytes = new byte[file.ContentLength];
                    var data = file.InputStream.Read(fileBytes, 0, Convert.ToInt32(file.ContentLength));

                    string[] formats = { "dd-MM-yyyy", "dd/MM/yyyy" };


                    using (var package = new ExcelPackage(file.InputStream))
                    {
                        var currentSheet = package.Workbook.Worksheets;
                        var workSheet = currentSheet.First();
                        var noOfCol = workSheet.Dimension.End.Column;
                        var noOfRow = workSheet.Dimension.End.Row;
                        for (int rowIterator = 2; rowIterator <= noOfRow; rowIterator++)
                        {
                            var tran = new Transaction();

                            tran.Consignment_no = workSheet.Cells[rowIterator, 2].Value.ToString().Trim();
                            tran.chargable_weight = Convert.ToDouble(workSheet.Cells[rowIterator, 3].Value);
                            tran.Mode = workSheet.Cells[rowIterator, 4].Value.ToString();
                            tran.Quanntity = Convert.ToInt16(workSheet.Cells[rowIterator, 6].Value);
                            tran.Pincode = workSheet.Cells[rowIterator, 7].Value.ToString();
                            string abc = workSheet.Cells[rowIterator, 8].Value.ToString();

                           // string abc = workSheet.Cells[rowIterator, 8].Value.ToString();


                            var da = (DateTime)workSheet.Cells[rowIterator, 8].Value;
                            // Get date value from Excel cell
                            var excelDate = da.ToOADate();

                            // Convert Excel date to .NET DateTime object
                            var date = new DateTime(1900, 1, 1).AddDays(excelDate - 2);

                            // Format date in desired format
                            var formattedDate = date.ToString("MM/dd/yyyy");


                            // Parse formatted date string to DateTime object
                            DateTime date1 = DateTime.ParseExact(formattedDate, "MM/dd/yyyy", CultureInfo.InvariantCulture);

                            tran.booking_date = date1;



                            tran.Type_t = workSheet.Cells[rowIterator, 9].Value.ToString();
                            tran.Customer_Id = workSheet.Cells[rowIterator, 10].Value.ToString();
                            tran.Amount = Convert.ToDouble(workSheet.Cells[rowIterator, 11].Value.ToString());

                            Transaction transaction = db.Transactions.Where(m => m.Consignment_no == tran.Consignment_no).FirstOrDefault();
                           

                            if (transaction != null)
                            {

                             
                                transaction.Amount = tran.Amount;
                                transaction.Customer_Id = tran.Customer_Id;

                                transaction.Consignment_no = tran.Consignment_no.Trim();
                                transaction.chargable_weight = tran.chargable_weight;
                                transaction.Mode = tran.Mode;
                                transaction.Quanntity = tran.Quanntity;
                                transaction.Pincode = tran.Pincode;
                               
                              
                                transaction.booking_date = tran.booking_date;
                                transaction.Type_t = tran.Type_t;

                                transaction.Pf_Code = db.Companies.Where(m => m.Company_Id == transaction.Customer_Id).Select(m => m.Pf_code).FirstOrDefault();
                                transaction.AdminEmp = 000;



                                db.Entry(transaction).State = EntityState.Modified;
                                db.SaveChanges();
                            }
                            else
                            {
                               
                                tran.Amount = tran.Amount;
                                tran.Customer_Id = tran.Customer_Id;

                                tran.Pf_Code = db.Companies.Where(m => m.Company_Id == tran.Customer_Id).Select(m => m.Pf_code).FirstOrDefault();
                                tran.AdminEmp = 000;



                                db.Transactions.Add(tran);
                                db.SaveChanges();
                            }

                        }
                    }

                    TempData["Success"] = "Excel File Uploaded SuccessFully";
                }
            }

           
            return RedirectToAction("importFromExcel");
        }

       

    }
}
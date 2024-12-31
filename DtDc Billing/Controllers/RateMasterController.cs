using DtDc_Billing.Entity_FR;
using DtDc_Billing.Models;
using Microsoft.Reporting.WebForms;
using Microsoft.SqlServer.Management.Smo;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Math;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Text;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Data.Entity;
using System.Data.Entity.Core.Metadata.Edm;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace DtDc_Billing.Controllers
{
    [SessionAdmin]
    public class RateMasterController : Controller
    {
        private DB_A43B74_wingsgrowdbEntities db = new DB_A43B74_wingsgrowdbEntities();

        // GET: RateMaster
        public ActionResult Index(string id)
        {

            ViewBag.companyid = Server.UrlDecode(Request.Url.Segments[3]);
            id = id.Replace("__", "&").Replace("xdotx", "."); ;
            var CompanyId = id;

            Company company = db.Companies.Where(m => m.Company_Id == CompanyId).FirstOrDefault();


           
                @ViewBag.Slabs = db.Ratems.Where(m => m.Company_id == CompanyId).FirstOrDefault();
           
                @ViewBag.Slabs1 = db.Nondoxes.Where(m => m.Company_id == CompanyId).FirstOrDefault();
           
                @ViewBag.Slabspri = db.Priorities.Where(m => m.Company_id == CompanyId).FirstOrDefault();
            
            ViewBag.Company = company;

            ViewBag.Dox     = db.Ratems.Where(m =>        m.Company_id      == CompanyId && m.Sector.BillD == true && (m.Sector.GEcreate==null || m.Sector.GEcreate==false)).OrderBy(m=>m.Sector.Priority).ToList();

            ViewBag.NonDox  = db.Nondoxes.Where(m =>      m.Company_id      == CompanyId && m.Sector.BillN == true && (m.Sector.GEcreate == null || m.Sector.GEcreate == false)).OrderBy(m => m.Sector.Priority).ToList();

            ViewBag.Plus    = db.dtdcPlus.Where(m =>      m.Company_id      == CompanyId && (m.Sector.GEcreate == null || m.Sector.GEcreate == false)).ToList();

            ViewBag.Ptp     = db.Dtdc_Ptp.Where(m =>      m.Company_id      == CompanyId && (m.Sector.GEcreate == null || m.Sector.GEcreate == false)).ToList();

            ViewBag.Cargo   = db.express_cargo.Where(m => m.Company_id      == CompanyId && m.Sector.BillD == true && (m.Sector.GEcreate == null || m.Sector.GEcreate == false)).Include(e => e.Sector).OrderBy(m => m.Sector.Priority).ToList();

            ViewBag.Priority = db.Priorities.Where(m => m.Company_id == CompanyId && (m.Sector.BillD == true || m.Sector.BillN == true) && (m.Sector.GEcreate == null || m.Sector.GEcreate == false)).OrderBy(m=>m.Sector.Priority).ToList();

            ViewBag.Laptops = db.RateLaptops.Where(m => m.Company_id == CompanyId && m.Sector.BillN == true && (m.Sector.GEcreate == null || m.Sector.GEcreate == false)).OrderBy(m => m.Sector.Priority).ToList();

            ViewBag.RevLaptops = db.RateRevLaptops.Where(m => m.Company_id == CompanyId && m.Sector.BillN == true && (m.Sector.GEcreate == null || m.Sector.GEcreate == false)).OrderBy(m => m.Sector.Priority).ToList();
            
           // ViewBag.Ecommerce = db.NewDtdc_Ecommerce.Where(m => m.Company_id == CompanyId && m.Sector.BillN == true).OrderBy(m => m.Sector.Priority).ToList();
            var getEcom = db.NewDtdc_Ecommerce.Where(m => m.Company_id == CompanyId && (m.Sector.GEcreate == null || m.Sector.GEcreate == false)).Include(e => e.Sector).OrderBy(m => m.Sector.Priority).ToList();
            ViewBag.Ecommerce = getEcom;

            
            ViewBag.com = getEcom.FirstOrDefault();

            //var getGEC = (from GECr in db.GECrates
            //              join sector in db.GECSectors on GECr.SectorName equals sector.SectorName

            //              join s in db.Sectors on       GECr.Sector_Id equals s.Sector_Id 
            //              where   GECr.Company_id == CompanyId && sector.Pf_code==company.Pf_code 
            //              select new GECrateModel
            //              {
            //                 SectorName = sector.SectorName,
            //                  GECrateId= GECr.GECrateId,
            //                  Slab1 = GECr.Slab1,
            //                  Slab2 = GECr.Slab2,
            //                  Slab3 = GECr.Slab3,
            //                  Slab4 = GECr.Slab4,
            //                  Uptosl1 = GECr.Uptosl1,
            //                  Uptosl2 = GECr.Uptosl2,
            //                  Uptosl3 = GECr.Uptosl3,
            //                  Uptosl4 = GECr.Uptosl4,
            //                  Company_id = GECr.Company_id,
            //                  NoOfSlab = GECr.NoOfSlab
            //              }).OrderBy(x=>x.Sector.Priority).ToList();
            // Extract the primitive values first
            // Extract the primitive value for companyPfCode first
            var companyPfCode = company.Pf_code;

            var getGEC = (from GECr in db.GECrates
                      
                          join s in db.Sectors on GECr.Sector_Id equals s.Sector_Id
                          where GECr.Company_id == CompanyId
                                && s.Pf_code == companyPfCode
                                && s.GEcreate==true
                                &&  (GECr.Sector.GEcreate != null || GECr.Sector.GEcreate == true)
                          // Directly checking if s.GECrates is true
                          select new
                          {
                              SectorName = s.Sector_Name,
                              GECrateId = GECr.GECrateId,
                              Slab1 = GECr.Slab1,
                              Slab2 = GECr.Slab2,
                              Slab3 = GECr.Slab3, 
                              Slab4 = GECr.Slab4,
                              Uptosl1 = GECr.Uptosl1,
                              Uptosl2 = GECr.Uptosl2,
                              Uptosl3 = GECr.Uptosl3,
                              Uptosl4 = GECr.Uptosl4,
                              Company_id = GECr.Company_id,
                              NoOfSlab = GECr.NoOfSlab,
                              Priority = s.Priority, // Include Priority for ordering
                             Sector_Id=GECr.Sector_Id
                          })
              .ToList() // Execute the query and bring the results into memory
              .OrderBy(x => x.Priority) // Order by Priority after the selection
              .Select(x => new GECrateModel
              {
                  SectorName = x.SectorName,
                  GECrateId = x.GECrateId,
                  Slab1 = x.Slab1,
                  Slab2 = x.Slab2,
                  Slab3 = x.Slab3,
                  Slab4 = x.Slab4,
                  Uptosl1 = x.Uptosl1,
                  Uptosl2 = x.Uptosl2,
                  Uptosl3 = x.Uptosl3,
                  Uptosl4 = x.Uptosl4,
                  Company_id = x.Company_id,
                  Sector_Id=x.Sector_Id,
                  NoOfSlab = x.NoOfSlab
              }).ToList();


            ////////if company does not having sector of same pf as companies pfcode(insert all data)/////////
            if (getGEC.Count() == 0)
            {
                string[] sectornamelist = new string[]
                        {
                            "CENTRAL I",
                            "CENTRAL II",
                            "EAST I",
                            "EAST II",
                            "NORTH EAST I",
                            "NORTH EAST II",
                            "NORTH EAST III",
                            "NORTH I",
                            "NORTH II",
                            "NORTH III",
                            "SOUTH I",
                            "SOUTH II",
                            "SOUTH III",
                            "WEST I",
                            "WEST II"
                        };


                var sector = db.Sectors.Where(m => m.Pf_code == companyPfCode && (m.GEcreate != null || m.GEcreate == true)).ToList();
                if (sector.Count == 0)
                {
                    var p = 1;
                    foreach (var i in sectornamelist)
                    {
                        Sector sn = new Sector();

                        sn.Pf_code = companyPfCode;
                        sn.CashD = false;
                        sn.CashN = false;
                        sn.BillD = false;
                        sn.BillN = false;
                        sn.Sector_Name = i;
                        sn.Priority = p;
                        sn.GEcreate = true;

                        db.Sectors.Add(sn);
                        db.SaveChanges();
                        p++;

                    }
                }
                else
                {

                    ///////delete old pfcode data//////////
                    var getOldSectors = db.GECrates.Where(x => x.Company_id == CompanyId).ToList();

                    if(getOldSectors.Count() > 0)
                    {
                        db.GECrates.RemoveRange(getOldSectors);
                        db.SaveChanges();
                    }


                    foreach (var i in sector)
                    {
                        GECrate ge = new GECrate();
                        ge.Slab1 = 1;
                        ge.Slab2 = 1;
                        ge.Slab3 = 1;
                        ge.Slab4 = 1;
                        ge.Uptosl1 = 1;
                        ge.Uptosl2 = 1;
                        ge.Uptosl3 = 1;
                        ge.Uptosl4 = 1;
                        ge.Company_id = CompanyId;
                        ge.NoOfSlab = 2;
                        ge.Sector_Id = i.Sector_Id;
                        ge.SectorName = i.Sector_Name;
                        db.GECrates.Add(ge);
                        db.SaveChanges();
                    }
                }



                var updatedgetGEC = (from GECr in db.GECrates

                                     join s in db.Sectors on GECr.Sector_Id equals s.Sector_Id
                                     where GECr.Company_id == CompanyId
                                           && s.Pf_code == companyPfCode
                                           && s.GEcreate == true
                                           && (GECr.Sector.GEcreate != null || GECr.Sector.GEcreate == true)
                                     // Directly checking if s.GECrates is true
                                     select new
                                     {
                                         SectorName = s.Sector_Name,
                                         GECrateId = GECr.GECrateId,
                                         Slab1 = GECr.Slab1,
                                         Slab2 = GECr.Slab2,
                                         Slab3 = GECr.Slab3,
                                         Slab4 = GECr.Slab4,
                                         Uptosl1 = GECr.Uptosl1,
                                         Uptosl2 = GECr.Uptosl2,
                                         Uptosl3 = GECr.Uptosl3,
                                         Uptosl4 = GECr.Uptosl4,
                                         Company_id = GECr.Company_id,
                                         NoOfSlab = GECr.NoOfSlab,
                                         Priority = s.Priority, // Include Priority for ordering
                                         Sector_Id = GECr.Sector_Id
                                     })
          .ToList() // Execute the query and bring the results into memory
          .OrderBy(x => x.Priority) // Order by Priority after the selection
          .Select(x => new GECrateModel
          {
              SectorName = x.SectorName,
              GECrateId = x.GECrateId,
              Slab1 = x.Slab1,
              Slab2 = x.Slab2,
              Slab3 = x.Slab3,
              Slab4 = x.Slab4,
              Uptosl1 = x.Uptosl1,
              Uptosl2 = x.Uptosl2,
              Uptosl3 = x.Uptosl3,
              Uptosl4 = x.Uptosl4,
              Company_id = x.Company_id,
              Sector_Id = x.Sector_Id,
              NoOfSlab = x.NoOfSlab
          }).ToList();



                ViewBag.GECratemaster = updatedgetGEC;
                ViewBag.SlabGEC = updatedgetGEC.FirstOrDefault();

            }
            else
            {

                ViewBag.GECratemaster = getGEC;
                ViewBag.SlabGEC = getGEC.FirstOrDefault();
            }
            //<-------------risk surch charge dropdown--------------->

            double? selectedval = company.Minimum_Risk_Charge;

            //selecteddrpval = db.Companies.Where(m => m.Company_Id == CompanyId).Select(m => m.isregister).FirstOrDefault();

            List<SelectListItem> items = new List<SelectListItem>();

            items.Add(new SelectListItem { Text = "--Select--", Value = "--Select--" });
            items.Add(new SelectListItem { Text = "IsRegister", Value = "IsRegister" });
            items.Add(new SelectListItem { Text = "NotRegister", Value = "NotRegister" });

            string Startdate = Convert.ToString(company.StartDate);//.ToString("MM/dd/yyyy");
            string[] StrDate = Startdate.Split(' ');
            ViewBag.StartDate = StrDate[0];

            string Enddate = Convert.ToString(company.EndDate);//.ToString("MM/dd/yyyy");
            string[] StrEndDate = Enddate.Split(' ');
            ViewBag.EndDate = StrEndDate[0];

            if (company.Isregister == null)
            {
                var selected = items.Where(x => x.Value == "--Select--").First();
                selected.Selected = true;
            }   
            else
            {

                var selected = items.Where(x => x.Value == company.Isregister).First();
                selected.Selected = true;
            }

            ViewBag.IsRegister = items;



            List<SelectListItem> itemsproduct = new List<SelectListItem>();

            itemsproduct.Add(new SelectListItem { Text = "Dox", Value = "Ratem" });

            itemsproduct.Add(new SelectListItem { Text = "NonDox", Value = "NonDox" });

            itemsproduct.Add(new SelectListItem { Text = "DTDCPLus", Value = "DTDCPLus" });

            itemsproduct.Add(new SelectListItem { Text = "DTDCPTP", Value = "DTDCPTP" });

            itemsproduct.Add(new SelectListItem { Text = "ExpressCargo", Value = "ExpressCargo" });

            itemsproduct.Add(new SelectListItem { Text = "Priority", Value = "Priority" });

            itemsproduct.Add(new SelectListItem { Text = "Laptops", Value = "Laptops" });

            itemsproduct.Add(new SelectListItem { Text = "RevPickupLaptops", Value = "RevPickupLaptops" });

            itemsproduct.Add(new SelectListItem { Text = "Ecommerce", Value = "Ecommerce" });
            itemsproduct.Add(new SelectListItem { Text = "GECMode", Value = "GECMode" });


            var types = company.ProductType;

            if (types != null)
            {
                string[] split = types.Split(',');

                foreach (var item in itemsproduct)
                {
                    if (split.Contains(item.Value))
                    {
                        item.Selected = true;

                    }
                }
            }
            ViewBag.Producttype = itemsproduct;



            //  ViewBag.Minimum_Risk_Charge = items;

            //<-------------risk surch charge dropdown--------------->

            ViewBag.Pf_code = new SelectList(db.Franchisees.Where(x => !x.PF_Code.ToUpper().StartsWith("PR")), "PF_Code", "PF_Code", company.Pf_code);

            @ViewBag.path = company.DocumentFilepath;

            ViewBag.companyid = company.Company_Id;
            ViewBag.Remark = company.Remark;
            ViewBag.Path = company.DocumentFilepath;

           
             var  arr = company.ProductType;

            //    string[] values = arr.Split(',');

            //for (int i = 0; i < values.Length; i++)
            //{
            //    values[i] = values[i].Trim();

            //}
            if(arr==null)
            {
                ViewBag.AllProduct = 0;
            }
          else
            { 
                ViewBag.AllProduct = arr;
            }
            return View();
        }

        public ActionResult ExportToExcel()
        {
            //List<Company> termsList11 = new List<Company>();
            var df = db.Companies.Where(d=>d.IsAgreementoption!=1).ToList();
            
            var gv = new GridView();
            //  gv.DataSource = bookingBLL.GetAllTutorRating().ToList().Select(x=>new {x.Tutor.Tutor_Firstname,x.TeachingSkill,x.SubjectKnowledge,x.Punctuality,x.OverallExperience,x.rate });
            gv.DataSource = df;

            gv.DataBind();
            Response.ClearContent();
            Response.Buffer = true;
            Response.AddHeader("content-disposition", "attachment; filename=AllRateMaster.xls");
            Response.ContentType = "application/ms-excel";
            Response.Charset = "";
            StringWriter objStringWriter = new StringWriter();
            HtmlTextWriter objHtmlTextWriter = new HtmlTextWriter(objStringWriter);
            gv.RenderControl(objHtmlTextWriter);
            Response.Output.Write(objStringWriter.ToString());
            Response.Flush();
            Response.End();
            //return RedirectToAction("Index", "RateMaster");
            return View("Index");
        }

        public ActionResult EditCompanyRateMaster()
        {
            return View(db.Companies.ToList());
        }


        public ActionResult EditCompanyRate(string Id)
        {
            var Idd = Id.Replace("__", "&").Replace("xdotx", "."); ;
            var pfcode = db.Companies.Where(m => m.Company_Id == Idd).FirstOrDefault();

            TempData["CompanyId"] = Id;
                var secotrs = db.Sectors.Where(m => m.Pf_code == pfcode.Pf_code     && (m.GEcreate == null || m.GEcreate == false)).ToList();
            
            Priority pri = new Priority();

            RateLaptop lap = new RateLaptop();

            RateRevLaptop revlap = new RateRevLaptop();

            NewDtdc_Ecommerce ecom = new NewDtdc_Ecommerce();
            GECrate gECrate = new GECrate();

            var prio = db.Priorities.Where(m => m.Company_id == "BASIC_TS").ToArray();
            
            var laps = db.RateLaptops.Where(m => m.Company_id == "BASIC_TS").ToArray();

            var revlaps = db.RateRevLaptops.Where(m => m.Company_id == "BASIC_TS").ToArray();

            var dtecom=db.Dtdc_Ecommerce.Where(m => m.Company_id == "BASIC_TS").ToArray();

            int j = 0;

            var ecommerces = db.NewDtdc_Ecommerce.Where(m => m.Company_id == Idd && m.Sector.Pf_code==pfcode.Pf_code).ToList();
            var gecrate = db.GECrates.Where(m => m.Company_id == Idd && m.Sector.Pf_code==pfcode.Pf_code && (m.Sector.GEcreate!=null || m.Sector.GEcreate==true)).ToList();
            
            string[] sectornamelist=  new string[]
                        {
                            "CENTRAL I",
                            "CENTRAL II",
                            "EAST I",
                            "EAST II",
                            "NORTH EAST I",
                            "NORTH EAST II",
                            "NORTH EAST III",
                            "NORTH I",
                            "NORTH II",
                            "NORTH III",
                            "SOUTH I",
                            "SOUTH II",
                            "SOUTH III",
                            "WEST I",
                            "WEST II"
                        };

            if (gecrate.Count == 0)
            {
            
                var sector = db.Sectors.Where(m => m.Pf_code == pfcode.Pf_code && (m.GEcreate != null || m.GEcreate == true)).ToList();
                if (sector.Count == 0)
                {
                    var p = 1;
                    foreach (var i in sectornamelist)
                    {
                        Sector sn = new Sector();

                        sn.Pf_code = pfcode.Pf_code;
                        sn.CashD = false;
                        sn.CashN = false;
                        sn.BillD = false;
                        sn.BillN = false;
                        sn.Sector_Name = i;
                        sn.Priority = p;
                        sn.GEcreate = true;

                        db.Sectors.Add(sn);
                        db.SaveChanges();
                        p++;

                    }
                }
                var sec = db.Sectors.Where(m => m.Pf_code == pfcode.Pf_code &&  (m.GEcreate != null || m.GEcreate == true)).ToList();
                foreach (var i in sec)
                {
                    GECrate ge = new GECrate();
                    ge.Slab1 = 1;
                    ge.Slab2 = 1;
                    ge.Slab3 = 1;
                    ge.Slab4 = 1;
                    ge.Uptosl1 = 1;
                    ge.Uptosl2 = 1;
                    ge.Uptosl3 = 1;
                    ge.Uptosl4 = 1;
                    ge.Company_id = Idd;
                    ge.NoOfSlab = 2;
                    ge.Sector_Id = i.Sector_Id;
                    ge.SectorName = i.Sector_Name;
                    db.GECrates.Add(ge);
                    db.SaveChanges();
                }

            }
            if (ecommerces.Count == 0)
            {
                foreach (var i in secotrs)
                {
                    NewDtdc_Ecommerce rm = new NewDtdc_Ecommerce();
                    rm.EcomPslab1 = 1;
                    rm.EcomPslab2 = 1;
                    rm.EcomPslab3 = 1;
                    rm.EcomPslab4 = 1;

                    rm.EcomGEslab1 = 1;
                    rm.EcomGEslab2 = 1;
                    rm.EcomGEslab3 = 1;
                    rm.EcomGEslab4 = 1;

                    rm.EcomPupto1 = 1;
                    rm.EcomPupto2 = 1;
                    rm.EcomPupto3 = 1;
                    rm.EcomPupto4 = 1;

                    rm.EcomGEupto1 = 1;
                    rm.EcomGEupto2 = 1;
                    rm.EcomGEupto3 = 1;
                    rm.EcomGEupto4 = 1;

                    rm.NoOfSlabN = 2;
                    rm.NoOfSlabS = 2;
                    rm.Company_id = Idd;
                    rm.Sector_Id = i.Sector_Id;
                    db.NewDtdc_Ecommerce.Add(rm);
                    db.SaveChanges();
                }
            }
            if (pfcode.Priorities.Count == 0)
            {
                foreach (var i in secotrs)
                {

                    pri.Company_id = Idd;
                    pri.Sector_Id = i.Sector_Id;
                    pri.prinoofslab = 2;

                    pri.prislab1 = prio[j].prislab1;
                    pri.prislab2 = prio[j].prislab2;
                    pri.prislab3 = prio[j].prislab3;
                    pri.prislab4 = prio[j].prislab4;

                    pri.priupto1 = prio[j].priupto1;
                    pri.priupto2 = prio[j].priupto2;
                    pri.priupto3 = prio[j].priupto3;
                    pri.priupto4 = prio[j].priupto4;

                    db.Priorities.Add(pri);
                    db.SaveChanges();

                    j++;
                }

            }

            int k = 0;

            if (pfcode.RateLaptops.Count == 0)
            {
                foreach (var i in secotrs)
                {

                    lap.Company_id = Idd;
                    lap.Sector_Id = i.Sector_Id;
                    lap.NoOfSlabN = 2;
                    lap.NoOfSlabS = 2;

                    lap.Aslab1 = laps[k].Aslab1;
                    lap.Aslab2 = laps[k].Aslab2;
                    lap.Aslab3 = laps[k].Aslab3;
                    lap.Aslab4 = laps[k].Aslab4;


                    lap.Sslab1 = laps[k].Sslab1;
                    lap.Sslab2 = laps[k].Sslab2;
                    lap.Sslab3 = laps[k].Sslab3;
                    lap.Sslab4 = laps[k].Sslab4;

                    lap.AUptosl1 = laps[k].AUptosl1;
                    lap.AUptosl2 = laps[k].AUptosl2;
                    lap.AUptosl3 = laps[k].AUptosl3;
                    lap.AUptosl4 = laps[k].AUptosl4;

                    lap.SUptosl1 = laps[k].SUptosl1;
                    lap.SUptosl2 = laps[k].SUptosl2;
                    lap.SUptosl3 = laps[k].SUptosl3;
                    lap.SUptosl4 = laps[k].SUptosl4;

                    db.RateLaptops.Add(lap);
                    db.SaveChanges();

                    k++;
                }

            }
            int l = 0;

            if (pfcode.RateRevLaptops.Count == 0)
            {
                foreach (var i in secotrs)
                {

                    revlap.Company_id = Idd;
                    revlap.Sector_Id = i.Sector_Id;
                    revlap.NoOfSlabN = 2;
                    revlap.NoOfSlabS = 2;

                    revlap.Aslab1 = revlaps[l].Aslab1;
                    revlap.Aslab2 = revlaps[l].Aslab2;
                    revlap.Aslab3 = revlaps[l].Aslab3;
                    revlap.Aslab4 = revlaps[l].Aslab4;


                    revlap.Sslab1 = revlaps[l].Sslab1;
                    revlap.Sslab2 = revlaps[l].Sslab2;
                    revlap.Sslab3 = revlaps[l].Sslab3;
                    revlap.Sslab4 = revlaps[l].Sslab4;

                    revlap.AUptosl1 = revlaps[l].AUptosl1;
                    revlap.AUptosl2 = revlaps[l].AUptosl2;
                    revlap.AUptosl3 = revlaps[l].AUptosl3;
                    revlap.AUptosl4 = revlaps[l].AUptosl4;

                    revlap.SUptosl1 = revlaps[l].SUptosl1;
                    revlap.SUptosl2 = revlaps[l].SUptosl2;
                    revlap.SUptosl3 = revlaps[l].SUptosl3;
                    revlap.SUptosl4 = revlaps[l].SUptosl4;

                    db.RateRevLaptops.Add(revlap);
                    db.SaveChanges();

                    l++;
                }

            }

         //   int jj = 0;

            //if (pfcode.Dtdc_Ecommerce.Count == 0)
            //{
            //    foreach (var i in secotrs)
            //    {

            //        ecom.Company_id = Idd;
            //        ecom.Sector_Id = i.Sector_Id;

            //        ecom.PUpto500gm = dtecom[jj].PUpto500gm;
            //        ecom.PAdd500gm = dtecom[jj].PAdd500gm;
            //        ecom.PAdd5kg = dtecom[jj].PAdd5kg;
            //        ecom.GEUpto500gm = dtecom[jj].GEUpto500gm;
            //        ecom.GEAdd500gm = dtecom[jj].GEAdd500gm;
            //        ecom.GEAdd5kg = dtecom[jj].GEAdd5kg;


            //        db.Dtdc_Ecommerce.Add(ecom);
            //        db.SaveChanges();

            //        jj++;
            //    }

            //}

            return RedirectToAction("Index", "RateMaster", new { id = Id });
        }




        public ActionResult AddCompany()
        {
            ViewBag.Pf_code = new SelectList(db.Franchisees.Where(x => !x.PF_Code.ToUpper().StartsWith("PR")), "PF_Code", "PF_Code");


            List<SelectListItem> items1 = new List<SelectListItem>();

            items1.Add(new SelectListItem { Text = "--Select--", Value = "--Select--" });
            items1.Add(new SelectListItem { Text = "IsRegister", Value = "IsRegister" });
            items1.Add(new SelectListItem { Text = "NotRegister", Value = "NotRegister" });


            ViewBag.IsRegister = items1;


            List<SelectListItem> itemsproduct = new List<SelectListItem>();

            itemsproduct.Add(new SelectListItem { Text = "Dox", Value = "Ratem" });

            itemsproduct.Add(new SelectListItem { Text = "NonDox", Value = "NonDox" });

            itemsproduct.Add(new SelectListItem { Text = "DTDCPLus", Value = "DTDCPLus" });

            itemsproduct.Add(new SelectListItem { Text = "DTDCPTP", Value = "DTDCPTP" });

            itemsproduct.Add(new SelectListItem { Text = "ExpressCargo", Value = "ExpressCargo" });

            itemsproduct.Add(new SelectListItem { Text = "Priority", Value = "Priority" });

            itemsproduct.Add(new SelectListItem { Text = "Laptops", Value = "Laptops" });

            itemsproduct.Add(new SelectListItem { Text = "RevPickupLaptops", Value = "RevPickupLaptops" });

            itemsproduct.Add(new SelectListItem { Text = "Ecommerce", Value = "Ecommerce" });
            itemsproduct.Add(new SelectListItem { Text = "GECMode", Value = "GECMode" });


            ViewBag.Producttype = itemsproduct;

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [HandleError]
        public ActionResult RatemasterCompany(Company empmodel,AddlogoModel logo, float[] slab1arr, float[] slab2arr, float[] slab3arr, float[] slab4arr, float[] Upto, int[] Sector_Id, int? only, string selected_tab,string IsRegister, string[] Producttype)
        {
            ViewBag.sr = db.Sectors.ToList();

            
                var abc = db.Companies.Where(m => m.Company_Id.ToLower() == empmodel.Company_Id.ToLower().Trim()).FirstOrDefault();

            if (abc != null)
            {
                ModelState.AddModelError("C_IdError", "Company Id Already Exist");

            }

            //if (logo.file == null )
            //{
            //    ModelState.AddModelError("C_DocError", "Please Select File");
               

            //}

            if (empmodel.Remark == null)
            {
               
                ModelState.AddModelError("C_codeError", "Please Enter Company Code");
            }

            if (empmodel.ProductType == null)
            {

                ModelState.AddModelError("C_proError", "Please Select Product Type");
            }

            var r = new Regex(@"([a-zA-Z0-9\s_\\.\-:])+(.pdf|.doc|.docx|.DOC|.DOCX)$");

            if (logo.file != null)
            {
                if (!r.IsMatch(logo.file.FileName))
                {
                    ModelState.AddModelError("C_DocError", "Only PDF/Word files allowed.");
                    //TempData["Success"] = "Only PDF files allowed!";
                }
            }
            //Take PfCode From Session//
            if (ModelState.IsValid)
            {
               
                // Business Logic
                ViewBag.Message = "Success or Failure Message";
                ModelState.Clear();
                TempData["CompanyId"] = empmodel.Company_Id.Trim();

                string _FileName = "";
                string _path = "";

                if (logo.file != null)
                {
                    _FileName = Path.GetFileName(logo.file.FileName);
                    _path = Server.MapPath("~/UploadedSoftCopy/") + _FileName;
                    logo.file.SaveAs(_path);
                    var imagepath  = Path.Combine(Server.MapPath("~/UploadedSoftCopy/") + _FileName);
                    empmodel.DocumentFilepath = _path;
                }

                var result = string.Join(",", Producttype);
                empmodel.ProductType = result;

                empmodel.IsAgreementoption = 0; // For agreement Option
                
                db.Companies.Add(empmodel);
                db.SaveChanges();
               

                var secotrs = db.Sectors.Where(m => m.Pf_code == empmodel.Pf_code && (m.GEcreate == null || m.GEcreate == false)).ToList();            

               

                var basicdox = db.Ratems.Where(m => m.Company_id == "BASIC_TS").ToArray();
                var basicnon = db.Nondoxes.Where(m => m.Company_id == "BASIC_TS").ToArray();
                var express = db.express_cargo.Where(m => m.Company_id == "BASIC_TS").ToArray();
                var prio = db.Priorities.Where(m => m.Company_id == "BASIC_TS").ToArray();
                var lapt = db.RateLaptops.Where(m => m.Company_id == "BASIC_TS").ToArray();
                var revlapt = db.RateRevLaptops.Where(m => m.Company_id == "BASIC_TS").ToArray();
                var basicecom = db.NewDtdc_Ecommerce.Where(m => m.Company_id == "BASIC_TS").ToArray();

                int j = 0;

                foreach (var i in secotrs)
                {
                    Ratem dox = new Ratem();
                    Nondox ndox = new Nondox();
                    express_cargo cs = new express_cargo();
                    Priority pri = new Priority();
                    RateLaptop lapto = new RateLaptop();
                    RateRevLaptop revlapto = new RateRevLaptop();
                    //New DTDC Ecommerce table is used
                    NewDtdc_Ecommerce ecom = new NewDtdc_Ecommerce();

                    dox.Company_id = empmodel.Company_Id.Trim();
                    dox.Sector_Id = i.Sector_Id;
                    dox.NoOfSlab = 2;

                    dox.slab1 = basicdox[j].slab1;
                    dox.slab2 = basicdox[j].slab2;
                    dox.slab3 = basicdox[j].slab3;
                    dox.slab4 = basicdox[j].slab4;

                    dox.Uptosl1 = basicdox[j].Uptosl1;
                    dox.Uptosl2 = basicdox[j].Uptosl2;
                    dox.Uptosl3 = basicdox[j].Uptosl3;
                    dox.Uptosl4 = basicdox[j].Uptosl4;

                    ndox.Company_id = empmodel.Company_Id.Trim();
                    ndox.Sector_Id = i.Sector_Id;
                    ndox.NoOfSlabN = 2;
                    ndox.NoOfSlabS = 2;

                    ndox.Aslab1 = basicnon[j].Aslab1;
                    ndox.Aslab2 = basicnon[j].Aslab2;
                    ndox.Aslab3 = basicnon[j].Aslab3;
                    ndox.Aslab4 = basicnon[j].Aslab4;


                    ndox.Sslab1 = basicnon[j].Sslab1;
                    ndox.Sslab2 = basicnon[j].Sslab2;
                    ndox.Sslab3 = basicnon[j].Sslab3;
                    ndox.Sslab4 = basicnon[j].Sslab4;

                    ndox.AUptosl1 = basicnon[j].AUptosl1;
                    ndox.AUptosl2 = basicnon[j].AUptosl2;
                    ndox.AUptosl3 = basicnon[j].AUptosl3;
                    ndox.AUptosl4 = basicnon[j].AUptosl4;

                    ndox.SUptosl1 = basicnon[j].SUptosl1;
                    ndox.SUptosl2 = basicnon[j].SUptosl2;
                    ndox.SUptosl3 = basicnon[j].SUptosl3;
                    ndox.SUptosl4 = basicnon[j].SUptosl4;

                    pri.Company_id = empmodel.Company_Id.Trim();
                    pri.Sector_Id = i.Sector_Id;
                    pri.prinoofslab = 2;

                    pri.prislab1 = prio[j].prislab1;
                    pri.prislab2 = prio[j].prislab2;
                    pri.prislab3 = prio[j].prislab3;
                    pri.prislab4 = prio[j].prislab4;

                    pri.priupto1 = prio[j].priupto1;
                    pri.priupto2 = prio[j].priupto2;
                    pri.priupto3 = prio[j].priupto3;
                    pri.priupto4 = prio[j].priupto4;



                    lapto.Company_id = empmodel.Company_Id.Trim();
                    lapto.Sector_Id = i.Sector_Id;
                    lapto.NoOfSlabN = 2;
                    lapto.NoOfSlabS = 2;

                    lapto.Aslab1 = lapt[j].Aslab1;
                    lapto.Aslab2 = lapt[j].Aslab2;
                    lapto.Aslab3 = lapt[j].Aslab3;
                    lapto.Aslab4 = lapt[j].Aslab4;


                    lapto.Sslab1 = lapt[j].Sslab1;
                    lapto.Sslab2 = lapt[j].Sslab2;
                    lapto.Sslab3 = lapt[j].Sslab3;
                    lapto.Sslab4 = lapt[j].Sslab4;

                    lapto.AUptosl1 = lapt[j].AUptosl1;
                    lapto.AUptosl2 = lapt[j].AUptosl2;
                    lapto.AUptosl3 = lapt[j].AUptosl3;
                    lapto.AUptosl4 = lapt[j].AUptosl4;

                    lapto.SUptosl1 = lapt[j].SUptosl1;
                    lapto.SUptosl2 = lapt[j].SUptosl2;
                    lapto.SUptosl3 = lapt[j].SUptosl3;
                    lapto.SUptosl4 = lapt[j].SUptosl4;


                    revlapto.Company_id = empmodel.Company_Id.Trim();
                    revlapto.Sector_Id = i.Sector_Id;
                    revlapto.NoOfSlabN = 2;
                    revlapto.NoOfSlabS = 2;

                    revlapto.Aslab1 = revlapt[j].Aslab1;
                    revlapto.Aslab2 = revlapt[j].Aslab2;
                    revlapto.Aslab3 = revlapt[j].Aslab3;
                    revlapto.Aslab4 = revlapt[j].Aslab4;


                    revlapto.Sslab1 = revlapt[j].Sslab1;
                    revlapto.Sslab2 = revlapt[j].Sslab2;
                    revlapto.Sslab3 = revlapt[j].Sslab3;
                    revlapto.Sslab4 = revlapt[j].Sslab4;

                    revlapto.AUptosl1 = revlapt[j].AUptosl1;
                    revlapto.AUptosl2 = revlapt[j].AUptosl2;
                    revlapto.AUptosl3 = revlapt[j].AUptosl3;
                    revlapto.AUptosl4 = revlapt[j].AUptosl4;

                    revlapto.SUptosl1 = revlapt[j].SUptosl1;
                    revlapto.SUptosl2 = revlapt[j].SUptosl2;
                    revlapto.SUptosl3 = revlapt[j].SUptosl3;
                    revlapto.SUptosl4 = revlapt[j].SUptosl4;

                    //New Updation of Ecommerce

                    //ecom.Company_id = empmodel.Company_Id.Trim();
                    //ecom.Sector_Id = i.Sector_Id;                
                    //ecom.PUpto500gm = basicecom[j].PUpto500gm;
                    //ecom.PAdd500gm = basicecom[j].PAdd500gm;
                    //ecom.PAdd5kg = basicecom[j].PAdd5kg;                 
                    //ecom.GEUpto500gm = basicecom[j].GEUpto500gm;
                    //ecom.GEAdd500gm = basicecom[j].GEAdd500gm;
                    //ecom.GEAdd5kg = basicecom[j].GEAdd5kg;
                    ecom.EcomPslab1 = 1;
                    ecom.EcomPslab2 = 1;
                    ecom.EcomPslab3 = 1;
                    ecom.EcomPslab4 = 1;

                    ecom.EcomGEslab1 = 1;
                    ecom.EcomGEslab2 = 1;
                    ecom.EcomGEslab3 = 1;
                    ecom.EcomGEslab4 = 1;

                    ecom.EcomPupto1 = 1;
                    ecom.EcomPupto2 = 1;
                    ecom.EcomPupto3 = 1;
                    ecom.EcomPupto4 = 1;

                    ecom.EcomGEupto1 = 1;
                    ecom.EcomGEupto2 = 1;
                    ecom.EcomGEupto3 = 1;
                    ecom.EcomGEupto4 = 1;

                    ecom.NoOfSlabN = 2;
                    ecom.NoOfSlabS = 2;
                    ecom.Company_id = empmodel.Company_Id.Trim();
                    ecom.Sector_Id = i.Sector_Id;
                    //db.NewDtdc_Ecommerce.Add(ecom);
                    //db.SaveChanges();

                    cs.Company_id = empmodel.Company_Id.Trim();
                    cs.Sector_Id = i.Sector_Id;

                    cs.Exslab1 = express[j].Exslab1;
                    cs.Exslab2 = express[j].Exslab2;

                   
                        db.Ratems.Add(dox);
                   
                        db.Nondoxes.Add(ndox);
                    
                        db.express_cargo.Add(cs);
                    
                        db.Priorities.Add(pri);
                   
                        db.RateLaptops.Add(lapto);
                   
                        db.RateRevLaptops.Add(revlapto);
                   
                        db.NewDtdc_Ecommerce.Add(ecom);
                   

                    j++;

                }

                int p = 0;

                var basicplu = db.dtdcPlus.Where(m => m.Company_id == "Basic_Ts").ToArray();
                var basicptp = db.Dtdc_Ptp.Where(m => m.Company_id == "Basic_Ts").ToArray();


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

                    dtplu.Company_id = empmodel.Company_Id.Trim();

                    dtplu.Upto500gm = basicplu[p].Upto500gm;
                    dtplu.U10to25kg = basicplu[p].U10to25kg;
                    dtplu.U25to50 = basicplu[p].U25to50;
                    dtplu.U50to100 = basicplu[p].U50to100;
                    dtplu.add100kg = basicplu[p].add100kg;
                    dtplu.Add500gm = basicplu[p].Add500gm;


                    stptp.Company_id = empmodel.Company_Id.Trim();
                    stptp.PUpto500gm = basicptp[p].PUpto500gm;
                    stptp.PAdd500gm = basicptp[p].PAdd500gm;
                    stptp.PU10to25kg = basicptp[p].PU10to25kg;
                    stptp.PU25to50 = basicptp[p].PU25to50;
                    stptp.Padd100kg = basicptp[p].Padd100kg;
                    stptp.PU50to100 = basicptp[p].PU50to100;

                    stptp.P2Upto500gm = basicptp[p].P2Upto500gm;
                    stptp.P2Add500gm = basicptp[p].P2Add500gm;
                    stptp.P2U10to25kg = basicptp[p].P2U10to25kg;
                    stptp.P2U25to50 = basicptp[p].P2U25to50;
                    stptp.P2add100kg = basicptp[p].P2add100kg;
                    stptp.P2U50to100 = basicptp[p].P2U50to100;

                   
                        db.dtdcPlus.Add(dtplu);
                   
                        db.Dtdc_Ptp.Add(stptp);
                   
                    p++;

                }


                db.SaveChanges();
                string[] sectornamelist = new string[]
               {
                      "CENTRAL I",
                      "CENTRAL II",
                      "EAST I",
                      "EAST II",
                      "NORTH EAST I",
                      "NORTH EAST II",
                      "NORTH EAST III",
                      "NORTH I",
                      "NORTH II",
                      "NORTH III",
                      "SOUTH I",
                      "SOUTH II",
                      "SOUTH III",
                      "WEST I",
                      "WEST II"
               };

                var sector = db.Sectors.Where(m => m.Pf_code == empmodel.Pf_code && m.GEcreate==true ).ToList();
                if (sector.Count == 0)
                {
                    var pt= 1;
                    foreach (var i in sectornamelist)
                    {
                        Sector sn = new Sector();

                        sn.Pf_code = empmodel.Pf_code;
                        sn.CashN = false;
                        sn.BillD = false;
                        sn.BillN = false;
                        sn.Sector_Name = i;
                        sn.Priority = pt;
                        sn.GEcreate = true;

                        db.Sectors.Add(sn);
                        db.SaveChanges();
                        pt++;

                    }
                  

                }

                var sec = db.Sectors.Where(m => m.Pf_code ==empmodel.Pf_code && m.GEcreate == true).ToList();
                if (sec.Count >= 1)
                {
                    foreach (var i in sec)
                    {
                        GECrate ge = new GECrate();
                        ge.Slab1 = 1;
                        ge.Slab2 = 1;
                        ge.Slab3 = 1;
                        ge.Slab4 = 1;
                        ge.Uptosl1 = 1;
                        ge.Uptosl2 = 1;
                        ge.Uptosl3 = 1;
                        ge.Uptosl4 = 1;
                        ge.Company_id = empmodel.Company_Id;
                        ge.NoOfSlab = 2;
                        ge.Sector_Id = i.Sector_Id;
                        ge.SectorName = i.Sector_Name;
                        db.GECrates.Add(ge);
                        db.SaveChanges();
                    }
                }
             
                @ViewBag.Slabs = db.Ratems.Where(m => m.Company_id == empmodel.Company_Id.Trim()).FirstOrDefault();                

                ViewBag.Company = new Company();

                ViewBag.Dox = db.Ratems.Where(m => m.Company_id == empmodel.Company_Id.Trim()).ToList();

                ViewBag.Pri = db.Priorities.Where(m => m.Company_id == empmodel.Company_Id.Trim()).ToList();
                //ViewBag.SuccessCompany = "Company Added SuccessFully"; To Open Next Tab

               
                double? selectedval = db.Companies.Where(m => m.Company_Id == empmodel.Company_Id.Trim()).Select(m => m.Minimum_Risk_Charge).FirstOrDefault();


                List<SelectListItem> items = new List<SelectListItem>();

                items.Add(new SelectListItem { Text = "--Select--", Value = "--Select--" });
                items.Add(new SelectListItem { Text = "IsRegister", Value = "IsRegister" });
                items.Add(new SelectListItem { Text = "NotRegister", Value = "NotRegister" });
               
  
                ViewBag.IsRegister = items;



                List<SelectListItem> itemsproduct = new List<SelectListItem>();

                itemsproduct.Add(new SelectListItem { Text = "Dox", Value = "Ratem" });

                itemsproduct.Add(new SelectListItem { Text = "NonDox", Value = "NonDox" });

                itemsproduct.Add(new SelectListItem { Text = "DTDCPLus", Value = "DTDCPLus" });

                itemsproduct.Add(new SelectListItem { Text = "DTDCPTP", Value = "DTDCPTP" });

                itemsproduct.Add(new SelectListItem { Text = "ExpressCargo", Value = "ExpressCargo" });

                itemsproduct.Add(new SelectListItem { Text = "Priority", Value = "Priority" });

                itemsproduct.Add(new SelectListItem { Text = "Laptops", Value = "Laptops" });

                itemsproduct.Add(new SelectListItem { Text = "RevPickupLaptops", Value = "RevPickupLaptops" });

                itemsproduct.Add(new SelectListItem { Text = "Ecommerce", Value = "Ecommerce" });
                itemsproduct.Add(new SelectListItem { Text = "GECMode", Value = "GECMode" });

                ViewBag.Producttype = itemsproduct;


                return RedirectToAction("Index","RateMaster",new { id = empmodel.Company_Id.Trim()} );


            }

            ViewBag.Company = new Company();

            //ViewBag.SuccessCompany = "Company Failed";To Ramain On Same Tab


            double? selectedval1 = db.Companies.Where(m => m.Company_Id == empmodel.Company_Id.Trim()).Select(m => m.Minimum_Risk_Charge).FirstOrDefault();


            List<SelectListItem> itemsreg = new List<SelectListItem>();

            itemsreg.Add(new SelectListItem { Text = "--Select--", Value = "--Select--" });
            itemsreg.Add(new SelectListItem { Text = "IsRegister", Value = "IsRegister" });
            itemsreg.Add(new SelectListItem { Text = "NotRegister", Value = "NotRegister" });


            ViewBag.IsRegister = itemsreg;



            List<SelectListItem> itemsproduct1 = new List<SelectListItem>();

            itemsproduct1.Add(new SelectListItem { Text = "Dox", Value = "Ratem" });

            itemsproduct1.Add(new SelectListItem { Text = "NonDox", Value = "NonDox" });

            itemsproduct1.Add(new SelectListItem { Text = "DTDCPLus", Value = "DTDCPLus" });

            itemsproduct1.Add(new SelectListItem { Text = "DTDCPTP", Value = "DTDCPTP" });

            itemsproduct1.Add(new SelectListItem { Text = "ExpressCargo", Value = "ExpressCargo" });

            itemsproduct1.Add(new SelectListItem { Text = "Priority", Value = "Priority" });

            itemsproduct1.Add(new SelectListItem { Text = "Laptops", Value = "Laptops" });

            itemsproduct1.Add(new SelectListItem { Text = "RevPickupLaptops", Value = "RevPickupLaptops" });

            itemsproduct1.Add(new SelectListItem { Text = "Ecommerce", Value = "Ecommerce" });
            itemsproduct1.Add(new SelectListItem { Text = "GECMode", Value = "GECMode" });

            ViewBag.Producttype = itemsproduct1;


            List<SelectListItem> items1 = new List<SelectListItem>();

            items1.Add(new SelectListItem { Text = "0", Value = "0" });
            items1.Add(new SelectListItem { Text = "50", Value = "50" });
            items1.Add(new SelectListItem { Text = "100", Value = "100" });

            if (selectedval1 == null)
            {
                var selected = items1.Where(x => x.Value == "0").First();
                selected.Selected = true;
            }
            else
            {
              

                var selected = items1.Where(x => x.Value == selectedval1.ToString()).First();
                selected.Selected = true;
            }

            ViewBag.Minimum_Risk_Charge = items1;



            ViewBag.Dox = db.Ratems.Where(m => m.Company_id == "Bala").ToList();

            ViewBag.Pf_code = new SelectList(db.Franchisees.Where(x => !x.PF_Code.ToUpper().StartsWith("PR")), "PF_Code", "PF_Code",empmodel.Pf_code);

            return View("AddCompany", empmodel);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        [HandleError]
        public ActionResult RatemasterEditCompany(Company empmodel, float[] slab1arr, float[] slab2arr, float[] slab3arr, float[] slab4arr, float[] Upto, int[] Sector_Id, int? only, string selected_tab,string Remark, string[] Producttype)
        {
            ViewBag.sr = db.Sectors.ToList();


            List<SelectListItem> itemsproduct = new List<SelectListItem>();

            itemsproduct.Add(new SelectListItem { Text = "Dox", Value = "Ratem" });

            itemsproduct.Add(new SelectListItem { Text = "NonDox", Value = "NonDox" });

            itemsproduct.Add(new SelectListItem { Text = "DTDCPLus", Value = "DTDCPLus" });

            itemsproduct.Add(new SelectListItem { Text = "DTDCPTP", Value = "DTDCPTP" });

            itemsproduct.Add(new SelectListItem { Text = "ExpressCargo", Value = "ExpressCargo" });

            itemsproduct.Add(new SelectListItem { Text = "Priority", Value = "Priority" });

            itemsproduct.Add(new SelectListItem { Text = "Laptops", Value = "Laptops" });

            itemsproduct.Add(new SelectListItem { Text = "RevPickupLaptops", Value = "RevPickupLaptops" });

            itemsproduct.Add(new SelectListItem { Text = "Ecommerce", Value = "Ecommerce" });
            itemsproduct.Add(new SelectListItem { Text = "GECMode", Value = "GECMode" });


            var types = db.Companies.Where(m => m.Company_Id == empmodel.Company_Id.Trim()).Select(m => m.ProductType).FirstOrDefault();

            if (types != null)
            {
                string[] split = types.Split(',');

                foreach (var item in itemsproduct)
                {
                    if (split.Contains(item.Value))
                    {
                        item.Selected = true;

                    }
                }
            }
            ViewBag.Producttype = itemsproduct;

            var abc = db.Companies.Where(m => m.Company_Id.ToLower() == empmodel.Company_Id.ToLower().Trim()).FirstOrDefault();

            foreach (ModelState modelState in ViewData.ModelState.Values)
            {
                foreach (ModelError error in modelState.Errors)
                {
                    Console.WriteLine(error.ErrorMessage);
                }
            }

            foreach (ModelState modelState in ViewData.ModelState.Values)
            {
                foreach (ModelError error in modelState.Errors)
                {
                    Console.WriteLine(error.ErrorMessage);
                }
            }

            if (empmodel.ProductType == null)
            {

                ModelState.AddModelError("C_proError", "Please Select Product Type");
            }

            if (Remark == "")
            {
                //ViewBag.Message = "Please Enter Remark";

                ModelState.AddModelError("remarkError", "Please Enter Remark");

            }
            else
            { 
            if (ModelState.IsValid)
            {


                // Business Logic
                ViewBag.Message = "Sucess or Failure Message";
                ModelState.Clear();
                TempData["CompanyId"] = empmodel.Company_Id.Trim();
       
                empmodel.IsAgreementoption = 0; // For agreement Option
                empmodel.Remark = Remark;

                    var result = string.Join(",", Producttype);
                    empmodel.ProductType = result;
                    empmodel.DocumentFilepath = abc.DocumentFilepath;

                    var local = db.Companies.Where(m => m.Company_Id == empmodel.Company_Id.Trim()).FirstOrDefault();

                if (local != null)
                {
                    db.Entry(local).State = EntityState.Detached;
                }


                db.Entry(empmodel).State = EntityState.Modified;
                db.SaveChanges();

                //<-------------risk surch charge dropdown--------------->

                double? selectedval = db.Companies.Where(m => m.Company_Id == empmodel.Company_Id.Trim()).Select(m => m.Minimum_Risk_Charge).FirstOrDefault();


                //List<SelectListItem> items = new List<SelectListItem>();

                //items.Add(new SelectListItem { Text = "0", Value = "0" });
                //items.Add(new SelectListItem { Text = "50", Value = "50" });
                //items.Add(new SelectListItem { Text = "100", Value = "100" });

                //if (selectedval == null)
                //{
                //    var selected = items.Where(x => x.Value == "0").First();
                //    selected.Selected = true;
                //}
                //else
                //{


                //    var selected = items.Where(x => x.Value == selectedval.ToString()).First();
                //    selected.Selected = true;
                //}

                List<SelectListItem> items = new List<SelectListItem>();

                items.Add(new SelectListItem { Text = "--Select--", Value = "--Select--" });
                items.Add(new SelectListItem { Text = "IsRegister", Value = "IsRegister" });
                items.Add(new SelectListItem { Text = "NotRegister", Value = "NotRegister" });


                    ViewBag.IsRegister = items;

                    ViewBag.Minimum_Risk_Charge = local.Minimum_Risk_Charge;

                    //<-------------risk surch charge dropdown--------------->
                    //updating all tables pf code

                    int[] secotrs = db.Sectors.Where(m => m.Pf_code == empmodel.Pf_code && (m.GEcreate == null || m.GEcreate == false)).Select(m=>m.Sector_Id).ToArray();
                    int[] GecSectors = db.Sectors.Where(m => m.Pf_code == empmodel.Pf_code && m.GEcreate == true).Select(x => x.Sector_Id).ToArray();

                int [] doxlist = db.Ratems.Where(m => m.Company_id == empmodel.Company_Id.Trim()).Select(m => m.Rete_Id).ToArray();
                int [] nonlist = db.Nondoxes.Where(m => m.Company_id == empmodel.Company_Id.Trim()).Select(m => m.Non_ID).ToArray();
                int [] cslist = db.express_cargo.Where(m => m.Company_id == empmodel.Company_Id.Trim()).Select(m => m.Exp_Id).ToArray();
                int[] lapslist = db.RateLaptops.Where(m => m.Company_id == empmodel.Company_Id.Trim()).Select(m => m.RateLaptop_ID).ToArray();
                int[] revlapslist = db.RateRevLaptops.Where(m => m.Company_id == empmodel.Company_Id.Trim()).Select(m => m.RateRevLaptop_ID).ToArray();
               
               int[] ecomlist = db.NewDtdc_Ecommerce.Where(m => m.Company_id == empmodel.Company_Id.Trim()).Select(m => m.Ecom_id).ToArray();

                    long[] geclist = db.GECrates.Where(m => m.Company_id == empmodel.Company_Id.Trim()).Select(m => m.GECrateId).ToArray();
                    int j = 0;

                 int cnt = doxlist.Count();
                    int geccount = GecSectors.Count();
                    for(int i = 0; i < geccount; i++)
                    {
                        GECrate geccreate = new GECrate();
                        int ge = Convert.ToInt32(geclist[i]);
                        geccreate = db.GECrates.Where(x =>x.GECrateId==ge).FirstOrDefault();
                        geccreate.Sector_Id = GecSectors[i];
                        db.Entry(geccreate).State = EntityState.Modified;
                        db.SaveChanges();


                    }
                    for (int i=0;i < cnt;i++)
                {                 

                    Ratem dox = new Ratem();
                    Nondox ndox = new Nondox();
                    express_cargo cs = new express_cargo();
                    RateLaptop ralaps = new RateLaptop();
                    RateRevLaptop revralaps = new RateRevLaptop();
                    NewDtdc_Ecommerce ecom = new NewDtdc_Ecommerce();
                        int d = doxlist[i], n = nonlist[i], ex=cslist[i],la=lapslist[i], revla = revlapslist[i], ec = ecomlist[i]; 

                    dox = db.Ratems.Where(m => m.Rete_Id == d).FirstOrDefault();
                    ndox = db.Nondoxes.Where(m => m.Non_ID ==n).FirstOrDefault();
                    cs = db.express_cargo.Where(m => m.Exp_Id == ex).FirstOrDefault();
                    ralaps = db.RateLaptops.Where(m => m.RateLaptop_ID == la).FirstOrDefault();
                    revralaps = db.RateRevLaptops.Where(m => m.RateRevLaptop_ID == revla).FirstOrDefault();
                        ecom = db.NewDtdc_Ecommerce.Where(m => m.Ecom_id == ec).FirstOrDefault();
                    dox.Sector_Id = secotrs[i];
                    ndox.Sector_Id = secotrs[i];
                    cs.Sector_Id = secotrs[i];
                    ralaps.Sector_Id = secotrs[i];
                    revralaps.Sector_Id = secotrs[i];
                        ecom.Sector_Id= secotrs[i];
                        db.Entry(dox).State = EntityState.Modified;
                    db.Entry(ndox).State = EntityState.Modified;
                    db.Entry(cs).State = EntityState.Modified;
                    db.Entry(ralaps).State = EntityState.Modified;
                    db.Entry(revralaps).State = EntityState.Modified;
                    db.Entry(ecom).State = EntityState.Modified;

                        db.SaveChanges();

                    j++;

                }


              
              

                ViewBag.Message = "Company Updated SuccessFully";

                @ViewBag.Slabs = db.Ratems.Where(m => m.Company_id == empmodel.Company_Id.Trim()).FirstOrDefault();

                ViewBag.Company = new Company();

                ViewBag.Dox = db.Ratems.Where(m => m.Company_id == "Bala").ToList();

                ViewBag.SuccessCompany = "Company Added SuccessFully";

                ViewBag.Pf_code = new SelectList(db.Franchisees.Where(x => !x.PF_Code.ToUpper().StartsWith("PR")), "PF_Code", "PF_Code", empmodel.Pf_code);

                return PartialView("RateMasterEditCompany", empmodel);

            }

          }
            double? selectedval1 = db.Companies.Where(m => m.Company_Id == empmodel.Company_Id.Trim()).Select(m => m.Minimum_Risk_Charge).FirstOrDefault();


            //List<SelectListItem> items1 = new List<SelectListItem>();

            //items1.Add(new SelectListItem { Text = "0", Value = "0" });
            //items1.Add(new SelectListItem { Text = "50", Value = "50" });
            //items1.Add(new SelectListItem { Text = "100", Value = "100" });

            //if (selectedval1 == null)
            //{
            //    var selected = items1.Where(x => x.Value == "0").First();
            //    selected.Selected = true;
            //}
            //else
            //{
            //    //foreach (var item in items)
            //    //{
            //    //    if (item.Value == selectedval.ToString())
            //    //    {
            //    //        item.Selected = true;
            //    //        break;
            //    //    }
            //    //}

            //    var selected = items1.Where(x => x.Value == selectedval1.ToString()).First();
            //    selected.Selected = true;
            //}

            //ViewBag.Minimum_Risk_Charge = items1;

            ViewBag.Company = new Company();

            //ViewBag.SuccessCompany = "Company Failed";To Ramain On Same Tab

            ViewBag.Dox = db.Ratems.Where(m => m.Company_id == "Bala").ToList();

            ViewBag.Pf_code = new SelectList(db.Franchisees.Where(x => !x.PF_Code.ToUpper().StartsWith("PR")), "PF_Code", "PF_Code", empmodel.Pf_code);

            List<SelectListItem> items1 = new List<SelectListItem>();

            items1.Add(new SelectListItem { Text = "--Select--", Value = "--Select--" });
            items1.Add(new SelectListItem { Text = "IsRegister", Value = "IsRegister" });
            items1.Add(new SelectListItem { Text = "NotRegister", Value = "NotRegister" });


            ViewBag.IsRegister = items1;


            return PartialView("RatemasterEditCompany", empmodel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [HandleError]
        public ActionResult RatemasterDox(int? only, FormCollection fc, float[] slab1, string comppid)
        {
            comppid = comppid.Replace("__", "&").Replace("xdotx", "."); ;
            var CompanyId = comppid;
            ViewBag.companyid = comppid;
            //if (only == 2)
            //{
            //    //tO clear array//Array.Clear(slab3arr, 0, slab3arr.Length);
            //    //tO clear array//Array.Clear(slab4arr, 0, slab3arr.Length);
            //} 
            //if (only == 2)
            //{
            //    //tO clear array//Array.Clear(slab4arr, 0, slab3arr.Length);
            //}



            ViewBag.Dox = db.Ratems.Where(m => m.Company_id == CompanyId).ToList();

            ViewBag.NonDox = db.Nondoxes.Where(m => m.Company_id == CompanyId).ToList();


            if (ModelState.IsValid)
            {

                var rateidarray = fc.GetValues("item.Rete_Id");
                var slab1arayy = fc.GetValues("item.slab1");
                var slab2arayy = fc.GetValues("item.slab2");
                var slab3arayy = fc.GetValues("item.slab3");
                var slab4arayy = fc.GetValues("item.slab4");
                var uptoarray = fc.GetValues("Upto");
                var noofslab = fc.GetValues("item.NoOfSlab");

                var sectoridarray = fc.GetValues("item.Sector_Id");

                for (int i = 0; i < rateidarray.Count(); i++)
                {
                    if (slab1arayy[i] == "")
                    {
                        slab1arayy[i] = "0";
                    }
                    if (slab2arayy[i] == "")
                    {
                        slab2arayy[i] = "0";
                    }
                    if (slab3arayy[i] == "")
                    {
                        slab3arayy[i] = "0";
                    }
                    if (slab4arayy[i] == "")
                    {
                        slab4arayy[i] = "0";
                    }
                }
                for (int i = 0; i < uptoarray.Count(); i++)
                {
                    if (uptoarray[i] == "")
                    {
                        uptoarray[i] = "0";
                    }
                }



                for (int i = 0; i < rateidarray.Count(); i++)
                {

                    Ratem rm = db.Ratems.Find(Convert.ToInt16(rateidarray[i]));

                    rm.slab1 = Convert.ToDouble(slab1arayy[i]);
                    rm.slab2 = Convert.ToDouble(slab2arayy[i]);
                    rm.slab3 = Convert.ToDouble(slab3arayy[i]);
                    rm.slab4 = Convert.ToDouble(slab4arayy[i]);
                    rm.Uptosl1 = Convert.ToDouble(uptoarray[0]);
                    rm.Uptosl2 = Convert.ToDouble(uptoarray[1]);
                    rm.Uptosl3 = Convert.ToDouble(uptoarray[2]);
                    rm.Uptosl4 = Convert.ToDouble(uptoarray[3]);
                    rm.Sector_Id = Convert.ToInt16(sectoridarray[i]);
                    rm.NoOfSlab = Convert.ToInt16(noofslab[0]);
                    rm.Company_id = CompanyId;




                    db.Entry(rm).State = EntityState.Modified;
                    db.SaveChanges();


                }

                var compid = comppid;

                ViewBag.Message = "Dox Updated SuccessFully";

                @ViewBag.Slabs = db.Ratems.Where(m => m.Company_id == compid).FirstOrDefault();

                return PartialView("RatemasterDox", db.Ratems.Where(m => m.Company_id == compid &&  m.Sector.BillD == true).OrderBy(m => m.Sector.Priority).ToList());
            }
            return PartialView("RatemasterDox", fc);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [HandleError]
        public ActionResult RateMasterGEC(int? only, FormCollection fc, float[] slab1, string comppid)
        {
            comppid = comppid.Replace("__", "&").Replace("xdotx", "."); ;
            var CompanyId = comppid;
            var company = db.Companies.Where(x => x.Company_Id == comppid).FirstOrDefault();
            ViewBag.companyid = comppid;
           
            if (ModelState.IsValid)
            {

                var rateidarray = fc.GetValues("item.GECrateId");
                var slab1arayy = fc.GetValues("item.Slab1");
                var slab2arayy = fc.GetValues("item.Slab2");
                var slab3arayy = fc.GetValues("item.Slab3");
                var slab4arayy = fc.GetValues("item.Slab4");
                var uptoarray = fc.GetValues("Upto");
                var noofslab = fc.GetValues("item.NoOfSlab");

                var sectoridarray = fc.GetValues("item.Sector_Id");

                for (int i = 0; i < rateidarray.Count(); i++)
                {
                    if (slab1arayy[i] == "")
                    {
                        slab1arayy[i] = "0";
                    }
                    if (slab2arayy[i] == "")
                    {
                        slab2arayy[i] = "0";
                    }
                    if (slab3arayy[i] == "")
                    {
                        slab3arayy[i] = "0";
                    }
                    if (slab4arayy[i] == "")
                    {
                        slab4arayy[i] = "0";
                    }
                }
                for (int i = 0; i < uptoarray.Count(); i++)
                {
                    if (uptoarray[i] == "")
                    {
                        uptoarray[i] = "0";
                    }
                }



                for (int i = 0; i < rateidarray.Count(); i++)
                {

                    GECrate rm = db.GECrates.Find(Convert.ToInt16(rateidarray[i]));

                    rm.Slab1 = Convert.ToDouble(slab1arayy[i]);
                    rm.Slab2 = Convert.ToDouble(slab2arayy[i]);
                    rm.Slab3 = Convert.ToDouble(slab3arayy[i]);
                    rm.Slab4 = Convert.ToDouble(slab4arayy[i]);
                    rm.Uptosl1 = Convert.ToDouble(uptoarray[0]);
                    rm.Uptosl2 = Convert.ToDouble(uptoarray[1]);
                    rm.Uptosl3 = Convert.ToDouble(uptoarray[2]);
                    rm.Uptosl4 = Convert.ToDouble(uptoarray[3]);
                   
                    rm.NoOfSlab = Convert.ToInt16(noofslab[0]);
                    rm.Company_id = CompanyId;
                    rm.Sector_Id = Convert.ToInt32(sectoridarray[i]);
                    


                    db.Entry(rm).State = EntityState.Modified;
                    db.SaveChanges();


                }

                var compid = comppid;

                ViewBag.Message = "GEC updated successfully";

                var companyPfCode = company.Pf_code;

                var getGEC = (from GECr in db.GECrates

                              join s in db.Sectors on GECr.Sector_Id equals s.Sector_Id
                              where GECr.Company_id == CompanyId
                                    && s.Pf_code == companyPfCode
                                    && s.GEcreate == true
                              // Directly checking if s.GECrates is true
                              select new
                              {
                                  SectorName = s.Sector_Name,
                                  GECrateId = GECr.GECrateId,
                                  Slab1 = GECr.Slab1,
                                  Slab2 = GECr.Slab2,
                                  Slab3 = GECr.Slab3,
                                  Slab4 = GECr.Slab4,
                                  Uptosl1 = GECr.Uptosl1,
                                  Uptosl2 = GECr.Uptosl2,
                                  Uptosl3 = GECr.Uptosl3,
                                  Uptosl4 = GECr.Uptosl4,
                                  Company_id = GECr.Company_id,
                                  NoOfSlab = GECr.NoOfSlab,
                                  Priority = s.Priority // Include Priority for ordering
                              })
              .ToList() // Execute the query and bring the results into memory
              .OrderBy(x => x.Priority) // Order by Priority after the selection
              .Select(x => new GECrateModel
              {
                  SectorName = x.SectorName,
                  GECrateId = x.GECrateId,
                  Slab1 = x.Slab1,
                  Slab2 = x.Slab2,
                  Slab3 = x.Slab3,
                  Slab4 = x.Slab4,
                  Uptosl1 = x.Uptosl1,
                  Uptosl2 = x.Uptosl2,
                  Uptosl3 = x.Uptosl3,
                  Uptosl4 = x.Uptosl4,
                  Company_id = x.Company_id,
                  NoOfSlab = x.NoOfSlab
              }).ToList();

                ViewBag.GECratemaster = getGEC;

                ViewBag.SlabGEC = getGEC.FirstOrDefault();

                return PartialView("RatemasterGEC", getGEC);
            }
            return PartialView("RatemasterGEC", fc);
        }

        [HttpPost]
        public ActionResult Priority(int? only, FormCollection fc, float[] slab1, string comppid)
        {
            //var CompanyId = TempData.Peek("CompanyId").ToString();
            comppid = comppid.Replace("__", "&").Replace("xdotx", "."); ;
            var CompanyId = comppid;
            ViewBag.companyid = comppid;

            //ViewBag.Dox = db.Ratems.Where(m => m.Company_id == CompanyId).ToList();

            //ViewBag.NonDox = db.Nondoxes.Where(m => m.Company_id == CompanyId).ToList();


            if (ModelState.IsValid)
            {

                var rateidarray = fc.GetValues("item.pri_id");
                var slab1arayy = fc.GetValues("item.prislab1");
                var slab2arayy = fc.GetValues("item.prislab2");
                var slab3arayy = fc.GetValues("item.prislab3");
                var slab4arayy = fc.GetValues("item.prislab4");
                var uptoarray = fc.GetValues("Upto");
                var noofslab = fc.GetValues("item.prinoofslab");

                var sectoridarray = fc.GetValues("item.Sector_Id");

                for (int i = 0; i < rateidarray.Count(); i++)
                {
                    if (slab1arayy[i] == "")
                    {
                        slab1arayy[i] = "0";
                    }
                    if (slab2arayy[i] == "")
                    {
                        slab2arayy[i] = "0";
                    }
                    if (slab3arayy[i] == "")
                    {
                        slab3arayy[i] = "0";
                    }
                    if (slab4arayy[i] == "")
                    {
                        slab4arayy[i] = "0";
                    }
                }
                for (int i = 0; i < uptoarray.Count(); i++)
                {
                    if (uptoarray[i] == "")
                    {
                        uptoarray[i] = "0";
                    }
                }



                for (int i = 0; i < rateidarray.Count(); i++)
                {

                    Priority pr = db.Priorities.Find(Convert.ToInt16(rateidarray[i]));

                    pr.prislab1 = Convert.ToDouble(slab1arayy[i]);
                    pr.prislab2 = Convert.ToDouble(slab2arayy[i]);
                    pr.prislab3 = Convert.ToDouble(slab3arayy[i]);
                    pr.prislab4 = Convert.ToDouble(slab4arayy[i]);
                    pr.priupto1 = Convert.ToDouble(uptoarray[0]);
                    pr.priupto2 = Convert.ToDouble(uptoarray[1]);
                    pr.priupto3 = Convert.ToDouble(uptoarray[2]);
                    pr.priupto4 = Convert.ToDouble(uptoarray[3]);
                    pr.Sector_Id = Convert.ToInt16(sectoridarray[i]);
                    pr.prinoofslab = Convert.ToInt16(noofslab[0]);
                    pr.Company_id = CompanyId;




                    db.Entry(pr).State = EntityState.Modified;
                    db.SaveChanges();


                }

                var compid = comppid;

                ViewBag.Message = "Updated SuccessFully";

                @ViewBag.Slabspri = db.Priorities.Where(m => m.Company_id == compid).FirstOrDefault();

                return PartialView("Priority", db.Priorities.Where(m => m.Company_id == compid && m.Sector.BillD == true).OrderBy(m => m.Sector.Priority).ToList());
            }
            return PartialView("Priority", fc);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        [HandleError]
        public ActionResult RatemasterNonDox(int? only, FormCollection fc, string comppid)
        {
            comppid = comppid.Replace("__", "&").Replace("xdotx", "."); ;
            var CompanyId = comppid;
            ViewBag.companyid = comppid;
            //if (only == 2)
            //{
            //    //tO clear array//Array.Clear(slab3arr, 0, slab3arr.Length);
            //    //tO clear array//Array.Clear(slab4arr, 0, slab3arr.Length);
            //}
            //if (only == 2)
            //{
            //    //tO clear array//Array.Clear(slab4arr, 0, slab3arr.Length);
            //}

            if (ModelState.IsValid)
            {
                var Non_IDarray = fc.GetValues("item.Non_ID");
                var Aslab1arayy = fc.GetValues("item.Aslab1");
                var Aslab2arayy = fc.GetValues("item.Aslab2");
                var Aslab3arayy = fc.GetValues("item.Aslab3");
                var Aslab4arayy = fc.GetValues("item.Aslab4");
                var Sslab1arayy = fc.GetValues("item.Sslab1");
                var Sslab2arayy = fc.GetValues("item.Sslab2");
                var Sslab3arayy = fc.GetValues("item.Sslab3");
                var Sslab4arayy = fc.GetValues("item.Sslab4");

                var Auptoarray = fc.GetValues("AUpto");
                var Suptoarray = fc.GetValues("SUpto");
                var sectoridarray = fc.GetValues("item.Sector_Id");
                var NoofslabN= fc.GetValues("item.NoOfSlabN");
                var NoofslabS = fc.GetValues("item.NoOfSlabS");



                for (int i = 0; i < Non_IDarray.Count(); i++)
                {
                    if (Aslab1arayy[i] == "")
                    {
                        Aslab1arayy[i] = "0";
                    }
                    if (Aslab2arayy[i] == "")
                    {
                        Aslab2arayy[i] = "0";
                    }
                    if (Aslab3arayy[i] == "")
                    {
                        Aslab3arayy[i] = "0";
                    }
                    if (Aslab4arayy[i] == "")
                    {
                        Aslab4arayy[i] = "0";
                    }
                    if (Sslab1arayy[i] == "")
                    {
                        Sslab1arayy[i] = "0";
                    }
                    if (Sslab2arayy[i] == "")
                    {
                        Sslab2arayy[i] = "0";
                    }
                    if (Sslab3arayy[i] == "")
                    {
                        Sslab3arayy[i] = "0";
                    }
                    if (Sslab4arayy[i] == "")
                    {
                        Sslab4arayy[i] = "0";
                    }
                }
                for (int i = 0; i < Auptoarray.Count(); i++)
                {
                    if (Auptoarray[i] == "")
                    {
                        Auptoarray[i] = "0";
                    }
                    if (Suptoarray[i] == "")
                    {
                        Suptoarray[i] = "0";
                    }
                }




                for (int i = 0; i < Non_IDarray.Count(); i++)
                {

                    Nondox rm = db.Nondoxes.Find(Convert.ToInt16(Non_IDarray[i]));

                    rm.Aslab1 = Convert.ToDouble(Aslab1arayy[i]);
                    rm.Aslab2 = Convert.ToDouble(Aslab2arayy[i]);
                    rm.Aslab3 = Convert.ToDouble(Aslab3arayy[i]);
                    rm.Aslab4 = Convert.ToDouble(Aslab4arayy[i]);
                    rm.Sslab1 = Convert.ToDouble(Sslab1arayy[i]);
                    rm.Sslab2 = Convert.ToDouble(Sslab2arayy[i]);
                    rm.Sslab3 = Convert.ToDouble(Sslab3arayy[i]);
                    rm.Sslab4 = Convert.ToDouble(Sslab4arayy[i]);
                    rm.AUptosl1 = Convert.ToDouble(Auptoarray[0]);
                    rm.AUptosl2 = Convert.ToDouble(Auptoarray[1]);
                    rm.AUptosl3 = Convert.ToDouble(Auptoarray[2]);
                    rm.AUptosl4 = Convert.ToDouble(Auptoarray[3]);
                    rm.SUptosl1 = Convert.ToDouble(Suptoarray[0]);
                    rm.SUptosl2 = Convert.ToDouble(Suptoarray[1]);
                    rm.SUptosl3 = Convert.ToDouble(Suptoarray[2]);
                    rm.SUptosl4 = Convert.ToDouble(Suptoarray[3]);
                    rm.Company_id = CompanyId;
                    rm.Sector_Id = Convert.ToInt16(sectoridarray[i]);
                    rm.NoOfSlabN = Convert.ToInt16(NoofslabN[0]);
                    rm.NoOfSlabS = Convert.ToInt16(NoofslabS[0]);

                    db.Entry(rm).State = EntityState.Modified;
                    db.SaveChanges();
                }


                var compid = comppid;

                ViewBag.Message = "NonDox Updated SuccessFully";

                @ViewBag.Slabs1 = db.Nondoxes.Where(m => m.Company_id == compid).FirstOrDefault();

                ViewBag.NonDox = db.Nondoxes.Where(m => m.Company_id == compid).ToList();

             

                return PartialView("RatemasterNonDox", db.Nondoxes.Where(m => m.Company_id == compid && m.Sector.BillD == true).OrderBy(m => m.Sector.Priority).ToList());

            }
            return PartialView("RatemasterNonDox", fc);



        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [HandleError]
        public ActionResult RatemasterPlus(float? go149, float? go99, FormCollection fc, string comppid)
        {
            comppid = comppid.Replace("__", "&").Replace("xdotx", "."); ;
            var CompanyId = comppid;
            ViewBag.companyid = comppid;
            if (ModelState.IsValid)
            {

                var plus_idarray = fc.GetValues("item.plus_id");
                var Upto500gmarray = fc.GetValues("item.Upto500gm");
                var U10to25kgarayy = fc.GetValues("item.U10to25kg");
                var U25to50arayy = fc.GetValues("item.U25to50");
                var U50to100arayy = fc.GetValues("item.U50to100");
                var add100kgarayy = fc.GetValues("item.add100kg");
                var Add500gmarayy = fc.GetValues("item.Add500gm");

                for (int i = 0; i < plus_idarray.Count(); i++)
                {
                    if (Upto500gmarray[i] == "")
                    {
                        Upto500gmarray[i] = "0";
                    }
                    if (U10to25kgarayy[i] == "")
                    {
                        U10to25kgarayy[i] = "0";
                    }
                    if (U25to50arayy[i] == "")
                    {
                        U25to50arayy[i] = "0";
                    }
                    if (U50to100arayy[i] == "")
                    {
                        U50to100arayy[i] = "0";
                    }
                    if (add100kgarayy[i] == "")
                    {
                        add100kgarayy[i] = "0";
                    }
                    if (Add500gmarayy[i] == "")
                    {
                        Add500gmarayy[i] = "0";
                    }
                }

                for (int i = 0; i < plus_idarray.Count(); i++)
                {
                    dtdcPlu rm = db.dtdcPlus.Find(Convert.ToInt16(plus_idarray[i]));

                    rm.Upto500gm = Convert.ToDouble(Upto500gmarray[i]);
                    rm.U10to25kg = Convert.ToDouble(U10to25kgarayy[i]);
                    rm.U25to50 = Convert.ToDouble(U25to50arayy[i]);
                    rm.U50to100 = Convert.ToDouble(U50to100arayy[i]);
                    rm.add100kg = Convert.ToDouble(add100kgarayy[i]);
                    rm.Add500gm = Convert.ToDouble(Add500gmarayy[i]);

                    db.Entry(rm).State = EntityState.Modified;
                    db.SaveChanges();
                }

                var compid = comppid;

                ViewBag.Message = "Dtdc Plus Updated SuccessFully";

                @ViewBag.Slabs = db.Dtdc_Ptp.Where(m => m.Company_id == compid).FirstOrDefault();

                return PartialView("RatemasterPlus", db.dtdcPlus.Where(m => m.Company_id == compid).ToList());
            }
            return PartialView("RatemasterPlus", fc);

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [HandleError]
        public ActionResult RatemasterPtp(FormCollection fc, string comppid)
        {

            comppid = comppid.Replace("__", "&").Replace("xdotx", "."); ;
            var CompanyId = comppid;
            ViewBag.companyid = comppid;
            if (ModelState.IsValid)
            {

                var Ptp_idarray = fc.GetValues("item.ptp_id");
                var PUpto500gmarray = fc.GetValues("item.PUpto500gm");
                var PAdd500gmarayy = fc.GetValues("item.PAdd500gm");
                var PU10to25kgarayy = fc.GetValues("item.PU10to25kg");
                var PU25to50arayy = fc.GetValues("item.PU25to50");
                var PU50to100arayy = fc.GetValues("item.PU50to100");
                var Padd100kgarayy = fc.GetValues("item.Padd100kg");

                var P2Upto500gmarray = fc.GetValues("item.P2Upto500gm");
                var P2Add500gmarayy = fc.GetValues("item.P2Add500gm");
                var P2U10to25kgarayy = fc.GetValues("item.P2U10to25kg");
                var P2U25to50arayy = fc.GetValues("item.P2U25to50");
                var P2U50to100arayy = fc.GetValues("item.P2U50to100");
                var P2add100kgarayy = fc.GetValues("item.P2add100kg");


                for (int i = 0; i < Ptp_idarray.Count(); i++)
                {
                    if (PUpto500gmarray[i] == "")
                    {
                        PUpto500gmarray[i] = "0";
                    }
                    if (PAdd500gmarayy[i] == "")
                    {
                        PAdd500gmarayy[i] = "0";
                    }
                    if (PU10to25kgarayy[i] == "")
                    {
                        PU10to25kgarayy[i] = "0";
                    }
                    if (PU25to50arayy[i] == "")
                    {
                        PU25to50arayy[i] = "0";
                    }
                    if (PU50to100arayy[i] == "")
                    {
                        PU50to100arayy[i] = "0";
                    }
                    if (Padd100kgarayy[i] == "")
                    {
                        Padd100kgarayy[i] = "0";
                    }
                    if (P2Upto500gmarray[i] == "")
                    {
                        P2Upto500gmarray[i] = "0";
                    }
                    if (P2Add500gmarayy[i] == "")
                    {
                        P2Add500gmarayy[i] = "0";
                    }
                    if (P2U10to25kgarayy[i] == "")
                    {
                        P2U10to25kgarayy[i] = "0";
                    }
                    if (P2U25to50arayy[i] == "")
                    {
                        P2U25to50arayy[i] = "0";
                    }
                    if (P2U50to100arayy[i] == "")
                    {
                        P2U50to100arayy[i] = "0";
                    }
                    if (P2add100kgarayy[i] == "")
                    {
                        P2add100kgarayy[i] = "0";
                    }
                }

                for (int i = 0; i < Ptp_idarray.Count(); i++)
                {


                    Dtdc_Ptp rm = db.Dtdc_Ptp.Find(Convert.ToInt16(Ptp_idarray[i]));


                    rm.PUpto500gm = Convert.ToDouble(PUpto500gmarray[i]);
                    rm.PAdd500gm = Convert.ToDouble(PAdd500gmarayy[i]);
                    rm.PU10to25kg = Convert.ToDouble(PU10to25kgarayy[i]);
                    rm.PU25to50= Convert.ToDouble(PU25to50arayy[i]);
                    rm.PU50to100 = Convert.ToDouble(PU50to100arayy[i]);
                    rm.Padd100kg = Convert.ToDouble(Padd100kgarayy[i]);
                    rm.P2Upto500gm = Convert.ToDouble(P2Upto500gmarray[i]);
                    rm.P2Add500gm = Convert.ToDouble(P2Add500gmarayy[i]);
                    rm.P2U10to25kg = Convert.ToDouble(P2U10to25kgarayy[i]);
                    rm.P2U25to50 = Convert.ToDouble(P2U25to50arayy[i]);
                    rm.P2U50to100 = Convert.ToDouble(P2U50to100arayy[i]);
                    rm.P2add100kg = Convert.ToDouble(P2add100kgarayy[i]);


                    db.Entry(rm).State = EntityState.Modified;
                    db.SaveChanges();




                }

                var compid = comppid;

                ViewBag.Message = "DtdcPtp Updated SuccessFully";

                @ViewBag.Slabs = db.Dtdc_Ptp.Where(m => m.Company_id == compid).FirstOrDefault();

                return PartialView("RatemasterPtp", db.Dtdc_Ptp.Where(m => m.Company_id == compid).Include(e => e.Sector).ToList());

            }

            return PartialView("RatemasterPtp", fc);



        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [HandleError]
        public ActionResult RatemasterCargo(float? Upto, FormCollection fc, string comppid)
        {
            comppid = comppid.Replace("__", "&").Replace("xdotx", "."); ;
            var CompanyId = comppid;
            ViewBag.companyid = comppid;

            if (ModelState.IsValid)
            {
                var Exp_Idarray = fc.GetValues("item.Exp_Id");
                var Exslab1array = fc.GetValues("item.Exslab1");
                var Exslab2arayy = fc.GetValues("item.Exslab2");
                var Sector_Idarayy = fc.GetValues("item.Sector_Id");


                for (int i = 0; i < Exp_Idarray.Count(); i++)
                {
                    if (Exslab1array[i] == "")
                    {
                        Exslab1array[i] = "0";
                    }
                    if (Exslab2arayy[i] == "")
                    {
                        Exslab2arayy[i] = "0";
                    }

                }

                for (int i = 0; i < Exp_Idarray.Count(); i++)
                {

                    express_cargo rm = db.express_cargo.Find(Convert.ToInt16(Exp_Idarray[i]));

                    rm.Exslab1 = Convert.ToDouble(Exslab1array[i]);
                    rm.Exslab2 = Convert.ToDouble(Exslab2arayy[i]);

                    ViewBag.Message = "Express Cargo Updated SuccessFully";

                    db.Entry(rm).State = EntityState.Modified;
                    db.SaveChanges();
                }
            }

            return PartialView("RateMasterExpressCargo", db.express_cargo.Where(m=>m.Company_id== comppid && m.Sector.BillD == true).OrderBy(m => m.Sector.Priority).ToList());

        }



        public ActionResult RateMaster()
        {
            List<Sector> sr = db.Sectors.ToList();

            return View(sr);
        }

        [HttpPost]
        public ActionResult RateMaster(float [] slab1arr, float[] slab2arr, float[] slab3arr, float[] slab4arr, float[] Upto, int[] Sector_Id, int ? only)
        {
            if(only==2)
            {
                //tO clear array//Array.Clear(slab3arr, 0, slab3arr.Length);
                //tO clear array//Array.Clear(slab4arr, 0, slab3arr.Length);
            }
            if (only == 2)
            {
                //tO clear array//Array.Clear(slab4arr, 0, slab3arr.Length);
            }



            for (int i = 0; i < Sector_Id.Count(); i++)
            {

                Ratem rm = new Ratem();

                rm.slab1 = slab1arr[i];
                rm.slab2 = slab2arr[i];
                rm.slab3 = slab3arr[i];
                rm.slab4 = slab4arr[i];                
                rm.Sector_Id= Sector_Id[i];

                db.Ratems.Add(rm);
                db.SaveChanges();      


            }


            return View();
        }

        [HttpGet]
        public ActionResult ReportPrinterMethod(string id)
        {
            HideShowTableDatasetModel hideTable = new HideShowTableDatasetModel();
            {

                 LocalReport lr = new LocalReport();

                var idd = id.Replace( "__", "&").Replace("xdotx", ".");
                var CompanyId = idd;


                Company company = db.Companies.Where(m => m.Company_Id == CompanyId).FirstOrDefault();

               
                    var dataset2 = db.Ratems.Where(m => m.Company_id == CompanyId).ToList();
                

               
                    var dataset3 = db.Nondoxes.Where(m => m.Company_id == CompanyId).ToList();
              
                if(company.ProductType!=null)
                {
                    var dataset4 = (from a in db.Sectors
                                    join ab in db.Ratems on a.Sector_Id equals ab.Sector_Id
                                    join c in db.Companies on ab.Company_id equals c.Company_Id
                                    where ab.Company_id == CompanyId && a.BillD == true
                                    && (a.GEcreate == null || a.GEcreate == false)
                                    orderby a.Priority
                                    select new
                                    {
                                        a.Sector_Name,
                                        ab.slab1,
                                        ab.slab2,
                                        ab.slab3,
                                        ab.slab4,
                                        ab.Uptosl1,
                                        ab.Uptosl2,
                                        ab.Uptosl3,
                                        ab.Uptosl4,
                                        ab.NoOfSlab,
                                        c.ProductType
                                    }).ToList();

                   
                    var checkedsd = (from ch in dataset4
                                     where company.ProductType.Contains("Ratem")
                                     select ch).Count();
                    if (checkedsd > 0)
                        hideTable.isRatem = true;

                    var dataset5 = (from a in db.Sectors
                                    join ab in db.Nondoxes on a.Sector_Id equals ab.Sector_Id
                                    join c in db.Companies on ab.Company_id equals c.Company_Id
                                    where ab.Company_id == CompanyId && a.BillN == true
                                               && (a.GEcreate == null || a.GEcreate == false)
                                    orderby a.Priority
                                    select new
                                    {
                                        a.Sector_Name,
                                        ab.Aslab1,
                                        ab.Aslab2,
                                        ab.Aslab3,
                                        ab.Aslab4,
                                        ab.Sslab1,
                                        ab.Sslab2,
                                        ab.Sslab3,
                                        ab.Sslab4,
                                        ab.AUptosl1,
                                        ab.AUptosl2,
                                        ab.AUptosl3,
                                        ab.AUptosl4,
                                        ab.SUptosl1,
                                        ab.SUptosl2,
                                        ab.SUptosl3,
                                        ab.SUptosl4,
                                        ab.NoOfSlabN,
                                        ab.NoOfSlabS,
                                        c.ProductType
                                    }).ToList();

                    var checkeN = (from ch in dataset5
                                   where company.ProductType.Contains("NonDox")
                                     select ch).Count();
                    if (checkeN > 0)
                        hideTable.isNondox = true;

                    var dataset6 = (from a in db.dtdcPlus
                                    join ab in db.Companies on a.Company_id equals ab.Company_Id
                                    where a.Company_id == CompanyId
                                              
                                    select new
                                    {
                                        a.plus_id,
                                        a.Sector_Id,
                                       a.destination,
                                        a.add100kg,
                                        a.U50to100,
                                        a.Add500gm,
                                        a.U25to50,
                                        a.Upto500gm,
                                        a.U10to25kg,
                                        ab.ProductType
                                    }).ToList();

                    var checkeDP = (from ch in dataset6
                                   where company.ProductType.Contains("DTDCPLus")
                                   select ch).Count();
                    if (checkeDP > 0)
                        hideTable.isDTDCPLUS = true;

                    var dataset7 = (from a in db.Dtdc_Ptp
                                    join ab in db.Companies on a.Company_id equals ab.Company_Id
                                    where a.Company_id == CompanyId
                                    select new
                                    {
                                        a.ptp_id,
                                        a.Sector_Id,
                                        a.dest,
                                        a.Padd100kg,
                                        a.PUpto500gm,
                                        a.PAdd500gm,
                                        a.PU10to25kg,
                                        a.PU25to50,
                                        a.PU50to100,
                                        a.P2add100kg,
                                        a.P2Add500gm, 
                                        a.P2U10to25kg,
                                        a.P2U25to50,
                                        a.P2U50to100,
                                       a.P2Upto500gm,                                      
                                        ab.ProductType
                                    }).ToList();
                    var checkePTP = (from ch in dataset7
                                    where company.ProductType.Contains("DTDCPTP")
                                    select ch).Count();
                    if (checkePTP > 0)
                        hideTable.isDTDCPTP = true;

                    var dataset8 = (from a in db.Sectors
                                    join ab in db.RateLaptops on a.Sector_Id equals ab.Sector_Id
                                    join c in db.Companies on ab.Company_id equals c.Company_Id
                                    where ab.Company_id == CompanyId && a.BillN == true
                                 && (a.GEcreate == null || a.GEcreate == false)
                                    orderby a.Priority
                                    select new
                                    {
                                        a.Sector_Name,
                                        ab.Aslab1,
                                        ab.Aslab2,
                                        ab.Aslab3,
                                        ab.Aslab4,
                                        ab.Sslab1,
                                        ab.Sslab2,
                                        ab.Sslab3,
                                        ab.Sslab4,
                                        ab.AUptosl1,
                                        ab.AUptosl2,
                                        ab.AUptosl3,
                                        ab.AUptosl4,
                                        ab.SUptosl1,
                                        ab.SUptosl2,
                                        ab.SUptosl3,
                                        ab.SUptosl4,
                                        ab.NoOfSlabN,
                                        ab.NoOfSlabS,
                                        c.ProductType
                                    }).ToList();

                    var checkeL = (from ch in dataset8
                                     where company.ProductType.Contains("Laptops")
                                     select ch).Count();
                    if (checkeL > 0)
                        hideTable.isLaptops = true;

                    var dataset9 = (from a in db.Sectors
                                    join ab in db.RateRevLaptops on a.Sector_Id equals ab.Sector_Id
                                    join c in db.Companies on ab.Company_id equals c.Company_Id
                                    where ab.Company_id == CompanyId && a.BillN == true && (a.GEcreate == null || a.GEcreate == false)
                                    orderby a.Priority
                                    select new
                                    {
                                        a.Sector_Name,
                                        ab.Aslab1,
                                        ab.Aslab2,
                                        ab.Aslab3,
                                        ab.Aslab4,
                                        ab.Sslab1,
                                        ab.Sslab2,
                                        ab.Sslab3,
                                        ab.Sslab4,
                                        ab.AUptosl1,
                                        ab.AUptosl2,
                                        ab.AUptosl3,
                                        ab.AUptosl4,
                                        ab.SUptosl1,
                                        ab.SUptosl2,
                                        ab.SUptosl3,
                                        ab.SUptosl4,
                                        ab.NoOfSlabN,
                                        ab.NoOfSlabS,
                                        c.ProductType
                                    }).ToList();

                    var checkeRL = (from ch in dataset9
                                   where company.ProductType.Contains("RevPickupLaptops")
                                   select ch).Count();
                    if (checkeRL > 0)
                        hideTable.isRevPickupLaptops= true;

                    var dataset10 = (from a in db.Sectors
                                     join ab in db.NewDtdc_Ecommerce on a.Sector_Id equals ab.Sector_Id
                                     join c in db.Companies on ab.Company_id equals c.Company_Id
                                     where ab.Company_id == CompanyId
                                      && (a.GEcreate == null || a.GEcreate == false)
                                     orderby a.Priority
                                     select new
                                     {
                                         a.Sector_Name,
                                         ab.EcomPslab1,
                                         ab.EcomPslab2,
                                         ab.EcomPslab3,
                                         ab.EcomPslab4,
                                         ab.EcomGEslab1,
                                         ab.EcomGEslab2,
                                         ab.EcomGEslab3,
                                         ab.EcomGEslab4,
                                         ab.EcomPupto1,
                                         ab.EcomPupto2,
                                         ab.EcomPupto3,
                                         ab.EcomPupto4,
                                         ab.EcomGEupto1,
                                         ab.EcomGEupto2,
                                         ab.EcomGEupto3,
                                         ab.EcomGEupto4,
                                         ab.NoOfSlabN,
                                         ab.NoOfSlabS,
                                         ab.Company_id,
                                         ab.Sector_Id,


                                         c.ProductType
                                     }).ToList();
                    var checkeE = (from ch in dataset10
                                    where company.ProductType.Contains("Ecommerce")
                                    select ch).Count();
                    if (checkeE > 0)
                        hideTable.isEcommerce = true;

                    var dataset11 = (from a in db.Sectors
                                     join ab in db.Priorities on a.Sector_Id equals ab.Sector_Id
                                     join c in db.Companies on ab.Company_id equals c.Company_Id
                                     where ab.Company_id == CompanyId
                                      && (a.GEcreate == null || a.GEcreate == false)
                                     orderby a.Priority
                                     select new
                                     {
                                         a.Sector_Name,
                                         ab.pri_id,
                                         ab.priupto4,
                                         ab.priupto3,
                                         ab.priupto1,
                                         ab.priupto2,
                                         ab.prinoofslab,
                                         ab.prislab1,
                                         ab.prislab2,
                                         ab.prislab3,
                                         ab.prislab4,                                                
                                         c.ProductType
                                     }).ToList();

                    var checkP = (from ch in dataset11
                                   where company.ProductType.Contains("Priority")
                                   select ch).Count();
                    if (checkP > 0)
                        hideTable.isPriority = true;

                    var dataset12 = (from a in db.Sectors
                                     join ab in db.GECrates on a.Sector_Id equals ab.Sector_Id
                                     join c in db.Companies on ab.Company_id equals c.Company_Id
                                     where ab.Company_id == CompanyId
                                      && (a.GEcreate != null || a.GEcreate == true)
                                     orderby a.Priority
                                     select new
                                     {

                                         ab.GECrateId,
                                         ab.Slab1,
                                         ab.Slab2,
                                         ab.Slab3,
                                         ab.Slab4,
                                         ab.Uptosl1,
                                         ab.Uptosl2,
                                         ab.Uptosl3,
                                         ab.Uptosl4,
                                         ab.Company_id,
                                         ab.NoOfSlab,
                                         ab.Sector_Id,
                                         ab.SectorName,
                                         c.ProductType
                                     }).ToList();
                    var CheckGE=(from ch in dataset12
                                 where company.ProductType.Contains("GECMode")
                                 select ch
                                 ).Count();
                    if(CheckGE>0)
                        hideTable.isGECreate= true;
                    var productList = new List<HideShowTableDatasetModel> { hideTable };

                    ReportDataSource hideReportData = new ReportDataSource("Hide", productList);
                    ReportDataSource rd4 = new ReportDataSource("DataSet4", dataset4);              
                    ReportDataSource rd5 = new ReportDataSource("DataSet5", dataset5);
                    ReportDataSource rd6 = new ReportDataSource("DataSet6", dataset6);
                    ReportDataSource rd7 = new ReportDataSource("DataSet7", dataset7);
                    ReportDataSource rd8 = new ReportDataSource("DataSet8", dataset8);
                    ReportDataSource rd9 = new ReportDataSource("DataSet9", dataset9);
                    ReportDataSource rd10 = new ReportDataSource("DataSet10", dataset10);
                    ReportDataSource rd11 = new ReportDataSource("DataSet11", dataset11);
                    ReportDataSource rd12 = new ReportDataSource("DataSet12", dataset12);

                    lr.DataSources.Add(hideReportData);
                    lr.DataSources.Add(rd4);
                    lr.DataSources.Add(rd5);
                    lr.DataSources.Add(rd6);
                    lr.DataSources.Add(rd7);
                    lr.DataSources.Add(rd8);
                    lr.DataSources.Add(rd9);
                    lr.DataSources.Add(rd10);
                    lr.DataSources.Add(rd11);
                    lr.DataSources.Add(rd12);


                }
                else
                {
                    var dataset4 = (from a in db.Sectors
                                    join ab in db.Ratems on a.Sector_Id equals ab.Sector_Id
                                    where ab.Company_id == CompanyId && a.BillD == true
                                     && (a.GEcreate == null || a.GEcreate == false)
                                    orderby a.Priority
                                    select new
                                    {
                                        a.Sector_Name,
                                        ab.slab1,
                                        ab.slab2,
                                        ab.slab3,
                                        ab.slab4,
                                        ab.Uptosl1,
                                        ab.Uptosl2,
                                        ab.Uptosl3,
                                        ab.Uptosl4,
                                        ab.NoOfSlab,
                                        ProductType="NULL"
                                    }).ToList();

                  
                    var dataset5 = (from a in db.Sectors
                                    join ab in db.Nondoxes on a.Sector_Id equals ab.Sector_Id
                                    where ab.Company_id == CompanyId && a.BillN == true
                                     && (a.GEcreate == null || a.GEcreate == false)
                                    orderby a.Priority
                                    select new
                                    {
                                        a.Sector_Name,
                                        ab.Aslab1,
                                        ab.Aslab2,
                                        ab.Aslab3,
                                        ab.Aslab4,
                                        ab.Sslab1,
                                        ab.Sslab2,
                                        ab.Sslab3,
                                        ab.Sslab4,
                                        ab.AUptosl1,
                                        ab.AUptosl2,
                                        ab.AUptosl3,
                                        ab.AUptosl4,
                                        ab.SUptosl1,
                                        ab.SUptosl2,
                                        ab.SUptosl3,
                                        ab.SUptosl4,
                                        ab.NoOfSlabN,
                                        ab.NoOfSlabS,
                                        ProductType = "NULL"
                                    }).ToList();

                    var dataset6 = (from a in db.dtdcPlus                                
                                    where a.Company_id == CompanyId
                                    select new
                                    {
                                        a.plus_id,
                                        a.Sector_Id,
                                        a.destination,
                                        a.add100kg,
                                        a.U50to100,
                                        a.Add500gm,
                                        a.U25to50,
                                        a.Upto500gm,
                                        a.U10to25kg,
                                        ProductType="NULL"
                                    }).ToList();

                    var dataset7 = (from a in db.Dtdc_Ptp                                
                                    where a.Company_id == CompanyId
                                    select new
                                    {
                                        a.ptp_id,
                                        a.Sector_Id,
                                        a.dest,
                                        a.Padd100kg,
                                        a.PUpto500gm,
                                        a.PAdd500gm,
                                        a.PU10to25kg,
                                        a.PU25to50,
                                        a.PU50to100,
                                        a.P2add100kg,
                                        a.P2Add500gm,
                                        a.P2U10to25kg,
                                        a.P2U25to50,
                                        a.P2U50to100,
                                        a.P2Upto500gm,
                                        ProductType = "NULL"
                                    }).ToList();


                    var dataset8 = (from a in db.Sectors
                                    join ab in db.RateLaptops on a.Sector_Id equals ab.Sector_Id                                 
                                    where ab.Company_id == CompanyId && a.BillN == true
                                     && (a.GEcreate == null || a.GEcreate == false)
                                    orderby a.Priority
                                    select new
                                    {
                                        a.Sector_Name,
                                        ab.Aslab1,
                                        ab.Aslab2,
                                        ab.Aslab3,
                                        ab.Aslab4,
                                        ab.Sslab1,
                                        ab.Sslab2,
                                        ab.Sslab3,
                                        ab.Sslab4,
                                        ab.AUptosl1,
                                        ab.AUptosl2,
                                        ab.AUptosl3,
                                        ab.AUptosl4,
                                        ab.SUptosl1,
                                        ab.SUptosl2,
                                        ab.SUptosl3,
                                        ab.SUptosl4,
                                        ab.NoOfSlabN,
                                        ab.NoOfSlabS,
                                        ProductType = "NULL"
                                    }).ToList();

                    var dataset9 = (from a in db.Sectors
                                    join ab in db.RateRevLaptops on a.Sector_Id equals ab.Sector_Id                                  
                                    where ab.Company_id == CompanyId && a.BillN == true
                                     && (a.GEcreate == null || a.GEcreate == false)
                                    orderby a.Priority
                                    select new
                                    {
                                        a.Sector_Name,
                                        ab.Aslab1,
                                        ab.Aslab2,
                                        ab.Aslab3,
                                        ab.Aslab4,
                                        ab.Sslab1,
                                        ab.Sslab2,
                                        ab.Sslab3,
                                        ab.Sslab4,
                                        ab.AUptosl1,
                                        ab.AUptosl2,
                                        ab.AUptosl3,
                                        ab.AUptosl4,
                                        ab.SUptosl1,
                                        ab.SUptosl2,
                                        ab.SUptosl3,
                                        ab.SUptosl4,
                                        ab.NoOfSlabN,
                                        ab.NoOfSlabS,
                                        ProductType = "NULL"
                                    }).ToList();


                    var dataset10 = (from a in db.Sectors
                                     join sec in db.NewDtdc_Ecommerce on a.Sector_Id equals sec.Sector_Id                                
                                     where sec.Company_id == CompanyId
                                      && (a.GEcreate == null || a.GEcreate == false)
                                     orderby a.Priority
                                     select new
                                     {
                                         a.Sector_Name,
                                         sec.EcomPslab1,
                                         sec.EcomPslab2,
                                         sec.EcomPslab3,
                                         sec.EcomPslab4,
                                         sec.EcomGEslab1,
                                         sec.EcomGEslab2,
                                         sec.EcomGEslab3,
                                         sec.EcomGEslab4,
                                        sec.EcomPupto1,
                                        sec.EcomPupto2,
                                        sec.EcomPupto3,
                                        sec.EcomPupto4,
                                        sec.EcomGEupto1,
                                        sec.EcomGEupto2,
                                        sec.EcomGEupto3,
                                        sec.EcomGEupto4,
                                        sec.NoOfSlabN,
                                        sec.NoOfSlabS,
                                        sec.Company_id,
                                        sec.Sector_Id,

                                         ProductType ="NULL"
                                     }).ToList();


                    var dataset11 = (from a in db.Sectors
                                     join ab in db.Priorities on a.Sector_Id equals ab.Sector_Id
                                     where ab.Company_id == CompanyId
                                      && (a.GEcreate == null || a.GEcreate == false)
                                     orderby a.Priority
                                     select new
                                     {
                                         a.Sector_Name,
                                         ab.pri_id,
                                         ab.priupto4,
                                         ab.priupto3,
                                         ab.priupto1,
                                         ab.priupto2,
                                         ab.prinoofslab,
                                         ab.prislab1,
                                         ab.prislab2,
                                         ab.prislab3,
                                         ab.prislab4,

                                         ProductType = "NULL"
                                     }).ToList();

                    var dataset12 = (from a in db.Sectors
                                     join ab in db.GECrates on a.Sector_Id equals ab.Sector_Id
                                     where ab.Company_id == CompanyId
                                      && (a.GEcreate != null || a.GEcreate == true)
                                     orderby a.Priority
                                     select new
                                     {

                                         ab.GECrateId,
                                         ab.Slab1,
                                         ab.Slab2,
                                         ab.Slab3,
                                         ab.Slab4,
                                         ab.Uptosl1,
                                         ab.Uptosl2,
                                         ab.Uptosl3,
                                         ab.Uptosl4,
                                         ab.Company_id,
                                         ab.NoOfSlab,
                                         ab.Sector_Id,
                                         ab.SectorName

                                     }).ToList();

                    ReportDataSource rd4 = new ReportDataSource("DataSet4", dataset4);                
                    ReportDataSource rd5 = new ReportDataSource("DataSet5", dataset5);             
                    ReportDataSource rd6 = new ReportDataSource("DataSet6", dataset6);
                    ReportDataSource rd7 = new ReportDataSource("DataSet7", dataset7);
                    ReportDataSource rd8 = new ReportDataSource("DataSet8", dataset8);
                    ReportDataSource rd9 = new ReportDataSource("DataSet9", dataset9);
                    ReportDataSource rd10 = new ReportDataSource("DataSet10", dataset10);
                    ReportDataSource rd11 = new ReportDataSource("DataSet11", dataset11);
                    ReportDataSource rd12 = new ReportDataSource("DataSet12", dataset12);
                    lr.DataSources.Add(rd4);
                    lr.DataSources.Add(rd5);
                    lr.DataSources.Add(rd6);
                    lr.DataSources.Add(rd7);
                    lr.DataSources.Add(rd8);
                    lr.DataSources.Add(rd9);
                    lr.DataSources.Add(rd10);
                    lr.DataSources.Add(rd11);
                    lr.DataSources.Add(rd12);
                }





               

                string Pfcode = company.Pf_code;

                var dataset = db.Franchisees.Where(m => m.PF_Code == Pfcode).ToList();
                var dataset1 = db.Companies.Where(m => m.Company_Id == CompanyId).ToList();

                string path = Path.Combine(Server.MapPath("~/RdlcReport"), "QuotationReport.rdlc");

                if (System.IO.File.Exists(path))
                {
                    lr.ReportPath = path;
                }

                ReportDataSource rd = new ReportDataSource("DataSet", dataset);
                ReportDataSource rd1 = new ReportDataSource("DataSet1", dataset1);
                ReportDataSource rd2 = new ReportDataSource("DataSet2", dataset2);
                ReportDataSource rd3 = new ReportDataSource("DataSet3", dataset3);
              
               
               
               

                lr.DataSources.Add(rd);
                lr.DataSources.Add(rd1);
                lr.DataSources.Add(rd2);
                lr.DataSources.Add(rd3);
                
             
             
                string reportType = "PDF";
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

        [HttpPost]
        public JsonResult CheckCompanyname(string Compname)
        {
           
            var data = (from d in db.Companies
                        where d.Company_Name == Compname
                        select d);

            bool isValid =Convert.ToBoolean(data.Count());
            return Json(isValid);
        }
        [HttpPost]
        public JsonResult CheckCompanyID(string CompId)
        {

            var data = (from d in db.Companies
                        where d.Company_Id == CompId
                        select d);

            bool isValid = Convert.ToBoolean(data.Count());
            return Json(isValid);
        }


        //--------------------For Agreement option Added on 14/9/22 ------------------------
      
        public ActionResult IndexforAgreementoption(string id)
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

            //selecteddrpval = db.Companies.Where(m => m.Company_Id == CompanyId).Select(m => m.isregister).FirstOrDefault();

            List<SelectListItem> items = new List<SelectListItem>();

            items.Add(new SelectListItem { Text = "--Select--", Value = "--Select--" });
            items.Add(new SelectListItem { Text = "IsRegister", Value = "IsRegister" });
            items.Add(new SelectListItem { Text = "NotRegister", Value = "NotRegister" });

            string Startdate = Convert.ToString(company.StartDate);//.ToString("MM/dd/yyyy");
            string[] StrDate = Startdate.Split(' ');
            ViewBag.StartDate = StrDate[0];

            string Enddate = Convert.ToString(company.EndDate);//.ToString("MM/dd/yyyy");
            string[] StrEndDate = Enddate.Split(' ');
            ViewBag.EndDate = StrEndDate[0];

            if (company.Isregister == null)
            {
                var selected = items.Where(x => x.Value == "--Select--").First();
                selected.Selected = true;
            }
            else
            {


                var selected = items.Where(x => x.Value == company.Isregister).First();
                selected.Selected = true;
            }

            ViewBag.IsRegister = items;

            //  ViewBag.Minimum_Risk_Charge = items;

            //<-------------risk surch charge dropdown--------------->

            ViewBag.Pf_code = new SelectList(db.Franchisees.Where(x => !x.PF_Code.ToUpper().StartsWith("PR")), "PF_Code", "PF_Code", company.Pf_code);

            return View();
        }


        public ActionResult AddCompanyforAgreementoption()
        {
            ViewBag.Pf_code = new SelectList(db.Franchisees.Where(x => !x.PF_Code.ToUpper().StartsWith("PR")), "PF_Code", "PF_Code");


            List<SelectListItem> items1 = new List<SelectListItem>();

            items1.Add(new SelectListItem { Text = "--Select--", Value = "--Select--" });
            items1.Add(new SelectListItem { Text = "IsRegister", Value = "IsRegister" });
            items1.Add(new SelectListItem { Text = "NotRegister", Value = "NotRegister" });


            ViewBag.IsRegister = items1;

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [HandleError]
        public ActionResult RatemasterCompanyforAgreementoption(Company empmodel, float[] slab1arr, float[] slab2arr, float[] slab3arr, float[] slab4arr, float[] Upto, int[] Sector_Id, int? only, string selected_tab, string IsRegister)
        {
            ViewBag.sr = db.Sectors.ToList();



            var abc = db.Companies.Where(m => m.Company_Id.ToLower() == empmodel.Company_Id.ToLower()).FirstOrDefault();

            if (abc != null)
            {
                ModelState.AddModelError("C_IdError", "Company Id Already Exist");

            }
            //Take PfCode From Session//
            if (ModelState.IsValid)
            {
                // Business Logic
                ViewBag.Message = "Sucess or Failure Message";
                ModelState.Clear();
                TempData["CompanyId"] = empmodel.Company_Id;
                empmodel.IsAgreementoption = 1; // for agreementoption 
                
                db.Companies.Add(empmodel);
                db.SaveChanges();




                var secotrs = db.Sectors.Where(m => m.Pf_code == empmodel.Pf_code && (m.GEcreate == null || m.GEcreate == false)).ToList();



                var basicdox = db.Ratems.Where(m => m.Company_id == "BASIC_TS").ToArray();
                var basicnon = db.Nondoxes.Where(m => m.Company_id == "BASIC_TS").ToArray();
                var express = db.express_cargo.Where(m => m.Company_id == "BASIC_TS").ToArray();
                var prio = db.Priorities.Where(m => m.Company_id == "BASIC_TS").ToArray();
                int j = 0;

                foreach (var i in secotrs)
                {
                    Ratem dox = new Ratem();
                    Nondox ndox = new Nondox();
                    express_cargo cs = new express_cargo();
                    Priority pri = new Priority();

                    dox.Company_id = empmodel.Company_Id;
                    dox.Sector_Id = i.Sector_Id;
                    dox.NoOfSlab = 2;

                    dox.slab1 = basicdox[j].slab1;
                    dox.slab2 = basicdox[j].slab2;
                    dox.slab3 = basicdox[j].slab3;
                    dox.slab4 = basicdox[j].slab4;

                    dox.Uptosl1 = basicdox[j].Uptosl1;
                    dox.Uptosl2 = basicdox[j].Uptosl2;
                    dox.Uptosl3 = basicdox[j].Uptosl3;
                    dox.Uptosl4 = basicdox[j].Uptosl4;

                    ndox.Company_id = empmodel.Company_Id;
                    ndox.Sector_Id = i.Sector_Id;
                    ndox.NoOfSlabN = 2;
                    ndox.NoOfSlabS = 2;

                    ndox.Aslab1 = basicnon[j].Aslab1;
                    ndox.Aslab2 = basicnon[j].Aslab2;
                    ndox.Aslab3 = basicnon[j].Aslab3;
                    ndox.Aslab4 = basicnon[j].Aslab4;


                    ndox.Sslab1 = basicnon[j].Sslab1;
                    ndox.Sslab2 = basicnon[j].Sslab2;
                    ndox.Sslab3 = basicnon[j].Sslab3;
                    ndox.Sslab4 = basicnon[j].Sslab4;

                    ndox.AUptosl1 = basicnon[j].AUptosl1;
                    ndox.AUptosl2 = basicnon[j].AUptosl2;
                    ndox.AUptosl3 = basicnon[j].AUptosl3;
                    ndox.AUptosl4 = basicnon[j].AUptosl4;

                    ndox.SUptosl1 = basicnon[j].SUptosl1;
                    ndox.SUptosl2 = basicnon[j].SUptosl2;
                    ndox.SUptosl3 = basicnon[j].SUptosl3;
                    ndox.SUptosl4 = basicnon[j].SUptosl4;

                    pri.Company_id = empmodel.Company_Id;
                    pri.Sector_Id = i.Sector_Id;
                    pri.prinoofslab = 2;

                    pri.prislab1 = prio[j].prislab1;
                    pri.prislab2 = prio[j].prislab2;
                    pri.prislab3 = prio[j].prislab3;
                    pri.prislab4 = prio[j].prislab4;

                    pri.priupto1 = prio[j].priupto1;
                    pri.priupto2 = prio[j].priupto2;
                    pri.priupto3 = prio[j].priupto3;
                    pri.priupto4 = prio[j].priupto4;

                    cs.Company_id = empmodel.Company_Id;
                    cs.Sector_Id = i.Sector_Id;

                    cs.Exslab1 = express[j].Exslab1;
                    cs.Exslab2 = express[j].Exslab2;

                    db.Ratems.Add(dox);
                    db.Nondoxes.Add(ndox);
                    db.express_cargo.Add(cs);
                    db.Priorities.Add(pri);

                    j++;

                }

                int p = 0;

                var basicplu = db.dtdcPlus.Where(m => m.Company_id == "Basic_Ts").ToArray();
                var basicptp = db.Dtdc_Ptp.Where(m => m.Company_id == "Basic_Ts").ToArray();


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

                    dtplu.Company_id = empmodel.Company_Id;

                    dtplu.Upto500gm = basicplu[p].Upto500gm;
                    dtplu.U10to25kg = basicplu[p].U10to25kg;
                    dtplu.U25to50 = basicplu[p].U25to50;
                    dtplu.U50to100 = basicplu[p].U50to100;
                    dtplu.add100kg = basicplu[p].add100kg;
                    dtplu.Add500gm = basicplu[p].Add500gm;


                    stptp.Company_id = empmodel.Company_Id;
                    stptp.PUpto500gm = basicptp[p].PUpto500gm;
                    stptp.PAdd500gm = basicptp[p].PAdd500gm;
                    stptp.PU10to25kg = basicptp[p].PU10to25kg;
                    stptp.PU25to50 = basicptp[p].PU25to50;
                    stptp.Padd100kg = basicptp[p].Padd100kg;
                    stptp.PU50to100 = basicptp[p].PU50to100;

                    stptp.P2Upto500gm = basicptp[p].P2Upto500gm;
                    stptp.P2Add500gm = basicptp[p].P2Add500gm;
                    stptp.P2U10to25kg = basicptp[p].P2U10to25kg;
                    stptp.P2U25to50 = basicptp[p].P2U25to50;
                    stptp.P2add100kg = basicptp[p].P2add100kg;
                    stptp.P2U50to100 = basicptp[p].P2U50to100;

                    db.dtdcPlus.Add(dtplu);
                    db.Dtdc_Ptp.Add(stptp);

                    p++;

                }


                db.SaveChanges();

                @ViewBag.Slabs = db.Ratems.Where(m => m.Company_id == empmodel.Company_Id).FirstOrDefault();

                ViewBag.Company = new Company();

                ViewBag.Dox = db.Ratems.Where(m => m.Company_id == empmodel.Company_Id).ToList();

                ViewBag.Pri = db.Priorities.Where(m => m.Company_id == empmodel.Company_Id).ToList();
                //ViewBag.SuccessCompany = "Company Added SuccessFully"; To Open Next Tab


                double? selectedval = db.Companies.Where(m => m.Company_Id == empmodel.Company_Id).Select(m => m.Minimum_Risk_Charge).FirstOrDefault();


                List<SelectListItem> items = new List<SelectListItem>();

                items.Add(new SelectListItem { Text = "--Select--", Value = "--Select--" });
                items.Add(new SelectListItem { Text = "IsRegister", Value = "IsRegister" });
                items.Add(new SelectListItem { Text = "NotRegister", Value = "NotRegister" });


                ViewBag.IsRegister = items;



                return RedirectToAction("IndexforAgreementoption", "RateMaster", new { id = empmodel.Company_Id });


            }

            ViewBag.Company = new Company();

            //ViewBag.SuccessCompany = "Company Failed";To Ramain On Same Tab


            double? selectedval1 = db.Companies.Where(m => m.Company_Id == empmodel.Company_Id).Select(m => m.Minimum_Risk_Charge).FirstOrDefault();


            List<SelectListItem> items1 = new List<SelectListItem>();

            items1.Add(new SelectListItem { Text = "0", Value = "0" });
            items1.Add(new SelectListItem { Text = "50", Value = "50" });
            items1.Add(new SelectListItem { Text = "100", Value = "100" });

            if (selectedval1 == null)
            {
                var selected = items1.Where(x => x.Value == "0").First();
                selected.Selected = true;
            }
            else
            {


                var selected = items1.Where(x => x.Value == selectedval1.ToString()).First();
                selected.Selected = true;
            }

            ViewBag.Minimum_Risk_Charge = items1;



            ViewBag.Dox = db.Ratems.Where(m => m.Company_id == "Bala").ToList();

            ViewBag.Pf_code = new SelectList(db.Franchisees.Where(x => !x.PF_Code.ToUpper().StartsWith("PR")), "PF_Code", "PF_Code", empmodel.Pf_code);

            return View("AddCompanyforAgreementoption", empmodel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [HandleError]
        public ActionResult RateMasterDoxforAgreementoption(int? only, FormCollection fc, float[] slab1, string comppid)
        {
            comppid = comppid.Replace("__", "&").Replace("xdotx", "."); ;
            var CompanyId = comppid;
            ViewBag.companyid = comppid;
            //if (only == 2)
            //{
            //    //tO clear array//Array.Clear(slab3arr, 0, slab3arr.Length);
            //    //tO clear array//Array.Clear(slab4arr, 0, slab3arr.Length);
            //} 
            //if (only == 2)
            //{
            //    //tO clear array//Array.Clear(slab4arr, 0, slab3arr.Length);
            //}



            ViewBag.Dox = db.Ratems.Where(m => m.Company_id == CompanyId).ToList();

            ViewBag.NonDox = db.Nondoxes.Where(m => m.Company_id == CompanyId).ToList();


            if (ModelState.IsValid)
            {

                var rateidarray = fc.GetValues("item.Rete_Id");
                var slab1arayy = fc.GetValues("item.slab1");
                var slab2arayy = fc.GetValues("item.slab2");
                var slab3arayy = fc.GetValues("item.slab3");
                var slab4arayy = fc.GetValues("item.slab4");
                var uptoarray = fc.GetValues("Upto");
                var noofslab = fc.GetValues("item.NoOfSlab");

                var sectoridarray = fc.GetValues("item.Sector_Id");

                for (int i = 0; i < rateidarray.Count(); i++)
                {
                    if (slab1arayy[i] == "")
                    {
                        slab1arayy[i] = "0";
                    }
                    if (slab2arayy[i] == "")
                    {
                        slab2arayy[i] = "0";
                    }
                    if (slab3arayy[i] == "")
                    {
                        slab3arayy[i] = "0";
                    }
                    if (slab4arayy[i] == "")
                    {
                        slab4arayy[i] = "0";
                    }
                }
                for (int i = 0; i < uptoarray.Count(); i++)
                {
                    if (uptoarray[i] == "")
                    {
                        uptoarray[i] = "0";
                    }
                }



                for (int i = 0; i < rateidarray.Count(); i++)
                {

                    Ratem rm = db.Ratems.Find(Convert.ToInt16(rateidarray[i]));

                    rm.slab1 = Convert.ToDouble(slab1arayy[i]);
                    rm.slab2 = Convert.ToDouble(slab2arayy[i]);
                    rm.slab3 = Convert.ToDouble(slab3arayy[i]);
                    rm.slab4 = Convert.ToDouble(slab4arayy[i]);
                    rm.Uptosl1 = Convert.ToDouble(uptoarray[0]);
                    rm.Uptosl2 = Convert.ToDouble(uptoarray[1]);
                    rm.Uptosl3 = Convert.ToDouble(uptoarray[2]);
                    rm.Uptosl4 = Convert.ToDouble(uptoarray[3]);
                    rm.Sector_Id = Convert.ToInt16(sectoridarray[i]);
                    rm.NoOfSlab = Convert.ToInt16(noofslab[0]);
                    rm.Company_id = CompanyId;




                    db.Entry(rm).State = EntityState.Modified;
                    db.SaveChanges();


                }

                var compid = comppid;

                ViewBag.Message = "Dox Updated SuccessFully";

                @ViewBag.Slabs = db.Ratems.Where(m => m.Company_id == compid).FirstOrDefault();

                return PartialView("RateMasterDoxforAgreementoption", db.Ratems.Where(m => m.Company_id == compid && m.Sector.BillD == true).OrderBy(m => m.Sector.Priority).ToList());
            }
            return PartialView("RateMasterDoxforAgreementoption", fc);
        }



        [HttpPost]
        [ValidateAntiForgeryToken]
        [HandleError]
        public ActionResult RatemasterNonDoxforAgreementoption(int? only, FormCollection fc, string comppid)
        {
            comppid = comppid.Replace("__", "&").Replace("xdotx", "."); ;
            var CompanyId = comppid;
            ViewBag.companyid = comppid;
            //if (only == 2)
            //{
            //    //tO clear array//Array.Clear(slab3arr, 0, slab3arr.Length);
            //    //tO clear array//Array.Clear(slab4arr, 0, slab3arr.Length);
            //}
            //if (only == 2)
            //{
            //    //tO clear array//Array.Clear(slab4arr, 0, slab3arr.Length);
            //}

            if (ModelState.IsValid)
            {
                var Non_IDarray = fc.GetValues("item.Non_ID");
                var Aslab1arayy = fc.GetValues("item.Aslab1");
                var Aslab2arayy = fc.GetValues("item.Aslab2");
                var Aslab3arayy = fc.GetValues("item.Aslab3");
                var Aslab4arayy = fc.GetValues("item.Aslab4");
                var Sslab1arayy = fc.GetValues("item.Sslab1");
                var Sslab2arayy = fc.GetValues("item.Sslab2");
                var Sslab3arayy = fc.GetValues("item.Sslab3");
                var Sslab4arayy = fc.GetValues("item.Sslab4");

                var Auptoarray = fc.GetValues("AUpto");
                var Suptoarray = fc.GetValues("SUpto");
                var sectoridarray = fc.GetValues("item.Sector_Id");
                var NoofslabN = fc.GetValues("item.NoOfSlabN");
                var NoofslabS = fc.GetValues("item.NoOfSlabS");



                for (int i = 0; i < Non_IDarray.Count(); i++)
                {
                    if (Aslab1arayy[i] == "")
                    {
                        Aslab1arayy[i] = "0";
                    }
                    if (Aslab2arayy[i] == "")
                    {
                        Aslab2arayy[i] = "0";
                    }
                    if (Aslab3arayy[i] == "")
                    {
                        Aslab3arayy[i] = "0";
                    }
                    if (Aslab4arayy[i] == "")
                    {
                        Aslab4arayy[i] = "0";
                    }
                    if (Sslab1arayy[i] == "")
                    {
                        Sslab1arayy[i] = "0";
                    }
                    if (Sslab2arayy[i] == "")
                    {
                        Sslab2arayy[i] = "0";
                    }
                    if (Sslab3arayy[i] == "")
                    {
                        Sslab3arayy[i] = "0";
                    }
                    if (Sslab4arayy[i] == "")
                    {
                        Sslab4arayy[i] = "0";
                    }
                }
                for (int i = 0; i < Auptoarray.Count(); i++)
                {
                    if (Auptoarray[i] == "")
                    {
                        Auptoarray[i] = "0";
                    }
                    if (Suptoarray[i] == "")
                    {
                        Suptoarray[i] = "0";
                    }
                }




                for (int i = 0; i < Non_IDarray.Count(); i++)
                {

                    Nondox rm = db.Nondoxes.Find(Convert.ToInt16(Non_IDarray[i]));

                    rm.Aslab1 = Convert.ToDouble(Aslab1arayy[i]);
                    rm.Aslab2 = Convert.ToDouble(Aslab2arayy[i]);
                    rm.Aslab3 = Convert.ToDouble(Aslab3arayy[i]);
                    rm.Aslab4 = Convert.ToDouble(Aslab4arayy[i]);
                    rm.Sslab1 = Convert.ToDouble(Sslab1arayy[i]);
                    rm.Sslab2 = Convert.ToDouble(Sslab2arayy[i]);
                    rm.Sslab3 = Convert.ToDouble(Sslab3arayy[i]);
                    rm.Sslab4 = Convert.ToDouble(Sslab4arayy[i]);
                    rm.AUptosl1 = Convert.ToDouble(Auptoarray[0]);
                    rm.AUptosl2 = Convert.ToDouble(Auptoarray[1]);
                    rm.AUptosl3 = Convert.ToDouble(Auptoarray[2]);
                    rm.AUptosl4 = Convert.ToDouble(Auptoarray[3]);
                    rm.SUptosl1 = Convert.ToDouble(Suptoarray[0]);
                    rm.SUptosl2 = Convert.ToDouble(Suptoarray[1]);
                    rm.SUptosl3 = Convert.ToDouble(Suptoarray[2]);
                    rm.SUptosl4 = Convert.ToDouble(Suptoarray[3]);
                    rm.Company_id = CompanyId;
                    rm.Sector_Id = Convert.ToInt16(sectoridarray[i]);
                    rm.NoOfSlabN = Convert.ToInt16(NoofslabN[0]);
                    rm.NoOfSlabS = Convert.ToInt16(NoofslabS[0]);

                    db.Entry(rm).State = EntityState.Modified;
                    db.SaveChanges();
                }


                var compid = comppid;

                ViewBag.Message = "NonDox Updated SuccessFully";

                @ViewBag.Slabs1 = db.Nondoxes.Where(m => m.Company_id == compid).FirstOrDefault();

                ViewBag.NonDox = db.Nondoxes.Where(m => m.Company_id == compid).ToList();



                return PartialView("RatemasterNonDoxforAgreementoption", db.Nondoxes.Where(m => m.Company_id == compid && m.Sector.BillD == true).OrderBy(m => m.Sector.Priority).ToList());

            }
            return PartialView("RatemasterNonDoxforAgreementoption", fc);



        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [HandleError]
        public ActionResult RatemasterPlusforAgreementoption(float? go149, float? go99, FormCollection fc, string comppid)
        {
            comppid = comppid.Replace("__", "&").Replace("xdotx", "."); ;
            var CompanyId = comppid;
            ViewBag.companyid = comppid;
            if (ModelState.IsValid)
            {

                var plus_idarray = fc.GetValues("item.plus_id");
                var Upto500gmarray = fc.GetValues("item.Upto500gm");
                var U10to25kgarayy = fc.GetValues("item.U10to25kg");
                var U25to50arayy = fc.GetValues("item.U25to50");
                var U50to100arayy = fc.GetValues("item.U50to100");
                var add100kgarayy = fc.GetValues("item.add100kg");
                var Add500gmarayy = fc.GetValues("item.Add500gm");

                for (int i = 0; i < plus_idarray.Count(); i++)
                {
                    if (Upto500gmarray[i] == "")
                    {
                        Upto500gmarray[i] = "0";
                    }
                    if (U10to25kgarayy[i] == "")
                    {
                        U10to25kgarayy[i] = "0";
                    }
                    if (U25to50arayy[i] == "")
                    {
                        U25to50arayy[i] = "0";
                    }
                    if (U50to100arayy[i] == "")
                    {
                        U50to100arayy[i] = "0";
                    }
                    if (add100kgarayy[i] == "")
                    {
                        add100kgarayy[i] = "0";
                    }
                    if (Add500gmarayy[i] == "")
                    {
                        Add500gmarayy[i] = "0";
                    }
                }

                for (int i = 0; i < plus_idarray.Count(); i++)
                {
                    dtdcPlu rm = db.dtdcPlus.Find(Convert.ToInt16(plus_idarray[i]));

                    rm.Upto500gm = Convert.ToDouble(Upto500gmarray[i]);
                    rm.U10to25kg = Convert.ToDouble(U10to25kgarayy[i]);
                    rm.U25to50 = Convert.ToDouble(U25to50arayy[i]);
                    rm.U50to100 = Convert.ToDouble(U50to100arayy[i]);
                    rm.add100kg = Convert.ToDouble(add100kgarayy[i]);
                    rm.Add500gm = Convert.ToDouble(Add500gmarayy[i]);

                    db.Entry(rm).State = EntityState.Modified;
                    db.SaveChanges();
                }

                var compid = comppid;

                ViewBag.Message = "Dtdc Plus Updated SuccessFully";

                @ViewBag.Slabs = db.Dtdc_Ptp.Where(m => m.Company_id == compid).FirstOrDefault();

                return PartialView("RatemasterPlusforAgreementoption", db.dtdcPlus.Where(m => m.Company_id == compid).ToList());
            }
            return PartialView("RatemasterPlusforAgreementoption", fc);

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [HandleError]
        public ActionResult RatemasterPtpforAgreementoption(FormCollection fc, string comppid)
        {

            comppid = comppid.Replace("__", "&").Replace("xdotx", "."); ;
            var CompanyId = comppid;
            ViewBag.companyid = comppid;
            if (ModelState.IsValid)
            {

                var Ptp_idarray = fc.GetValues("item.ptp_id");
                var PUpto500gmarray = fc.GetValues("item.PUpto500gm");
                var PAdd500gmarayy = fc.GetValues("item.PAdd500gm");
                var PU10to25kgarayy = fc.GetValues("item.PU10to25kg");
                var PU25to50arayy = fc.GetValues("item.PU25to50");
                var PU50to100arayy = fc.GetValues("item.PU50to100");
                var Padd100kgarayy = fc.GetValues("item.Padd100kg");

                var P2Upto500gmarray = fc.GetValues("item.P2Upto500gm");
                var P2Add500gmarayy = fc.GetValues("item.P2Add500gm");
                var P2U10to25kgarayy = fc.GetValues("item.P2U10to25kg");
                var P2U25to50arayy = fc.GetValues("item.P2U25to50");
                var P2U50to100arayy = fc.GetValues("item.P2U50to100");
                var P2add100kgarayy = fc.GetValues("item.P2add100kg");


                for (int i = 0; i < Ptp_idarray.Count(); i++)
                {
                    if (PUpto500gmarray[i] == "")
                    {
                        PUpto500gmarray[i] = "0";
                    }
                    if (PAdd500gmarayy[i] == "")
                    {
                        PAdd500gmarayy[i] = "0";
                    }
                    if (PU10to25kgarayy[i] == "")
                    {
                        PU10to25kgarayy[i] = "0";
                    }
                    if (PU25to50arayy[i] == "")
                    {
                        PU25to50arayy[i] = "0";
                    }
                    if (PU50to100arayy[i] == "")
                    {
                        PU50to100arayy[i] = "0";
                    }
                    if (Padd100kgarayy[i] == "")
                    {
                        Padd100kgarayy[i] = "0";
                    }
                    if (P2Upto500gmarray[i] == "")
                    {
                        P2Upto500gmarray[i] = "0";
                    }
                    if (P2Add500gmarayy[i] == "")
                    {
                        P2Add500gmarayy[i] = "0";
                    }
                    if (P2U10to25kgarayy[i] == "")
                    {
                        P2U10to25kgarayy[i] = "0";
                    }
                    if (P2U25to50arayy[i] == "")
                    {
                        P2U25to50arayy[i] = "0";
                    }
                    if (P2U50to100arayy[i] == "")
                    {
                        P2U50to100arayy[i] = "0";
                    }
                    if (P2add100kgarayy[i] == "")
                    {
                        P2add100kgarayy[i] = "0";
                    }
                }

                for (int i = 0; i < Ptp_idarray.Count(); i++)
                {


                    Dtdc_Ptp rm = db.Dtdc_Ptp.Find(Convert.ToInt16(Ptp_idarray[i]));


                    rm.PUpto500gm = Convert.ToDouble(PUpto500gmarray[i]);
                    rm.PAdd500gm = Convert.ToDouble(PAdd500gmarayy[i]);
                    rm.PU10to25kg = Convert.ToDouble(PU10to25kgarayy[i]);
                    rm.PU25to50 = Convert.ToDouble(PU25to50arayy[i]);
                    rm.PU50to100 = Convert.ToDouble(PU50to100arayy[i]);
                    rm.Padd100kg = Convert.ToDouble(Padd100kgarayy[i]);
                    rm.P2Upto500gm = Convert.ToDouble(P2Upto500gmarray[i]);
                    rm.P2Add500gm = Convert.ToDouble(P2Add500gmarayy[i]);
                    rm.P2U10to25kg = Convert.ToDouble(P2U10to25kgarayy[i]);
                    rm.P2U25to50 = Convert.ToDouble(P2U25to50arayy[i]);
                    rm.P2U50to100 = Convert.ToDouble(P2U50to100arayy[i]);
                    rm.P2add100kg = Convert.ToDouble(P2add100kgarayy[i]);


                    db.Entry(rm).State = EntityState.Modified;
                    db.SaveChanges();




                }

                var compid = comppid;

                ViewBag.Message = "DtdcPtp Updated SuccessFully";

                @ViewBag.Slabs = db.Dtdc_Ptp.Where(m => m.Company_id == compid).FirstOrDefault();

                return PartialView("RatemasterPtpforAgreementoption", db.Dtdc_Ptp.Where(m => m.Company_id == compid).Include(e => e.Sector).ToList());

            }

            return PartialView("RatemasterPtpforAgreementoption", fc);



        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [HandleError]
        public ActionResult RatemasterCargoforAgreementoption(float? Upto, FormCollection fc, string comppid)
        {
            comppid = comppid.Replace("__", "&").Replace("xdotx", "."); ;
            var CompanyId = comppid;
            ViewBag.companyid = comppid;

            if (ModelState.IsValid)
            {
                var Exp_Idarray = fc.GetValues("item.Exp_Id");
                var Exslab1array = fc.GetValues("item.Exslab1");
                var Exslab2arayy = fc.GetValues("item.Exslab2");
                var Sector_Idarayy = fc.GetValues("item.Sector_Id");


                for (int i = 0; i < Exp_Idarray.Count(); i++)
                {
                    if (Exslab1array[i] == "")
                    {
                        Exslab1array[i] = "0";
                    }
                    if (Exslab2arayy[i] == "")
                    {
                        Exslab2arayy[i] = "0";
                    }

                }

                for (int i = 0; i < Exp_Idarray.Count(); i++)
                {

                    express_cargo rm = db.express_cargo.Find(Convert.ToInt16(Exp_Idarray[i]));

                    rm.Exslab1 = Convert.ToDouble(Exslab1array[i]);
                    rm.Exslab2 = Convert.ToDouble(Exslab2arayy[i]);

                    ViewBag.Message = "Express Cargo Updated SuccessFully";

                    db.Entry(rm).State = EntityState.Modified;
                    db.SaveChanges();
                }
            }

            return PartialView("RateMasterExpressCargoforAgreementoption", db.express_cargo.Where(m => m.Company_id == comppid && m.Sector.BillD == true).OrderBy(m => m.Sector.Priority).ToList());

        }

        [HttpPost]
        public ActionResult PriorityforAgreementoption(int? only, FormCollection fc, float[] slab1, string comppid)
        {
            //var CompanyId = TempData.Peek("CompanyId").ToString();
            comppid = comppid.Replace("__", "&").Replace("xdotx", "."); ;
            var CompanyId = comppid;
            ViewBag.companyid = comppid;

            //ViewBag.Dox = db.Ratems.Where(m => m.Company_id == CompanyId).ToList();

            //ViewBag.NonDox = db.Nondoxes.Where(m => m.Company_id == CompanyId).ToList();


            if (ModelState.IsValid)
            {

                var rateidarray = fc.GetValues("item.pri_id");
                var slab1arayy = fc.GetValues("item.prislab1");
                var slab2arayy = fc.GetValues("item.prislab2");
                var slab3arayy = fc.GetValues("item.prislab3");
                var slab4arayy = fc.GetValues("item.prislab4");
                var uptoarray = fc.GetValues("Upto");
                var noofslab = fc.GetValues("item.prinoofslab");

                var sectoridarray = fc.GetValues("item.Sector_Id");

                for (int i = 0; i < rateidarray.Count(); i++)
                {
                    if (slab1arayy[i] == "")
                    {
                        slab1arayy[i] = "0";
                    }
                    if (slab2arayy[i] == "")
                    {
                        slab2arayy[i] = "0";
                    }
                    if (slab3arayy[i] == "")
                    {
                        slab3arayy[i] = "0";
                    }
                    if (slab4arayy[i] == "")
                    {
                        slab4arayy[i] = "0";
                    }
                }
                for (int i = 0; i < uptoarray.Count(); i++)
                {
                    if (uptoarray[i] == "")
                    {
                        uptoarray[i] = "0";
                    }
                }



                for (int i = 0; i < rateidarray.Count(); i++)
                {

                    Priority pr = db.Priorities.Find(Convert.ToInt16(rateidarray[i]));

                    pr.prislab1 = Convert.ToDouble(slab1arayy[i]);
                    pr.prislab2 = Convert.ToDouble(slab2arayy[i]);
                    pr.prislab3 = Convert.ToDouble(slab3arayy[i]);
                    pr.prislab4 = Convert.ToDouble(slab4arayy[i]);
                    pr.priupto1 = Convert.ToDouble(uptoarray[0]);
                    pr.priupto2 = Convert.ToDouble(uptoarray[1]);
                    pr.priupto3 = Convert.ToDouble(uptoarray[2]);
                    pr.priupto4 = Convert.ToDouble(uptoarray[3]);
                    pr.Sector_Id = Convert.ToInt16(sectoridarray[i]);
                    pr.prinoofslab = Convert.ToInt16(noofslab[0]);
                    pr.Company_id = CompanyId;




                    db.Entry(pr).State = EntityState.Modified;
                    db.SaveChanges();


                }

                var compid = comppid;

                ViewBag.Message = "Updated SuccessFully";

                @ViewBag.Slabspri = db.Priorities.Where(m => m.Company_id == compid).FirstOrDefault();

                return PartialView("PriorityforAgreementoption", db.Priorities.Where(m => m.Company_id == compid && m.Sector.BillD == true).OrderBy(m => m.Sector.Priority).ToList());
            }
            return PartialView("PriorityforAgreementoption", fc);
        }


        public ActionResult EditCompanyRateMasterforAgreementoption()
        {


            return View(db.Companies.Where(d=>d.IsAgreementoption==1).ToList());
        }


        public ActionResult EditCompanyRateforAgreementoption(string Id)
        {
            var Idd = Id.Replace("__", "&").Replace("xdotx", "."); ;
            var pfcode = db.Companies.Where(m => m.Company_Id == Idd).FirstOrDefault();

            TempData["CompanyId"] = Id;
            var secotrs = db.Sectors.Where(m => m.Pf_code == pfcode.Pf_code && (m.GEcreate == null || m.GEcreate == false)).ToList();
            Priority pri = new Priority();

            var prio = db.Priorities.Where(m => m.Company_id == "BASIC_TS").ToArray();
            int j = 0;


            if (pfcode.Priorities.Count == 0)
            {
                foreach (var i in secotrs)
                {

                    pri.Company_id = Idd;
                    pri.Sector_Id = i.Sector_Id;
                    pri.prinoofslab = 2;

                    pri.prislab1 = prio[j].prislab1;
                    pri.prislab2 = prio[j].prislab2;
                    pri.prislab3 = prio[j].prislab3;
                    pri.prislab4 = prio[j].prislab4;

                    pri.priupto1 = prio[j].priupto1;
                    pri.priupto2 = prio[j].priupto2;
                    pri.priupto3 = prio[j].priupto3;
                    pri.priupto4 = prio[j].priupto4;

                    db.Priorities.Add(pri);
                    db.SaveChanges();

                    j++;
                }

            }
            return RedirectToAction("IndexforAgreementoption", "RateMaster", new { id = Id });
        }


        public ActionResult ExportToExcelforAgreementoption()
        {
            //List<Company> termsList11 = new List<Company>();
            var df = db.Companies.Where(d=>d.IsAgreementoption==1).ToList();

            var gv = new GridView();
            //  gv.DataSource = bookingBLL.GetAllTutorRating().ToList().Select(x=>new {x.Tutor.Tutor_Firstname,x.TeachingSkill,x.SubjectKnowledge,x.Punctuality,x.OverallExperience,x.rate });
            gv.DataSource = df;

            gv.DataBind();
            Response.ClearContent();
            Response.Buffer = true;
            Response.AddHeader("content-disposition", "attachment; filename=AllRateMasterExportToExcelforAgreement.xls");
            Response.ContentType = "application/ms-excel";
            Response.Charset = "";
            StringWriter objStringWriter = new StringWriter();
            HtmlTextWriter objHtmlTextWriter = new HtmlTextWriter(objStringWriter);
            gv.RenderControl(objHtmlTextWriter);
            Response.Output.Write(objStringWriter.ToString());
            Response.Flush();
            Response.End();
            return View("IndexforAgreementoption");
        }

        [HttpGet]
        public ActionResult ReportPrinterMethodforAgreementoption(string id)
        {
            {

                LocalReport lr = new LocalReport();

                var idd = id.Replace("__", "&").Replace("xdotx", ".");
                var CompanyId = idd;


                Company company = db.Companies.Where(m => m.Company_Id == CompanyId).FirstOrDefault();

                var dataset2 = db.Ratems.Where(m => m.Company_id == CompanyId).ToList();

                var dataset3 = db.Nondoxes.Where(m => m.Company_id == CompanyId).ToList();

                var dataset4 = (from a in db.Sectors
                                join ab in db.Ratems on a.Sector_Id equals ab.Sector_Id
                                where ab.Company_id == CompanyId && a.BillD == true
                                 && (a.GEcreate == null || a.GEcreate == false)
                                orderby a.Priority
                                select new
                                {
                                    a.Sector_Name,
                                    ab.slab1,
                                    ab.slab2,
                                    ab.slab3,
                                    ab.slab4,
                                    ab.Uptosl1,
                                    ab.Uptosl2,
                                    ab.Uptosl3,
                                    ab.Uptosl4,
                                    ab.NoOfSlab
                                }).ToList();

                var dataset5 = (from a in db.Sectors
                                join ab in db.Nondoxes on a.Sector_Id equals ab.Sector_Id
                                where ab.Company_id == CompanyId && a.BillN == true && (a.GEcreate == null || a.GEcreate == false)
                                orderby a.Priority
                                select new
                                {
                                    a.Sector_Name,
                                    ab.Aslab1,
                                    ab.Aslab2,
                                    ab.Aslab3,
                                    ab.Aslab4,
                                    ab.Sslab1,
                                    ab.Sslab2,
                                    ab.Sslab3,
                                    ab.Sslab4,
                                    ab.AUptosl1,
                                    ab.AUptosl2,
                                    ab.AUptosl3,
                                    ab.AUptosl4,
                                    ab.SUptosl1,
                                    ab.SUptosl2,
                                    ab.SUptosl3,
                                    ab.SUptosl4,
                                    ab.NoOfSlabN,
                                    ab.NoOfSlabS
                                }).ToList();

                var dataset6 = db.dtdcPlus.Where(m => m.Company_id == CompanyId).ToList();

                var dataset7 = db.Dtdc_Ptp.Where(m => m.Company_id == CompanyId).ToList();

                string Pfcode = company.Pf_code;

                var dataset = db.Franchisees.Where(m => m.PF_Code == Pfcode).ToList();
                var dataset1 = db.Companies.Where(m => m.Company_Id == CompanyId).ToList();

                var datasetagr = db.Companies.Where(m => m.Company_Id == CompanyId).ToList();


                

                string path = Path.Combine(Server.MapPath("~/RdlcReport"), "QuotationAgreementReport.rdlc");

                if (System.IO.File.Exists(path))
                {   
                    lr.ReportPath = path;
                }

                ReportDataSource rd = new ReportDataSource("DataSet", dataset);
                ReportDataSource rd1 = new ReportDataSource("DataSet1", dataset1);
                ReportDataSource rd2 = new ReportDataSource("DataSet2", dataset2);
                ReportDataSource rd3 = new ReportDataSource("DataSet3", dataset3);
                ReportDataSource rd4 = new ReportDataSource("DataSet4", dataset4);
                ReportDataSource rd5 = new ReportDataSource("DataSet5", dataset5);
                ReportDataSource rd6 = new ReportDataSource("DataSet6", dataset6);
                ReportDataSource rd7 = new ReportDataSource("DataSet7", dataset7);
                ReportDataSource rd8 = new ReportDataSource("DataSetagrcomp", datasetagr);
    

                lr.DataSources.Add(rd);
                lr.DataSources.Add(rd1);
                lr.DataSources.Add(rd2);
                lr.DataSources.Add(rd3);
                lr.DataSources.Add(rd4);
                lr.DataSources.Add(rd5);
                lr.DataSources.Add(rd6);
                lr.DataSources.Add(rd7);
                lr.DataSources.Add(rd8);
              

                string reportType = "PDF";
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



        [HttpPost]
        [ValidateAntiForgeryToken]
        [HandleError]
        public ActionResult RatemasterEditCompanyforAgreementoption(Company empmodel, float[] slab1arr, float[] slab2arr, float[] slab3arr, float[] slab4arr, float[] Upto, int[] Sector_Id, int? only, string selected_tab)
        {
            ViewBag.sr = db.Sectors.ToList();


            var abc = db.Companies.Where(m => m.Company_Id.ToLower() == empmodel.Company_Id.ToLower()).FirstOrDefault();

            foreach (ModelState modelState in ViewData.ModelState.Values)
            {
                foreach (ModelError error in modelState.Errors)
                {
                    Console.WriteLine(error.ErrorMessage);
                }
            }

            foreach (ModelState modelState in ViewData.ModelState.Values)
            {
                foreach (ModelError error in modelState.Errors)
                {
                    Console.WriteLine(error.ErrorMessage);
                }
            }

            if (ModelState.IsValid)
            {


                // Business Logic
                ViewBag.Message = "Sucess or Failure Message";
                ModelState.Clear();
                TempData["CompanyId"] = empmodel.Company_Id;
                empmodel.IsAgreementoption = 1; // for agreementoption

                var local = db.Companies.Where(m => m.Company_Id == empmodel.Company_Id).FirstOrDefault();

                if (local != null)
                {
                    db.Entry(local).State = EntityState.Detached;
                }


                db.Entry(empmodel).State = EntityState.Modified;
                db.SaveChanges();

                //<-------------risk surch charge dropdown--------------->

                double? selectedval = db.Companies.Where(m => m.Company_Id == empmodel.Company_Id).Select(m => m.Minimum_Risk_Charge).FirstOrDefault();


                //List<SelectListItem> items = new List<SelectListItem>();

                //items.Add(new SelectListItem { Text = "0", Value = "0" });
                //items.Add(new SelectListItem { Text = "50", Value = "50" });
                //items.Add(new SelectListItem { Text = "100", Value = "100" });

                //if (selectedval == null)
                //{
                //    var selected = items.Where(x => x.Value == "0").First();
                //    selected.Selected = true;
                //}
                //else
                //{


                //    var selected = items.Where(x => x.Value == selectedval.ToString()).First();
                //    selected.Selected = true;
                //}

                List<SelectListItem> items = new List<SelectListItem>();

                items.Add(new SelectListItem { Text = "--Select--", Value = "--Select--" });
                items.Add(new SelectListItem { Text = "IsRegister", Value = "IsRegister" });
                items.Add(new SelectListItem { Text = "NotRegister", Value = "NotRegister" });


                ViewBag.IsRegister = items;


                ViewBag.Minimum_Risk_Charge = local.Minimum_Risk_Charge;




                //<-------------risk surch charge dropdown--------------->
                //updating all tables pf code

                int[] secotrs = db.Sectors.Where(m => m.Pf_code == empmodel.Pf_code && (m.GEcreate == null || m.GEcreate == false)).Select(m => m.Sector_Id).ToArray();


                int[] doxlist = db.Ratems.Where(m => m.Company_id == empmodel.Company_Id).Select(m => m.Rete_Id).ToArray();
                int[] nonlist = db.Nondoxes.Where(m => m.Company_id == empmodel.Company_Id).Select(m => m.Non_ID).ToArray();
                int[] cslist = db.express_cargo.Where(m => m.Company_id == empmodel.Company_Id).Select(m => m.Exp_Id).ToArray();


                int j = 0;

                int cnt = doxlist.Count();

                for (int i = 0; i < cnt; i++)
                {

                    Ratem dox = new Ratem();
                    Nondox ndox = new Nondox();
                    express_cargo cs = new express_cargo();

                    int d = doxlist[i], n = nonlist[i], ex = cslist[i];

                    dox = db.Ratems.Where(m => m.Rete_Id == d).FirstOrDefault();
                    ndox = db.Nondoxes.Where(m => m.Non_ID == n).FirstOrDefault();
                    cs = db.express_cargo.Where(m => m.Exp_Id == ex).FirstOrDefault();

                    dox.Sector_Id = secotrs[i];
                    ndox.Sector_Id = secotrs[i];
                    cs.Sector_Id = secotrs[i];


                    db.Entry(dox).State = EntityState.Modified;
                    db.Entry(ndox).State = EntityState.Modified;
                    db.Entry(cs).State = EntityState.Modified;



                    db.SaveChanges();

                    j++;

                }





                ViewBag.Message = "Company Updated SuccessFully";

                @ViewBag.Slabs = db.Ratems.Where(m => m.Company_id == empmodel.Company_Id).FirstOrDefault();

                ViewBag.Company = new Company();

                ViewBag.Dox = db.Ratems.Where(m => m.Company_id == "Bala").ToList();

                ViewBag.SuccessCompany = "Company Added SuccessFully";

                ViewBag.Pf_code = new SelectList(db.Franchisees.Where(x => !x.PF_Code.ToUpper().StartsWith("PR")), "PF_Code", "PF_Code", empmodel.Pf_code);

                return PartialView("RateMasterEditCompanyforAgreementoption", empmodel);

            }

            double? selectedval1 = db.Companies.Where(m => m.Company_Id == empmodel.Company_Id).Select(m => m.Minimum_Risk_Charge).FirstOrDefault();


            //List<SelectListItem> items1 = new List<SelectListItem>();

            //items1.Add(new SelectListItem { Text = "0", Value = "0" });
            //items1.Add(new SelectListItem { Text = "50", Value = "50" });
            //items1.Add(new SelectListItem { Text = "100", Value = "100" });

            //if (selectedval1 == null)
            //{
            //    var selected = items1.Where(x => x.Value == "0").First();
            //    selected.Selected = true;
            //}
            //else
            //{
            //    //foreach (var item in items)
            //    //{
            //    //    if (item.Value == selectedval.ToString())
            //    //    {
            //    //        item.Selected = true;
            //    //        break;
            //    //    }
            //    //}

            //    var selected = items1.Where(x => x.Value == selectedval1.ToString()).First();
            //    selected.Selected = true;
            //}

            //ViewBag.Minimum_Risk_Charge = items1;

            ViewBag.Company = new Company();

            //ViewBag.SuccessCompany = "Company Failed";To Ramain On Same Tab

            ViewBag.Dox = db.Ratems.Where(m => m.Company_id == "Bala").ToList();

            ViewBag.Pf_code = new SelectList(db.Franchisees.Where(x => !x.PF_Code.ToUpper().StartsWith("PR")), "PF_Code", "PF_Code", empmodel.Pf_code);

            List<SelectListItem> items1 = new List<SelectListItem>();

            items1.Add(new SelectListItem { Text = "--Select--", Value = "--Select--" });
            items1.Add(new SelectListItem { Text = "IsRegister", Value = "IsRegister" });
            items1.Add(new SelectListItem { Text = "NotRegister", Value = "NotRegister" });


            ViewBag.IsRegister = items1;


            return PartialView("RatemasterEditCompany", empmodel);
        }



        [HttpPost]
        [ValidateAntiForgeryToken]
        [HandleError]
        public ActionResult RatemasterLaptops(int? only, FormCollection fc, string comppid)
        {
            comppid = comppid.Replace("__", "&").Replace("xdotx", "."); ;
            var CompanyId = comppid;
            ViewBag.companyid = comppid;
            //if (only == 2)
            //{
            //    //tO clear array//Array.Clear(slab3arr, 0, slab3arr.Length);
            //    //tO clear array//Array.Clear(slab4arr, 0, slab3arr.Length);
            //}
            //if (only == 2)
            //{
            //    //tO clear array//Array.Clear(slab4arr, 0, slab3arr.Length);
            //}

            if (ModelState.IsValid)
            {
                var Non_IDarray = fc.GetValues("item.RateLaptop_ID");
                var Aslab1arayy = fc.GetValues("item.Aslab1");
                var Aslab2arayy = fc.GetValues("item.Aslab2");
                var Aslab3arayy = fc.GetValues("item.Aslab3");
                var Aslab4arayy = fc.GetValues("item.Aslab4");
                var Sslab1arayy = fc.GetValues("item.Sslab1");
                var Sslab2arayy = fc.GetValues("item.Sslab2");
                var Sslab3arayy = fc.GetValues("item.Sslab3");
                var Sslab4arayy = fc.GetValues("item.Sslab4");

                var Auptoarray = fc.GetValues("AUpto");
                var Suptoarray = fc.GetValues("SUpto");
                var sectoridarray = fc.GetValues("item.Sector_Id");
                var NoofslabN = fc.GetValues("item.NoOfSlabN");
                var NoofslabS = fc.GetValues("item.NoOfSlabS");



                for (int i = 0; i < Non_IDarray.Count(); i++)
                {
                    if (Aslab1arayy[i] == "")
                    {
                        Aslab1arayy[i] = "0";
                    }
                    if (Aslab2arayy[i] == "")
                    {
                        Aslab2arayy[i] = "0";
                    }
                    if (Aslab3arayy[i] == "")
                    {
                        Aslab3arayy[i] = "0";
                    }
                    if (Aslab4arayy[i] == "")
                    {
                        Aslab4arayy[i] = "0";
                    }
                    if (Sslab1arayy[i] == "")
                    {
                        Sslab1arayy[i] = "0";
                    }
                    if (Sslab2arayy[i] == "")
                    {
                        Sslab2arayy[i] = "0";
                    }
                    if (Sslab3arayy[i] == "")
                    {
                        Sslab3arayy[i] = "0";
                    }
                    if (Sslab4arayy[i] == "")
                    {
                        Sslab4arayy[i] = "0";
                    }
                }
                for (int i = 0; i < Auptoarray.Count(); i++)
                {
                    if (Auptoarray[i] == "")
                    {
                        Auptoarray[i] = "0";
                    }
                    if (Suptoarray[i] == "")
                    {
                        Suptoarray[i] = "0";
                    }
                }




                for (int i = 0; i < Non_IDarray.Count(); i++)
                {

                    RateLaptop rm = db.RateLaptops.Find(Convert.ToInt16(Non_IDarray[i]));

                    rm.Aslab1 = Convert.ToDouble(Aslab1arayy[i]);
                    rm.Aslab2 = Convert.ToDouble(Aslab2arayy[i]);
                    rm.Aslab3 = Convert.ToDouble(Aslab3arayy[i]);
                    rm.Aslab4 = Convert.ToDouble(Aslab4arayy[i]);
                    rm.Sslab1 = Convert.ToDouble(Sslab1arayy[i]);
                    rm.Sslab2 = Convert.ToDouble(Sslab2arayy[i]);
                    rm.Sslab3 = Convert.ToDouble(Sslab3arayy[i]);
                    rm.Sslab4 = Convert.ToDouble(Sslab4arayy[i]);
                    rm.AUptosl1 = Convert.ToDouble(Auptoarray[0]);
                    rm.AUptosl2 = Convert.ToDouble(Auptoarray[1]);
                    rm.AUptosl3 = Convert.ToDouble(Auptoarray[2]);
                    rm.AUptosl4 = Convert.ToDouble(Auptoarray[3]);
                    rm.SUptosl1 = Convert.ToDouble(Suptoarray[0]);
                    rm.SUptosl2 = Convert.ToDouble(Suptoarray[1]);
                    rm.SUptosl3 = Convert.ToDouble(Suptoarray[2]);
                    rm.SUptosl4 = Convert.ToDouble(Suptoarray[3]);
                    rm.Company_id = CompanyId;
                    rm.Sector_Id = Convert.ToInt16(sectoridarray[i]);
                    rm.NoOfSlabN = Convert.ToInt16(NoofslabN[0]);
                    rm.NoOfSlabS = Convert.ToInt16(NoofslabS[0]);

                    db.Entry(rm).State = EntityState.Modified;
                    db.SaveChanges();
                }


                var compid = comppid;

                ViewBag.Message = "Laptops Rate Updated SuccessFully";

                @ViewBag.Slabs1 = db.RateLaptops.Where(m => m.Company_id == compid).FirstOrDefault();

                ViewBag.Laptops = db.RateLaptops.Where(m => m.Company_id == compid).ToList();



                return PartialView("RatemasterLaptops", db.RateLaptops.Where(m => m.Company_id == compid && m.Sector.BillD == true).OrderBy(m => m.Sector.Priority).ToList());

            }
            return PartialView("RatemasterLaptops", fc);



        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [HandleError]
        public ActionResult RatemasterRevLaptops(int? only, FormCollection fc, string comppid)
        {
            comppid = comppid.Replace("__", "&").Replace("xdotx", "."); ;
            var CompanyId = comppid;
            ViewBag.companyid = comppid;
            //if (only == 2)
            //{
            //    //tO clear array//Array.Clear(slab3arr, 0, slab3arr.Length);
            //    //tO clear array//Array.Clear(slab4arr, 0, slab3arr.Length);
            //}
            //if (only == 2)
            //{
            //    //tO clear array//Array.Clear(slab4arr, 0, slab3arr.Length);
            //}

            if (ModelState.IsValid)
            {
                var Non_IDarray = fc.GetValues("item.RateRevLaptop_ID");
                var Aslab1arayy = fc.GetValues("item.Aslab1");
                var Aslab2arayy = fc.GetValues("item.Aslab2");
                var Aslab3arayy = fc.GetValues("item.Aslab3");
                var Aslab4arayy = fc.GetValues("item.Aslab4");
                var Sslab1arayy = fc.GetValues("item.Sslab1");
                var Sslab2arayy = fc.GetValues("item.Sslab2");
                var Sslab3arayy = fc.GetValues("item.Sslab3");
                var Sslab4arayy = fc.GetValues("item.Sslab4");

                var Auptoarray = fc.GetValues("AUpto");
                var Suptoarray = fc.GetValues("SUpto");
                var sectoridarray = fc.GetValues("item.Sector_Id");
                var NoofslabN = fc.GetValues("item.NoOfSlabN");
                var NoofslabS = fc.GetValues("item.NoOfSlabS");



                for (int i = 0; i < Non_IDarray.Count(); i++)
                {
                    if (Aslab1arayy[i] == "")
                    {
                        Aslab1arayy[i] = "0";
                    }
                    if (Aslab2arayy[i] == "")
                    {
                        Aslab2arayy[i] = "0";
                    }
                    if (Aslab3arayy[i] == "")
                    {
                        Aslab3arayy[i] = "0";
                    }
                    if (Aslab4arayy[i] == "")
                    {
                        Aslab4arayy[i] = "0";
                    }
                    if (Sslab1arayy[i] == "")
                    {
                        Sslab1arayy[i] = "0";
                    }
                    if (Sslab2arayy[i] == "")
                    {
                        Sslab2arayy[i] = "0";
                    }
                    if (Sslab3arayy[i] == "")
                    {
                        Sslab3arayy[i] = "0";
                    }
                    if (Sslab4arayy[i] == "")
                    {
                        Sslab4arayy[i] = "0";
                    }
                }
                for (int i = 0; i < Auptoarray.Count(); i++)
                {
                    if (Auptoarray[i] == "")
                    {
                        Auptoarray[i] = "0";
                    }
                    if (Suptoarray[i] == "")
                    {
                        Suptoarray[i] = "0";
                    }
                }




                for (int i = 0; i < Non_IDarray.Count(); i++)
                {

                    RateRevLaptop rm = db.RateRevLaptops.Find(Convert.ToInt16(Non_IDarray[i]));

                    rm.Aslab1 = Convert.ToDouble(Aslab1arayy[i]);
                    rm.Aslab2 = Convert.ToDouble(Aslab2arayy[i]);
                    rm.Aslab3 = Convert.ToDouble(Aslab3arayy[i]);
                    rm.Aslab4 = Convert.ToDouble(Aslab4arayy[i]);
                    rm.Sslab1 = Convert.ToDouble(Sslab1arayy[i]);
                    rm.Sslab2 = Convert.ToDouble(Sslab2arayy[i]);
                    rm.Sslab3 = Convert.ToDouble(Sslab3arayy[i]);
                    rm.Sslab4 = Convert.ToDouble(Sslab4arayy[i]);
                    rm.AUptosl1 = Convert.ToDouble(Auptoarray[0]);
                    rm.AUptosl2 = Convert.ToDouble(Auptoarray[1]);
                    rm.AUptosl3 = Convert.ToDouble(Auptoarray[2]);
                    rm.AUptosl4 = Convert.ToDouble(Auptoarray[3]);
                    rm.SUptosl1 = Convert.ToDouble(Suptoarray[0]);
                    rm.SUptosl2 = Convert.ToDouble(Suptoarray[1]);
                    rm.SUptosl3 = Convert.ToDouble(Suptoarray[2]);
                    rm.SUptosl4 = Convert.ToDouble(Suptoarray[3]);
                    rm.Company_id = CompanyId;
                    rm.Sector_Id = Convert.ToInt16(sectoridarray[i]);
                    rm.NoOfSlabN = Convert.ToInt16(NoofslabN[0]);
                    rm.NoOfSlabS = Convert.ToInt16(NoofslabS[0]);

                    db.Entry(rm).State = EntityState.Modified;
                    db.SaveChanges();
                }


                var compid = comppid;

                ViewBag.Message = "RevLaptops Rate Updated SuccessFully";

                @ViewBag.Slabs1 = db.RateRevLaptops.Where(m => m.Company_id == compid).FirstOrDefault();

                ViewBag.RevLaptops = db.RateRevLaptops.Where(m => m.Company_id == compid).ToList();



                return PartialView("RateMasterRevLaptops", db.RateRevLaptops.Where(m => m.Company_id == compid && m.Sector.BillD == true).OrderBy(m => m.Sector.Priority).ToList());

            }
            return PartialView("RateMasterRevLaptops", fc);



        }

        //--------------------End---------------------------------------------


        public ActionResult Download(string id)
        {
            var invoice = db.Companies.Where(m => m.Company_Id == id).FirstOrDefault();
            
            //string savePath = Server.MapPath("/UploadedSoftCopy/" + invoice.DocumentFilepath.Replace('/', '-'));

           
           string savePath = "http://admin.atkxpress.com/UploadedSoftCopy/" + invoice.DocumentFilepath.Replace('/', '-');
            return Redirect(savePath);
        }

        public ActionResult UploadDocument()
        {
            return PartialView();
        }

        [HttpPost]
        public ActionResult UploadDocument(AddlogoModel logo, string Company_Id,Company empmodel)
        {
            var r = new Regex(@"([a-zA-Z0-9\s_\\.\-:])+(.pdf|.doc|.docx|.DOC|.DOCX)$");
            if (!r.IsMatch(logo.file.FileName))
            {             
                TempData["Success"] = "Only PDF/Word files allowed!";
            }
            else
            {              
                string _FileName = "";
                string _path = "";

                if (logo.file.ContentLength > 0)
                {
                    _FileName = Path.GetFileName(logo.file.FileName);
                    _path = Server.MapPath("~/UploadedSoftCopy/") + _FileName;
                    logo.file.SaveAs(_path);
                }
                var lo = (from d in db.Companies
                          where d.Company_Id == Company_Id
                          select d).FirstOrDefault();


                lo.DocumentFilepath = _FileName;

                db.Entry(lo).State = EntityState.Modified;
                db.SaveChanges();
                TempData["Success"] = "File Uploaded Successfully!";

                @ViewBag.Slabs = db.Ratems.Where(m => m.Company_id == Company_Id).FirstOrDefault();

                ViewBag.Company = new Company();

                ViewBag.Dox = db.Ratems.Where(m => m.Company_id == "Bala").ToList();

                List<SelectListItem> items1 = new List<SelectListItem>();

                items1.Add(new SelectListItem { Text = "--Select--", Value = "--Select--" });
                items1.Add(new SelectListItem { Text = "IsRegister", Value = "IsRegister" });
                items1.Add(new SelectListItem { Text = "NotRegister", Value = "NotRegister" });
                

                ViewBag.IsRegister = items1;

                List<SelectListItem> itemsproduct1 = new List<SelectListItem>();

                itemsproduct1.Add(new SelectListItem { Text = "Dox", Value = "Ratem" });

                itemsproduct1.Add(new SelectListItem { Text = "NonDox", Value = "NonDox" });

                itemsproduct1.Add(new SelectListItem { Text = "DTDCPLus", Value = "DTDCPLus" });

                itemsproduct1.Add(new SelectListItem { Text = "DTDCPTP", Value = "DTDCPTP" });

                itemsproduct1.Add(new SelectListItem { Text = "ExpressCargo", Value = "ExpressCargo" });

                itemsproduct1.Add(new SelectListItem { Text = "Priority", Value = "Priority" });

                itemsproduct1.Add(new SelectListItem { Text = "Laptops", Value = "Laptops" });

                itemsproduct1.Add(new SelectListItem { Text = "RevPickupLaptops", Value = "RevPickupLaptops" });

                itemsproduct1.Add(new SelectListItem { Text = "Ecommerce", Value = "Ecommerce" });
                itemsproduct1.Add(new SelectListItem { Text = "GECMode", Value = "GECMode" });

                ViewBag.Producttype = itemsproduct1;

                ViewBag.Pf_code = new SelectList(db.Franchisees.Where(x => !x.PF_Code.ToUpper().StartsWith("PR")), "PF_Code", "PF_Code", empmodel.Pf_code);

                return RedirectToAction("Index", "RateMaster", new { id = Company_Id });
                //return PartialView("RateMasterEditCompany", empmodel);
            }

            @ViewBag.Slabs = db.Ratems.Where(m => m.Company_id == Company_Id).FirstOrDefault();

            ViewBag.Company = new Company();

            ViewBag.Dox = db.Ratems.Where(m => m.Company_id == "Bala").ToList();

            List<SelectListItem> items = new List<SelectListItem>();

            items.Add(new SelectListItem { Text = "--Select--", Value = "--Select--" });
            items.Add(new SelectListItem { Text = "IsRegister", Value = "IsRegister" });
            items.Add(new SelectListItem { Text = "NotRegister", Value = "NotRegister" });


            ViewBag.IsRegister = items;


            List<SelectListItem> itemsproduct2 = new List<SelectListItem>();

            itemsproduct2.Add(new SelectListItem { Text = "Dox", Value = "Ratem" });

            itemsproduct2.Add(new SelectListItem { Text = "NonDox", Value = "NonDox" });

            itemsproduct2.Add(new SelectListItem { Text = "DTDCPLus", Value = "DTDCPLus" });

            itemsproduct2.Add(new SelectListItem { Text = "DTDCPTP", Value = "DTDCPTP" });

            itemsproduct2.Add(new SelectListItem { Text = "ExpressCargo", Value = "ExpressCargo" });

            itemsproduct2.Add(new SelectListItem { Text = "Priority", Value = "Priority" });

            itemsproduct2.Add(new SelectListItem { Text = "Laptops", Value = "Laptops" });

            itemsproduct2.Add(new SelectListItem { Text = "RevPickupLaptops", Value = "RevPickupLaptops" });

            itemsproduct2.Add(new SelectListItem { Text = "Ecommerce", Value = "Ecommerce" });
            itemsproduct2.Add(new SelectListItem { Text = "GECMode", Value = "GECMode" });


            ViewBag.Producttype = itemsproduct2;

            ViewBag.Pf_code = new SelectList(db.Franchisees.Where(x => !x.PF_Code.ToUpper().StartsWith("PR")), "PF_Code", "PF_Code", empmodel.Pf_code);
            //return PartialView("RateMasterEditCompany", empmodel);
            return RedirectToAction("Index", "RateMaster", new { id = Company_Id });
            //return PartialView(logo);

        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        [HandleError]
        public ActionResult RateMasterEcommerce(FormCollection fc, string comppid,float PAdditionalPerKG,float GEAdditionalPerKG)
        {

            comppid = comppid.Replace("__", "&").Replace("xdotx", "."); ;
            var CompanyId = comppid;
            ViewBag.companyid = comppid;
            if (ModelState.IsValid)
            {

                var ecom_idarray = fc.GetValues("item.Ecom_id");
                var PUpto500gm = fc.GetValues("item.PUpto500gm");
                var PAdd500gm = fc.GetValues("item.PAdd500gm");
                var PAdd5kg = fc.GetValues("item.PAdd5kg");
                var GEUpto500gm = fc.GetValues("item.GEUpto500gm");
                var GEAdd500gm = fc.GetValues("item.GEAdd500gm");
                var GEAdd5kg = fc.GetValues("item.GEAdd5kg");
               

                for (int i = 0; i < ecom_idarray.Count(); i++)
                {
                    if (PUpto500gm[i] == "")
                    {
                        PUpto500gm[i] = "0";
                    }
                    if (PAdd500gm[i] == "")
                    {
                        PAdd500gm[i] = "0";
                    }
                    if (PAdd5kg[i] == "")
                    {
                        PAdd5kg[i] = "0";
                    }
                    if (GEUpto500gm[i] == "")
                    {
                        GEUpto500gm[i] = "0";
                    }
                    if (GEAdd500gm[i] == "")
                    {
                        GEAdd500gm[i] = "0";
                    }
                    if (GEAdd5kg[i] == "")
                    {
                        GEAdd5kg[i] = "0";
                    }               
                }

                for (int i = 0; i < ecom_idarray.Count(); i++)
                {


                    Dtdc_Ecommerce rm = db.Dtdc_Ecommerce.Find(Convert.ToInt16(ecom_idarray[i]));


                    rm.PUpto500gm = Convert.ToDouble(PUpto500gm[i]);
                    rm.PAdd500gm = Convert.ToDouble(PAdd500gm[i]);
                    rm.PAdd5kg = Convert.ToDouble(PAdd5kg[i]);
                    rm.GEUpto500gm = Convert.ToDouble(GEUpto500gm[i]);
                    rm.GEAdd500gm = Convert.ToDouble(GEAdd500gm[i]);
                    rm.GEAdd5kg = Convert.ToDouble(GEAdd5kg[i]);
                    rm.PAdditionalPerKG = PAdditionalPerKG;
                    rm.GEAdditionalPerKG = GEAdditionalPerKG;
                    db.Entry(rm).State = EntityState.Modified;
                    db.SaveChanges();




                }

                var compid = comppid;

                ViewBag.Message = "Ecommerce Updated SuccessFully";

                @ViewBag.Slabs = db.Dtdc_Ecommerce.Where(m => m.Company_id == compid).FirstOrDefault();

                return PartialView("RateMasterEcommerce", db.Dtdc_Ecommerce.Where(m => m.Company_id == compid).Include(e => e.Sector).ToList());

            }

            return PartialView("RateMasterEcommerce", fc);



        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [HandleError]
        public ActionResult RateMasterNewEcommerce(FormCollection fc, string comppid)
        {

            comppid = comppid.Replace("__", "&").Replace("xdotx", "."); ;
            var CompanyId = comppid;
            ViewBag.companyid = comppid;
            if (ModelState.IsValid)
            {

                var ecom_idarray = fc.GetValues("item.Ecom_id");
                var EcomPslab1 = fc.GetValues("item.EcomPslab1");
                var EcomPslab2 = fc.GetValues("item.EcomPslab2");
                var EcomPslab3 = fc.GetValues("item.EcomPslab3");
                var EcomPslab4 = fc.GetValues("item.EcomPslab4");

                var EcomGEslab1 = fc.GetValues("item.EcomGEslab1");
                var EcomGEslab2 = fc.GetValues("item.EcomGEslab2");
                var EcomGEslab3 = fc.GetValues("item.EcomGEslab3");
                var EcomGEslab4 = fc.GetValues("item.EcomGEslab4");

                //var EcomPupto1 = fc.GetValues("item.EcomPupto1");
                //var EcomPupto2 = fc.GetValues("item.EcomPupto2");
                //var EcomPupto3 = fc.GetValues("item.EcomPupto3");
                //var EcomPupto4 = fc.GetValues("item.EcomPupto4");

                //var EcomGEupto1 = fc.GetValues("item.EcomGEupto1");
                //var EcomGEupto2 = fc.GetValues("item.EcomGEupto2");
                //var EcomGEupto3 = fc.GetValues("item.EcomGEupto3");
                //var EcomGEupto4 = fc.GetValues("item.EcomGEupto4");

                var Auptoarray = fc.GetValues("AUpto");
                var Suptoarray = fc.GetValues("SUpto");

                var NoOfSlabN = fc.GetValues("item.NoOfSlabN");
                var NoOfSlabS = fc.GetValues("item.NoOfSlabS");


                for (int i = 0; i < ecom_idarray.Count(); i++)
                {
                    if (EcomPslab1[i] == "")
                    {
                        EcomPslab1[i] = "0";
                    }
                    
                    
                    
                    
                    
                    
                    if (EcomPslab2[i] == "")
                    {
                        EcomPslab2[i] = "0";
                    }
                    if (EcomPslab3[i] == "")
                    {
                        EcomPslab3[i] = "0";
                    }
                    if (EcomPslab4[i] == "")
                    {
                        EcomPslab4[i] = "0";
                    }
                    if (EcomGEslab1[i] == "")
                    {
                        EcomGEslab1[i] = "0";
                    }
                    if (EcomGEslab2[i] == "")
                    {
                        EcomGEslab2[i] = "0";
                    }
                    if (EcomGEslab3[i] == "")
                    {
                        EcomGEslab3[i] = "0";
                    }
                    if (EcomGEslab4[i] == "")
                    {
                        EcomGEslab4[i] = "0";
                    }

                }

                for (int i = 0; i < Auptoarray.Count(); i++)
                {
                    if (Auptoarray[i] == "")
                    {
                        Auptoarray[i] = "0";
                    }
                    if (Suptoarray[i] == "")
                    {
                        Suptoarray[i] = "0";
                    }
                }


                for (int i = 0; i < ecom_idarray.Count(); i++)
                {


                    NewDtdc_Ecommerce rm = db.NewDtdc_Ecommerce.Find(Convert.ToInt16(ecom_idarray[i]));


                    rm.EcomPslab1 = Convert.ToDouble(EcomPslab1[i]);
                    rm.EcomPslab2 = Convert.ToDouble(EcomPslab2[i]);
                    rm.EcomPslab3 = Convert.ToDouble(EcomPslab3[i]);
                    rm.EcomPslab4 = Convert.ToDouble(EcomPslab4[i]);

                    rm.EcomGEslab1 = Convert.ToDouble(EcomGEslab1[i]);
                    rm.EcomGEslab2 = Convert.ToDouble(EcomGEslab2[i]);
                    rm.EcomGEslab3 = Convert.ToDouble(EcomGEslab3[i]);
                    rm.EcomGEslab4 = Convert.ToDouble(EcomGEslab4[i]);

                    rm.EcomPupto1 = Convert.ToDouble(Auptoarray[0]);
                    rm.EcomPupto2 = Convert.ToDouble(Auptoarray[1]);
                    rm.EcomPupto3 = Convert.ToDouble(Auptoarray[2]);
                    rm.EcomPupto4 = Convert.ToDouble(Auptoarray[3]);

                    rm.EcomGEupto1 = Convert.ToDouble(Suptoarray[0]);
                    rm.EcomGEupto2 = Convert.ToDouble(Suptoarray[1]);
                    rm.EcomGEupto3 = Convert.ToDouble(Suptoarray[2]);
                    rm.EcomGEupto4 = Convert.ToDouble(Suptoarray[3]);

                    rm.NoOfSlabN = Convert.ToInt16(NoOfSlabN[0]);
                    rm.NoOfSlabS = Convert.ToInt16(NoOfSlabS[0]);

                    db.Entry(rm).State = EntityState.Modified;
                    db.SaveChanges();

                }

                var compid = comppid;

                ViewBag.Message = "Ecommerce Updated SuccessFully";


                var getData = db.NewDtdc_Ecommerce.Where(m => m.Company_id == CompanyId).Include(e => e.Sector).OrderBy(m => m.Sector.Priority).ToList();
               
               // var getData = db.Dtdc_Ecommerce.Where(m => m.Company_id == compid).ToList();

                ViewBag.com = getData.FirstOrDefault();
                ViewBag.Ecommerce = getData;

                return PartialView("RateMasterNewEcommerce", getData);

            }

            return PartialView("RateMasterNewEcommerce", fc);

        }

    }
}
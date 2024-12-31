using DtDc_Billing.Entity_FR;
using DtDc_Billing.Models;
using DtdcCashCounter.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DtDc_Billing.Controllers
{
    [SessionAdmin]
    public class CashBookingController : Controller
    {

        private DB_A43B74_wingsgrowdbEntities db = new DB_A43B74_wingsgrowdbEntities();
        // GET: CashBooking
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult SenderPhoneAutocomplete()
        {


            var entity = db.Receipt_details.
Select(e => new
{
    e.sender_phone
}).Distinct().ToList();


            return Json(entity, JsonRequestBehavior.AllowGet);
        }


        public ActionResult ReceipentsPhoneAutocomplete()
        {


            var entity = db.Receipt_details.
Select(e => new
{
    e.Reciepents_phone
}).Distinct().ToList();


            return Json(entity, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ReceipentsCityAutocomplete()
        {

            var entity = db.Destinations.
Select(e => new
{
    e.Name
}).Distinct().ToList();


            return Json(entity, JsonRequestBehavior.AllowGet);
        }

        public JsonResult FillSenderdetails(string sender_phone)
        {

            db.Configuration.ProxyCreationEnabled = false;


            var suggestions = (from s in db.Receipt_details
                               where s.sender_phone == sender_phone
                               select s).FirstOrDefault();

            return Json(suggestions, JsonRequestBehavior.AllowGet);
        }

        public JsonResult FillReceipentsdetails(string phone)
        {
            db.Configuration.ProxyCreationEnabled = false;

            var suggestions = (from s in db.Receipt_details
                               where s.Reciepents_phone == phone
                               select s).FirstOrDefault();

            return Json(suggestions, JsonRequestBehavior.AllowGet);
        }

        public JsonResult FillDestination(string pincode)
        {
            var suggestions = from s in db.Destinations
                              where s.Pincode == pincode
                              select s;

            return Json(suggestions, JsonRequestBehavior.AllowGet);
        }


        public JsonResult FillSenderCityState(string pincode)
        {
            var suggestions = from s in db.Destinations
                              where s.Pincode == pincode
                              select s;

            return Json(suggestions, JsonRequestBehavior.AllowGet);
        }


        public JsonResult FillRecepentsPincode(string Name)
        {
            var suggestions = from s in db.Destinations
                              where s.Name == Name
                              select s;

            return Json(suggestions, JsonRequestBehavior.AllowGet);
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

        public string Walletamount(string sender_phone)
        {
            WalletPoint suggestions = (from s in db.WalletPoints
                                       where s.MobileNo == sender_phone
                                       select s).FirstOrDefault();

            if (suggestions == null)
            {
                return "0";
            }
            else
            {
                return suggestions.Wallet_Money.ToString();
            }




        }


        public JsonResult Calculation(float Actualwaight, float VolumetricWaight, string ShipmentType, string Pincode)
        {
            double? DoxNonDoxAmt = 10;

            string pfcode = Session["pfCode"].ToString();

            List<JsonArrayCalc> jsonarray = new List<JsonArrayCalc>();



            List<Sector> sector = db.Sectors.Where(m => m.Pf_code == pfcode && (m.CashD == true || m.CashN == true)).ToList();


            float highwaight;

            if (Actualwaight > VolumetricWaight)
            {
                highwaight = Actualwaight;
            }
            else
            {
                highwaight = VolumetricWaight;
            }

            string CashRate = string.Concat("Cash_", pfcode);


            int sectorfound = (db.Sectors.Where(m => m.Sector_Name == "Rest Of India" && m.Pf_code == pfcode).Select(m => m.Sector_Id).FirstOrDefault());

            int flag = 0;
            foreach (var i in sector)
            {
                string[] sectarray = i.Pincode_values.Split(',');

                foreach (var m in sectarray)
                {
                    if (m.Contains("-"))
                    {
                        string[] pinarr = m.Split('-');

                        if (Convert.ToInt64(Pincode) >= Convert.ToInt64(pinarr[0]) && Convert.ToInt64(Pincode) <= Convert.ToInt64(pinarr[1]))
                        {
                            sectorfound = i.Sector_Id;
                            flag = 1;
                            break;

                        }
                    }
                    else if (m == Pincode)
                    {
                        sectorfound = i.Sector_Id;
                    }


                }
                if (flag == 1)
                {
                    break;
                }
            }



            if (true)//ShipmentType == "D"
            {
                Ratem dox = db.Ratems.Where(m => m.Sector_Id == sectorfound && m.Company_id == CashRate).FirstOrDefault();

                if (dox.NoOfSlab == 2)
                {
                    if (highwaight <= dox.Uptosl1)
                    {
                        DoxNonDoxAmt = Convert.ToDouble(dox.slab1);
                    }
                    else
                    {
                        // 0.500 /  (2 - 0.25)

                        double weightmod = (highwaight - Convert.ToDouble(dox.Uptosl1)) / Convert.ToDouble(dox.Uptosl4);

                        weightmod = Math.Ceiling(weightmod);

                        DoxNonDoxAmt = Convert.ToDouble(dox.slab1 + (dox.slab4 * weightmod));
                    }
                }
                else if (dox.NoOfSlab == 3)
                {
                    if (highwaight <= dox.Uptosl1)
                    {
                        DoxNonDoxAmt = Convert.ToDouble(dox.slab1);
                    }
                    else if (highwaight <= dox.Uptosl2)
                    {
                        DoxNonDoxAmt = Convert.ToDouble(dox.slab2);
                    }
                    else
                    {
                        double weightmod = (highwaight - Convert.ToDouble(dox.Uptosl2)) / Convert.ToDouble(dox.Uptosl4);

                        weightmod = Math.Ceiling(weightmod);

                        DoxNonDoxAmt = Convert.ToDouble(dox.slab2 + (dox.slab4 * weightmod));
                    }



                }
                else if (dox.NoOfSlab == 4)
                {

                    if (highwaight <= dox.Uptosl1)
                    {
                        DoxNonDoxAmt = Convert.ToDouble(dox.slab1);
                    }

                    else if (highwaight <= dox.Uptosl2)
                    {
                        DoxNonDoxAmt = Convert.ToDouble(dox.slab2);
                    }

                    else if (highwaight <= dox.Uptosl3)
                    {
                        DoxNonDoxAmt = Convert.ToDouble(dox.slab3);
                    }
                    else
                    {
                        double weightmod = (highwaight - Convert.ToDouble(dox.Uptosl3)) / Convert.ToDouble(dox.Uptosl4);

                        weightmod = Math.Ceiling(weightmod);

                        DoxNonDoxAmt = Convert.ToDouble(dox.slab3 + (dox.slab4 * weightmod));
                    }

                }



                JsonArrayCalc js = new JsonArrayCalc();
                js.name = "Doxamount";
                js.Amount = DoxNonDoxAmt;
                jsonarray.Add(js);

                // return Json(new { DoxAmount = Amount });
            }
            if (ShipmentType == "N")
            {
                Nondox nondox = db.Nondoxes.Where(m => m.Sector_Id == sectorfound && m.Company_id == CashRate).FirstOrDefault();

                double? AirAmount = 0.0;

                if (nondox.NoOfSlabN == 2)
                {
                    if (highwaight <= nondox.AUptosl1)
                    {
                        AirAmount = Convert.ToDouble(nondox.Aslab1) * nondox.AUptosl1;
                    }
                    else
                    {
                        double weightmod = (highwaight - Convert.ToDouble(nondox.AUptosl1)) / Convert.ToDouble(nondox.AUptosl4);

                        weightmod = Math.Ceiling(weightmod);

                        AirAmount = Convert.ToDouble((nondox.Aslab1 * nondox.AUptosl1) + (nondox.Aslab4 * weightmod));
                    }
                }
                else if (nondox.NoOfSlabN == 3)
                {
                    if (highwaight <= nondox.AUptosl1)
                    {
                        AirAmount = Convert.ToDouble(nondox.Aslab1);
                    }
                    else if (highwaight <= nondox.AUptosl2)
                    {
                        AirAmount = Convert.ToDouble(nondox.Aslab2);
                    }
                    else
                    {
                        double weightmod = (highwaight - Convert.ToDouble(nondox.AUptosl2)) / Convert.ToDouble(nondox.AUptosl4);

                        weightmod = Math.Ceiling(weightmod);

                        AirAmount = Convert.ToDouble(nondox.Aslab2 + (nondox.Aslab4 * weightmod));
                    }



                }
                else if (nondox.NoOfSlabN == 4)
                {

                    if (highwaight <= nondox.AUptosl1)
                    {
                        AirAmount = Convert.ToDouble(nondox.Aslab1) * nondox.AUptosl1;
                    }

                    else if (highwaight <= nondox.AUptosl2)
                    {
                        AirAmount = Convert.ToDouble(nondox.Aslab2);
                    }
                    else if (highwaight <= nondox.AUptosl3)
                    {
                        AirAmount = Convert.ToDouble(nondox.Aslab3);
                    }
                    else
                    {
                        double weightmod = (highwaight - Convert.ToDouble(nondox.AUptosl3)) / Convert.ToDouble(nondox.AUptosl4);

                        weightmod = Math.Ceiling(weightmod);

                        AirAmount = Convert.ToDouble(nondox.Aslab3 + (nondox.Aslab4 * weightmod));
                    }

                }



                JsonArrayCalc js = new JsonArrayCalc();
                js.name = "AirAmount";
                js.Amount = AirAmount;
                jsonarray.Add(js);

                ///////////////////////////////////Air Surface /////////////////////////////

                Nondox nondox1 = db.Nondoxes.Where(m => m.Sector_Id == sectorfound && m.Company_id == CashRate).FirstOrDefault();
                double? Amountsurf = 0.0;
                //double? Amount1;
                //double? Min_Weight;

                if (nondox.NoOfSlabS == 2)
                {
                    if (highwaight <= nondox.SUptosl1)
                    {
                        Amountsurf = Convert.ToDouble(nondox.Sslab1) * nondox.SUptosl1;
                    }
                    else
                    {
                        double weightmod = (highwaight - Convert.ToDouble(nondox.SUptosl1)) / Convert.ToDouble(nondox.SUptosl4);

                        weightmod = Math.Ceiling(weightmod);


                        Amountsurf = Convert.ToDouble((nondox.Sslab1 * nondox.SUptosl1) + (nondox.Sslab4 * weightmod));
                    }
                }
                else if (nondox.NoOfSlabS == 3)
                {
                    if (highwaight <= nondox.SUptosl1)
                    {
                        //Amountsurf = Convert.ToDouble(nondox.Sslab1);
                        Amountsurf = Convert.ToDouble(nondox.Sslab1) * nondox.SUptosl1;
                    }
                    else if (highwaight <= nondox.SUptosl2)
                    {
                        // Amountsurf = Convert.ToDouble(nondox.Sslab2);
                        Amountsurf = Convert.ToDouble(nondox.Sslab2) * nondox.SUptosl2;
                    }
                    else
                    {
                        double weightmod = (highwaight - Convert.ToDouble(nondox.SUptosl2)) / Convert.ToDouble(nondox.SUptosl4);

                        weightmod = Math.Ceiling(weightmod);

                        Amountsurf = Convert.ToDouble((nondox.Sslab2 * nondox.SUptosl2) + (nondox.Sslab4 * weightmod));
                    }


                }
                else if (nondox.NoOfSlabS == 4)
                {

                    if (highwaight <= nondox.SUptosl1)
                    {
                        //Amountsurf = Convert.ToDouble(nondox.Sslab1);
                        Amountsurf = Convert.ToDouble(nondox.Sslab1) * nondox.SUptosl1;

                    }

                    else if (highwaight <= nondox.SUptosl2)
                    {
                        // Amountsurf = Convert.ToDouble(nondox.Sslab2);
                        Amountsurf = Convert.ToDouble(nondox.Sslab2) * nondox.SUptosl2;
                    }
                    else if (highwaight <= nondox.SUptosl3)
                    {
                        Amountsurf = Convert.ToDouble(nondox.Sslab3);
                        Amountsurf = Convert.ToDouble(nondox.Sslab3) * nondox.SUptosl3;
                    }
                    else
                    {
                        double weightmod = (highwaight - Convert.ToDouble(nondox.SUptosl3)) / Convert.ToDouble(nondox.SUptosl4);

                        weightmod = Math.Ceiling(weightmod);

                        Amountsurf = Convert.ToDouble((nondox.Sslab3 * nondox.SUptosl3) + (nondox.Sslab4 * weightmod));
                    }

                }




                JsonArrayCalc jssurf = new JsonArrayCalc();
                jssurf.name = "Amountsurf";
                jssurf.Amount = Amountsurf;
                jsonarray.Add(jssurf);

                //return Json(new { nonAisr = Amount, nonsurf = Amountsurf });

            }




            //long pin = Pincode;

            List<Service_list> service_List = db.Service_list.Where(m => m.Pincode == Pincode).ToList();



            for (int i = 0; i < service_List.Count; i++)
            {

                double? amount1;
                double? amount2;

                if (service_List[i].Service_ == "CITY PRIMETIME PLUS - 12:00")
                {
                    Dtdc_Ptp dtdc_Ptp = db.Dtdc_Ptp.Where(m => m.dest.Contains("City") && m.Company_id == CashRate).FirstOrDefault();



                    if (highwaight <= 10)
                    {

                        amount1 = dtdc_Ptp.PUpto500gm;


                        if (highwaight > 0.500)
                        {
                            double weightmod = (highwaight - 0.500) / Convert.ToDouble(0.500);

                            weightmod = Math.Ceiling(weightmod);

                            amount1 = amount1 + (dtdc_Ptp.PAdd500gm * weightmod);
                        }

                    }
                    else if (highwaight > 10 && highwaight <= 25)
                    {
                        amount1 = Math.Ceiling(highwaight) * dtdc_Ptp.PU10to25kg;
                    }
                    else if (highwaight > 25 && highwaight <= 50)
                    {
                        amount1 = Math.Ceiling(highwaight) * dtdc_Ptp.PU25to50;
                    }
                    else if (highwaight > 50 && highwaight <= 100)
                    {
                        amount1 = Math.Ceiling(highwaight) * dtdc_Ptp.PU50to100;
                    }
                    else
                    {
                        amount1 = Math.Ceiling(highwaight) * dtdc_Ptp.Padd100kg;
                    }



                    JsonArrayCalc js = new JsonArrayCalc();
                    js.name = service_List[i].Service_;
                    js.Amount = amount1;
                    jsonarray.Add(js);

                }
                else if (service_List[i].Service_ == "CITY PRIMETIME PLUS - 2 PM")
                {

                    Dtdc_Ptp dtdc_Ptp = db.Dtdc_Ptp.Where(m => m.dest.Contains("City") && m.Company_id == CashRate).FirstOrDefault();



                    if (highwaight <= 10)
                    {

                        amount1 = dtdc_Ptp.P2Upto500gm;


                        if (highwaight > 0.500)
                        {
                            double weightmod = (highwaight - Convert.ToDouble(0.500)) / Convert.ToDouble(0.500);

                            weightmod = Math.Ceiling(weightmod);

                            amount1 = amount1 + (dtdc_Ptp.P2Add500gm * weightmod);
                        }

                    }
                    else if (highwaight > 10 && highwaight <= 25)
                    {
                        amount1 = Math.Ceiling(highwaight) * dtdc_Ptp.P2U10to25kg;
                    }
                    else if (highwaight > 25 && highwaight <= 50)
                    {
                        amount1 = Math.Ceiling(highwaight) * dtdc_Ptp.P2U25to50;
                    }
                    else if (highwaight > 50 && highwaight <= 100)
                    {
                        amount1 = Math.Ceiling(highwaight) * dtdc_Ptp.P2U50to100;
                    }
                    else
                    {
                        amount1 = Math.Ceiling(highwaight) * dtdc_Ptp.P2add100kg;
                    }

                    JsonArrayCalc js = new JsonArrayCalc();
                    js.name = service_List[i].Service_;
                    js.Amount = amount1;
                    jsonarray.Add(js);
                }
                else if (service_List[i].Service_ == "ZONAL PRIMETIME PLUS 12.00")
                {

                    Dtdc_Ptp dtdc_Ptp = db.Dtdc_Ptp.Where(m => m.dest.Contains("ZONAL") && m.Company_id == CashRate).FirstOrDefault();



                    if (highwaight <= 10)
                    {

                        amount1 = dtdc_Ptp.PUpto500gm;


                        if (highwaight > 0.500)
                        {
                            double weightmod = (highwaight - Convert.ToDouble(0.500)) / Convert.ToDouble(0.500);

                            weightmod = Math.Ceiling(weightmod);

                            amount1 = amount1 + (dtdc_Ptp.PAdd500gm * weightmod);
                        }

                    }
                    else if (highwaight > 10 && highwaight <= 25)
                    {
                        amount1 = Math.Ceiling(highwaight) * dtdc_Ptp.PU10to25kg;
                    }
                    else if (highwaight > 25 && highwaight <= 50)
                    {
                        amount1 = Math.Ceiling(highwaight) * dtdc_Ptp.PU25to50;
                    }
                    else if (highwaight > 50 && highwaight <= 100)
                    {
                        amount1 = Math.Ceiling(highwaight) * dtdc_Ptp.PU50to100;
                    }
                    else
                    {
                        amount1 = Math.Ceiling(highwaight) * dtdc_Ptp.Padd100kg;
                    }


                    JsonArrayCalc js = new JsonArrayCalc();
                    js.name = service_List[i].Service_;
                    js.Amount = amount1;
                    jsonarray.Add(js);

                }
                else if (service_List[i].Service_ == "ZONAL PRIMETIME PLUS - 2 PM")
                {
                    Dtdc_Ptp dtdc_Ptp = db.Dtdc_Ptp.Where(m => m.dest.Contains("ZONAL") && m.Company_id == CashRate).FirstOrDefault();



                    if (highwaight <= 10)
                    {

                        amount1 = dtdc_Ptp.P2Upto500gm;


                        if (highwaight > 0.500)
                        {
                            double weightmod = (highwaight - Convert.ToDouble(0.500)) / Convert.ToDouble(0.500);

                            weightmod = Math.Ceiling(weightmod);

                            amount1 = amount1 + (dtdc_Ptp.P2Add500gm * weightmod);
                        }

                    }
                    else if (highwaight > 10 && highwaight <= 25)
                    {
                        amount1 = Math.Ceiling(highwaight) * dtdc_Ptp.P2U10to25kg;
                    }
                    else if (highwaight > 25 && highwaight <= 50)
                    {
                        amount1 = Math.Ceiling(highwaight) * dtdc_Ptp.P2U25to50;
                    }
                    else if (highwaight > 50 && highwaight <= 100)
                    {
                        amount1 = Math.Ceiling(highwaight) * dtdc_Ptp.P2U50to100;
                    }
                    else
                    {
                        amount1 = Math.Ceiling(highwaight) * dtdc_Ptp.P2add100kg;
                    }

                    JsonArrayCalc js = new JsonArrayCalc();
                    js.name = service_List[i].Service_;
                    js.Amount = amount1;
                    jsonarray.Add(js);

                }
                else if (service_List[i].Service_ == "METRO PRIMETIME PLUS-12.00")
                {
                    Dtdc_Ptp dtdc_Ptp = db.Dtdc_Ptp.Where(m => m.dest.Contains("METRO") && m.Company_id == CashRate).FirstOrDefault();



                    if (highwaight <= 10)
                    {

                        amount1 = dtdc_Ptp.PUpto500gm;


                        if (highwaight > 0.500)
                        {
                            double weightmod = (highwaight - Convert.ToDouble(0.500)) / Convert.ToDouble(0.500);

                            weightmod = Math.Ceiling(weightmod);

                            amount1 = amount1 + (dtdc_Ptp.PAdd500gm * weightmod);
                        }

                    }
                    else if (highwaight > 10 && highwaight <= 25)
                    {
                        amount1 = Math.Ceiling(highwaight) * dtdc_Ptp.PU10to25kg;
                    }
                    else if (highwaight > 25 && highwaight <= 50)
                    {
                        amount1 = Math.Ceiling(highwaight) * dtdc_Ptp.PU25to50;
                    }
                    else if (highwaight > 50 && highwaight <= 100)
                    {
                        amount1 = Math.Ceiling(highwaight) * dtdc_Ptp.PU50to100;
                    }
                    else
                    {
                        amount1 = Math.Ceiling(highwaight) * dtdc_Ptp.Padd100kg;
                    }

                    JsonArrayCalc js = new JsonArrayCalc();
                    js.name = service_List[i].Service_;
                    js.Amount = amount1;
                    jsonarray.Add(js);

                }
                else if (service_List[i].Service_ == "METRO PRIMETIME PLUS - 2 PM")
                {
                    Dtdc_Ptp dtdc_Ptp = db.Dtdc_Ptp.Where(m => m.dest.Contains("METRO") && m.Company_id == CashRate).FirstOrDefault();



                    if (highwaight <= 10)
                    {

                        amount1 = dtdc_Ptp.P2Upto500gm;


                        if (highwaight > 0.500)
                        {
                            double weightmod = (highwaight - Convert.ToDouble(0.500)) / Convert.ToDouble(0.500);

                            weightmod = Math.Ceiling(weightmod);

                            amount1 = amount1 + (dtdc_Ptp.P2Add500gm * weightmod);
                        }

                    }
                    else if (highwaight > 10 && highwaight <= 25)
                    {
                        amount1 = Math.Ceiling(highwaight) * dtdc_Ptp.P2U10to25kg;
                    }
                    else if (highwaight > 25 && highwaight <= 50)
                    {
                        amount1 = Math.Ceiling(highwaight) * dtdc_Ptp.P2U25to50;
                    }
                    else if (highwaight > 50 && highwaight <= 100)
                    {
                        amount1 = Math.Ceiling(highwaight) * dtdc_Ptp.P2U50to100;
                    }
                    else
                    {
                        amount1 = Math.Ceiling(highwaight) * dtdc_Ptp.P2add100kg;
                    }


                    JsonArrayCalc js = new JsonArrayCalc();
                    js.name = service_List[i].Service_;
                    js.Amount = amount1;
                    jsonarray.Add(js);
                }
                else if (service_List[i].Service_ == "CITY OFFICE COLLECT PLUS" || service_List[i].Service_ == "CITY OFFICE COLLECT PLUS")
                {
                    dtdcPlu dtdc_plus = db.dtdcPlus.Where(m => m.destination.Contains("CITY") && m.Company_id == CashRate).FirstOrDefault();



                    if (highwaight <= 10)
                    {

                        amount1 = dtdc_plus.Upto500gm;


                        if (highwaight > 0.500)
                        {
                            double weightmod = (highwaight - Convert.ToDouble(0.500)) / Convert.ToDouble(0.500);

                            weightmod = Math.Ceiling(weightmod);

                            amount1 = amount1 + (dtdc_plus.Add500gm * weightmod);
                        }

                    }
                    else if (highwaight > 10 && highwaight <= 25)
                    {
                        amount1 = Math.Ceiling(highwaight) * dtdc_plus.U10to25kg;
                    }
                    else if (highwaight > 25 && highwaight <= 50)
                    {
                        amount1 = Math.Ceiling(highwaight) * dtdc_plus.U25to50;
                    }
                    else if (highwaight > 50 && highwaight <= 100)
                    {
                        amount1 = Math.Ceiling(highwaight) * dtdc_plus.U50to100;
                    }
                    else
                    {
                        amount1 = Math.Ceiling(highwaight) * dtdc_plus.add100kg;
                    }

                    JsonArrayCalc js = new JsonArrayCalc();
                    js.name = service_List[i].Service_;
                    js.Amount = amount1;
                    jsonarray.Add(js);

                }
                else if (service_List[i].Service_ == "ZONAL OFFICE COLLECT PLUS" || service_List[i].Service_ == "ZONAL BLUE" || service_List[i].Service_ == "ZONAL OFFICE COLLECT BLUE" || service_List[i].Service_ == "ZONAL OFFICE COLLECT PLUS" || service_List[i].Service_ == "ZONAL PLUS" || service_List[i].Service_ == "ZONAL PLUS PEC" || service_List[i].Service_ == "SPECIAL ZONAL 2 BLUE" || service_List[i].Service_ == "SPECIAL ZONAL 2 PLUS" || service_List[i].Service_ == "ZONAL GREEN" || service_List[i].Service_ == "ZONAL OFFICE COLLECT GREEN" || service_List[i].Service_ == "ZONAL BLUE PEC" || service_List[i].Service_ == "SPECIAL ZONAL BLUE" || service_List[i].Service_ == "SPECIAL ZONAL PLUS" || service_List[i].Service_ == "SPECIAL ZONAL PLUS PEC" || service_List[i].Service_ == "SPECIAL ZONAL BLUE PEC" || service_List[i].Service_ == "SPECIAL ZONAL GREEN" || service_List[i].Service_ == "PEC ZONAL BLUE 2- SPECIAL" || service_List[i].Service_ == "PEC ZONAL PLUS 2- SPECIAL")
                {
                    dtdcPlu dtdc_plus = db.dtdcPlus.Where(m => m.destination.Contains("ZONAL") && m.Company_id == CashRate).FirstOrDefault();



                    if (highwaight <= 10)
                    {

                        amount1 = dtdc_plus.Upto500gm;


                        if (highwaight > 0.500)
                        {
                            double weightmod = (highwaight - Convert.ToDouble(0.500)) / Convert.ToDouble(0.500);

                            weightmod = Math.Ceiling(weightmod);

                            amount1 = amount1 + (dtdc_plus.Add500gm * weightmod);
                        }

                    }
                    else if (highwaight > 10 && highwaight <= 25)
                    {
                        amount1 = Math.Ceiling(highwaight) * dtdc_plus.U10to25kg;
                    }
                    else if (highwaight > 25 && highwaight <= 50)
                    {
                        amount1 = Math.Ceiling(highwaight) * dtdc_plus.U25to50;
                    }
                    else if (highwaight > 50 && highwaight <= 100)
                    {
                        amount1 = Math.Ceiling(highwaight) * dtdc_plus.U50to100;
                    }
                    else
                    {
                        amount1 = Math.Ceiling(highwaight) * dtdc_plus.add100kg;
                    }
                    JsonArrayCalc js = new JsonArrayCalc();
                    js.name = service_List[i].Service_;
                    js.Amount = amount1;
                    jsonarray.Add(js);
                }
                else if (service_List[i].Service_ == "METRO BLUE" || service_List[i].Service_ == "METRO OFFICE COLLECT BLUE" || service_List[i].Service_ == "METRO OFFICE COLLECT PLUS" || service_List[i].Service_ == "METRO PLUS" || service_List[i].Service_ == "METRO PLUS PEC")
                {
                    dtdcPlu dtdc_plus = db.dtdcPlus.Where(m => m.destination.Contains("Metro") && m.Company_id == CashRate).FirstOrDefault();



                    if (highwaight <= 10)
                    {

                        amount1 = dtdc_plus.Upto500gm;


                        if (highwaight > 0.500)
                        {
                            double weightmod = (highwaight - Convert.ToDouble(0.500)) / Convert.ToDouble(0.500);

                            weightmod = Math.Ceiling(weightmod);

                            amount1 = amount1 + (dtdc_plus.Add500gm * weightmod);
                        }

                    }
                    else if (highwaight > 10 && highwaight <= 25)
                    {
                        amount1 = Math.Ceiling(highwaight) * dtdc_plus.U10to25kg;
                    }
                    else if (highwaight > 25 && highwaight <= 50)
                    {
                        amount1 = Math.Ceiling(highwaight) * dtdc_plus.U25to50;
                    }
                    else if (highwaight > 50 && highwaight <= 100)
                    {
                        amount1 = Math.Ceiling(highwaight) * dtdc_plus.U50to100;
                    }
                    else
                    {
                        amount1 = Math.Ceiling(highwaight) * dtdc_plus.add100kg;
                    }
                    JsonArrayCalc js = new JsonArrayCalc();
                    js.name = service_List[i].Service_;
                    js.Amount = amount1;
                    jsonarray.Add(js);
                }
                else if (service_List[i].Service_ == "NATIONAL BLUE" || service_List[i].Service_ == "NATIONAL OFFICE COLLECT BLUE" || service_List[i].Service_ == "NATIONAL OFFICE COLLECT PLUS" || service_List[i].Service_ == "NATIONAL PLUS" || service_List[i].Service_ == "NATIONAL BLUE PEC" || service_List[i].Service_ == "NATIONAL GREEN" || service_List[i].Service_ == "NATIONAL OFFICE COLLECT GREEN" || service_List[i].Service_ == "NATIONAL PLUS PEC")
                {
                    dtdcPlu dtdc_plus = db.dtdcPlus.Where(m => m.destination.Contains("NATIONAL") && m.Company_id == CashRate).FirstOrDefault();



                    if (highwaight <= 10)
                    {

                        amount1 = dtdc_plus.Upto500gm;


                        if (highwaight > 0.500)
                        {
                            double weightmod = (highwaight - Convert.ToDouble(0.500)) / Convert.ToDouble(0.500);

                            weightmod = Math.Ceiling(weightmod);

                            amount1 = amount1 + (dtdc_plus.Add500gm * weightmod);
                        }

                    }
                    else if (highwaight > 10 && highwaight <= 25)
                    {
                        amount1 = Math.Ceiling(highwaight) * dtdc_plus.U10to25kg;
                    }
                    else if (highwaight > 25 && highwaight <= 50)
                    {
                        amount1 = Math.Ceiling(highwaight) * dtdc_plus.U25to50;
                    }
                    else if (highwaight > 50 && highwaight <= 100)
                    {
                        amount1 = Math.Ceiling(highwaight) * dtdc_plus.U50to100;
                    }
                    else
                    {
                        amount1 = Math.Ceiling(highwaight) * dtdc_plus.add100kg;
                    }
                    JsonArrayCalc js = new JsonArrayCalc();
                    js.name = service_List[i].Service_;
                    js.Amount = amount1;
                    jsonarray.Add(js);
                }
                else if (service_List[i].Service_ == "REGIONAL PLUS" || service_List[i].Service_ == "REGIONAL PLUS PEC")
                {
                    dtdcPlu dtdc_plus = db.dtdcPlus.Where(m => m.destination.Contains("REGIONAL") && m.Company_id == CashRate).FirstOrDefault();



                    if (highwaight <= 10)
                    {

                        amount1 = dtdc_plus.Upto500gm;


                        if (highwaight > 0.500)
                        {
                            double weightmod = (highwaight - Convert.ToDouble(0.500)) / Convert.ToDouble(0.500);

                            weightmod = Math.Ceiling(weightmod);

                            amount1 = amount1 + (dtdc_plus.Add500gm * weightmod);
                        }

                    }
                    else if (highwaight > 10 && highwaight <= 25)
                    {
                        amount1 = Math.Ceiling(highwaight) * dtdc_plus.U10to25kg;
                    }
                    else if (highwaight > 25 && highwaight <= 50)
                    {
                        amount1 = Math.Ceiling(highwaight) * dtdc_plus.U25to50;
                    }
                    else if (highwaight > 50 && highwaight <= 100)
                    {
                        amount1 = Math.Ceiling(highwaight) * dtdc_plus.U50to100;
                    }
                    else
                    {
                        amount1 = Math.Ceiling(highwaight) * dtdc_plus.add100kg;
                    }

                    JsonArrayCalc js = new JsonArrayCalc();
                    js.name = service_List[i].Service_;
                    js.Amount = amount1;
                    jsonarray.Add(js);
                }

                else if (service_List[i].Service_ == "GROUND EXPRESS CARGO")
                {
                    express_cargo express = db.express_cargo.Where(m => m.Sector_Id == sectorfound && m.Company_id == CashRate).FirstOrDefault();
                    double? Amount;
                    double? Amount1;
                    double? Min_Weight;


                    {
                        if (highwaight <= 50)
                        {
                            Amount = Convert.ToDouble(express.Exslab1);
                        }
                        else
                        {


                            Amount = Math.Ceiling(highwaight) * express.Exslab2;

                        }




                    }

                    //return Json(new { ExpAmount = Amount });
                    JsonArrayCalc js = new JsonArrayCalc();
                    js.name = service_List[i].Service_;
                    js.Amount = Amount;
                    jsonarray.Add(js);
                }

            }


            //if Json ayyay amount is null then amount set to be zero

            List<JsonArrayCalc> jsonarrayForsendBack = new List<JsonArrayCalc>();

            foreach (JsonArrayCalc i in jsonarray)
            {
                if (i.Amount == null)
                {
                    i.Amount = 0;
                }
                jsonarrayForsendBack.Add(i);

            }


            ///////

            return Json(jsonarrayForsendBack, JsonRequestBehavior.AllowGet);
        }



        public string consignmentval(string Consignment)
        {
            string Consignmentno = db.Receipt_details.Where(m => m.Consignment_No == Consignment.ToUpper()).Select(m => m.Consignment_No).FirstOrDefault();

            if (Consignmentno == null)
            {
                return "1";

            }
            else
            {
                return "0";
            }
        }


        public JsonResult Consignment(string Consignment_No)
        {
            db.Configuration.ProxyCreationEnabled = false;

            Receipt_details rc = (from u in db.Receipt_details
                                  where u.Consignment_No == Consignment_No
                                  select u).FirstOrDefault();

            rc.CreateDateString = rc.Datetime_Cons.Value.Date.ToString("dd/MM/yyyy");

            return Json(rc, JsonRequestBehavior.AllowGet);
        }


        public ActionResult ConsignmentDetails()
        {

            return View();
        }


     
        [HttpPost]
        public ActionResult ConsignmentDetails(Receipt_details reciept_Details, string Submit, bool? DisableDisc)
        {

            ////credit Amount logic///
            reciept_Details.Paid_Amount = Convert.ToInt16(reciept_Details.Credit_Amount);
            /////////////////////////

            ViewBag.Insuarance = reciept_Details.Shipmenttype;
            ViewBag.yesinsurance = reciept_Details.Insurance;

            var consignment = db.Receipt_details.Where(m => m.Consignment_No == reciept_Details.Consignment_No).FirstOrDefault();

            ViewBag.dates = consignment.Datetime_Cons;
            if (consignment == null)
            {
                ModelState.AddModelError("Consignment", "Consignment Dosent Exist");
            }
            else if (ModelState.IsValid)
            {
                reciept_Details.Pf_Code = reciept_Details.Pf_Code.ToString();
                reciept_Details.User_Id = reciept_Details.User_Id;

             
                  
                    reciept_Details.secure_Pack = consignment.secure_Pack;
                    reciept_Details.Passport = consignment.Passport;
                    reciept_Details.OfficeSunday = consignment.OfficeSunday;
                reciept_Details.User_Id = consignment.User_Id;
                reciept_Details.Pf_Code = consignment.Pf_Code.ToString();
                reciept_Details.Datetime_Cons = consignment.Datetime_Cons;



                reciept_Details.Datetime_Cons = consignment.Datetime_Cons;



                reciept_Details.Receipt_Id = consignment.Receipt_Id;

                db.Entry(consignment).State = EntityState.Detached;
                //////////////////////////////////////////////////////////////////////////


                db.Entry(reciept_Details).State = EntityState.Modified;
                db.SaveChanges();


                ViewBag.Success = "Consignment Updated Successfully...!!!";
                ////////////////////////////////////////

                ModelState.Clear();
                return View(new Receipt_details());
            }
            //ViewBag.WalletPopints = reciept_Details.Discount;


            return View(reciept_Details);
        }
        [HttpGet]
        public JsonResult GetReuestToUpdateAmount()
        {
            var data = from r in db.Receipt_details
                       join u in db.Users on r.User_Id equals u.User_Id
                       where r.IsRequesttoChangeAmt ==true
                       select new
                       {
                           r.Receipt_Id,
                           r.Consignment_No,
                           r.Charges_Total,
                           r.UpdatedAmount,
                           r.Remark,
                           r.Pf_Code,
                           EmployeeName = u.Name // Adjust property name based on your model
                       };

            return Json(data, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public JsonResult ApproveRequest(int id)
        {
            var receipt = db.Receipt_details.Where(x=>x.Receipt_Id==id).FirstOrDefault();
            if (receipt != null)
            {
                receipt.IsApprove = true;
                receipt.Charges_Total =(float) receipt.UpdatedAmount;
                receipt.IsRequesttoChangeAmt = false;
                db.SaveChanges();
                return Json(new { message = "Request approved successfully." });
            }
            return Json(new { message = "Request not found." });
        }

        [HttpPost]
        public JsonResult RejectRequest(int id)
        {
            var receipt = db.Receipt_details.Where(x=>x.Receipt_Id==id).FirstOrDefault();
            if (receipt != null)
            {
                receipt.IsApprove = false;
                receipt.IsRequesttoChangeAmt = false;
                db.SaveChanges();
                return Json(new { message = "Request rejected successfully." });
            }
            return Json(new { message = "Request not found." });
        }

    }
}
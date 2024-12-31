using DtDc_Billing.Entity_FR;
using DtDc_Billing.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DtDc_Billing.Controllers
{
    [SessionAdmin]
    public class EditRateMasterController : Controller
    {
        private DB_A43B74_wingsgrowdbEntities db = new DB_A43B74_wingsgrowdbEntities();
        // GET: EditRateMaster

        //public ActionResult ListCompany()
        //{
            
        //    return View(db.Companies.Include(c => c.Franchisee).ToList());
        //}

        public ActionResult EditCompanyRate()
        {
          

            return View(db.Companies.ToList());
        }

        public ActionResult Edit(string id)
        {
            TempData["CompanyId"] = id;

            @ViewBag.Slabs = db.Ratems.Where(m => m.Company_id == id).FirstOrDefault();

            return PartialView("Editdox", db.Ratems.Where(m => m.Company_id == id).ToList());

            
        }
    }
}
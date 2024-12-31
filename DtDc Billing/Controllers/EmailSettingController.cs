using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using DtDc_Billing.Entity_FR;

namespace DtDc_Billing.Controllers
{
    public class EmailSettingController : Controller
    {
        private DB_A43B74_wingsgrowdbEntities db = new DB_A43B74_wingsgrowdbEntities();

        // GET: EmailSetting

        public ActionResult Index()
        {
            var data = db.EmailPromotions.ToList();
            return View(data);
        }

        // GET: EmailSetting/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            EmailPromotion emailPromotion = db.EmailPromotions.Find(id);
            if (emailPromotion == null)
            {
                return HttpNotFound();
            }
            return View(emailPromotion);
        }

        // GET: EmailSetting/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: EmailSetting/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public ActionResult Create(EmailPromotion emailPromotion)
        {
            if (ModelState.IsValid)
            {
                db.EmailPromotions.Add(emailPromotion);
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
                
                return RedirectToAction("Index");
            }

            return View(emailPromotion);
        }

        // GET: EmailSetting/Edit/5
        [ValidateInput(false)]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            EmailPromotion emailPromotion = db.EmailPromotions.Find(id);
            if (emailPromotion == null)
            {
                return HttpNotFound();
            }
            return View(emailPromotion);
        }

        // POST: EmailSetting/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public ActionResult Edit([Bind(Include = "eid,s_mail,s_pass,image1,subject,message,note")] EmailPromotion emailPromotion)
        {
          

            // update
           
            
                var res = db.EmailPromotions.Where(a=>a.eid!=emailPromotion.eid).ToList();

                // update
                foreach (var r in res)
                {
                    r.message = "False";
                    //db.Entry(r).State = EntityState.Detached;
                    db.Entry(r).State = EntityState.Modified;
                    db.SaveChanges();
                }

                // save
              
           
            if (ModelState.IsValid)
            {
                emailPromotion.message = emailPromotion.message;
                db.Entry(emailPromotion).State = EntityState.Detached;
                db.Entry(emailPromotion).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(emailPromotion);
        }

        // GET: EmailSetting/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            EmailPromotion emailPromotion = db.EmailPromotions.Find(id);
            if (emailPromotion == null)
            {
                return HttpNotFound();
            }
            return View(emailPromotion);
        }

        // POST: EmailSetting/Delete/5
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            EmailPromotion emailPromotion = db.EmailPromotions.Find(id);
            db.EmailPromotions.Remove(emailPromotion);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}

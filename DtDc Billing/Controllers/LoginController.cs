using DtDc_Billing.Entity_FR;
using DtDc_Billing.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DtDc_Billing.Controllers
{
    public class LoginController : Controller
    {
        private DB_A43B74_wingsgrowdbEntities db = new DB_A43B74_wingsgrowdbEntities();
        // GET: Login
        public ActionResult Login()
        {            
            
            return View();
        }

        [HttpPost]
        public ActionResult Login(Login login)
        {
            var obj = db.Employees.Where(a => a.email.Equals(login.Username) && a.E_Password.Equals(login.Password)).FirstOrDefault();

            if (ModelState.IsValid)
            {
                if (obj != null)
                {
                    Session["PfID"] = obj.PF_Code.ToString();
                    //Session["EmpId"]=
                    return RedirectToAction("Index", "Home");
                }
            }
            return View(login);
        }


        public ActionResult Support(string ReturnUrl)
        {
            ViewBag.ReturnUrl = ReturnUrl;
            return View();
        }

        [HttpPost]
        public ActionResult Support(CustomerSupport customerSupport, string ReturnUrl)
        {

            var obj = db.Users.Where(a => a.Email.Equals(customerSupport.Email) && a.Password_U.Equals(customerSupport.Password_U) && a.Usertype == "Customer Support").FirstOrDefault();

            if (obj != null)
            {
                Session["csid"] = obj.User_Id.ToString();
                Session["csmail"] = obj.Email.ToString();

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
                    return RedirectToAction("Dashboard", "CustomerSupport");
                }


            }
            else
            {
                ModelState.AddModelError("LoginAuth", "Username or Password Is Incorrect");
            }
            return View();
        }

    }
}
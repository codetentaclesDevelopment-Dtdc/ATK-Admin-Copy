using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace DtDc_Billing.Models
{
    public class SessionAdmin : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            HttpContext ctx = HttpContext.Current;
            if (ctx.Request.Cookies.Get("Cookies") == null)// Old code=ctx.Request.Cookies.Get("AdminValue") == null
            {
                filterContext.Result = new RedirectToRouteResult(
                      new RouteValueDictionary(
                          new
                          {
                              controller = "Admin",
                              action = "AdminLogin",
                              returnUrl = filterContext.HttpContext.Request.Url.GetComponents(UriComponents.PathAndQuery, UriFormat.SafeUnescaped)
                          }));
            }
            //old
            //HttpContext ctx = HttpContext.Current;
            //if (HttpContext.Current.Session["Admin"] == null )
            //{
            //    filterContext.Result = new RedirectToRouteResult(
            //          new RouteValueDictionary(
            //              new
            //              {
            //                  controller = "Admin",
            //                  action = "AdminLogin",
            //                  returnUrl = filterContext.HttpContext.Request.Url.GetComponents(UriComponents.PathAndQuery, UriFormat.SafeUnescaped)
            //              }));
            //}
            //cookies
            //HttpCookie cookie = new HttpCookie("adid", HttpContext.Current.Session["Admin"].ToString());
            //cookie.Expires = DateTime.Now.AddDays(30);

            //filterContext.HttpContext.Response.Cookies.Add(cookie);

            //if (cookie == null)
            //{
            //    filterContext.Result = new RedirectToRouteResult(
            //          new RouteValueDictionary(
            //              new
            //              {
            //                  controller = "Admin",
            //                  action = "AdminLogin",
            //                  returnUrl = filterContext.HttpContext.Request.Url.GetComponents(UriComponents.PathAndQuery, UriFormat.SafeUnescaped)
            //              }));
            //}


            base.OnActionExecuting(filterContext);
        }
    }
}
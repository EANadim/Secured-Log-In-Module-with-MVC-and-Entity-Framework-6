using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace WebApplication1.Controllers
{
    public class AdminController : Controller
    {
        // GET: Admin
        public ActionResult Index()
        {
            try
            { 
                if (!(Session["type"]).ToString().Equals("admin"))
                {
                    return this.RedirectToAction("Index", "Home");
                }
            }
            catch (Exception ex)
            {
                return this.RedirectToAction("Index", "Home");
            }

            return View();
        }
        public ActionResult LogOut()
        {
            Session["email"] = "";
            Session["password"] = "";
            Session["type"] = "";

            if(Session["ChkRememberme"].ToString().Equals("true"))
            {
                HttpCookie nameCookie = Request.Cookies["email"]; //Fetch the Cookie using its Key.
                nameCookie.Expires = DateTime.Now.AddDays(-1); //Set the Expiry date to past date.
                Response.Cookies.Add(nameCookie); //Update the Cookie in Browser.
            }
            Session["ChkRememberme"] = "";

            return this.RedirectToAction("Index", "Home");
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Mvc;
using WebApplication1.Models;

namespace WebApplication1.Controllers
{
    public class HomeController : Controller
    {
        [HttpGet]
        public ActionResult Index()
        {
            if (Session["error"] != null)
            {
                if (Session["error"].Equals("yes"))
                {
                    ViewBag.Message = "Email or password invalid";
                }
                else if (Session["error"].Equals("block3"))
                {
                    ViewBag.Message = "Your account will be blocked for 3 minutes for 3 failed attempt";
                }
                else if (Session["error"].Equals("block"))
                {
                    ViewBag.Message = "Your account is blocked for several minutes.Please try again after some times";
                }
                else
                {
                    ViewBag.Message = "";
                }
            }
            else
            {
                ViewBag.Message = "";
            }
            try
            {
                if (Session["type"].Equals("user"))
                {
                    return this.RedirectToAction("Index", "User");
                }
                else if (Session["type"].Equals("admin"))
                {
                    return this.RedirectToAction("Index", "Admin");
                }
            }
            catch (Exception ex)
            {
                Session["email"] = "";          //Used for Home Controller
                Session["password"] = "";
                Session["type"] = "";

                Session["ChkRememberme"] = "";

                Session["emailReg"] = "";       //Used for Registration Controller
                Session["passwordReg"] = "";
                Session["nameReg"] = "";
                Session["genderReg"] = "";
                Session["addressReg"] = "";
                Session["typeReg"] = "";
                Session["questionReg"] = "";
                Session["answerReg"] = "";

                Session["emailForget"] = "";     //Used for Forget Controller
                Session["questionForget"] = "";

                return View();
            }

            Session["email"] = "";
            Session["password"] = "";
            Session["type"] = "";

            Session["ChkRememberme"] = "";

            Session["emailReg"] = "";
            Session["passwordReg"] = "";
            Session["nameReg"] = "";
            Session["genderReg"] = "";
            Session["addressReg"] = "";
            Session["typeReg"] = "";
            Session["questionReg"] = "";
            Session["answerReg"] = "";

            Session["emailForget"] = "";     //Used for Forget Controller
            Session["questionForget"] = "";

            Session["error"] = "";

            return View();
        }

        [HttpPost]
        public ActionResult Index(FormCollection form)
        {

            int emailFound = 0;
            int passwordFound = 0;

            string email = "";
            string password = "";
            string type = "";
            string ChkRememberme = "";
            DateTime blocktime=DateTime.Now;

            Session["error"] = "";

            email = form["email"].ToString();
            password = form["password"].ToString();

            MD5 md5Hash = MD5.Create();

            try
            {
                ChkRememberme = form["ChkRememberme"].ToString(); 

                using (AccountDBContext conn = new AccountDBContext())
                {
                    try
                    {
                        var result = from stu in conn.Accounts //This is for checked ChkRememberme
                                     select stu;
                        foreach (Account acc in result)
                        {
                            if (email.Equals(acc.Email))
                            {
                                emailFound = 1;
                                type = acc.Type;
                                blocktime = Convert.ToDateTime(acc.Blocktime);

                                if (VerifyMd5Hash(md5Hash, password, acc.Password.ToString()))
                                {
                                    passwordFound = 1;
                                }
                            }
                        }
                        if (emailFound == 1 && passwordFound == 1)
                        {
                            if (blocktime < DateTime.Now)
                            {
                                HttpCookie nameCookie = new HttpCookie("email"); //Create a Cookie with a suitable Key.                                    
                                nameCookie.Values["email"] = email; //Set the Cookie value.
                                nameCookie.Expires = DateTime.Now.AddDays(30); //Set the Expiry date.
                                Response.Cookies.Add(nameCookie); //Add the Cookie to Browser.

                                Session["email"] = email;
                                Session["password"] = password;
                                Session["type"] = type;
                                Session["ChkRememberme"] = "true";

                                Account acnt = new Account();
                                acnt.UpdateAttemptToZero(email);

                                if (type.Equals("user"))
                                {
                                    return this.RedirectToAction("Index", "User");
                                }
                                else if (type.Equals("admin"))
                                {
                                    return this.RedirectToAction("Index", "Admin");
                                }
                                else
                                {
                                    Session["error"] = "yes";
                                    return RedirectToAction("Index");
                                }
                            }
                            else if (blocktime > DateTime.Now)
                            {
                                Session["error"] = "block";
                            }
                        }
                        if (emailFound == 1 && passwordFound == 0)
                        {
                            Session["error"] = "yes";

                            using (AccountDBContext connection = new AccountDBContext())
                            {
                                try
                                {
                                    var query = from acc in connection.Accounts //This is for checked ChkRememberme
                                                select acc;
                                    foreach (Account account in query)
                                    {
                                        if (email.Equals(account.Email))
                                        {
                                            if (account.Attempt <= 1)
                                            {
                                                Account ac = new Account();
                                                ac.UpdateAttempt(email);
                                            }
                                            else if (account.Attempt == 2)
                                            {
                                                Account ac = new Account();
                                                ac.UpdateAttempt(email);
                                                if (ac.UpdateBlockTime(email, 3))
                                                {
                                                    Session["error"] = "block3";
                                                }
                                            }
                                            else if (account.Attempt == 3)
                                            {
                                                Account ac = new Account();
                                                ac.UpdateAttempt(email);
                                            }
                                            else if (account.Attempt>= 4)
                                            {
                                                Account ac = new Account();
                                                ac.UpdateAttempt(email);
                                                if (ac.UpdateBlockTime(email, 5))
                                                {
                                                    return this.RedirectToAction("Index", "Forget");
                                                }
                                            }
                                        }
                                    }
                                }
                                catch (Exception ex)
                                {

                                }
                            }

                            return RedirectToAction("Index");
                        }
                        if (emailFound == 0)
                        {
                            Session["error"] = "yes";
                            return RedirectToAction("Index");
                        }
                    }
                    catch (Exception ex)
                    {
                        return RedirectToAction("Index");
                    }
                }
            }
            catch (Exception ex)
            {
                using (AccountDBContext conn = new AccountDBContext())
                {
                    try
                    {
                        var result = from stu in conn.Accounts  //This is for unchecked ChkRememberme
                                     select stu;
                        foreach (Account acc in result)
                        {
                            if (email.Equals(acc.Email))
                            {
                                emailFound = 1;
                                type = acc.Type;
                                blocktime = Convert.ToDateTime(acc.Blocktime);
                                if (VerifyMd5Hash(md5Hash, password, acc.Password.ToString()))
                                {
                                    passwordFound = 1;
                                }
                            }
                        }
                        if (emailFound == 1 && passwordFound == 1)
                        {
                            if (blocktime < DateTime.Now)
                            {
                                Session["email"] = email;
                                Session["password"] = password;
                                Session["type"] = type;
                                Session["ChkRememberme"] = "";

                                Account ac = new Account();
                                ac.UpdateAttemptToZero(email);

                                if (type.Equals("user"))
                                {
                                    return this.RedirectToAction("Index", "User");
                                }
                                else if (type.Equals("admin"))
                                {
                                    return this.RedirectToAction("Index", "Admin");
                                }
                                else
                                {
                                    Session["error"] = "yes";
                                    return RedirectToAction("Index");
                                }
                            }
                            else if (blocktime > DateTime.Now)
                            {
                                Session["error"] = "block";
                            }
                        }
                        if (emailFound == 1 && passwordFound == 0)
                        {
                            Session["error"] = "yes";

                            using (AccountDBContext connection = new AccountDBContext())
                            {
                                try
                                {
                                    var query = from acc in connection.Accounts //This is for checked ChkRememberme
                                                select acc;
                                    foreach (Account account in query)
                                    {
                                        if (email.Equals(account.Email))
                                        {
                                            if (account.Attempt <= 1)
                                            {
                                                Account ac = new Account();
                                                ac.UpdateAttempt(email);
                                            }
                                            else if (account.Attempt == 2)
                                            {
                                                Account ac = new Account();
                                                ac.UpdateAttempt(email);
                                                if (ac.UpdateBlockTime(email, 3))
                                                {
                                                    Session["error"] = "block3";
                                                }
                                            }
                                            else if (account.Attempt == 3)
                                            {
                                                Account ac = new Account();
                                                ac.UpdateAttempt(email);
                                            }
                                            else if (account.Attempt >= 4)
                                            {
                                                Account ac = new Account();
                                                ac.UpdateAttempt(email);
                                                if (ac.UpdateBlockTime(email, 5))
                                                {
                                                    return this.RedirectToAction("Index", "Forget");
                                                }
                                            }
                                        }
                                    }
                                }
                                catch (Exception exce)
                                {

                                }
                            }

                            return RedirectToAction("Index");
                        }
                        if (emailFound == 0)
                        {
                            Session["error"] = "yes";
                            return RedirectToAction("Index");
                        }
                    }
                    catch (Exception exc)
                    {

                    }
                }
                return RedirectToAction("Index");
            }
            return RedirectToAction("Index");
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        static string GetMd5Hash(MD5 md5Hash, string input)
        {

            byte[] data = md5Hash.ComputeHash(Encoding.UTF8.GetBytes(input));
            StringBuilder sBuilder = new StringBuilder();
            for (int i = 0; i < data.Length; i++)
            {
                sBuilder.Append(data[i].ToString("x2"));
            }
            return sBuilder.ToString();
        }
        static bool VerifyMd5Hash(MD5 md5Hash, string input, string hash)
        {
            string hashOfInput = GetMd5Hash(md5Hash, input);
            StringComparer comparer = StringComparer.OrdinalIgnoreCase;
            if (0 == comparer.Compare(hashOfInput, hash))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
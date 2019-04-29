using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebApplication1.Models;

namespace WebApplication1.Controllers
{
    public class ForgetController : Controller
    {
        // GET: Forget
        public ActionResult Index()
        {
            Session["emailForget"] = "";
            Session["questionForget"] = "";

            Session["error"] = ""; //used for log in purpose 

            if (Session["email"] != null)
            {
                if (Session["email"] == "")
                {
                    return View();
                }
                else
                {
                    return this.RedirectToAction("Index", "Home");
                }
            }
            return View();
        }

        [HttpPost]
        public ActionResult Index(FormCollection form)
        {
            int emailFound = 0;
            using (AccountDBContext conn = new AccountDBContext())
            {
                try
                {
                    var result = from acc in conn.Accounts    //Using query expression syntax
                                 select acc;
                    foreach (Account ac in result)
                    {
                        if (ac.Email.ToString().Equals(form["email"].ToString()))
                        {
                            emailFound = 1;
                            Session["emailForget"] = ac.Email.ToString();
                            Session["questionForget"] = ac.Question.ToString();
                        }
                    }
                    if (emailFound == 1)
                    {
                        return RedirectToAction("Question");
                    }
                    else
                    {
                        return RedirectToAction("WrongEmail");
                    }
                }
                catch (Exception ex)
                {

                    return RedirectToAction("WrongEmail");
                }
            }
        }

        public ActionResult Question()
        {
            if (Session["emailForget"].Equals(""))
            {
                return RedirectToAction("Index");
            }
            else
            {
                ViewBag.Question = Session["questionForget"];
                return View();
            }
        }

        [HttpPost]
        public ActionResult Question(FormCollection form)
        {
            int answerFound = 0;
            using (AccountDBContext conn = new AccountDBContext())
            {
                try
                {
                    var result = from acc in conn.Accounts    //Using query expression syntax
                                 select acc;
                    foreach (Account ac in result)
                    {
                        if (ac.Answer.ToString().Equals(form["answer"].ToString()) && ac.Email.ToString().Equals(Session["emailForget"].ToString()))
                        {
                            answerFound = 1;
                        }
                    }
                    if (answerFound == 1)
                    {
                        return RedirectToAction("NewPassword");
                    }
                    else
                    {
                        return RedirectToAction("WrongAnswer");
                    }
                }
                catch (Exception ex)
                {
                    return RedirectToAction("WrongAnswer");
                }
            }
        }

        public ActionResult WrongEmail()
        {
            Session["emailForget"] = "";
            Session["questionForget"] = "";

            return View();
        }
        public ActionResult NewPassword()
        {
            if (Session["emailForget"].Equals(""))
            {
                return RedirectToAction("Index");
            }
            else
            {
                return View();
            }
        }
        [HttpPost]
        public ActionResult NewPassword(FormCollection form)
        {
            Account ac = new Account();
            if (ac.UpdatePassword(Session["emailForget"].ToString(), form["password"].ToString()))
            {
                if (ac.UpdateBlockTime(Session["emailForget"].ToString(), 0))
                {
                    ac.UpdateAttemptToZero(Session["emailForget"].ToString());
                    return RedirectToAction("Success");
                }
                else
                {
                    return RedirectToAction("Unsuccess");
                }
            }
            else
            {
                return RedirectToAction("Unsuccess");
            }
        }
        public ActionResult WrongAnswer()
        {
            return View();
        }
        public ActionResult Success()
        {
            Session["emailForget"] = "";
            Session["questionForget"] = "";

            return View();
        }
        public ActionResult Unsuccess()
        {
            Session["emailForget"] = "";
            Session["questionForget"] = "";

            return View();
        }
    }
}
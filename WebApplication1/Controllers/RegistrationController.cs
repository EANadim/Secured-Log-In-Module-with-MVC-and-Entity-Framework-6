using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebApplication1.Models;

namespace WebApplication1.Controllers
{
    public class RegistrationController : Controller
    {
        // GET: Registration
        public ActionResult Index()
        {
            Session["error"] = "";  //used for log in purpose

            if (Session["email"] == "")
            {
                ViewBag.emailReg = Session["emailReg"];
                ViewBag.nameReg = Session["nameReg"];
                ViewBag.passwordReg = Session["passwordReg"];
                ViewBag.genderReg = Session["genderReg"];
                ViewBag.addressReg = Session["addressReg"];
                ViewBag.questionReg = Session["questionReg"];
                ViewBag.answerReg = Session["answerReg"];
                ViewBag.typeReg = Session["typeReg"];

                return View();
            }
            else
            {
                return this.RedirectToAction("Index", "Home");
            }
        }
        [HttpPost]
        public ActionResult Index(FormCollection form)
        {
            int emailFound = 0;

            string email = "";
            string password = "";
            string name = "";
            string gender = "";
            string address = "";
            string type = "";

            email = form["email"].ToString();
            password = form["password"].ToString();
            name = form["name"].ToString();
            gender = form["gender"].ToString();
            address = form["address"].ToString();
            type = form["type"].ToString();

            Session["emailReg"] = email;
            Session["passwordReg"] = password;
            Session["nameReg"] = name;
            Session["genderReg"] = gender;
            Session["addressReg"] = address;
            Session["typeReg"] = type;

            using (AccountDBContext conn = new AccountDBContext())
            {
                try
                {
                    var result = from acc in conn.Accounts   //Using query expression syntax
                                 select acc.Email;
                    foreach (string eml in result)
                    {
                        if (eml.Equals(email))
                        {
                            emailFound = 1;
                        }
                    }
                }
                catch (Exception ex)
                {

                }
            }
            if (emailFound == 1)
            {
                return RedirectToAction("unsuccess");
            }
            else
            {
                return RedirectToAction("Registration2");
            }
        }
        public ActionResult Registration2()
        {
            if (Session["emailReg"] != null)
            {
                if (Session["emailReg"] == "" && Session["passwordReg"] == "" && Session["nameReg"] == "" && Session["genderReg"] == "" && Session["addressReg"] == "" && Session["typeReg"] == "")
                {
                    return RedirectToAction("Index");
                }
                else
                {
                    return View();
                }
            }
            else
            {
                return RedirectToAction("Index");
            }
        }

        [HttpPost]
        public ActionResult Registration2(FormCollection form)
        {
            string question = "";
            string answer = "";

            question = form["question"].ToString();
            answer = form["answer"].ToString();

            Session["questionReg"] = question;
            Session["answerReg"] = answer;

            Account ac = new Account();
            ac.Name = Session["nameReg"].ToString();
            ac.Email = Session["emailReg"].ToString();
            ac.Password = Session["passwordReg"].ToString();
            ac.Gender= Session["genderReg"].ToString();
            ac.Address= Session["addressReg"].ToString();
            ac.Question= Session["questionReg"].ToString();
            ac.Answer= Session["answerReg"].ToString();
            ac.Type = Session["typeReg"].ToString();

            Account ac2 = new Account();
            if (ac2.Insert(ac))
            {
                return RedirectToAction("success");
            }
            else
            {
                return RedirectToAction("unsuccess");
            }
        }
        public ActionResult success()
        {
            ViewBag.emailReg = Session["emailReg"];
            ViewBag.nameReg = Session["nameReg"];
            ViewBag.genderReg = Session["genderReg"];
            ViewBag.addressReg = Session["addressReg"];
            ViewBag.typeReg = Session["typeReg"];

            Session["emailReg"] = "";
            Session["passwordReg"] = "";
            Session["nameReg"] = "";
            Session["genderReg"] = "";
            Session["addressReg"] = "";
            Session["typeReg"] = "";
            Session["questionReg"] = "";
            Session["answerReg"] = "";

            return View();
        }
        public ActionResult unsuccess()
        {
            Session["emailReg"] = "";
            Session["passwordReg"] = "";
            Session["nameReg"] = "";
            Session["genderReg"] = "";
            Session["addressReg"] = "";
            Session["typeReg"] = "";
            Session["questionReg"] = "";
            Session["answerReg"] = "";

            return View();
        }
    }
}
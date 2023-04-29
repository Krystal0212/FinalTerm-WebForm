using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Mvc;
using WebForm.Models;
using System.Data.SqlClient;
using System.Data;

namespace WebForm.Controllers
{
    public class HomeController : Controller
    {
        private distributorManageEntities db = new distributorManageEntities();
        SqlConnection cn;
        SqlCommand cm;
        DataTable tb;


        public ActionResult Index()
        {
            return View();
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

        public ActionResult Login()
        {
            ViewBag.Message = "Please login your account for a better experience";

            return View();
        }

        public ActionResult Signup()
        {
            ViewBag.Message = "Please sign up below";

            return View();
        }

        public void connection()
        {

        }

        [HttpPost]
        public ActionResult verify(string email, string password)
        {
            if (ModelState.IsValid)
            {
                var f_password = GetMD5(password);
                var data = db.accounts.Where(s => s.username.Equals(email) && s.pass.Equals(f_password)).ToList();
                if (data.Count() > 0)
                {
                }
                else
                {
                    ViewBag.error = "Login failed";
                    return RedirectToAction("Login");
                }
            }
            return View();
        }

        public static string GetMD5(string str)
        {
            MD5 md5 = new MD5CryptoServiceProvider();
            byte[] fromData = Encoding.UTF8.GetBytes(str);
            byte[] targetData = md5.ComputeHash(fromData);
            string byte2String = null;

            for (int i = 0; i < targetData.Length; i++)
            {
                byte2String += targetData[i].ToString("x2");

            }
            return byte2String;
        }



        //Logout
        public ActionResult Logout()
        {
            Session.Clear();//remove session
            return RedirectToAction("Login");
        }

    }
}
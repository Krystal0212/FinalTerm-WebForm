using System;
using System.Data;
using System.Data.SqlClient;
using System.Security.Cryptography;
using System.Text;
using System.Web.Mvc;
using WebForm.Models;
using BC = BCrypt.Net.BCrypt;

namespace WebForm.Controllers
{
    public class HomeController : Controller
    {
        private distributorManageEntities db = new distributorManageEntities();
        SqlDataAdapter data;
        SqlDataReader dr;
        SqlConnection cn;
        SqlCommand cm = new SqlCommand();
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


        public void connect()
        {
            string s = "initial catalog = distributorManage; data source = ACERLT; integrated security = true";
            //string s = "initial catalog = distributorManage; data source = DESKTOP-502NHKM\\DUNG; integrated security = true";
            cn = new SqlConnection(s);
            cn.Open();

        }

        public void actionQuery(string sql)
        {
            connect();
            SqlCommand cmd = new SqlCommand(sql, cn);

            cmd.ExecuteNonQuery();
        }

        public DataTable selectQuery(string sql)
        {
            connect();
            SqlDataAdapter dataAdapter = new SqlDataAdapter(sql, cn);
            DataTable dt = new DataTable();
            dataAdapter.Fill(dt);
            return dt;
        }

        [HttpPost]
        public ActionResult verify(account accountInserted)
        {
            string password = getBCrypt(accountInserted.pass);
            string passwordInserted = accountInserted.pass;

            string sql = "select * from account where username = N'" + accountInserted.username + "' ";
            DataTable accountReference = selectQuery(sql);
            string passwordReference = accountReference.Rows[0][1].ToString();
            Boolean b = BC.Verify(passwordInserted, passwordReference);

            if (BC.Verify(passwordInserted, passwordReference))
            {
                string loginID = accountReference.Rows[0][3].ToString();
                Session["resellerid"] = loginID;
                return RedirectToAction("Index", "Home", new { resellerID = loginID }); // Pass the username as a parameter to the Index action
            }
            ModelState.AddModelError("username", "Invalid username or password");
            return View("Login");
        }

        public ActionResult verifyByEmail(string email, string password)
        {

            //if (ModelState.IsValid)
            //{
            //    var f_password = GetMD5(password);
            //    var data = db.accounts.Where(s => s.username.Equals(email) && s.pass.Equals(f_password)).ToList();
            //    if (data.Count() > 0)
            //    {
            //    }
            //    else
            //    {
            //        ViewBag.error = "Login failed";
            //        return RedirectToAction("Login");
            //    }
            //}
            return View();
        }

        public static string getBCrypt(string password)
        {
            return BCrypt.Net.BCrypt.HashPassword(password);
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


        [ActionName("IndexWithResellerID")]
        public ActionResult Index(string resellerID)
        {
            ViewBag.Username = resellerID;
            return View();
        }

        public ActionResult order(string resellerID)
        {
            Session["username"] = resellerID;
            return RedirectToAction("Index", "CurrentGoods", new
            {
                username = resellerID
            });
        }

        //Logout
        public ActionResult Logout()
        {
            Session.Clear();//remove session
            return RedirectToAction("Login");
        }

    }
}
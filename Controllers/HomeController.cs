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
            return View();
        }

        public ActionResult Login()
        {
            ViewBag.Message = "Please login your account for a better experience";

            return View();
        }

        [HttpGet]
        public ActionResult Signup()
        {
            ViewBag.Message = "Please sign up below";

            return View();
        }

        //[HttpPost]
        //public ActionResult Signup([Bind(Include = "username,pass,accType,loginID")] Cart acc)
        //{
        //    string sql = "insert into Account values ('"+ acc.username +"', '"+ acc.pass+"') ";

        //    return RedirectToAction("Index","Home");
        //}


        public void connect()
        {
            //string s = "initial catalog = distributorManage; data source = ACERLT; integrated security = true; Min Pool Size=5;Max Pool Size=1000;Connect Timeout=60";
            string s = "initial catalog = distributorManage; data source = DESKTOP-502NHKM\\DUNG; integrated security = true; Min Pool Size=5;Max Pool Size=1000;Connect Timeout=60";
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
        public string Verify(account accountInserted)
        {
            string password = getBCrypt(accountInserted.pass);
            string passwordInserted = accountInserted.pass;

            string sql = "select * from account where username = N'" + accountInserted.username + "' ";
            DataTable accountReference = selectQuery(sql);

            if (accountReference.Rows.Count != 0)
            {
                string passwordReference = accountReference.Rows[0][1].ToString();Boolean b = BC.Verify(passwordInserted, passwordReference);

                if (BC.Verify(passwordInserted, passwordReference))
                {
                    string loginID = accountReference.Rows[0][3].ToString();
                    Session["resellerid"] = loginID;
                    return "Logged in successfully"; // Pass the username as a parameter to the Index action
                }
            }
            
            ModelState.AddModelError("username", "Invalid username or password");
            return "Invalid username or password";
        }

        [HttpPost]
        public string Signupacc(account accountInserted)
        {
            //Account != account datatype
            string password = getBCrypt(accountInserted.pass);
            string username = accountInserted.username;

            string sql = "SELECT * FROM account WHERE username = N'" + accountInserted.username + "'";
            DataTable accountReference = selectQuery(sql);

            if (accountReference.Rows.Count != 0)
            {
                ModelState.AddModelError("username", "Username existed");
                return "Username existed";
            }

            string nextResellerID = getID("R", "reseller", "resellerID");

            string SignUpsql = "insert into account values (N'" + username + "', N'" + password + "', N'agent', N'" + nextResellerID + "') ";
            actionQuery(SignUpsql);

            string sqlInsert = "insert into reseller(resellerID,username) values ('"+nextResellerID+ "', '" + username + "')";
            actionQuery(sqlInsert);

            return "Sign up successfully";
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
            return RedirectToAction("Index", "CurrentGoods", new
            {
                username = resellerID
            });
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
        Boolean countCheck(int data)
        {
            if (data >= 1)
            {
                return true;
            }
            return false;
        }

        public Boolean isExist(string txt, string tableName, string IDName)
        {
            string sql = " select * from " + tableName + " where " + IDName + " = N'" + txt + "' ";
            DataTable dt = selectQuery(sql);
            if (countCheck(dt.Rows.Count))
            {
                return true;
            }
            return false;
        }

        private string checkCurrentID(string tableName, string IDName)
        {
            string sql = " select * from " + tableName + " order by " + IDName + "";
            DataTable dt = selectQuery(sql);


            return dt.Rows[dt.Rows.Count - 1][0].ToString();
        }

        private string getID(string ma, string tableName, string IDName)
        {
            string slang = "";
            if (!isExist(ma + "0001", tableName, IDName))
            {
                slang = ma + "0001";
            }
            else
            {
                string maIDtruoc = checkCurrentID(tableName, IDName);
                int txtNum = Int16.Parse(maIDtruoc.Substring(maIDtruoc.Length - 4));
                slang = ma + generateID(txtNum);
            }

            return slang;
        }

        private string generateID(int num)
        {
            string res = (++num).ToString();
            int checker = 10;
            while (res.Length < 4)
            {
                double a = res.Length;
                if (Int16.Parse(res) > checker)
                {
                    checker = checker * 10;
                }
                else
                {
                    res = "0" + res;
                }
            }
            return res;
        }
    }
}
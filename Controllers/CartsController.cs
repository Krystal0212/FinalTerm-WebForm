using MAM = Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using WebForm.Models;
using System.Data.SqlClient;
using System.Data.Entity.Infrastructure;
using System.Diagnostics;
using Newtonsoft.Json.Linq;
using static System.Data.Entity.Infrastructure.Design.Executor;
using System.Configuration;

namespace WebForm.Controllers
{
    public class CartsController : Controller
    {
        private distributorManageEntities db = new distributorManageEntities();
        SqlDataAdapter data;
        SqlDataReader dr;
        SqlConnection cn;
        SqlCommand cm = new SqlCommand();
        DataTable tb;

        //for paypal
        private readonly DbContext context;

        // GET: Carts
        public ActionResult Index()
        {
            return View(db.Carts.ToList());
        }

        [ActionName("IndexWithResellerID")]
        public ActionResult Index(string resellerID)
        {
            var carts = db.Carts.Where(c => c.resellerID == resellerID).ToList();
            return View(db.Carts.ToList());
            
        }

        // GET: Carts/Details/5
        [ActionName("IndexWithResellerID")]
        public ActionResult Details(string resellerID)
        {
            if (resellerID == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Cart cart = db.Carts.Find(resellerID);
            if (cart == null)
            {
                return HttpNotFound();
            }
            return View(cart);
        }

        // GET: Carts/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Carts/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "cartNumber,orderID,itemID,pricePerItem,quantity,totalPrice")] Cart cart)
        {
            if (ModelState.IsValid)
            {
                db.Carts.Add(cart);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(cart);
        }

        // GET: Carts/Edit/5
        [HttpGet]
        public ActionResult Edit(string resellerID, string itemID)
        {
            if (resellerID == null || itemID == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Cart cart = db.Carts.FirstOrDefault(c => c.resellerID == resellerID && c.itemID == itemID);

            if (cart == null )
            {
                return HttpNotFound();
            }
            return View(cart);
        }

        // POST: Carts/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "cartNumber,orderID,itemID,pricePerItem,quantity,totalPrice")] Cart cart)
        {
            if (ModelState.IsValid)
            {
                db.Entry(cart).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(cart);
        }

        // GET: Carts/Delete/5
        [HttpGet]
        public ActionResult Delete(string resellerID,  string itemID)
        {
            if (resellerID == null || itemID == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Cart cart = db.Carts.FirstOrDefault(c => c.resellerID == resellerID && c.itemID == itemID);

            if (cart == null)
            {
                return HttpNotFound();
            }
            return View(cart);
        
        }

        // POST: Carts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string resellerID, string itemID)
        {
            Cart cart = db.Carts.FirstOrDefault(c => c.resellerID == resellerID && c.itemID == itemID);
            db.Carts.Remove(cart);
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


        public ActionResult callUrl(string id, int quantity)
        {
            string url = "/Carts/AddCart/" + id+ "?quantity="+ quantity;
            return Redirect(url);
        }

        [HttpGet]
        public ActionResult AddCartUrl(string id, int quantity)
        {
            return callUrl(id,quantity);
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

        [HttpGet]
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

        string currentCartNumber = "";

        [HttpPost]
        public JsonResult Add(string resellerID ,string id, int quantity)
        {
            string goodID = id;
            string sql = "select * from cart";

            //get row count of cart table 
            DataTable dtCart = selectQuery(sql);
            int count = dtCart.Rows.Count;

            // get product name and price
            sql = "select goodName,price from CurrentGoods where goodID = '" + goodID + "'";
            DataTable dt = selectQuery(sql);
            string goodName = dt.Rows[0][0].ToString();
            int price = Int32.Parse(dt.Rows[0][1].ToString());
            int total = price * quantity;

            if (count != 0)
            {
                string sqlGetCurrentOrderID = "select orderID from cart order by orderID";
                string currentOrderID = selectQuery(sqlGetCurrentOrderID).Rows[0][0].ToString();
                string currentCartNumber = dtCart.Rows[dtCart.Rows.Count - 1][0].ToString();

                string sqlCheckExit = "select * from cart where cartNumber = 'C0001' and itemID = '" + goodID + "' and resellerID ='"+resellerID+"'";
                DataTable dtCheckExit = selectQuery(sqlCheckExit);

                if (countCheck(dtCheckExit.Rows.Count))
                {
                    
                    int storedQuantity = Int32.Parse(dtCheckExit.Rows[0][5].ToString());
                    quantity = quantity + storedQuantity;
                    total = price * quantity;

                    sql = "update cart set quantity = " + quantity + ", totalPrice = " + total + " where cartNumber = 'C0001' and itemID = '" + goodID + "' ";
                }
                else
                {
                    sql = "insert into Cart values ( '" + currentCartNumber + "','" + currentOrderID + "','" + goodID + "','" + goodName + "'," + price + "," + quantity + "," + total + ",'"+resellerID+"')";
                }
                actionQuery(sql);
            }
            else
            {
                string nextOrderID = getID("O", "orderList", "orderID");

                sql = "insert into Cart values ( 'C0001','"+nextOrderID+"','" + goodID + "','"+goodName+"'," + price + "," + quantity + "," + total + ",'" + resellerID + "')";
                actionQuery(sql);
            }

            return Json(new { success = true });
        }

        public ActionResult purchaseByPAYPAL()
        {
            

            return View();
        }
    }
}

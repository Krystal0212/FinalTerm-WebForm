using System;
using System.Data;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using WebForm.Models;

namespace WebForm.Controllers
{
    public class orderListsController : Controller
    {
        private distributorManageEntities db = new distributorManageEntities();
        SqlDataAdapter data;
        SqlDataReader dr;
        SqlConnection cn;
        SqlCommand cm = new SqlCommand();
        DataTable tb;

        // GET: orderLists

        [HttpGet]
        public ActionResult Index(string resellerID, string payout)
        {
            if (Session["resellerID"] == null)
            {
                return RedirectToAction("Index", "Home");
            }

            //Check new or old reseller
            string sql3 = "select * from orderList where resellerID = '" + resellerID + "' ";
            DataTable b = selectQuery(sql3);

            string sqlCheckCart = "select * from Cart where resellerID = '" + resellerID + "' ";
            DataTable c = selectQuery(sqlCheckCart);
            if (b.Rows.Count == 0 & c.Rows.Count > 0)
            {

                ViewBag.status = "new reseller";
                var order = db.orderLists.Where(o => o.resellerID == resellerID).ToList();
                return View(order);

            }
            else
            {
                ViewBag.status = "old reseller";
                if (payout.Equals("checked"))
                {
                    payout = "unchecked";
                    string sql = "select orderID, resellerID from Cart where resellerID = '" + resellerID + "'";
                    DataTable a = selectQuery(sql);
                    string orderID = a.Rows[0][0].ToString();
                    string date = DateTime.Today.ToString("yyyy-MM-dd");

                    string sql2 = "select paymentMethod, _address, phoneNumber, email from orderList where resellerID = '" + resellerID + "' ";
                    DataTable infor = selectQuery(sql2);
                    string paymentMethod = "Payment on delivery";
                    string address = infor.Rows[0][1].ToString();
                    string phoneNumber = infor.Rows[0][2].ToString();
                    string email = infor.Rows[0][3].ToString();

                    //tinh thanh tien
                    string sqlPriceCalculate = "select SUM(totalPrice) from Cart where resellerID = '" + resellerID + "' and orderID = '" + orderID + "'";
                    DataTable PriceCalculate = selectQuery(sqlPriceCalculate);
                    string thanhtien = PriceCalculate.Rows[0][0].ToString();

                    //insert orderlist
                    string sqlInsert = "insert into orderList values('" + orderID + "','" + resellerID + "','" + date + "','" + "Processing" + "', '" + paymentMethod + "', '" + address + "', '" + phoneNumber + "', '" + email + "', '" + thanhtien + "', '" + "Verifying..." + "')";
                    actionQuery(sqlInsert);

                    //insert orderDetail
                    string sqlInsertDetail = "insert into orderdetail(orderID, itemID, itemname, pricePerItem, quantity, totalPrice)\nselect orderID, itemID, itemname,  pricePerItem, quantity, totalPrice from cart where resellerID = '" + resellerID + "' ";
                    actionQuery(sqlInsertDetail);

                    //xoa cart
                    string sqlDelete = "delete from cart where orderId = '" + orderID + "' and resellerID = '" + resellerID + "'";
                    actionQuery(sqlDelete);

                    var order = db.orderLists.Where(o => o.resellerID == resellerID).ToList();
                    return View(order);
                }
                else
                {
                    var order = db.orderLists.Where(o => o.resellerID == resellerID).ToList();
                    return View(order);
                }

            }

        }


        //[ActionName("IndexWithJustResellerID")]
        //[Route("orderLists/Index/{resellerID}")]
        [HttpPost]
        public ActionResult Index(string resellerID)
        {
            var order = db.orderLists.Where(o => o.resellerID == resellerID).ToList();
            return View(order);
        }

        //[HttpPost]
        //public ActionResult Index([Bind(Include = "cartNumber,orderID,itemID,pricePerItem,quantity,totalPrice")] Cart cart)
        //{
        //    return RedirectToAction("Index", new { resellerID = Session["resellerid"] });
        //}

        // GET: orderLists/Details/5
        public ActionResult Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            orderList orderList = db.orderLists.Find(id);
            if (orderList == null)
            {
                return HttpNotFound();
            }
            return View(orderList);
        }


        [HttpGet]
        public ActionResult Information(string resellerID)
        {
            if (Session["resellerID"] == null)
            {
                return RedirectToAction("Index", "Home");
            }
            orderList order = db.orderLists.FirstOrDefault(o => o.resellerID == resellerID);



            // Pass the current quantity to the view as a ViewBag property

            return View(order);
        }
        // GET: orderLists/Create

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Information([Bind(Include = "paymentMethod,C_address,phoneNumber,email")] orderList orderList, string resellerID)
        {
            string sql = "select orderID, resellerID from Cart where resellerID = '" + resellerID + "'";
            DataTable a = selectQuery(sql);
            string orderID = a.Rows[0][0].ToString();
            string date = DateTime.Today.ToString("yyyy-MM-dd");

            //tinh thanh tien
            string sqlPriceCalculate = "select SUM(totalPrice) from Cart where resellerID = '" + resellerID + "' and orderID = '" + orderID + "'";
            DataTable PriceCalculate = selectQuery(sqlPriceCalculate);
            string thanhtien = PriceCalculate.Rows[0][0].ToString();

            //insert order
            string sql2 = "insert into orderList values('" + orderID + "','" + resellerID + "','" + date + "','" + "Processing" + "', '" + orderList.paymentMethod + "', '" + orderList.C_address + "', '" + orderList.phoneNumber + "', '" + orderList.email + "', '" + thanhtien + "', '" + "Verifying..." + "')";
            actionQuery(sql2);
            //orderList order = db.orderLists.FirstOrDefault(o => o.resellerID == resellerID);
            //return View("Index",order);

            //insert orderDetail
            string sqlInsertDetail = "insert into orderdetail( orderID, itemID, itemname, pricePerItem, quantity, totalPrice)\nselect orderID, itemID, itemname,  pricePerItem, quantity, totalPrice from cart where resellerID = '" + resellerID + "' ";
            actionQuery(sqlInsertDetail);

            //xoa cart
            string sqlDelete = "delete from cart where orderId = '" + orderID + "' and resellerID = '" + resellerID + "' ";
            actionQuery(sqlDelete);

            var orders = db.orderLists.Where(o => o.resellerID == resellerID).ToList();
            return View("Index", orders);
        }


        public ActionResult Create()
        {
            return View();
        }

        // POST: orderLists/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "orderID,resellerID,createdDay")] orderList orderList)
        {
            if (ModelState.IsValid)
            {
                db.orderLists.Add(orderList);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(orderList);
        }

        // GET: orderLists/Edit/5
        public ActionResult Edit(string id)
        {

            if (Session["resellerID"] == null)
            {
                return RedirectToAction("Index", "Home");
            }
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            orderList orderList = db.orderLists.Find(id);
            if (orderList == null)
            {
                return HttpNotFound();
            }
            return View(orderList);
        }

        // POST: orderLists/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "orderID,resellerID,createdDay,C_status,paymentMethod,C_address,phoneNumber,email,totalPrice")] orderList orderList, string resellerID, string orderID, string payout)
        {
            string sql = "update orderList set _address = '" + orderList.C_address + "', phoneNumber = '" + orderList.phoneNumber + "', email ='" + orderList.email + "' where orderID = '" + orderID + "'";
            actionQuery(sql);
            return RedirectToAction("Index", new { resellerID = Session["resellerid"], payout });
        }


        // GET: orderLists/Delete/5
        public ActionResult Delete(string id)
        {
            if (Session["resellerID"] == null)
            {
                return RedirectToAction("Index", "Home");
            }
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            orderList orderList = db.orderLists.Find(id);
            if (orderList == null)
            {
                return HttpNotFound();
            }
            return View(orderList);
        }

        // POST: orderLists/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id, string payout)
        {
            orderList orderList = db.orderLists.Find(id);
            db.orderLists.Remove(orderList);
            db.SaveChanges();

            string sql = "delete from orderDetail where orderID = '" + id + "' ";
            actionQuery(sql);
            return RedirectToAction("Index", new { resellerID = Session["resellerid"], payout });
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
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
    }
}

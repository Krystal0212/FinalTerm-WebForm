﻿using MAM = Microsoft.AspNetCore.Mvc;
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
using PayPal;
using PayPal.Api;
using Microsoft.Ajax.Utilities;


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

        // GET: Carts
        [HttpGet]
        public ActionResult Index(string resellerID)
        {
           
            if (Session["resellerID"] == null)
            {
                return RedirectToAction("Index", "Home");
            }

            List<Cart> currentItems = db.Carts.Where(c => c.resellerID == resellerID).ToList();
            Session["resellerid"] = resellerID;
            // For Paypal
            Session["Cart"] = currentItems;

            string sql = "select * from Cart where resellerID = '" + resellerID + "' ";
            DataTable a = selectQuery(sql);
            if(a.Rows.Count > 0)
            {
                ViewBag.status = "Exist";
            }
            var carts = db.Carts.Where(c => c.resellerID == resellerID).ToList();
            return View(carts);
        }

        


        // GET: Carts/Details/5
        [HttpGet]
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
            if (Session["resellerID"] == null)
            {
                return RedirectToAction("Index", "Home");
            }

            if (resellerID == null || itemID == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Cart cart = db.Carts.FirstOrDefault(c => c.resellerID == resellerID && c.itemID == itemID);

            if (cart == null)
            {
                return HttpNotFound();
            }

            var currentItem = db.CurrentGoods.FirstOrDefault(g => g.goodID == cart.itemID);

            int maxQuantity = 0;
            if (currentItem != null && currentItem.quantity.HasValue)
            {
                maxQuantity = currentItem.quantity.Value;
            }

            // Pass the current quantity to the view as a ViewBag property
            ViewBag.MaxQuantity = maxQuantity;

            return View(cart);
        }

        // POST: Carts/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "cartNumber,orderID,itemID,pricePerItem,quantity,totalPrice")] Cart cart, string resellerID, string itemID)
        {
            string sql = "update Cart set quantity = '" + cart.quantity + "' where resellerID = '" + resellerID + "' and itemID = '" + itemID + "'  ";
            actionQuery(sql);


            string sql2 = "select price from CurrentGoods where goodID = '" + itemID + "' ";
            DataTable a = selectQuery(sql2);
            string totalprice = a.Rows[0][0].ToString();

            string sql3 = "update Cart set totalPrice = '" + cart.quantity * Int32.Parse(totalprice) + "' where resellerID = '" + resellerID + "' and itemID = '" + itemID + "'  ";
            actionQuery(sql3);

            return RedirectToAction("Index", new { resellerID = Session["resellerid"] });
        }


        // GET: Carts/Delete/5
        [HttpGet]
        public ActionResult Delete(string resellerID, string itemID)
        {
            if (Session["resellerID"] == null)
            {
                return RedirectToAction("Index", "Home");
            }

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
            string sql = "Delete from Cart where resellerID = '" + resellerID + "' and itemID = '" + itemID + "' ";
            actionQuery(sql);
            return RedirectToAction("Index", new { resellerID = Session["resellerid"] });
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
            string url = "/Carts/AddCart/" + id + "?quantity=" + quantity;
            return Redirect(url);
        }

        [HttpGet]
        public ActionResult AddCartUrl(string id, int quantity)
        {
            return callUrl(id, quantity);
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
        public JsonResult Add(string resellerID, string id, int quantity)
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
                string sqlGetCurrentOrderID = "select orderID from cart where resellerID = '"+resellerID+"' order by orderID desc";
                string currentOrderID = selectQuery(sqlGetCurrentOrderID).Rows[0][0].ToString();

                string sqlCheckExit = "select * from cart where itemID = '" + goodID + "' and resellerID ='" + resellerID + "'";
                DataTable dtCheckExit = selectQuery(sqlCheckExit);

                if (countCheck(dtCheckExit.Rows.Count))
                {

                    int storedQuantity = Int32.Parse(dtCheckExit.Rows[0][5].ToString());
                    quantity = quantity + storedQuantity;

                    string sqlCheckQuantity = "select quantity from CurrentGoods where goodID = '" + goodID + "'  ";
                    DataTable MaxQuantity = selectQuery(sqlCheckQuantity);
                    int MaxQuan = Int32.Parse(MaxQuantity.Rows[0][0].ToString());

                    if (quantity > MaxQuan)
                    {

                        return Json(new { success = false });
                    }

                    total = price * quantity;
                    sql = "update cart set quantity = " + quantity + ", totalPrice = " + total + " where resellerID = '"+ resellerID +"' and itemID = '" + goodID + "' ";
                }
                else
                {
                    sql = "insert into Cart values ( '" + getID("CR","Cart","cartNumber") + "','" + currentOrderID + "','" + goodID + "','" + goodName + "'," + price + "," + quantity + "," + total + ",'" + resellerID + "')";
                }
                actionQuery(sql);
            }
            else
            {
                
                string nextOrderID = getID("O", "orderList", "orderID");

                sql = "insert into Cart values ( '" + getID("CR", "Cart", "cartNumber") + "','" + nextOrderID + "','" + goodID + "','" + goodName + "'," + price + "," + quantity + "," + total + ",'" + resellerID + "')";
                actionQuery(sql);
            }

            return Json(new { success = true });
        }

        // PayPal Section
        private string strCart = "Cart";
        private Payment payment;

        private Payment CreatePayment(APIContext apiContext, string redirectUrl)
        {
            var listItems = new ItemList() { items = new List<Item>() };

            List<Cart> listCarts = (List<Cart>)Session[strCart];
            foreach (var cart in listCarts)
            {
                listItems.items.Add(new Item()
                {
                    name = cart.itemname,
                    currency = "USD",
                    price = cart.pricePerItem.ToString(),
                    quantity = cart.quantity.ToString(),
                    sku = "sku"
                });
            }

            var payer = new Payer() { payment_method = "paypal" };

            //Do the configuration RedirectURLs here with redirectURLs object
            var redirUrls = new RedirectUrls()
            {
                cancel_url = redirectUrl,
                return_url = redirectUrl
            };

            //Create datails object
            var details = new Details()
            {
                tax = "1",
                shipping = "2",
                subtotal = (listCarts.Sum(x => x.quantity * x.pricePerItem)).ToString()
            };

            //Create amount object
            var amount = new Amount()
            {
                currency = "USD",
                total = (Convert.ToDouble(details.tax) + Convert.ToDouble(details.shipping) + Convert.ToDouble(details.subtotal)).ToString(),
                details = details
            };

            var a = apiContext.ToString();

            //Transaction
            var transactionList = new List<Transaction>();
            transactionList.Add(new Transaction()
            {
                description = "Testing transaction description",
                invoice_number = Convert.ToString((new Random()).Next(100000)),
                amount = amount,
                item_list = listItems
            });

            payment = new Payment()
            {
                intent = "sale",
                payer = payer,
                transactions = transactionList,
                redirect_urls = redirUrls,
            };

            return payment.Create(apiContext);
        }

        // Execute Payment
        private Payment ExecutePayment(APIContext apiContext, string payerID, string paymentID)
        {
            var paymentExecution = new PaymentExecution()
            {
                payer_id = payerID
            };
            payment = new Payment() { id = paymentID };
            return payment.Execute(apiContext, paymentExecution);
        }

        public ActionResult PayByPaypal()
        {
            // Get context depends on ClientID and SecretKey
            APIContext apiContext = PaypalConfiguration.GetAPIContext();

            try
            {
                string payerId = Request.Params["PayerID"];
                Boolean a = string.IsNullOrEmpty(payerId);
                if (string.IsNullOrEmpty(payerId))
                {
                    // Create payment
                    string baseURI = Request.Url.Scheme + "://" + Request.Url.Authority + "/Carts/PayByPaypal?";
                    var guid = Convert.ToString((new Random()).Next(100000));
                    var createdPayment = CreatePayment(apiContext, baseURI + "guid=" + guid);

                    // Receive returned link from Paypal response to create call function
                    var links = createdPayment.links.GetEnumerator();
                    string paypalRedirectUrl = string.Empty;
                    while (links.MoveNext())
                    {
                        Links link = links.Current;
                        if (link.rel.ToLower().Trim().Equals("approval_url"))
                        {
                            paypalRedirectUrl = link.href;
                        }
                    }
                    Session.Add(guid, createdPayment.id);

                    // Add extra
                    return Redirect(paypalRedirectUrl);
                }
                else
                {
                    // This section can only be executed if received all payment params from previous call
                    var guid = Request.Params["guid"];
                    var executePayment = ExecutePayment(apiContext, payerId, Session[guid] as string);
                    if (executePayment.state.ToLower() != "approved")
                    {
                        return View("Failure", Session["resellerid"]);
                    }
                }
            }
            catch (Exception ex)
            {
                PaypalLogger.Log("Error: " + ex.Message);
                return View("Failure", Session["resellerid"]);
            }

            //change some local path and add "/" between controllers and views in baseURI to get right urls
            //change redirecttoaction() to redirect, reduce the total in amount in CreatePayment

            //return View("Success", Session["resellerid"]);
            return RedirectToAction("Success", "Carts", new { resellerID = Session["resellerid"] });
        }

        public ActionResult Paypal()
        {


            //var orders = db.orderLists.Where(o => o.resellerID == resellerID).ToList();
            //return View("Index", orders);
            return View("");
        }

        [HttpGet]
        public ActionResult Success(string resellerID)
        {
            string address = "";
            string email = "";
            string phoneNumber = "";

            //kiem tra xem co du lieu tu truoc chua
            string sql = "select * from orderList where resellerID = '" + Session["resellerid"] + "' order by orderID ";
            DataTable a = selectQuery(sql);

            if (a.Rows.Count != 0)
            {
                address = a.Rows[0][5].ToString();
                email = a.Rows[0][7].ToString();
                phoneNumber = a.Rows[0][6].ToString();
            }

            //string resellerID = (string)Session["resellerid"];
            sql = "select orderID, resellerID from Cart where resellerID = '" + resellerID + "'";
            a = selectQuery(sql);
            string orderID = a.Rows[0][0].ToString();
            string date = DateTime.Today.ToString("yyyy-MM-dd");

            //tinh thanh tien
            string sqlPriceCalculate = "select SUM(totalPrice) from Cart where resellerID = '" + resellerID + "' and orderID = '" + orderID + "'";
            DataTable PriceCalculate = selectQuery(sqlPriceCalculate);
            Double extraFee = 0;
            string thanhtien = (Double.Parse(PriceCalculate.Rows[0][0].ToString()) + extraFee).ToString();

            //insert order
            string sql2 = "insert into orderList values('" + orderID + "','" + resellerID + "','" + date + "','" + "Processing" + "', ' Purchase via Paypal', '" + address + "', '" + phoneNumber + "', '" + email + "', '" + thanhtien + "', '" + "Verifying..." + "')";
            actionQuery(sql2);
            //orderList order = db.orderLists.FirstOrDefault(o => o.resellerID == resellerID);
            //return View("Index",order);

            //insert orderDetail
            string sqlInsertDetail = "insert into orderdetail( orderID, itemID, itemname, pricePerItem, quantity, totalPrice)\nselect orderID, itemID, itemname,  pricePerItem, quantity, totalPrice from cart where resellerID = '" + resellerID + "' ";
            actionQuery(sqlInsertDetail);

            //xoa cart
            string sqlDelete = "delete from cart where orderId = '" + orderID + "' and resellerID = '" + resellerID + "' ";
            actionQuery(sqlDelete);
            return View("Success", Session["resellerid"]);
        }


        public ActionResult Failure()
        {
            return View("Failure", Session["resellerid"]);
        }
    }
}

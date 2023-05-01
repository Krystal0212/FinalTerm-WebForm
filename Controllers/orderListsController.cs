using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using WebForm.Models;

namespace WebForm.Controllers
{
    public class orderListsController : Controller
    {
        private distributorManageEntities db = new distributorManageEntities();

        // GET: orderLists
        public ActionResult Index()
        {
            return View(db.orderLists.ToList());
        }

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

        // GET: orderLists/Create
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
        public ActionResult Edit([Bind(Include = "orderID,resellerID,createdDay")] orderList orderList)
        {
            if (ModelState.IsValid)
            {
                db.Entry(orderList).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(orderList);
        }

        // GET: orderLists/Delete/5
        public ActionResult Delete(string id)
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

        // POST: orderLists/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            orderList orderList = db.orderLists.Find(id);
            db.orderLists.Remove(orderList);
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
    }
}

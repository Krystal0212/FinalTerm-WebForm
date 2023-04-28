using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace WebForm.Models
{
    public class CurrentGoodsController : Controller
    {
        private distributorManageEntities db = new distributorManageEntities();

        // GET: CurrentGoods
        public ActionResult Index()
        {
            return View(db.CurrentGoods.ToList());
        }

        // GET: CurrentGoods/Details/5
        public ActionResult Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CurrentGood currentGood = db.CurrentGoods.Find(id);
            if (currentGood == null)
            {
                return HttpNotFound();
            }
            return View(currentGood);
        }

        // GET: CurrentGoods/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: CurrentGoods/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "goodID,goodName,Quantity,Price")] CurrentGood currentGood)
        {
            if (ModelState.IsValid)
            {
                db.CurrentGoods.Add(currentGood);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(currentGood);
        }

        // GET: CurrentGoods/Edit/5
        public ActionResult Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CurrentGood currentGood = db.CurrentGoods.Find(id);
            if (currentGood == null)
            {
                return HttpNotFound();
            }
            return View(currentGood);
        }

        // POST: CurrentGoods/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "goodID,goodName,Quantity,Price")] CurrentGood currentGood)
        {
            if (ModelState.IsValid)
            {
                db.Entry(currentGood).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(currentGood);
        }

        // GET: CurrentGoods/Delete/5
        public ActionResult Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CurrentGood currentGood = db.CurrentGoods.Find(id);
            if (currentGood == null)
            {
                return HttpNotFound();
            }
            return View(currentGood);
        }

        // POST: CurrentGoods/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            CurrentGood currentGood = db.CurrentGoods.Find(id);
            db.CurrentGoods.Remove(currentGood);
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

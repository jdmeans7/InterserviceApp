﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using InterserviceApp.Models;

namespace InterserviceApp.Controllers
{
    public class StaffDetailsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: StaffDetails
        public ActionResult Index()
        {
            return View(db.StaffDetails.ToList());
        }

        // GET: StaffDetails/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            is_staffDetails is_staffDetails = db.StaffDetails.Find(id);
            if (is_staffDetails == null)
            {
                return HttpNotFound();
            }
            return View(is_staffDetails);
        }

        // GET: StaffDetails/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: StaffDetails/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "badgeID,fName,lName,dept,phone,birthdate")] is_staffDetails is_staffDetails)
        {
            if (ModelState.IsValid)
            {
                db.StaffDetails.Add(is_staffDetails);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(is_staffDetails);
        }

        // GET: StaffDetails/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            is_staffDetails is_staffDetails = db.StaffDetails.Find(id);
            if (is_staffDetails == null)
            {
                return HttpNotFound();
            }
            return View(is_staffDetails);
        }

        // POST: StaffDetails/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "detailsID,badgeID,fName,lName,dept,phone,birthdate")] is_staffDetails is_staffDetails)
        {
            if (ModelState.IsValid)
            {
                db.Entry(is_staffDetails).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(is_staffDetails);
        }

        // GET: StaffDetails/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            is_staffDetails is_staffDetails = db.StaffDetails.Find(id);
            if (is_staffDetails == null)
            {
                return HttpNotFound();
            }
            return View(is_staffDetails);
        }

        // POST: StaffDetails/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            is_staffDetails is_staffDetails = db.StaffDetails.Find(id);
            db.StaffDetails.Remove(is_staffDetails);
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

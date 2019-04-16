using System;
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
    public class CourseController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Course
        [Authorize(Roles = "IS_Training, IS_Admin, IS_Secretary")]
        public ActionResult Index()
        {
            return View(db.Courses.ToList());
        }

        // GET: Course/Details/5
        [Authorize(Roles = "IS_Training, IS_Admin, IS_Secretary")]
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            is_Course is_Course = db.Courses.Find(id);
            if (is_Course == null)
            {
                return HttpNotFound();
            }
            return View(is_Course);
        }

        // GET: Course/Create
        [Authorize(Roles = "IS_Training, IS_Admin, IS_Secretary")]
        public ActionResult Create()
        {
            return View();
        }

        // POST: Course/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize(Roles = "IS_Training, IS_Admin, IS_Secretary")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "courseID,courseCode,desc")] is_Course is_Course)
        {
            if (ModelState.IsValid)
            {
                db.Courses.Add(is_Course);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(is_Course);
        }

        // GET: Course/Edit/5
        [Authorize(Roles = "IS_Training, IS_Admin, IS_Secretary")]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            is_Course is_Course = db.Courses.Find(id);
            if (is_Course == null)
            {
                return HttpNotFound();
            }
            return View(is_Course);
        }

        // POST: Course/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize(Roles = "IS_Training, IS_Admin, IS_Secretary")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "courseID,courseCode,desc")] is_Course is_Course)
        {
            if (ModelState.IsValid)
            {
                db.Entry(is_Course).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(is_Course);
        }

        // GET: Course/Delete/5
        [Authorize(Roles = "IS_Training, IS_Admin, IS_Secretary")]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            is_Course is_Course = db.Courses.Find(id);
            if (is_Course == null)
            {
                return HttpNotFound();
            }
            return View(is_Course);
        }

        // POST: Course/Delete/5
        [Authorize(Roles = "IS_Training, IS_Admin, IS_Secretary")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            is_Course is_Course = db.Courses.Find(id);
            db.Courses.Remove(is_Course);
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

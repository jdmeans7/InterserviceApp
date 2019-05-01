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
        [Authorize(Roles = "IS_Admin, IS_Secretary, IS_Training")]
        public ActionResult Index(string searchString)
        {
            var courses = from s in db.Courses select s;

            if (!String.IsNullOrEmpty(searchString))
            {
                courses = courses.Where(s => s.courseCode.Contains(searchString)
                                       || s.desc.Contains(searchString));
            }
            return View(courses.ToList());
        }

        // GET: Course/Require/5
        [Authorize(Roles = "IS_Training, IS_Admin, IS_Secretary")]
        public ActionResult Require(int? id)
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

        // POST: Course/Require/5
        [HttpPost]
        [Authorize(Roles = "IS_Training, IS_Admin, IS_Secretary")]
        public ActionResult Require(int courseID, String yes, String no)
        {
            if (yes != null)
            {
                is_Course co = db.Courses.Find(courseID);
                co.required = true;
                db.Entry(co).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("ClassPortal", "Home");
            }
            if (no != null)
            {
                is_Course co = db.Courses.Find(courseID);
                co.required = false;
                db.Entry(co).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("ClassPortal", "Home");
            }
            return View();
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

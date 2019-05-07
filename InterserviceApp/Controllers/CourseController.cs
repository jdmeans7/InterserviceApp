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
    /// <summary>
    /// Controller for handling all course functionality
    /// </summary>
    public class CourseController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        /// <summary>
        /// Page to show all courses that are active
        /// </summary>
        /// <param name="searchString">String used to filter results</param>
        /// <returns></returns>
        [Authorize(Roles = "IS_Admin, IS_Secretary, IS_Training")]
        public ActionResult Index(string searchString)
        {
            var courses = from s in db.Courses where s.active == true select s;

            if (!String.IsNullOrEmpty(searchString))
            {
                courses = courses.Where(s => s.courseCode.Contains(searchString)
                                       || s.desc.Contains(searchString));
            }
            return View(courses.ToList());
        }

        /// <summary>
        /// Get method for setting a course to required
        /// </summary>
        /// <param name="id">Course ID</param>
        /// <returns></returns>
        [Authorize(Roles = "IS_Admin, IS_Secretary")]
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

        /// <summary>
        /// Post method for setting a course to required
        /// </summary>
        /// <param name="courseID"></param>
        /// <param name="yes">This will be equal to "Yes" if the yes button is clicked</param>
        /// <param name="no">This will be equal to "No" if the no button is clicked</param>
        /// <returns></returns>
        [HttpPost]
        [Authorize(Roles = "IS_Admin, IS_Secretary")]
        public ActionResult Require(int courseID, String yes, String no)
        {
            try
            {
                if (yes != null) //If yes is selected
                {
                    is_Course co = db.Courses.Find(courseID);
                    co.required = true;
                    db.Entry(co).State = EntityState.Modified;
                    db.SaveChanges();
                    return RedirectToAction("ClassPortal", "Home");
                }
                if (no != null) //If no is selected
                {
                    is_Course co = db.Courses.Find(courseID);
                    co.required = false;
                    db.Entry(co).State = EntityState.Modified;
                    db.SaveChanges();
                    return RedirectToAction("ClassPortal", "Home");
                }
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine(e);
            }
            return View();
        }

        /// <summary>
        /// Get method for creating a course
        /// </summary>
        /// <returns></returns>
        [Authorize(Roles = "IS_Admin, IS_Secretary")]
        public ActionResult Create()
        {
            return View();
        }

        /// <summary>
        /// Handles the form submit for the creation of courses
        /// </summary>
        /// <param name="is_Course">Course that is being created</param>
        /// <returns></returns>
        [Authorize(Roles = "IS_Admin, IS_Secretary")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "courseID,courseCode,desc")] is_Course is_Course)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    //If there is no course with a matching description, add the new course
                    if (db.Courses.Where(i => i.desc == is_Course.desc).ToList().Count() == 0)
                    {
                        db.Courses.Add(is_Course);
                        db.SaveChanges();
                    }
                    return RedirectToAction("Index");
                }
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine(e);
            }
            return View(is_Course);
        }

        /// <summary>
        /// Get method for editing a course
        /// </summary>
        /// <param name="id">Course ID</param>
        /// <returns></returns>
        [Authorize(Roles = "IS_Admin, IS_Secretary")]
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

        /// <summary>
        /// Post method for editing a course
        /// </summary>
        /// <param name="is_Course">Course to be edited</param>
        /// <returns></returns>
        [Authorize(Roles = "IS_Admin, IS_Secretary")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "courseID,courseCode,desc")] is_Course is_Course)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    db.Entry(is_Course).State = EntityState.Modified;
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine(e);
            }
            return View(is_Course);
        }

        /// <summary>
        /// Get method for deleting a course
        /// </summary>
        /// <param name="id">Course ID</param>
        /// <returns></returns>
        [Authorize(Roles = "IS_Admin, IS_Secretary")]
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

        /// <summary>
        /// Post method for handling the deletion of a course. Delete is a soft delete
        /// </summary>
        /// <param name="id">Course ID</param>
        /// <returns></returns>
        [Authorize(Roles = "IS_Admin, IS_Secretary")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            try
            {
                is_Course is_Course = db.Courses.Find(id);
                is_Course.active = false; //Soft delete
                db.Entry(is_Course).State = EntityState.Modified;
                db.SaveChanges();
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine(e);
            }
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

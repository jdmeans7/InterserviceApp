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
    public class ClassController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Class
        public ActionResult Index()
        {
            //var classes = db.Classes.Include(i => i.Course);
            return View(db.Classes.ToList());
        }

        // GET: Class/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            is_Class is_Class = db.Classes.Find(id);
            if (is_Class == null)
            {
                return HttpNotFound();
            }
            return View(is_Class);
        }

        // GET: Class/Create
        public ActionResult Create()
        {
            ViewBag.courseID = new SelectList(db.Courses, "courseID", "desc");
            return View();
        }

        // POST: Class/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "classID,date,startTime,room,capacity,justification,fees,courseID")] is_Class is_Class)
        {
            if (ModelState.IsValid)
            {
                db.Classes.Add(is_Class);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.courseID = new SelectList(db.Courses, "courseID", "courseCode", is_Class.courseID);
            return View(is_Class);
        }

        // GET: Class/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            is_Class is_Class = db.Classes.Find(id);
            if (is_Class == null)
            {
                return HttpNotFound();
            }
            ViewBag.courseID = new SelectList(db.Courses, "courseID", "courseCode", is_Class.courseID);
            return View(is_Class);
        }

        // POST: Class/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "classID,date,startTime,room,capacity,justification,fees,courseID")] is_Class is_Class)
        {
            if (ModelState.IsValid)
            {
                db.Entry(is_Class).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.courseID = new SelectList(db.Courses, "courseID", "courseCode", is_Class.courseID);
            return View(is_Class);
        }

        // GET: Class/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            is_Class is_Class = db.Classes.Find(id);
            if (is_Class == null)
            {
                return HttpNotFound();
            }
            return View(is_Class);
        }

        // POST: Class/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            is_Class is_Class = db.Classes.Find(id);
            db.Classes.Remove(is_Class);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        // GET: HomeRegister
        public ActionResult HomeRegister()
        {
            //var RequiredCourses = db.Courses.Where(c => c.required == true).Select(c => c.courseID).ToList();
            //ArrayList ReqClasses = new ArrayList();
            //foreach (var x in RequiredCourses)
            //{
            //    ReqClasses.Add(db.Classes.Where(a => a.courseID == x).FirstOrDefault());
            //}
            //ViewBag.RequiredClasses = ReqClasses;
            //db.Classes.Join(db.Courses.Where(x => x.required), a => a.courseID, b => b.courseID, (a, b) => a).ToList()
            
            var result = (from is_Class in db.Classes
                          join is_Course in db.Courses
                          on is_Class.courseID equals is_Course.courseID
                          select new
                          {
                              classID = is_Class.classID
                              //courseID = is_Class.courseID,
                              //req = is_Course.required
                          }).ToList();
            var ids = new List<Object>();
            foreach (var r in result)
            {
                ids.Add(r.classID);
            }
            ViewBag.ReqClasses = ids;
            
            //var classes = (from is_Class in db.Classes where ids.Contains(is_Class.classID) select is_Class).ToList();
            //var classes = db.Classes.Include(i => i.Course);
            return View(db.Classes.ToList());
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

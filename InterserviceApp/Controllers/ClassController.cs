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
        /** ClassController/Index
         * Index method for Classes.
         */
        [Authorize(Roles ="IS_Admin, IS_Secretary, IS_Training")]
        public ActionResult Index(string searchString)
        {
            var nullDate = DateTime.Parse("0001-01-01"); //Date auto-inserted when field is null, used to get blackboard classes
            var classes = from s in db.Classes.Include(i => i.Course).Where(a => a.date >= System.DateTime.Today || a.date == nullDate).OrderBy(x => x.approved) select s;
            if (!String.IsNullOrEmpty(searchString))
            {
                classes = classes.Where(s => s.Course.courseCode.ToString().Contains(searchString)
                                        || s.Course.desc.Contains(searchString)
                                        || s.justification.Contains(searchString)
                                        || s.room.Contains(searchString));
            }
            return View(classes.ToList());
        }

        [Authorize(Roles = "IS_Admin, IS_Secretary, IS_Training")]
        public ActionResult OldClasses(string searchString)
        {
            var classes = from s in db.Classes.Include(i => i.Course).OrderBy(x => x.date) select s;
            if (!String.IsNullOrEmpty(searchString))
            {
                classes = classes.Where(s => s.Course.courseCode.ToString().Contains(searchString)
                                        || s.justification.Contains(searchString)
                                        || s.room.Contains(searchString));
            }
            return View(classes.ToList());
        }

        // GET: Class/Details/5
        /**
         * Method for approving classes. Used by secretaries to approve or deny a class. Approved classes can be registered for by students.
         */
        [Authorize(Roles = "IS_Admin, IS_Secretary")]
        public ActionResult Approve(int? id)
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

        [Authorize(Roles = "IS_Admin, IS_Secretary")]
        [HttpPost]
        public ActionResult Approve(String approve, String deny, int classID)
        {
            if(approve != null) //If the approve button was selected
            {
                is_Class cl = db.Classes.Find(classID);
                cl.approved = true;
                db.Entry(cl).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("ClassPortal", "Home");
            }
            else if (deny != null) //If the deny button was selected
            {
                is_Class cl = db.Classes.Find(classID);
                cl.approved = false;
                db.Entry(cl).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("ClassPortal", "Home");
            }

            return View();
        }

        // GET: Class/Create
        [Authorize(Roles = "IS_Admin, IS_Training, IS_Secretary")]
        public ActionResult Create()
        {
            ViewBag.courseID = new SelectList(db.Courses, "courseID", "desc");
            return View();
        }

        // POST: Class/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize(Roles = "IS_Training, IS_Admin, IS_Secretary")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "classID,date,startTime,room,capacity,justification,fees,hyperlink,blackboard,courseID")] is_Class is_Class, String physblack)
        {
            //if (ModelState.IsValid)
            //{
                if(physblack == "Physical")
                {
                    is_Class.blackboard = false;
                    db.Classes.Add(is_Class);
                    db.SaveChanges();
                    return RedirectToAction("ClassPortal", "Home");
                }
                else if (physblack == "Blackboard")
                {
                    is_Class.blackboard = true;
                    db.Classes.Add(is_Class);
                    db.SaveChanges();
                    return RedirectToAction("ClassPortal", "Home");
                }
                db.Classes.Add(is_Class);
                db.SaveChanges();
                return RedirectToAction("ClassPortal", "Home");
            //}

            ViewBag.courseID = new SelectList(db.Courses, "courseID", "courseCode", is_Class.courseID);
            return View(is_Class);
        }

        // GET: Class/Edit/5
        [Authorize(Roles = "IS_Training, IS_Admin, IS_Secretary")]
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
        [Authorize(Roles = "IS_Training, IS_Admin, IS_Secretary")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "classID,date,startTime,room,capacity,justification,fees,courseID")] is_Class is_Class)
        {
            if (ModelState.IsValid)
            {
                db.Entry(is_Class).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("ClassPortal", "Home");
            }
            ViewBag.courseID = new SelectList(db.Courses, "courseID", "courseCode", is_Class.courseID);
            return View(is_Class);
        }

        // GET: Class/Delete/5
        [Authorize(Roles = "IS_Admin, IS_Secretary")]
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
        [Authorize(Roles = "IS_Admin, IS_Secretary")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            is_Class is_Class = db.Classes.Find(id);
            db.Classes.Remove(is_Class);
            db.SaveChanges();
            return RedirectToAction("ClassPortal", "Home");
        }

        [Authorize(Roles = "IS_Admin, IS_Secretary")]
        public ActionResult ApproveStaffClass(int? id)
        {
            ViewBag.ClassID = id;
            ApprovingModel ap = new ApprovingModel();
            ap.StaffClasses = db.StaffClasses.Include(a => a.Class).Include(b => b.Staff).Where(x => x.classID == id).ToList();
            return View(ap);
        }

        [Authorize(Roles = "IS_Admin, IS_Secretary")]
        [HttpPost]
        public ActionResult ApproveStaffClass(ApprovingModel ap, int? id)
        {
            foreach (var s in ap.StaffClasses)
            {
                is_StaffClass sc = db.StaffClasses.First(x => x.badgeID == s.badgeID && x.classID == s.classID);
                sc.approved = s.approved;
                db.Entry(sc).State = EntityState.Modified;
                db.SaveChanges();
            }
           
            return RedirectToAction("ClassPortal", "Home");
        }

        [Authorize(Roles = "IS_Admin, IS_Secretary")]
        public ActionResult Attendance(int? id)
        {
            ViewBag.ClassID = id;
            ApprovingModel ap = new ApprovingModel();
            ap.StaffClasses = db.StaffClasses.Include(a => a.Class).Include(b => b.Staff).Where(x => x.classID == id && x.approved == true).OrderBy(z => z.status).ToList();
            return View(ap);
        }

        [Authorize(Roles = "IS_Admin, IS_Secretary")]
        [HttpPost]
        public ActionResult Attendance(ApprovingModel ap, int? id)
        {
            foreach (var s in ap.StaffClasses)
            {
                is_StaffClass sc = db.StaffClasses.First(x => x.badgeID == s.badgeID && x.classID == s.classID);
                sc.status = s.status;
                db.Entry(sc).State = EntityState.Modified;
                db.SaveChanges();
            }

            return RedirectToAction("ClassPortal", "Home");
        }

        [Authorize(Roles = "IS_Admin, IS_Secretary")]
        public ActionResult ApproveStaffClassSingle(int? classID, int? badgeID)
        {
            //var scid = db.StaffClasses.Include(a => a.Class).Include(b => b.Staff).Where(x => x.classID == classID && x.badgeID == badgeID).Select(i => i.id).ToList()[0];
            //is_StaffClass sc = db.StaffClasses.Find(scid);
            return View(db.StaffClasses.Include(a => a.Class).Include(b => b.Staff).Where(x => x.classID == classID && x.badgeID == badgeID).ToList()[0]);
        }

        [Authorize(Roles = "IS_Admin, IS_Secretary")]
        [HttpPost]
        public ActionResult ApproveStaffClassSingle(String approve, String deny, int? id)
        {
            if (approve != null) //If the approve button was selected
            {
                is_StaffClass sc = db.StaffClasses.Find(id);
                sc.approved = true;
                db.Entry(sc).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("ClassPortal", "Home");
            }
            else if (deny != null) //If the deny button was selected
            {
                is_StaffClass sc = db.StaffClasses.Find(id);
                sc.approved = false;
                db.Entry(sc).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("ClassPortal", "Home");
            }

            return View();
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

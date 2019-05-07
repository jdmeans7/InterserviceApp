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
    /// Controller dealing with all class functionality
    /// </summary>
    public class ClassController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        /// <summary>
        /// Page to show all classes that have not already occurred
        /// </summary>
        /// <param name="searchString">String used to filter results</param>
        /// <returns></returns>
        [Authorize(Roles ="IS_Admin, IS_Secretary, IS_Training")]
        public ActionResult Index(string searchString)
        {
            var classes = from s in db.Classes.Include(i => i.Course).Where(a => a.MOA != true && (a.date >= System.DateTime.Today || a.blackboard == true)).OrderByDescending(x => x.approved) select s;
            if (!String.IsNullOrEmpty(searchString))
            {
                classes = classes.Where(s => s.Course.courseCode.ToString().Contains(searchString)
                                        || s.Course.desc.Contains(searchString)
                                        || s.justification.Contains(searchString)
                                        || s.room.Contains(searchString));
            }
            return View(classes.ToList());
        }

        /// <summary>
        /// Page to show all classes that have ever been created
        /// </summary>
        /// <param name="searchString">String used to filter results</param>
        /// <returns></returns>
        [Authorize(Roles = "IS_Admin, IS_Secretary, IS_Training")]
        public ActionResult OldClasses(string searchString)
        {
            var classes = from s in db.Classes.Include(i => i.Course).OrderBy(x => x.date).Where(a => a.MOA != true) select s;
            if (!String.IsNullOrEmpty(searchString))
            {
                classes = classes.Where(s => s.Course.courseCode.ToString().Contains(searchString)
                                        || s.justification.Contains(searchString)
                                        || s.room.Contains(searchString));
            }
            return View(classes.ToList());
        }

        /// <summary>
        /// Method for approving classes. Used by secretaries to approve or deny a class. Approved classes can be registered for by students.
        /// </summary>
        /// <param name="id">Class ID passed from view</param>
        /// <returns></returns>
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

        /// <summary>
        /// Class to handle the post for approving classes
        /// </summary>
        /// <param name="approve">String passed from button on view, will be null if deny is clicked</param>
        /// <param name="deny">String passed from button on view, will be null if approve is clicked</param>
        /// <param name="classID"></param>
        /// <returns></returns>
        [Authorize(Roles = "IS_Admin, IS_Secretary")]
        [HttpPost]
        public ActionResult Approve(String approve, String deny, int classID)
        {
            try
            {
                if (approve != null) //If the approve button was selected
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
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine(e);
            }
            return View();
        }

        /// <summary>
        /// Handles the get view for the creation of classes
        /// </summary>
        /// <returns></returns>
        [Authorize(Roles = "IS_Admin, IS_Training, IS_Secretary")]
        public ActionResult Create()
        {
            ViewBag.courseID = new SelectList(db.Courses.Where(x => x.active == true), "courseID", "desc");
            return View();
        }

        /// <summary>
        /// Handles the form submit for the creation of classes
        /// </summary>
        /// <param name="is_Class">Class that is being created</param>
        /// <param name="physblack">String passed from view to differentiate between different types of classes. Will be "Physical" if physical class is selected, "Blackboard" if blackboard is selected</param>
        /// <returns></returns>
        [Authorize(Roles = "IS_Training, IS_Admin, IS_Secretary")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "classID,date,startTime,room,capacity,justification,fees,hyperlink,blackboard,courseID")] is_Class is_Class, String physblack)
        {
            try
            {
                if (physblack == "Physical")
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
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine(e);

            }
            return RedirectToAction("ClassPortal", "Home");
        }

        /// <summary>
        /// Get method for editing a class
        /// </summary>
        /// <param name="id">Class id for getting the class to edit</param>
        /// <returns></returns>
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

        /// <summary>
        /// Post method for editing a class
        /// </summary>
        /// <param name="is_Class">Class to be edited</param>
        /// <returns></returns>
        [Authorize(Roles = "IS_Training, IS_Admin, IS_Secretary")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "classID,date,startTime,room,capacity,justification,fees,courseID")] is_Class is_Class)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    db.Entry(is_Class).State = EntityState.Modified;
                    db.SaveChanges();
                    return RedirectToAction("ClassPortal", "Home");
                }
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine(e);
            }
            ViewBag.courseID = new SelectList(db.Courses, "courseID", "courseCode", is_Class.courseID);
            return View(is_Class);
        }

        /// <summary>
        /// Get method for deleting a class
        /// </summary>
        /// <param name="id">Class id to be deleted</param>
        /// <returns></returns>
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

        /// <summary>
        /// Post method for deleting a class
        /// </summary>
        /// <param name="id">Class id to be deleted</param>
        /// <returns></returns>
        [Authorize(Roles = "IS_Admin, IS_Secretary")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            try
            {
                is_Class is_Class = db.Classes.Find(id);
                db.Classes.Remove(is_Class);
                db.SaveChanges();
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine(e);
            }
            return RedirectToAction("ClassPortal", "Home");
        }

        /// <summary>
        /// Get method for approving a staff to take a class
        /// </summary>
        /// <param name="id">Class id for class</param>
        /// <returns></returns>
        [Authorize(Roles = "IS_Admin, IS_Secretary")]
        public ActionResult ApproveStaffClass(int? id)
        {
            ViewBag.ClassID = id;
            ApprovingModel ap = new ApprovingModel();
            ap.StaffClasses = db.StaffClasses.Include(a => a.Class).Include(b => b.Staff).Where(x => x.classID == id).ToList(); //Get all staff classes that relate to this class
            return View(ap);
        }

        /// <summary>
        /// Post method for approving a staff to take a class
        /// </summary>
        /// <param name="ap">List of staff classes and users together</param>
        /// <param name="id"></param>
        /// <returns></returns>
        [Authorize(Roles = "IS_Admin, IS_Secretary")]
        [HttpPost]
        public ActionResult ApproveStaffClass(ApprovingModel ap, int? id)
        {
            foreach (var s in ap.StaffClasses)
            {
                try
                {
                    is_StaffClass sc = db.StaffClasses.First(x => x.badgeID == s.badgeID && x.classID == s.classID); //Find individual staff class
                    sc.approved = s.approved; //Set staff classes approved column to reflect the user's action
                    db.Entry(sc).State = EntityState.Modified;
                    db.SaveChanges();
                }
                catch (Exception e)
                {
                    System.Diagnostics.Debug.WriteLine(e);
                }
            }
           
            return RedirectToAction("ClassPortal", "Home");
        }

        /// <summary>
        /// Get method for taking attendance for a class
        /// </summary>
        /// <param name="id">Class ID for class taking attendance</param>
        /// <returns></returns>
        [Authorize(Roles = "IS_Admin, IS_Secretary")]
        public ActionResult Attendance(int? id)
        {
            ViewBag.ClassID = id;
            ApprovingModel ap = new ApprovingModel();
            ap.StaffClasses = db.StaffClasses.Include(a => a.Class).Include(b => b.Staff).Where(x => x.classID == id && x.approved == true).OrderBy(z => z.status).ToList();
            return View(ap);
        }

        /// <summary>
        /// Post method for taking attendance for a class
        /// </summary>
        /// <param name="ap">List of staff classes with staff</param>
        /// <param name="id"></param>
        /// <returns></returns>
        [Authorize(Roles = "IS_Admin, IS_Secretary")]
        [HttpPost]
        public ActionResult Attendance(ApprovingModel ap, int? id)
        {
            foreach (var s in ap.StaffClasses)
            {
                try
                {
                    is_StaffClass sc = db.StaffClasses.First(x => x.badgeID == s.badgeID && x.classID == s.classID);
                    sc.status = s.status; //Set staff classes status column to reflect the user's action
                    db.Entry(sc).State = EntityState.Modified;
                    db.SaveChanges();
                }
                catch(Exception e)
                {
                    System.Diagnostics.Debug.WriteLine(e);
                }
            }

            return RedirectToAction("ClassPortal", "Home");
        }

        /// <summary>
        /// Get method for a supervisor to approve a single staff for a class. This is what is sent in a link to supervisors
        /// </summary>
        /// <param name="classID"></param>
        /// <param name="badgeID"></param>
        /// <returns></returns>
        [Authorize(Roles = "IS_Admin, IS_Secretary")]
        public ActionResult ApproveStaffClassSingle(int? classID, int? badgeID)
        {
            return View(db.StaffClasses.Include(a => a.Class).Include(b => b.Staff).Where(x => x.classID == classID && x.badgeID == badgeID).ToList()[0]);
        }

        /// <summary>
        /// Post method for a supervisor to approve a single staff for a class. THis is what is sent in a link to supervisors
        /// </summary>
        /// <param name="approve">This will be equal to "Approve" if the approve button was clicked</param>
        /// <param name="deny">This will be equal to "Deny" if the approve button was clicked</param>
        /// <param name="id">StaffClass id</param>
        /// <returns></returns>
        [Authorize(Roles = "IS_Admin, IS_Secretary")]
        [HttpPost]
        public ActionResult ApproveStaffClassSingle(String approve, String deny, int? id)
        {
            try
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
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine(e);
            }

            return View();
        }

        public ActionResult ClassReportForm()
        {
            ViewBag.Courses = db.Courses.Select(x => x.desc).ToList();
            return View();
        }

        public ActionResult StaffTakenClass(string syear, string coursedesc)
        {
            if (syear != null)
            {
                var year = Int32.Parse(syear);
                is_Course C = db.Courses.First(x => x.desc == coursedesc && x.active == true);
                var courseID = C.courseID;
                var StaffClasses = db.StaffClasses.Include(a=> a.Class).Include(b => b.Staff).Where(x => x.endDate.Year == year && x.Class.courseID == courseID && x.approved == true && x.status == true).ToList();
                ViewBag.SC = StaffClasses;
                return View();
            }
            else return View();
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

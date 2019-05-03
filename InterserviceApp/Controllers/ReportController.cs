using InterserviceApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace InterserviceApp.Controllers
{
    
    public class ReportController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        // GET: Report
        public ActionResult Index()
        {
            List<String> reports = new List<String>(new string[] { "All staff that have required classes that need taken", "Option 2" });
            ViewBag.Reports = reports;
            return View();
        }

        [Authorize(Roles = "IS_Admin, IS_Secretary")]
        [HttpPost]
        public ActionResult Index(string Report)
        {
            if (Report == "All staff that have required classes that need taken")
            {
                return RedirectToAction("TakeClasses");
            }
            else return View();
        }

        [Authorize(Roles = "IS_Admin, IS_Secretary")]
        public ActionResult TakeClasses()
        {
            int month = System.DateTime.Now.Month;
            int flagMonth = month - 1;
            if (flagMonth == 0)
                flagMonth = 12;

            //Get staff users who have a birthday that needs checked and aren't flagged
            List<is_staffDetails> staff = db.StaffDetails.Where(i => i.birthdate.Month == month || i.birthdate.Month == flagMonth && i.flag != true).ToList();

            //HashMap of required courses 
            //Had to cast from ILookup to Lookup
            Dictionary<int, is_Course> requiredCourses = db.Courses.Where(i => i.required == true).ToDictionary(i => i.courseID, i => i);

            //List to contain users who will get notified
            List<is_staffDetails> toNotify = new List<is_staffDetails>();

            foreach (is_staffDetails sD in staff)
            {
                //Counter to check the number of courses they have taken
                int counter = 0;

                //Get courses that a user has taken
                IQueryable<is_Course> query = from c in db.Courses
                                              join cl in db.Classes on c.courseID equals cl.courseID
                                              join sC in db.StaffClasses on cl.classID equals sC.classID
                                              where sD.badgeID == sC.badgeID && sC.status == true
                                              select c;
                List<is_Course> courses = query.Distinct().ToList();

                //Count courses taken and email or flag appropriately
                foreach (is_Course c in courses)
                {
                    if (requiredCourses.ContainsKey(c.courseID))
                    {
                        counter++;
                    }
                }

                if (requiredCourses.Count > counter)
                {
                    toNotify.Add(sD);
                }
            }

            return View(toNotify);
        }
    }
}
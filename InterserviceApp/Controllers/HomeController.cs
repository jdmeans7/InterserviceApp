using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using InterserviceApp.Models;
using System.Data.Entity;
using System.Net;

namespace InterserviceApp.Controllers
{
    public class HomeController : Controller
    {
        ApplicationDbContext db = new ApplicationDbContext();

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        public ActionResult ClassPortal()
        {
            return View();
        }

        public ActionResult HomeRegister()
        {
            
            var classes = db.Classes.Include(i => i.Course).Where(a => a.Course.required == true);
            return View(classes);
        }

        [ChildActionOnly]
        public ActionResult Classes()
        {
            return PartialView(db.Classes.Include(i => i.Course).ToList());
        }

        // GET: Home/Register
        public ActionResult Register(int? id)
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
            var supervisors = db.StaffDetails.Where(x => x.supervisor == true).Select(a => new { a.fName, a.lName }).ToList();
            var sups = new List<String>();
            foreach (var s in supervisors)
            {
                sups.Add(s.fName + " " + s.lName);
            }
            ViewBag.Supervisors = sups;
            return View(is_Class);
        }

        // POST: Home/Register
        [HttpPost]
        public ActionResult Register(String BadgeID, int classID)
        {
            if (BadgeID == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var badge = Int32.Parse(BadgeID);
            is_staffDetails Staff = db.StaffDetails.Find(badge);
            is_Class Class = db.Classes.Find(classID);

            // TODO: Actually handle these errors instead of just returning HttpNotFound
            if (Staff == null)
            {
                return HttpNotFound();
            }
            if (Class == null)
            {
                return HttpNotFound();
            }

            is_StaffClass StaffClass = new is_StaffClass{
                                                            badgeID = badge,
                                                            classID = classID,
                                                            approved = false,
                                                            endDate = Class.date,
                                                            status = false
                                                        };

            if (ModelState.IsValid)
            {
                db.StaffClasses.Add(StaffClass);
                db.SaveChanges();
                return RedirectToAction("HomeRegister");
            }
            return View(Class);
        }
    }
}
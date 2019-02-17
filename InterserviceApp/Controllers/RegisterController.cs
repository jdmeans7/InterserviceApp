using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using InterserviceApp.Models;

namespace InterserviceApp.Controllers
{
    public class RegisterController : Controller
    {

        private InterserviceModels db = new InterserviceModels();

        // GET: Register
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult StaffRegister()
        {
            ViewBag.Classes = new SelectList(db.Courses.Select(d => d.desc).ToList());
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult StaffRegister([Bind(Include = "detailsID,badgeID,fName,lName,dept,phone,birthdate")] is_staffDetails is_staffDetails, int choice)
        {
            if (ModelState.IsValid)
            {
                db.is_staffDetails.Add(is_staffDetails);
                is_StaffClass staffClass = new is_StaffClass
                {
                    badgeID = is_staffDetails.badgeID,
                    classID = choice,
                    approved = false,
                    endDate = DateTime.Parse("01/01/2001 01:00:00 AM"),
                    status = false
                };
                db.StaffClasses.Add(staffClass);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(is_staffDetails);
        }
    }
}
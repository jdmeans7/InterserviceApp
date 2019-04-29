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

    using System.Configuration;
    using System.Data.SqlClient;
    using System.IO;
    using System.Net.Mail;
    using System.Web.UI;

    public class StaffDetailsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: StaffDetails
        /// <summary>
        /// Index to populate user table for secretaries
        /// </summary>
        /// <param name="searchString">String entered for searching</param>
        /// <returns></returns>
        [Authorize]
        public ActionResult Index(string searchString)
        {
            var staffDetails = from s in db.StaffDetails select s;
            if (!String.IsNullOrEmpty(searchString))
            {
                staffDetails = staffDetails.Where(s => s.lName.Contains(searchString)
                                       || s.fName.Contains(searchString)
                                       || s.dept.Contains(searchString)
                                       || s.badgeID.ToString().Contains(searchString));
            }
            return View(staffDetails.ToList());
        }

        // GET: StaffDetails/Details/5
        [Authorize(Roles = "IS_Admin, IS_Secretary")]
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
        [Authorize(Roles = "IS_Admin, IS_Secretary")]
        public ActionResult Create()
        {
            return View();
        }

        // POST: StaffDetails/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize(Roles = "IS_Admin, IS_Secretary")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "badgeID,fName,lName,email,dept,phone,birthdate,supervisor")] is_staffDetails is_staffDetails)
        {
            if (ModelState.IsValid)
            {
                //If database doesn't already have the user, add the user
                if (db.StaffDetails.Find(is_staffDetails.badgeID) == null) {
                    db.StaffDetails.Add(is_staffDetails);
                    db.SaveChanges();
                }
                return RedirectToAction("Index");
            }

            return View(is_staffDetails);
        }

        // GET: StaffDetails/Edit/5
        [Authorize(Roles = "IS_Admin, IS_Secretary")]
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
        [Authorize(Roles = "IS_Admin, IS_Secretary")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "detailsID,badgeID,fName,lName,email,dept,phone,birthdate,flag,supervisor")] is_staffDetails is_staffDetails)
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
        [Authorize(Roles = "IS_Admin, IS_Secretary")]
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
        [Authorize(Roles = "IS_Admin, IS_Secretary")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            is_staffDetails is_staffDetails = db.StaffDetails.Find(id);
            db.StaffDetails.Remove(is_staffDetails);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        //Find users that need to take classes
        [Authorize(Roles = "IS_Admin, IS_Secretary")]
        public ActionResult Notify()
        {
            //Get current month and figure out birth month to check for flagging
            int month = System.DateTime.Now.Month;
            int flagMonth = month - 1;
            if (flagMonth == 0)
                flagMonth = 12;

            //Get staff users who have a birthday that needs checked and aren't flagged
            List<is_staffDetails> staff = db.StaffDetails.Where(i => i.birthdate.Month == month || i.birthdate.Month == flagMonth && i.flag != true).ToList();

            //HashMap of required courses 
            //Had to cast from ILookup to Lookup
            Dictionary <int, is_Course > requiredCourses = db.Courses.Where(i => i.required == true).ToDictionary(i => i.courseID, i => i);

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
                    System.Diagnostics.Debug.WriteLine(c.courseID);
                    if (requiredCourses.ContainsKey(c.courseID))
                    {
                        counter++;
                    }
                }

                System.Diagnostics.Debug.WriteLine(sD.badgeID + ": " + counter);

                if (requiredCourses.Count > counter)
                {

                    toNotify.Add(sD);

                    if (sD.birthdate.Month == month)
                    {
                        NotificationEmail(sD);
                    }
                    else
                    {
                        FlagEmail(sD);
                    }
                }
            }

            return View(toNotify);
        }

        [Authorize(Roles = "IS_Admin, IS_Secretary")]
        public ActionResult SendEmail(int? id, string subject, string body)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            is_staffDetails x = db.StaffDetails.Find(id);
            String email = x.email;
            MailMessage mail = new MailMessage();
            mail.To.Add(email);
            //   mail.To.Add("Another Email ID where you wanna send same email");
            mail.From = new MailAddress("InterserviceApplication@gmail.com");
            // mail.Subject = staffDetails.EmailSubject;
            mail.Subject = subject;
            //string Body = staffDetails.SendEmail;
            mail.Body = body;
            //mail.Body = "<h1>Hello</h1>";
            //mail.Attachments.Add(new Attachment("C:\\file.zip"));
            mail.IsBodyHtml = true;
            SmtpClient smtp = new SmtpClient();
            smtp.Host = "smtp.gmail.com"; //Or Your SMTP Server Address
            smtp.Credentials = new System.Net.NetworkCredential
                 ("InterserviceApplication@gmail.com", "Admin123!");


            //Or your Smtp Email ID and Password
            smtp.EnableSsl = true;
            smtp.Send(mail);
            if (x == null)
            {
                return HttpNotFound();
            }
            return View(x);
        }

        //Send notification email
        private void NotificationEmail(is_staffDetails staff)
        {
            //Really don't know how error prone this is
            try
            {
                string email = staff.email;
                MailMessage mail = new MailMessage();
                mail.To.Add(email);
                mail.From = new MailAddress("InterserviceApplication@gmail.com");
                mail.Subject = "Notification for Required Classes";
                mail.Body = "Hello, " + staff.fName + " " + staff.lName + "\n\nThis is your birth month and you need to retake your required classes for the year.\n" +
                    "\nPlease use the Interservice application to view your required courses and schedule to take them within the month.\n\nThank you, and have a nice day!";

                mail.IsBodyHtml = false;
                SmtpClient smtp = new SmtpClient();
                smtp.Host = "smtp.gmail.com"; //Or Your SMTP Server Address
                smtp.Credentials = new System.Net.NetworkCredential
                     ("InterserviceApplication@gmail.com", "Admin123!");


                //Or your Smtp Email ID and Password
                smtp.EnableSsl = true;
                smtp.Send(mail);
            }
            catch (Exception e)
            {
                //Literally just prints the error to the debug console in Visual Studio
                System.Diagnostics.Debug.WriteLine(e);
            }
        }

        private void FlagEmail(is_staffDetails staff)
        {
            //Try finally guarantees that even if an email doesn't get sent, or something goes wrong, the users are still flagged.
            try
            {
                string email = staff.email;
                MailMessage mail = new MailMessage();
                mail.To.Add(email);
                mail.From = new MailAddress("EncompassingSol@gmail.com");
                mail.Subject = "Notification for Required Classes";
                mail.Body = "Hello, " + staff.fName + " " + staff.lName + "\n\nLast month was your birth month and our records show that you didn't complete all of your required courses within the month.\n" +
                    "\nYour account will be flagged as having not taken the courses within the alloted time.\n" +
                    "\nYou need to retake your required classes for the year to get your account unflagged.\n" +
                    "\nPlease use the Interservice application to view your required courses and schedule to take them as soon as possible.\n\nThank you, and have a nice day!";

                mail.IsBodyHtml = false;
                SmtpClient smtp = new SmtpClient();
                smtp.Host = "smtp.gmail.com"; //Or Your SMTP Server Address
                smtp.Credentials = new System.Net.NetworkCredential
                        ("InterserviceApplication@gmail.com", "Admin123!");


                //Or your Smtp Email ID and Password
                smtp.EnableSsl = true;
                smtp.Send(mail);
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine(e);
            }
            finally
            {
                staff.flag = true;
                db.SaveChanges();
            }
        }

        //Display view with editable approve and status for a particular staffclass
        [Authorize(Roles = "IS_Admin, IS_Secretary")]
        public ActionResult EditStaffClass(int id) {
            try
            {
                return View(db.StaffClasses.Find(id));
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine(e);
                return RedirectToAction("Index");
            }
        }

        //Method for saving changes to staffclass from staffupdate view
        [Authorize(Roles = "IS_Admin, IS_Secretary")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditStaffClass([Bind(Include = "id,badgeID,classID,approved,endDate,status")] is_StaffClass staffClass)
        {
            if (ModelState.IsValid)
            {
                db.Entry(staffClass).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("StaffUpdate", staffClass.badgeID);
            }
            return View(staffClass);
        }

        //Display view used to manually add course credit to user
        [Authorize(Roles = "IS_Admin, IS_Secretary")]
        public ActionResult AddStaffClass(int id = Int32.MinValue)
        {
            //If there was no ID sent return to index
            if (id == Int32.MinValue)
            {
                return RedirectToAction("Index");
            }

            //Create StaffClass with default values
            is_StaffClass staffClass = new is_StaffClass
            {
                badgeID = id,
                endDate = System.DateTime.Now
            };
            return View(staffClass);
        }

        [Authorize(Roles = "IS_Admin, IS_Secretary")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddStaffClass(is_StaffClass staffClass)
        {
            //Add class to DB before adding to staff class
            is_Class c = new is_Class
            {
                courseID = Int32.Parse(Request["CourseID"]),
                approved = true,
                date = staffClass.endDate,
                MOA = true
            };

            //Try to add class to database, then attach class to staff class
            //then add the staffclass to the database
            try
            {
                db.Classes.Add(c);
                db.SaveChanges();

                //Get class that was just added to DB to use for staff class
                staffClass.approved = true;
                staffClass.status = true;
                staffClass.Staff = db.StaffDetails.Find(staffClass.badgeID);
                staffClass.Class = db.Classes.ToList().Last();
                staffClass.classID = staffClass.Class.classID;

                db.StaffClasses.Add(staffClass);
                db.SaveChanges();

                return RedirectToAction("Index");
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine(e);
                return View(staffClass);
            }
        }

        [Authorize(Roles ="IS_Admin, IS_Secretary")]
        public ActionResult StaffUpdate(int? id, string year)
        {
            int intyear;
            if (String.IsNullOrEmpty(year))
            {
                intyear = System.DateTime.Now.Year;
            }
            else
            {
               intyear = Int32.Parse(year);
            }
            ViewBag.TakenReq = db.StaffClasses.Include(a => a.Staff).Include(b => b.Class).Where(x => x.badgeID == id && x.Class.date.Year == intyear && x.Class.Course.required == true && x.status == true).ToList();
            //ViewBag.NeededReq = db.StaffClasses.Include(a => a.Staff).Include(b => b.Class).Where(x => x.badgeID == id && x.Class.date.Year == intyear && x.Class.Course.required == true && x.status == false).ToList();
            List<is_Course> reqCourses = db.Courses.Include(a => a.Classes).Where(x => x.required == true).ToList();
            List<is_Course> needToTake = new List<is_Course>();
            foreach (var c in reqCourses)
            {
                if (db.StaffClasses.Where(x => x.badgeID == id && x.Class.date.Year == intyear && x.Class.Course.courseID == c.courseID && x.status == true).Any())
                {
                }
                else
                {
                    needToTake.Add(c);
                }
            }
            ViewBag.NeededReq = needToTake;
            return View(db.StaffClasses.Include(a => a.Staff).Include(b => b.Class).Where(x => x.badgeID == id && x.Class.date.Year == intyear).ToList());
        }

        /*
         [HttpPost]
        public ActionResult StaffUpdate(int? badgeID, string year)
        {
            int intyear = Int32.Parse(year);
            return View(db.StaffClasses.Include(a => a.Staff).Include(b => b.Class).Where(x => x.badgeID == badgeID && x.Class.date.Year == intyear).ToList());
        }
        */

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

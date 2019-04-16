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
        public ActionResult Create([Bind(Include = "badgeID,fName,lName,email,dept,phone,birthdate")] is_staffDetails is_staffDetails)
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
        public ActionResult Edit([Bind(Include = "detailsID,badgeID,fName,lName,email,dept,phone,birthdate")] is_staffDetails is_staffDetails)
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
            //get current date, and convert to numeric value
            string[] sArr = System.DateTime.Now.ToString().Split(' ');
            sArr = sArr = sArr[0].Split('/');

            int month = Int32.Parse(sArr[0]) * 100;
            int day = Int32.Parse(sArr[1]);

            int currentMonthDay = month + day;
            //end

            List<is_staffDetails> staffList = db.StaffDetails.ToList();
            List<is_staffDetails> toNotify = new List<is_staffDetails>();
            List<is_staffDetails> toRedFlag = new List<is_staffDetails>();

            foreach (is_staffDetails i in staffList)
            {
                string s = i.birthdate.ToString();

                sArr = s.Split(' ');
                sArr = sArr[0].Split('/');

                //Combined birth month and day are turned into numeric value. 
                //Ex: April 1st becomes 401
                month = Int32.Parse(sArr[0]) * 100;
                day = Int32.Parse(sArr[1]);

                int bDay = month + day;

                //If today is your birthday, receive email
                if (bDay == currentMonthDay)
                {
                    toNotify.Add(i);
                }
                //If you are a month overdue, get a warning email
                else if (bDay <= (currentMonthDay - 100))
                {
                    toRedFlag.Add(i);
                }
                //If your birthday is in December, check for month overdue 
                else if ((bDay - 1100) == currentMonthDay)
                {
                    toRedFlag.Add(i);
                }
            }

            return View(toNotify);
        }

        [Authorize]
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
            mail.From = new MailAddress("EncompassingSol@gmail.com");
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

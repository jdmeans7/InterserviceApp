﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using InterserviceApp.Models;
using System.Data.Entity;
using System.Net;
using System.Web.Services;

namespace InterserviceApp.Controllers
{

    using System.Configuration;
    using System.Data.SqlClient;
    using System.IO;
    using System.Net.Mail;
    using System.Web.UI;

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

        [Authorize(Roles = "IS_Admin, IS_Secretary, IS_Training")]
        public ActionResult ClassPortal()
        {
            return View();
        }

        /// <summary>
        /// Method for displaying home register page. This page is shown to all users and is used to register for classes
        /// </summary>
        /// <param name="searchString">String entered by user to filter results</param>
        /// <returns></returns>
        public ActionResult HomeRegister(string searchString)
        {
            var classes = db.Classes.Include(i => i.Course).Where(a => a.Course.required == true && a.approved == true && a.MOA != true && a.date >= System.DateTime.Today || a.blackboard == true);
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
        /// Partial view to show a list of classes
        /// </summary>
        /// <returns></returns>
        [ChildActionOnly]
        public ActionResult Classes()
        {
            var nullDate = DateTime.Parse("0001-01-01"); //Date auto-inserted when field is null, used to get blackboard classes
            return PartialView(db.Classes.Include(i => i.Course).Where(a => a.Course.required == false && a.approved == true && a.MOA != true && a.date >= System.DateTime.Today || a.blackboard == true).ToList());
        }

        /// <summary>
        /// Method for displaying view to register for a class
        /// </summary>
        /// <param name="id">Class ID</param>
        /// <returns></returns>
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

        /// <summary>
        /// Method for handling the registration for a class by a staff
        /// </summary>
        /// <param name="BadgeID">Badge ID of staff member</param>
        /// <param name="classID">Class Id of class</param>
        /// <param name="selectedSup">Supervisor that is emailed</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Register(String BadgeID, int classID, String selectedSup)
        {
            if (BadgeID == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var badge = Int32.Parse(BadgeID);
            is_staffDetails Staff = db.StaffDetails.Find(badge);
            is_Class Class = db.Classes.Find(classID);

            String[] supname = selectedSup.Split(' ');
            var first = supname[0];
            var last = supname[1];
            is_staffDetails Supervisor = db.StaffDetails.Where(x => x.fName == first && x.lName == last).First();

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
                try
                {
                    String email = Supervisor.email;
                    MailMessage mail = new MailMessage();
                    mail.To.Add(email);
                    //   mail.To.Add("Another Email ID where you wanna send same email");
                    mail.From = new MailAddress("InterserviceApplication@gmail.com");
                    // mail.Subject = staffDetails.EmailSubject;
                    mail.Subject = Staff.fName + " " + Staff.lName + " is Requesting Your Approval to Take a Class";
                    //string Body = staffDetails.SendEmail;
                    //String url = System.Web.HttpContext.Current.Request.Url.AbsoluteUri;
                    String bodyText = Staff.fName + " " + Staff.lName + " would like to take the following class:" + "<br>"
                        + $"<b>Course ID: {StaffClass.Class.Course.courseID}" + $" | Course Description: {StaffClass.Class.Course.desc}" + "</b><br>"
                        + "Approve the class here: " + "http://localhost:54330/Class/ApproveStaffClassSingle?classID=" + StaffClass.classID + "&badgeID=" + Staff.badgeID;
                    mail.Body = bodyText;
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
                   
                }
                catch (Exception e)
                {
                    System.Diagnostics.Debug.WriteLine(e);
                }
                if (Supervisor == null)
                {
                    return HttpNotFound();
                }
                db.SaveChanges();
                return RedirectToAction("HomeRegister");
            }
            return View(Class);
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using InterserviceApp.Models;
using System.Data.Entity;

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
            /*
            var result = (from Class in db.Classes
                          join Course in db.Courses
                          on Class.courseID equals Course.courseID
                          select new
                          {
                              classID = Class.classID
                          }).ToList();
            List<int> ids = new List<int>();
            foreach(var item in result)
            {
                ids.Add(item.classID);
            }
            ViewBag.Req = ids;
            */
            var classes = db.Classes.Include(i => i.Course).Where(a => a.Course.required == true);
            return View(classes);
        }

        [ChildActionOnly]
        public ActionResult Classes()
        {
            return PartialView(db.Classes.Include(i => i.Course).ToList());
        }
    }
}
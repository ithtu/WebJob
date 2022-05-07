using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace WebJob.Controllers
{
    public class JobsController : Controller
    {
        // GET: Jobs
        public ActionResult ListBySkill()
        {
            return View();
        }
        public ActionResult Details()
        {
            return View();
        }
    }
}
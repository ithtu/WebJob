using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebJob.Models;


namespace WebJob.Areas.Admin.Controllers
{
    public class AdHomeController : Controller
    {
        JobContextDataContext dt = new JobContextDataContext();
        // GET: Admin/AdHome
        public ActionResult AdIndex()
        {
            
            if (Session["account"] == null || Session["role"] == null)
            {
                return RedirectToAction("Login", "AdLogin");
            }
            return RedirectToAction("Index","Admin");
        }
    }
}
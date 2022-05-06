using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace WebJob.Controllers
{
    public class UserController : Controller
    {
        // GET: UserLogin
        public ActionResult Login()
        {
            return View();
        }
       
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebJob.Models;

namespace WebJob.Controllers
{
    public class UserController : Controller
    {
        JobContextDataContext dt = new JobContextDataContext();
        // GET: UserLogin
        public ActionResult Index()
        {
            account acc = (account)Session["account"];
            if (acc == null || int.Parse(acc.id_role) != 2 || acc.confirm == null)
            {
                return RedirectToAction("Login", "User");
            }
            employee emp = dt.employees.SingleOrDefault(p => p.id_account == acc.id_account);
            return View(emp);
        }
        public ActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Login(FormCollection collection)
        {
            var email = collection["email"];
            var password = collection["password"];
            employee acc = dt.employees.SingleOrDefault(p => p.email == email && p.account.password == password);
            if (acc != null)
            {

                Session["account"] = acc;

                if (int.Parse(acc.account.id_role) == 1)
                {
                    return RedirectToAction("Index", "Admin");
                }
                else if (acc.account.confirm == null)
                {
                    ViewBag.Notify = "Tài khoản không tồn tại !!!";
                    return this.Login();
                }
                else
                {
                    Session["name"] = acc.name_employee.ToString();
                    return RedirectToAction("Contact", "Home");
                    
                }
            }
            ViewBag.Notify = "Tên đăng nhập hoặc mật khẩu không đúng !!!";

            return this.Login();
        }
        public ActionResult Register() 
        {
            return View();
        }
        public ActionResult Logout()
        {
            Session.Clear();//remove session
            return RedirectToAction("Login");
        }
    }
}
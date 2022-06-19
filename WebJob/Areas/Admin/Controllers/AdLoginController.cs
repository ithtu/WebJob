using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebJob.Models;
using System.Web.Security;

namespace WebJob.Areas.Admin.Controllers
{
    public class AdLoginController : Controller
    {
        JobContextDataContext dt = new JobContextDataContext();
        // GET: Admin/AdLogin
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
                Session["role"] = acc.account.id_role;

                if (acc.account.id_role != 1)
                {
                    ViewBag.Notify = "Tài khoản không tồn tại !!!";

                }
                else if (acc.account.confirm == null)
                {
                    ViewBag.Notify = "Tài khoản không tồn tại !!!";
                    return this.Login();
                }
                else
                {
                    Session["role"] = "3";
                    Session["avatar"] = acc.avatar;
                    Session["name"] = acc.name_employee.ToString();
                    return RedirectToAction("Index", "Admin");
                }
            }
            ViewBag.Notify = "Tên đăng nhập hoặc mật khẩu không đúng !!!";

            return this.Login();
        }
        public ActionResult Logout()
        {
            Session.Clear();
            return RedirectToAction("Login", "AdLogin");
        }
    }
}

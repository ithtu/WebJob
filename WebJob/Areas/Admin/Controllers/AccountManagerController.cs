using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebJob.Models;

namespace WebJob.Areas.Admin.Controllers
{
    public class AccountManagerController : Controller
    {
        // GET: Admin/AccountManager
        JobContextDataContext db = new JobContextDataContext();
        public ActionResult Account()
        {
            if (Session["account"] == null || Session["role"] == null)
            {
                return RedirectToAction("Login", "AdLogin");
            }
            var acc = db.accounts.ToList().OrderBy(x => x.id_account);
            return View(acc);
        }
        public ActionResult Add()
        {
            if (Session["account"] == null || Session["role"] == null)
            {
                return RedirectToAction("Login", "AdLogin");
            }
            ViewBag.id_roless = new SelectList(db.roles.ToList().OrderBy(x => x.id_role), "id_role", "name_role");
            return View();

        }
        [HttpPost]
        public ActionResult Add(FormCollection collection)
        {
            ViewBag.id_role = new SelectList(db.roles.ToList().OrderBy(x => x.id_role), "id_role", "name_role");
            //--------------------------------------------------------------------------------------------------------------------
            cover_letter let = new cover_letter();
            profile pro = new profile();
            account account = new account();
            //--------------------------------------------------------------------------------------------------------------------
            employee emp = new employee();
            var name_em = collection["name_employee"];
            var dob = String.Format("{0:dd/MM/yyyy}", collection["day_birth"]);
            var gender = collection["gender"];
            var phone = collection["phonenumber_employee"];
            var email = collection["email"];
            var add = collection["address_employee"];

                emp.id_employee = Nanoid.Nanoid.Generate(size:5);
                emp.name_employee = name_em;
                emp.day_birth = DateTime.Parse(dob);
                emp.gender = gender;
                emp.phonenumber_employee = phone;   
                emp.email = email;
                emp.address_employee = add;
                emp.id_account = Nanoid.Nanoid.Generate(size:5);
                emp.id_letter = let.id_letter;
                emp.id_profile = pro.id_profile;

            db.employees.InsertOnSubmit(emp);
            //----------------------------------------------------------------------------------------------
            var pass = collection["account.password"];
            var idrole = int.Parse(collection["account.role.id_role"]); 
            var confirm = collection["confirm"];
            role ro = db.roles.SingleOrDefault(p => p.id_role == idrole);
            account.id_account = emp.id_account;
            account.password = pass;
            account.confirm = confirm;
            account.id_role = ro.id_role  ;
            


            db.accounts.InsertOnSubmit(account);
            db.SubmitChanges();
            return RedirectToAction("Account");
            /* return this.Add();*/
        }


    }
}
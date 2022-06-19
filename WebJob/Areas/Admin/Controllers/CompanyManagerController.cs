using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebJob.Models;

namespace WebJob.Areas.Admin.Controllers
{
    public class CompanyManagerController : Controller
    {
        // GET: Admin/CompanyManager
        JobContextDataContext db = new JobContextDataContext();
        /*public ActionResult Company()
        {
            var all_company = from tt in db.companies select tt;
            return View(all_company);
        }
        public ActionResult Add()
        {
            if (Session["account"] == null || Session["role"] == null)
            {
                return RedirectToAction("Login", "AdLogin");
            }
            return View();
        }
        [HttpPost]
        public ActionResult Add(FormCollection collection, company dm)
        {
           // var idcom = collection["id_company"];
            var name = collection["name_company"];
            var addr = collection["address"];
            var number = collection["phonenumber"];

            if (string.IsNullOrEmpty(name))
            {
                ViewData["Error"] = "Don't empty!";
            }
            else
            {

                dm.id_company = Nanoid.Nanoid.Generate(size:5);
                dm.name_company = name;
                dm.address = addr;
                dm.phonenumber = number;
                db.companies.InsertOnSubmit(dm);
                db.SubmitChanges();

                return RedirectToAction("Company");
            }
            return this.Add();
        }
        public ActionResult Edit(String id)
        {
            var E_job = db.companies.First(m => m.id_company == id);
            return View(E_job);
        }
        [HttpPost]
        public ActionResult Edit(String id, FormCollection collection)
        {
            var company = db.companies.First(m => m.id_company == id);
            var name = collection["name_company"];
            var idcompany = collection["id_company"];
            var addr = collection["address"];
            var phone = collection["phonenumber"];

            company.id_company = id;
            if (string.IsNullOrEmpty(name))
            {
                ViewData["Error"] = "Don't empty!";
            }
            else
            {
                company.id_company =idcompany;
                company.name_company = name;
                company.address = addr;
                company.phonenumber = phone;
                
                UpdateModel(company);
                db.SubmitChanges();
                return RedirectToAction("Company");
            }
            return this.Edit(id);
        }*/
        public ActionResult Delete(String id)
        {
            company dm = db.companies.SingleOrDefault(x => x.id_company  == id);
            db.companies.DeleteOnSubmit(dm);
            db.SubmitChanges();
            return Redirect(Request.UrlReferrer.ToString());
        }
    }
}
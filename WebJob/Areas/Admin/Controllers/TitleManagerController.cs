using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebJob.Models;


namespace WebJob.Areas.Admin.Controllers
{
    public class TitleManagerController : Controller
    {
        // GET: Admin/TitleManager
        JobContextDataContext db = new JobContextDataContext() ;
        public ActionResult Title()
        {
            if (Session["account"] == null || Session["role"] == null)
            {
                return RedirectToAction("Login", "AdLogin");
            }
            var all_title = from tt in db.job_titles select tt;
            return View(all_title);
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
        public ActionResult Add(FormCollection collection,job_title dm)
        {
            var name = collection["name_title"];
         // var id = collection["id_title"];

            if (string.IsNullOrEmpty(name))
            {
                ViewData["Error"] = "Don't empty!";
            }
            else
            {
                dm.id_title = Nanoid.Nanoid.Generate(size:5);
                dm.name_title = name;
                db.job_titles.InsertOnSubmit(dm);
                db.SubmitChanges();
                return RedirectToAction("Title");
            }



            return this.Add();
        }
        public ActionResult Edit(String id)
        {
            var E_category = db.job_titles.First(m => m.id_title == id);
            return View(E_category);
        }
        [HttpPost]
        public ActionResult Edit(String id, FormCollection collection)
        {
            var titile = db.job_titles.First(m => m.id_title == id);
            var E_tenloai = collection["name_title"];
            titile.id_title = id;
            if (string.IsNullOrEmpty(E_tenloai))
            {
                ViewData["Error"] = "Don't empty!";
            }
            else
            {
                titile.name_title = E_tenloai;
                UpdateModel(titile);
                db.SubmitChanges();
                return RedirectToAction("Title");
            }
            return this.Edit(id);
        }
        public ActionResult Delete(String id)
        {
            job_title dm = db.job_titles.SingleOrDefault(x=>x.id_title ==id);
            db.job_titles.DeleteOnSubmit(dm);
            db.SubmitChanges();
            return Redirect(Request.UrlReferrer.ToString());
        }
    }
}

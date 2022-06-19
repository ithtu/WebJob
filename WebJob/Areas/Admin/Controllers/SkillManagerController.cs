using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebJob.Models;

namespace WebJob.Areas.Admin.Controllers
{
    public class SkillManagerController : Controller
    {
        // GET: Admin/SkillManager
        JobContextDataContext db = new JobContextDataContext();
        public ActionResult Skill()
        {
            if (Session["account"] == null || Session["role"] == null)
            {
                return RedirectToAction("Login", "AdLogin");
            }
            var all_skill = from tt in db.job_skills select tt;
            return View(all_skill);
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
        public ActionResult Add(FormCollection collection, job_skill dm)
        {
            var name = collection["name_skill"];
            //var id = collection["id_skill"];

            if (string.IsNullOrEmpty(name))
            {
                ViewData["Error"] = "Don't empty!";
            }
            else
            {
                dm.id_skill = Nanoid.Nanoid.Generate(size:5);
                dm.name_skill = name;
                db.job_skills.InsertOnSubmit(dm);
                db.SubmitChanges();
                return RedirectToAction("Skill");
            }



            return this.Add();
        }
        public ActionResult Edit(String id)
        {
            var E_skill = db.job_skills.First(m => m.id_skill == id);
            return View(E_skill);
        }
        [HttpPost]
        public ActionResult Edit(String id, FormCollection collection)
        {
            var skill = db.job_skills.First(m => m.id_skill == id);
            var E_skill = collection["name_skill"];
            skill.id_skill = id;
            if (string.IsNullOrEmpty(E_skill))
            {
                ViewData["Error"] = "Don't empty!";
            }
            else
            {
                skill.name_skill = E_skill;
                UpdateModel(skill);
                db.SubmitChanges();
                return RedirectToAction("Skill");
            }
            return this.Edit(id);
        }
        public ActionResult Delete(String id)
        {
            job_skill dm = db.job_skills.SingleOrDefault(x => x.id_skill == id);
            db.job_skills.DeleteOnSubmit(dm);
            db.SubmitChanges();
            return Redirect(Request.UrlReferrer.ToString());
        }
    }
}

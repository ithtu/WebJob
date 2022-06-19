using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebJob.Models;

namespace WebJob.Areas.Admin.Controllers
{
    public class JobManagerController : Controller
    {
        // GET: Admin/JobManager
        JobContextDataContext db = new JobContextDataContext();
        public ActionResult Job()
        {
            if (Session["account"] == null || Session["role"] == null)
            {
                return RedirectToAction("Login", "AdLogin");
            }
            var all_job = db.job_details.ToList().OrderBy(x=>x.job.id_job)  ;
                
            return View(all_job);

        }
        public ActionResult Add()
        {
            if (Session["account"] == null || Session["role"] == null)
            {
                return RedirectToAction("Login", "AdLogin");
            }
            ViewBag.id_skill = new SelectList(db.job_skills.ToList().OrderBy(x => x.id_skill), "id_skill", "name_skill");
                ViewBag.id_title = new SelectList(db.job_titles.ToList().OrderBy(x => x.id_title), "id_title", "name_title");
                ViewBag.id_company = new SelectList(db.companies.ToList().OrderBy(x => x.id_company), "id_company", "name_company");
                // ViewBag.id_job = new SelectList(db.jobs.ToList().OrderBy(x => x.id_job), "id_job", "id_job");

            return View();
            
        }
        [HttpPost]
        public ActionResult Add(FormCollection collection)
        {
            ViewBag.id_skill = new SelectList(db.job_skills.ToList().OrderBy(x => x.id_skill), "id_skill", "name_skill");
            ViewBag.id_title = new SelectList(db.job_titles.ToList().OrderBy(x => x.id_title), "id_title", "name_title");
            ViewBag.id_company = new SelectList(db.companies.ToList().OrderBy(x => x.id_company), "id_company", "name_company");
            // ViewBag.id_job = new SelectList(db.jobs.ToList().OrderBy(x => x.id_job), "id_job", "id_job");
            job_detail dm = new job_detail();
            //--------------------------------------------------------------------------------------------------------------------
            job jb = new job();
            var comm = collection["id_company"];
            var idtitle = collection["id_title"];

            company co = db.companies.SingleOrDefault(p => p.id_company == comm);
            job_title ti = db.job_titles.SingleOrDefault(p => p.id_title == idtitle);
            var name = collection["job.name"];
            jb.name = name;
            jb.id_job = Nanoid.Nanoid.Generate(size: 5);
            jb.id_company = co.id_company;
            jb.release_date = DateTime.Now;
            jb.id_title = ti.id_title;

            db.jobs.InsertOnSubmit(jb);
            /*db.SubmitChanges();*/



            //Add job detail---------------------------------------------------------------------------------------------------------

                var salary = collection["salary"];
                var amount = collection["amount"];
                var description = collection["job_description"];
                var requiment = collection["job_requiment"];
                var idskill = collection["id_skill"];
               /* var idjob = collection["id_job"];*/
                job_skill ski = db.job_skills.SingleOrDefault(p =>p.id_skill == idskill);
           

                dm.id_job_detail = Nanoid.Nanoid.Generate(size: 5);
                dm.id_job = jb.id_job;
                dm.id_skill = ski.id_skill;
                dm.salary = int.Parse(salary);
                dm.amount = int.Parse(amount);
                /*dm.job_description = description;
                dm.job_requiment = requiment;*/
                db.job_details.InsertOnSubmit(dm);
                db.SubmitChanges();

            
            //----------------------------------------------------------------------------------------------

            



            return RedirectToAction("Job");
           /* return this.Add();*/
        }
        public ActionResult Edit(String id)
        {
            if (Session["account"] == null || Session["role"] == null)
            {
                return RedirectToAction("Login", "AdLogin");
            }
            ViewBag.id_skill = new SelectList(db.job_skills.ToList().OrderBy(x => x.name_skill), "id_skill", "name_skill");
            ViewBag.id_title = new SelectList(db.job_titles.ToList().OrderBy(x => x.name_title), "id_title", "name_title");
            ViewBag.id_company = new SelectList(db.companies.ToList().OrderBy(x => x.name_company), "id_company", "name_company");
            var E_job = db.job_details.First(x => x.id_job_detail==id);
            return View(E_job);
        }
        [HttpPost]
        public ActionResult Edit(String id ,FormCollection collection)
        {
            ViewBag.id_skill = new SelectList(db.job_skills.ToList().OrderBy(x => x.id_skill), "id_skill", "name_skill");
            ViewBag.id_title = new SelectList(db.job_titles.ToList().OrderBy(x => x.id_title), "id_title", "name_title");
            ViewBag.id_company = new SelectList(db.companies.ToList().OrderBy(x => x.id_company), "id_company", "name_company");
            //---------------------------------
            job jb = new job();
            var comm = collection["id_company"];
            var idtitle = collection["id_title"];
            var name = collection["job.name"];
            var dm = db.job_details.First(m => m.id_job_detail == id);
            
            company co = db.companies.SingleOrDefault(p => p.id_company == comm);
            job_title ti = db.job_titles.SingleOrDefault(p => p.id_title == idtitle);
            
            jb.name = name;
            jb.id_job = dm.id_job;
            jb.id_company = co.id_company;
            jb.release_date = DateTime.Now;
            jb.id_title = ti.id_title;

            UpdateModel(jb);
            //edit detail
            
            
            var salary = collection["salary"];
            var amount = collection["amount"];
            var description = collection["job_description"];
            var requiment = collection["job_requiment"];
            var idskill = collection["id_skill"];
            job_skill ski = db.job_skills.SingleOrDefault(p => p.id_skill == idskill);
            dm.id_job_detail = id;
            dm.salary = int.Parse(salary);
            dm.amount = int.Parse(amount);
            dm.id_job = jb.id_job;
           /* dm.job_description = description;
            dm.job_requiment = requiment;*/
            dm.id_skill = ski.id_skill;
            UpdateModel(dm);

     
            

             db.SubmitChanges();
            return RedirectToAction("Job");
            
          
        }
        public ActionResult Delete(String id)
        {
            if (Session["account"] == null || Session["role"] == null)
            {
                return RedirectToAction("Login", "AdLogin");
            }
            job_detail dm = db.job_details.SingleOrDefault(x => x.id_job_detail == id);
            var item = db.jobs.SingleOrDefault(p => p.id_job == dm.id_job);
            db.job_details.DeleteOnSubmit(dm);
            db.jobs.DeleteOnSubmit(item);
            db.SubmitChanges();
            return Redirect(Request.UrlReferrer.ToString());
        }
        
    }
}
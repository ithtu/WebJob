using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebJob.Models;
using Microsoft.AspNet.Identity.Owin;
using System.IO;

namespace WebJob.Controllers
{
    public class JobsController : Controller
    {
        JobContextDataContext dt = new JobContextDataContext();
        // GET: Jobs
        public ActionResult ListBySkill()
        {
            var list1 = dt.job_skills.ToList();
            ViewBag.Skills = list1;
            return View(list1);
        }
        public ActionResult ListByTitle()
        {
            var list = dt.job_titles.ToList();
            return View(list);
        }
        public ActionResult ListByCompany()
        {
            var list = dt.companies.ToList();
            return View(list);
        }
        public ActionResult Details(string id)
        {
            employee acc = (employee)Session["account"];
            var list = dt.job_details.First(p => p.id_job_detail == id && p.job.active !=null);

            var listcmt = dt.comment_employees.Where(p => p.id_job_detail == id).ToList();
            ViewBag.ListCMT = listcmt;
            if (acc == null)
            {
                return View(list);
            }
            ViewBag.ID = acc.id_employee;
            following_job follow = dt.following_jobs.FirstOrDefault(p => p.id_employee == acc.id_employee && p.id_job_detail == list.id_job_detail);
            if (follow == null)
            {
                list.isFollowing = true;
            }
            return View(list);
        }
        ///Hide jobs in title

        public ActionResult ListJobs(string id)
        {
            var list = dt.job_details.Where(p => p.id_skill == id && p.job.active != null|| p.job.id_title == id && p.job.active != null || p.job.id_company == id && p.job.active != null).OrderBy(p => p.job.release_date).ToList();
            employee acc = (employee)Session["account"];
            if (acc == null)
            {
                foreach (var item in list)
                {
                    item.isFollowing = true;
                    item.isApply = true;
                }
                return View(list);
            }

            //follow
            foreach (var item in list)
            {
                following_job follow = dt.following_jobs.FirstOrDefault(p => p.id_employee == acc.id_account && p.id_job_detail == item.id_job_detail);
                if (follow == null)
                {
                    item.isFollowing = true;
                }
                apply_job apply = dt.apply_jobs.FirstOrDefault(p => p.id_employee == acc.id_account && p.id_job_detail == item.id_job_detail);
                if( apply == null)
                {
                    item.isApply = true;
                }
            }

            return View(list);
        }
        [HttpPost]
        public JsonResult SearchJob(String keyword)
        {
            if (string.IsNullOrEmpty(keyword))
            {
                return Json(null);
            }
            var result = new List<Object>();
            /*List<string> b = new List<string>();*/
            /*foreach*/
            var listResult = dt.job_details/*.Where(s => s.active_status == null || s.active_status == true)*/.Where(p => p.job.name.Contains(keyword.Trim().ToLower()) ||
                                                                                                                      p.job_skill.name_skill.Contains(keyword.Trim().ToLower()) ||
                                                                                                                      p.job.job_title.name_title.Contains(keyword.Trim().ToLower()) ||
                                                                                                                      p.job.company.name_company.Contains(keyword.Trim().ToLower())).ToList();
            if (listResult == null)
            {
                return Json(null);
            }
            else
            {
                listResult = listResult.OrderByDescending(p => p.job.release_date).Take(5).ToList();
                foreach (var item in listResult)
                {
                    var resultJob = dt.job_details.Where(p => p.id_job == item.id_job && p.job.active !=null)
                                                                                   .OrderByDescending(p => p.amount)
                                                                                   .Take(1).Select(a => new {
                                                                                       /*id = a.version_id,*/
                                                                                       job_name = a.job.name,
                                                                                       skill = a.job_skill.name_skill,
                                                                                       salary = string.Format("{0:C0}", a.salary),

                                                                                       id = a.id_job_detail
                                                                                   });
                    /*b.Add(resultPhone_version.product.product_name);*/
                    result.Add(resultJob);
                }
            }
            return Json(result);
        }

        [HttpGet]
        public ActionResult SaveJob()
        {
            if (Session["account"] == null)
            {
                return RedirectToAction("Login", "User");
            }
            var list = dt.following_jobs.ToList();
            return View(list);
        }

        [HttpPost]
        public ActionResult SaveJob(string id)
        {
            following_job job = new following_job();
            employee acc = (employee)Session["account"];
            job.id_employee = acc.id_employee;
            job.id_job_detail = id;


            var following = dt.following_jobs.FirstOrDefault(p => p.id_employee == job.id_employee && p.id_job_detail == job.id_job_detail);
            if (following == null)
            {

                dt.following_jobs.InsertOnSubmit(job);
                dt.SubmitChanges();
            }
            else
            {
                dt.following_jobs.DeleteOnSubmit(following);
                dt.SubmitChanges();
            }
            dt.SubmitChanges();
            return this.SaveJob();
        }
        [HttpPost]
        public ActionResult Comment(string id, string content)
        {
            employee acc = (employee)Session["account"];
            if (acc == null)
            {
                return RedirectToAction("Login", "User");
            }
            else
            {

                comment_employee cmt = new comment_employee();
                cmt.id_comments = Nanoid.Nanoid.Generate(size: 10);
                cmt.id_employee = acc.id_employee;
                cmt.comment_date = DateTime.Now;
                cmt.id_job_detail = id;
                cmt.content = content;

                dt.comment_employees.InsertOnSubmit(cmt);
                dt.SubmitChanges();
            }

            return Content("ok");
        }
        [HttpPost]
        public ActionResult DelComment(string id, string idcmt)
        {
            employee acc = (employee)Session["account"];
            var find = dt.comment_employees.Where(p => p.id_employee == acc.id_employee && p.id_job_detail == id && p.id_comments == idcmt);
            var find2 = dt.comment_employees.ToList();

            if (find != null)
            {
                dt.comment_employees.DeleteAllOnSubmit(find);
            }
            dt.SubmitChanges();
            return Content("oke");
        }
        [HttpGet]
        public ActionResult ApplyJob(string id)
        {
            employee acc = (employee)Session["account"];
            if(acc == null)
            {
                return RedirectToAction("Login", "User");
            }
            var item = dt.job_details.Where(p =>p.id_job_detail == id).First() ;

            return View(item);
        }
        [HttpPost]
        public ActionResult ApplyJob(FormCollection collection, apply_job apply, string id)
        {
            var item = dt.job_details.Where(p => p.id_job_detail == id).First();
            employee acc = (employee)Session["account"];
            var name = collection["cv_name"];
            var filecv = collection["cv_file"];
            var advantages = collection["cv_advantages"];

            apply.id_job_detail = item.id_job_detail;
            apply.id_employee = acc.id_employee;
            apply.name_apply = name;
            apply.cv_apply = filecv;
            apply.note = advantages;

            dt.apply_jobs.InsertOnSubmit(apply);
            dt.SubmitChanges();

            return RedirectToAction("Index","Home");
        }
        public string FileUpLoad(HttpPostedFileBase file)
        {
            String FileExt = Path.GetExtension(file.FileName).ToLower();


            if (FileExt == ".pdf" || FileExt == ".doc" || FileExt == ".docx")
            {
                file.SaveAs(Server.MapPath("~/Content/CV/" + file.FileName));
                return  file.FileName;

            }
            else
            {
                ViewBag.Error = "File khác định dạng";
                return ViewBag.Error;

            }
        }
       /* public FileResult Download(string fileName)
        {
            string path = Server.MapPath("~/Content/CV") + fileName;
            byte[] bytes = System.IO.File.ReadAllBytes(path);
            return File(bytes, "application/pdf", fileName);
        }*/
       /* [HttpPost]
        public ActionResult ReplyComment(string id,string idCmt, string content)
        {
            employee acc = (employee)Session["account"];
            
            if (acc == null)
            {
                return RedirectToAction("Login", "User");
            }
            else
            {
                *//*var find = dt.comment_employees.Where(p => p.id_comments == idCmt);
                if()*//*
                comment_employee cmt = new comment_employee();
                cmt.id_comments = Nanoid.Nanoid.Generate(size: 10);
                cmt.id_employee = acc.id_employee;
                cmt.comment_date = DateTime.Now;
                cmt.id_job_detail = id;
                cmt.content = content;

                dt.comment_employees.InsertOnSubmit(cmt);
                dt.SubmitChanges();
            }

            return Content("ok");

        }*/
    }
}
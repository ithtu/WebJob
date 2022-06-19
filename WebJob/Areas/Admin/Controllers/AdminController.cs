using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebJob.Models;
namespace WebJob.Areas.Admin.Controllers
{
    public class AdminController : Controller
    {
        JobContextDataContext dt = new JobContextDataContext();
        // GET: Admin/Admin
        public ActionResult Index()
        {
            if (Session["account"] == null || Session["role"] == null)
            {
                return RedirectToAction("Login", "AdLogin");
            }
            var list = dt.employees.Where(p=>p.account.confirm == null).ToList();
            var list2 = dt.companies.Where(p => p.account.confirm == null).ToList();
            ViewBag.companies = list2;
            return View(list);
        }
        

        [HttpPost]
        public ActionResult ConfirmEmp(string id)
        {
            account acc = (account)Session["account"];
            if (acc == null || acc.id_role != 1)
            {
                return RedirectToAction("Login", "AdLogin");
            }
            var item = dt.accounts.FirstOrDefault(p => p.id_account == id);

            item.confirm = "t";
            UpdateModel(item);
            dt.SubmitChanges();
            return RedirectToAction("Index");
        }
        [HttpPost]
        public ActionResult DelConfirmEmp(string id)
        {

            if (Session["account"] == null || Session["role"] == null)
            {
                return RedirectToAction("Login", "AdLogin");
            }
            var item = dt.accounts.FirstOrDefault(p => p.id_account == id);

            
            dt.accounts.DeleteOnSubmit(item);
            dt.SubmitChanges();
            return RedirectToAction("Index");
        }
        public ActionResult Customer()
        {
            if (Session["account"] == null || Session["role"] == null)
            {
                return RedirectToAction("Login", "AdLogin");
            }

            var list = dt.employees.Where(p => p.account.id_role == 2).ToList();
                return View(list);
           
            
        }
        public ActionResult Company()
        {
            if (Session["account"] == null || Session["role"] == null)
            {
                return RedirectToAction("Login", "AdLogin");
            }


            var list = dt.companies.Where(p => p.account.id_role == 3).ToList();
            return View(list);


        }
        public ActionResult Admin()
        {
            if (Session["account"] == null || Session["role"] == null)
            {
                return RedirectToAction("Login", "AdLogin");
            }


            var list = dt.employees.Where(p => p.account.id_role == 1).ToList();
            return View(list);


        }
        public ActionResult ConfirmJob()
        {
            if (Session["account"] == null || Session["role"] == null)
            {
                return RedirectToAction("Login", "AdLogin");
            }

            var list = dt.job_details.Where(p => p.job.active == null).ToList();
            return View(list);
        }
        [HttpPost]
        public ActionResult ConfirmJobs(string id)
        {
            if (Session["account"] == null || Session["role"] == null)
            {
                return RedirectToAction("Login", "AdLogin");
            }
            var item = dt.job_details.FirstOrDefault(p => p.id_job_detail == id);

            item.job.active = "t";
            UpdateModel(item);
            dt.SubmitChanges();
            return RedirectToAction("Index");
        }
        [HttpPost]
        public ActionResult DelConfirmJob(string id)
        {
            if (Session["account"] == null || Session["role"] == null)
            {
                return RedirectToAction("Login", "AdLogin");
            }
            var item = dt.job_details.FirstOrDefault(p => p.id_job_detail == id);
            var item2 = dt.jobs.FirstOrDefault(p => p.id_job == item.id_job);

            dt.job_details.DeleteOnSubmit(item);
            dt.jobs.DeleteOnSubmit(item2);
            dt.SubmitChanges();
            return RedirectToAction("Index");
        }
    }
}
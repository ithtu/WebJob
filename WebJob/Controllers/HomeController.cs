using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebJob.Models;

namespace WebJob.Controllers
{
    public class HomeController : Controller
    {
        JobContextDataContext dt = new JobContextDataContext();   
        public ActionResult Index()
        {
            var list = dt.job_skills.ToList().Take(20);
            ViewBag.Skills = list;
            var list2 = dt.companies.ToList().Take(8);
            ViewBag.Companies = list2;
           foreach (var item in list2)
            {
                var job = dt.job_details.Where(p => p.job.id_company == item.id_company);
                ViewBag.Count = job.Count();
            }    
           
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
        public ActionResult ConfirmSignUp(string Token, string id)
        {
            token tokenConfirm = dt.tokens.SingleOrDefault(p => p.Token1 == Token && DateTime.Now >= p.time1 && DateTime.Now <= p.time2);
            account acc = dt.accounts.SingleOrDefault(p => p.id_account == id);
            if (tokenConfirm != null && acc != null)
            {
                acc.confirm = "XN";
                UpdateModel(acc);
                /*dt.SubmitChanges();*/
                dt.tokens.DeleteOnSubmit(tokenConfirm);
                dt.SubmitChanges();
                return View(acc);
            }
            else
                ViewBag.ThongBao = "LINK XÁC NHẬN ĐÃ HẾT HẠN";
            return View(acc);
        }
        public ActionResult ConfirmResetMail(string Token, string AccID)
        {
            token tokenConfirm = dt.tokens.SingleOrDefault(p => p.Token1 == Token && DateTime.Now >= p.time1 && DateTime.Now <= p.time2);
            account acc = dt.accounts.SingleOrDefault(p => p.id_account == AccID);

            if (tokenConfirm != null && acc != null)
            {
                acc.password = Nanoid.Nanoid.Generate(size: 10);
                UpdateModel(acc);
                dt.SubmitChanges();
                dt.tokens.DeleteOnSubmit(tokenConfirm);
                dt.SubmitChanges();
                return View(acc);
            }
            else
                ViewBag.ThongBao = "LINK XÁC NHẬN ĐÃ HẾT HẠN";
            return View(acc);
        }
        [ChildActionOnly]
        public ActionResult RenderNav()
        {
            var list = dt.job_skills.ToList().Take(20);
            ViewBag.Skills = list;
            var list2 = dt.job_titles.ToList().Take(20);
            ViewBag.Titles = list2;
            var list3 = dt.companies.ToList().Take(20);
            ViewBag.Companies = list3;
            return PartialView("_Header");
        }
    }

}
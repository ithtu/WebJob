using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Web;
using System.Web.Mvc;
using WebJob.Models;

namespace WebJob.Controllers
{
    public class CompanyController : Controller
    {
        JobContextDataContext dt = new JobContextDataContext();
        // GET: Company
        
        public ActionResult Details(string id)
        {
            employee acc = (employee)Session["account"];
            var item = dt.jobs.FirstOrDefault(p => p.id_company == id);
            var list = dt.job_details.Where(p => p.job.id_company == id).ToList();
            ViewBag.JobCompany = list;
            
            if (acc == null)
            {
                item.company.isFollowing = true;
                return View(item);
            }
            
            
            following_company follow = dt.following_companies.FirstOrDefault(p => p.id_employee == acc.id_employee && p.id_company == id);
            if (follow == null)
            {
                item.company.isFollowing = true;
            }
            return View(item);
        }
        [HttpGet]
        public ActionResult FollowCompany()
        {
            employee acc = (employee)Session["account"];
            if(acc == null)
            {
                return RedirectToAction("Login", "User");
            } 
            
            return View();
        }
        public ActionResult ListJobs(string id)
        {
            var list = dt.job_details.Where(p => p.id_skill == id || p.job.id_title == id || p.job.id_company == id).OrderBy(p => p.job.release_date).ToList();
            company acc = (company)Session["account"];
            if (acc == null)
            {
                foreach (var item in list)
                {
                    item.isFollowing = true;
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
            }

            return View(list);
        }
        [HttpPost]
        public ActionResult FollowCompany(string id)
        {
            following_company follow = new following_company();

            employee acc = (employee)Session["account"];
            if (acc == null)
            {
                return RedirectToAction("Login", "User");
            }
            follow.id_employee = acc.id_employee;
            follow.id_company = id;


            var following = dt.following_companies.FirstOrDefault(p => p.id_employee == follow.id_employee && p.id_company == follow.id_company);
            if (following == null)
            {

                dt.following_companies.InsertOnSubmit(follow);
                dt.SubmitChanges();
            }
            else
            {
                dt.following_companies.DeleteOnSubmit(following);
                dt.SubmitChanges();
            }
            dt.SubmitChanges();
            return this.FollowCompany();
        }
        public ActionResult RegisterCompany()
        {
            return View();
        }
        [HttpPost]
        public ActionResult RegisterCompany(FormCollection collection)
        {
            account acc = new account();
            company company = new company();

            var name = collection["name"];
            var pass = collection["password"];
            var confirmPass = collection["confirm_password"];
            var email = collection["email"];
            var address = collection["address"];
            var phoneNumber = collection["phone"];
            var logo = collection["logo"];


            var acc_check_mail = dt.employees.SingleOrDefault(s => s.email == email);
            var acc_check_phone = dt.employees.SingleOrDefault(s => s.phonenumber_employee == phoneNumber);


            if (String.IsNullOrEmpty(confirmPass))
            {
                ViewData["Error_Null_ConfirmPass"] = "Vui lòng nhập mật khẩu xác nhận !!!";
                return this.RegisterCompany();
            }
            else if (acc_check_mail != null && acc_check_phone != null)
            {
                ViewData["Error_mail"] = "Email này đã được đăng ký !!!";
                ViewData["Error_phone"] = "Số điện thoại này đã được đăng ký !!!";
                return this.RegisterCompany();
            }
            else if (acc_check_mail != null)
            {
                ViewData["Error_mail"] = "Email này đã được đăng ký !!!";
                return this.RegisterCompany();
            }
            else if (acc_check_phone != null)
            {
                ViewData["Error_phone"] = "Số điện thoại này đã được đăng ký !!!";
                return this.RegisterCompany();
            }
            else
            {
                if (!pass.Equals(confirmPass))
                {
                    ViewData["Error_Not_Same"] = "Mật khẩu không khớp !!!";
                    return this.RegisterCompany();
                }
                else
                {
                    //ADD account
                    acc.id_account = Nanoid.Nanoid.Generate(size: 10);

                    acc.password = pass;
                    acc.id_role = 3; // 1: Admin 2: Khách Hàng
                    dt.accounts.InsertOnSubmit(acc);
                    /*dt.SubmitChanges();*/

                    //ADD customer
                    company.id_company = Nanoid.Nanoid.Generate(size: 10);
                    company.name_company = name;
                    company.address = address;
                    company.email = email;
                    company.phonenumber = phoneNumber;
                    company.logo = logo;

                    company.id_account = acc.id_account;
                    dt.companies.InsertOnSubmit(company);
                    dt.SubmitChanges();

                    if (ModelState.IsValid)
                    {
                        try
                        {
                            var senderEmail = new MailAddress("quoctupdn@gmail.com", "CellphoneX");
                            var receiverEmail = new MailAddress(email, "Receiver");
                            var password = "";
                            var sub = "XAC_THUC_TAI_KHOAN";
                            token tk = new token();
                            tk.Token1 = Nanoid.Nanoid.Generate(size: 10);
                            tk.time1 = DateTime.Now;
                            tk.time2 = DateTime.Now.AddMinutes(2);
                            dt.tokens.InsertOnSubmit(tk);
                            dt.SubmitChanges();
                            var link = string.Format("{0}", Url.Action("ConfirmSignUp", "Home", new { Token = tk.Token1, id = acc.id_account }, Request.Url.Scheme));
                            var body = "Vui lòng click vào link để xác nhận tài khoản: " + link + "\n" +
                                        "Xác nhận này chỉ có hiệu lực đến " + DateTime.Now.AddMinutes(2) + "\n" +
                                        "Xin cảm ơn quý khách !!!";

                            var smtp = new SmtpClient
                            {
                                Host = "smtp.gmail.com",
                                Port = 587,
                                EnableSsl = true,
                                DeliveryMethod = SmtpDeliveryMethod.Network,
                                UseDefaultCredentials = false,
                                Credentials = new NetworkCredential(senderEmail.Address, password)
                            };
                            using (var mess = new MailMessage(senderEmail, receiverEmail)
                            {
                                Subject = sub,
                                Body = body,

                            })
                            {
                                smtp.Send(mess);
                            }
                            return RedirectToAction("ConfirmSignup", "User");

                        }
                        catch (Exception)
                        {
                            ViewBag.Error = "Some Error";
                        }
                    }

                }
                return RedirectToAction("Index", "Home");
            }
        }
        public ActionResult LoginCompany()
        {
            return View();
        }
        [HttpPost]
        public ActionResult LoginCompany(FormCollection collection)
        {
            var email = collection["email"];
            var password = collection["password"];

            company acc1 = dt.companies.SingleOrDefault(p => p.email == email && p.account.password == password);
            if (acc1 != null)
            {


                Session["account"] = acc1;
                if (acc1.account.confirm == null)
                {
                    ViewBag.Notify = "Tài khoản không tồn tại !!!";
                    return this.LoginCompany();
                }
                else
                {
                    Session["name"] = acc1.name_company.ToString();
                    Session["role"] = acc1.account.id_role;

                    return RedirectToAction("Index", "Home");

                }
            }
            ViewBag.Notify = "Tên đăng nhập hoặc mật khẩu không đúng !!!";

            return this.LoginCompany();
        }
        public ActionResult AddJob()
        {
            ViewBag.id_skill = new SelectList(dt.job_skills.ToList().OrderBy(x => x.id_skill), "id_skill", "name_skill");
            ViewBag.id_title = new SelectList(dt.job_titles.ToList().OrderBy(x => x.id_title), "id_title", "name_title");
            ViewBag.id_company = new SelectList(dt.companies.ToList().OrderBy(x => x.id_company), "id_company", "name_company");
            return View();
        }
        [HttpPost]
        public ActionResult AddJob(FormCollection collection)
        {

            ViewBag.id_skill = new SelectList(dt.job_skills.ToList().OrderBy(x => x.id_skill), "id_skill", "name_skill");
            ViewBag.id_title = new SelectList(dt.job_titles.ToList().OrderBy(x => x.id_title), "id_title", "name_title");
            ViewBag.id_company = new SelectList(dt.companies.ToList().OrderBy(x => x.id_company), "id_company", "name_company");
            // ViewBag.id_job = new SelectList(db.jobs.ToList().OrderBy(x => x.id_job), "id_job", "id_job");
            job_detail dm = new job_detail();
            //--------------------------------------------------------------------------------------------------------------------
            job jb = new job();
            company acc = (company)Session["account"];
            
            var idtitle = collection["id_title"];

            company co = dt.companies.SingleOrDefault(p => p.id_company == acc.id_company);
            job_title ti = dt.job_titles.SingleOrDefault(p => p.id_title == idtitle);
            var name = collection["job.name"];
            jb.name = name;
            jb.id_job = Nanoid.Nanoid.Generate(size: 5);
            jb.id_company = co.id_company;
            jb.release_date = DateTime.Now;
            jb.id_title = ti.id_title;

            dt.jobs.InsertOnSubmit(jb);
             //Add job detail---------------------------------------------------------------------------------------------------------

            var salary = collection["salary"];
            var amount = collection["amount"];
            var description = collection["job_description"];
            var requiment = collection["job_requiment"];
            var idskill = collection["id_skill"];
            /* var idjob = collection["id_job"];*/
            job_skill ski = dt.job_skills.SingleOrDefault(p => p.id_skill == idskill);


            dm.id_job_detail = Nanoid.Nanoid.Generate(size: 5);
            dm.id_job = jb.id_job;
            dm.id_skill = ski.id_skill;
            dm.salary = decimal.Parse(salary);
            dm.amount = int.Parse(amount);
            /*dm.job_description = description;
            dm.job_requiment = requiment;*/
            dt.job_details.InsertOnSubmit(dm);
            dt.SubmitChanges();
            return RedirectToAction("Index","Home");
            /* return this.Add();*/
        }
        public ActionResult MyJobs(string id)
        {
            company acc = (company)Session["account"];
            id = acc.id_company;
            var list = dt.job_details.Where(p => p.job.id_company == id);
            return View(list);

        }
        public ActionResult ListCV(string id)
        {
            company acc = (company)Session["account"];
            id = acc.id_company;
            var list = dt.apply_jobs.Where(p => p.job_detail.job.id_company == id).ToList();
            return View(list);
        }
        public FileResult Download(string fileName)
        {
            string path = Server.MapPath("~/Content/CV/" + fileName) ;
            byte[] bytes = System.IO.File.ReadAllBytes(path);
            return File(bytes, "application/pdf", fileName);
        }


    }
}
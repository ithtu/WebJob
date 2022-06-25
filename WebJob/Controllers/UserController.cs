using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Web;
using System.Web.Mvc;
using WebJob.Models;

namespace WebJob.Controllers
{
    public class UserController : Controller
    {
        JobContextDataContext dt = new JobContextDataContext();
        // GET: UserLogin
        public ActionResult Index()
        {
            account acc = (account)Session["account"];
            if (acc == null || acc.id_role != 2 || acc.confirm == null)
            {
                return RedirectToAction("Login", "User");
            }
            employee emp = dt.employees.SingleOrDefault(p => p.id_account == acc.id_account);
            return View(emp);
        }
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
         
            if (acc != null )
            {

                Session["account"] = acc;

                if (acc.account.id_role == 1)
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
                    Session["name"] = acc.name_employee.ToString();
                    return RedirectToAction("Index", "Home");
                }                    
            }
            ViewBag.Notify = "Tên đăng nhập hoặc mật khẩu không đúng !!!";

            return this.Login();
        }
        public ActionResult Register() 
        {
            return View();
        }
        [HttpPost]
        public ActionResult Register(FormCollection collection)
        {
            account acc = new account();
            employee employee = new employee();
            var name = collection["name"];
            var pass = collection["password"];
            var confirmPass = collection["confirm_password"];
            var email = collection["email"];
            var address = collection["address"];
            var phoneNumber = collection["phone"];
            var sex = collection["gender"];
            var ava = collection["logo"];
            var dateOfBirth = string.Format("{0:MM/dd/yyyy}", collection["birthday"]);

            var acc_check_mail = dt.employees.SingleOrDefault(s => s.email == email);
            var acc_check_phone = dt.employees.SingleOrDefault(s => s.phonenumber_employee == phoneNumber);
            

            if (String.IsNullOrEmpty(confirmPass))
            {
                ViewData["Error_Null_ConfirmPass"] = "Vui lòng nhập mật khẩu xác nhận !!!";
                return this.Register();
            }
            else if (acc_check_mail != null && acc_check_phone != null )
            {
                ViewData["Error_mail"] = "Email này đã được đăng ký !!!";
                ViewData["Error_phone"] = "Số điện thoại này đã được đăng ký !!!";
                                return this.Register();
            }
            else if (acc_check_mail != null)
            {
                ViewData["Error_mail"] = "Email này đã được đăng ký !!!";
                return this.Register();
            }
            else if (acc_check_phone != null)
            {
                ViewData["Error_phone"] = "Số điện thoại này đã được đăng ký !!!";
                return this.Register();
            }         
            else if (!pass.Equals(confirmPass))
            {
                ViewData["Error_Not_Same"] = "Mật khẩu không khớp !!!";
                return this.Register();
            }         
            else
            {
                //ADD account
                acc.id_account = Nanoid.Nanoid.Generate(size: 10);
                        
                acc.password = pass;
                acc.id_role = 2 ; // 1: Admin 2: Khách Hàng
                dt.accounts.InsertOnSubmit(acc);
                        

                //ADD customer
                employee.id_account = acc.id_account;
                employee.id_employee = Nanoid.Nanoid.Generate(size: 10);
                employee.name_employee = name;
                employee.day_birth = DateTime.Parse(dateOfBirth);
                employee.gender = sex;
                employee.email = email;
                employee.phonenumber_employee = phoneNumber;
                employee.address_employee = address;
                employee.avatar = ava;
                    
                        
                dt.employees.InsertOnSubmit(employee);
                dt.SubmitChanges();


               /* try
                {
                    var senderEmail = new MailAddress("store.confirmmail@gmail.com", "CellphoneX");
                    var receiverEmail = new MailAddress(email, "Receiver");
                    var password = "tqfjkbylnznhkzhx";
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
                }*/


                return RedirectToAction("Index", "Home");

            }
           
        }
            
        
       

        public ActionResult ConfirmSignup()
        {
            return View();
        }
        public ActionResult Logout()
        {
            Session.Clear();//remove session
            return RedirectToAction("Index","Home");
        }
        public ActionResult ListSave()
        {
            employee acc = (employee)Session["account"];
            var list = dt.following_jobs.Where(p => p.id_employee == acc.id_employee).ToList();
            return View(list);
        }
        public ActionResult profile()
        {
            employee acc = (employee)Session["account"];
            employee emp = dt.employees.SingleOrDefault(p => p.id_employee == acc.id_employee);
            return View(emp);
        }
        public ActionResult EditProfile(string id)
        {
            var item = dt.employees.FirstOrDefault(p => p.id_employee == id);
            return View(item);
        }
        [HttpPost]
        public ActionResult EditProfile(string id, FormCollection collection)
        {
            
            var item = dt.employees.FirstOrDefault(p => p.id_employee == id);
            var name = collection["name"];
            
            var email = collection["email"];
            var address = collection["address"];
            var phoneNumber = collection["phone"];
            var sex = collection["gender"];
            var ava = collection["logo"];
            var dateOfBirth = string.Format("{0:MM/dd/yyyy}", collection["birthday"]);

            item.name_employee = name;
            item.day_birth = DateTime.Parse(dateOfBirth);
            item.address_employee = address;
            item.gender = sex;
            item.avatar = ava;
            item.phonenumber_employee = phoneNumber;
            UpdateModel(item);
            dt.SubmitChanges();
            return RedirectToAction("profile");
              
        }
        public ActionResult ListFollow()
        {
            employee acc = (employee)Session["account"];
            var list1 = dt.following_companies.Where(p => p.id_employee == acc.id_employee).ToList();
            return View(list1);
        }
        public ActionResult ListApply()
        {

            employee acc = (employee)Session["account"];
            var list1 = dt.apply_jobs.Where(p => p.id_employee == acc.id_employee).ToList();
            return View(list1);
        }
        public FileResult Download(string fileName)
        {
            string path = Server.MapPath("~/Content/CV/" + fileName);
            byte[] bytes = System.IO.File.ReadAllBytes(path);
            return File(bytes, "application/pdf", fileName);
        }
        public string ProcessUpload(HttpPostedFileBase file)
        {
            if (file == null)
            {
                return "";
            }
                
            file.SaveAs(Server.MapPath("~/Content/images/" + file.FileName));
            return "/Content/images/" + file.FileName;
        }
    }
}
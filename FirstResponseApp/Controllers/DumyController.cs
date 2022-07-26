using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using FirstResponseApp.Data;
using FirstResponseApp.Helper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace FirstResponseApp.Controllers
{
    public class DumyController : Controller
    {
        private readonly ApplicationDbContext _applicationDbContext;

        public DumyController(ApplicationDbContext applicationDbContext)
        {
            this._applicationDbContext = applicationDbContext;
        }
        public IActionResult Index()
        {

            // var t = _applicationDbContext.TbUsers.FromSqlInterpolated($"GetList s").ToList();

            Matter obj = new Matter();
            List<SelectListItem> list = new List<SelectListItem>();
            list.Add(new SelectListItem { Text = "Linfox First Response", Value = "1" });
            list.Add(new SelectListItem { Text = "Linfox Reports Following Examination", Value = "2" });
            list.Add(new SelectListItem { Text = "Linfox Rehabilitation Reviews/ Examinations", Value = "3" });
            list.Add(new SelectListItem { Text = "Linfox Reviews On The Papers (No Exam)", Value = "4" });
            list.Add(new SelectListItem { Text = "Linfox Follow Up", Value = "5" });
            list.Add(new SelectListItem { Text = "First Response Calendar", Value = "6" });
            list.Add(new SelectListItem { Text = "Armaguard", Value = "7" });
            list.Add(new SelectListItem { Text = "Armaguard First Response", Value = "8" });
            list.Add(new SelectListItem { Text = "Armaguard Report Following Examinations", Value = "9" });
            list.Add(new SelectListItem { Text = "Armaguard Reviews On The Paper (No Exam)", Value = "10" });
            list.Add(new SelectListItem { Text = "Armaguard Rehabilitation Reviews/ Examinations", Value = "11" });
            list.Add(new SelectListItem { Text = "Armaguard Follow Up", Value = "12" });
            list.Add(new SelectListItem { Text = "Linfox", Value = "13" });

            List<SelectListItem> listOrg = new List<SelectListItem>();
            listOrg.Add(new SelectListItem { Text = "Linfox", Value = "1" });
            listOrg.Add(new SelectListItem { Text = "HWLE", Value = "2" });
            listOrg.Add(new SelectListItem { Text = "Midlaw", Value = "3" });
            obj.ChannelList = list;
            obj.WaitingOnList = listOrg;

            string tem11 = generatePassword(7);

            return View(obj);
        }


        public string generatePassword(int length)
        {
            string validChars = "ABCDEFGHJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789!@#$%^&*?_-";
            Random random = new Random();

            // Select one random character at a time from the string  
            // and create an array of chars  
            char[] chars = new char[length];
            for (int i = 0; i < length; i++)
            {
                chars[i] = validChars[random.Next(0, validChars.Length)];
            }
            return new string(chars);
        }


        public IActionResult SendMail()
        {
            var mailingList = new List<string>()
            {
                "shelendra.vishwakarma@systematixindia.com",
                "darpan.chaturvedi@systematixindia.com",
                "puneet.shinde@systematixindia.com",
                "akhilesh.upadhyay@systematixindia.com",
                "nimish.tiwari@systematixindia.com",
                "nitesh.tiwari@systematixindia.com",
                "ankita.shrivastava@systematixindia.com"
            };

            string subject = "Salary Increment ";
            string body = "Dear Employee,<br /><br />Your work parformance is too good last 1 month. Managment decied to give party. you can go for party with your family and Reimbursement bill upto 5000 <br /><br />Thanks.";

            Parallel.ForEach(mailingList, row =>
            {
                SendEmail(row, subject, body);
            });

            return View();
        }

        public IActionResult AddNewUser()
        {
            User obj = new User();
            List<SelectListItem> listOrg = new List<SelectListItem>();
            listOrg.Add(new SelectListItem { Text = "Linfox", Value = "1" });
            listOrg.Add(new SelectListItem { Text = "HWLE", Value = "2" });
            listOrg.Add(new SelectListItem { Text = "Midlaw", Value = "3" });

            obj.OrganizetionList = listOrg;
            return View(obj);
        }


        private bool SendEmail(string recipient, string subject, string body)
        {
            MailMessage mm = new MailMessage("shelendra.vishwakarma@gmail.com", recipient);
            mm.Subject = subject;
            mm.Body = body;
            mm.IsBodyHtml = true;
            SmtpClient smtp = new SmtpClient();
            smtp.Host = "smtp.gmail.com";
            smtp.EnableSsl = true;
            NetworkCredential NetworkCred = new NetworkCredential();
            NetworkCred.UserName = "siplInboundio@gmail.com";
            NetworkCred.Password = "Sipl@1234";
            smtp.UseDefaultCredentials = true;
            smtp.Credentials = NetworkCred;
            smtp.Port = 587;
            smtp.Send(mm);
            return true;
        }


        [Obsolete]
        public async Task<JsonResult> GetApplicationData(string FilterSearch)
        {
            List<AddList> Model = new List<AddList>();
            try
            {
                FilterSearch = string.IsNullOrEmpty(FilterSearch) ? "" : FilterSearch;


                for (int i = 0; i <= 10; i++)
                {
                    AddList obj = new AddList();
                    obj.Id = i;
                    obj.Name = "test";
                    obj.Channel = "channel";
                    obj.WaitingOn = "waiting";
                    obj.AddedBy = "AddedBy";
                    obj.AddedOn = "AddedOn";
                    Model.Add(obj);
                }

            }
            catch (Exception ex)
            {

            }
            return Json(new { recordsFiltered = Model.Count(), recordsTotal = Model.Count(), data = Model });

        }

        public IActionResult CreateMatter()
        {
            Matter obj = new Matter();
            List<SelectListItem> list = new List<SelectListItem>();
            list.Add(new SelectListItem { Text = "name", Value = "1" });
            List<SelectListItem> listOrg = new List<SelectListItem>();
            listOrg.Add(new SelectListItem { Text = "Linfox", Value = "1" });
            listOrg.Add(new SelectListItem { Text = "HWLE", Value = "2" });
            listOrg.Add(new SelectListItem { Text = "Midlaw", Value = "3" });

            List<SelectListItem> listUrs = new List<SelectListItem>();
            listUrs.Add(new SelectListItem { Text = "Barry", Value = "1" });
            listUrs.Add(new SelectListItem { Text = "John", Value = "2" });
            listUrs.Add(new SelectListItem { Text = "Stan", Value = "3" });
            obj.ChannelList = list;
            obj.WaitingOnList = listOrg;
            obj.UserNameList = listUrs;
            return View(obj);
        }

        public IActionResult TicketDetail()
        {
            return View();
        }

        public IActionResult Profile()
        {
            return View();
        }
        public IActionResult AddNewTicket()
        {
            Matter obj = new Matter();
            List<SelectListItem> list = new List<SelectListItem>();
            list.Add(new SelectListItem { Text = "Linfox First Response", Value = "1" });
            list.Add(new SelectListItem { Text = "Linfox Reports Following Examination", Value = "2" });
            list.Add(new SelectListItem { Text = "Linfox Rehabilitation Reviews/ Examinations", Value = "3" });
            list.Add(new SelectListItem { Text = "Linfox Reviews On The Papers (No Exam)", Value = "4" });
            list.Add(new SelectListItem { Text = "Linfox Follow Up", Value = "5" });
            list.Add(new SelectListItem { Text = "First Response Calendar", Value = "6" });
            list.Add(new SelectListItem { Text = "Armaguard", Value = "7" });
            list.Add(new SelectListItem { Text = "Armaguard First Response", Value = "8" });
            list.Add(new SelectListItem { Text = "Armaguard Report Following Examinations", Value = "9" });
            list.Add(new SelectListItem { Text = "Armaguard Reviews On The Paper (No Exam)", Value = "10" });
            list.Add(new SelectListItem { Text = "Armaguard Rehabilitation Reviews/ Examinations", Value = "11" });
            list.Add(new SelectListItem { Text = "Armaguard Follow Up", Value = "12" });
            list.Add(new SelectListItem { Text = "Linfox", Value = "13" });

            List<SelectListItem> listOrg = new List<SelectListItem>();
            listOrg.Add(new SelectListItem { Text = "Linfox", Value = "1" });
            listOrg.Add(new SelectListItem { Text = "HWLE", Value = "2" });
            listOrg.Add(new SelectListItem { Text = "Midlaw", Value = "3" });

            List<SelectListItem> listUrs = new List<SelectListItem>();
            listUrs.Add(new SelectListItem { Text = "Barry", Value = "1" });
            listUrs.Add(new SelectListItem { Text = "John", Value = "2" });
            listUrs.Add(new SelectListItem { Text = "Stan", Value = "3" });
            obj.ChannelList = list;
            obj.WaitingOnList = listOrg;
            obj.UserNameList = listUrs;
            obj.TicketList = listUrs;
            return View(obj);
        }

        public IActionResult User()
        {
            User obj = new User();
            List<SelectListItem> listOrg = new List<SelectListItem>();
            listOrg.Add(new SelectListItem { Text = "Linfox", Value = "1" });
            listOrg.Add(new SelectListItem { Text = "HWLE", Value = "2" });
            listOrg.Add(new SelectListItem { Text = "Midlaw", Value = "3" });

            List<SelectListItem> listActive = new List<SelectListItem>();
            listActive.Add(new SelectListItem { Text = "Active", Value = "1" });
            listActive.Add(new SelectListItem { Text = "Deactive", Value = "2" });

            obj.OrganizetionList = listOrg;
            obj.ActiveList = listActive;
            return View(obj);
        }
        public IActionResult Notifications()
        {
            return View();
        }
    }
}
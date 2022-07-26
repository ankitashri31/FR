using FirstResponseApp.Areas.Ticket.Models;
using FirstResponseApp.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Threading;
using System.Threading.Tasks;
using static FirstResponseApp.Models.UtilityHelper.UtilityDto;

namespace FirstResponseApp.Models
{
    public class Utility
    {


        private IWebHostEnvironment _hostingEnvironment;
        private IConfiguration _iconfiguration;
        public static IWebHostEnvironment GlobalHostingEnvironment { get; set; }
        public static IConfiguration GlobalConfigurationEnvironment { get; set; }
        private UserManager<ApplicationUser> userManager;
        public ApplicationDbContext Context { get; }

        public IConfiguration configuration;

        public Utility(ApplicationDbContext applicationDbContext, UserManager<ApplicationUser> UserManager)
        {
            Context = applicationDbContext;
            userManager = UserManager;
        }

        public string DecryptString(string encrString)
        {
            byte[] b;
            string decrypted;
            try
            {
                b = Convert.FromBase64String(encrString);
                decrypted = System.Text.ASCIIEncoding.ASCII.GetString(b);
            }
            catch (FormatException fe)
            {
                decrypted = "";
            }
            return decrypted;
        }
        public string EnryptString(string strEncrypted)
        {
            byte[] b = System.Text.ASCIIEncoding.ASCII.GetBytes(strEncrypted);
            string encrypted = Convert.ToBase64String(b);
            return encrypted;
        }

        #region Sending email to Approver on creation of application.
        public async Task SentConfirmMail(ConfirmEmail model)
        {
            try
            {

                ConfirmEmail ApplicationData = new ConfirmEmail()
                {
                    Id = Convert.ToInt32(model.Id),
                    Title = model.Title,
                    Email_To = model.Email_To,
                    First_Name = model.First_Name,
                    Password = model.Password,
                    Status = model.Status,
                    Last_Name = model.Last_Name,
                    Url = model.Url,
                    EmailKey = model.EmailKey
                };
                string UserFullName = model.First_Name + " " + model.Last_Name;
                MailMessage Message = new MailMessage();
                var MailBody = await GetMailDataFromDb(ApplicationData.EmailKey);
                string MailContent = MailBody.EmailContent;
                MailContent = MailContent.Replace("{EMAIL_TITLE}", ApplicationData.Title);
                MailContent = MailContent.Replace("{EMAIL}", ApplicationData.Email_To);
                MailContent = MailContent.Replace("{UserFullName}", UserFullName);
                MailContent = MailContent.Replace("{Emailaddress}", ApplicationData.Email_To);
                MailContent = MailContent.Replace("{Password}", ApplicationData.Password);
                MailContent = MailContent.Replace("{Status}", ApplicationData.Status);
                MailContent = MailContent.Replace("{LINK}", ApplicationData.Url);
                MailContent = MailContent.Replace("{BANNER_IMAGE}", ApplicationData.Url + "/Images/Banner.jpg");
                MailContent = MailContent.Replace("{TOP_IMAGE}", ApplicationData.Url + "/Images/topimage.png");
                MailContent = MailContent.Replace("{BOTTOM_IMAGE}", ApplicationData.Url + "/Images/bottomimage.png");


                Message.Subject = ApplicationData.Title;
                Message.Body = MailContent;
                Message.IsBodyHtml = true;
                var result = SendEmail(Message, ApplicationData.Email_To);
            }
            catch (Exception ex)
            {
            }
        }

        public async Task SentBulkMail(ConfirmEmail model)
        {
            try
            {

                ConfirmEmail ApplicationData = new ConfirmEmail()
                {
                    Id = Convert.ToInt32(model.Id),
                    Title = model.Title,
                    First_Name = model.First_Name,
                    Password = model.Password,
                    Status = model.Status,
                    Last_Name = model.Last_Name,
                    Url = model.Url,
                    EmailKey = model.EmailKey,
                    UserEmailList = model.UserEmailList,
                    TicketName = model.TicketName,
                    OrganisationName = model.OrganisationName,
                    TicketId = model.TicketId,
                    UpdatedByName = model.UpdatedByName,
                    LoggedInOrganisation = model.LoggedInOrganisation,
                    WebsitePath = model.WebsitePath
                };
                MailMessage Message = new MailMessage();
                var MailBody = await GetMailDataFromDb(ApplicationData.EmailKey);
                string MailContent = MailBody.EmailContent;
                MailContent = MailContent.Replace("{EMAIL_TITLE}", ApplicationData.Title);
                MailContent = MailContent.Replace("{OrganisationName}", ApplicationData.OrganisationName);
                MailContent = MailContent.Replace("{TicketName}", ApplicationData.TicketName);
                MailContent = MailContent.Replace("{Id}", Convert.ToString(ApplicationData.Id));
                MailContent = MailContent.Replace("{LINK}", ApplicationData.Url);
                MailContent = MailContent.Replace("{BANNER_IMAGE}", ApplicationData.WebsitePath + "/Images/Banner.jpg");
                MailContent = MailContent.Replace("{TOP_IMAGE}", ApplicationData.WebsitePath + "/Images/topimage.png");
                MailContent = MailContent.Replace("{BOTTOM_IMAGE}", ApplicationData.WebsitePath + "/Images/bottomimage.png");
                MailContent = MailContent.Replace("{TicketId}", ApplicationData.TicketId.ToString());
                MailContent = MailContent.Replace("{UpdatedByUserName}", ApplicationData.UpdatedByName);
                MailContent = MailContent.Replace("{LoggedInOrganisation}", ApplicationData.LoggedInOrganisation);
                string mc = MailContent;
                var MailList = ApplicationData.UserEmailList.AsEnumerable();
                smtpSetting SMTP_Settings = await GetSmtpSettings();

                Task.Run(() => SendBulkEmailInTaskLoop(MailList, ApplicationData.Title, mc, SMTP_Settings));

                //Parallel.ForEach(mailList, async row =>

                //{
                //   await  SendBulkEmail(row, ApplicationData.Title, mc, SMTP_Settings);
                //});

            }
            catch (Exception ex)
            {
            }
        }

        private async Task<bool> SendBulkEmail(string recipient, string subject, string body, smtpSetting SMTP_Settings)
        {

            // smtpSetting SMTP_Settings = await GetSmtpSettings();

            MailMessage mm = new MailMessage(SMTP_Settings.DefaultFromAddress, recipient);
            mm.Subject = subject;
            mm.Body = body;
            mm.IsBodyHtml = true;
            SmtpClient smtp = new SmtpClient();
            smtp.Host = SMTP_Settings.SmtpHost;
            // smtp.EnableSsl = true;
            if (string.IsNullOrEmpty(SMTP_Settings.SmtpEnableSsl))
            {
                smtp.EnableSsl = false;
            }
            else
            {
                smtp.EnableSsl = SMTP_Settings.SmtpEnableSsl.ToLower() == "true" ? true : false;
            }
            NetworkCredential NetworkCred = new NetworkCredential();
            NetworkCred.UserName = SMTP_Settings.SmtpUserName;
            NetworkCred.Password = SMTP_Settings.SmtpPassword;
            smtp.UseDefaultCredentials = true;
            smtp.Credentials = NetworkCred;
            
            smtp.Port = Convert.ToInt16(SMTP_Settings.SmtpPort);

            await smtp.SendMailAsync(mm);
            Thread.Sleep(2000);
            return true;
        }
        #endregion

        private async Task<bool> SendBulkEmailInTaskLoop(IEnumerable<UserEmailValues> emails, string subject, string body, smtpSetting SMTP_Settings)
        {
            string OriginalEmailContent = body;
            // smtpSetting SMTP_Settings = await GetSmtpSettings();
            foreach (var email in emails)
            {
                try
                {
                    // string EmailBody = body;
                    body = body.Replace("{UserFullName}", email.FullName);
                    MailMessage mm = new MailMessage(SMTP_Settings.DefaultFromAddress, email.EmailAddress);
                    mm.Subject = subject;
                    mm.Body = body;
                    mm.IsBodyHtml = true;
                    SmtpClient smtp = new SmtpClient();
                    smtp.Host = SMTP_Settings.SmtpHost;
                    NetworkCredential NetworkCred = new NetworkCredential();
                    NetworkCred.UserName = SMTP_Settings.SmtpUserName;
                    if (string.IsNullOrEmpty(SMTP_Settings.SmtpPassword))
                    {
                        NetworkCred.Password = "";
                    }
                    else
                    {
                        NetworkCred.Password = SMTP_Settings.SmtpPassword;
                    }
                    smtp.UseDefaultCredentials = true;
                    smtp.Credentials = NetworkCred;

                    if (string.IsNullOrEmpty(SMTP_Settings.SmtpEnableSsl))
                    {
                        smtp.EnableSsl = false;
                    }
                    else
                    {
                        smtp.EnableSsl = SMTP_Settings.SmtpEnableSsl.ToLower() == "true" ? true : false;
                    }

                    smtp.Port = Convert.ToInt16(SMTP_Settings.SmtpPort);
                  
                    await smtp.SendMailAsync(mm);
                    body = OriginalEmailContent;
                }
                catch (Exception ex)
                {

                }

            }
            return true;
        }




        #region sending mail to verefier
        public async Task<bool> SendEmail(MailMessage MailMessage, string Email_To)
        {
            try
            {
                smtpSetting SMTP_Settings = await GetSmtpSettings();

                MailMessage Message = new MailMessage();
                // smtpSetting SMTP_Settings = await _PrimaryUserManagerService.GetSmtpSettings();
                SmtpClient emailClient = new SmtpClient(SMTP_Settings.SmtpHost);
                if (SMTP_Settings != null)
                {
                    Message.To.Add(Email_To);
                    Message.From = new MailAddress(SMTP_Settings.SmtpUserName);
                }

                Message.Subject = MailMessage.Subject;
                Message.Body = MailMessage.Body;

                Message.IsBodyHtml = true;

                emailClient.Port = Convert.ToInt16(SMTP_Settings.SmtpPort);
                if (string.IsNullOrEmpty(SMTP_Settings.SmtpPassword))
                {
                    emailClient.Credentials = new System.Net.NetworkCredential(SMTP_Settings.SmtpUserName, "");
                }
                else
                {
                    emailClient.Credentials = new System.Net.NetworkCredential(SMTP_Settings.SmtpUserName, SMTP_Settings.SmtpPassword);
                }

                if (string.IsNullOrEmpty(SMTP_Settings.SmtpEnableSsl))
                {
                    emailClient.EnableSsl = false;
                }
                else
                {
                    emailClient.EnableSsl = SMTP_Settings.SmtpEnableSsl.ToLower() == "true" ? true : false;
                }
                //Convert.ToBoolean(SMTP_Settings.SmtpEnableSsl);

                emailClient.Send(Message);
                return true;
            }
            catch (Exception ex)
            {
                //  Logger.LogError(ex);
                return false;
            }
        }



        #endregion

        public async Task<EmailTemplates> GetMailDataFromDb(string MailKey)
        {

            var tem = Context.EmailTemplates.ToList();
            return Context.EmailTemplates.FirstOrDefault(x => x.EmailTitle.ToLower().Trim() == MailKey.ToLower().Trim());
        }

        public async Task<smtpSetting> GetSmtpSettings()
        {
            SmtpSetting sm = Context.SmtpSetting.FirstOrDefault();
            smtpSetting smm = new smtpSetting()
            {
                DefaultFromAddress = sm.DefaultFromAddress,
                DefaultFromDisplayName = sm.DefaultFromDisplayName,
                SmtpEnableSsl = sm.SmtpEnableSsl,
                SmtpHost = sm.SmtpHost,
                SmtpPort = sm.SmtpPort,
                SmtpPassword = sm.SmtpPassword,
                SmtpUserName = sm.SmtpUserName
            };
            return smm;
        }


        #region Generate Password
        public string GeneratePassword(int lowercase, int uppercase, int numerics, int specialCase)
        {
            string generated = "!";
            try
            {
                string lowers = "abcdefghijklmnopqrstuvwxyz";
                string uppers = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
                string number = "0123456789";
                string special = "@#$%&*??";

                Random random = new Random();


                for (int i = 1; i <= lowercase; i++)
                {
                    generated = generated.Insert(
                        random.Next(generated.Length),
                        lowers[random.Next(lowers.Length - 1)].ToString());
                }
                for (int i = 1; i <= uppercase; i++)
                {
                    generated = generated.Insert(
                        random.Next(generated.Length),
                        uppers[random.Next(uppers.Length - 1)].ToString());
                }
                for (int i = 1; i <= numerics; i++)
                {
                    generated = generated.Insert(
                        random.Next(generated.Length),
                        number[random.Next(number.Length - 1)].ToString());
                }
                for (int i = 1; i <= specialCase; i++)
                {
                    generated = generated.Insert(
                        random.Next(generated.Length),
                        special[random.Next(special.Length - 1)].ToString());
                }
            }
            catch (Exception ex)
            {

            }
            return generated.Replace("!", string.Empty);

        }
        #endregion

        #region Audit log generation

        public void AuditHistoryEntry(string logMessage, string actionUrl, int? ticketId, string loggedIdUserId)
        {

            try
            {
                // int UserId = SessionUser.UserId;
                string UserMail = string.Empty;

                TbAuditHistoryMaster historyLog = new TbAuditHistoryMaster();
                //historyLog.AuditEventTypeId = EventId;
                historyLog.UserMasterId = loggedIdUserId;
                historyLog.UserEmailAddress = UserMail;
                historyLog.RecordedDate = System.DateTime.Now;
                historyLog.AuditEventDescription = logMessage;
                historyLog.AuditEventName = actionUrl;
                historyLog.TicketMasterId = ticketId;

                Context.TbAuditHistoryMaster.Add(historyLog);
                Context.SaveChanges();
                long Id = historyLog.Id;
            }
            catch (Exception ex) { }
        }


        #endregion

        public void CreateErrorLogs(Exception exe, string Path)
        {
            try
            {
                var HostingEnv = GlobalHostingEnvironment;
                var fileSavePath = HostingEnv.ContentRootPath + "\\ErrorLogs.txt";
                //Creating the path where pdf format of forms will be save.
                DirectoryInfo dirInfo = new DirectoryInfo(fileSavePath);

                // check if file exists and date is same then write error log in same file
                if (File.Exists(fileSavePath))
                {
                    try
                    {
                        var Configuration = GlobalConfigurationEnvironment;
                        if (Configuration != null)
                        {
                            var IsDeleted = Configuration["IsDeleteADUserDatatxtContent"];

                            if (!string.IsNullOrEmpty(IsDeleted) || IsDeleted != null)
                            {
                                if (IsDeleted.ToLower() == "true")
                                {
                                    // clear text file
                                    File.WriteAllText(fileSavePath, "");
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {

                    }
                    //write to same file
                    using (StreamWriter SW = File.AppendText(fileSavePath))
                    {
                        try
                        {
                            SW.WriteLine(DateTime.Now + " ------------------------------------------------------------------------------------------------------------------");

                            SW.WriteLine(exe.Message);
                            if (!string.IsNullOrEmpty(Path))
                            {
                                SW.WriteLine(Path);
                            }
                            //SW.Close();
                        }
                        catch (Exception ex)
                        {

                        }
                        finally
                        {
                            if (SW != null)
                            {
                                SW.Flush();
                                SW.Close();
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {

            }
        }

    }
}

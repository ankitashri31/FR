using FirstResponseApp.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Threading.Tasks;
using static FirstResponseApp.Models.UtilityHelper.UtilityDto;

namespace FirstResponseApp.Models.UtilityHelper
{
    public class Utility
    {
        ApplicationDbContext Context = new ApplicationDbContext();

        private IWebHostEnvironment _hostingEnvironment;
        private IConfiguration _iconfiguration;
        public static IWebHostEnvironment GlobalHostingEnvironment2 { get; set; }
        public static IConfiguration GlobalConfigurationEnvironment2 { get; set; }
        public void AuditHistoryEntry(string logMessage, string actionUrl, int? ticketMasterId, string loggedIdUserId)
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
                historyLog.TicketMasterId = ticketMasterId;
                Context.TbAuditHistoryMaster.Add(historyLog);
                Context.SaveChanges();
                long Id = historyLog.Id;
            }
            catch (Exception ex) { }
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
            //TbUserMaster Sender_Mail = null;
            //if (model.ApplicationCreatorId.HasValue && model.ApplicationCreatorId.Value != 0)
            //{
            //    Sender_Mail = await _PrimaryUserManagerService.GetUserMasterDataById(model.ApplicationCreatorId.Value);
            //}
            //else
            //{
            //    Sender_Mail = await _PrimaryUserManagerService.GetUserMasterDataById(model.Id);
            //}
            ////var Sender_Mail = await _PrimaryUserManagerService.GetUserMasterDataById(model.Id);
            //try
            //{
            //    if (Sender_Mail != null)
            //    {
            //        ConfirmEmail ApplicationData = new ConfirmEmail()
            //        {
            //            Id = Convert.ToInt32(model.Id),
            //            Title = model.Title,
            //            //Email_From =  //"siplInboundioa@gmail.com",
            //            Email_To = Sender_Mail.EmailAddress,
            //            First_Name = model.First_Name,
            //            Last_Name = model.Last_Name,
            //            Url = model.Url,
            //            ApproverName = Sender_Mail.FirstName + " " + Sender_Mail.LastName,
            //            EmailKey = model.EmailKey
            //        };


            //        string UserFullName = model.First_Name + " " + model.Last_Name;
            //        if (string.IsNullOrEmpty(UserFullName.Trim()))
            //        {
            //            UserFullName = "User Details not supplied!";
            //        }

            //        MailMessage Message = new MailMessage();
            //        var MailBody = await _PrimaryUserManagerService.GetMailDataFromDb(ApplicationData.EmailKey);
            //        string MailContent = MailBody.EmailContent;
            //        MailContent = MailContent.Replace("{EMAIL_TITLE}", ApplicationData.Title);
            //        MailContent = MailContent.Replace("{Approver_NAME}", ApplicationData.ApproverName);
            //        MailContent = MailContent.Replace("{EMAIL}", ApplicationData.Email_To);
            //        MailContent = MailContent.Replace("{UserFullName}", UserFullName);
            //        MailContent = MailContent.Replace("{LINK}", ApplicationData.Url);
            //        MailContent = MailContent.Replace("{BANNER_IMAGE}", ApplicationData.Url + "/Images/Banner.jpg");
            //        MailContent = MailContent.Replace("{TOP_IMAGE}", ApplicationData.Url + "/Images/topimage.png");
            //        MailContent = MailContent.Replace("{BOTTOM_IMAGE}", ApplicationData.Url + "/Images/bottomimage.png");


            //        Message.Subject = ApplicationData.Title;
            //        Message.Body = MailContent;
            //        Message.IsBodyHtml = true;
            //        var result = SendEmail(Message, ApplicationData.Email_To);
            //    }
            //}
            //catch (Exception ex)
            //{
            //}
        }
        #endregion

        #region sending mail to verefier
        public async Task<bool> SendEmail(MailMessage MailMessage, string Email_To)
        {
            try
            {
                MailMessage Message = new MailMessage();
                //smtpSetting SMTP_Settings =  await _PrimaryUserManagerService.GetSmtpSettings();
                //SmtpClient emailClient = new SmtpClient(SMTP_Settings.SmtpHost);
                //if (SMTP_Settings != null)
                //{
                //    Message.To.Add(Email_To);
                //    Message.From = new MailAddress(SMTP_Settings.SmtpUserName);
                //}

                //Message.Subject = MailMessage.Subject;
                //Message.Body = MailMessage.Body;

                //Message.IsBodyHtml = true;

                //emailClient.Port = Convert.ToInt16(SMTP_Settings.SmtpPort);
                //if (string.IsNullOrEmpty(SMTP_Settings.SmtpPassword))
                //{
                //    emailClient.Credentials = new System.Net.NetworkCredential(SMTP_Settings.SmtpUserName, "");
                //}
                //else
                //{
                //    emailClient.Credentials = new System.Net.NetworkCredential(SMTP_Settings.SmtpUserName, SMTP_Settings.SmtpPassword);
                //}

                //if (string.IsNullOrEmpty(SMTP_Settings.SmtpEnableSsl))
                //{
                //    emailClient.EnableSsl = false;
                //}
                //else
                //{
                //    emailClient.EnableSsl = SMTP_Settings.SmtpEnableSsl.ToLower() == "true" ? true : false;
                //}
                ////Convert.ToBoolean(SMTP_Settings.SmtpEnableSsl);

                //emailClient.Send(Message);
                return true;
            }
            catch (Exception ex)
            {
                //  Logger.LogError(ex);
                return false;
            }
        }
        #endregion

        #region write logs data
        public void CreateLogs(List<UsersData> UserList)
        {
            try
            {
                var HostingEnv = GlobalHostingEnvironment2;
                var fileSavePath = HostingEnv.ContentRootPath + "\\ErrorLogs.txt";
                //Creating the path where pdf format of forms will be save.
                DirectoryInfo dirInfo = new DirectoryInfo(fileSavePath);

                // check if file exists and date is same then write error log in same file
                if (File.Exists(fileSavePath))
                {

                    try
                    {
                        var Configuration = GlobalConfigurationEnvironment2;
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
                            SW.WriteLine(DateTime.Now + " Count: " + UserList.Count() + " ------------------------------------------------------------------------------------------------------------------");

                            // SW.Close();

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


        public void CreateErrorLogs(Exception exe, string Path)
        {
            try
            {
                var HostingEnv = GlobalHostingEnvironment2;
                var fileSavePath = HostingEnv.ContentRootPath + "\\ErrorLogs.txt";
                //Creating the path where pdf format of forms will be save.
                DirectoryInfo dirInfo = new DirectoryInfo(fileSavePath);

                // check if file exists and date is same then write error log in same file
                if (File.Exists(fileSavePath))
                {
                    try
                    {
                        var Configuration = GlobalConfigurationEnvironment2;
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
        #endregion
    }
}

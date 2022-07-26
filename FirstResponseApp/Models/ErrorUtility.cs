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
    public class ErrorUtility
    {


        private IWebHostEnvironment _hostingEnvironment;
        private IConfiguration _iconfiguration;
        public static IWebHostEnvironment GlobalHostingEnvironmentError  { get; set; }
        public static IConfiguration GlobalConfigurationEnvironmentError { get; set; }
        private UserManager<ApplicationUser> userManager;
        public ApplicationDbContext Context { get; }

        public IConfiguration configuration;

        public void CreateErrorLogs(Exception exe, string Path, string StackTrace)
        {
            try
            {
                var HostingEnv = GlobalHostingEnvironmentError;
                var fileSavePath = HostingEnv.ContentRootPath + "\\ErrorLogs.txt";
                //Creating the path where pdf format of forms will be save.
                DirectoryInfo dirInfo = new DirectoryInfo(fileSavePath);

                // check if file exists and date is same then write error log in same file
                if (File.Exists(fileSavePath))
                {
                    try
                    {
                        var Configuration = GlobalConfigurationEnvironmentError;
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
                            SW.WriteLine("StackTrace : "+ StackTrace);
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

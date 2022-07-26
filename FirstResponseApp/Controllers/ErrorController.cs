using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Diagnostics;
using FirstResponseApp.Models;
using Microsoft.AspNetCore.Identity;
using FirstResponseApp.Data;
using System.IO;
using Microsoft.Extensions.Configuration;

namespace FirstResponseApp.Controllers
{
    public class ErrorController : Controller
    {
        private readonly IHttpContextAccessor _HttpContextAccessor;
        private UserManager<ApplicationUser> _userManager;
        public ApplicationDbContext _Context { get; set; }
        public static IConfiguration GlobalConfigurationEnv { get; set; }

        public ErrorController(IHttpContextAccessor HttpContextAccessor, UserManager<ApplicationUser> userManager, ApplicationDbContext Context)
        {
            _HttpContextAccessor = HttpContextAccessor;
            _userManager = userManager;
            _Context = Context;

        }

        ErrorUtility UtilityModel = new ErrorUtility();

        public IActionResult Index()
        {
            return View();
        }
        [Route("Error")]
        public async Task<IActionResult> HTTPStatusCodeHandler()
        {
            int ErrorCode = 0;
            string Path = string.Empty;
            var feature = _HttpContextAccessor.HttpContext.Features.Get<IExceptionHandlerFeature>();


            if (feature != null)
            {
                Path = feature.GetType().GetProperties()[1].GetValue(feature, null).ToString();
            }
            ErrorCode = _HttpContextAccessor.HttpContext.Response.StatusCode;
            if (ErrorCode == 0)
            {
                ErrorCode = 404;
            }

            ErrorData ErrorModel = new ErrorData();
            try
            {
                switch (ErrorCode)
                {
                    case 404:
                        ErrorModel.ErrorCode = 404;
                        ErrorModel.ErrorMessage = "Sorry, the resource you requested could not be found!";
                        //ErrorModel.Error = feature.Error;
                        try
                        {
                            UtilityModel.CreateErrorLogs(feature.Error, Path, feature.Error.StackTrace);
                        }
                        catch (Exception ex)
                        {

                        }
                        return RedirectToAction("E404", "Error");
                    case 500:

                        ErrorModel.ErrorCode = 500;
                        ErrorModel.ErrorMessage = "Sorry, the resource you requested could not be found!";
                        try
                        {
                            UtilityModel.CreateErrorLogs(feature.Error, Path, feature.Error.StackTrace);
                        }
                        catch (Exception ex)
                        {

                        }
                        return RedirectToAction("E500", "Error");
                }
            }
            catch (Exception ex)
            {

            }
            return View("E404");
        }

        [HttpGet]
        [Route("Error/E404")]
        public ActionResult E404(ErrorData ErrorModel)
        {
            ViewBag.ErrorMessage = "Sorry, the resource you requested could not be found!";
            ViewBag.ErrorCode = "404 : Page Not Found";
            return View();
        }

        [HttpGet]
        [Route("Error/E500")]
        public ActionResult E500(ErrorData ErrorModel)
        {
            ViewBag.ErrorMessage = "Sorry! something went wrong, please try again.";
            ViewBag.ErrorCode = "500";
            return View();
        }


        [HttpGet]
        [Route("Error/E403")]
        public ActionResult E403(ErrorData ErrorModel)
        {
            ViewBag.ErrorMessage = "Sorry! you are not authorize to access this page.";
            ViewBag.ErrorCode = "403";
            return View();
        }
    }



    public class ErrorData
    {
        public int ErrorCode { get; set; }
        public string ErrorMessage { get; set; }
        public Exception Error { get; set; }
    }
}
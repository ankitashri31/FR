using FirstResponseApp.Areas.Ticket.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FirstResponseApp.Models
{

    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true, Inherited = true)]
    public class AuthenticateLinfoxUser : Attribute, IAuthorizationFilter
    {
        private readonly AppService _appService;
        //private readonly IHttpContextAccessor _httpContextAccessor;
        //private ISession _session => _httpContextAccessor.HttpContext.Session;
        //public AuthenticateLinfoxUser(AppService appService)
        //{
        //    _appService = appService;

        //}
        public void OnAuthorization(AuthorizationFilterContext filterContext)
        {

            HandleUnauthorizedRequest(filterContext);

        }


        protected void HandleUnauthorizedRequest(AuthorizationFilterContext filterContext)
        {
            UserSessionDetail temSession = new UserSessionDetail();
            try
            {
                temSession = JsonConvert.DeserializeObject<UserSessionDetail>(filterContext.HttpContext.Session.GetString("UserSession")); //filterContext.HttpContext.Session.GetString("OrganizationId");
              
            }
            catch(Exception ex)
            {
                filterContext.Result = new RedirectResult("~/identity/account/login");
            }

            int OrganisationId = Convert.ToInt32(temSession.OrganizationId);
            bool IsActive = temSession.IsActive;
            if (temSession == null)
            {
                bool isAjaxCall = filterContext.HttpContext.Request.Headers["x-requested-with"] == "XMLHttpRequest";
                if (isAjaxCall)
                {
                    var statusCodeObj = new System.Web.Mvc.HttpStatusCodeResult(460, "SessionTimeout");

                    filterContext.HttpContext.Response.StatusCode = statusCodeObj.StatusCode;
                    filterContext.Result = new JsonResult(new
                    {
                        Error = statusCodeObj.StatusDescription
                    });

                    UserSessionDetail UserInfo = new UserSessionDetail();
                    filterContext.HttpContext.Session.SetString("UserSession", JsonConvert.SerializeObject(UserInfo));

                }
                else
                {
                   
                    filterContext.Result = new RedirectResult("~/identity/account/login");
                }

            }
            else
            {
                try
                {
                    if (IsActive == false)
                    {
                        UserSessionDetail UserInfo = new UserSessionDetail();
                        filterContext.HttpContext.Session.SetString("UserSession", JsonConvert.SerializeObject(UserInfo));
                        filterContext.Result = new RedirectResult("~/identity/account/login");
                    }
                    else
                    {
                        if (OrganisationId == 3)
                        {
                            // valuid user

                        }
                        else if (OrganisationId == 0)
                        {
                            filterContext.Result = new RedirectResult("~/identity/account/login");
                        }
                        else
                        {
                            bool isAjaxCall = filterContext.HttpContext.Request.Headers["x-requested-with"] == "XMLHttpRequest";
                            if (isAjaxCall)
                            {
                                var statusCodeObj = new System.Web.Mvc.HttpStatusCodeResult(467, "NotAuthorized");

                                filterContext.HttpContext.Response.StatusCode = statusCodeObj.StatusCode;
                                filterContext.Result = new JsonResult(new
                                {
                                    Error = statusCodeObj.StatusDescription
                                });

                                UserSessionDetail UserInfo = new UserSessionDetail();
                                filterContext.HttpContext.Session.SetString("UserSession", JsonConvert.SerializeObject(UserInfo));

                            }
                            else
                            {
                                //filterContext.Result = new RedirectToActionResult("E403", "Error", new { area = "", ErrorMessage = "User is not authenticated or session has timed out." });
                                filterContext.Result = new RedirectResult("~/error/E403");
                                //filterContext.Result = new RedirectToActionResult("Logout", "account", new { area = "Identity", ErrorMessage = "User not authorized to access this page." });
                            }
                        }
                    }
                }
                catch(Exception ex)
                {
                    filterContext.Result = new RedirectResult("~/identity/account/login");
                }

              }
           }
    } 
}

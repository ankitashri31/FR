using FirstResponseApp.Areas.Ticket.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
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
    public class AuthenticateAllUsers : Attribute, IAuthorizationFilter
    {
        private readonly AppService _appService;
        //private readonly IHttpContextAccessor _httpContextAccessor;
        //private ISession _session => _httpContextAccessor.HttpContext.Session;
        //public AuthenticateAllUsers(AppService appService)
        //{
        //    _appService = appService;

        //}
        public void OnAuthorization(AuthorizationFilterContext filterContext)
        {

            HandleUnauthorizedRequest(filterContext);

        }


        protected void  HandleUnauthorizedRequest(AuthorizationFilterContext filterContext)
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
            bool IsActive = temSession.IsActive;
            int OrganisationId = Convert.ToInt32(temSession.OrganizationId);
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
                    UserSessionDetail UserInfo = new UserSessionDetail();
                    filterContext.HttpContext.Session.SetString("UserSession", JsonConvert.SerializeObject(UserInfo));
                    filterContext.Result = new RedirectResult("~/identity/account/login");               
                }
            }
            else
            {
                try
                {
                    if (IsActive == false)
                    {

                        filterContext.Result = new RedirectResult("~/identity/account/login");
                    }
                    else
                    {

                        if (OrganisationId == 1 || OrganisationId == 2 || OrganisationId == 3)
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

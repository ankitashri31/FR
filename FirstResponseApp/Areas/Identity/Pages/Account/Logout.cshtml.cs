using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using FirstResponseApp.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;

namespace FirstResponseApp.Areas.Identity.Pages.Account
{
    [AllowAnonymous]
    public class LogoutModel : PageModel
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ILogger<LogoutModel> _logger;

        public LogoutModel(SignInManager<ApplicationUser> signInManager, ILogger<LogoutModel> logger)
        {
            _signInManager = signInManager;
            _logger = logger;
        }

        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPost(string returnUrl = null)
        {
            await _signInManager.SignOutAsync();
            _logger.LogInformation("User logged out.");
            UserSessionDetail UserInfo = new UserSessionDetail();
          
            if (returnUrl != null)
            {
                HttpContext.Session.SetString("UserSession", JsonConvert.SerializeObject(UserInfo));
                return RedirectToAction("Login", "Account", new { area = "Identity" });
            }
            else
            {

                HttpContext.Session.SetString("UserSession", JsonConvert.SerializeObject(UserInfo));
                return RedirectToAction("Login", "Account", new { area = "Identity" });
            }
        }
    }
}

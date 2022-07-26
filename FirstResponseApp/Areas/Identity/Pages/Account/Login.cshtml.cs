using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using FirstResponseApp.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using FirstResponseApp.Areas.Ticket.Models;

namespace FirstResponseApp.Areas.Identity.Pages.Account
{
    [AllowAnonymous]
    public class LoginModel : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ILogger<LoginModel> _logger;
        private readonly IEmailSender _emailSender;
        private readonly IConfiguration _iconfiguration;
        private readonly AppService _appService;

        public LoginModel(SignInManager<ApplicationUser> signInManager, 
            ILogger<LoginModel> logger,
            UserManager<ApplicationUser> userManager,
            IEmailSender emailSender,
            IConfiguration iconfiguration, AppService appService)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _emailSender = emailSender;
            _logger = logger;
            this._iconfiguration = iconfiguration;
            _appService = appService;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public IList<AuthenticationScheme> ExternalLogins { get; set; }

        public string ReturnUrl { get; set; }

        [TempData]
        public string ErrorMessage { get; set; }

        public class InputModel
        {
            [Required]
            [EmailAddress]
            public string Email { get; set; }

            [Required]
            [DataType(DataType.Password)]
            public string Password { get; set; }

            [Display(Name = "Remember me?")]
            public bool RememberMe { get; set; }
        }

        public async Task OnGetAsync(string returnUrl = null)
        {
            //-----------For showing Forget password Popup ------------
            if (!string.IsNullOrEmpty((string)TempData["Success"]))
            {
                string strCheck = (string)TempData["Success"];
                if (strCheck == "PasswordReset")
                {
                    ViewData["Message"] = "Password has been reset successfully";
                }
                else {
                    ViewData["Message"] = "Password reset link has been sent successfully on your registered email id";
                }
                ViewData["Success"] = (string)TempData["Success"];
            }


            if (!string.IsNullOrEmpty(ErrorMessage))
            {
                ModelState.AddModelError(string.Empty, ErrorMessage);
            }

            returnUrl = returnUrl ?? Url.Content("~/");

            // Clear the existing external cookie to ensure a clean login process
            await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);

            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();

            ReturnUrl = returnUrl;
        }

        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            returnUrl = returnUrl ?? Url.Content("~/");

            if (ModelState.IsValid)
            {
                // This doesn't count login failures towards account lockout
                // To enable password failures to trigger account lockout, set lockoutOnFailure: true
                var result = await _signInManager.PasswordSignInAsync(Input.Email, Input.Password, Input.RememberMe, false /*lockoutOnFailure: true*/);
                if (result.Succeeded)
                {

                    var user = await _userManager.FindByEmailAsync(Input.Email);

                    if (user.IsActive)
                    {
                        var GetOrganisationData = await _appService.GetOrganisationList();
                        string OrganisationName = GetOrganisationData.Where(i => i.Id == user.OrganisationId).Select(i => i.OrganisationName).FirstOrDefault();
                        //-------------Set Session---Start-------
                        var userInfo = new UserSessionDetail() { Id = user.Id, EmailId = user.Email, IsActive = user.IsActive, Name = user.Name + " " + user.LastName, OrganizationId = user.OrganisationId.ToString(), OrganisationName = OrganisationName };
                        HttpContext.Session.SetString("UserSession", JsonConvert.SerializeObject(userInfo));
                        //-------------Set Session----End------
                        user = user;
                        user.LastLogOnDateTime = DateTime.UtcNow;  //-----------Set Last Login Time
                        var lastLogin = await _userManager.UpdateAsync(user);


                        if (user.IsFirstTimeChangedPassword == false)
                        {
                            return RedirectToAction("ResetPassword", "Account", new { area = "Identity" });
                        }

                        if (user.IsActive == false)
                        {
                            ViewData["Errore"] = "User has been deactivated by admin.";
                            ModelState.AddModelError(string.Empty, "User is deactivated by the Admin.");
                            return Page();
                        }

                        _logger.LogInformation("User logged in.");
                        return RedirectToAction("Dashboard", "TicketUser", new { area = "Ticket" });
                    }
                    else {
                        ViewData["Errore"] = "This account has been deactivated by the admin.";
                        ModelState.AddModelError(string.Empty, "This account has been deactivated by the admin.");
                        return Page();
                    }
                }
                if (result.RequiresTwoFactor)
                {
                   // return RedirectToPage("./LoginWith2fa", new { ReturnUrl = returnUrl, RememberMe = Input.RememberMe });
                }
               
                if (result.IsLockedOut)
                {
                    
                    //_logger.LogWarning("User account locked out.");
                    //return RedirectToPage("./Lockout");
                }
                else
                {
                    ViewData["Errore"] = "The email address or password is not valid, please try again."; 
                    ModelState.AddModelError(string.Empty, "The email address or password is not valid, please try again.");
                    return Page();
                }
            }

            // If we got this far, something failed, redisplay form
            return Page();
        }

        public async Task<IActionResult> OnPostSendVerificationEmailAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var user = await _userManager.FindByEmailAsync(Input.Email);
            if (user == null)
            {
                ModelState.AddModelError(string.Empty, "Verification email sent. Please check your email.");
            }

            var userId = await _userManager.GetUserIdAsync(user);
            var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            var callbackUrl = Url.Page(
                "/Account/ConfirmEmail",
                pageHandler: null,
                values: new { userId = userId, code = code },
                protocol: Request.Scheme);
            await _emailSender.SendEmailAsync(
                Input.Email,
                "Confirm your email",
                $"Please confirm your account by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.");

            ModelState.AddModelError(string.Empty, "Verification email sent. Please check your email.");
            return Page();
        }
    }
}

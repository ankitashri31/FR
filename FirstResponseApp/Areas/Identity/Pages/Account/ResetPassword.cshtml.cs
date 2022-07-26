using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using FirstResponseApp.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;

namespace FirstResponseApp.Areas.Identity.Pages.Account
{
    [AllowAnonymous]
    public class ResetPasswordModel : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public ResetPasswordModel(UserManager<ApplicationUser> userManager, IHttpContextAccessor httpContextAccessor)
        {
            _userManager = userManager;
            _httpContextAccessor = httpContextAccessor;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public class InputModel
        {
            
            [EmailAddress]
            public string Email { get; set; }

            [Required]
            [Display(Name = "Old password")]
            [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
            [DataType(DataType.Password)]
            public string OldPassword { get; set; }

            [Required]
            [Display(Name = "New password")]
            [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 8)]
            [DataType(DataType.Password)]
            [RegularExpression("^((?=.*?[A-Z])(?=.*?[a-z])(?=.*?[0-9])|(?=.*?[A-Z])(?=.*?[a-z])(?=.*?[^a-zA-Z0-9])|(?=.*?[A-Z])(?=.*?[0-9])(?=.*?[^a-zA-Z0-9])|(?=.*?[a-z])(?=.*?[0-9])(?=.*?[^a-zA-Z0-9])).{8,}$", ErrorMessage = "Passwords must be at least 8 characters and contain at 3 of 4 of the following: upper case (A-Z), lower case (a-z), number (0-9) and special character (e.g. !@#$%^&*)")]
            public string Password { get; set; }

            [Required]
            [DataType(DataType.Password)]
            [Display(Name = "Confirm password")]
            [Compare("Password", ErrorMessage = "The new password and confirm password do not match.")]
            public string ConfirmPassword { get; set; }

            public string Code { get; set; }
        }

        public IActionResult OnGet()
        {
            UserSessionDetail UserInfo = new UserSessionDetail();
            //try
            //{
            //    UserInfo = JsonConvert.DeserializeObject<UserSessionDetail>(filterContext.HttpContext.Session.GetString("UserSession")); //filterContext.HttpContext.Session.GetString("OrganizationId");

            //}
            //catch (Exception ex)
            //{
            //    filterContext.Result = new RedirectResult("~/identity/account/login");
            //}

            try
            {
                UserInfo = JsonConvert.DeserializeObject<UserSessionDetail>(HttpContext.Session.GetString("UserSession"));
            }
            catch(Exception ex)
            {
                return Redirect("~/identity/account/login");
            }
            string code = null;
            code = UserInfo.Id;
            if (code == null)
            {
                return BadRequest("A code must be supplied for password reset.");
            }
            else
            {
                Input = new InputModel
                {
                    Code = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(code))
                };
                
                return Page();
            }
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            // var user = await _userManager.FindByEmailAsync(Input.Email);
            var userId = _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                // Don't reveal that the user does not exist
                return RedirectToPage("./ResetPasswordConfirmation");
            }


            //--------Check new password are same as new----------
            var checkOldPassword = await _userManager.CheckPasswordAsync(user, Input.Password);
            if (checkOldPassword == true)
            {
                ViewData["Errore"] = "New Password can't same as Old Password.";
                ModelState.AddModelError("Error", "New Password can't same as Old Password");
            }
            else
            {

                var result = await _userManager.ChangePasswordAsync(user, Input.OldPassword, Input.Password);
                if (result.Succeeded)
                {
                    ApplicationUser updateUser = new ApplicationUser();
                    user.IsFirstTimeChangedPassword = true;
                    updateUser = user;
                    var changeStatus = await _userManager.UpdateAsync(updateUser);
                    TempData["Success"] = "PasswordReset";
                    //return RedirectToAction("Dashboard", "TicketUser", new { area = "Ticket" });
                    try
                    {
                        UserSessionDetail UserInfo = new UserSessionDetail();
                        HttpContext.Session.SetString("UserSession", JsonConvert.SerializeObject(UserInfo));
                    }
                    catch(Exception ex)
                    {

                    }
                    return RedirectToAction("Login", "Account", new { area = "Identity" });
                }

                foreach (var error in result.Errors)
                {
                    
                    ViewData["Errore"] = error.Description; // "New Password can't same as Old Password.";
                    if ("Incorrect password." == error.Description)
                    {
                        ViewData["Errore"] = "Old password do not match.";
                    }
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            return Page();
        }


       
    }
}

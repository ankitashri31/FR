using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using FirstResponseApp.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Configuration;
using static FirstResponseApp.Models.UtilityHelper.UtilityDto;

namespace FirstResponseApp.Areas.Identity.Pages.Account
{
    [AllowAnonymous]
    public class ForgotPasswordModel : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IEmailSender _emailSender;
        private readonly Utility utility;
        private readonly IConfiguration iconfiguration;
        public ForgotPasswordModel(IConfiguration iconfiguration, Utility _utility, UserManager<ApplicationUser> userManager, IEmailSender emailSender)
        {
            _userManager = userManager;
            _emailSender = emailSender;
            this.utility = _utility;
            this.iconfiguration = iconfiguration;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public class InputModel
        {
            [Required]
            [EmailAddress]
            public string Email { get; set; }
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByEmailAsync(Input.Email);
                
                if (user != null)
                {
                    if (user.IsActive)
                    {
                        string strGeneratePass = utility.GeneratePassword(2, 2, 2, 2);  //--Lower / Upper / Numeric /Special

                        //-----------Set Password By system and send new password into mail---------
                        var code = await _userManager.GeneratePasswordResetTokenAsync(user);
                        var result = await _userManager.ResetPasswordAsync(user, code, strGeneratePass);

                        ApplicationUser updateUser = new ApplicationUser();
                        user.IsFirstTimeChangedPassword = false;
                        updateUser = user;
                        var changeStatus = await _userManager.UpdateAsync(updateUser);

                        ConfirmEmail confirmEmail = new ConfirmEmail()
                        {
                            First_Name = user.Name,
                            Last_Name = user.LastName,
                            Title = "HWLE First Response – Forgot Password",
                            EmailKey = "Forgot",
                            Password = strGeneratePass,
                            Email_To = user.Email,
                            Url = iconfiguration["WebsiteURL"],
                            WebsitePath = iconfiguration["WebsiteURL"]
                        };
                        await utility.SentConfirmMail(confirmEmail);

                        TempData["Success"] = "Success";
                    }
                    else {
                        ViewData["Errore"] = "This account has been deactivated by the admin";
                        ModelState.AddModelError("Error", "This account has been deactivated by the admin");
                        return Page();
                    }
                }
                else {
                    ViewData["Errore"] = "The entered email id is not registered";
                    ModelState.AddModelError("Error", "The entered email id is not registered");
                    return Page();
                }

                return RedirectToPage("./Login");
            }

            return Page();
        }
    }
}

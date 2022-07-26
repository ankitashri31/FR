using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FirstResponseApp.Areas.Ticket.Models;
using FirstResponseApp.Data;
using FirstResponseApp.Helper;
using FirstResponseApp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using static FirstResponseApp.Models.UtilityHelper.UtilityDto;

namespace FirstResponseApp.Areas.Ticket.Controllers
{
    [Authorize]
    [Area("Ticket")]
    public class TicketUserController : Controller
    {
        private readonly AppService appService;
        private UserManager<ApplicationUser> userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly Utility utility;
        private readonly IConfiguration iconfiguration;


        // Utility utility = new Utility();

        #region to declare the static variables
        public TicketUserController(IConfiguration iconfiguration, Utility _utility, AppService appService, UserManager<ApplicationUser> _userManager, SignInManager<ApplicationUser> signInManager)
        {
            this.appService = appService;
            this.userManager = _userManager;
            this._signInManager = signInManager;
            this.utility = _utility;
            this.iconfiguration = iconfiguration;

        }
        #endregion

     
        [Route("TicketUser")]
        [Route("TicketUser/Index")]
        public IActionResult Index()
        {
            var UserInfo = JsonConvert.DeserializeObject<UserSessionDetail>(HttpContext.Session.GetString("UserSession"));
            return View();
        }



        [HttpGet]
        [AuthenticateLinfoxUser]
        [Route("TicketUser/AddTicket")]
        public async Task<IActionResult> AddTicket()
        {
            ViewBag.WebsitePath = iconfiguration["WebsiteURL"];
            TicketDto TicketData = new TicketDto();
            TicketData = await BindTicketData();



            return View(TicketData);
        }

        [HttpGet]
        [AuthenticateHWLEUser]
        [Route("TicketUser/HWLEAddTicket")]
        public async Task<IActionResult> HWLEAddTicket()
        {
            ViewBag.WebsitePath = iconfiguration["WebsiteURL"];
            TicketDto TicketData = new TicketDto();
            TicketData = await BindTicketData();
            return View(TicketData);
        }

        [HttpPost]
        [AuthenticateAllUsers]

        [Route("TicketUser/AddTicket")]
        public async Task<ActionResult> AddTicket(TicketDto TicketDto)
        {
            UserSessionDetail UserInfo = new UserSessionDetail();
            try
            {
                 UserInfo = JsonConvert.DeserializeObject<UserSessionDetail>(HttpContext.Session.GetString("UserSession"));
            }
            catch (Exception ex)
            {
                return Redirect("~/identity/account/login");
            }
            
            if (TicketDto != null)
            {
                
                TicketDto.LoggedInUserId = UserInfo.Id;
                TicketDto.LoggedInOrganisationId = Convert.ToInt64(UserInfo.OrganizationId);
                await appService.AddTicket(TicketDto);
            }

            TempData["Success"] = "AddTicket";

            bool IsUserActive = await appService.GetUserDataByIdStatus(UserInfo.Id);
            UserInfo.IsActive = IsUserActive;
            HttpContext.Session.SetString("UserSession", JsonConvert.SerializeObject(UserInfo));

            return RedirectToAction("Dashboard", "TicketUser");

        }

        [Route("TicketUser/TicketDetail")]
        [AuthenticateAllUsers]
        public async Task<ActionResult> TicketDetail(string id)
        {

            UserSessionDetail UserInfo = new UserSessionDetail();
            try
            {
                UserInfo = JsonConvert.DeserializeObject<UserSessionDetail>(HttpContext.Session.GetString("UserSession"));
            }
            catch (Exception ex)
            {
                return Redirect("~/identity/account/login");
            }

           // var UserInfo = JsonConvert.DeserializeObject<UserSessionDetail>(HttpContext.Session.GetString("UserSession"));
           // var tem = iconfiguration;
            ViewBag.WebsitePath = iconfiguration["WebsiteURL"];
            TicketDto Model = new TicketDto();
            string TicketId = utility.DecryptString(id);
            Model = await GetTicketDetails(Convert.ToInt64(TicketId));

            TicketDto TicketData = new TicketDto();
            TicketData = await BindTicketData();
            Model.OrganisationList = TicketData.OrganisationList;
            Model.ChannelList = TicketData.ChannelList;
            Model.MatterNumberView = Model.MatterNumber;

            utility.AuditHistoryEntry(UserInfo.OrganisationName + "  user has viewed the ticket details of all ticket requests.", Request.Path.Value, Convert.ToInt32(TicketId), UserInfo.Id);

            bool IsUserActive = await appService.GetUserDataByIdStatus(UserInfo.Id);
            UserInfo.IsActive = IsUserActive;
            HttpContext.Session.SetString("UserSession", JsonConvert.SerializeObject(UserInfo));

            return View(Model);
        }



        #region Bind data on ticket details

        public async Task<TicketDto> GetTicketDetails(long TicketId)
        {
            //var UserInfo = JsonConvert.DeserializeObject<UserSessionDetail>(HttpContext.Session.GetString("UserSession"));

            UserSessionDetail UserInfo = new UserSessionDetail();
            try
            {
                UserInfo = JsonConvert.DeserializeObject<UserSessionDetail>(HttpContext.Session.GetString("UserSession"));
            }
            catch (Exception ex)
            {
          
            }

            var TicketDetail = await appService.GetTicketDataById(TicketId);
            var OrganisationData = await appService.GetOrganisationList();
            var ChannelData = await appService.GetChannelMasterData();
            var UserData  = await appService.GetUserData();
            var ResponseDocuments = await appService.GetResponseDocumentByTicketId(TicketId);


            TicketDto TicketData = new TicketDto();
            try
            {
                TicketData.LoggedInOrganisationId = Convert.ToInt64(UserInfo.OrganizationId);
                TicketData.TicketName = TicketDetail.TicketName;
                TicketData.Id = TicketDetail.Id;  //------- Id----TbTicketResponse---
                TicketData.TicketMasterId = TicketDetail.Id; //-----------Ticket Id-- TbTicketMaster
                TicketData.MatterNumber = TicketDetail.MatterNumber;
                TicketData.ChannelId = TicketDetail.ChannelId;
                TicketData.OrganisationId = Convert.ToInt32(TicketDetail.WaitingOnOrganisationId);
                TicketData.OrganisationName = OrganisationData.Where(i => i.Id == TicketDetail.WaitingOnOrganisationId).Select(i => i.OrganisationName).FirstOrDefault();
                TicketData.ChannelName = ChannelData.Where(i => i.Id == TicketDetail.ChannelId).Select(i => i.ChannelName).FirstOrDefault();
                TicketData.CreatedByName = UserData.Where(i => i.Id == TicketDetail.CreatedByUserId).Select(i => i.Name + " " + i.LastName).FirstOrDefault();
                TicketData.AddedOn = TicketDetail.CreatedOn.Value.ToString("dd/MM/yyyy");
                TicketData.StatusId = TicketDetail.StatusId;
                TicketData.Linfoxtext = TicketDetail.TicketNotes;

                TicketData.LastUpdatedOnLinfox = TicketDetail.CreatedOn.Value.ToString("dd/MM/yyyy");
                TicketData.LastUpdatedByLinfox = UserData.Where(i => i.Id == TicketDetail.CreatedByUserId).Select(i => i.Name + " " + i.LastName).FirstOrDefault();

                TicketData.LastUpdatedOnHwle = "NA";
                TicketData.LastUpdatedByHwle = "NA";

                TicketData.LastUpdatedOnMedilow = "NA";
                TicketData.LastUpdatedByMedilow = "NA";

                long LinfoxId = OrganisationData.Where(i => i.OrganisationName.ToLower().Trim() == "linfox").Select(i => i.Id).FirstOrDefault();
                long HWLEId = OrganisationData.Where(i => i.OrganisationName.ToLower().Trim() == "hwle").Select(i => i.Id).FirstOrDefault();
                long MedilawId = OrganisationData.Where(i => i.OrganisationName.ToLower().Trim() == "medilaw").Select(i => i.Id).FirstOrDefault();

                if (ResponseDocuments.Count > 0)
                {
                    //------------check lingox comment exist in reponse table or not------------
                    var chekcLinfoxText = ResponseDocuments.Where(i => i.OrganisationId == LinfoxId && i.TicketMasterId == TicketDetail.Id).OrderByDescending(i => i.Id).Select(i => i.LinfoxNotes).FirstOrDefault();
                    var chekcLinfoxTextBy = ResponseDocuments.Where(i => i.OrganisationId == LinfoxId && i.TicketMasterId == TicketDetail.Id).OrderByDescending(i => i.Id).Select(i => i.CreatedByUserId).FirstOrDefault();
                    if (chekcLinfoxText != null) {
                        TicketData.LastUpdatedByLinfox = UserData.Where(i => i.Id == chekcLinfoxTextBy).Select(i => i.Name + " " + i.LastName).FirstOrDefault();
                        TicketData.LastUpdatedOnLinfox = ResponseDocuments.Where(i => i.OrganisationId == LinfoxId && i.TicketMasterId == TicketDetail.Id).OrderByDescending(i => i.Id).Select(i => i.LastUpdatedOn.Value.ToString("dd/MM/yyyy")).FirstOrDefault();
                        TicketData.Linfoxtext = ResponseDocuments.Where(i => i.OrganisationId == LinfoxId && i.TicketMasterId == TicketDetail.Id).OrderByDescending(i => i.Id).Select(i => i.LinfoxNotes).FirstOrDefault();
                    }
                    //---------------------------------------------------------
                    var HWLEUpdatedOn = ResponseDocuments.Where(i => i.OrganisationId == HWLEId && i.TicketMasterId == TicketDetail.Id).OrderByDescending(i => i.Id).Select(i => i.LastUpdatedOn.Value).FirstOrDefault();
                    var HWLEUpdatedOnBy = ResponseDocuments.Where(i => i.OrganisationId == HWLEId && i.TicketMasterId == TicketDetail.Id).OrderByDescending(i => i.Id).Select(i => i.CreatedByUserId).FirstOrDefault();
                    if (HWLEUpdatedOn!=null) {
                        TicketData.LastUpdatedByHwle = UserData.Where(i => i.Id == HWLEUpdatedOnBy).Select(i => i.Name + " " + i.LastName).FirstOrDefault();
                        TicketData.LastUpdatedOnHwle = ResponseDocuments.Where(i => i.OrganisationId == HWLEId && i.TicketMasterId == TicketDetail.Id).OrderByDescending(i => i.Id).Select(i => i.LastUpdatedOn.Value.ToString("dd/MM/yyyy")).FirstOrDefault();

                    }
                    else {
                        TicketData.LastUpdatedByHwle = "NA";
                        TicketData.LastUpdatedOnHwle = "NA";
                    }

                    TicketData.HWLEText = ResponseDocuments.Where(i => i.OrganisationId == HWLEId && i.TicketMasterId == TicketDetail.Id).OrderByDescending(i => i.Id).Select(i => i.HWLENotes).FirstOrDefault();
                    //----------------------------------------------------------
                    var MedilawLastUpdatedOn = ResponseDocuments.Where(i => i.OrganisationId == MedilawId && i.TicketMasterId == TicketDetail.Id).OrderByDescending(i => i.Id).Select(i => i.LastUpdatedOn.Value).FirstOrDefault();
                    var MedilawLastUpdatedOnBy = ResponseDocuments.Where(i => i.OrganisationId == MedilawId && i.TicketMasterId == TicketDetail.Id).OrderByDescending(i => i.Id).Select(i => i.CreatedByUserId).FirstOrDefault();
                    if (HWLEUpdatedOn != null)
                    {
                        TicketData.LastUpdatedByMedilow = UserData.Where(i => i.Id == MedilawLastUpdatedOnBy).Select(i => i.Name + " " + i.LastName).FirstOrDefault();
                        TicketData.LastUpdatedOnMedilow = ResponseDocuments.Where(i => i.OrganisationId == MedilawId && i.TicketMasterId == TicketDetail.Id).OrderByDescending(i => i.Id).Select(i => i.LastUpdatedOn.Value.ToString("dd/MM/yyyy")).FirstOrDefault();
                    }
                    else
                    {
                        TicketData.LastUpdatedByMedilow = "NA";
                        TicketData.LastUpdatedOnMedilow = "NA";
                    }
                    TicketData.MedilawText = ResponseDocuments.Where(i => i.OrganisationId == MedilawId && i.TicketMasterId == TicketDetail.Id).OrderByDescending(i => i.Id).Select(i => i.MedilawNotes).FirstOrDefault();
                    //---------------------------------------------------------------------
                }

                if(TicketData.LastUpdatedOnLinfox==null)
                {
                    TicketData.LastUpdatedByLinfox = "NA";
                    TicketData.LastUpdatedOnLinfox = "NA";
                }

                if (TicketData.LastUpdatedOnHwle == null)
                {
                    TicketData.LastUpdatedByHwle = "NA";
                    TicketData.LastUpdatedOnHwle = "NA";
                }
                if (TicketData.LastUpdatedOnMedilow == null)
                {
                    TicketData.LastUpdatedByMedilow = "NA";
                    TicketData.LastUpdatedOnMedilow = "NA";
                }

                // TicketData.WaitingOn = TicketDetail.WaitingOnOrganisationId;
                TicketData.WaitingOnName= OrganisationData.Where(i => i.Id== TicketDetail.WaitingOnOrganisationId).Select(i => i.OrganisationName).FirstOrDefault();
                TicketData.EncryptedtTicketId = utility.EnryptString(TicketDetail.Id.ToString());
                var OrgList = await appService.GetOrganisationList();
                if (OrgList.Count() > 0)
                {
                    TicketData.OrganisationList = OrgList.Select(x =>
                                  new SelectListItem()
                                  {
                                      Text = x.OrganisationName.ToString(),
                                      Value = x.Id.ToString(),
                                  }).OrderBy(i => i.Text).ToList();
                }
            }
            catch(Exception ex)
            {

            }



            return TicketData;

        }
        #endregion


        #region Get Document Grid data by ticket id
        [HttpGet]
        [Route("TicketUser/GetDocumentsByTicketId")]
        public async Task<JsonResult> GetDocumentsByTicketId(string id)
        {
            //var UserInfo = JsonConvert.DeserializeObject<UserSessionDetail>(HttpContext.Session.GetString("UserSession"));

            UserSessionDetail UserInfo = new UserSessionDetail();
            try
            {
                UserInfo = JsonConvert.DeserializeObject<UserSessionDetail>(HttpContext.Session.GetString("UserSession"));
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    redirectUrl = Url.Action("login", "/account/", new { area = "identity" }),
                    isRedirect = true
                });
            }

            DocumentData DocumentList = new DocumentData();
            try
            {
                var DocumentData = await appService.GetDocumentListingData(Convert.ToInt64(id), UserInfo.Id);
                DocumentList.DocumentCount = DocumentData.Count();
                DocumentList.Data = DocumentData.ToArray();

            }
            catch (Exception ex)
            {

            }
            return Json(new { recordsFiltered = DocumentList.DocumentCount, recordsTotal = DocumentList.DocumentCount, data = DocumentList.Data });

        }
        #endregion



        #region Post Ticket Details data
        [HttpPost]
        public async Task<ActionResult> UpdateTicketDetails(TicketDto TicketModel)
        {
            if (TicketModel.LoggedInOrganisationId == 3) {

                await appService.InsertOrUpdateTicket(TicketModel);
                TicketModel.WaitingOn = TicketModel.OrganisationId;
            }
            if (TicketModel.LoggedInOrganisationId == 2 || TicketModel.LoggedInOrganisationId == 3)
            {
                TicketModel.MatterNumber = TicketModel.MatterNumberView;
            }

            //var UserInfo = JsonConvert.DeserializeObject<UserSessionDetail>(HttpContext.Session.GetString("UserSession"));

            UserSessionDetail UserInfo = new UserSessionDetail();
            try
            {
                UserInfo = JsonConvert.DeserializeObject<UserSessionDetail>(HttpContext.Session.GetString("UserSession"));
            }
            catch (Exception ex)
            {
                return Redirect("~/identity/account/login");
            }

            TicketModel.LoggedInUserId = UserInfo.Id;
            TicketModel.LoggedInOrganisationId = Convert.ToInt64(UserInfo.OrganizationId);
            await appService.UpdateTicketResponse(TicketModel);

            TempData["Success"] = "AddComment";
            //utility.AuditHistoryEntry(UserInfo.OrganisationName + "  user has updated ticket details.", Request.Path.Value, Convert.ToInt32(TicketModel.Id), UserInfo.Id);

            return RedirectToAction("Dashboard", "TicketUser");
        }

        #endregion


        #region Add User/ Edit User
        [HttpGet]
        [AuthenticateHWLEUser]
        [Route("TicketUser/AddUser")]
        public async Task<IActionResult> AddUser()
        {
            // var UserInfo = JsonConvert.DeserializeObject<UserSessionDetail>(HttpContext.Session.GetString("UserSession"));

            UserSessionDetail UserInfo = new UserSessionDetail();
            try
            {
                UserInfo = JsonConvert.DeserializeObject<UserSessionDetail>(HttpContext.Session.GetString("UserSession"));
                
            }
            catch (Exception ex)
            {
                return Redirect("~/identity/account/login");
            }

            UserDto obj = new UserDto();
            var orgList = await appService.GetOrganisationList();
            if (orgList.Count() > 0)
            {
                obj.OrganizationList = orgList.Select(x =>
                              new SelectListItem()
                              {
                                  Text = x.OrganisationName.ToString(),
                                  Value = x.Id.ToString(),
                              }).OrderBy(i => i.Text).ToList();
            }
            utility.AuditHistoryEntry(UserInfo.OrganisationName+" user has viewed the add user section.", Request.Path.Value, null, UserInfo.Id);

            bool IsUserActive = await appService.GetUserDataByIdStatus(UserInfo.Id);
            UserInfo.IsActive = IsUserActive;
            HttpContext.Session.SetString("UserSession", JsonConvert.SerializeObject(UserInfo));

            return View(obj);
        }

        [HttpPost]
        [AuthenticateHWLEUser]
        [Route("TicketUser/AddUser")]
        public async Task<IActionResult> AddUser(UserDto dTo, string returnUrl = null)
        {
            returnUrl = returnUrl ?? Url.Content("~/");
            //var ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
            // var LoginUserInfo = JsonConvert.DeserializeObject<UserSessionDetail>(HttpContext.Session.GetString("UserSession"));

            UserSessionDetail LoginUserInfo = new UserSessionDetail();
            try
            {
                LoginUserInfo = JsonConvert.DeserializeObject<UserSessionDetail>(HttpContext.Session.GetString("UserSession"));
            }
            catch (Exception ex)
            {
               
                return Redirect("~/identity/account/login");
            }

            UserDto obj = new UserDto();
            if (!ModelState.IsValid)
            { 

                var varTickeUser = new ApplicationUser
                {
                    Name = dTo.Name,
                    UserName = dTo.EmailAddress,
                    LastName = dTo.LastName,
                    Email = dTo.EmailAddress,
                    OrganisationId = Convert.ToInt32(dTo.OrganisationId),
                    IsNotifyOnActiveTicket = dTo.IsNotifyOnActive,
                    IsNotifyOnUpdateTicket = dTo.IsNotifyOnUpdate,
                    IsNotifyOnCloseTicket = dTo.IsNotifyOnClose,
                    CreatedBy = LoginUserInfo.Id,
                    CreatedOn = DateTime.UtcNow,
                    IsActive = true,
                };

                string strGeneratePass = utility.GeneratePassword(2,2,2,2); //---Lower / Upper / Numeric / Spical
                var result = await userManager.CreateAsync(varTickeUser, strGeneratePass);
                if (result.Succeeded)
                {


                    ConfirmEmail confirmEmail = new ConfirmEmail()
                    {
                        First_Name = dTo.Name,
                        Last_Name = dTo.LastName,
                       // Title = "HWLE First Response – Account Created",
                        Title = "First response portal - Account Creation",
                        EmailKey = "AddUser",
                        Password = strGeneratePass,
                        Email_To =  dTo.EmailAddress,
                        Url = iconfiguration["WebsiteURL"],
                        WebsitePath = iconfiguration["WebsiteURL"]
                    };
                  await utility.SentConfirmMail(confirmEmail);

                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            //-------------Load form-------------------
            UserDto objLoad = new UserDto();
            var orgList = await appService.GetOrganisationList();
            if (orgList.Count() > 0)
            {
                obj.OrganizationList = orgList.Select(x =>
                              new SelectListItem()
                              {
                                  Text = x.OrganisationName.ToString(),
                                  Value = x.Id.ToString(),
                              }).OrderBy(i => i.Text).ToList();
            }

            TempData["Success"] = "AddUser";
            
            // return View("AddUser", objLoad);
            utility.AuditHistoryEntry(LoginUserInfo.OrganisationName + " user has added new user "+ dTo.Name +" "+ dTo.LastName + " from add user section", Request.Path.Value, null, LoginUserInfo.Id);

            return RedirectToAction("AllUser");
        }

        [HttpGet]
        [Route("TicketUser/ViewUser")]
        [AuthenticateHWLEUser]
        public async Task<IActionResult> ViewUser(string id)
        {
           // var UserInfo = JsonConvert.DeserializeObject<UserSessionDetail>(HttpContext.Session.GetString("UserSession"));

            UserSessionDetail UserInfo = new UserSessionDetail();
            try
            {
                UserInfo = JsonConvert.DeserializeObject<UserSessionDetail>(HttpContext.Session.GetString("UserSession"));
            }
            catch (Exception ex)
            {

                return Redirect("~/identity/account/login");
            }

            ViewBag.WebsitePath = iconfiguration["WebsiteURL"];
            UserDto obj = new UserDto();
            obj.LoginOrganisationId = UserInfo.OrganizationId;
            var orgList = await appService.GetOrganisationList();
            var user = await userManager.FindByIdAsync(id);
            if (user != null)
            {
                obj.Id = user.Id;
                obj.Name = user.Name;
                obj.LastName = user.LastName;
                obj.EmailAddress = user.Email;
                obj.UserName = user.UserName;
                obj.OrganisationName = orgList.Where(i => i.Id == user.OrganisationId).Select(i => i.OrganisationName).FirstOrDefault();
                obj.CreatedOnShow = user.CreatedOn != null ? Convert.ToDateTime(user.CreatedOn).ToString("dd/MM/yyyy") : "";
                obj.IsNotifyOnActive = user.IsNotifyOnActiveTicket;
                obj.IsNotifyOnUpdate = user.IsNotifyOnUpdateTicket;
                obj.IsNotifyOnClose = user.IsNotifyOnCloseTicket;
                obj.IsActive = user.IsActive;
                obj.LastLogOnDateTime = user.LastLogOnDateTime;
            }
            var CreatedByName = await userManager.FindByIdAsync(user.CreatedBy);
            if (CreatedByName != null) {
                obj.CreatedBy = CreatedByName.Name +" "+ CreatedByName.LastName;
            }

            if (!string.IsNullOrEmpty((string)TempData["Success"]))
            {
                ViewBag.Success = (string)TempData["Success"];
                ViewBag.Message = "Profile has been updated successfully";
            }

            utility.AuditHistoryEntry(UserInfo.OrganisationName + " user has viewed the profile of user ID "+ id + "", Request.Path.Value, null, UserInfo.Id);

            bool IsUserActive = await appService.GetUserDataByIdStatus(UserInfo.Id);
            UserInfo.IsActive = IsUserActive;
            HttpContext.Session.SetString("UserSession", JsonConvert.SerializeObject(UserInfo));

            return View(obj);
        }

        [HttpGet]
        [AuthenticateAllUsers]
        [Route("TicketUser/EditUser")]
        public async Task<IActionResult> EditUser(string id)
        {
            ViewBag.WebsitePath = iconfiguration["WebsiteURL"];
            //  var UserInfo = JsonConvert.DeserializeObject<UserSessionDetail>(HttpContext.Session.GetString("UserSession"));
            UserSessionDetail UserInfo = new UserSessionDetail();
            try
            {
                UserInfo = JsonConvert.DeserializeObject<UserSessionDetail>(HttpContext.Session.GetString("UserSession"));
            }
            catch (Exception ex)
            {

                return Redirect("~/identity/account/login");
            }

            UserDto obj = new UserDto();
            var orgList = await appService.GetOrganisationList();
            if (orgList.Count() > 0)
            {
                obj.OrganizationList = orgList.Select(x =>
                              new SelectListItem()
                              {
                                  Text = x.OrganisationName.ToString(),
                                  Value = x.Id.ToString(),
                              }).OrderBy(i => i.Text).ToList();
            }

             var user = await userManager.FindByIdAsync(id);
            if (user != null) {
                obj.Id = user.Id;
                obj.Name = user.Name;
                obj.LastName = user.LastName;
                obj.EmailAddress = user.Email;
                obj.EmailAddressEdit = user.Email;
                obj.OrganisationId = user.OrganisationId.ToString();
                obj.IsNotifyOnActive = user.IsNotifyOnActiveTicket;
                obj.IsNotifyOnUpdate = user.IsNotifyOnUpdateTicket;
                obj.IsNotifyOnClose = user.IsNotifyOnCloseTicket;
            }

            utility.AuditHistoryEntry(UserInfo.OrganisationName + "  user has viewed edit user section.", Request.Path.Value, null, UserInfo.Id);

            bool IsUserActive = await appService.GetUserDataByIdStatus(UserInfo.Id);
            UserInfo.IsActive = IsUserActive;
            HttpContext.Session.SetString("UserSession", JsonConvert.SerializeObject(UserInfo));

            return View(obj);
        }

        [HttpPost]
        [AuthenticateHWLEUser]
        [Route("TicketUser/EditUser")]
        public async Task<IActionResult> EditUser(UserDto dTo, string returnUrl = null)
        {
            returnUrl = returnUrl ?? Url.Content("~/");
            //var LoginUserInfo = JsonConvert.DeserializeObject<UserSessionDetail>(HttpContext.Session.GetString("UserSession"));
            UserSessionDetail LoginUserInfo = new UserSessionDetail();
            try
            {
                LoginUserInfo = JsonConvert.DeserializeObject<UserSessionDetail>(HttpContext.Session.GetString("UserSession"));
            }
            catch (Exception ex)
            {

                return Redirect("~/identity/account/login");
            }

            UserDto obj = new UserDto();
            if (!ModelState.IsValid)
            {
                 var varSaveDetail = await userManager.FindByIdAsync(dTo.Id);
                if (varSaveDetail != null) {

                    if (!string.IsNullOrEmpty(dTo.Name))
                    {
                        varSaveDetail.Name = dTo.Name;
                    }
                    if (!string.IsNullOrEmpty(dTo.EmailAddressEdit))
                    {
                        varSaveDetail.UserName = dTo.EmailAddressEdit;
                    }
                    if (!string.IsNullOrEmpty(dTo.LastName))
                    {
                        varSaveDetail.LastName = dTo.LastName;
                    }
                    if (!string.IsNullOrEmpty(dTo.EmailAddressEdit))
                    {
                        varSaveDetail.Email = dTo.EmailAddressEdit;
                    }
                    if (!string.IsNullOrEmpty(dTo.OrganisationId))
                    {
                        varSaveDetail.OrganisationId = Convert.ToInt32(dTo.OrganisationId);
                    }

                    varSaveDetail.IsNotifyOnActiveTicket = dTo.IsNotifyOnActive;
                    varSaveDetail.IsNotifyOnUpdateTicket = dTo.IsNotifyOnUpdate;
                    varSaveDetail.IsNotifyOnCloseTicket = dTo.IsNotifyOnClose;
                    varSaveDetail.LastModifiedBy = LoginUserInfo.Id;
                    varSaveDetail.LastUpdatedOn = DateTime.UtcNow;
                    varSaveDetail.IsActive = true;
                }
                var result = await userManager.UpdateAsync(varSaveDetail);
                if (result.Succeeded)
                {
                    ConfirmEmail confirmEmail = new ConfirmEmail()
                    {
                        First_Name = dTo.Name,
                        Last_Name = dTo.LastName,
                        Title = "First response portal - Profile updated",
                        EmailKey = "UpdateProfile",
                        Email_To = dTo.EmailAddress,
                        Url = iconfiguration["WebsiteURL"],
                        WebsitePath = iconfiguration["WebsiteURL"]
                    };
                    await utility.SentConfirmMail(confirmEmail);
                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
                TempData["Success"] = "UpdateUser";

            }
            utility.AuditHistoryEntry(LoginUserInfo.OrganisationName + "  user has edited user's profile ID "+ dTo.Id + ".", Request.Path.Value, null, LoginUserInfo.Id);

            return RedirectToAction("ViewUser", "TicketUser", new {id=dTo.Id });
        }

        [HttpPost]
        [AuthenticateHWLEUser]
        [Route("TicketUser/DeactiveUser")]
        public async Task<IActionResult> DeactiveUser(string id, string url)
        {
            //var LoginUserInfo = JsonConvert.DeserializeObject<UserSessionDetail>(HttpContext.Session.GetString("UserSession"));

            UserSessionDetail LoginUserInfo = new UserSessionDetail();
            try
            {
                LoginUserInfo = JsonConvert.DeserializeObject<UserSessionDetail>(HttpContext.Session.GetString("UserSession"));
            }
            catch (Exception ex)
            {

                return Redirect("~/identity/account/login");
            }
            string strIsActive = "";
            UserDto obj = new UserDto();
            bool IsSelfAccountDeactivated = false;
            if (id != null)
            {
                var varSaveDetail = await userManager.FindByIdAsync(id);
                if (varSaveDetail != null)
                {
                    varSaveDetail.LastUpdatedOn = DateTime.UtcNow;
                    if (varSaveDetail.IsActive == true)
                    {
                        varSaveDetail.LastDeactivatedOn = DateTime.UtcNow;
                        varSaveDetail.IsActive = false;
                        varSaveDetail.CreatedBy = LoginUserInfo.Id;
                        strIsActive = "deactivated";
                        //added to get logged out
                        if(LoginUserInfo.Id == id)
                        {
                            IsSelfAccountDeactivated = true;
                        }
                    }
                    else
                    {
                        varSaveDetail.LastDeactivatedOn = null;
                        varSaveDetail.IsActive = true;
                        varSaveDetail.CreatedBy = LoginUserInfo.Id;
                        strIsActive = "Activated"; 
                    }
                }
                var result = await userManager.UpdateAsync(varSaveDetail);
                if (result.Succeeded)
                {


                    var user = varSaveDetail;
                    var orgList = await appService.GetOrganisationList();
                    obj.Id = user.Id;
                    obj.Name = user.Name;
                        obj.LastName = user.LastName;
                        obj.EmailAddress = user.Email;
                        obj.OrganisationName = orgList.Where(i => i.Id == user.OrganisationId).Select(i => i.OrganisationName).FirstOrDefault();
                        obj.CreatedOnShow = user.CreatedOn != null ? Convert.ToDateTime(user.CreatedOn).ToString("dd/MM/yyyy") : "";
                        obj.IsNotifyOnActive = user.IsNotifyOnActiveTicket;
                        obj.IsNotifyOnUpdate = user.IsNotifyOnUpdateTicket;
                        obj.IsNotifyOnClose = user.IsNotifyOnCloseTicket;
                        obj.IsActive = user.IsActive;

                    var CreatedByName = await userManager.FindByIdAsync(user.CreatedBy);
                    if (CreatedByName != null)
                    {
                        obj.CreatedBy = CreatedByName.Name;
                    }


                    //----------------Send Dactive mail-----------
                    try {
                        ConfirmEmail confirmEmail = new ConfirmEmail()
                        {
                            First_Name = user.Name,
                            Last_Name = user.LastName,
                            Title = "First response portal – Account " + strIsActive ,
                            EmailKey = "Status",
                            Status = strIsActive,
                            Email_To = user.Email,
                            Url = iconfiguration["WebsiteURL"],
                            WebsitePath = iconfiguration["WebsiteURL"]
                        };
                        await utility.SentConfirmMail(confirmEmail);
                    }
                    catch (Exception ex) { 
                    }

                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }
            ViewBag.Success = "Form submitted.";
            ViewBag.Message = "User Has been " + strIsActive + " successfully ";

            utility.AuditHistoryEntry(LoginUserInfo.OrganisationName + " user has deactivated selected user Id "+ id + "", Request.Path.Value, null, LoginUserInfo.Id);

           // if(IsSelfAccountDeactivated==true)
            //{
                //await _signInManager.SignOutAsync();
                //return RedirectToAction("Login", "Account", new { area = "Identity" });
                //return RedirectToAction("login", "account", new { area = "identity" });
           // }

            if (url == "AllUser")
                return Json(new { success = true, responseText = "Your message successfuly sent!" });
            else
                return View("ViewUser", obj);
        }

        [HttpPost]
        [AuthenticateAllUsers]
        [Route("TicketUser/ResetPassword")]
        public async Task<IActionResult> ResetPassword(string id, string url)
        {
            // UserInfo = JsonConvert.DeserializeObject<UserSessionDetail>(HttpContext.Session.GetString("UserSession"));
            UserSessionDetail UserInfo = new UserSessionDetail();
            try
            {
                UserInfo = JsonConvert.DeserializeObject<UserSessionDetail>(HttpContext.Session.GetString("UserSession"));
            }
            catch (Exception ex)
            {

                return Redirect("~/identity/account/login");
            }

            UserDto obj = new UserDto();
            if (id != null)
            {
                var user = await userManager.FindByIdAsync(id);

                if (user != null)
                {
                    string strGeneratePass = utility.GeneratePassword(2, 2, 2, 2);  //--Lower / Upper / Numeric /Special

                    //-----------Set Password By system and send new password into mail---------
                    var code = await userManager.GeneratePasswordResetTokenAsync(user);
                    var result = await userManager.ResetPasswordAsync(user, code, strGeneratePass);
                    if (result.Succeeded) {
                        var orgList = await appService.GetOrganisationList();
                        obj.Id = user.Id;
                        obj.Name = user.Name;
                        obj.LastName = user.LastName;
                        obj.EmailAddress = user.Email;
                        obj.OrganisationName = orgList.Where(i => i.Id == user.OrganisationId).Select(i => i.OrganisationName).FirstOrDefault();
                        obj.CreatedOnShow = user.CreatedOn != null ? Convert.ToDateTime(user.CreatedOn).ToString("dd/MM/yyyy") : "";
                        obj.IsNotifyOnActive = user.IsNotifyOnActiveTicket;
                        obj.IsNotifyOnUpdate = user.IsNotifyOnUpdateTicket;
                        obj.IsNotifyOnClose = user.IsNotifyOnCloseTicket;

                        var CreatedByName = await userManager.FindByIdAsync(user.CreatedBy);
                        if (CreatedByName != null)
                        {
                            obj.CreatedBy = CreatedByName.Name;
                        }
                    }

                    ApplicationUser updateUser = new ApplicationUser();
                    user.IsFirstTimeChangedPassword = false;
                    updateUser = user;
                    var changeStatus = await userManager.UpdateAsync(updateUser);

                    ConfirmEmail confirmEmail = new ConfirmEmail()
                    {
                        First_Name = user.Name,
                        Last_Name = user.LastName,
                       // Title = "HWLE First Response – Password Reset",
                        Title= "First response portal - Password reset",
                        EmailKey = "GeneratePassword",
                        Password = strGeneratePass,
                        Email_To = user.Email,
                        Url = iconfiguration["WebsiteURL"],
                        WebsitePath = iconfiguration["WebsiteURL"]
                    };
                    await utility.SentConfirmMail(confirmEmail);


                }
                else
                {
                    ModelState.AddModelError("Error", "This mail id not exist in Database");
                }

            }
            utility.AuditHistoryEntry(UserInfo.OrganisationName + " user has reset the password", Request.Path.Value, null, UserInfo.Id);

            ViewBag.Success = "Form submitted.";
            ViewBag.Message = "System generated password has been sent on registered email address";
            if (url == "AllUser")
              return Json(new { success = true, responseText = "Your message successfuly sent!" });
            else
                return View("ViewUser", obj);
        }


        [HttpGet]
        [AuthenticateHWLEUser]
        [Route("TicketUser/AllUser")]
        public async Task<IActionResult> AllUser()
        {
            //var UserInfo = JsonConvert.DeserializeObject<UserSessionDetail>(HttpContext.Session.GetString("UserSession"));
            UserSessionDetail UserInfo = new UserSessionDetail();
            try
            {
                UserInfo = JsonConvert.DeserializeObject<UserSessionDetail>(HttpContext.Session.GetString("UserSession"));
            }
            catch (Exception ex)
            {
                return Redirect("~/identity/account/login");
            }
            ViewBag.WebsitePath = iconfiguration["WebsiteURL"];
            UserDto obj = new UserDto();
            var orgList = await appService.GetOrganisationList();
            if (orgList.Count() > 0)
            {
                obj.OrganizationList = orgList.Select(x =>
                              new SelectListItem()
                              {
                                  Text = x.OrganisationName.ToString(),
                                  Value = x.Id.ToString(),
                              }).OrderBy(i => i.Text).ToList();
            }
            List<SelectListItem> listActive = new List<SelectListItem>();
            listActive.Add(new SelectListItem { Text = "Active", Value = "1" });
            listActive.Add(new SelectListItem { Text = "Inactive", Value = "2" });
            obj.IsActiveList = listActive;
            utility.AuditHistoryEntry(UserInfo.OrganisationName + "  user has viewed the all users section.", Request.Path.Value, null, UserInfo.Id);


            if (!string.IsNullOrEmpty((string)TempData["Success"]))
            {
                ViewBag.Success = (string)TempData["Success"];
                ViewBag.Message = "New user has been created successfully.";
            }

            bool IsUserActive = await appService.GetUserDataByIdStatus(UserInfo.Id);
            UserInfo.IsActive = IsUserActive;
            HttpContext.Session.SetString("UserSession", JsonConvert.SerializeObject(UserInfo));

            return View(obj);
        }


        [HttpPost]
 
        [Route("TicketUser/GetUserData")]
        public async Task<JsonResult> GetUserData(string IsActive, string Organisationid, string CreatedOn, string TextSearch)
        {
            UserData Model = new UserData();
            try
            {
                var UserData = await appService.GetUserListingData(Convert.ToInt64(IsActive), Convert.ToInt64(Organisationid), CreatedOn, TextSearch);
                Model.ApplicationCount = UserData.Count();
                Model.Data = UserData.ToArray();
                //var UserInfo = JsonConvert.DeserializeObject<UserSessionDetail>(HttpContext.Session.GetString("UserSession"));
                UserSessionDetail UserInfo = new UserSessionDetail();
                try
                {
                    UserInfo = JsonConvert.DeserializeObject<UserSessionDetail>(HttpContext.Session.GetString("UserSession"));
                }
                catch (Exception ex)
                {
                    return Json(new
                    {
                        redirectUrl = Url.Action("login", "/account/", new { area = "identity" }),
                        isRedirect = true
                    });
                }

                bool IsUserActive = await appService.GetUserDataByIdStatus(UserInfo.Id);
                UserInfo.IsActive = IsUserActive;
                HttpContext.Session.SetString("UserSession", JsonConvert.SerializeObject(UserInfo));
            }
            catch (Exception ex)
            {

            }
            return Json(new { recordsFiltered = Model.ApplicationCount, recordsTotal = Model.ApplicationCount, data = Model.Data.OrderByDescending(i=>i.CreatedOnDate)});

            

        }


        [AllowAnonymous]
        [Route("TicketUser/UserAlreadyExistsAsync11")]
        public async Task<JsonResult> UserAlreadyExistsAsync11(string emailaddress)
        {
            var result =
                await userManager.FindByNameAsync(emailaddress) ??
                await userManager.FindByEmailAsync(emailaddress);
            return Json(result == null);

        }

        [AllowAnonymous]
        [Route("TicketUser/UserAlreadyExistsAsyncEdit")]
        public async Task<JsonResult> UserAlreadyExistsAsyncEdit(string EmailAddressEdit,string id)
        {
            var result = (String)null;
            bool varStatus = await appService.IsEmailIdExist(EmailAddressEdit,id);
            if (varStatus == true)
                return Json(result == null);
            else
                return Json(result == "Already Exsit");

        }


        [Route("TicketUser/Profile")]
      
        [AuthenticateAllUsers]
        public async Task<IActionResult> Profile()
        {
            //var UserInfo = JsonConvert.DeserializeObject<UserSessionDetail>(HttpContext.Session.GetString("UserSession"));
            UserSessionDetail UserInfo = new UserSessionDetail();
            try
            {
                UserInfo = JsonConvert.DeserializeObject<UserSessionDetail>(HttpContext.Session.GetString("UserSession"));
            }
            catch (Exception ex)
            {
                return Redirect("~/identity/account/login");
            }

            UserDto obj = new UserDto();
            var orgList = await appService.GetOrganisationList();
            var user = await userManager.FindByIdAsync(UserInfo.Id);
            if (user != null)
            {
                obj.Id = user.Id;
                obj.Name = user.Name;
                obj.LastName = user.LastName;
                obj.EmailAddress = user.Email;
                obj.UserName = user.UserName;
                obj.OrganisationName = orgList.Where(i => i.Id == user.OrganisationId).Select(i => i.OrganisationName).FirstOrDefault();
                obj.CreatedOnShow = user.CreatedOn != null ? Convert.ToDateTime(user.CreatedOn).ToString("dd/MM/yyyy") : "";
                obj.IsNotifyOnActive = user.IsNotifyOnActiveTicket;
                obj.IsNotifyOnUpdate = user.IsNotifyOnUpdateTicket;
                obj.IsNotifyOnClose = user.IsNotifyOnCloseTicket;
            }
            var CreatedByName = await userManager.FindByIdAsync(user.CreatedBy);
            if (CreatedByName != null)
            {
                obj.CreatedBy = CreatedByName.Name+" "+ CreatedByName.LastName;
            }
            utility.AuditHistoryEntry(UserInfo.OrganisationName + " user has viewed profile section", Request.Path.Value, null, UserInfo.Id);

            if (!string.IsNullOrEmpty((string)TempData["Success"]))
            {
                ViewBag.Success = (string)TempData["Success"];
                ViewBag.Message = "your profile has been updated successfully";
            }

            
            bool IsUserActive = await appService.GetUserDataByIdStatus(UserInfo.Id);
            UserInfo.IsActive = IsUserActive;
            HttpContext.Session.SetString("UserSession", JsonConvert.SerializeObject(UserInfo));


            return View(obj);
            
        }


        [Route("TicketUser/MyProfile")]
     
        [AuthenticateAllUsers]
        public async Task<IActionResult> MyProfile(string id)
        {
            //var UserInfo = JsonConvert.DeserializeObject<UserSessionDetail>(HttpContext.Session.GetString("UserSession"));
            UserSessionDetail UserInfo = new UserSessionDetail();
            try
            {
                UserInfo = JsonConvert.DeserializeObject<UserSessionDetail>(HttpContext.Session.GetString("UserSession"));
            }
            catch (Exception ex)
            {
                return Redirect("~/identity/account/login");
            }
            UserDto obj = new UserDto();
            var orgList = await appService.GetOrganisationList();
            obj.LoginOrganisationId = UserInfo.OrganizationId;
            if (orgList.Count() > 0)
            {
                obj.OrganizationList = orgList.Select(x =>
                              new SelectListItem()
                              {
                                  Text = x.OrganisationName.ToString(),
                                  Value = x.Id.ToString(),
                              }).OrderBy(i=>i.Text).ToList();
            }

            var user = await userManager.FindByIdAsync(id);
            if (user != null)
            {
                obj.Id = user.Id;
                obj.Name = user.Name;
                obj.LastName = user.LastName;
                obj.EmailAddress = user.Email;
                obj.EmailAddressEdit = user.Email;
                obj.OrganisationId = user.OrganisationId.ToString();
                obj.IsNotifyOnActive = user.IsNotifyOnActiveTicket;
                obj.IsNotifyOnUpdate = user.IsNotifyOnUpdateTicket;
                obj.IsNotifyOnClose = user.IsNotifyOnCloseTicket;
            }

            utility.AuditHistoryEntry(UserInfo.OrganisationName + " user has viewed profile section", Request.Path.Value, null, UserInfo.Id);

            bool IsUserActive = await appService.GetUserDataByIdStatus(UserInfo.Id);
            UserInfo.IsActive = IsUserActive;
            HttpContext.Session.SetString("UserSession", JsonConvert.SerializeObject(UserInfo));

            return View(obj);

        }

        [HttpPost]
        [Route("TicketUser/MyProfile")]

        public async Task<IActionResult> MyProfile(UserDto dTo, string returnUrl = null)
        {
          //  var UserInfo = JsonConvert.DeserializeObject<UserSessionDetail>(HttpContext.Session.GetString("UserSession"));
            returnUrl = returnUrl ?? Url.Content("~/");
           // var LoginUserInfo = JsonConvert.DeserializeObject<UserSessionDetail>(HttpContext.Session.GetString("UserSession"));

            UserSessionDetail UserInfo = new UserSessionDetail();
            UserSessionDetail LoginUserInfo = new UserSessionDetail();
            try
            {
                UserInfo = JsonConvert.DeserializeObject<UserSessionDetail>(HttpContext.Session.GetString("UserSession"));
                LoginUserInfo = JsonConvert.DeserializeObject<UserSessionDetail>(HttpContext.Session.GetString("UserSession"));
            }
            catch (Exception ex)
            {
                return Redirect("~/identity/account/login");
            }


            UserDto obj = new UserDto();
            if (!ModelState.IsValid)
            {
                var varSaveDetail = await userManager.FindByIdAsync(dTo.Id);
                if (varSaveDetail != null)
                {


                    if(!string.IsNullOrEmpty(dTo.Name))
                    {
                        varSaveDetail.Name = dTo.Name;
                    }
                    if (!string.IsNullOrEmpty(dTo.LastName))
                    {
                        varSaveDetail.LastName = dTo.LastName;
                    }
                    if (!string.IsNullOrEmpty(dTo.EmailAddressEdit))
                    {
                        varSaveDetail.Email = dTo.EmailAddressEdit;
                        varSaveDetail.UserName = dTo.EmailAddressEdit;
                    }
    
                    varSaveDetail.LastModifiedBy = LoginUserInfo.Id;
                    varSaveDetail.LastUpdatedOn = DateTime.UtcNow;
                    //varSaveDetail.IsActive = true;
                }
                var result = await userManager.UpdateAsync(varSaveDetail);
                if (result.Succeeded)
                {
                    // return LocalRedirect(returnUrl);
                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
               
            }

            utility.AuditHistoryEntry(UserInfo.OrganisationName + " user has updated profile section", Request.Path.Value, null, UserInfo.Id);

            TempData["Success"] = "UpdatedUser";

           

            return RedirectToAction("Profile", dTo);
        }

        #endregion

   
        [AuthenticateAllUsers]
        [Route("TicketUser/Dashboard")]
        public async Task<ActionResult> Dashboard()
        {

            if (!string.IsNullOrEmpty((string)TempData["Success"]))
            {
                ViewBag.Success = (string)TempData["Success"];
                string strChek = (string)TempData["Success"];
                if (strChek.ToLower() == "addcomment")
                {
                    ViewBag.Message = "Ticket details has been updated successfully";
                }
                else if (strChek.ToLower() == "passwordreset")
                {
                    ViewBag.Message = "The password has been reset successfully.";
                }
                else
                {
                    ViewBag.Message = "Ticket has been created successfully";
                }
            }

            TicketDto Model = new TicketDto();
            ViewBag.WebsitePath = iconfiguration["WebsiteURL"];
            // var UserInfo = JsonConvert.DeserializeObject<UserSessionDetail>(HttpContext.Session.GetString("UserSession"));
            UserSessionDetail UserInfo = new UserSessionDetail();
            try
            {
                UserInfo = JsonConvert.DeserializeObject<UserSessionDetail>(HttpContext.Session.GetString("UserSession"));
             
            }
            catch (Exception ex)
            {
                return Redirect("~/identity/account/login");
            }

            ViewBag.LoggedInOrganisationId = UserInfo.OrganizationId;
            Model = await BindTicketData();
            Model.LoggedInOrganisationId = Convert.ToInt64(UserInfo.OrganizationId);
            utility.AuditHistoryEntry(UserInfo.OrganisationName+ "  user has viewed the dashboard of all ticket requests.", Request.Path.Value, null, UserInfo.Id);

            bool IsUserActive = await appService.GetUserDataByIdStatus(UserInfo.Id);
            UserInfo.IsActive = IsUserActive;
            HttpContext.Session.SetString("UserSession", JsonConvert.SerializeObject(UserInfo));

            return View(Model);

        }

        #region Downloading single document

        [AllowAnonymous]
        public IActionResult DonwloadDocumentById(int id,string type)
        {
            //var UserInfo = JsonConvert.DeserializeObject<UserSessionDetail>(HttpContext.Session.GetString("UserSession"));
            UserSessionDetail UserInfo = new UserSessionDetail();
            try
            {
                UserInfo = JsonConvert.DeserializeObject<UserSessionDetail>(HttpContext.Session.GetString("UserSession"));

            }
            catch (Exception ex)
            {
                return Redirect("~/identity/account/login");
            }

            var document = (dynamic)null;
            if (type == "ticketdocument") {
                document = appService.GetDocumentById(id).Result;
            }
            else
            {
                document = appService.GetDocumentFromResponseById(id).Result; 
            }
            string word = document.FileName;
            try
            {
                utility.AuditHistoryEntry(UserInfo.OrganisationName + " user has download the document.", Request.Path.Value, Convert.ToInt32(id), UserInfo.Id);

                return File(document.FileData, "application/force-download", word);
            }
            catch (Exception ex)
            {

            }
            utility.AuditHistoryEntry(UserInfo.OrganisationName + " user has download the document.", Request.Path.Value, Convert.ToInt32(id), UserInfo.Id);

            return File(document.FileData, "application/force-download", word);
        }
        #endregion


        #region Bind ticket active grid data
        [HttpGet]
        [Route("TicketUser/GetTicketData")]
        public async Task<JsonResult> GetTicketData()
        {
            TicketData Model = new TicketData();
            try
            {
               
                var TicketData = await appService.GetActiveListingData(0,0,"","");
                Model.ApplicationCount = TicketData.Count();
                Model.Data = TicketData.ToArray();

            }
            catch (Exception ex)
            {

            }
            UserSessionDetail UserInfo = new UserSessionDetail();
            try
            {
                UserInfo = JsonConvert.DeserializeObject<UserSessionDetail>(HttpContext.Session.GetString("UserSession"));
                
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    redirectUrl = Url.Action("login", "/account/", new { area = "identity" }),
                    isRedirect = true
                });
            }
            // var UserInfo = JsonConvert.DeserializeObject<UserSessionDetail>(HttpContext.Session.GetString("UserSession"));
            bool IsUserActive = await appService.GetUserDataByIdStatus(UserInfo.Id);
            UserInfo.IsActive = IsUserActive;
            HttpContext.Session.SetString("UserSession", JsonConvert.SerializeObject(UserInfo));

            return Json(new { recordsFiltered = Model.ApplicationCount, recordsTotal = Model.ApplicationCount, data = Model.Data });

        }
        #endregion

        #region Bind ticket closed grid data
        [HttpGet]
        [Route("TicketUser/GetClosedTicketData")]
        public async Task<JsonResult> GetClosedTicketData(string ChannelId, string Organisationid, string CreatedOn, string TextSearch)
        {
            TicketData Model = new TicketData();
            try
            {
                var TicketData = await appService.GetClosedTicketListingData(Convert.ToInt64(ChannelId), Convert.ToInt64(Organisationid), CreatedOn, TextSearch);
                Model.ApplicationCount = TicketData.Count();
                Model.Data = TicketData.ToArray();

            }
            catch (Exception ex)
            {

            }
            return Json(new { recordsFiltered = Model.ApplicationCount, recordsTotal = Model.ApplicationCount, data = Model.Data });

        }
        #endregion



        #region Method to bind all input data for Add Ticket view

        public async Task<TicketDto> BindTicketData()
        {
            TicketDto TicketData = new TicketDto();
            try
            {
                var OrganisationList = await appService.GetOrganisationList();
                if (OrganisationList.Count() > 0)
                {
                    TicketData.OrganisationList = OrganisationList.Select(x =>
                                  new SelectListItem()
                                  {
                                      Text = x.OrganisationName.ToString(),
                                      Value = x.Id.ToString(),
                                  }).OrderBy(i=>i.Text).ToList();
                }

                var ChannelList = await appService.GetChannelMasterData();
                if (ChannelList.Count() > 0)
                {
                    TicketData.ChannelList = ChannelList.Select(x =>
                                  new SelectListItem()
                                  {
                                      Text = x.ChannelName.ToString(),
                                      Value = x.Id.ToString(),
                                  }).OrderBy(i => i.Text).ToList();
                }
            }
            catch(Exception ex)
            {

            }

           
            return TicketData;
        }

        #endregion


        #region Bind ticket active grid data by Search filter
        [HttpPost]
        [Route("TicketUser/GetActiveTicketBySearch")]
        public async Task<JsonResult> GetActiveTicketBySearch(string ChannelId, string Organisationid, string CreatedOn, string TextSearch)
        {
            TicketData Model = new TicketData();
            //var UserInfo = JsonConvert.DeserializeObject<UserSessionDetail>(HttpContext.Session.GetString("UserSession"));
            UserSessionDetail UserInfo = new UserSessionDetail();
            try
            {
                UserInfo = JsonConvert.DeserializeObject<UserSessionDetail>(HttpContext.Session.GetString("UserSession"));

            }
            catch (Exception ex)
            {
                return Json(new
                {
                    redirectUrl = Url.Action("login", "/account/", new { area = "identity" }),
                    isRedirect = true
                });
            }
            List<TicketModel> TicketData = new List<TicketModel>();
            try
            {
                
                ViewBag.LoggedInOrganisationId = UserInfo.OrganizationId;
                TicketData = await appService.GetActiveListingData(Convert.ToInt64(ChannelId), Convert.ToInt64(Organisationid), CreatedOn, TextSearch);
              
                Model.ApplicationCount = TicketData.Count();
                Model.Data = TicketData.ToArray();
               

            }
            catch (Exception ex)
            {

            }
            bool IsUserActive = await appService.GetUserDataByIdStatus(UserInfo.Id);
            UserInfo.IsActive = IsUserActive;
            HttpContext.Session.SetString("UserSession", JsonConvert.SerializeObject(UserInfo));

            return Json(new { recordsFiltered = Model.ApplicationCount, recordsTotal = Model.ApplicationCount, data = Model.Data });

        }
        #endregion


        #region Close ticket by Id
        [HttpGet]
        [Route("TicketUser/CloseTicketById")]
        public async Task<JsonResult> CloseTicketById(string id)
        {
            //var UserInfo = JsonConvert.DeserializeObject<UserSessionDetail>(HttpContext.Session.GetString("UserSession"));
            UserSessionDetail UserInfo = new UserSessionDetail();
            try
            {
                UserInfo = JsonConvert.DeserializeObject<UserSessionDetail>(HttpContext.Session.GetString("UserSession"));

            }
            catch (Exception ex)
            {
                return Json(new
                {
                    redirectUrl = Url.Action("login", "/account/", new { area = "identity" }),
                    isRedirect = true
                });
            }
            string IsSuccess = string.Empty;
            try
            {
               IsSuccess = await appService.CloseTicketById(Convert.ToInt64(id), UserInfo.Id);
               utility.AuditHistoryEntry(UserInfo.OrganisationName+" user has closed the ticket.", Request.Path.Value,Convert.ToInt32(id), UserInfo.Id);

            }
            catch (Exception ex)
            {

            }
            bool IsUserActive = await appService.GetUserDataByIdStatus(UserInfo.Id);
            UserInfo.IsActive = IsUserActive;
            HttpContext.Session.SetString("UserSession", JsonConvert.SerializeObject(UserInfo));
            return Json(IsSuccess);
        }

        #endregion


        #region delete document by Id
        [HttpGet]
        [Route("TicketUser/DeleteDocumentById")]
        public async Task<JsonResult> DeleteDocumentById(string id, string TableType)
        {
            // var UserInfo = JsonConvert.DeserializeObject<UserSessionDetail>(HttpContext.Session.GetString("UserSession"));
            UserSessionDetail UserInfo = new UserSessionDetail();
            try
            {
                UserInfo = JsonConvert.DeserializeObject<UserSessionDetail>(HttpContext.Session.GetString("UserSession"));
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    redirectUrl = Url.Action("login", "/account/", new { area = "identity" }),
                    isRedirect = true
                });
            }

            string IsSuccess = string.Empty;
            try
            {
                IsSuccess = await appService.DeleteDocumentByTableId(Convert.ToInt64(id), TableType, UserInfo.Id);
                utility.AuditHistoryEntry(UserInfo.OrganisationName + " user has closed the ticket.", Request.Path.Value, Convert.ToInt32(id), UserInfo.Id);

            }
            catch (Exception ex)
            {

            }
            return Json(IsSuccess);
        }

        #endregion
    }
}
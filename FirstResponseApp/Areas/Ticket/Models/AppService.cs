using FirstResponseApp.Data;
using FirstResponseApp.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Newtonsoft.Json;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using static FirstResponseApp.Models.UtilityHelper.UtilityDto;
using Microsoft.AspNetCore.Mvc.Filters;

namespace FirstResponseApp.Areas.Ticket.Models
{
    public class AppService
    {
        #region  variables declairation
        private UserManager<ApplicationUser> userManager;
        private Utility _utility;
        public ApplicationDbContext Context;
        private readonly IConfiguration iconfiguration;
        // Utility _utility = new Utility();
        // ApplicationDbContext Context = new ApplicationDbContext();


        public AppService(IConfiguration iconfiguration, ApplicationDbContext applicationDbContext,
                           UserManager<ApplicationUser> userManager,
                           Utility utility)
        {
            this.Context = applicationDbContext;
            this.userManager = userManager;
            this._utility = utility;
            this.iconfiguration = iconfiguration;
        }
        #endregion

        #region  Getting List of All Organisation 
        public async Task<List<TbOrganisationMaster>> GetOrganisationList()
        {
            List<TbOrganisationMaster> orgList = new List<TbOrganisationMaster>();
            try
            {
                orgList = Context.TbOrganisationMaster.Where(e => e.IsActive == true).ToList();
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return orgList;
        }


        public async Task<bool> IsEmailIdExist(string EmailAddressEdit, string id)
        {
            bool status = false;
            try
            {


                var chk = Context.Users.Where(e => e.Email == EmailAddressEdit && e.Id == id).ToList();
                if (chk != null)
                {
                    if (chk.Count == 1)
                    {
                        status = true;
                    }
                    else
                    {
                        var chkEmail = Context.Users.Where(e => e.Email == EmailAddressEdit).ToList();
                        if (chkEmail != null)
                        {
                            if (chkEmail.Count == 0)
                            {
                                status = true;
                            }
                            else
                            {
                                status = false;
                            }
                        }
                    }

                }
                return status;
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return status;
        }
        #endregion


        #region Get all list of user

        public async Task<List<ApplicationUser>> GetUserData()
        {
            List<ApplicationUser> UserList = new List<ApplicationUser>();
            try
            {
                //UserList = Context.Users.Where(e => e.IsActive == true).ToList();
                UserList = Context.Users.ToList();
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return UserList;
        }

        public async Task<bool> GetUserDataByIdStatus(string Id)
        {
            bool Status = false;
            try
            {
                //UserList = Context.Users.Where(e => e.IsActive == true).ToList();
                Status = Context.Users.Where(i => i.Id == Id).Select(i => i.IsActive).FirstOrDefault();
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return Status;
        }

        public async Task<List<UserModel>> GetUserListingData(long IsActive, long OrganisationId, string CreatedDate, string textSearch)
        {
            List<UserModel> UserModelList = new List<UserModel>();

            try
            {
                bool temActive;
                var AllUserData = await GetUserData();
                var Organisationdata = await GetOrganisationList();
                if (AllUserData.Count() > 0)
                {
                    UserModelList = (from userRow in AllUserData
                                     select new UserModel
                                     {
                                         Id = userRow.Id,
                                         Name = userRow.Name + " " + userRow.LastName,
                                         EmailAddress = userRow.Email,
                                         OrganisationId = userRow.OrganisationId,
                                         Organisation = Organisationdata.Where(i => i.Id == userRow.OrganisationId).Select(i => i.OrganisationName).FirstOrDefault(),
                                         EmailType = returnEmailType(userRow.IsNotifyOnActiveTicket, userRow.IsNotifyOnUpdateTicket, userRow.IsNotifyOnCloseTicket),
                                         Status = userRow.IsActive == true ? "Active" : "Inactive",
                                         IsActive = userRow.IsActive == true ? true : false,
                                         AddedOn = userRow.CreatedOn.ToString("dd/MM/yyyy"), //.Value.ToString("dd/MM/yyyy"),
                                         CreatedOnDate = userRow.CreatedOn,
                                         DeactvatedOn = (userRow.LastDeactivatedOn.HasValue) ? Convert.ToString(Convert.ToDateTime(userRow.LastDeactivatedOn.ToString()).ToString("dd/MM/yyyy")) : "NA", //checkeDate(userRow.LastDeactivatedOn.HasValue)
                                     }).OrderByDescending(i => i.CreatedOnDate).ToList();



                    UserModelList = UserModelList
                       .WhereIf(OrganisationId > 0, x => x.OrganisationId == Convert.ToInt64(OrganisationId))
                       .WhereIf(textSearch != null, (x => (x.EmailAddress.ToLower().Trim().Contains(textSearch.ToLower().Trim())) || (x.Name.ToLower().Trim().Contains(textSearch.ToLower().Trim()))))
                       // .WhereIf(textSearch != null, x => x.Name.ToLower().Trim().Contains(textSearch.ToLower().Trim()))
                       .WhereIf(!string.IsNullOrEmpty(CreatedDate), x => x.CreatedOnDate.Date == Convert.ToDateTime(CreatedDate).Date).OrderByDescending(i => i.CreatedOnDate).ToList();
                    if (IsActive != 0)
                    {
                        if (IsActive == 1) { temActive = true; } else { temActive = false; }
                        UserModelList = UserModelList
                       .Where(x => x.IsActive == temActive).OrderByDescending(i => i.CreatedOnDate).ToList();
                    }
                }
            }
            catch (Exception ex)
            {

            }

            return UserModelList;
        }
        //public async Task<List<UserModel>> GetUserListingData()
        //{
        //    List<UserModel> UserModelList = new List<UserModel>();

        //    try
        //    {
        //        var AllUserData = await GetUserData();
        //        var Organisationdata = await GetOrganisationList();
        //        var ChannelList = await GetChannelMasterData();
        //        if (AllUserData.Count() > 0)
        //        {
        //            UserModelList = (from userRow in AllUserData
        //                             select new UserModel
        //                             {
        //                                 Id = userRow.Id,
        //                                 Name = userRow.Name,
        //                                 EmailAddress = userRow.Email,
        //                                 Organisation =  Organisationdata.Where(i => i.Id == userRow.OrganisationId).Select(i => i.OrganisationName).FirstOrDefault(),
        //                                 EmailType = returnEmailType(userRow.IsNotifyOnActiveTicket, userRow.IsNotifyOnUpdateTicket, userRow.IsNotifyOnCloseTicket),
        //                                 Status = userRow.IsActive==true?"Active":"Deactive",
        //                                 AddedOn = userRow.CreatedOn.ToString("dd/MM/yyyy"), //.Value.ToString("dd/MM/yyyy"),
        //                             }).ToList();
        //        }
        //    }
        //    catch (Exception ex)
        //    {

        //    }

        //    return UserModelList;
        //}
        #endregion

        public string returnEmailType(bool Active, bool Update, bool Close)
        {
            string strEmailType = "";
            if (Active)
            {
                strEmailType = "Open";
            }
            if (Update)
            {
                if (strEmailType == "")
                    strEmailType = "Update";
                else
                    strEmailType = strEmailType + " / Update";
            }
            if (Close)
            {
                if (strEmailType == "")
                    strEmailType = "Close";
                else
                    strEmailType = strEmailType + " / Close";
            }
            return strEmailType;
        }

        public async Task<TbTicketDocuments> GetDocumentById(int? DocumentId)
        {
            TbTicketDocuments Documents = new TbTicketDocuments();
            try
            {
                Documents = Context.TbTicketDocuments.Where(i => i.Id == DocumentId).FirstOrDefault();
            }
            catch (Exception ex)
            {
                string error = ex.Message;
            }
            return Documents;
        }

        public async Task<TbTicketResponseDocuments> GetDocumentFromResponseById(int? DocumentId)
        {
            TbTicketResponseDocuments Documents = new TbTicketResponseDocuments();
            try
            {
                Documents = Context.TbTicketResponseDocuments.Where(i => i.Id == DocumentId).FirstOrDefault();
            }
            catch (Exception ex)
            {
                string error = ex.Message;
            }
            return Documents;
        }

        #region To mark ticket as close

        public async Task<string> CloseTicketById(long Id, string UserId)
        {
            string IsSuccess = string.Empty;
            try
            {
                var TicketData = await GetTicketDataById(Id);

                if (TicketData != null)
                {
                    TicketData.StatusId = 2;
                    TicketData.ClosedBy = UserId;
                    TicketData.ClosedOn = DateTime.UtcNow;
                    Context.TbTicketMaster.Update(TicketData);
                    await Context.SaveChangesAsync();
                    IsSuccess = "True";
                }

                #region to send emails on ticket close


                string strTicketName = TicketData.TicketName;
                string strOrganisationName = Context.TbOrganisationMaster.Where(x => x.Id == TicketData.WaitingOnOrganisationId).Select(t => t.OrganisationName).FirstOrDefault();

                var GetAllUsers = await GetUserData();
                List<UserEmailValues> GetAllEmailAddress = new List<UserEmailValues>();
                GetAllEmailAddress = (from data in GetAllUsers
                                      where data.IsNotifyOnCloseTicket == true
                                      select new UserEmailValues
                                      {
                                          EmailAddress = data.Email,
                                          FullName = data.Name + " " + data.LastName

                                      }).ToList();


                //GetAllUsers.Where(i => i.IsNotifyOnCloseTicket == true).Select(i => i.Email).ToList();
                string FullName = GetAllUsers.Where(i => i.Id == UserId).Select(i => i.Name + " " + i.LastName).FirstOrDefault();
                long OrganisationId = GetAllUsers.Where(i => i.Id == UserId).Select(i => i.OrganisationId).FirstOrDefault();
                string OrganisationName = Context.TbOrganisationMaster.Where(x => x.Id == OrganisationId).Select(i => i.OrganisationName).FirstOrDefault();
                string EncryptedTicketId = _utility.EnryptString(Id.ToString());
                ConfirmEmail confirmEmail = new ConfirmEmail()
                {
                    UserEmailList = GetAllEmailAddress,
                    // Title = "HWLE First Response – Closed Ticket",
                    Title = "First response portal - Ticket closed",
                    EmailKey = "CloseTicket",
                    Url = iconfiguration["WebsiteURL"] + "ticketuser/TicketDetail?id=" + EncryptedTicketId,
                    TicketName = strTicketName,
                    OrganisationName = strOrganisationName,
                    TicketId = TicketData.Id,
                    Id = Convert.ToInt32(TicketData.Id),
                    UpdatedByName = FullName,
                    LoggedInOrganisation = OrganisationName,
                    WebsitePath = iconfiguration["WebsiteURL"]
                };

                await _utility.SentBulkMail(confirmEmail);

                #endregion


            }
            catch (Exception ex)
            {

            }
            return IsSuccess;
        }
        #endregion


        #region Fetch data for Channel

        public async Task<List<TbChannelMaster>> GetChannelMasterData()
        {

            List<TbChannelMaster> orgList = new List<TbChannelMaster>();
            try
            {
                orgList = Context.TbChannelMaster.Where(e => e.isActive == true).ToList();
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return orgList;
        }

        #endregion



        #region update verification application
        public async Task AddTicket(TicketDto MyTicketModel)
        {
            string strTicketName = "";
            string strOrganisationName = "";
            long TicketMasterId = 0;
            if (MyTicketModel.Id > 0)
            {
                try
                {
                    var TicketData = Context.TbTicketMaster.Where(x => x.Id == MyTicketModel.Id).FirstOrDefault();
                    if (TicketData != null)
                    {


                    }
                }
                catch (Exception ex)
                { }
            }
            else
            {
                // insert data
                try
                {
                    strTicketName = MyTicketModel.TicketName;
                    var varOrganisation = Context.TbOrganisationMaster.Where(x => x.Id == MyTicketModel.OrganisationId).Select(t => t.OrganisationName).FirstOrDefault();
                    strOrganisationName = varOrganisation;

                    TbTicketMaster TicketMaster = new TbTicketMaster();
                    TicketMaster.MatterNumber = MyTicketModel.MatterNumber;
                    TicketMaster.ChannelId = MyTicketModel.ChannelId;
                    TicketMaster.TicketName = MyTicketModel.TicketName;
                    TicketMaster.TicketNotes = MyTicketModel.TicketNotes;
                    TicketMaster.WaitingOnOrganisationId = MyTicketModel.OrganisationId;
                    TicketMaster.CreatedOn = DateTime.UtcNow;
                    TicketMaster.IsActive = true;
                    TicketMaster.StatusId = 1;
                    TicketMaster.CreatedByUserId = MyTicketModel.LoggedInUserId;
                    await Context.TbTicketMaster.AddAsync(TicketMaster);
                    await Context.SaveChangesAsync();
                    TicketMasterId = TicketMaster.Id;

                    string FileTitle = string.Empty;
                    #region Store document for ticket
                    if (TicketMasterId > 0)
                    {
                        if (MyTicketModel.Documents != null)
                        {
                            if (MyTicketModel.Documents[0] != null)
                            {
                                for (int i = 0; i < MyTicketModel.Documents.Length; i++)
                                {
                                    TbTicketDocuments documentsDto = new TbTicketDocuments();
                                    byte[] str = await ReadFileContent(MyTicketModel.Documents[i]);

                                    var filename = MyTicketModel.Documents[i].FileName.Split('\\');
                                    FileTitle = filename[filename.Length - 1];
                                    documentsDto.FileName = filename[filename.Length - 1];
                                    documentsDto.ContentType = MyTicketModel.Documents[i].ContentType;
                                    documentsDto.Extension = Path.GetExtension(MyTicketModel.Documents[i].FileName);
                                    documentsDto.TicketMasterId = TicketMasterId;
                                    documentsDto.IsActive = true;
                                    documentsDto.FileData = str;
                                    documentsDto.CreatedByUserId = MyTicketModel.LoggedInUserId;
                                    documentsDto.CreatedOn = DateTime.UtcNow;
                                    await Context.TbTicketDocuments.AddAsync(documentsDto);
                                    await Context.SaveChangesAsync();
                                }
                            }

                            try
                            {

                                _utility.AuditHistoryEntry(strOrganisationName + " user has added document " + FileTitle + " for new ticket " + MyTicketModel.TicketName + "", "Ticket/TicketUser/AddTicket", Convert.ToInt32(TicketMasterId), MyTicketModel.CreatedByUserId);

                            }
                            catch (Exception ex)
                            {

                            }
                        }
                    }
                    #endregion

                    #region to send email to all users who are allowed 

                    var GetAllUsers = await GetUserData();
                    List<UserEmailValues> GetAllEmailAddress = new List<UserEmailValues>();
                    GetAllEmailAddress = (from data in GetAllUsers
                                          where data.IsNotifyOnActiveTicket == true
                                          select new UserEmailValues
                                          {
                                              EmailAddress = data.Email,
                                              FullName = data.Name + " " + data.LastName
                                          }).ToList();


                    //GetAllUsers.Where(i => i.IsNotifyOnActiveTicket == true).Select(i => i.Email).ToList();
                    string OrganisationName = Context.TbOrganisationMaster.Where(x => x.Id == MyTicketModel.LoggedInOrganisationId).Select(i => i.OrganisationName).FirstOrDefault();
                    string FullUserName = GetAllUsers.Where(i => i.Id == MyTicketModel.LoggedInUserId).Select(i => i.Name + " " + i.LastName).FirstOrDefault();
                    string EncryptedTicketId = _utility.EnryptString(TicketMasterId.ToString());
                    ConfirmEmail confirmEmail = new ConfirmEmail()
                    {
                        UserEmailList = GetAllEmailAddress,
                        // Title = "HWLE First Response – New Ticket",
                        Title = "First response portal - Ticket creation",
                        EmailKey = "AddTicket",
                        Url = iconfiguration["WebsiteURL"] + "ticketuser/TicketDetail?id=" + EncryptedTicketId,
                        TicketName = strTicketName,
                        OrganisationName = strOrganisationName,
                        LoggedInOrganisation = OrganisationName,
                        Id = Convert.ToInt32(TicketMasterId),
                        UpdatedByName = FullUserName,
                        WebsitePath = iconfiguration["WebsiteURL"]

                    };

                    await _utility.SentBulkMail(confirmEmail);

                    #endregion

                    try
                    {
                        _utility.AuditHistoryEntry(strOrganisationName + " user has added new ticket " + MyTicketModel.TicketName + " from add ticket section", "Ticket/TicketUser/AddTicket", Convert.ToInt32(TicketMasterId), MyTicketModel.CreatedByUserId);

                    }
                    catch (Exception ex)
                    {

                    }
                }
                catch (Exception ex)
                { }
            }
        }

        public async Task SendEmailOnTicketCreation(int TicketId, string strTicketName, string strOrganisationName)
        {
            try
            {
                #region to send email to all users who are allowed 

                var GetAllUsers = await GetUserData();
                List<UserEmailValues> GetAllEmailAddress = new List<UserEmailValues>();
                GetAllEmailAddress = (from data in GetAllUsers
                                      where data.IsNotifyOnActiveTicket == true
                                      select new UserEmailValues
                                      {
                                          EmailAddress = data.Email,
                                          FullName = data.Name + " " + data.LastName

                                      }).ToList();//GetAllUsers.Where(i => i.IsNotifyOnActiveTicket == true).Select(i => i.Email).ToList();

                ConfirmEmail confirmEmail = new ConfirmEmail()
                {
                    UserEmailList = GetAllEmailAddress,
                    Title = "First response portal - Ticket creation",
                    EmailKey = "AddTicket",
                    Url = iconfiguration["WebsiteURL"],
                    TicketName = strTicketName,
                    OrganisationName = strOrganisationName,
                    WebsitePath = iconfiguration["WebsiteURL"]
                };

                await _utility.SentBulkMail(confirmEmail);
            }
            catch (Exception ex)
            {

            }

            #endregion
        }



        public async Task InsertOrUpdateTicket(TicketDto MyTicketModel)
        {
            string strTicketName = "";
            string strOrganisationName = "";
            long TicketMasterId = 0;
            if (MyTicketModel.Id > 0)
            {
                try
                {
                    var TicketData = Context.TbTicketMaster.Where(x => x.Id == MyTicketModel.Id).FirstOrDefault();
                    if (TicketData != null)
                    {
                        TicketData.TicketName = MyTicketModel.TicketName;
                        TicketData.MatterNumber = MyTicketModel.MatterNumber;
                        TicketData.ChannelId = MyTicketModel.ChannelId;
                        TicketData.WaitingOnOrganisationId = MyTicketModel.OrganisationId;
                        Context.TbTicketMaster.Update(TicketData);
                        await Context.SaveChangesAsync();
                    }
                }
                catch (Exception ex)
                { }
            }
            else
            {
                // insert data
                try
                {
                    strTicketName = MyTicketModel.TicketName;
                    var varOrganisation = Context.TbOrganisationMaster.Where(x => x.Id == MyTicketModel.OrganisationId).Select(t => t.OrganisationName).FirstOrDefault();
                    strOrganisationName = varOrganisation;

                    TbTicketMaster TicketMaster = new TbTicketMaster();
                    TicketMaster.MatterNumber = MyTicketModel.MatterNumber;
                    TicketMaster.ChannelId = MyTicketModel.ChannelId;
                    TicketMaster.TicketName = MyTicketModel.TicketName;
                    TicketMaster.TicketNotes = MyTicketModel.TicketNotes;
                    TicketMaster.WaitingOnOrganisationId = MyTicketModel.OrganisationId;
                    TicketMaster.CreatedOn = DateTime.UtcNow;
                    TicketMaster.IsActive = true;
                    TicketMaster.StatusId = 1;
                    TicketMaster.CreatedByUserId = MyTicketModel.LoggedInUserId;
                    await Context.TbTicketMaster.AddAsync(TicketMaster);
                    await Context.SaveChangesAsync();
                    TicketMasterId = TicketMaster.Id;

                    #region Store document for ticket
                    if (TicketMasterId > 0)
                    {
                        if (MyTicketModel.Documents != null)
                        {
                            if (MyTicketModel.Documents[0] != null)
                            {
                                for (int i = 0; i < MyTicketModel.Documents.Length; i++)
                                {
                                    TbTicketDocuments documentsDto = new TbTicketDocuments();
                                    byte[] str = await ReadFileContent(MyTicketModel.Documents[i]);

                                    var filename = MyTicketModel.Documents[i].FileName.Split('\\');
                                    documentsDto.FileName = filename[filename.Length - 1];
                                    documentsDto.ContentType = MyTicketModel.Documents[i].ContentType;
                                    documentsDto.Extension = Path.GetExtension(MyTicketModel.Documents[i].FileName);
                                    documentsDto.TicketMasterId = TicketMasterId;
                                    documentsDto.IsActive = true;
                                    documentsDto.FileData = str;
                                    documentsDto.CreatedByUserId = MyTicketModel.LoggedInUserId;
                                    await Context.TbTicketDocuments.AddAsync(documentsDto);
                                    await Context.SaveChangesAsync();
                                }
                            }
                        }
                    }
                    #endregion

                    #region to send email to all users who are allowed 

                    var GetAllUsers = await GetUserData();
                    List<UserEmailValues> GetAllEmailAddress = new List<UserEmailValues>();
                    GetAllEmailAddress = (from data in GetAllUsers
                                          where data.IsNotifyOnActiveTicket == true
                                          select new UserEmailValues
                                          {
                                              EmailAddress = data.Email,
                                              FullName = data.Name + " " + data.LastName

                                          }).ToList();
                    //GetAllUsers.Where(i => i.IsNotifyOnActiveTicket == true).Select(i => i.Email).ToList();

                    ConfirmEmail confirmEmail = new ConfirmEmail()
                    {
                        UserEmailList = GetAllEmailAddress,
                        Title = "First response portal - Ticket creation",
                        EmailKey = "AddTicket",
                        Url = iconfiguration["WebsiteURL"],
                        TicketName = strTicketName,
                        OrganisationName = strOrganisationName,
                        WebsitePath = iconfiguration["WebsiteURL"]
                    };

                    await _utility.SentBulkMail(confirmEmail);

                    #endregion
                }
                catch (Exception ex)
                { }
            }
        }

        #endregion

        #region get input file stream data
        public async Task<byte[]> ReadFileContent(IFormFile file)
        {
            var Content = new byte[1];
            try
            {
                using (var memoryStream = new MemoryStream())
                {
                    await file.CopyToAsync(memoryStream);

                    // Upload the file if less than 2 MB

                    Content = memoryStream.ToArray();

                }
            }
            catch (Exception ex)
            {

            }
            return Content;
        }
        #endregion


        #region Get Ticket Master Data


        public async Task<List<TbTicketMaster>> GetTicketData()
        {
            List<TbTicketMaster> TicketList = new List<TbTicketMaster>();
            try
            {
                TicketList = Context.TbTicketMaster.Where(e => e.IsActive == true).ToList();
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return TicketList;
        }



        #endregion

        #region Get Ticket Master Data by Id


        public async Task<TbTicketMaster> GetTicketDataById(long Id)
        {
            TbTicketMaster TicketList = new TbTicketMaster();
            try
            {
                TicketList = Context.TbTicketMaster.Where(e => e.IsActive == true && e.Id == Id).FirstOrDefault();
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return TicketList;
        }



        #endregion


        #region Get Ticket documents Data by Id


        public async Task<TbTicketDocuments> GetTicketDocuments(long TicketId)
        {
            TbTicketDocuments TicketList = new TbTicketDocuments();
            try
            {
                TicketList = Context.TbTicketDocuments.Where(e => e.IsActive == true && e.IsDeleted == false && e.TicketMasterId == TicketId).FirstOrDefault();
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return TicketList;
        }



        #endregion

        #region Get Response documents Data by Ticket Id 
        public async Task<List<TbTicketResponse>> GetResponseDocumentByTicketId(long TicketId)
        {
            List<TbTicketResponse> TicketList = new List<TbTicketResponse>();
            try
            {
                TicketList = Context.TbTicketResponse.Where(e => e.IsActive == true && e.TicketMasterId == TicketId).ToList();
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return TicketList;
        }
        #endregion

        #region Get Response documents Data by Ticket Id 
        public async Task<List<TbTicketResponseDocuments>> GetResponseDocumentsByTicketId(long TicketId)
        {
            List<TbTicketResponseDocuments> TicketList = new List<TbTicketResponseDocuments>();
            try
            {
                TicketList = Context.TbTicketResponseDocuments.Where(e => e.IsActive == true && e.TicketMasterId == TicketId && e.IsDeleted == false).ToList();
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return TicketList;
        }
        #endregion

        #region Get Response documents Data by Ticket Id 
        public async Task<TbTicketResponseDocuments> GetDocumentResponseById(long Id)
        {
            TbTicketResponseDocuments TicketList = new TbTicketResponseDocuments();
            try
            {
                TicketList = Context.TbTicketResponseDocuments.Where(e => e.Id == Id).FirstOrDefault();
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return TicketList;
        }
        #endregion



        #region Get Ticket Master Data

        public async Task<List<EmailTemplates>> GetEmailtemplates()
        {
            List<EmailTemplates> TicketList = new List<EmailTemplates>();
            try
            {
                TicketList = Context.EmailTemplates.ToList();

            }
            catch (Exception ex)
            {
                throw ex;
            }

            return TicketList;
        }

        //public async Task<List<EmailTemplates>> GetEmailTemplates()
        //{
        //    List<EmailTemplates> TicketList = new List<EmailTemplates>();
        //    try
        //    {
        //        TicketList = Context.EmailTemplates.ToList();

        //       return EmailTemplates;
        //}

        #endregion

        #region Bind data for Ticket Listing grid

        public async Task<List<TicketModel>> GetActiveListingData(long ChannelId, long OrganisationId, string CreatedDate, string textSearch)
        {
            List<TicketModel> TicketModelList = new List<TicketModel>();

            try
            {
                var AllTicketData = await GetTicketData();
                var Organisationdata = await GetOrganisationList();
                var ChannelList = await GetChannelMasterData();
                var UserData = await GetUserData();

                TicketModelList = (from ticket in AllTicketData
                                   where ticket.StatusId == 1
                                   select new TicketModel
                                   {
                                       Id = ticket.Id,
                                       Name = ticket.TicketName,
                                       Channel = ChannelList.Where(i => i.Id == ticket.ChannelId).Select(i => i.ChannelName).FirstOrDefault(),
                                       WaitingOn = Organisationdata.Where(i => i.Id == ticket.WaitingOnOrganisationId).Select(i => i.OrganisationName).FirstOrDefault(),
                                       AddedOn = ticket.CreatedOn.Value.ToString("dd/MM/yyyy"),
                                       AddedBy = UserData.Where(i => i.Id == ticket.CreatedByUserId).Select(i => i.Name + " " + i.LastName).FirstOrDefault(),
                                       ChannelId = ticket.ChannelId,
                                       OrganisationId = ticket.WaitingOnOrganisationId,
                                       CreatedOnDate = ticket.CreatedOn.Value,
                                       EncryptedTicketId = _utility.EnryptString(ticket.Id.ToString()),
                                   }).ToList();

                bool? IsContainsDigit = null;
                if (!string.IsNullOrEmpty(textSearch))
                {

                    bool ContainsDigit = textSearch.Trim().All(char.IsDigit);

                    if (ContainsDigit)
                    {
                        IsContainsDigit = true;
                    }
                    else
                    {
                        IsContainsDigit = false;
                    }
                }


                TicketModelList = TicketModelList
                .WhereIf(ChannelId > 0, x => x.ChannelId == Convert.ToInt64(ChannelId))
                .WhereIf(OrganisationId > 0, x => x.OrganisationId == Convert.ToInt64(OrganisationId))
                .WhereIf(!string.IsNullOrEmpty(CreatedDate), x => x.CreatedOnDate.Date == Convert.ToDateTime(CreatedDate).Date)
                .WhereIf(IsContainsDigit == false, x => x.Name.ToLower().Trim().Contains(textSearch.ToLower().Trim()))
                //.ToLower().Trim().Contains(textSearch.ToLower().Trim()
                .WhereIf(IsContainsDigit == true, x => x.Id == Convert.ToInt64(textSearch.Trim())).OrderByDescending(i => i.Id).ToList();
                //if (!string.IsNullOrEmpty(textSearch))
                //{
                //    IsContainsDigit = false;
                //    bool ContainsDigit = textSearch.Any(char.IsDigit);

                //    if(ContainsDigit)
                //    {
                //        IsContainsDigit = true;
                //    }
                //}


                //TicketModelList = TicketModelList
                //                .WhereIf(ChannelId > 0, x => x.ChannelId == Convert.ToInt64(ChannelId))
                //                .WhereIf(OrganisationId > 0, x => x.OrganisationId == Convert.ToInt64(OrganisationId))
                //               .WhereIf(!string.IsNullOrEmpty(CreatedDate), x=>x.CreatedOnDate.Date== Convert.ToDateTime(CreatedDate).Date)
                //               .WhereIf(IsContainsDigit == false, x => x.Name.ToLower().Trim().Contains(textSearch.ToLower().Trim()))
                //               .WhereIf(IsContainsDigit==true, x => x.Id== Convert.ToInt64(textSearch)).OrderByDescending(i=>i.Id).ToList();



            }
            catch (Exception ex)
            {

            }

            return TicketModelList;
        }

        #endregion


        #region Get DocumentData by TicketId
        public async Task<List<DocumentListing>> GetDocumentListingData(long TicketId, string UserId)
        {
            List<DocumentListing> TicketModelList = new List<DocumentListing>();
            DocumentListing Ticket = new DocumentListing();
            var Organisationdata = await GetOrganisationList();

            UserSessionDetail temSession = new UserSessionDetail();


            try
            {
                var AllTicketData = await GetTicketDataById(TicketId);
                var DocumentData = (from response in Context.TbTicketResponseDocuments
                                    where response.IsActive == true && response.TicketMasterId == TicketId && response.IsDeleted == false
                                    select new TbTicketResponseDocuments
                                    {

                                        Id = response.Id,
                                        TicketMasterId = response.TicketMasterId,
                                        TicketResponseId = response.TicketResponseId,
                                        FileName = response.FileName,
                                        Extension = response.Extension,
                                        ContentType = response.ContentType,
                                        Link = response.Link,
                                        CreatedByUserId = response.CreatedByUserId,
                                        IsActive = response.IsActive,
                                        CreatedOn = response.CreatedOn,
                                        IsDeleted = response.IsDeleted,
                                        DeletedBy = response.DeletedBy
                                    }).ToList();

                // await GetResponseDocumentsByTicketId(TicketId);

                var ResponseData = await GetResponseDocumentByTicketId(TicketId);
                var TicketDocuments = (from document in Context.TbTicketDocuments
                                       where document.IsActive == true && document.IsDeleted == false && document.TicketMasterId == TicketId
                                       select new TbTicketDocuments
                                       {
                                           Id = document.Id,
                                           TicketMasterId = document.TicketMasterId,
                                           FileName = document.FileName,
                                           Extension = document.Extension,
                                           ContentType = document.ContentType,
                                           Link = document.Link,
                                           CreatedByUserId = document.CreatedByUserId,
                                           IsActive = document.IsActive,
                                           IsDeleted = document.IsDeleted,
                                           DeletedBy = document.DeletedBy,
                                           CreatedOn = document.CreatedOn
                                       }).FirstOrDefault();

                //await GetTicketDocuments(TicketId);
                var UserData = await GetUserData();

                #region commented code
                //TicketModelList = (from response in DocumentData
                //                   join masterresponse in ResponseData on response.TicketResponseId equals masterresponse.Id
                //                   select new DocumentListing
                //                   {
                //                       DocumentName= response.FileName,
                //                       AddedBy= UserData.Where(i=>i.Id== response.CreatedByUserId).Select(i=>i.Name+" "+i.LastName).FirstOrDefault(),
                //                       AddedOn= response.CreatedOn.ToString("dd/MM/yyyy"),
                //                       Id = response.Id,
                //                       Organisation= Organisationdata.Where(i => i.Id == masterresponse.OrganisationId).Select(i => i.OrganisationName).FirstOrDefault(),
                //                       TableType="ticketresponse",
                //                       CreatedByUserId= response.CreatedByUserId,
                //                       IsCreatedByLoggedInuser= response.CreatedByUserId == UserId?"true":"false"

                //                   }).ToList();

                //if(TicketDocuments!=null)
                //{
                //    Ticket.AddedBy =  UserData.Where(i => i.Id == AllTicketData.CreatedByUserId).Select(i => i.Name + " " + i.LastName).FirstOrDefault();
                //    Ticket.Organisation = Organisationdata.Where(i => i.Id == AllTicketData.WaitingOnOrganisationId).Select(i => i.OrganisationName).FirstOrDefault();
                //    Ticket.AddedOn =  AllTicketData.CreatedOn.Value.ToString("dd/MM/yyyy");
                //    Ticket.DocumentName = TicketDocuments.FileName;
                //    Ticket.Id = TicketDocuments.Id;
                //    Ticket.TableType = "ticketdocument";
                //    Ticket.CreatedByUserId = TicketDocuments.CreatedByUserId;
                //    Ticket.IsCreatedByLoggedInuser = TicketDocuments.CreatedByUserId == UserId ? "true" : "false";
                //    TicketModelList.Add(Ticket);
                //}
                #endregion

                TicketModelList = (from response in DocumentData
                                   join masterresponse in ResponseData on response.TicketResponseId equals masterresponse.Id
                                   select new DocumentListing
                                   {
                                       DocumentName = response.FileName,
                                       AddedBy = UserData.Where(i => i.Id == response.CreatedByUserId).Select(i => i.Name + " " + i.LastName).FirstOrDefault(),
                                       AddedOn = response.CreatedOn.ToString("dd/MM/yyyy"),
                                       Id = response.Id,
                                       Organisation = Organisationdata.Where(i => i.Id == masterresponse.OrganisationId).Select(i => i.OrganisationName).FirstOrDefault(),
                                       TableType = "ticketresponse",
                                       CreatedByUserId = response.CreatedByUserId,
                                       IsCreatedByLoggedInuser = response.CreatedByUserId == UserId ? "true" : "false"

                                   }).ToList();

                if (TicketDocuments != null)
                {
                    Ticket.AddedBy = UserData.Where(i => i.Id == AllTicketData.CreatedByUserId).Select(i => i.Name + " " + i.LastName).FirstOrDefault();
                    var tem = UserData.Where(i => i.Id == AllTicketData.CreatedByUserId).Select(i => i.OrganisationId).FirstOrDefault();
                    Ticket.Organisation = Organisationdata.Where(i => i.Id == tem).Select(i => i.OrganisationName).FirstOrDefault();
                    Ticket.AddedOn = AllTicketData.CreatedOn.Value.ToString("dd/MM/yyyy");
                    Ticket.DocumentName = TicketDocuments.FileName;
                    Ticket.Id = TicketDocuments.Id;
                    Ticket.TableType = "ticketdocument";
                    Ticket.CreatedByUserId = TicketDocuments.CreatedByUserId;
                    Ticket.IsCreatedByLoggedInuser = TicketDocuments.CreatedByUserId == UserId ? "true" : "false";
                    TicketModelList.Add(Ticket);
                }
            }
            catch (Exception ex)
            {

            }
            return TicketModelList;
        }

        #endregion

        #region Select Document without filedata

        public TbTicketDocuments GetTicketDocumentsSkipFileData(long Id)
        {
            TbTicketDocuments TicketDoc = new TbTicketDocuments();
            try
            {
                TicketDoc = (from document in Context.TbTicketDocuments
                             where document.Id == Id
                             select new TbTicketDocuments
                             {
                                 Id = document.Id,
                                 TicketMasterId = document.TicketMasterId,
                                 FileName = document.FileName,
                                 Extension = document.Extension,
                                 ContentType = document.ContentType,
                                 Link = document.Link,
                                 CreatedByUserId = document.CreatedByUserId,
                                 IsActive = document.IsActive,
                                 IsDeleted = document.IsDeleted,
                                 DeletedBy = document.DeletedBy,
                                 CreatedOn = document.CreatedOn
                             }).FirstOrDefault();
            }
            catch (Exception ex)
            {

            }

            return TicketDoc;
        }

        #endregion


        #region Select Document without filedata from response documents table

        public TbTicketResponseDocuments GetTicketResponseDocumentsSkipFileData(long Id)
        {
            TbTicketResponseDocuments TicketDoc = new TbTicketResponseDocuments();
            try
            {
                TicketDoc = (from response in Context.TbTicketResponseDocuments
                             where  response.Id == Id 
                             select new TbTicketResponseDocuments
                             {
                                 Id = response.Id,
                                 TicketMasterId = response.TicketMasterId,
                                 TicketResponseId = response.TicketResponseId,
                                 FileName = response.FileName,
                                 Extension = response.Extension,
                                 ContentType = response.ContentType,
                                 Link = response.Link,
                                 CreatedByUserId = response.CreatedByUserId,
                                 IsActive = response.IsActive,
                                 CreatedOn = response.CreatedOn,
                                 IsDeleted = response.IsDeleted,
                                 DeletedBy = response.DeletedBy
                             }).FirstOrDefault();
            }
            catch (Exception ex)
            {

            }

            return TicketDoc;
        }

        #endregion

        #region Bind data for Ticket Listing grid search filter

        public async Task<List<TicketModel>> GetActiveListingDataBySearch(int? ChannelId, int? OrganisationId, string CreatedOn, string SearchText)
        {
            List<TicketModel> TicketModelList = new List<TicketModel>();

            try
            {
                var AllTicketData = await GetTicketData();
                var Organisationdata = await GetOrganisationList();
                var ChannelList = await GetChannelMasterData();
                var AllUser = await GetUserData();

                TicketModelList = (from ticket in AllTicketData
                                   select new TicketModel
                                   {
                                       Id = ticket.Id,
                                       Name = ticket.TicketName,
                                       Channel = ChannelList.Where(i => i.Id == ticket.ChannelId).Select(i => i.ChannelName).FirstOrDefault(),
                                       WaitingOn = Organisationdata.Where(i => i.Id == ticket.WaitingOnOrganisationId).Select(i => i.OrganisationName).FirstOrDefault(),
                                       AddedOn = ticket.CreatedOn.Value.ToString("dd/MM/yyyy"),
                                       AddedBy = AllUser.Where(i => i.Id == ticket.CreatedByUserId).Select(i => i.Name + " " + i.LastName).FirstOrDefault(),
                                       ChannelId = ticket.ChannelId,
                                       OrganisationId = ticket.WaitingOnOrganisationId
                                   }).ToList();


            }
            catch (Exception ex)
            {

            }

            return TicketModelList;
        }

        #endregion

        #region Bind data for closed Ticket Listing grid

        public async Task<List<TicketModel>> GetClosedTicketListingData(long ChannelId, long OrganisationId, string CreatedDate, string textSearch)
        {
            List<TicketModel> TicketModelList = new List<TicketModel>();

            try
            {
                var AllUser = await GetUserData();
                var AllTicketData = await GetTicketData();
                var Organisationdata = await GetOrganisationList();
                var ChannelList = await GetChannelMasterData();
                var UserData = await GetUserData();

                TicketModelList = (from ticket in AllTicketData
                                   where ticket.StatusId == 2
                                   select new TicketModel
                                   {
                                       Id = ticket.Id,
                                       Name = ticket.TicketName,
                                       Channel = ChannelList.Where(i => i.Id == ticket.ChannelId).Select(i => i.ChannelName).FirstOrDefault(),
                                       WaitingOn = Organisationdata.Where(i => i.Id == ticket.WaitingOnOrganisationId).Select(i => i.OrganisationName).FirstOrDefault(),
                                       AddedOn = ticket.CreatedOn.Value.ToString("dd/MM/yyyy"),
                                       AddedBy = AllUser.Where(i => i.Id == ticket.CreatedByUserId).Select(i => i.Name + " " + i.LastName).FirstOrDefault(),
                                       //"test user",
                                       ChannelId = ticket.ChannelId,
                                       OrganisationId = ticket.WaitingOnOrganisationId,
                                       CreatedOnDate = ticket.CreatedOn.Value,
                                       ClosedOn = ticket.ClosedOn.Value.ToString("dd/MM/yyyy"),
                                       ClosedBy = UserData.Where(i => i.Id == ticket.ClosedBy).Select(i => i.Name + " " + i.LastName).FirstOrDefault(),
                                       EncryptedTicketId = _utility.EnryptString(ticket.Id.ToString()),
                                   }).ToList();

                bool? IsContainsDigit = null;
                if (!string.IsNullOrEmpty(textSearch))
                {

                    bool ContainsDigit = textSearch.Trim().All(char.IsDigit);

                    if (ContainsDigit)
                    {
                        IsContainsDigit = true;
                    }
                    else
                    {
                        IsContainsDigit = false;
                    }
                }


                TicketModelList = TicketModelList
                                .WhereIf(ChannelId > 0, x => x.ChannelId == Convert.ToInt64(ChannelId))
                                .WhereIf(OrganisationId > 0, x => x.OrganisationId == Convert.ToInt64(OrganisationId))
                               .WhereIf(!string.IsNullOrEmpty(CreatedDate), x => x.CreatedOnDate.Date == Convert.ToDateTime(CreatedDate).Date)
                               .WhereIf(IsContainsDigit == false, x => x.Name.ToLower().Trim().Contains(textSearch.ToLower().Trim()))
                               .WhereIf(IsContainsDigit == true, x => x.Id == Convert.ToInt64(textSearch.Trim())).OrderByDescending(i => i.Id).ToList();

            }
            catch (Exception ex)
            {

            }

            return TicketModelList;
        }

        #endregion


        #region  Update ticket details data


        public async Task<bool> UpdateTicketResponse(TicketDto Model)
        {

            var organisationData = await GetOrganisationList();
            var HWLEId = organisationData.Where(i => i.OrganisationName.ToLower().Trim() == "hwle").Select(i => i.Id).FirstOrDefault();
            var MedilawId = organisationData.Where(i => i.OrganisationName.ToLower().Trim() == "medilaw").Select(i => i.Id).FirstOrDefault();
            var LinFoxId = organisationData.Where(i => i.OrganisationName.ToLower().Trim() == "linfox").Select(i => i.Id).FirstOrDefault();
            bool IsSuccess = false;
            try
            {
                TbTicketResponse Response = new TbTicketResponse();

                Response.TicketMasterId = Model.Id;
                if (Model.WaitingOn > 0)
                {
                    Response.WaitingOnOrganisationId = Model.WaitingOn;
                }

                if (Model.LoggedInOrganisationId == HWLEId)
                {
                    Response.HWLENotes = Model.HWLEText;
                }
                else if (Model.LoggedInOrganisationId == MedilawId)
                {
                    Response.MedilawNotes = Model.MedilawText;
                }
                else if (Model.LoggedInOrganisationId == LinFoxId)
                {
                    Response.LinfoxNotes = Model.Linfoxtext;
                }
                Response.CreatedByUserId = Model.LoggedInUserId;
                Response.CreatedOn = DateTime.UtcNow;
                Response.LastUpdatedOn = DateTime.UtcNow;
                Response.OrganisationId = Model.LoggedInOrganisationId;
                Response.IsActive = true;
                await Context.TbTicketResponse.AddAsync(Response);
                await Context.SaveChangesAsync();
                long ResponseId = Response.Id;

                var TicketData = Context.TbTicketMaster.Where(x => x.Id == Model.Id).FirstOrDefault();
                string strTicketName = TicketData.TicketName;
                var varOrganisation = Context.TbOrganisationMaster.Where(x => x.Id == TicketData.WaitingOnOrganisationId).Select(t => t.OrganisationName).FirstOrDefault();
                string strOrganisationName = varOrganisation;
                string FileTitle = string.Empty;

                if (Model.Documents != null)
                {
                    if (Model.Documents[0] != null)
                    {
                        TbTicketResponseDocuments ResponseDoc = new TbTicketResponseDocuments();
                        for (int i = 0; i < Model.Documents.Length; i++)
                        {
                            #region Store document for ticket

                            byte[] str = await ReadFileContent(Model.Documents[i]);

                            var filename = Model.Documents[i].FileName.Split('\\');
                            FileTitle = filename[filename.Length - 1];
                            ResponseDoc.FileName = filename[filename.Length - 1];
                            ResponseDoc.ContentType = Model.Documents[i].ContentType;
                            ResponseDoc.Extension = Path.GetExtension(Model.Documents[i].FileName);
                            ResponseDoc.TicketMasterId = Model.Id;
                            ResponseDoc.IsActive = true;
                            ResponseDoc.FileData = str;
                            ResponseDoc.TicketMasterId = Model.Id;
                            ResponseDoc.TicketResponseId = ResponseId;
                            ResponseDoc.CreatedOn = DateTime.UtcNow;
                            ResponseDoc.CreatedByUserId = Model.LoggedInUserId;
                            await Context.TbTicketResponseDocuments.AddAsync(ResponseDoc);
                            await Context.SaveChangesAsync();
                            #endregion

                        }

                        try
                        {

                            _utility.AuditHistoryEntry(strOrganisationName + " user has added document " + FileTitle + " for ticket " + TicketData.TicketName + "", "Ticket/TicketUser/TicketDetail", Convert.ToInt32(Model.Id), Model.LoggedInUserId);

                        }
                        catch (Exception ex)
                        {

                        }
                    }
                }


                if (TicketData != null)
                {
                    TicketData.WaitingOnOrganisationId = Model.WaitingOn;
                    TicketData.MatterNumber = Model.MatterNumber;
                    Context.TbTicketMaster.Update(TicketData);
                    await Context.SaveChangesAsync();
                }

                #region to send email to all users who are allowed 




                var GetAllUsers = await GetUserData();
                List<UserEmailValues> GetAllEmailAddress = new List<UserEmailValues>();
                GetAllEmailAddress = (from data in GetAllUsers
                                      where data.IsNotifyOnUpdateTicket == true
                                      select new UserEmailValues
                                      {
                                          EmailAddress = data.Email,
                                          FullName = data.Name + " " + data.LastName
                                      }).ToList();

                //GetAllUsers.Where(i => i.IsNotifyOnUpdateTicket == true).Select(i => i.Email).ToList();
                string FullName = GetAllUsers.Where(i => i.Id == Model.LoggedInUserId).Select(i => i.Name + " " + i.LastName).FirstOrDefault();

                string OrganisationName = Context.TbOrganisationMaster.Where(x => x.Id == Model.LoggedInOrganisationId).Select(i => i.OrganisationName).FirstOrDefault();
                string EncryptedTicketId = _utility.EnryptString(Model.Id.ToString());

                ConfirmEmail confirmEmail = new ConfirmEmail()
                {
                    UserEmailList = GetAllEmailAddress,
                    //Title = "HWLE First Response – Updated Ticket",
                    Title = "First response portal - Ticket update",
                    EmailKey = "UpdateTicket",
                    Id = Convert.ToInt32(Model.Id),
                    Url = iconfiguration["WebsiteURL"] + "ticketuser/TicketDetail?id=" + EncryptedTicketId,
                    TicketName = strTicketName,
                    OrganisationName = strOrganisationName,
                    LoggedInOrganisation = OrganisationName,
                    UpdatedByName = FullName,
                    WebsitePath = iconfiguration["WebsiteURL"]

                };

                await _utility.SentBulkMail(confirmEmail);

                #endregion

                IsSuccess = true;

                try
                {
                    _utility.AuditHistoryEntry(strOrganisationName + " user has updated ticket details for" + TicketData.TicketName + " from update ticket details section", "Ticket/TicketUser/TicketDetail", Convert.ToInt32(Model.Id), Model.LoggedInUserId);

                }
                catch (Exception ex)
                {

                }
            }
            catch (Exception ex)
            {


            }
            return IsSuccess;
        }



        #endregion


        #region Delete documeny by id and table type


        public async Task<string> DeleteDocumentByTableId(long Id, string TableType, string UserId)
        {
            string IsSucess = string.Empty;
            try
            {
                if (TableType == "ticketdocument")
                {
                    var TicketDoc = GetTicketDocumentsSkipFileData(Id);
                   // await GetDocumentById(Convert.ToInt32(Id));
                    if (TicketDoc != null)
                    {
                        TicketDoc.IsDeleted = true;
                        TicketDoc.DeletedBy = UserId;
                        TicketDoc.IsActive = false;
                        Context.TbTicketDocuments.Update(TicketDoc);
                        await Context.SaveChangesAsync();
                        IsSucess = "True";
                    }
                }
                else if (TableType == "ticketresponse")
                {
                    var TicketDoc = GetTicketResponseDocumentsSkipFileData(Id);
                        //await GetDocumentResponseById(Id);
                    if (TicketDoc != null)
                    {
                        TicketDoc.IsDeleted = true;
                        TicketDoc.DeletedBy = UserId;
                        TicketDoc.IsActive = false;
                        Context.TbTicketResponseDocuments.Update(TicketDoc);
                        await Context.SaveChangesAsync();
                        IsSucess = "True";
                    }
                }
            }
            catch (Exception ex)
            {

            }

            return IsSucess;
        }



        #endregion

    }
}

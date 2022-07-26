using FirstResponseApp.Areas.Ticket.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FirstResponseApp.Models.UtilityHelper
{
    public class UtilityDto
    {
        public class ConfirmEmail
        {
            public int Id { get; set; }
            public string First_Name { get; set; }
            public string Last_Name { get; set; }
            public string UpdatedByName { get; set; }
            public string Title { get; set; }
            public string Email_To { get; set; }
            public string Password { get; set; }
            public string LoggedInOrganisation { get; set; }
            public string Email_From { get; set; }
            public string Subject { get; set; }
            public string Url { get; set; }
            public string ApproverName { get; set; }
            public string EmailKey { get; set; }

            public string WebsitePath { get; set; }
            public string Status { get; set; }
            public int? ApplicationCreatorId { get; set; }
            public string EncodedViewApplicationpath { get; set; }

            public string TicketName { get; set; }

            public long TicketId { get; set; }

            public string OrganisationName { get; set; }
            public string MatterNumber { get; set; }

            public List<UserEmailValues> UserEmailList { get; set; }
        }

        public class smtpSetting
        {

            public string DefaultFromAddress { get; set; }
            public string DefaultFromDisplayName { get; set; }
            public string SmtpHost { get; set; }
            public string SmtpPort { get; set; }
            public string SmtpPassword { get; set; }
            public string SmtpEnableSsl { get; set; }
            public string SmtpUserName { get; set; }


        }

        public class UsersData
        {
            public string FirstName { get; set; }
            public string LastName { get; set; }
            public string SAMAccountName { get; set; }
            public string UserPrincipalName { get; set; }
            public string Email { get; set; }
            public string UserName { get; set; }
            public string DisplayName { get; set; }
            public string Title { get; set; }
            public bool isMapped { get; set; }
            public bool isAccountExpire { get; set; }
            public bool isAccountDisable { get; set; }
        }
    }
}

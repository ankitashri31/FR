using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace FirstResponseApp.Areas.Ticket.Models
{
    public class UserDto 
    {
        public string Id { get; set; }

        [Required]
        [Display(Name = "First Name")]
        [MaxLength(30)]
        public string Name { get; set; }

        [Required]
        [Display(Name = "Last Name")]
        [MaxLength(30)]
        public string LastName { get; set; }

        [Required]
        [Display(Name = "Email Address")]
        [Remote("UserAlreadyExistsAsync11", "TicketUser", ErrorMessage = "Email id already exists")]
        [RegularExpression(@"^([a-zA-Z0-9_\-\.]+)@((\[[0-9]{1,3}" +
                            @"\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([a-zA-Z0-9\-]+\" +
                            @".)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$",
                            ErrorMessage = "Email is not valid")]
        [MaxLength(70)]
        public string EmailAddress { get; set; }

        [Required]
        [Display(Name = "Email Address")]
        [Remote("UserAlreadyExistsAsyncEdit", "TicketUser", AdditionalFields= "Id", ErrorMessage = "Email id already exists")]
        [RegularExpression(@"^([a-zA-Z0-9_\-\.]+)@((\[[0-9]{1,3}" +
                            @"\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([a-zA-Z0-9\-]+\" +
                            @".)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$",
                            ErrorMessage = "Email is not valid")]
        [MaxLength(70)]
        public string EmailAddressEdit { get; set; }

        [Required]
        [Display(Name = "Notification Condition")]
        public string NotificationCondition { get; set; }

        [Required]
        [Display(Name = "Organization")]
        public string OrganisationId { get; set; }
        public List<SelectListItem> OrganizationList { get; set; }
        public string OrganisationName { get; set; }

        public bool IsNotifyOnActive { get; set; }
        public bool IsNotifyOnUpdate { get; set; }
        public bool IsNotifyOnClose { get; set; }

        public DateTime? LastLogOnDateTime { get; set; }

        public DateTime? LastUpdatedOn { get; set; }
        public string CreatedBy { get; set; }

        public DateTime CreatedOn { get; set; }
        public string CreatedOnShow { get; set; }
        public bool IsDeleted { get; set; }
        public string DeletedBy { get; set; }

        [Display(Name = "Status")]
        public bool IsActive { get; set; }
        public List<SelectListItem> IsActiveList { get; set; }

        [Display(Name = "Search Date")]
        public string SearchDate { get; set; }
        public string Search { get; set; }


        [Required(ErrorMessage = "You must select at least one Landscape Application!")]
        public ICollection<string> LandscapeServices { get; set; }

        public string LoginOrganisationId { get; set; }

        public string UserName { get; set; }
    }
}

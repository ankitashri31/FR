using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace FirstResponseApp.Helper
{
    public class User
    {
        public long Id { get; set; }

        [Required]

        [Display(Name = "First Name")]
        public string Name { get; set; }

        [Display(Name = "Last Name")]
        public string LastName { get; set; }

        [Display(Name = "Email Address")]
        public string EmailId { get; set; }

        [Display(Name = "Notification Condition")]
        public string NotificationCondition { get; set; }

        public string SearchDate { get; set; }
        public string Search { get; set; }

        public bool Active { get; set; }
        public bool Update { get; set; }
        public bool Cloase { get; set; }

        [Display(Name = "Organization")]
        public string Organizetion { get; set; }
        public List<SelectListItem> OrganizetionList { get; set; }

        public List<SelectListItem> ActiveList { get; set; }
    }
}

using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace FirstResponseApp.Models
{
    public class ApplicationUser : IdentityUser
    {
        
        public string Name { get; set; }
        public string LastName { get; set; }


        public TbOrganisationMaster Organisation { get; set; }
        [ForeignKey("Organisation")]
        public long OrganisationId { get; set; }

        public bool IsNotifyOnActiveTicket  { get; set; }
        public bool IsNotifyOnUpdateTicket { get; set; }
        public bool IsNotifyOnCloseTicket { get; set; }

        public DateTime? LastLogOnDateTime { get; set; }

        public DateTime? LastUpdatedOn { get; set; }
        public string CreatedBy { get; set; }
        public string LastModifiedBy { get; set; }
        public DateTime CreatedOn { get; set; }

        public bool IsDeleted { get; set; }
        public string DeletedBy { get; set; }
        public bool IsFirstTimeChangedPassword { get; set; }
        public bool IsActive { get; set; }

        public DateTime? LastDeactivatedOn { get; set; }
    }
}

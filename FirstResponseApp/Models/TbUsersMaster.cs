using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace FirstResponseApp.Models
{
    public class TbUsersMaster 
    {
        [Key]
        public long Id { get; set; }
        public string Name { get; set; }
        public string LastName { get; set; }

        public string UserName { get; set; }
        public string Password { get; set; }
        public string EmailAddress { get; set; }

        public TbOrganisationMaster Organisation { get; set; }
        [ForeignKey("Organisation")]
        public long OrganisationId { get; set; }

        public bool IsNotifyOnActive { get; set; }
        public bool IsNotifyOnUpdate { get; set; }
        public bool IsNotifyOnClose { get; set; }

        public DateTime? LastLogOnDateTime { get; set; }

        public DateTime? LastUpdatedOn  { get; set; }
        public string CreatedBy { get; set; }

        public DateTime CreatedOn { get; set; }

        public bool IsDeleted { get; set; }
        public string DeletedBy { get; set; }

        public bool IsActive { get; set; }

    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace FirstResponseApp.Models
{
    public class TbTicketResponse
    {
        [Key]
        public long Id { get; set; }

        public TbTicketMaster Master { get; set; }
        [ForeignKey("Master")]
        public long TicketMasterId { get; set; }


        public TbOrganisationMaster WaitingOn  { get; set; }
        [ForeignKey("WaitingOn")]
        public long WaitingOnOrganisationId { get; set; }
          
        public string LinfoxNotes { get; set; }

        public string MedilawNotes { get; set; }

        public string HWLENotes { get; set; }

        public TbTicketStatusMaster Status { get; set; }
        [ForeignKey("Status")]
        public long? StatusId { get; set; }

        public ApplicationUser UserId { get; set; }
        [ForeignKey("UserId")]
        public string UserMasterId { get; set; }

        public long? OrganisationId { get; set; }

        public string FileName { get; set; }
        public string Extension { get; set; }
        public string ContentType { get; set; }
        public string Link { get; set; }
        public byte[] FileData { get; set; }

        public string CreatedByUserId { get; set; }

        public DateTime CreatedOn { get; set; }
        public bool IsDeleted { get; set; }
        public string DeletedBy { get; set; }

        public bool IsActive { get; set; }

        public DateTime? LastUpdatedOn { get; set; }
    }
}

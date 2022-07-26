using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace FirstResponseApp.Models
{
    public class TbTicketMaster
    {
        [Key]
        public long Id { get; set; }

        public TbChannelMaster Channel { get; set; }
        [ForeignKey("Channel")]
        public long ChannelId { get; set; }

        public string MatterNumber{get; set;}

        public TbOrganisationMaster WaitingOn { get; set; }
        [ForeignKey("WaitingOn")]
        public long WaitingOnOrganisationId { get; set; }

        public string TicketName { get; set; }
        public string TicketNotes { get; set; }
      
        public DateTime? CreatedOn { get; set; }
        public DateTime? ClosedOn { get; set; }

        public string ClosedBy { get; set; }
        public string CreatedByUserId { get; set; }
        public bool IsDeleted { get; set; }

        public string DeletedBy { get; set; }

        public bool IsActive { get; set; }

        public TbTicketStatusMaster Status { get; set; }
        [ForeignKey("Status")]
        public long? StatusId { get; set; }
    }
}

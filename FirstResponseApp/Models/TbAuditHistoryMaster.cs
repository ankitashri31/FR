using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace FirstResponseApp.Models
{
    public class TbAuditHistoryMaster
    {
        [Key]
        public long Id { get; set; }

        public TbTicketMaster TicketMaster { get; set; }
        [ForeignKey("TicketMaster")]
        public long? TicketMasterId { get; set; }

        public long? AuditEventTypeId { get; set; }

        public ApplicationUser UserMaster { get; set; }
        [ForeignKey("UserMaster")]
        public string UserMasterId { get; set; }
        public string UserEmailAddress { get; set; }
        public DateTime? RecordedDate { get; set; }
        public string BrowserVersion { get; set; }
        public string AuditEventName { get; set; }
        public string AuditEventDescription { get; set; }
      
    }
}

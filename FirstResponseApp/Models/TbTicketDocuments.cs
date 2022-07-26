using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace FirstResponseApp.Models
{
    public class TbTicketDocuments
    {
        [Key]
        public long Id { get; set; }

        public TbTicketMaster TicketMaster { get; set; }
        [ForeignKey("TicketMaster")]
        public long TicketMasterId { get; set; }  
        public string FileName { get; set; }
        public string Extension { get; set; }
        public string ContentType { get; set; }
        public string Link { get; set; }
        public byte[] FileData { get; set; }
        public string CreatedByUserId { get; set; }
        public bool IsActive { get; set; }

        public bool IsDeleted { get; set; }

        public string DeletedBy { get; set; }

        public DateTime CreatedOn { get; set; }
    }
}

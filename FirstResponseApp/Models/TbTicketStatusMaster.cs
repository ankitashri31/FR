using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FirstResponseApp.Models
{
    public class TbTicketStatusMaster
    {
        public long Id { get; set; }
        public string Status { get; set; }
        public bool IsActive { get; set; }
    }
}

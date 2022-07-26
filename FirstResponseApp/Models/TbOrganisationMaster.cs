using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace FirstResponseApp.Models
{
    public class TbOrganisationMaster 
    {
        [Key]
        public long Id { get; set; }
        public string OrganisationName { get; set; }
        public bool? IsActive { get; set; }
    }
}

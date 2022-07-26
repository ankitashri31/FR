using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FirstResponseApp.Models
{
    public class UserSessionDetail
    {
        public string Id {get;set;}

        public string OrganizationId { get; set; }
        public string Name { get; set; }
        public string EmailId { get; set; }
        public string OrganisationName { get; set; }

        public bool IsActive { get; set; }
    }
}

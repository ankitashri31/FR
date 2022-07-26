using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FirstResponseApp.Areas.Ticket.Models
{
    public class UserModel
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string EmailAddress { get; set; }
        public string EmailType { get; set; }
        public string Organisation { get; set; }
        public long OrganisationId { get; set; }
        public string Status { get; set; }
        public bool IsActive { get; set; }
        public string AddedOn { get; set; }
        public DateTime CreatedOnDate { get; set; }

        public string DeactvatedOn { get; set; }
    }

    public class UserData
    {
        public UserModel[] Data { get; set; }
        public int ApplicationCount { get; set; }
    }
}

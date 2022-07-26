using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace FirstResponseApp.Helper
{
    public class Matter
    {
        public long Id { get; set; }

        [Display(Name = "Ticket Name")]
        public string TicketName { get; set; }
        public string Notes { get; set; }

        [Display(Name = "User Name")]
        public string UserName { get; set; }
        public List<SelectListItem> UserNameList { get; set; }

        //------------------------------------

        [Display(Name = "Search Date")]
        public string SearchDate { get; set; }
        public string Search { get; set; }

        public string Ticket { get; set; }
        public List<SelectListItem> TicketList { get; set; }

        [Display(Name = "Waiting On")]
        public string WaitingOn { get; set; }
        public List<SelectListItem> WaitingOnList { get; set; }
        public string Channel { get; set; }
        public List<SelectListItem> ChannelList { get; set; }
    }
}

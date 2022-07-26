using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace FirstResponseApp.Areas.Ticket.Controllers
{
    [Area("Ticket")]
    public class TicketController : Controller
    {
        [Route("Ticket")]
        [Route("Ticket/Index")]
        public IActionResult Index()
        {
            return View();
        }
    }
}
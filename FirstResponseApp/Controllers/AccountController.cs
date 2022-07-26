using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FirstResponseApp.Controllers
{
    public class AccountController : Controller
    {
        private readonly IHttpContextAccessor _HttpContextAccessor;
        public IActionResult Index()
        {
            return View();
        }
    }
}
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Dfc.DiscoverSkillsAndCareers.WebApp.Models;

namespace Dfc.DiscoverSkillsAndCareers.WebApp.Controllers
{
    [Route("start")]
    public class StartController : Controller
    {
        public IActionResult Index()
        {
            var model = new StartViewModel()
            {
                // TODO:
            };
            return View("Start", model);
        }
    }
}

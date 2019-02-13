using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Dfc.DiscoverSkillsAndCareers.WebApp.Models;

namespace Dfc.DiscoverSkillsAndCareers.WebApp.Controllers
{
    [Route("finish")]
    public class FinishController : Controller
    {
        public IActionResult Index()
        {
            var model = new FinishViewModel()
            {
                // TODO:
            };
            return View("Finish", model);
        }
    }
}
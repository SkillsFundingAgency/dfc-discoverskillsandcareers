using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Dfc.DiscoverSkillsAndCareers.WebApp.Models;

namespace Dfc.DiscoverSkillsAndCareers.WebApp.Controllers
{
    [Route("save-my-progress")]
    public class SaveProgressController : Controller
    {
        public IActionResult Index()
        {
            var model = new SaveProgressViewModel()
            {
                // TODO:
            };
            return View("SaveProgress", model);
        }
    }
}
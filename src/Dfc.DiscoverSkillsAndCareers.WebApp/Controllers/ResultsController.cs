using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Dfc.DiscoverSkillsAndCareers.WebApp.Models;

namespace Dfc.DiscoverSkillsAndCareers.WebApp.Controllers
{
    [Route("results")]
    public class ResultsController : Controller
    {
        public IActionResult Index()
        {
            var model = new ResultsViewModel()
            {
                // TODO:
            };
            return View("Results", model);
        }
    }
}
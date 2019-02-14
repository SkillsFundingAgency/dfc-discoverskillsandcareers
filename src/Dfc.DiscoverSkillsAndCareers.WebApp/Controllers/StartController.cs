using Dfc.DiscoverSkillsAndCareers.WebApp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Dfc.DiscoverSkillsAndCareers.WebApp.Controllers
{
    [Route("start")]
    public class StartController : Controller
    {
        readonly ILogger<StartController> Logger;

        public StartController(ILogger<StartController> logger)
        {
            Logger = logger;
        }

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

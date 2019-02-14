using Dfc.DiscoverSkillsAndCareers.Services;
using Dfc.DiscoverSkillsAndCareers.WebApp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace Dfc.DiscoverSkillsAndCareers.WebApp.Controllers
{
    [Route("finish")]
    public class FinishController : Controller
    {
        readonly ILogger<FinishController> Logger;
        readonly IUserSessionService UserSessionService;

        public FinishController(ILogger<FinishController> logger, 
            IUserSessionService userSessionService)
        {
            Logger = logger;
            UserSessionService = userSessionService;
        }

        public async Task<IActionResult> Index()
        {
            await UserSessionService.Init(Request);

            var model = new FinishViewModel()
            {
                SessionId = UserSessionService.Session.PrimaryKey
            };
            return View("Finish", model);
        }
    }
}
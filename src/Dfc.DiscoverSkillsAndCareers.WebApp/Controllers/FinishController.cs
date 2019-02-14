using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Dfc.DiscoverSkillsAndCareers.WebApp.Models;
using Dfc.DiscoverSkillsAndCareers.Services;

namespace Dfc.DiscoverSkillsAndCareers.WebApp.Controllers
{
    [Route("finish")]
    public class FinishController : Controller
    {
        readonly IUserSessionService UserSessionService;

        public FinishController(IUserSessionService userSessionService)
        {
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
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Dfc.DiscoverSkillsAndCareers.Services;
using Dfc.DiscoverSkillsAndCareers.WebApp.Models;

namespace Dfc.DiscoverSkillsAndCareers.WebApp.Controllers
{
    public class HomeController : Controller
    {
        readonly IUserSessionService UserSessionService;

        public HomeController(IUserSessionService userSessionService)
        {
            UserSessionService = userSessionService;
        }

        public IActionResult Index()
        {
            var model = new IndexViewModel()
            {
                // TODO:
            };
            return View(model);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        public class ReloadRequest
        {
            public string Code { get; set; }
        }

        [HttpPost]
        [Route("reload")]
        public async Task<IActionResult> Reload([FromForm] ReloadRequest reloadRequest)
        {
            await UserSessionService.Reload(reloadRequest.Code);

            if (UserSessionService.HasSession)
            {
                if (UserSessionService.Session.IsComplete)
                {
                    // Session has complete, redirect to results
                    var redirectResponse = new RedirectResult($"/results");
                    Response.Cookies.Append("ncs-session-id", UserSessionService.Session.PrimaryKey);
                    return redirectResponse;
                }
                else
                {
                    // Session is not complete so continue where we was last
                    var redirectResponse = new RedirectResult($"/q/{UserSessionService.Session.CurrentQuestion}");
                    Response.Cookies.Append("ncs-session-id", UserSessionService.Session.PrimaryKey);
                    return redirectResponse;
                }
            }
            // Session could not be found
            var model = new IndexViewModel()
            {
                HasReloadError = true
            };
            return View("Index", model);
        }
    }
}

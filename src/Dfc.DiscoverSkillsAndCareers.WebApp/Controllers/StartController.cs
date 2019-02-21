using Dfc.DiscoverSkillsAndCareers.WebApp.Models;
using Dfc.DiscoverSkillsAndCareers.WebApp.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace Dfc.DiscoverSkillsAndCareers.WebApp.Controllers
{
    [Route("start")]
    public class StartController : BaseController
    {
        readonly ILogger<StartController> Logger;
        readonly IApiServices ApiServices;

        public StartController(ILogger<StartController> logger,
            IApiServices apiServices)
        {
            Logger = logger;
            ApiServices = apiServices;
        }

        public async Task<IActionResult> Index()
        {
            var sessionId = await TryGetSessionId(Request);
            var model = await ApiServices.GetContentModel<StartViewModel>("startpage");
            if (string.IsNullOrEmpty(sessionId) == false)
            {
                Response.Cookies.Append("ncs-session-id", sessionId);
            }
            return View("Start", model);
        }
    }
}

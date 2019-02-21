using Dfc.DiscoverSkillsAndCareers.WebApp.Models;
using Dfc.DiscoverSkillsAndCareers.WebApp.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace Dfc.DiscoverSkillsAndCareers.WebApp.Controllers
{
    [Route("finish")]
    public class FinishController : BaseController
    {
        readonly ILogger<FinishController> Logger;
        readonly IApiServices ApiServices;

        public FinishController(ILogger<FinishController> logger,
            IApiServices apiServices)
        {
            Logger = logger;
            ApiServices = apiServices;
        }

        public async Task<IActionResult> Index()
        {
            var sessionId = await TryGetSessionId(Request);

            if (string.IsNullOrEmpty(sessionId))
            {
                return Redirect("/");
            }

            var model = await ApiServices.GetContentModel<FinishViewModel>("finishpage");
            Response.Cookies.Append("ncs-session-id", sessionId);
            return View("SaveProgress", model);
        }
    }
}
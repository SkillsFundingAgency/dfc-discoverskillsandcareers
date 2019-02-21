using Dfc.DiscoverSkillsAndCareers.WebApp.Models;
using Dfc.DiscoverSkillsAndCareers.WebApp.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Diagnostics;
using System.Threading.Tasks;

namespace Dfc.DiscoverSkillsAndCareers.WebApp.Controllers
{
    public class HomeController : BaseController
    {
        readonly ILogger<HomeController> Logger;
        readonly IApiServices ApiServices;

        public HomeController(ILogger<HomeController> logger,
            IApiServices apiServices)
        {
            Logger = logger;
            ApiServices = apiServices;
        }

        public async Task<IActionResult> Index()
        {
            var sessionId = await TryGetSessionId(Request);
            var model = await ApiServices.GetContentModel<IndexViewModel>("indexpage");
            model.SessionId = sessionId;
            if (string.IsNullOrEmpty(sessionId) == false)
            {
                Response.Cookies.Append("ncs-session-id", sessionId);
            }
            return View("Index", model);
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

        [HttpGet]
        [Route("reload")]
        public IActionResult ReloadGet()
        {
            return Redirect("/");
        }

        [HttpPost]
        [Route("reload")]
        public async Task<IActionResult> Reload([FromForm] ReloadRequest reloadRequest)
        {

            var nextQuestionResponse = await ApiServices.NextQuestion(reloadRequest.Code);
            if (nextQuestionResponse == null)
            {
                var model = await ApiServices.GetContentModel<IndexViewModel>("indexpage");
                model.HasReloadError = true;
                return View("Index", model);
            }
            Response.Cookies.Append("ncs-session-id", nextQuestionResponse.SessionId);
            if (nextQuestionResponse.IsComplete)
            {
                // Session has complete, redirect to results
                var redirectResponse = new RedirectResult($"/results");
                return redirectResponse;
            }
            else
            {
                // Session is not complete so continue where we was last
                var redirectResponse = new RedirectResult($"/q/{nextQuestionResponse.NextQuestionNumber}");
                return redirectResponse;
            }
        }
    }
}

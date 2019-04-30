using Dfc.DiscoverSkillsAndCareers.WebApp.Models;
using Dfc.DiscoverSkillsAndCareers.WebApp.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using System.Web;

namespace Dfc.DiscoverSkillsAndCareers.WebApp.Controllers
{
    public class HomeController : BaseController
    {
        readonly ILogger<HomeController> _log;
        readonly IApiServices _apiServices;

        public HomeController(
            ILogger<HomeController> log,
            IApiServices apiServices)
        {
            _log = log;
            _apiServices = apiServices;
        }

        public async Task<IActionResult> Index(string e = "", string v = "")
        {
            var correlationId = Guid.NewGuid();
            try
            {
                var sessionId = await TryGetSessionId(Request);
                var model = await _apiServices.GetContentModel<IndexViewModel>("indexpage", correlationId);
                model.SessionId = sessionId;
                model.HasReloadError = !string.IsNullOrEmpty(e);
                if (e == "1")
                {
                    model.ResumeErrorMessage = model.MissingCodeErrorMessage;
                }
                if (string.IsNullOrEmpty(sessionId) == false)
                {
                    AppendCookie(sessionId);
                }
                if (v == "2")
                {
                    return View("Version2", model);
                }
                return View("Index", model);
            }
            catch (Exception ex)
            {
                _log.LogError(ex, $"Correlation Id: {correlationId} - An error occurred rendering action {nameof(Index)}");
                return StatusCode(500);
            }
        }

        public class ReloadRequest
        {
            public string Code { get; set; }
        }

        [HttpGet]
        [Route("reload")]
        public async Task<IActionResult> ReloadGet()
        {
            var sessionId = await TryGetSessionId(Request);
            if (!string.IsNullOrEmpty(sessionId))
            {
                return await Reload(new ReloadRequest { Code = sessionId });
            }
            return Redirect("/");
        }

        [HttpPost]
        [Route("reload")]
        public async Task<IActionResult> Reload([FromForm] ReloadRequest reloadRequest)
        {
            var correlationId = Guid.NewGuid();
            try
            {
                if (string.IsNullOrEmpty(reloadRequest?.Code))
                {
                    return Redirect("/?e=1");
                }

                reloadRequest.Code = reloadRequest.Code.Replace(" ", "").ToLower();
                if (reloadRequest.Code != HttpUtility.UrlEncode(reloadRequest.Code))
                {
                    return Redirect("/?e=2");
                }

                var nextQuestionResponse = await _apiServices.Reload(reloadRequest.Code, correlationId);
                if (nextQuestionResponse == null)
                {
                    return Redirect("/?e=2");
                }

                AppendCookie(nextQuestionResponse.SessionId);

                if (nextQuestionResponse.IsComplete)
                {
                    if (nextQuestionResponse.IsFilterAssessment)
                    {
                        return new RedirectResult($"/results/{nextQuestionResponse.JobCategorySafeUrl}");
                    }
                    else
                    {
                        return new RedirectResult($"/results");
                    }
                }

                if (nextQuestionResponse.IsFilterAssessment)
                {
                    return new RedirectResult($"/q/{nextQuestionResponse.JobCategorySafeUrl}/{nextQuestionResponse.QuestionNumber}");
                }
                else
                {
                    return new RedirectResult($"/q/short/{nextQuestionResponse.QuestionNumber}");
                }
            }
            catch (Exception ex)
            {
                _log.LogError(ex, $"Correlation Id: {correlationId} - An error occurred rendering action {nameof(Reload)}");
                return StatusCode(500);
            }
        }
    }
}

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

        public async Task<IActionResult> Index(string e = "")
        {
            var correlationId = Guid.NewGuid();
            try
            {
                var sessionId = await TryGetSessionId();
                var model = new IndexViewModel { SessionId = sessionId };
                
                if (e == "1")
                {
                    model.ErrorMessage = "Enter your reference";
                } else if (e == "2")
                {
                    model.ErrorMessage = "The reference could not be found";
                }
                
                if (string.IsNullOrEmpty(sessionId) == false)
                {
                    AppendCookie(sessionId);
                }
                
                return View("Index", model);
            }
            catch (Exception ex)
            {
                _log.LogError(ex, $"Correlation Id: {correlationId} - An error occurred rendering action {nameof(Index)}");
                return RedirectToAction("Error500", "Error");
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
            var sessionId = await TryGetSessionId();
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

                    return new RedirectResult($"/results");
                }

                var questionUrl = nextQuestionResponse.GetQuestionPageNumber();
                if (nextQuestionResponse.IsFilterAssessment)
                {
                    return new RedirectResult($"/q/{nextQuestionResponse.JobCategorySafeUrl}/{questionUrl}");
                }

                return new RedirectResult($"/q/short/{questionUrl}");
            }
            catch (System.Net.Http.HttpRequestException)
            {
                return Redirect("/?e=2");
            }
            catch (Exception ex)
            {
                _log.LogError(ex, $"Correlation Id: {correlationId} - An error occurred rendering action {nameof(Reload)}");
                return RedirectToAction("Error500", "Error");
            }
        }
    }
}

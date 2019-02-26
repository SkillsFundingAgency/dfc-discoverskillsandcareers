using Dfc.DiscoverSkillsAndCareers.WebApp.Models;
using Dfc.DiscoverSkillsAndCareers.WebApp.Services;
using DFC.Common.Standard.Logging;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace Dfc.DiscoverSkillsAndCareers.WebApp.Controllers
{
    public class HomeController : BaseController
    {
        readonly ILogger<HomeController> Log;
        readonly ILoggerHelper LoggerHelper;
        readonly IApiServices ApiServices;

        public HomeController(
            ILogger<HomeController> log,
            ILoggerHelper loggerHelper,
            IApiServices apiServices)
        {
            Log = log;
            LoggerHelper = loggerHelper;
            ApiServices = apiServices;
        }

        public async Task<IActionResult> Index()
        {
            var correlationId = Guid.NewGuid();
            try
            {
                LoggerHelper.LogMethodEnter(Log);

                var sessionId = await TryGetSessionId(Request);
                var model = await ApiServices.GetContentModel<IndexViewModel>("indexpage", correlationId);
                model.SessionId = sessionId;
                if (string.IsNullOrEmpty(sessionId) == false)
                {
                    Response.Cookies.Append("ncs-session-id", sessionId);
                }
                return View("Index", model);
            }
            catch (Exception ex)
            {
                LoggerHelper.LogException(Log, correlationId, ex);
                return StatusCode(500);
            }
            finally
            {
                LoggerHelper.LogMethodExit(Log);
            }
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
            var correlationId = Guid.NewGuid();
            try
            {
                LoggerHelper.LogMethodEnter(Log);

                var nextQuestionResponse = await ApiServices.NextQuestion(reloadRequest.Code ?? "", correlationId);
                if (nextQuestionResponse == null)
                {
                    var model = await ApiServices.GetContentModel<IndexViewModel>("indexpage", correlationId);
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
            catch (Exception ex)
            {
                LoggerHelper.LogException(Log, correlationId, ex);
                return StatusCode(500);
            }
            finally
            {
                LoggerHelper.LogMethodExit(Log);
            }
        }
    }
}

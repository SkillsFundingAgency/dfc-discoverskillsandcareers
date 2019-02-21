using Dfc.DiscoverSkillsAndCareers.WebApp.Models;
using Dfc.DiscoverSkillsAndCareers.WebApp.Services;
using DFC.Common.Standard.Logging;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace Dfc.DiscoverSkillsAndCareers.WebApp.Controllers
{
    [Route("save-my-progress")]
    public class SaveProgressController : BaseController
    {
        readonly ILogger<SaveProgressController> Log;
        readonly ILoggerHelper LoggerHelper;
        readonly IApiServices ApiServices;

        public SaveProgressController(
            ILogger<SaveProgressController> log,
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

                if (string.IsNullOrEmpty(sessionId))
                {
                    return Redirect("/");
                }

                var model = await ApiServices.GetContentModel<SaveProgressViewModel>("saveprogresspage", correlationId);

                var nextQuestionResponse = await ApiServices.NextQuestion(sessionId, correlationId);
                model.SessionId = sessionId;
                model.Code = nextQuestionResponse.ReloadCode;
                model.SessionDate = nextQuestionResponse.StartedDt.ToString("dd MMMM yyyy");
                model.Status = $"{nextQuestionResponse.RecordedAnswersCount} out of {nextQuestionResponse.MaxQuestionsCount} statements complete";
                Response.Cookies.Append("ncs-session-id", sessionId);
                return View("SaveProgress", model);
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
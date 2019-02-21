using Dfc.DiscoverSkillsAndCareers.WebApp.Models;
using Dfc.DiscoverSkillsAndCareers.WebApp.Services;
using DFC.Common.Standard.Logging;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace Dfc.DiscoverSkillsAndCareers.WebApp.Controllers
{
    [Route("results")]
    public class ResultsController : BaseController
    {
        readonly ILogger<ResultsController> Log;
        readonly ILoggerHelper LoggerHelper;
        readonly IApiServices ApiServices;

        public ResultsController(
            ILogger<ResultsController> log,
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

                var resultsResponse = await ApiServices.Results(sessionId, correlationId);

                var contentName = $"{resultsResponse.AssessmentType.ToLower()}page";
                var model = await ApiServices.GetContentModel<ResultsViewModel>(contentName, correlationId);
                model.SessionId = sessionId;
                model.AssessmentType = resultsResponse.AssessmentType;
                model.JobFamilies = resultsResponse.JobFamilies;
                model.JobFamilyCount = resultsResponse.JobFamilyCount;
                model.JobFamilyMoreCount = resultsResponse.JobFamilyMoreCount;
                model.Traits = resultsResponse.Traits;
                return View("Results", model);
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
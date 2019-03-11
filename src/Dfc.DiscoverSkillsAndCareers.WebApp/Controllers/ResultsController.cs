using Dfc.DiscoverSkillsAndCareers.WebApp.Config;
using Dfc.DiscoverSkillsAndCareers.WebApp.Models;
using Dfc.DiscoverSkillsAndCareers.WebApp.Services;
using DFC.Common.Standard.Logging;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
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
        readonly AppSettings AppSettings;

        public ResultsController(
            ILogger<ResultsController> log,
            ILoggerHelper loggerHelper,
            IApiServices apiServices,
            IOptions<AppSettings> appSettings)
        {
            Log = log;
            LoggerHelper = loggerHelper;
            ApiServices = apiServices;
            AppSettings = appSettings.Value;
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

                var contentName = $"{resultsResponse.AssessmentType.ToLower()}resultpage";
                var model = await ApiServices.GetContentModel<ResultsViewModel>(contentName, correlationId);
                model.SessionId = sessionId;
                model.AssessmentType = resultsResponse.AssessmentType;
                model.JobFamilies = resultsResponse.JobFamilies;
                model.JobFamilyCount = resultsResponse.JobFamilyCount;
                model.JobFamilyMoreCount = resultsResponse.JobFamilyMoreCount;
                model.Traits = resultsResponse.Traits;
                model.UseFilteringQuestions = AppSettings.UseFilteringQuestions;
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

        [Route("filtered/{jobCategory}")]
        public async Task<IActionResult> StartFilteredForJobCategory(string jobCategory)
        {
            if (!AppSettings.UseFilteringQuestions) return NotFound();

            var correlationId = Guid.NewGuid();
            try
            {
                LoggerHelper.LogMethodEnter(Log);

                var sessionId = await TryGetSessionId(Request);
                if (string.IsNullOrEmpty(sessionId))
                {
                    return Redirect("/");
                }

                var resultsResponse = await ApiServices.StartFilteredForJobCategory(correlationId, sessionId, jobCategory);
                Response.Cookies.Append("ncs-session-id", sessionId, new Microsoft.AspNetCore.Http.CookieOptions() { Secure = true });
                var redirectResponse = new RedirectResult($"/qf/{1}"); //TODO: start from 1 or last if previous?
                return redirectResponse;

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

        [Route("{jobCategory}")]
        public async Task<IActionResult> ResultsFilteredForJobCategory(string jobCategory)
        {
            if (!AppSettings.UseFilteringQuestions) return NotFound();

            var correlationId = Guid.NewGuid();
            try
            {
                LoggerHelper.LogMethodEnter(Log);

                var sessionId = await TryGetSessionId(Request);
                if (string.IsNullOrEmpty(sessionId))
                {
                    return Redirect("/");
                }

                var resultsForJobCategoryResponse = await ApiServices.ResultsForJobCategory(sessionId, jobCategory, correlationId);

                var contentName = $"filteredresultpage";
                var model = await ApiServices.GetContentModel<ResultsForJobCategoryViewModel>(contentName, correlationId);
                model.SessionId = sessionId;
                model.JobProfiles = resultsForJobCategoryResponse.JobProfiles;
                return View("ResultsForJobCategory", model);
            }
            catch (System.Net.Http.HttpRequestException)
            {
                return Redirect("/results");
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
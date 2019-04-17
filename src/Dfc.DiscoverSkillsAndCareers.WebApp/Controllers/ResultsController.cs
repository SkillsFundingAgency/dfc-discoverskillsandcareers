using Dfc.DiscoverSkillsAndCareers.Models;
using Dfc.DiscoverSkillsAndCareers.WebApp.Config;
using Dfc.DiscoverSkillsAndCareers.WebApp.Models;
using Dfc.DiscoverSkillsAndCareers.WebApp.Services;
using DFC.Common.Standard.Logging;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Linq;
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
            string sessionId = null;
            try
            {
                LoggerHelper.LogMethodEnter(Log);

                sessionId = await TryGetSessionId(Request);
                if (string.IsNullOrEmpty(sessionId))
                {
                    return Redirect("/");
                }

                var resultsResponse = await ApiServices.Results(sessionId, correlationId);

                var lastFilterResult = resultsResponse.JobFamilies
                                                      .Where(x => x.FilterAssessment != null)
                                                      .OrderByDescending(x => x.FilterAssessment.CreatedDt)
                                                      .Select(x => x.FilterAssessment)
                                                      .FirstOrDefault();
                if (lastFilterResult != null)
                {
                    return await ReturnResultsView(resultsResponse, correlationId, lastFilterResult.JobFamilyNameUrlSafe, sessionId);
                }

                var contentName = $"{resultsResponse.AssessmentType.ToLower()}resultpage";
                var model = await ApiServices.GetContentModel<ResultsViewModel>(contentName, correlationId);
                model.SessionId = sessionId;
                model.Code = SaveProgressController.GetDisplayCode(sessionId.Split("-")[1]);
                model.AssessmentType = resultsResponse.AssessmentType;
                model.JobFamilies = resultsResponse.JobFamilies;
                model.JobFamilyCount = resultsResponse.JobFamilyCount;
                model.JobFamilyMoreCount = resultsResponse.JobFamilyMoreCount;
                model.Traits = resultsResponse.Traits;
                model.UseFilteringQuestions = AppSettings.UseFilteringQuestions;
                model.JobProfiles = resultsResponse.JobProfiles;
                return View("Results", model);
            }
            catch (System.Net.Http.HttpRequestException ex)
            {
                LoggerHelper.LogException(Log, correlationId, ex);
                if (!string.IsNullOrEmpty(sessionId))
                {
                    return Redirect("/reload");
                }
                return Redirect("/");
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
                AppendCookie(sessionId);
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

                var resultsForJobCategoryResponse = await ApiServices.Results(sessionId, correlationId);
                return await ReturnResultsView(resultsForJobCategoryResponse, correlationId, jobCategory, sessionId);
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

        [NonAction]
        public async Task<IActionResult> ReturnResultsView(ResultsResponse resultsResponse, Guid correlationId, string jobCategory, string sessionId)
        {
            var contentName = $"filteredresultpage";
            var model = await ApiServices.GetContentModel<ResultsViewModel>(contentName, correlationId);
            model.SessionId = sessionId;
            model.Code = SaveProgressController.GetDisplayCode(sessionId.Split("-")[1]);
            model.AssessmentType = resultsResponse.AssessmentType;
            model.JobFamilies = ReOrderWithFirst(resultsResponse.JobFamilies, jobCategory);
            model.JobFamilyCount = resultsResponse.JobFamilyCount;
            model.JobFamilyMoreCount = resultsResponse.JobFamilyMoreCount;
            model.Traits = resultsResponse.Traits;
            model.UseFilteringQuestions = AppSettings.UseFilteringQuestions;
            model.JobProfiles = resultsResponse.JobProfiles;
            model.WhatYouToldUs = resultsResponse.WhatYouToldUs;
            return View("ResultsForJobCategory", model);
        }

        private JobFamilyResult[] ReOrderWithFirst(JobFamilyResult[] jobFamilyResults, string jobCategory)
        {
            var highlightCategory = jobFamilyResults.Where(x => x.FilterAssessment?.JobFamilyNameUrlSafe == jobCategory).FirstOrDefault();
            var otherCategories = jobFamilyResults.Where(x => x.FilterAssessment?.JobFamilyNameUrlSafe != jobCategory).ToList();
            var newList = otherCategories.ToList();
            newList.Insert(0, highlightCategory);
            return newList.ToArray();
        }
    }
}
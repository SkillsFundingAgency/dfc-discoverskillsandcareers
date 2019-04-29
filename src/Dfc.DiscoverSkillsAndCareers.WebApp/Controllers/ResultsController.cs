using Dfc.DiscoverSkillsAndCareers.Models;
using Dfc.DiscoverSkillsAndCareers.WebApp.Config;
using Dfc.DiscoverSkillsAndCareers.WebApp.Models;
using Dfc.DiscoverSkillsAndCareers.WebApp.Services;
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
        readonly ILogger<ResultsController> _log;
        readonly IApiServices _apiServices;
        readonly AppSettings _appSettings;

        public ResultsController(
            ILogger<ResultsController> log,
            IApiServices apiServices,
            IOptions<AppSettings> appSettings)
        {
            _log = log;
            _apiServices = apiServices;
            _appSettings = appSettings.Value;
        }

        public async Task<IActionResult> Index()
        {
            var correlationId = Guid.NewGuid();
            string sessionId = null;
            try
            {
                sessionId = await TryGetSessionId(Request);
                if (string.IsNullOrEmpty(sessionId))
                {
                    return Redirect("/");
                }

                var resultsResponse = await _apiServices.Results(sessionId, correlationId);

                var lastFilterResult = resultsResponse.JobFamilies
                                                      .Where(x => x.FilterAssessment != null)
                                                      .OrderByDescending(x => x.FilterAssessment.CreatedDt)
                                                      .Select(x => x.FilterAssessment)
                                                      .FirstOrDefault();
                if (lastFilterResult != null)
                {
                    AppendCookie(sessionId);
                    return Redirect($"/results/{lastFilterResult.JobFamilyNameUrlSafe}");
                }

                var contentName = $"{resultsResponse.AssessmentType.ToLower()}resultpage";
                var model = await _apiServices.GetContentModel<ResultsViewModel>(contentName, correlationId);
                model.SessionId = sessionId;
                model.Code = SaveProgressController.GetDisplayCode(sessionId.Split("-")[1]);
                model.AssessmentType = resultsResponse.AssessmentType;
                model.JobFamilies = resultsResponse.JobFamilies;
                model.JobFamilyCount = resultsResponse.JobFamilyCount;
                model.JobFamilyMoreCount = resultsResponse.JobFamilyMoreCount;
                model.Traits = resultsResponse.Traits;
                model.UseFilteringQuestions = _appSettings.UseFilteringQuestions;
                model.JobProfiles = resultsResponse.JobProfiles;
                model.ExploreCareersBaseUrl = _appSettings.ExploreCareersBaseUrl;
                return View("Results", model);
            }
            catch (System.Net.Http.HttpRequestException ex)
            {
                _log.LogError(ex, $"Correlation Id: {correlationId} - A dependency error occurred rendering action {nameof(Index)}");
                if (!string.IsNullOrEmpty(sessionId))
                {
                    return Redirect("/reload");
                }
                return Redirect("/");
            }
            catch (Exception ex)
            {
                _log.LogError(ex, $"Correlation Id: {correlationId} - An error occurred rendering action {nameof(Index)}");
                return StatusCode(500);
            }
        }

        [Route("filtered/{jobCategory}")]
        public async Task<IActionResult> StartFilteredForJobCategory(string jobCategory)
        {
            if (!_appSettings.UseFilteringQuestions) return NotFound();

            var correlationId = Guid.NewGuid();
            try
            {
                var sessionId = await TryGetSessionId(Request);
                if (string.IsNullOrEmpty(sessionId))
                {
                    return Redirect("/");
                }

                var _ = await _apiServices.StartFilteredForJobCategory(correlationId, sessionId, jobCategory);
                AppendCookie(sessionId);
                var redirectResponse = new RedirectResult($"/q/{jobCategory}/01");
                return redirectResponse;

            }
            catch (Exception ex)
            {
                _log.LogError(ex, $"Correlation Id: {correlationId} - An error occurred rendering action {nameof(StartFilteredForJobCategory)}");
                return StatusCode(500);
            }
        }

        [Route("{jobCategory}")]
        public async Task<IActionResult> ResultsFilteredForJobCategory(string jobCategory)
        {
            if (!_appSettings.UseFilteringQuestions) return NotFound();

            var correlationId = Guid.NewGuid();
            try
            {
                var sessionId = await TryGetSessionId(Request);
                if (string.IsNullOrEmpty(sessionId))
                {
                    return Redirect("/");
                }

                var resultsForJobCategoryResponse = await _apiServices.Results(sessionId, correlationId);
                return await ReturnResultsView(resultsForJobCategoryResponse, correlationId, jobCategory, sessionId);
            }
            catch (System.Net.Http.HttpRequestException)
            {
                return Redirect("/results");
            }
            catch (Exception ex)
            {
                _log.LogError(ex, $"Correlation Id: {correlationId} - An error occurred rendering action {nameof(ResultsFilteredForJobCategory)}");
                return StatusCode(500);
            }
        }

        [NonAction]
        public async Task<IActionResult> ReturnResultsView(ResultsResponse resultsResponse, Guid correlationId, string jobCategory, string sessionId)
        {
            var contentName = $"filteredresultpage";
            var model = await _apiServices.GetContentModel<ResultsViewModel>(contentName, correlationId);
            model.SessionId = sessionId;
            model.Code = SaveProgressController.GetDisplayCode(sessionId.Split("-")[1]);
            model.AssessmentType = resultsResponse.AssessmentType;
            model.JobFamilies = ReOrderWithFirst(resultsResponse.JobFamilies, jobCategory);
            model.JobFamilyCount = resultsResponse.JobFamilyCount;
            model.JobFamilyMoreCount = resultsResponse.JobFamilyMoreCount;
            model.Traits = resultsResponse.Traits;
            model.UseFilteringQuestions = _appSettings.UseFilteringQuestions;
            model.JobProfiles = resultsResponse.JobProfiles;
            model.WhatYouToldUs = resultsResponse.WhatYouToldUs;
            model.ExploreCareersBaseUrl = _appSettings.ExploreCareersBaseUrl;
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
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

                var lastFilterResult = resultsResponse.JobCategories
                                                      .Where(x => x.FilterAssessmentResult != null)
                                                      .OrderByDescending(x => x.FilterAssessmentResult.CreatedDt)
                                                      .Select(x => x.FilterAssessmentResult)
                                                      .FirstOrDefault();
                if (lastFilterResult != null)
                {
                    AppendCookie(sessionId);
                    return Redirect($"/results/{lastFilterResult.JobFamilyNameUrlSafe}");
                }

                var model = new ResultsViewModel
                {
                    SessionId = sessionId,
                    Code = SaveProgressController.GetDisplayCode(sessionId.Split("-")[1]),
                    AssessmentType = resultsResponse.AssessmentType,
                    JobCategories = resultsResponse.JobCategories,
                    JobFamilyCount = resultsResponse.JobFamilyCount,
                    JobFamilyMoreCount = resultsResponse.JobFamilyMoreCount,
                    Traits = resultsResponse.Traits,
                    UseFilteringQuestions = _appSettings.UseFilteringQuestions,
                    JobProfiles = resultsResponse.JobProfiles,
                    ExploreCareersBaseUrl = _appSettings.ExploreCareersBaseUrl
                };
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
                return RedirectToAction("Error500", "Error");
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

                var response = await _apiServices.StartFilteredForJobCategory(correlationId, sessionId, jobCategory);
                AppendCookie(sessionId);
                var redirectResponse = new RedirectResult($"/q/{jobCategory}/{response.QuestionNumber.ToQuestionPageNumber()}");
                return redirectResponse;

            }
            catch (Exception ex)
            {
                _log.LogError(ex, $"Correlation Id: {correlationId} - An error occurred rendering action {nameof(StartFilteredForJobCategory)}");
                return RedirectToAction("Error500", "Error");
            }
        }
        
        [Route("filtered/{jobCategory}/show")]
        public async Task<IActionResult> ShowResults(string jobCategory)
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

                var resultsForJobCategoryResponse = await _apiServices.ResultsForJobCategory(sessionId, jobCategory, correlationId);
                return ReturnResultsView(resultsForJobCategoryResponse, jobCategory, sessionId);

            }
            catch (System.Net.Http.HttpRequestException)
            {
                return Redirect("/results");
            }
            catch (Exception ex)
            {
                _log.LogError(ex, $"Correlation Id: {correlationId} - An error occurred rendering action {nameof(StartFilteredForJobCategory)}");
                return RedirectToAction("Error500", "Error");
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

                var resultsForJobCategoryResponse = await _apiServices.ResultsForJobCategory(sessionId, jobCategory, correlationId);
                return ReturnResultsView(resultsForJobCategoryResponse, jobCategory, sessionId);
            }
            catch (Exception ex)
            {
                _log.LogError(ex, $"Correlation Id: {correlationId} - An error occurred rendering action {nameof(ResultsFilteredForJobCategory)}");
                return RedirectToAction("Error500", "Error");
            }
        }

        [NonAction]
        private IActionResult ReturnResultsView(ResultsResponse resultsResponse, string jobCategory, string sessionId)
        {
            var model = new ResultsViewModel
            {
                SessionId = sessionId,
                Code = SaveProgressController.GetDisplayCode(sessionId.Split("-")[1]),
                AssessmentType = resultsResponse.AssessmentType,
                JobCategories = ReOrderWithFirst(resultsResponse.JobCategories, jobCategory),
                JobFamilyCount = resultsResponse.JobFamilyCount,
                JobFamilyMoreCount = resultsResponse.JobFamilyMoreCount,
                Traits = resultsResponse.Traits,
                UseFilteringQuestions = _appSettings.UseFilteringQuestions,
                JobProfiles = resultsResponse.JobProfiles,
                WhatYouToldUs = resultsResponse.WhatYouToldUs,
                ExploreCareersBaseUrl = _appSettings.ExploreCareersBaseUrl
            };


            return View("ResultsForJobCategory", model);
        }

        private JobCategoryResult[] ReOrderWithFirst(JobCategoryResult[] jobCategoryResults, string jobCategory)
        {
            var highlightCategory = jobCategoryResults.FirstOrDefault(x => x.FilterAssessmentResult?.JobFamilyNameUrlSafe == jobCategory);
            var otherCategories = 
                jobCategoryResults
                    .Where(x => x.FilterAssessmentResult?.JobFamilyNameUrlSafe != jobCategory)
                    .OrderByDescending(x => x.ResultsShown ? 100 : 0 + x.FilterAssessmentResult?.SuggestedJobProfiles.Count())
                    .ToList();
            
            if (highlightCategory != null)
            {
                otherCategories.Insert(0, highlightCategory);
            }

            return otherCategories.ToArray();
        }
    }
}
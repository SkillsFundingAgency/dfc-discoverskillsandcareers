
using Dfc.DiscoverSkillsAndCareers.WebApp.Models;
using Dfc.DiscoverSkillsAndCareers.WebApp.Services;
using DFC.Common.Standard.Logging;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace Dfc.DiscoverSkillsAndCareers.WebApp.Controllers
{
    [Route("finish")]
    public class FinishController : BaseController
    {
        readonly ILogger<FinishController> _log;
        readonly IApiServices _apiServices;

        public FinishController(
            ILogger<FinishController> log,
            IApiServices apiServices)
        {
            _log = log;
            _apiServices = apiServices;
        }

        [Route("{jobCategory}")]
        public async Task<IActionResult> FinishWithJobCategory(string jobCategory)
        {
            var correlationId = Guid.NewGuid();
            try
            {
                var sessionId = await TryGetSessionId(Request);
                if (string.IsNullOrEmpty(sessionId))
                {
                    return Redirect("/");
                }

                var viewName ="FinishFilteredAssessment";
                var contentName = "finishjobcategorypage";
                var model = await _apiServices.GetContentModel<FinishViewModel>(contentName, correlationId);
                model.JobCategorySafeUrl = jobCategory;
                AppendCookie(sessionId);
                return View(viewName, model);
            }
            catch (Exception ex)
            {
                _log.LogError(ex, $"Correlation Id: {correlationId} - An error occurred rendering action {nameof(FinishWithJobCategory)}");
                return StatusCode(500);
            }
        }

        public async Task<IActionResult> Index()
        {
            var correlationId = Guid.NewGuid();
            try
            {
                var sessionId = await TryGetSessionId(Request);
                if (string.IsNullOrEmpty(sessionId))
                {
                    return Redirect("/");
                }

                var viewName = "Finish";
                var contentName = "finishpage";
                var model = await _apiServices.GetContentModel<FinishViewModel>(contentName, correlationId);
                AppendCookie(sessionId);
                return View(viewName, model);
            }
            catch (Exception ex)
            {
                _log.LogError(ex, $"Correlation Id: {correlationId} - An error occurred rendering action {nameof(Index)}");
                return StatusCode(500);
            }
        }
    }
}
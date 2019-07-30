using Dfc.DiscoverSkillsAndCareers.WebApp.Models;
using Dfc.DiscoverSkillsAndCareers.WebApp.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.DataProtection;

namespace Dfc.DiscoverSkillsAndCareers.WebApp.Controllers
{
    [Route("finish")]
    public class FinishController : BaseController
    {
        readonly ILogger<FinishController> _log;
        readonly IApiServices _apiServices;

        public FinishController(
            ILogger<FinishController> log,
            IApiServices apiServices, IDataProtectionProvider dataProtectionProvider) : base(dataProtectionProvider)
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
                var model = new FinishViewModel
                {
                    IsFilterAssessment = true,
                    JobCategorySafeUrl = jobCategory
                };
                AppendCookie(sessionId);
                return View(viewName, model);
            }
            catch (Exception ex)
            {
                _log.LogError(ex, $"Correlation Id: {correlationId} - An error occurred rendering action {nameof(FinishWithJobCategory)}");
                return RedirectToAction("Error500", "Error");
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
                var model = new FinishViewModel();
                AppendCookie(sessionId);
                return View(viewName, model);
            }
            catch (Exception ex)
            {
                _log.LogError(ex, $"Correlation Id: {correlationId} - An error occurred rendering action {nameof(Index)}");
                return RedirectToAction("Error500", "Error");
            }
        }
    }
}
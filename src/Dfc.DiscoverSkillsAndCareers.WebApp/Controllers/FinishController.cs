using Dfc.DiscoverSkillsAndCareers.WebApp.Models;
using Dfc.DiscoverSkillsAndCareers.WebApp.Services;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace Dfc.DiscoverSkillsAndCareers.WebApp.Controllers
{
    [Route("finish")]
    public class FinishController : BaseController
    {
        private readonly ILogger<FinishController> _log;
        private readonly ILayoutService _layoutService;

        public FinishController(ILogger<FinishController> log, IDataProtectionProvider dataProtectionProvider, ILayoutService layoutService) : base(dataProtectionProvider)
        {
            _log = log;
            _layoutService = layoutService;
        }

        [Route("{jobCategory}")]
        public async Task<IActionResult> FinishWithJobCategory(string jobCategory)
        {
            var correlationId = Guid.NewGuid();
            try
            {
                var sessionId = await TryGetSessionId();
                if (string.IsNullOrEmpty(sessionId))
                {
                    return Redirect("/");
                }

                var viewName = "FinishFilteredAssessment";
                var model = new FinishViewModel
                {
                    IsFilterAssessment = true,
                    JobCategorySafeUrl = jobCategory,
                    Layout = _layoutService.GetLayout(Request)
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
                var sessionId = await TryGetSessionId();
                if (string.IsNullOrEmpty(sessionId))
                {
                    return Redirect("/");
                }

                var viewName = "Finish";
                var model = new FinishViewModel
                {
                    Layout = _layoutService.GetLayout(Request)
                };

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
using Dfc.DiscoverSkillsAndCareers.WebApp.Models;
using Dfc.DiscoverSkillsAndCareers.WebApp.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace Dfc.DiscoverSkillsAndCareers.WebApp.Controllers
{
    [Route("information-sources")]
    public class InformationSourcesController : BaseController
    {
        readonly ILogger<InformationSourcesController> _log;
        readonly IApiServices _apiServices;

        public InformationSourcesController(
            ILogger<InformationSourcesController> log,
            IApiServices apiServices)
        {
            _log = log;
            _apiServices = apiServices;
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

                var viewName ="InformationSources";
                var contentName = "informationsources";
                var model = await _apiServices.GetContentModel<InformationSourcesViewModel>(contentName, correlationId);
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

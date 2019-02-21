using Dfc.DiscoverSkillsAndCareers.WebApp.Models;
using Dfc.DiscoverSkillsAndCareers.WebApp.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace Dfc.DiscoverSkillsAndCareers.WebApp.Controllers
{
    [Route("results")]
    public class ResultsController : BaseController
    {
        readonly ILogger<ResultsController> Logger;
        readonly IApiServices ApiServices;

        public ResultsController(ILogger<ResultsController> logger,
            IApiServices apiServices)
        {
            Logger = logger;
            ApiServices = apiServices;
        }

        public async Task<IActionResult> Index()
        {
            var sessionId = await TryGetSessionId(Request);

            if (string.IsNullOrEmpty(sessionId))
            {
                return Redirect("/");
            }

            var resultsResponse = await ApiServices.Results(sessionId);

            var contentName = $"{resultsResponse.AssessmentType.ToLower()}page";
            var model = await ApiServices.GetContentModel<ResultsViewModel>(contentName);
            model.SessionId = sessionId;
            model.AssessmentType = resultsResponse.AssessmentType;
            model.JobFamilies = resultsResponse.JobFamilies;
            model.JobFamilyCount = resultsResponse.JobFamilyCount;
            model.JobFamilyMoreCount = resultsResponse.JobFamilyMoreCount;
            model.Traits = resultsResponse.Traits;
            return View("Results", model);
        }
    }
}
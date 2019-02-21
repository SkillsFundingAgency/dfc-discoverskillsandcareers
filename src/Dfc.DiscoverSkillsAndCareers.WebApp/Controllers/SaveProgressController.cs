using Dfc.DiscoverSkillsAndCareers.WebApp.Models;
using Dfc.DiscoverSkillsAndCareers.WebApp.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace Dfc.DiscoverSkillsAndCareers.WebApp.Controllers
{
    [Route("save-my-progress")]
    public class SaveProgressController : BaseController
    {
        readonly ILogger<SaveProgressController> Logger;
        readonly IApiServices ApiServices;

        public SaveProgressController(ILogger<SaveProgressController> logger,
            IApiServices apiServices)
        {
            Logger = logger;
            ApiServices = apiServices;
        }

        public async Task<IActionResult> Index()
        {
            var sessionId = await TryGetSessionId(Request);
            var model = await ApiServices.GetContentModel<SaveProgressViewModel>("saveprogresspage");

            var nextQuestionResponse = await ApiServices.NextQuestion(sessionId);
            model.SessionId = sessionId;
            model.Code = nextQuestionResponse.ReloadCode;
            model.SessionDate = nextQuestionResponse.StartedDt.ToString("dd MMMM yyyy");
            model.Status = $"{nextQuestionResponse.RecordedAnswersCount} out of {nextQuestionResponse.MaxQuestionsCount} statements complete";
            Response.Cookies.Append("ncs-session-id", sessionId);
            return View("SaveProgress", model);
        }
    }
}
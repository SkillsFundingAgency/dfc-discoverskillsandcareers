using Dfc.DiscoverSkillsAndCareers.Services;
using Dfc.DiscoverSkillsAndCareers.WebApp.Models;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Dfc.DiscoverSkillsAndCareers.WebApp.Controllers
{
    [Route("save-my-progress")]
    public class SaveProgressController : Controller
    {
        readonly IUserSessionService UserSessionService;

        public SaveProgressController(IUserSessionService userSessionService)
        {
            UserSessionService = userSessionService;
        }

        public async Task<IActionResult> Index()
        {
            await UserSessionService.Init(Request);

            var model = new SaveProgressViewModel()
            {
                Code = UserSessionService.Session.UserSessionId,
                SessionDate = UserSessionService.Session.StartedDt.ToString("dd MMMM yyyy"),
                Status = $"{UserSessionService.Session.RecordedAnswers.Count} out of {UserSessionService.Session.MaxQuestions} statements complete",
                SessionId = UserSessionService.Session.PrimaryKey
            };
            return View("SaveProgress", model);
        }
    }
}
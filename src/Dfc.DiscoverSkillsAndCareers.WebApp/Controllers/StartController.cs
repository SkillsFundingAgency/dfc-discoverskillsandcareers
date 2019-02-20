using Dfc.DiscoverSkillsAndCareers.WebApp.Config;
using Dfc.DiscoverSkillsAndCareers.WebApp.Models;
using Dfc.DiscoverSkillsAndCareers.WebApp.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Threading.Tasks;

namespace Dfc.DiscoverSkillsAndCareers.WebApp.Controllers
{
    [Route("start")]
    public class StartController : Controller
    {
        readonly ILogger<StartController> Logger;
        readonly AppSettings AppSettings;
        readonly IApiServices ApiServices;

        public StartController(ILogger<StartController> logger,
            IOptions<AppSettings> appSettings,
            IApiServices apiServices)
        {
            Logger = logger;
            AppSettings = appSettings.Value;
            ApiServices = apiServices;
        }

        public async Task<IActionResult> Index()
        {
            var model = await ApiServices.GetContentModel<StartViewModel>("startpage");
            return View("Start", model);
        }
    }
}

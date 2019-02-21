using Dfc.DiscoverSkillsAndCareers.WebApp.Models;
using Dfc.DiscoverSkillsAndCareers.WebApp.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace Dfc.DiscoverSkillsAndCareers.WebApp.Controllers
{
    [Route("start")]
    public class StartController : Controller
    {
        readonly ILogger<StartController> Logger;
        readonly IApiServices ApiServices;

        public StartController(ILogger<StartController> logger,
            IApiServices apiServices)
        {
            Logger = logger;
            ApiServices = apiServices;
        }

        public async Task<IActionResult> Index()
        {
            var model = await ApiServices.GetContentModel<StartViewModel>("startpage");
            return View("Start", model);
        }
    }
}

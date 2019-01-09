using Microsoft.AspNetCore.Mvc;

namespace Dfc.DiscoverSkillsAndCareers.Landing
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Preamble()
        {
            return View();
        }
    }
}
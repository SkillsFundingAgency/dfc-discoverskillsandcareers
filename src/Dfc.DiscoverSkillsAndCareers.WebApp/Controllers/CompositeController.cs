using Microsoft.AspNetCore.Mvc;
using System;

namespace Dfc.DiscoverSkillsAndCareers.WebApp.Controllers
{
    [Route("composite")]
    public class CompositeController : BaseController
    {
        [Route("styles/{**article}")]
        public IActionResult Styles(string article)
        {
            var data = new[]
            {
                $"<link rel=\"preload\" href=\"https://localhost:5003/assets/fonts/light-f38ad40456-v1.woff2\" as=\"font\" type=\"font/woff2\" crossorigin />",
                $"<link rel=\"preload\" href=\"https://localhost:5003/assets/fonts/bold-a2452cb66f-v1.woff2\" as=\"font\" type=\"font/woff2\" crossorigin />",
                $"<link rel=\"stylesheet\" href=\"https://localhost:5003/assets/css/main.css\" />",
                $"<link rel=\"stylesheet\" href=\"https://localhost:5003/assets/css/print.css\" media=\"print\" />"
            };

            return new OkObjectResult(String.Join(Environment.NewLine, data));
        }
    }
}
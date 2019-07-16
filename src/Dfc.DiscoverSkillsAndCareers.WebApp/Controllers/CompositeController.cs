using System;
using System.Net;
using System.Net.Http;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;

namespace Dfc.DiscoverSkillsAndCareers.WebApp.Controllers
{
    [Route("composite")]
    public class CompositeController : BaseController
    {
        [Route("styles")]
        public IActionResult Styles()
        {
            var data = new[]
            {
                $"<link rel=\"preload\" href=\"{ApplyCompositeBasePath("/assets/fonts/light-f38ad40456-v1.woff2")}\" as=\"font\" type=\"font/woff2\" crossorigin />",
                $"<link rel=\"preload\" href=\"{ApplyCompositeBasePath("/assets/fonts/bold-a2452cb66f-v1.woff2")}\" as=\"font\" type=\"font/woff2\" crossorigin />",
                $"<link rel=\"stylesheet\" href=\"{ApplyCompositeBasePath("/assets/css/main.css")}\" />",
                $"<link rel=\"stylesheet\" href=\"{ApplyCompositeBasePath("/assets/css/print.css")}\" media=\"print\" />"
            };
            
            return new OkObjectResult(String.Join(Environment.NewLine, data));
        }
    }
}
using Dfc.DiscoverSkillsAndCareers.WebApp.Config;
using Microsoft.AspNetCore.Mvc;
using System;
using Microsoft.Extensions.Options;

namespace Dfc.DiscoverSkillsAndCareers.WebApp.Controllers
{
    [Route("composite")]
    public class CompositeController : BaseController
    {
        private readonly IOptions<AppSettings> settings;

        public CompositeController(IOptions<AppSettings> settings)
        {
            this.settings = settings;
        }

        [Route("styles/{**article}")]
        public IActionResult Styles(string article)
        {
            var data = new[]
            {
                $"<link rel=\"preload\" href=\"{settings?.Value.AssetsCDNUrl}/fonts/light-f38ad40456-v1.woff2\" as=\"font\" type=\"font/woff2\" crossorigin />",
                $"<link rel=\"preload\" href=\"{settings?.Value.AssetsCDNUrl}/fonts/bold-a2452cb66f-v1.woff2\" as=\"font\" type=\"font/woff2\" crossorigin />",
                $"<link rel=\"stylesheet\" href=\"{settings?.Value.AssetsCDNUrl}/css/main.css\" />",
                $"<link rel=\"stylesheet\" href=\"{settings?.Value.AssetsCDNUrl}/css/print.css\" media=\"print\" />",
                $"<script src=\"{settings?.Value.AssetsCDNUrl}/js/site.js\" type=\"text/javascript\"></script>"
            };

            return new OkObjectResult(String.Join(Environment.NewLine, data));
        }
    }
}
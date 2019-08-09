using Dfc.DiscoverSkillsAndCareers.WebApp.Config;
using Microsoft.AspNetCore.Mvc;
using System;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.Extensions.Options;

namespace Dfc.DiscoverSkillsAndCareers.WebApp.Controllers
{
    [Route("composite")]
    public class CompositeController : BaseController
    {
        private readonly IOptions<AppSettings> settings;

        public CompositeController(IOptions<AppSettings> settings, IDataProtectionProvider dataProtectionProvider)
            : base(dataProtectionProvider)
        {
            this.settings = settings;
        }

        [Route("styles/{**article}")]
        public IActionResult Styles(string article)
        {
            var data = new[]
            {
                $"<link rel=\"stylesheet\" href=\"{settings?.Value.AssetsCDNUrl}/css/main.css\" />",
                $"<link rel=\"stylesheet\" href=\"{settings?.Value.AssetsCDNUrl}/css/print.css\" media=\"print\" />",
                $"<script src=\"{settings?.Value.AssetsCDNUrl}/js/site.js\" type=\"text/javascript\"></script>"
            };

            return new OkObjectResult(String.Join(Environment.NewLine, data));
        }
    }
}
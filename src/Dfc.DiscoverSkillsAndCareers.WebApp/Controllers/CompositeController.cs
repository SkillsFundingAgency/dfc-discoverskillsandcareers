using Dfc.DiscoverSkillsAndCareers.WebApp.Config;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System;

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
                $"<meta name=\"viewport\" content=\"width=device-width, initial-scale=1\" />",
                $"<meta name=\"theme-color\" content=\"#0b0c0c\" />",
                $"<meta http-equiv=\"X-UA-Compatible\" content=\"IE=edge\" />",
                $"<link rel=\"stylesheet\" href=\"{settings?.Value.AssetsCDNUrl}/css/dysac.min.css\" />",
                $"<link rel=\"stylesheet\" href=\"{settings?.Value.AssetsCDNUrl}/css/dysacprint.min.css\" media=\"print\" />",
                $"<script src=\"{settings?.Value.AssetsCDNUrl}/js/dysac.min.js\" type=\"text/javascript\"></script>"
            };

            return new OkObjectResult(String.Join(Environment.NewLine, data));
        }
    }
}
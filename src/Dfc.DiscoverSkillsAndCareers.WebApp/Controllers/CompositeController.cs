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
        [Route("styles/{**article}")]
        public IActionResult Styles(string article)
        {
            var data = new[]
            {
                $"<link rel=\"stylesheet\" href=\"{settings?.Value.AssetsCDNUrl}/css/dysac.min.css\" />",
                $"<link rel=\"stylesheet\" href=\"{settings?.Value.AssetsCDNUrl}/css/dysacprint.min.css\" media=\"print\" />",
                $"<script src=\"{settings?.Value.AssetsCDNUrl}/js/dysac.min.js\" type=\"text/javascript\"></script>"
            };

            return new OkObjectResult(String.Join(Environment.NewLine, data));
        }
    }
}
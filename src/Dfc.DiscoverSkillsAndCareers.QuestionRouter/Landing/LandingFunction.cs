using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net;

namespace Dfc.DiscoverSkillsAndCareers.QuestionRouter.Landing
{
    public static class LandingFunction
    {
        [FunctionName("LandingFunction")]
        public static async Task<HttpResponseMessage> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = "index")] HttpRequestMessage req,
            ILogger log)
        {
            log.LogInformation("LandingFunction processed a request.");

            string html = "<body><a href='/q/1'>Get started</a></body>";

            var response = req.CreateResponse(HttpStatusCode.OK);
            response.Content = new StringContent(html);
            response.Content.Headers.ContentType = new MediaTypeHeaderValue("text/html");
            return response;
        }
    }
}

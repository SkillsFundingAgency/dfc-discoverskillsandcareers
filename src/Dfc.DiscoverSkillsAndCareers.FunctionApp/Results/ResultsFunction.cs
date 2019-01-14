using System;
using System.Threading.Tasks;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;

namespace Dfc.DiscoverSkillsAndCareers.FunctionApp.Results
{
    public static class ResultsFunction
    {
        [FunctionName("ResultsFunction")]
        public static async Task<HttpResponseMessage> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = "results")] HttpRequestMessage req,
            ILogger log, ExecutionContext context)
        {
            var appSettings = ConfigurationHelper.ReadConfiguration(context);
            log.LogDebug($"ResultsFunction appSettings={Newtonsoft.Json.JsonConvert.SerializeObject(appSettings)}");

            var sessionHelper = await SessionHelper.CreateWithInit(req, appSettings);
       
            if (!sessionHelper.HasSession)
            {
                // TODO:
                throw new Exception("Session not found");
            }

            CalculateResult.Run(sessionHelper.Session);

            await sessionHelper.UpdateSession();

            var html = new BuildPageHtml(sessionHelper).Html;

            // Ok html response
            var response = req.CreateResponse(HttpStatusCode.OK);
            response.Content = new StringContent(html);
            response.Content.Headers.ContentType = new MediaTypeHeaderValue("text/html");
            return response;
        }
    }
}

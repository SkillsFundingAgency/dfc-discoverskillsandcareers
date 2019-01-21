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
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = "results")] HttpRequestMessage req,
            ILogger log, ExecutionContext context)
        {
            try
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

                // Build page html
                string blobName = "results.html";
                var templateHtml = BlobStorageHelper.GetBlob(sessionHelper.Config.BlobStorage, blobName).Result;
                if (templateHtml == null)
                {
                    throw new Exception($"Blob {blobName} could not be found");
                }
                var html = new BuildPageHtml(templateHtml, sessionHelper).Html;

                // Ok html response
                var response = req.CreateResponse(HttpStatusCode.OK);
                response.Content = new StringContent(html);
                response.Content.Headers.ContentType = new MediaTypeHeaderValue("text/html");
                return response;
            }

            catch (Exception ex)
            {
                log.LogError(ex, "ResultsFunction run");
                throw;
            }
        }
    }
}

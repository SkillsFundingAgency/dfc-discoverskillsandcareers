using System;
using System.Threading.Tasks;
using System.Net.Http;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using static Dfc.DiscoverSkillsAndCareers.FunctionApp.Helpers.HttpResponseHelpers;

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
                    // No session so redirect to question #1 and generate a new session
                    return RedirectStartAtQuestionOne(req);
                }

                // Return the results or last unanswered question page if not complete
                return await ResultsOrLastQuestionPage(req, sessionHelper);
            }

            catch (Exception ex)
            {
                log.LogError(ex, "ResultsFunction run");
                throw;
            }
        }

        public static async Task<HttpResponseMessage> ResultsOrLastQuestionPage(HttpRequestMessage req, SessionHelper sessionHelper)
        {
            if (!sessionHelper.Session.IsComplete)
            {
                // Session is not complete so continue where we was last
                return RedirectToQuestionNumber(req, sessionHelper);
            }

            CalculateResult.Run(sessionHelper.Session);

            await sessionHelper.UpdateSession();

            return CreateResultsPage(req, sessionHelper);
        }

        public static HttpResponseMessage CreateResultsPage(HttpRequestMessage req, SessionHelper sessionHelper)
        {
            // Build page html from the template blob
            string blobName = "results.html";
            var templateHtml = BlobStorageHelper.GetBlob(sessionHelper.Config.BlobStorage, blobName).Result;
            if (templateHtml == null)
            {
                throw new Exception($"Blob {blobName} could not be found");
            }
            var html = new BuildPageHtml(templateHtml, sessionHelper).Html;

            // Ok html response
            return new HttpHtmlWithSessionCookieResponse(req, html, sessionHelper.Session.PrimaryKey);
        }
    }
}

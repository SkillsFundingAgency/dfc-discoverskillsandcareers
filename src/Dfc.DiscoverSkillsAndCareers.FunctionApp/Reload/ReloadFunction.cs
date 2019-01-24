using System;
using System.Threading.Tasks;
using System.Net.Http;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using System.Linq;
using Dfc.DiscoverSkillsAndCareers.FunctionApp.Helpers;
using static Dfc.DiscoverSkillsAndCareers.FunctionApp.Helpers.HttpResponseHelpers;

namespace Dfc.DiscoverSkillsAndCareers.FunctionApp.Reload
{
    public static class ReloadFunction
    {
        [FunctionName("ReloadFunction")]
        public static async Task<HttpResponseMessage> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", "get", Route = "reload")] HttpRequestMessage req,
            ILogger log, ExecutionContext context)
        {
            try
            {
                var appSettings = ConfigurationHelper.ReadConfiguration(context);
                log.LogDebug($"ReloadFunction appSettings={Newtonsoft.Json.JsonConvert.SerializeObject(appSettings)}");

                var sessionHelper = await SessionHelper.CreateWithInit(req, appSettings);
                var code = sessionHelper.FormData?.GetValues("code").FirstOrDefault();
                if (string.IsNullOrEmpty(code) == false)
                {
                    await sessionHelper.Reload(code);
                }
                if (!sessionHelper.HasSession)
                {
                    // Build page html from the template blob
                    string blobName = "index.html";
                    var templateHtml = BlobStorageHelper.GetBlob(sessionHelper.Config.BlobStorage, blobName).Result;
                    if (templateHtml == null)
                    {
                        throw new Exception($"Blob {blobName} could not be found in {sessionHelper.Config.BlobStorage.ContainerName}");
                    }
                    string html = templateHtml;
                    html = html.Replace("/assets/css/main", $"{sessionHelper.Config.StaticSiteDomain}/assets/css/main");
                    html = html.Replace("<span id=\"codeNotFoundMessage\" style=\"display:none\"", "<span id=\"codeNotFoundMessage\" style=\"display:block\"");
                    return OKHtmlWithCookie(req, html, null);
                }
                // We have a session from our code, so display the last point
                return await Results.ResultsFunction.ResultsOrLastQuestionPage(req, sessionHelper);
            }

            catch (Exception ex)
            {
                log.LogError(ex, "ReloadFunction run");
                throw;
            }
        }
    }
}

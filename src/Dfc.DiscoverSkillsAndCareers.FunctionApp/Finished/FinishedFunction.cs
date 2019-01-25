using System;
using System.Threading.Tasks;
using System.Net.Http;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Dfc.DiscoverSkillsAndCareers.FunctionApp.Helpers;
using static Dfc.DiscoverSkillsAndCareers.FunctionApp.Helpers.HttpResponseHelpers;

namespace Dfc.DiscoverSkillsAndCareers.FunctionApp.Finish
{
    public static class FinishFunction
    {
        [FunctionName("FinishFunction")]
        public static async Task<HttpResponseMessage> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = "finish")] HttpRequestMessage req,
            ILogger log, ExecutionContext context)
        {
            try
            {
                var appSettings = ConfigurationHelper.ReadConfiguration(context);
                log.LogDebug($"FinishFunction appSettings={Newtonsoft.Json.JsonConvert.SerializeObject(appSettings)}");

                var sessionHelper = await SessionHelper.CreateWithInit(req, appSettings);
                if (!sessionHelper.HasSession)
                {
                    return RedirectToNewSession(req);
                }

                // Build page html from the template blob
                string blobName = "finish.html";
                var templateHtml = BlobStorageHelper.GetBlob(sessionHelper.Config.BlobStorage, blobName).Result;
                if (templateHtml == null)
                {
                    throw new Exception($"Blob {blobName} could not be found");
                }
                string html = templateHtml;
                html = html.Replace("/assets/css/main", $"{sessionHelper.Config.StaticSiteDomain}/assets/css/main");
                html = html.Replace("[session_id]", sessionHelper.Session.PrimaryKey);
                html = html.Replace("[static_site_domain]", sessionHelper.Config.StaticSiteDomain);

                // Ok html response
                return OKHtmlWithCookie(req, html, sessionHelper.Session.PrimaryKey);
            }

            catch (Exception ex)
            {
                log.LogError(ex, "FinishFunction run");
                return InternalServerError(req, context);
            }
        }
    }
}

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
                if (code == "500test")
                {
                    throw new Exception("500 test");
                }
                if (string.IsNullOrEmpty(code) == false)
                {
                    await sessionHelper.Reload(code);
                }
                if (string.IsNullOrEmpty(code?.Trim()) || !sessionHelper.HasSession)
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
                    html = html.Replace("/information-sources.html", $"{sessionHelper.Config.StaticSiteDomain}/information-sources.html");
                    html = html.Replace("<div class=\"app-resume-panel__input govuk-form-group\"", "<div class=\"app-resume-panel__input govuk-form-group govuk-form-group--error\"");
                    html = html.Replace("Your reference number</label>", "Your reference number</label><span id=\"code-error\" class=\"govuk-error-message\">The code could not be found</span>");
                    html = html.Replace("<input class=\"govuk-input\" id=\"code\" name=\"code\" type=\"text\"", "<input class=\"govuk-input govuk-input--error\" id=\"code\" name=\"code\" type=\"text\" aria-describedby=\"code-error\"");
                    html = html.Replace("/start.html", $"{sessionHelper.Config.StaticSiteDomain}/start.html");
                    return OKHtmlWithCookie(req, html, null);
                }
                // We have a session from our code, so display the last point
                return await Results.ResultsFunction.ResultsOrLastQuestionPage(req, sessionHelper);
            }

            catch (Exception ex)
            {
                log.LogError(ex, "ReloadFunction run");
                return InternalServerError(req, context);
            }
        }
    }
}

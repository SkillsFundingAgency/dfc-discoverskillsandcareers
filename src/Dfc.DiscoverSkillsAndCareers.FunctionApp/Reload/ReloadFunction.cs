using System;
using System.Threading.Tasks;
using System.Net.Http;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using System.Linq;
using Dfc.DiscoverSkillsAndCareers.FunctionApp.Helpers;

namespace Dfc.DiscoverSkillsAndCareers.FunctionApp.Reload
{
    public static class ReloadFunction
    {
        [FunctionName("ReloadFunction")]
        public static async Task<HttpResponseMessage> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "reload")] HttpRequestMessage req,
            ILogger log, ExecutionContext context)
        {
            try
            {
                var appSettings = ConfigurationHelper.ReadConfiguration(context);
                log.LogDebug($"ReloadFunction appSettings={Newtonsoft.Json.JsonConvert.SerializeObject(appSettings)}");

                var sessionHelper = await SessionHelper.CreateWithInit(req, appSettings);
                var code = sessionHelper.FormData.GetValues("code").FirstOrDefault();
                await sessionHelper.Reload(code);
                if (!sessionHelper.HasSession)
                {
                    // TODO: return not found page (waiting on design)
                    throw new Exception("Session not found");
                }

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

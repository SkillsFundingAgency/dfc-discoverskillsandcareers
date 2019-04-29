using System;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Dfc.DiscoverSkillsAndCareers.Models;
using Dfc.DiscoverSkillsAndCareers.Repositories;
using DFC.Functions.DI.Standard.Attributes;
using DFC.HTTP.Standard;
using DFC.Swagger.Standard.Annotations;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace Dfc.DiscoverSkillsAndCareers.AssessmentFunctionApp.AssessmentApi
{
    public static class ImportTestSessionHttpTrigger
    {
        [FunctionName("ImportTestSessionHttpTrigger")]
        [ProducesResponseType(typeof(Question), (int) HttpStatusCode.OK)]
        [Response(HttpStatusCode = (int) HttpStatusCode.OK, Description = "Import test sessions", ShowSchema = true)]
        [Response(HttpStatusCode = (int) HttpStatusCode.BadRequest, Description = "Request was malformed",
            ShowSchema = false)]
        [Response(HttpStatusCode = (int) HttpStatusCode.Unauthorized, Description = "API key is unknown or invalid",
            ShowSchema = false)]
        [Response(HttpStatusCode = (int) HttpStatusCode.Forbidden, Description = "Insufficient access",
            ShowSchema = false)]
        [Display(Name = "ImportTestSessionProfiles", Description = "Imports a test session")]

        public static async Task<HttpResponseMessage> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "import/test-session")]
            HttpRequest req,
            ILogger log,
            [Inject] IHttpRequestHelper httpRequestHelper,
            [Inject] IHttpResponseMessageHelper httpResponseMessageHelper,
            [Inject] IUserSessionRepository userSessionRepository,
            [Inject] IOptions<AppSettings> appSettings
        )
        {
            string inputJson = "";
            using (var streamReader = new StreamReader(req.Body))
            {
                inputJson = streamReader.ReadToEnd();
            }

            try
            {
                req.Headers.TryGetValue("import-key", out var headerValue);
                if (headerValue.ToString() != "Hgfy-gYh3") return httpResponseMessageHelper.BadRequest();

                var data = JsonConvert.DeserializeObject<UserSession>(inputJson);
                var sessionId = SessionIdHelper.GenerateSessionId(appSettings.Value.SessionSalt, DateTime.UtcNow);

                data.UserSessionId = sessionId;
                data.Salt = appSettings.Value.SessionSalt;
                await userSessionRepository.CreateUserSession(data);

                return httpResponseMessageHelper.Ok(JsonConvert.SerializeObject(new
                {
                    message = "Ok",
                    sessionCreated = sessionId
                }));
            }

            catch (Exception ex)
            {
                return httpResponseMessageHelper.Ok(JsonConvert.SerializeObject(new
                {
                    message = ex.ToString()
                }));
            }
        }
    }
}
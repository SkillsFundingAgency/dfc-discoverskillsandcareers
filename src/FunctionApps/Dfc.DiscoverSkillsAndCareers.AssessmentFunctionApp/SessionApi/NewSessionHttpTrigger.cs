using Dfc.DiscoverSkillsAndCareers.FunctionApp.Helpers;
using Dfc.DiscoverSkillsAndCareers.FunctionApp.Models;
using Dfc.DiscoverSkillsAndCareers.Models;
using Dfc.DiscoverSkillsAndCareers.Repositories;
using DFC.Common.Standard.Logging;
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
using System;
using System.ComponentModel.DataAnnotations;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace Dfc.DiscoverSkillsAndCareers.FunctionApp
{
    public static class NewSessionHttpTrigger
    {
        [FunctionName("NewSessionHttpTrigger")]
        [ProducesResponseType(typeof(DscSession), (int)HttpStatusCode.OK)]
        [Response(HttpStatusCode = (int)HttpStatusCode.OK, Description = "Creates a new user session", ShowSchema = true)]
        [Response(HttpStatusCode = (int)HttpStatusCode.Unauthorized, Description = "API key is unknown or invalid", ShowSchema = false)]
        [Response(HttpStatusCode = (int)HttpStatusCode.Forbidden, Description = "Insufficient access", ShowSchema = false)]
        [Display(Name = "Get", Description = "Creates a new user session and returns the session details")]

        public static async Task<HttpResponseMessage> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "session")]HttpRequest req,
            ILogger log,
            [Inject]ILoggerHelper loggerHelper,
            [Inject]IHttpRequestHelper httpRequestHelper,
            [Inject]IHttpResponseMessageHelper httpResponseMessageHelper,
            [Inject]IUserSessionRepository userSessionRepository,
            [Inject]IQuestionRepository questionRepository,
            [Inject]IOptions<AppSettings> appSettings)
        {
            loggerHelper.LogMethodEnter(log);

            var correlationId = httpRequestHelper.GetDssCorrelationId(req);
            if (string.IsNullOrEmpty(correlationId))
                log.LogInformation("Unable to locate 'DssCorrelationId' in request header");

            if (!Guid.TryParse(correlationId, out var correlationGuid))
            {
                log.LogInformation("Unable to parse 'DssCorrelationId' to a Guid");
                correlationGuid = Guid.NewGuid();
            }

            var currentQuestionSetInfo = await questionRepository.GetCurrentQuestionSetVersion();

            string partitionKey = DateTime.Now.ToString("yyyyMM");
            string salt = appSettings.Value.SessionSalt;
            var userSession = new UserSession()
            {
                UserSessionId = SessionIdHelper.GenerateSessionId(salt),
                Salt = salt,
                StartedDt = DateTime.Now,
                LanguageCode = "en",
                PartitionKey = partitionKey,
                QuestionSetVersion = currentQuestionSetInfo.QuestionSetVersion,
                MaxQuestions = currentQuestionSetInfo.MaxQuestions,
                CurrentQuestion = 1,
                AssessmentType = currentQuestionSetInfo.AssessmentType.ToLower()
            };
            await userSessionRepository.CreateUserSession(userSession);

            loggerHelper.LogMethodExit(log);

            var result = new DscSession()
            {
                SessionId = userSession.PrimaryKey
            };
            return httpResponseMessageHelper.Ok(JsonConvert.SerializeObject(result));
        }
    }
}

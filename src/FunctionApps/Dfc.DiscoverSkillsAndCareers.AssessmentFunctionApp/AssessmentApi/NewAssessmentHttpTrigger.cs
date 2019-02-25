using Dfc.DiscoverSkillsAndCareers.AssessmentFunctionApp.Models;
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

namespace Dfc.DiscoverSkillsAndCareers.AssessmentFunctionApp
{
    public static class NewAssessmentHttpTrigger
    {
        [FunctionName("NewAssessmentHttpTrigger")]
        [ProducesResponseType(typeof(DscSession), (int)HttpStatusCode.OK)]
        [Response(HttpStatusCode = (int)HttpStatusCode.OK, Description = "Creates a new assessment session", ShowSchema = true)]
        [Response(HttpStatusCode = (int)HttpStatusCode.Unauthorized, Description = "API key is unknown or invalid", ShowSchema = false)]
        [Response(HttpStatusCode = (int)HttpStatusCode.Forbidden, Description = "Insufficient access", ShowSchema = false)]
        [Display(Name = "NewAssessment", Description = "Creates a new assessment session and returns the assessment session details")]

        public static async Task<HttpResponseMessage> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "assessment")]HttpRequest req,
            ILogger log,
            // [Inject]ILoggerHelper loggerHelper,
            [Inject]IHttpRequestHelper httpRequestHelper,
            [Inject]IHttpResponseMessageHelper httpResponseMessageHelper,
            [Inject]IUserSessionRepository userSessionRepository,
            [Inject]IQuestionRepository questionRepository,
            [Inject]IOptions<AppSettings> appSettings)
        {
            Console.WriteLine("Run");

            Guid correlationGuid;
            try
            {
                // loggerHelper.LogMethodEnter(log);

                var correlationId = httpRequestHelper.GetDssCorrelationId(req);
                if (string.IsNullOrEmpty(correlationId))
                    log.LogInformation("Unable to locate 'DssCorrelationId' in request header");

                if (!Guid.TryParse(correlationId, out correlationGuid))
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

                // loggerHelper.LogMethodExit(log);

                var result = new DscSession()
                {
                    SessionId = userSession.PrimaryKey
                };
                return httpResponseMessageHelper.Ok(JsonConvert.SerializeObject(result));
            }

            catch (Exception ex)
            {
                correlationGuid = Guid.NewGuid();
                // loggerHelper.LogException(log, correlationGuid, ex);

                return new HttpResponseMessage()
                {
                    StatusCode = HttpStatusCode.InternalServerError,
                    Content = new StringContent(ex.ToString())
                };
            }
        }
    }
}

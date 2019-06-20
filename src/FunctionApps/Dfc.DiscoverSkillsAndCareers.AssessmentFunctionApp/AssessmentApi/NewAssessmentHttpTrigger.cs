using System;
using System.ComponentModel.DataAnnotations;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Dfc.DiscoverSkillsAndCareers.AssessmentFunctionApp.Models;
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
    public static class NewAssessmentHttpTrigger
    {
        [FunctionName("NewAssessmentHttpTrigger")]
        [ProducesResponseType(typeof(FilterSessionResponse), (int)HttpStatusCode.OK)]
        [Response(HttpStatusCode = (int)HttpStatusCode.OK, Description = "Creates a new assessment session", ShowSchema = true)]
        [Response(HttpStatusCode = (int)HttpStatusCode.Unauthorized, Description = "API key is unknown or invalid", ShowSchema = false)]
        [Response(HttpStatusCode = (int)HttpStatusCode.Forbidden, Description = "Insufficient access", ShowSchema = false)]
        [Display(Name = "Get", Description = "Creates a new assessment session and returns the assessment session details")]

        public static async Task<HttpResponseMessage> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "assessment")]HttpRequest req,
            ILogger log,
            [Inject]IHttpRequestHelper httpRequestHelper,
            [Inject]IHttpResponseMessageHelper httpResponseMessageHelper,
            [Inject]IUserSessionRepository userSessionRepository,
            [Inject]IQuestionSetRepository questionSetRepository,
            [Inject]IOptions<AppSettings> appSettings)
        {
            try
            {
                var correlationId = httpRequestHelper.GetDssCorrelationId(req);
                if (string.IsNullOrEmpty(correlationId))
                {
                    log.LogWarning("Unable to locate 'DssCorrelationId' in request header");
                    correlationId = Guid.NewGuid().ToString();
                }

                log.LogInformation($"CorrelationId: {correlationId} - Creating a new assessment");

                // Get the assessmentType and questionSetTitle values from the query string
                var queryDictionary = System.Web.HttpUtility.ParseQueryString(req.QueryString.ToString());
                var assessmentType = queryDictionary.Get("assessmentType");
                if (string.IsNullOrEmpty(assessmentType))
                {
                    log.LogInformation($"CorrelationId: {correlationId} - Missing assessmentType {assessmentType}");
                    return httpResponseMessageHelper.BadRequest();
                }

                // Get the current question set version for this assesssment type and title (supplied by CMS - configured in appsettings)
                var currentQuestionSetInfo = await questionSetRepository.GetCurrentQuestionSet(assessmentType);
                if (currentQuestionSetInfo == null)
                {
                    log.LogInformation($"CorrelationId: {correlationId} - Unable to load latest question set {assessmentType}");
                    return httpResponseMessageHelper.NoContent();
                }

                // Create a new user session
                string partitionKey = DateTime.Now.ToString("yyyyMM");
                string salt = appSettings.Value.SessionSalt;
                var userSession = new UserSession()
                {
                    UserSessionId = SessionIdHelper.GenerateSessionId(salt),
                    Salt = salt,
                    StartedDt = DateTime.Now,
                    LanguageCode = "en",
                    PartitionKey = partitionKey,
                    AssessmentState = new AssessmentState(currentQuestionSetInfo.QuestionSetVersion, currentQuestionSetInfo.MaxQuestions),
                    AssessmentType = currentQuestionSetInfo.AssessmentType.ToLower()
                };
                await userSessionRepository.CreateUserSession(userSession);

                log.LogInformation($"CorrelationId: {correlationId} - Finished creating new assessment {userSession.UserSessionId}");

                var result = new FilterSessionResponse()
                {
                    SessionId = userSession.PrimaryKey
                };
                return httpResponseMessageHelper.Ok(JsonConvert.SerializeObject(result));
            }
            catch (Exception ex)
            {
                log.LogError(ex, "Fatal exception {message}", ex.ToString());
                return new HttpResponseMessage { StatusCode = HttpStatusCode.InternalServerError };
            }
        }
    }
}

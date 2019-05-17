using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
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
    public static class AssessmentReloadHttpTrigger
    {
        [FunctionName(nameof(AssessmentReloadHttpTrigger))]
        [ProducesResponseType(typeof(AssessmentQuestionResponse), (int)HttpStatusCode.OK)]
        [Response(HttpStatusCode = (int)HttpStatusCode.OK, Description = "Gets the given question for a given user session", ShowSchema = true)]
        [Response(HttpStatusCode = (int)HttpStatusCode.NotFound, Description = "No such session exists", ShowSchema = false)]
        [Response(HttpStatusCode = (int)HttpStatusCode.BadRequest, Description = "The request is malformed", ShowSchema = false)]
        [Response(HttpStatusCode = (int)HttpStatusCode.Unauthorized, Description = "API key is unknown or invalid", ShowSchema = false)]
        [Response(HttpStatusCode = (int)HttpStatusCode.Forbidden, Description = "Insufficient access", ShowSchema = false)]
        [Display(Name = "GET - Assessment Question", Description = "")]

        public static async Task<HttpResponseMessage> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "assessment/{sessionId}/reload")]HttpRequest req, 
            string sessionId,
            ILogger log,
            [Inject]IHttpRequestHelper httpRequestHelper,
            [Inject]IHttpResponseMessageHelper httpResponseMessageHelper,
            [Inject]IUserSessionRepository userSessionRepository,
            [Inject]IQuestionRepository questionRepository,
            [Inject]IOptions<AppSettings> appSettings)
        {
            try
            {
                var correlationId = httpRequestHelper.GetDssCorrelationId(req);
                if (string.IsNullOrEmpty(correlationId))
                    log.LogInformation("Unable to locate 'DssCorrelationId' in request header");

                if (!Guid.TryParse(correlationId, out var correlationGuid))
                {
                    log.LogInformation("Unable to parse 'DssCorrelationId' to a Guid");
                    correlationGuid = Guid.NewGuid();
                }

                if (string.IsNullOrEmpty(sessionId))
                {
                    log.LogInformation($"CorrelationId: {correlationGuid} - Session Id not supplied");
                    return httpResponseMessageHelper.BadRequest();
                }

                if (string.IsNullOrEmpty(appSettings?.Value.SessionSalt))
                {
                    log.LogInformation($"CorrelationId: {correlationGuid} -Session salt not missing from configuration");
                    return httpResponseMessageHelper.BadRequest();
                }

                if (!sessionId.Contains("-"))
                {
                    var datetimeStamp = SessionIdHelper.Decode(appSettings?.Value.SessionSalt, sessionId);
                    if (datetimeStamp == null)
                    {
                        log.LogError($"CorrelationId: {correlationGuid} - Session Id does not exist {sessionId}");
                        return httpResponseMessageHelper.BadRequest();
                    }
                    string partitionKey = SessionIdHelper.GetYearMonth(datetimeStamp);
                    sessionId = $"{partitionKey}-{sessionId}";
                }

                var userSession = await userSessionRepository.GetUserSession(sessionId);
                if (userSession == null)
                {
                    log.LogError($"CorrelationId: {correlationGuid} - Session Id does not exist {sessionId}");
                    return httpResponseMessageHelper.NoContent();
                }

                var questionSetVersion = userSession.CurrentQuestionSetVersion;

                var question = await questionRepository.GetQuestion(userSession.CurrentQuestion, questionSetVersion);
                if (question == null)
                {
                    log.LogInformation($"CorrelationId: {correlationGuid} - Question number {userSession.CurrentQuestion} could not be found on session {userSession.PrimaryKey}");
                    return httpResponseMessageHelper.NoContent();
                }

                int percentComplete = Convert.ToInt32((((decimal)userSession.CurrentQuestion - 1M) / (decimal)userSession.MaxQuestions) * 100);
                var response = new AssessmentQuestionResponse()
                {
                    CurrentFilterAssessmentCode = userSession.FilteredAssessmentState?.CurrentFilterAssessmentCode,
                    IsComplete = userSession.IsComplete,
                    NextQuestionNumber = userSession.FindNextQuestionToAnswer(),
                    QuestionId = question.QuestionId,
                    QuestionText = question.Texts.FirstOrDefault(x => x.LanguageCode.ToLower() == "en")?.Text,
                    TraitCode = question.TraitCode,
                    QuestionNumber = question.Order,
                    SessionId = userSession.PrimaryKey,
                    PercentComplete = percentComplete,
                    ReloadCode = userSession.UserSessionId,
                    MaxQuestionsCount = userSession.MaxQuestions,
                    RecordedAnswersCount = userSession.CurrentRecordedAnswers.Count(),
                    StartedDt = userSession.StartedDt,
                    IsFilterAssessment = userSession.IsFilterAssessment,
                    JobCategorySafeUrl = (userSession.CurrentState as FilteredAssessmentState)?.JobFamilyNameUrlSafe
                };

                return httpResponseMessageHelper.Ok(JsonConvert.SerializeObject(response));
            }
            catch (Exception ex)
            {
                log.LogError(ex, "Fatal exception {message}", ex.Message);
                return new HttpResponseMessage { StatusCode = HttpStatusCode.InternalServerError };
            }
        }
    }
}
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
    public static class AssessmentQuestionHttpTrigger
    {
        [FunctionName(nameof(AssessmentQuestionHttpTrigger))]
        [ProducesResponseType(typeof(AssessmentQuestionResponse), (int)HttpStatusCode.OK)]
        [Response(HttpStatusCode = (int)HttpStatusCode.OK, Description = "Gets the given question for a given user session", ShowSchema = true)]
        [Response(HttpStatusCode = (int)HttpStatusCode.NotFound, Description = "No such session exists", ShowSchema = false)]
        [Response(HttpStatusCode = (int)HttpStatusCode.BadRequest, Description = "The request is malformed", ShowSchema = false)]
        [Response(HttpStatusCode = (int)HttpStatusCode.Unauthorized, Description = "API key is unknown or invalid", ShowSchema = false)]
        [Response(HttpStatusCode = (int)HttpStatusCode.Forbidden, Description = "Insufficient access", ShowSchema = false)]
        [Display(Name = "GET - Assessment Question", Description = "")]

        public static async Task<HttpResponseMessage> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "assessment/{sessionId}/{assessment}/q/{questionNumber}")]HttpRequest req, 
            string sessionId,
            string assessment,
            int questionNumber,
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

                if (string.IsNullOrEmpty(assessment))
                {
                    log.LogInformation($"CorrelationId: {correlationGuid} - Assessment not supplied");
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
                        log.LogError($"CorrelationId: {correlationGuid} - Could not decode session id correctly: {sessionId}");
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

                if (!userSession.TrySetStateToExistingSession(assessment))
                {
                    log.LogError($"CorrelationId: {correlationGuid} - Unable to set assessment - {assessment} on session {sessionId}");
                    return httpResponseMessageHelper.NoContent();
                }

                var questionSetVersion = userSession.CurrentQuestionSetVersion;

                var question = await questionRepository.GetQuestion(questionNumber, questionSetVersion);
                if (question == null)
                {
                    log.LogInformation($"CorrelationId: {correlationGuid} - Question number {userSession.CurrentQuestion} could not be found on session {userSession.PrimaryKey}");
                    return httpResponseMessageHelper.NoContent();
                }

                await userSessionRepository.UpdateUserSession(userSession);

                int percentComplete = Convert.ToInt32((((decimal)questionNumber - 1M) / (decimal)userSession.MaxQuestions) * 100);
                var response = new AssessmentQuestionResponse()
                {
                    CurrentFilterAssessmentCode = userSession.FilteredAssessmentState?.CurrentFilterAssessmentCode,
                    IsComplete = userSession.IsComplete,
                    NextQuestionNumber = userSession.FindNextUnansweredQuestion(),
                    QuestionId = question.QuestionId,
                    QuestionText = question.Texts.FirstOrDefault(x => x.LanguageCode.ToLower() == "en")?.Text,
                    TraitCode = question.TraitCode,
                    QuestionNumber = question.Order,
                    SessionId = userSession.PrimaryKey,
                    PercentComplete = percentComplete,
                    ReloadCode = userSession.UserSessionId,
                    MaxQuestionsCount = userSession.MaxQuestions,
                    RecordedAnswersCount = userSession.CurrentRecordedAnswers.Count(),
                    RecordedAnswer = userSession.CurrentRecordedAnswers.SingleOrDefault(r => r.QuestionNumber == questionNumber)?.SelectedOption,
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

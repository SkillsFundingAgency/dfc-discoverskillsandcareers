using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Dfc.DiscoverSkillsAndCareers.AssessmentFunctionApp.Models;
using Dfc.DiscoverSkillsAndCareers.Models;
using Dfc.DiscoverSkillsAndCareers.Models.Extensions;
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

     Question question = null;
                
                if (assessment.EqualsIgnoreCase("short"))
                {
                    question = await questionRepository.GetQuestion(questionNumber, userSession.CurrentQuestionSetVersion);
                    userSession.AssessmentState.CurrentQuestion = questionNumber;
                } 
                else 
                {
                    userSession.FilteredAssessmentState.CurrentFilterAssessmentCode = JobCategoryHelper.GetCode(assessment);
                    userSession.FilteredAssessmentState.CurrentQuestion = questionNumber;
                    
                    question = await questionRepository.GetQuestion(userSession.FilteredAssessmentState.CurrentQuestionId);

                }
                
                if (question == null)
                {
                    log.LogInformation($"CorrelationId: {correlationGuid} - Question number {userSession.CurrentQuestion} could not be found on session {userSession.PrimaryKey}");
                    return httpResponseMessageHelper.NoContent();
                }

                var percentageComplete = userSession.AssessmentState.PercentageComplete;
                
                var nextQuestion = question.IsFilterQuestion ? userSession.FilteredAssessmentState.MoveToNextQuestion() : userSession.AssessmentState.MoveToNextQuestion();
                await userSessionRepository.UpdateUserSession(userSession);

                var response = new AssessmentQuestionResponse()
                {
                    CurrentFilterAssessmentCode = userSession.FilteredAssessmentState?.CurrentFilterAssessmentCode,
                    IsComplete = question.IsFilterQuestion ? (userSession.FilteredAssessmentState?.IsComplete ?? false) : userSession.AssessmentState.IsComplete,
                    NextQuestionNumber = nextQuestion,
                    QuestionId = question.QuestionId,
                    QuestionText = question.Texts.FirstOrDefault(x => x.LanguageCode.ToLower() == "en")?.Text,
                    TraitCode = question.TraitCode,
                    QuestionNumber = questionNumber,
                    SessionId = userSession.PrimaryKey,
                    PercentComplete = percentageComplete,
                    ReloadCode = userSession.UserSessionId,
                    MaxQuestionsCount = userSession.MaxQuestions,
                    RecordedAnswersCount = userSession.RecordedAnswers.Length,
                    RecordedAnswer = userSession.RecordedAnswers.SingleOrDefault(r => r?.QuestionNumber == questionNumber)?.SelectedOption,
                    StartedDt = userSession.StartedDt,
                    IsFilterAssessment = !assessment.EqualsIgnoreCase("short"),
                    JobCategorySafeUrl = userSession.FilteredAssessmentState?.JobFamilyNameUrlSafe
                };

                return httpResponseMessageHelper.Ok(JsonConvert.SerializeObject(response));
            }
            catch (Exception ex)
            {
                log.LogError(ex, "Fatal exception {message}", ex.ToString());
                return new HttpResponseMessage { StatusCode = HttpStatusCode.InternalServerError };
            }
        }
    }
}

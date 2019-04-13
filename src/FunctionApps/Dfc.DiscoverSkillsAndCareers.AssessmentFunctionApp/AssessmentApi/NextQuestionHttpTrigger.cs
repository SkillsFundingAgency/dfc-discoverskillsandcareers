using Dfc.DiscoverSkillsAndCareers.Models;
using Dfc.DiscoverSkillsAndCareers.Repositories;
using Dfc.DiscoverSkillsAndCareers.AssessmentFunctionApp.Models;
using DFC.Common.Standard.Logging;
using DFC.Functions.DI.Standard.Attributes;
using DFC.HTTP.Standard;
using DFC.Swagger.Standard.Annotations;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;

namespace Dfc.DiscoverSkillsAndCareers.AssessmentFunctionApp
{
    public static class NextQuestionHttpTrigger
    {
        [FunctionName("NextQuestionHttpTrigger")]
        [ProducesResponseType(typeof(Question), (int)HttpStatusCode.OK)]
        [Response(HttpStatusCode = (int)HttpStatusCode.OK, Description = "Gets the next question for a given user session", ShowSchema = true)]
        [Response(HttpStatusCode = (int)HttpStatusCode.NotFound, Description = "No such session exists", ShowSchema = false)]
        [Response(HttpStatusCode = (int)HttpStatusCode.BadRequest, Description = "The request is malformed", ShowSchema = false)]
        [Response(HttpStatusCode = (int)HttpStatusCode.Unauthorized, Description = "API key is unknown or invalid", ShowSchema = false)]
        [Response(HttpStatusCode = (int)HttpStatusCode.Forbidden, Description = "Insufficient access", ShowSchema = false)]
        [Display(Name = "NextQuestion", Description = "")]

        public static async Task<HttpResponseMessage> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "assessment/{sessionId}/next")]HttpRequest req, 
            string sessionId,
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

            if (string.IsNullOrEmpty(sessionId))
            {
                loggerHelper.LogInformationMessage(log, correlationGuid, "Session Id not supplied");
                return httpResponseMessageHelper.BadRequest();
            }

            if (string.IsNullOrEmpty(appSettings?.Value.SessionSalt))
            {
                loggerHelper.LogInformationMessage(log, correlationGuid, "Session salt not missing from configuration");
                return httpResponseMessageHelper.BadRequest();
            }

            var userSession = await userSessionRepository.GetUserSession(sessionId);
            if (userSession == null)
            {
                loggerHelper.LogInformationMessage(log, correlationGuid, string.Format("Session Id does not exist {0}", sessionId));
                return httpResponseMessageHelper.NoContent();
            }

            var questionSetVersion = userSession.CurrentQuestionSetVersion;
            var question = await questionRepository.GetQuestion(userSession.CurrentQuestion, questionSetVersion);
            if (question == null)
            {
                loggerHelper.LogInformationMessage(log, correlationGuid, $"Question number {userSession.CurrentQuestion} could not be found on session {userSession.PrimaryKey}");
                return httpResponseMessageHelper.NoContent();
            }

            await userSessionRepository.UpdateUserSession(userSession);

            int answerCount = userSession.CurrentRecordedAnswers.Count();
            int percentComplete = Convert.ToInt32(((answerCount) / Convert.ToDecimal(userSession.MaxQuestions)) * 100);
            var nextQuestionResponse = new NextQuestionResponse()
            {
                CurrentFilterAssessmentCode = userSession.CurrentFilterAssessmentCode,
                IsComplete = userSession.IsComplete,
                NextQuestionNumber = GetNextQuestionNumber(userSession.CurrentQuestion, userSession.MaxQuestions),
                QuestionId = question.QuestionId,
                QuestionText = question.Texts.Where(x => x.LanguageCode.ToLower() == "en").FirstOrDefault()?.Text,
                TraitCode = question.TraitCode,
                QuestionNumber = question.Order,
                SessionId = userSession.PrimaryKey,
                PercentComplete = percentComplete,
                ReloadCode = userSession.UserSessionId,
                MaxQuestionsCount = userSession.MaxQuestions,
                RecordedAnswersCount = userSession.RecordedAnswers.Length,
                StartedDt = userSession.StartedDt,
                IsFilterAssessment = userSession.IsFilterAssessment,
                JobCategorySafeUrl = userSession.CurrentFilterAssessment?.JobFamilyNameUrlSafe
            };

            loggerHelper.LogMethodExit(log);

            return httpResponseMessageHelper.Ok(JsonConvert.SerializeObject(nextQuestionResponse));
        }

        public static int? GetNextQuestionNumber(int questionNumber, int maxQuestions)
        {
            if (questionNumber + 1 > maxQuestions)
            {
                return null;
            }
            return questionNumber + 1;
        }
    }
}

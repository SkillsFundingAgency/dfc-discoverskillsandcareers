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
using Newtonsoft.Json;
using System;
using System.ComponentModel.DataAnnotations;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace Dfc.DiscoverSkillsAndCareers.SessionFunctionApp
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
        [Display(Name = "Get", Description = "")]

        public static async Task<HttpResponseMessage> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "session/{sessionId}/next")]HttpRequest req, 
            string sessionId,
            ILogger log,
            [Inject]ILoggerHelper loggerHelper,
            [Inject]IHttpRequestHelper httpRequestHelper,
            [Inject]IHttpResponseMessageHelper httpResponseMessageHelper,
            [Inject]IUserSessionRepository userSessionRepository,
            [Inject]IQuestionRepository questionRepository)
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

            var userSession = await userSessionRepository.GetUserSession(sessionId);
            if (userSession == null)
            {
                loggerHelper.LogInformationMessage(log, correlationGuid, string.Format("Session Id does not exist {0}", sessionId));
                return httpResponseMessageHelper.NoContent();
            }

            userSession.CurrentQuestion = GetNextQuestionNumber(userSession.CurrentQuestion, userSession.MaxQuestions);
            var question = await questionRepository.GetQuestion(userSession.CurrentQuestion, userSession.QuestionSetVersion);
            if (question == null)
            {
                loggerHelper.LogInformationMessage(log, correlationGuid, $"Question number {userSession.CurrentQuestion} could not be found on session {userSession.PrimaryKey}");
                return httpResponseMessageHelper.NoContent();
            }

            await userSessionRepository.UpdateUserSession(userSession);

            loggerHelper.LogMethodExit(log);

            return httpResponseMessageHelper.Ok(JsonConvert.SerializeObject(question));
        }

        public static int GetNextQuestionNumber(int questionNumber, int maxQuestions)
        {
            if (questionNumber <= 0)
            {
                return 1;
            }
            else if (questionNumber > maxQuestions)
            {
                return maxQuestions;
            }
            return questionNumber;
        }
    }
}

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
using Newtonsoft.Json;
using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.IO;
using Dfc.DiscoverSkillsAndCareers.Services;

namespace Dfc.DiscoverSkillsAndCareers.AssessmentFunctionApp
{
    public static class PostAnswerHttpTrigger
    {
        [FunctionName("PostAnswerHttpTrigger")]
        [ProducesResponseType(typeof(PostAnswerResponse), (int)HttpStatusCode.OK)]
        [Response(HttpStatusCode = (int)HttpStatusCode.OK, Description = "Stores the answer for the given question against the current session", ShowSchema = true)]
        [Response(HttpStatusCode = (int)HttpStatusCode.NotFound, Description = "No such session exists", ShowSchema = false)]
        [Response(HttpStatusCode = (int)HttpStatusCode.BadRequest, Description = "The request is malformed", ShowSchema = false)]
        [Response(HttpStatusCode = (int)HttpStatusCode.Unauthorized, Description = "API key is unknown or invalid", ShowSchema = false)]
        [Response(HttpStatusCode = (int)HttpStatusCode.Forbidden, Description = "Insufficient access", ShowSchema = false)]
        [Display(Name = "Post", Description = "Stores an answer for a given question against the current session.")]

        public static async Task<HttpResponseMessage> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "session/{sessionId}")]HttpRequest req,
            string sessionId,
            ILogger log,
            [Inject]ILoggerHelper loggerHelper,
            [Inject]IHttpRequestHelper httpRequestHelper,
            [Inject]IHttpResponseMessageHelper httpResponseMessageHelper,
            [Inject]IUserSessionRepository userSessionRepository,
            [Inject]IQuestionRepository questionRepository,
            [Inject]IResultsService resultsService)
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

            PostAnswerRequest postAnswerRequest;
            using (var streamReader = new StreamReader(req.Body))
            {
                var body = streamReader.ReadToEnd();
                postAnswerRequest = JsonConvert.DeserializeObject<PostAnswerRequest>(body);
            }

            AnswerOption answerValue;
            if (Enum.TryParse(postAnswerRequest.SelectedOption, out answerValue) == false)
            {
                loggerHelper.LogInformationMessage(log, correlationGuid, $"Answer supplied is invalid {postAnswerRequest.SelectedOption}");
                return httpResponseMessageHelper.BadRequest();
            }

            var userSession = await userSessionRepository.GetUserSession(sessionId);
            if (userSession == null)
            {
                loggerHelper.LogInformationMessage(log, correlationGuid, string.Format("Session Id does not exist {0}", sessionId));
                return httpResponseMessageHelper.NoContent();
            }

            var question = await questionRepository.GetQuestion(postAnswerRequest.QuestionId);
            if (question == null)
            {
                loggerHelper.LogInformationMessage(log, correlationGuid, string.Format("Question Id does not exist {0}", postAnswerRequest.QuestionId));
                return httpResponseMessageHelper.NoContent();
            }

            var previousAnswers = userSession.RecordedAnswers.Where(x => x.QuestionId == postAnswerRequest.QuestionId).ToList();
            userSession.RecordedAnswers.RemoveAll(x => x.QuestionId == postAnswerRequest.QuestionId);
            var answer = new Answer()
            {
                AnsweredDt = DateTime.Now,
                SelectedOption = answerValue,
                QuestionId = question.QuestionId,
                QuestionNumber = question.Order.ToString(),
                QuestionText = question.Texts.FirstOrDefault(x=> x.LanguageCode.ToLower() == "en")?.Text,
                TraitCode = question.TraitCode,
                IsNegative = question.IsNegative,
            };
            userSession.RecordedAnswers.Add(answer);

            ManageIfComplete(userSession);
            if (userSession.IsComplete)
            {
                // Calculate the result
                switch (userSession.AssessmentType)
                {
                    case "short":
                        {
                            await resultsService.CalculateShortAssessment(userSession);
                            break;
                        }
                    default:
                        {
                            throw new NotImplementedException(); 
                        }
                }
            }
            else
            {
                userSession.CurrentQuestion = GetNextQuestionToAnswerNumber(userSession);
            }
            await userSessionRepository.UpdateUserSession(userSession);

            var result = new PostAnswerResponse()
            {
                IsSuccess = true,
                IsComplete = userSession.IsComplete
            };

            loggerHelper.LogMethodExit(log);

            return httpResponseMessageHelper.Ok(JsonConvert.SerializeObject(result));
        }

        public static void ManageIfComplete(UserSession userSession)
        {
            bool allQuestionsAnswered = userSession.RecordedAnswers.Count == userSession.MaxQuestions;
            if (allQuestionsAnswered)
            {
                // We have complete the session as we have all the answers
                userSession.IsComplete = true;
                userSession.CompleteDt = DateTime.Now;
            }
        }

        public static int GetNextQuestionToAnswerNumber(UserSession userSession)
        {
            for (int i = 1; i <= userSession.MaxQuestions; i++)
            {
                if (!userSession.RecordedAnswers.Any(x => x.QuestionNumber == i.ToString()))
                {
                    return i;
                }
            }
            throw new Exception("All questions answered");
        }
    }
}

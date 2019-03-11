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
using Dfc.DiscoverSkillsAndCareers.AssessmentFunctionApp.Services;
using Dfc.DiscoverSkillsAndCareers.AssessmentFunctionApp.Models;

namespace Dfc.DiscoverSkillsAndCareers.AssessmentFunctionApp
{
    public static class PostAnswerHttpTrigger
    {
        [FunctionName("PostAnswerHttpTrigger")]
        [ProducesResponseType(typeof(PostAnswerResponse), (int)HttpStatusCode.OK)]
        [Response(HttpStatusCode = (int)HttpStatusCode.OK, Description = "Stores the answer for the given question against the current session", ShowSchema = true)]
        [Response(HttpStatusCode = (int)HttpStatusCode.NotFound, Description = "No such session exists", ShowSchema = false)]
        [Response(HttpStatusCode = (int)HttpStatusCode.BadRequest, Description = "The answer value provided is not valid", ShowSchema = false)]
        [Response(HttpStatusCode = (int)HttpStatusCode.Unauthorized, Description = "API key is unknown or invalid", ShowSchema = false)]
        [Response(HttpStatusCode = (int)HttpStatusCode.Forbidden, Description = "Insufficient access", ShowSchema = false)]
        [Display(Name = "PostAnswer", Description = "Stores an answer for a given question against the current session.")]

        public static async Task<HttpResponseMessage> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "assessment/{sessionId}")]HttpRequest req,
            string sessionId,
            ILogger log,
            [Inject]ILoggerHelper loggerHelper,
            [Inject]IHttpRequestHelper httpRequestHelper,
            [Inject]IHttpResponseMessageHelper httpResponseMessageHelper,
            [Inject]IUserSessionRepository userSessionRepository,
            [Inject]IQuestionRepository questionRepository,
            [Inject]IAssessmentCalculationService resultsService,
            [Inject]IFilterAssessmentCalculationService filterAssessmentCalculationService)
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

            var answer = new Answer()
            {
                AnsweredDt = DateTime.Now,
                SelectedOption = answerValue,
                QuestionId = question.QuestionId,
                QuestionNumber = question.Order.ToString(),
                QuestionText = question.Texts.FirstOrDefault(x=> x.LanguageCode.ToLower() == "en")?.Text,
                TraitCode = question.TraitCode,
                IsNegative = question.IsNegative,
                QuestionSetVersion = userSession.CurrentQuestionSetVersion
            };
            if (userSession.IsFilterAssessment)
            {
                // Update the answer count on the filter assessment 
                userSession.CurrentFilterAssessment.RecordedAnswerCount = userSession.CurrentRecordedAnswers.Count();
            }
            // Add the answer to the session answers
            var newAnswerSet = userSession.RecordedAnswers.Where(x => x.QuestionId != postAnswerRequest.QuestionId).ToList();
            newAnswerSet.Add(answer);
            userSession.RecordedAnswers = newAnswerSet.ToArray();

            ManageIfComplete(userSession);
            if (userSession.IsComplete)
            {
                // Calculate the result
                if (!userSession.IsFilterAssessment)
                {
                    switch (userSession.AssessmentType)
                    {
                        case "short":
                            {
                                await resultsService.CalculateAssessment(userSession);
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
                    await filterAssessmentCalculationService.CalculateAssessment(userSession);
                }
            }
            else
            {
                userSession.CurrentQuestion = GetNextQuestionToAnswerNumber(userSession);
            }

            var result = new PostAnswerResponse()
            {
                IsSuccess = true,
                IsComplete = userSession.IsComplete,
                IsFilterAssessment = userSession.IsFilterAssessment,
                JobCategorySafeUrl = userSession.CurrentFilterAssessment?.JobFamilyNameUrlSafe
            };


            if (userSession.IsComplete)
            {
                // If we are complete ensure we are no longer in a filtered assessment
                //userSession.CurrentFilterAssessmentCode = null;
            }
            // Update the session
            await userSessionRepository.UpdateUserSession(userSession);

            loggerHelper.LogMethodExit(log);

            return httpResponseMessageHelper.Ok(JsonConvert.SerializeObject(result));
        }

        /// <summary>
        /// Updates the IsComplete property on the UserSession based off the current answers and max questions.
        /// </summary>
        public static void ManageIfComplete(UserSession userSession)
        {
            bool allQuestionsAnswered = userSession.CurrentRecordedAnswers.Count() == userSession.CurrentMaxQuestions;
            if (allQuestionsAnswered)
            {
                userSession.IsComplete = true;
                if (!userSession.IsFilterAssessment)
                {
                    userSession.CompleteDt = DateTime.Now;
                }
            }
            else
            {
                userSession.IsComplete = false;
            }
        }

        /// <summary>
        /// Gets the next question number that should be answered.
        /// </summary>
        public static int GetNextQuestionToAnswerNumber(UserSession userSession)
        {
            for (int i = 1; i <= userSession.CurrentMaxQuestions; i++)
            {
                if (!userSession.CurrentRecordedAnswers.Any(x => x.QuestionNumber == i.ToString()))
                {
                    return i;
                }
            }
            throw new Exception("All questions answered");
        }
    }
}

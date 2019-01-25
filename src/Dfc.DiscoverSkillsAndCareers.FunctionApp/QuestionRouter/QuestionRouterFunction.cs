using System;
using System.Net.Http;
using System.Threading.Tasks;
using Dfc.DiscoverSkillsAndCareers.FunctionApp.Helpers;
using Dfc.DiscoverSkillsAndCareers.Models;
using Dfc.DiscoverSkillsAndCareers.Repositories;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using static Dfc.DiscoverSkillsAndCareers.FunctionApp.Helpers.HttpResponseHelpers;

namespace Dfc.DiscoverSkillsAndCareers.FunctionApp.QuestionRouter
{
    public static class QuestionRouterFunction
    {
        [FunctionName("QuestionRouterFunction")]
        public static async Task<HttpResponseMessage> Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = "q/{questionInput?}")]HttpRequestMessage req, string questionInput, ILogger log, ExecutionContext context)
        {
            try
            {
                var appSettings = ConfigurationHelper.ReadConfiguration(context);
                log.LogInformation($"QuestionRouterFunction questionInput={questionInput} appSettings={Newtonsoft.Json.JsonConvert.SerializeObject(appSettings)}");

                var questionRepository = new QuestionRepository(appSettings.CosmosSettings);

                var sessionHelper = await SessionHelper.CreateWithInit(req, appSettings);
                int questionNumber = 0;
                if (sessionHelper.HasSession)
                {
                    // Set the current question based on the request
                    int.TryParse(questionInput, out questionNumber);
                    if (sessionHelper.HasInputError == false && questionNumber > 0)
                    {
                        int shouldDisplayQuestion = GetCurrentQuestionNumber(questionNumber, sessionHelper.Session.MaxQuestions);
                        if (shouldDisplayQuestion != questionNumber)
                        {
                            return RedirectToQuestionNumber(req, shouldDisplayQuestion, sessionHelper.Session.PrimaryKey);
                        }
                        sessionHelper.Session.CurrentQuestion = shouldDisplayQuestion;
                    }
                }
                var assessmentType = sessionHelper?.QueryDictionary.Get("assessmentType");
                if ((questionNumber == 0 || questionInput == "1") && assessmentType == "short")
                {
                    // Setup a new session with the current question set version
                    var currentQuestionSetInfo = await questionRepository.GetCurrentQuestionSetVersion();
                    await SetupNewSession(sessionHelper, currentQuestionSetInfo);
                    return RedirectToQuestionNumber(req, sessionHelper);
                }

                if (!sessionHelper.HasSession)
                {
                    // Session id is missing, redirect to question 1
                    return RedirectToNewSession(req);
                }

                // Determine if we are complete and update the session
                ManageIfComplete(sessionHelper.Session);
                await sessionHelper.UpdateSession();

                // Load the question to display
                string questionId = $"{sessionHelper.Session.QuestionSetVersion}-{sessionHelper.Session.CurrentQuestion}";
                var question = await questionRepository.GetQuestion(questionId);
                if (question == null)
                {
                    throw new Exception($"Question {questionId} could not be found");
                }

                // Return the question page
                return CreateQuestionPage(req, sessionHelper, question);
            }

            catch (Exception ex)
            {
                log.LogError(ex, "QuestionRouterFunction run");
                return InternalServerError(req, context);
            }
        }

        public static int GetCurrentQuestionNumber(int questionNumber, int maxQuestions)
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

        private static async Task SetupNewSession(SessionHelper sessionHelper, QuestionSetInfo questionSetInfo)
        {
            // Create a new session
            await sessionHelper.CreateSession(questionSetInfo.QuestionSetVersion, questionSetInfo.MaxQuestions);
        }

        public static void ManageIfComplete(UserSession userSession)
        {
            bool allQuestionsAnswered = userSession.RecordedAnswers.Count == userSession.MaxQuestions;
            if (allQuestionsAnswered && userSession.CurrentQuestion >= userSession.MaxQuestions)
            {
                userSession.IsComplete = true;
                userSession.CompleteDt = DateTime.Now;
            }
        }

        public static HttpResponseMessage CreateQuestionPage(HttpRequestMessage req, SessionHelper sessionHelper, Question question)
        {
            // Build page html from the template blob
            string blobName = "questions.html";
            var templateHtml = BlobStorageHelper.GetBlob(sessionHelper.Config.BlobStorage, blobName).Result;
            if (templateHtml == null)
            {
                throw new Exception($"Blob {blobName} could not be found in {sessionHelper.Config.BlobStorage.ContainerName}");
            }
            var html = new BuildPageHtml(templateHtml, sessionHelper, question).Html;

            // Ok html response
            return OKHtmlWithCookie(req, html, sessionHelper.Session.PrimaryKey);
        }
    }
}

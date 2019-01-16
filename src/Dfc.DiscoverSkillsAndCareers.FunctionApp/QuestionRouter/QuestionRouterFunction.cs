using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Dfc.DiscoverSkillsAndCareers.Models;
using Dfc.DiscoverSkillsAndCareers.Repositories;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;

namespace Dfc.DiscoverSkillsAndCareers.FunctionApp.QuestionRouter
{
    public static class QuestionRouterFunction
    {
        [FunctionName("QuestionRouterFunction")]
        public static async Task<HttpResponseMessage> Run([HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = "q/{questionInput?}")]HttpRequestMessage req, string questionInput, ILogger log, ExecutionContext context)
        {
            var appSettings = ConfigurationHelper.ReadConfiguration(context);
            log.LogDebug($"QuestionRouterFunction questionInput={questionInput} appSettings={Newtonsoft.Json.JsonConvert.SerializeObject(appSettings)}");

            var questionRepository = new QuestionRepository(appSettings.CosmosSettings);

            var sessionHelper = await SessionHelper.CreateWithInit(req, appSettings);
            if (sessionHelper.HasSession)
            {
                int questionNumber;
                int.TryParse(questionInput, out questionNumber);
                if (sessionHelper.HasInputError == false)
                {
                    sessionHelper.Session.CurrentQuestion = questionNumber;
                }
            }
            else if (questionInput == "1")
            {
                // Setup a new session with the current question set version
                var currentQuestionSetInfo = await questionRepository.GetCurrentQuestionSetVersion();
                await SetupNewSession(sessionHelper, currentQuestionSetInfo);
            }

            if (!sessionHelper.HasSession)
            {
                // Session id is missing, redirect to question 1
                var redirectResponse = req.CreateResponse(HttpStatusCode.Redirect);
                var uri = req.RequestUri;
                var host = uri.AbsoluteUri.Replace(uri.AbsolutePath, "");
                redirectResponse.Headers.Location = new System.Uri($"{host}/q/1");
                return redirectResponse;
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

            string errorMessage = sessionHelper.HasInputError ? "You must select an answer" : string.Empty;
            int percentComplete = Convert.ToInt32(((sessionHelper.Session.CurrentQuestion - 1) / Convert.ToDecimal(sessionHelper.Session.MaxQuestions)) * 100);
            // Build page html
            var html = new BuildPageHtml(sessionHelper, question, errorMessage, percentComplete.ToString()).Html;

            // Ok html response
            var response = req.CreateResponse(HttpStatusCode.OK);
            response.Content = new StringContent(html);
            response.Content.Headers.ContentType = new MediaTypeHeaderValue("text/html");
            return response;
        }

        private static async Task SetupNewSession(SessionHelper sessionHelper, QuestionSetInfo questionSetInfo)
        {
            // Create a new session
            await sessionHelper.CreateSession();
            // Assign the question set details
            sessionHelper.Session.QuestionSetVersion = questionSetInfo.QuestionSetVersion;
            sessionHelper.Session.MaxQuestions = questionSetInfo.MaxQuestions;
            sessionHelper.Session.CurrentQuestion = 1;
        }

        public static void ManageIfComplete(UserSession userSession)
        {
            if (userSession.CurrentQuestion >= userSession.MaxQuestions)
            {
                userSession.IsComplete = true;
                userSession.CompleteDt = DateTime.Now;
            }
        }
    }
}

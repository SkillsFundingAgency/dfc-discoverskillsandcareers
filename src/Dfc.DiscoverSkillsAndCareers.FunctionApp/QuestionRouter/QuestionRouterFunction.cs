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
        public static async Task<HttpResponseMessage> Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = "q/{questionInput?}")]HttpRequestMessage req, string questionInput, ILogger log, ExecutionContext context)
        {
            try
            {
                var appSettings = ConfigurationHelper.ReadConfiguration(context);
                log.LogInformation($"QuestionRouterFunction questionInput={questionInput} appSettings={Newtonsoft.Json.JsonConvert.SerializeObject(appSettings)}");

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

                // Build page html
                var templateHtml = BlobStorageHelper.GetBlob(sessionHelper.Config.BlobStorage, "questions.html").Result;
                if (templateHtml == null)
                {
                    throw new Exception($"Blob could not be found");
                }
                var html = new BuildPageHtml(templateHtml, sessionHelper, question).Html;

                // Ok html response
                var response = req.CreateResponse(HttpStatusCode.OK);
                response.Content = new StringContent(html);
                response.Content.Headers.ContentType = new MediaTypeHeaderValue("text/html");
                return response;
            }

            catch (Exception ex)
            {
                log.LogError(ex, "QuestionRouterFunction run");
                var response = req.CreateResponse(HttpStatusCode.InternalServerError);
                response.Content = new StringContent("{ \"message\": \"" + ex.Message + "\" }");
                response.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                return response;
            }
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

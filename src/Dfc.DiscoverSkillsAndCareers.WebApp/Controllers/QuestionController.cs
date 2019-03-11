using Dfc.DiscoverSkillsAndCareers.WebApp.Config;
using Dfc.DiscoverSkillsAndCareers.WebApp.Models;
using Dfc.DiscoverSkillsAndCareers.WebApp.Services;
using DFC.Common.Standard.Logging;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Threading.Tasks;

namespace Dfc.DiscoverSkillsAndCareers.WebApp.Controllers
{
    public class QuestionController : BaseController
    {
        readonly ILogger<QuestionController> Log;
        readonly ILoggerHelper LoggerHelper;
        readonly IApiServices ApiServices;
        readonly AppSettings AppSettings;

        public QuestionController(
            ILogger<QuestionController> log,
            ILoggerHelper loggerHelper,
            IApiServices apiServices,
            IOptions<AppSettings> appSettings)
        {
            Log = log;
            LoggerHelper = loggerHelper;
            ApiServices = apiServices;
            AppSettings = appSettings.Value;
        }

        [HttpPost]
        [Route("qf/{questionNumber:int}")]
        public async Task<IActionResult> AnswerFilteringQuestion(int questionNumber) => await AnswerQuestion(questionNumber);

        [HttpPost]
        [Route("q/{questionNumber:int}")]
        public async Task<IActionResult> AnswerQuestion(int questionNumber)
        {
            var correlationId = Guid.NewGuid();
            string sessionId = null;
            try
            {
                LoggerHelper.LogMethodEnter(Log);

                sessionId = await TryGetSessionId(Request);

                PostAnswerRequest postAnswerRequest = new PostAnswerRequest()
                {
                    QuestionId = GetFormValue("questionId"),
                    SelectedOption = GetFormValue("selected_answer")
                };
                PostAnswerResponse postAnswerResponse = await ApiServices.PostAnswer(sessionId, postAnswerRequest, correlationId);
                return await NextQuestion(sessionId, postAnswerResponse == null);
            }
            catch (System.Net.Http.HttpRequestException)
            {
                return await NextQuestion(sessionId, true);
            }
            catch (Exception ex)
            {
                LoggerHelper.LogException(Log, correlationId, ex);
                return StatusCode(500);
            }
            finally
            {
                LoggerHelper.LogMethodExit(Log);
            }
        }

        [HttpGet]
        [Route("qf/{questionNumber:int}")]
        public async Task<IActionResult> AtQuestionNumber(int questionNumber) => await AtQuestionNumber(questionNumber, null);

        [HttpGet("assessment/{assessmentType}")]
        public async Task<IActionResult> NewAssessment(string assessmentType)
        {
              var correlationId = Guid.NewGuid();
            try
            {
                LoggerHelper.LogMethodEnter(Log);
                
                var queryDictionary = System.Web.HttpUtility.ParseQueryString(AppSettings.AssessmentQuestionSetNames);
                var title = queryDictionary.Get(assessmentType);
                var newSessionResponse = await ApiServices.NewSession(correlationId, assessmentType, title);
                if (newSessionResponse == null)
                {
                    throw new Exception($"Failed to create session for assessment type {assessmentType} using question set {title}");
                }
                var sessionId = newSessionResponse.SessionId;
                Response.Cookies.Append("ncs-session-id", sessionId, new Microsoft.AspNetCore.Http.CookieOptions() { Secure = true });
                var redirectResponse = new RedirectResult($"/q/1");
                return redirectResponse;

            }
            catch (Exception ex)
            {
                LoggerHelper.LogException(Log, correlationId, ex);
                return StatusCode(500);
            }
            finally
            {
                LoggerHelper.LogMethodExit(Log);
            }
        }

        [HttpGet]
        [Route("q/{questionNumber:int}")]
        public async Task<IActionResult> AtQuestionNumber(int questionNumber, string assessmentType)
        {
            var correlationId = Guid.NewGuid();
            try
            {
                LoggerHelper.LogMethodEnter(Log);
                
                var sessionId = await TryGetSessionId(Request);
                if (string.IsNullOrEmpty(sessionId))
                {
                    return Redirect("/");
                }
                return await NextQuestion(sessionId, false);
            }
            catch (Exception ex)
            {
                LoggerHelper.LogException(Log, correlationId, ex);
                return StatusCode(500);
            }
            finally
            {
                LoggerHelper.LogMethodExit(Log);
            }
        }

        [NonAction]
        public async Task<IActionResult> NextQuestion(string sessionId, bool invalidAnswer)
        {
            var correlationId = Guid.NewGuid();
            try
            {
                LoggerHelper.LogMethodEnter(Log);

                var nextQuestionResponse = await ApiServices.NextQuestion(sessionId, correlationId);

                if (nextQuestionResponse == null)
                {
                    throw new Exception($"Question not found for session {sessionId}");
                }
                var model = await ApiServices.GetContentModel<QuestionViewModel>("questionpage", correlationId);
                var formRoute = GetAnswerFormPostRoute(nextQuestionResponse, invalidAnswer);
                int displayPercentComplete = nextQuestionResponse.PercentComplete - (nextQuestionResponse.PercentComplete % 10);

                model.Code = sessionId;
                model.ErrorMessage = !invalidAnswer ? string.Empty : model.NoAnswerErrorMessage;
                model.FormRoute = formRoute;
                model.Percentage = displayPercentComplete.ToString();
                model.PercentrageLeft = nextQuestionResponse.PercentComplete == 0 ? "" : nextQuestionResponse.PercentComplete.ToString();
                model.QuestionId = nextQuestionResponse.QuestionId;
                model.QuestionNumber = nextQuestionResponse.QuestionNumber;
                model.SessionId = sessionId;
                model.TraitCode = nextQuestionResponse.TraitCode;
                model.QuestionText = nextQuestionResponse.QuestionText;
                model.IsFilterAssessment = nextQuestionResponse.IsFilterAssessment;

                Response.Cookies.Append("ncs-session-id", sessionId, new Microsoft.AspNetCore.Http.CookieOptions() { Secure = true });
                var viewName = model.IsFilterAssessment ? "FilteringQuestion" : "Question";
                return View(viewName, model);
            }
            catch (System.Net.Http.HttpRequestException)
            {
                return Redirect("/");
            }
            catch (Exception ex)
            {
                LoggerHelper.LogException(Log, correlationId, ex);
                return StatusCode(500);
            }
            finally
            {
                LoggerHelper.LogMethodExit(Log);
            }
        }

        private string GetAnswerFormPostRoute(NextQuestionResponse nextQuestionResponse, bool invalidAnswer)
        {
            var questionRoute = nextQuestionResponse.IsFilterAssessment ? "qf" : "q";
            var finishRoute = nextQuestionResponse.IsFilterAssessment ? $"finish/{nextQuestionResponse.JobCategorySafeUrl}" : "finish";
            var nextRoute = nextQuestionResponse.MaxQuestionsCount == nextQuestionResponse.QuestionNumber ? $"/{finishRoute}"
                : $"/{questionRoute}/{nextQuestionResponse.NextQuestionNumber.ToString()}";
            return nextRoute;
        }
    }
}
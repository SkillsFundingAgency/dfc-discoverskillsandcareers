using Dfc.DiscoverSkillsAndCareers.WebApp.Models;
using Dfc.DiscoverSkillsAndCareers.WebApp.Services;
using DFC.Common.Standard.Logging;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace Dfc.DiscoverSkillsAndCareers.WebApp.Controllers
{
    public class QuestionController : BaseController
    {
        readonly ILogger<QuestionController> Log;
        readonly ILoggerHelper LoggerHelper;
        readonly IApiServices ApiServices;

        public QuestionController(
            ILogger<QuestionController> log,
            ILoggerHelper loggerHelper,
            IApiServices apiServices)
        {
            Log = log;
            LoggerHelper = loggerHelper;
            ApiServices = apiServices;
        }

        [HttpPost]
        [Route("q/{questionNumber:int}")]
        public async Task<IActionResult> AnswerQuestion(int questionNumber)
        {
            var correlationId = Guid.NewGuid();
            try
            {
                LoggerHelper.LogMethodEnter(Log);

                var sessionId = await TryGetSessionId(Request);

                PostAnswerRequest postAnswerRequest = new PostAnswerRequest()
                {
                    QuestionId = GetFormValue("questionId"),
                    SelectedOption = GetFormValue("selected_answer")
                };
                PostAnswerResponse postAnswerResponse = await ApiServices.PostAnswer(sessionId, postAnswerRequest, correlationId);
                return await NextQuestion(sessionId);
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

                if (questionNumber == 1 && string.IsNullOrEmpty(assessmentType) == false)
                {
                    var newSessionResponse = await ApiServices.NewSession(correlationId);
                    sessionId = newSessionResponse.SessionId;
                }
                return await NextQuestion(sessionId);
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
        public async Task<IActionResult> NextQuestion(string sessionId)
        {
            var correlationId = Guid.NewGuid();
            try
            {
                LoggerHelper.LogMethodEnter(Log);

                var nextQuestionResponse = await ApiServices.NextQuestion(sessionId, correlationId);

                var model = await ApiServices.GetContentModel<QuestionViewModel>("questionpage", correlationId);
                var nextRoute = nextQuestionResponse.MaxQuestionsCount == nextQuestionResponse.QuestionNumber ? "/finish" : $"/q/{nextQuestionResponse.NextQuestionNumber.ToString()}";

                model.Code = sessionId;
                model.ErrorMessage = string.Empty;
                model.FormRoute = nextRoute;
                model.Percentage = nextQuestionResponse.PercentComplete.ToString();
                model.PercentrageLeft = nextQuestionResponse.PercentComplete == 0 ? "" : nextQuestionResponse.PercentComplete.ToString();
                model.QuestionId = nextQuestionResponse.QuestionId;
                model.QuestionNumber = nextQuestionResponse.QuestionNumber;
                model.SessionId = sessionId;
                model.TraitCode = nextQuestionResponse.TraitCode;
                model.QuestionText = nextQuestionResponse.QuestionText;

                Response.Cookies.Append("ncs-session-id", sessionId);
                return View("Question", model);
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
    }
}
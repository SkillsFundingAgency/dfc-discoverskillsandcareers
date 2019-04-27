﻿using Dfc.DiscoverSkillsAndCareers.WebApp.Config;
using Dfc.DiscoverSkillsAndCareers.WebApp.Models;
using Dfc.DiscoverSkillsAndCareers.WebApp.Services;
using DFC.Common.Standard.Logging;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Threading.Tasks;
using System.Web;

namespace Dfc.DiscoverSkillsAndCareers.WebApp.Controllers
{
    public class QuestionController : BaseController
    {
        readonly ILogger<QuestionController> _log;
        readonly IApiServices _apiServices;
        readonly AppSettings _appSettings;

        public QuestionController(
            ILogger<QuestionController> log,
            IApiServices apiServices,
            IOptions<AppSettings> appSettings)
        {
            _log = log;
            _apiServices = apiServices;
            _appSettings = appSettings.Value;
        }

        [HttpPost]
        [Route("q/{assessment}/{questionNumber:int}")]
        public async Task<IActionResult> AnswerQuestion(string assessment, int questionNumber)
        {
            var correlationId = Guid.NewGuid();
            string sessionId = null;
            try
            {
                sessionId = await TryGetSessionId(Request);

                if (sessionId == null || sessionId != HttpUtility.UrlEncode(sessionId))
                {
                    return BadRequest();
                }

                var postAnswerRequest = new PostAnswerRequest()
                {
                    QuestionId = GetFormValue("questionId"),
                    SelectedOption = GetFormValue("selected_answer")
                };
                
                var postAnswerResponse = await _apiServices.PostAnswer(sessionId, postAnswerRequest, correlationId);
                
                if (postAnswerRequest.SelectedOption == null || postAnswerResponse == null)
                {
                    return await NextQuestion(sessionId, assessment, questionNumber, true);
                }
                if (postAnswerResponse.IsComplete)
                {
                    var finishEndpoint = postAnswerResponse.IsFilterAssessment ? $"/finish/{postAnswerResponse.JobCategorySafeUrl}" : "/finish";
                    AppendCookie(sessionId);
                    return Redirect(finishEndpoint);
                }
                var url = $"/q/{assessment}/{postAnswerResponse.NextQuestionNumber}";
                return Redirect(url);
            }
            catch (System.Net.Http.HttpRequestException)
            {
                return await NextQuestion(sessionId, assessment, questionNumber, true);
            }
            catch (Exception ex)
            {
                _log.LogError(ex,$"Correlation Id: {correlationId} - An error occurred in session {sessionId} answering question: {questionNumber} in assessment {assessment}.");
                return StatusCode(500);
            }
        }

        [HttpGet("assessment/{assessment}")]
        public async Task<IActionResult> NewAssessment(string assessment)
        {
            var correlationId = Guid.NewGuid();
            try
            {                
                var queryDictionary = HttpUtility.ParseQueryString(_appSettings.AssessmentQuestionSetNames);
                var title = queryDictionary.Get(assessment);

                if(assessment != HttpUtility.UrlEncode(assessment))
                {
                    return BadRequest();
                }
                
                if (title != HttpUtility.UrlEncode(title))
                {
                    return BadRequest();
                }

                var newSessionResponse = await _apiServices.NewSession(correlationId, assessment, title);
                if (newSessionResponse == null)
                {
                    throw new Exception($"Failed to create session for assessment type {assessment} using question set {title}");
                }
                var sessionId = newSessionResponse.SessionId;
                AppendCookie(sessionId);
                
                var redirectResponse = new RedirectResult($"/q/{assessment}/1");
                
                return redirectResponse;

            }
            catch (Exception ex)
            {
                _log.LogError(ex, $"Correlation Id: {correlationId} - An error occured creating a new assessment of type {assessment}");
                return StatusCode(500);
            }
        }

        [HttpGet]
        [Route("q/{assessment}/{questionNumber:int}")]
        public async Task<IActionResult> AtQuestionNumber(string assessment, int questionNumber)
        {
            var correlationId = Guid.NewGuid();
            try
            {
                var sessionId = await TryGetSessionId(Request);
                if (string.IsNullOrEmpty(sessionId))
                {
                    return Redirect("/");
                }
                return await NextQuestion(sessionId, assessment, questionNumber, false);
            }
            catch (Exception ex)
            {
                _log.LogError(ex, $"Correlation Id: {correlationId} - An error occured getting question {questionNumber} for assessment type {assessment}");
                return StatusCode(500);
            }
        }

        [NonAction]
        public async Task<IActionResult> NextQuestion(string sessionId, string assessment, int questionNumber, bool invalidAnswer)
        {
            var correlationId = Guid.NewGuid();
            try
            {
                var nextQuestionResponse = await _apiServices.Question(sessionId, assessment, questionNumber, correlationId);

                if (nextQuestionResponse == null)
                {
                    throw new Exception($"Question not found for session {sessionId}");
                }
                var model = await _apiServices.GetContentModel<QuestionViewModel>("questionpage", correlationId);
                var formRoute = GetAnswerFormPostRoute(nextQuestionResponse, assessment);
                int displayPercentComplete = nextQuestionResponse.PercentComplete;

                model.Code = sessionId;
                model.ErrorMessage = !invalidAnswer ? string.Empty : model.NoAnswerErrorMessage;
                model.FormRoute = formRoute;
                model.Percentage = displayPercentComplete.ToString();
                model.PercentrageLeft = nextQuestionResponse.PercentComplete == 0 ? "" : nextQuestionResponse.PercentComplete.ToString();
                model.QuestionId = nextQuestionResponse.QuestionId;
                model.QuestionNumber = nextQuestionResponse.QuestionNumber;
                model.RecordedAnswer = nextQuestionResponse.RecordedAnswer;
                model.SessionId = sessionId;
                model.TraitCode = nextQuestionResponse.TraitCode;
                model.QuestionText = nextQuestionResponse.QuestionText;
                model.IsFilterAssessment = nextQuestionResponse.IsFilterAssessment;

                AppendCookie(sessionId);
                var viewName = model.IsFilterAssessment ? "FilteringQuestion" : "Question";
                return View(viewName, model);
            }
            catch (System.Net.Http.HttpRequestException)
            {
                return Redirect("/");
            }
            catch (Exception ex)
            {
                _log.LogError(ex, $"Correlation Id: {correlationId} - An error occured while getting \"next\" question {questionNumber} for assessment type {assessment}");
                return StatusCode(500);
            }
        }

        private string GetAnswerFormPostRoute(AssessmentQuestionResponse assessmentQuestionResponse, string assessment)
        {
            var nextRoute = $"/q/{assessment}/{(assessmentQuestionResponse.NextQuestionNumber ?? assessmentQuestionResponse.MaxQuestionsCount).ToString()}";
            return nextRoute;
        }
    }
}
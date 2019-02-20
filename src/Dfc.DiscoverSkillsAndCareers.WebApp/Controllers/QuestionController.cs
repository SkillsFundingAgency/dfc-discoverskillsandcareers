using Dfc.DiscoverSkillsAndCareers.WebApp.Models;
using Dfc.DiscoverSkillsAndCareers.WebApp.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace Dfc.DiscoverSkillsAndCareers.WebApp.Controllers
{
    public class QuestionController : BaseController
    {
        readonly ILogger<QuestionController> Logger;
        readonly IApiServices ApiServices;

        public QuestionController(ILogger<QuestionController> logger,
            IApiServices apiServices)
        {
            Logger = logger;
            ApiServices = apiServices;
        }

        [HttpPost]
        [Route("q/{questionNumber:int}")]
        public async Task<IActionResult> AnswerQuestion(int questionNumber)
        {
            var sessionId = await TryGetSessionId(Request);

            PostAnswerRequest postAnswerRequest = new PostAnswerRequest()
            {
                QuestionId = GetFormValue("questionId"),
                SelectedOption = GetFormValue("selected_answer")
            };
            PostAnswerResponse postAnswerResponse = await ApiServices.PostAnswer(sessionId, postAnswerRequest);
            return await NextQuestion(sessionId);
        }

        [HttpGet]
        [Route("q/{questionNumber:int}")]
        public async Task<IActionResult> AtQuestionNumber(int questionNumber, string assessmentType)
        {
            var sessionId = await TryGetSessionId(Request);

            if (questionNumber == 1 && string.IsNullOrEmpty(assessmentType) == false)
            {
                var newSessionResponse = await ApiServices.NewSession();
                sessionId = newSessionResponse.SessionId;
            }
            return await NextQuestion(sessionId);
        }

        [NonAction]
        public async Task<IActionResult> NextQuestion(string sessionId)
        { 
            var nextQuestionResponse = await ApiServices.NextQuestion(sessionId);

            var model = await ApiServices.GetContentModel<QuestionViewModel>("questionpage");
            var nextRoute = nextQuestionResponse.IsComplete ? "/finish" : $"/q/{nextQuestionResponse.NextQuestionNumber.ToString()}";

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
    }
}
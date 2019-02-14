using Dfc.DiscoverSkillsAndCareers.Models;
using Dfc.DiscoverSkillsAndCareers.Repositories;
using Dfc.DiscoverSkillsAndCareers.Services;
using Dfc.DiscoverSkillsAndCareers.WebApp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Dfc.DiscoverSkillsAndCareers.WebApp.Controllers
{
    public class QuestionController : Controller
    {
        readonly ILogger<QuestionController> Logger;
        readonly IUserSessionService UserSessionService;
        readonly IQuestionRepository QuestionRepository;

        public QuestionController(ILogger<QuestionController> logger,
            IUserSessionService userSessionService,
            IQuestionRepository questionRepository)
        {
            Logger = logger;
            UserSessionService = userSessionService;
            QuestionRepository = questionRepository;
        }

        [Route("q")]
        public IActionResult Index()
        {
            return View("Question");
        }

        [HttpPost]
        [Route("q/{questionNumber:int}")]
        public async Task<IActionResult> AnswerQuestion(int questionNumber)
        {
            var nextQuestion = Request.Path.Value.Split("/").LastOrDefault();
            int.TryParse(nextQuestion, out questionNumber);
            return await QuestionRouter(questionNumber, null);
        }

        [HttpGet]
        [Route("q/{questionNumber:int}")]
        public async Task<IActionResult> AtQuestionNumber(int questionNumber, string assessmentType)
        {
            return await QuestionRouter(questionNumber, assessmentType);
        }

        [NonAction]
        public async Task<IActionResult> QuestionRouter(int questionNumber, string assessmentType)
        { 
            await UserSessionService.Init(Request);

            if (UserSessionService.HasSession)
            {
                //int? nextQuestion = UserSessionService.GetFormNumberValue("nextQuestionNumber");
                //questionNumber = nextQuestion ?? questionNumber;
                if (UserSessionService.HasInputError == false && questionNumber > 0)
                {
                    int shouldDisplayQuestion = GetCurrentQuestionNumber(questionNumber, UserSessionService.Session.MaxQuestions);
                    if (shouldDisplayQuestion != questionNumber)
                    {
                        return RedirectToQuestionNumber(shouldDisplayQuestion, UserSessionService.Session.PrimaryKey);
                    }
                    UserSessionService.Session.CurrentQuestion = shouldDisplayQuestion;
                }
            }

            if ((questionNumber == 0 || questionNumber == 1) && assessmentType == "short")
            {
                // Setup a new session with the current question set version
                var currentQuestionSetInfo = await QuestionRepository.GetCurrentQuestionSetVersion();
                await SetupNewSession(currentQuestionSetInfo);
                return RedirectToQuestionNumber(UserSessionService.Session);
            }

            if (!UserSessionService.HasSession)
            {
                // Session id is missing, redirect to question 1
                return RedirectToNewSession();
            }

            // Determine if we are complete and update the session
            ManageIfComplete(UserSessionService.Session);
            await UserSessionService.UpdateSession();

            // Load the question to display
            string questionId = $"{UserSessionService.Session.QuestionSetVersion}-{UserSessionService.Session.CurrentQuestion}";
            var question = await QuestionRepository.GetQuestion(questionId);
            if (question == null)
            {
                throw new Exception($"Question {questionId} could not be found");
            }

            string errorMessage = UserSessionService.HasInputError ? "Please select an option above or this does not apply to continue" : string.Empty;
            int percentComplete = Convert.ToInt32(((UserSessionService.Session.RecordedAnswers.Count) / Convert.ToDecimal(UserSessionService.Session.MaxQuestions)) * 100);
            int displayPercentComplete = percentComplete - (percentComplete % 10);
            var nextRoute = GetNextRoute(UserSessionService.Session);
            var buttonText = "Next";

            var model = new QuestionViewModel()
            {
                ButtonText = buttonText,
                Code = UserSessionService.Session.UserSessionId,
                ErrorMessage = errorMessage,
                FormRoute = nextRoute,
                Percentage = displayPercentComplete.ToString(),
                PercentrageLeft = displayPercentComplete == 0 ? "" : displayPercentComplete.ToString(),
                QuestionId = question.QuestionId.ToString(),
                QuestionNumber = question.Order,
                SessionId = UserSessionService.Session.PrimaryKey,
                TraitCode = question.TraitCode,
                QuestionText = question.Texts.Where(x => x.LanguageCode.ToLower() == "en".ToLower()).FirstOrDefault()?.Text
            };
            Response.Cookies.Append("ncs-session-id", UserSessionService.Session.PrimaryKey);
            return View("Question", model);
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

        public static string GetNextRoute(UserSession userSession)
        {
            if (userSession.IsComplete || userSession.RecordedAnswers.Count + 1 >= userSession.MaxQuestions)
            {
                return "/finish";
            }
            else if (userSession.CurrentQuestion + 1 <= userSession.MaxQuestions)
            {
                return $"/q/{userSession.CurrentQuestion + 1}";
            }
            else
            {
                // Goto last unaswered question
                int questionNumber = 1;
                for (int i = 1; i < userSession.MaxQuestions; i++)
                {
                    if (userSession.RecordedAnswers.Any(x => x.QuestionNumber == i.ToString()) == false)
                    {
                        questionNumber = i;
                        break;
                    }
                }
                return $"/q/{questionNumber}";
            }
        }

        private async Task SetupNewSession(QuestionSetInfo questionSetInfo)
        {
            // Create a new session
            await UserSessionService.CreateSession(questionSetInfo.QuestionSetVersion, questionSetInfo.MaxQuestions, questionSetInfo.AssessmentType);
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

        public static IActionResult RedirectToNewSession()
        {
            return new RedirectResult($"/q/1?assessmentType=short");
        }

        public IActionResult RedirectToQuestionNumber(int questionNumber, string sessionId)
        {
            var redirectResponse = new RedirectResult($"/q/{questionNumber}");
            Response.Cookies.Append("ncs-session-id", sessionId);
            return redirectResponse;
        }

        public IActionResult RedirectToQuestionNumber(UserSession session) => RedirectToQuestionNumber(session.CurrentQuestion, session.PrimaryKey);
    }
}
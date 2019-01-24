using System;
using System.Threading.Tasks;
using System.Collections.Specialized;
using System.Net.Http;
using System.Linq;
using Dfc.DiscoverSkillsAndCareers.Models;
using Dfc.DiscoverSkillsAndCareers.Repositories;

namespace Dfc.DiscoverSkillsAndCareers.FunctionApp.Helpers
{
    public class SessionHelper
    {
        private UserSessionRepository userSessionRepository;

        public static async Task<SessionHelper> CreateWithInit(HttpRequestMessage request, AppSettings appSettings)
        {
            
            var sessionHelper = new SessionHelper();
            await sessionHelper.Init(request, appSettings);
            return sessionHelper;
        }

        public async Task Init(HttpRequestMessage request, AppSettings appSettings)
        {
            this.Config = appSettings;
            userSessionRepository = new UserSessionRepository(appSettings.CosmosSettings);
            UserSession userSession = null;

            if (request.Content.IsFormData())
            {
                FormData = await request.Content.ReadAsFormDataAsync();
            }

            string sessionId = TryGetSessionId(request);
            if (string.IsNullOrEmpty(sessionId) == false)
            {
                userSession = await userSessionRepository.GetUserSession(sessionId);
            }
            Session = userSession;

            if (HasSession && FormData != null)
            {
                await CheckForAnswer();
            }
        }

        public string TryGetSessionId(HttpRequestMessage request)
        {
            string sessionId = string.Empty;
            var cookieSessionId = request.Headers.GetCookies("ncs-session-id").FirstOrDefault()?.Cookies.Where(x => x.Name == "ncs-session-id").FirstOrDefault()?.Value;
            sessionId = cookieSessionId;

            var queryDictionary = System.Web.HttpUtility.ParseQueryString(request.RequestUri.Query);
            var code = queryDictionary.Get("sessionId");
            if (string.IsNullOrEmpty(code) == false)
            {
                sessionId = code;
            }

            if (request.Content.IsFormData())
            {
                try
                {
                    var formSessionId = FormData.GetValues("sessionId").FirstOrDefault();
                    if (string.IsNullOrEmpty(formSessionId) == false)
                    {
                        sessionId = formSessionId;
                    }
                }
                catch { };
            }
            return sessionId;
        }

        public NameValueCollection FormData { get; private set; }
        public UserSession Session { get; private set; }
        public bool HasSession => Session != null;
        public AppSettings Config { get; private set; }
        public bool HasInputError { get; private set; }

        public async Task CreateSession(string languageCode = "en")
        {
            string partitionKey = DateTime.Now.ToString("yyyyMM");
            string salt = Guid.NewGuid().ToString();
            Session = new UserSession()
            {
                UserSessionId = SessionIdHelper.GenerateSessionId(salt),
                Salt = salt,
                StartedDt = DateTime.Now,
                LanguageCode = languageCode,
                PartitionKey = partitionKey
            };
            await userSessionRepository.CreateUserSession(Session);
        }

        public async Task UpdateSession()
        {
            if (!HasSession) throw new Exception("No session created");
            await userSessionRepository.UpdateUserSession(Session);
        }

        public async Task Reload(string code)
        {
            string partitionKey = DateTime.Now.ToString("yyyyMM"); // TODO:
            var userSession = await userSessionRepository.GetUserSession(code, partitionKey);
            Session = userSession;
        }

        private async Task CheckForAnswer()
        {
            AnswerOption answerValue;
            HasInputError = false;
            if (Enum.TryParse(FormData.GetValues("selected_answer")?.FirstOrDefault(), out answerValue))
            {
                string questionId = FormData.GetValues("questionId").FirstOrDefault();
                var questionRepository = new QuestionRepository(this.Config.CosmosSettings);
                var question = await questionRepository.GetQuestion(questionId);
                if (question == null)
                {
                    throw new Exception($"QuestionId {questionId} could not be found on session {Session?.UserSessionId}");
                }
                var previousAnswers = Session.RecordedAnswers.Where(x => x.QuestionId == questionId).ToList();
                Session.RecordedAnswers.RemoveAll(x => x.QuestionId == questionId);
                var answer = new Answer()
                {
                    AnsweredDt = DateTime.Now,
                    SelectedOption = answerValue,
                    QuestionId = questionId,
                    QuestionNumber = FormData.GetValues("questionNumber")?.FirstOrDefault(),
                    QuestionText = FormData.GetValues("questionText")?.FirstOrDefault(),
                    TraitCode = question.TraitCode,
                    IsNegative = question.IsNegative,
                };
                Session.RecordedAnswers.Add(answer);
                if (Session.RecordedAnswers.Count == Session.MaxQuestions)
                {
                    // We have complete the session as we have all the answers
                    Session.IsComplete = true;
                    Session.CompleteDt = DateTime.Now;
                    await userSessionRepository.UpdateUserSession(Session);
                }
            }
            else
            {
                HasInputError = true;
            }
        }
    }
}

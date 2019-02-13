using System;
using System.Collections.Specialized;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Dfc.DiscoverSkillsAndCareers.Models;
using Dfc.DiscoverSkillsAndCareers.Repositories;
using Dfc.DiscoverSkillsAndCareers.WebApp.Config;
using Dfc.DiscoverSkillsAndCareers.WebApp.Helpers;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;

namespace Dfc.DiscoverSkillsAndCareers.Services
{
    public class UserSessionService : IUserSessionService
    {
        readonly IUserSessionRepository UserSessionRepository;
        readonly IQuestionRepository QuestionRepository;

        public UserSessionService(IOptions<AppSettings> appSettings, IUserSessionRepository userSessionRepository, IQuestionRepository questionRepository)
        {
            Config = appSettings?.Value;
            UserSessionRepository = userSessionRepository;
            QuestionRepository = questionRepository;
        }

        public async Task Init(HttpRequest request)
        {
            UserSession userSession = null;

            if (request.HasFormContentType)
            {
                FormData = await request.ReadFormAsync();
            }

            string sessionId = TryGetSessionId(request);
            if (string.IsNullOrEmpty(sessionId) == false)
            {
                userSession = await UserSessionRepository.GetUserSession(sessionId);
            }
            Session = userSession;

            if (HasSession && FormData != null)
            {
                await CheckForAnswer();
            }
        }

        public IFormCollection FormData { get; private set; }
        public UserSession Session { get; private set; }
        public bool HasSession { get { return Session != null; } }
        public IAppSettings Config { get; private set; }
        public bool HasInputError { get; private set; }
        public NameValueCollection QueryDictionary { get; private set; }
        private string SessionSalt = "ncs";

        public string TryGetSessionId(HttpRequest request)
        {
            string sessionId = string.Empty;
            string cookieSessionId;
            request.Cookies.TryGetValue("ncs-session-id", out cookieSessionId);
            sessionId = cookieSessionId;

            QueryDictionary = System.Web.HttpUtility.ParseQueryString(request.QueryString.ToString());
            var code = QueryDictionary.Get("sessionId");
            if (string.IsNullOrEmpty(code) == false)
            {
                sessionId = code;
            }

            if (request.HasFormContentType)
            {
                try
                {
                    string formSessionId = GetFormValue("sessionId");

                    if (string.IsNullOrEmpty(formSessionId) == false)
                    {
                        sessionId = formSessionId;
                    }
                }
                catch { };
            }
            return sessionId;
        }

        public string GetFormValue(string key)
        {
            if (FormData == null) return null;
            StringValues value;
            FormData.TryGetValue(key, out value);
            if (value.Count == 0) return null;
            return value[0];
        }

        public int? GetFormNumberValue(string key)
        {
            var value = GetFormValue(key);
            int number;
            if (int.TryParse(value, out number))
            {
                return number;
            }
            return null;
        }

        public async Task CreateSession(string questionSetVersion, int maxQuestions, string languageCode = "en")
        {
            string partitionKey = DateTime.Now.ToString("yyyyMM");
            string salt = SessionSalt;
            Session = new UserSession()
            {
                UserSessionId = SessionIdHelper.GenerateSessionId(salt),
                Salt = salt,
                StartedDt = DateTime.Now,
                LanguageCode = languageCode,
                PartitionKey = partitionKey,
                QuestionSetVersion = questionSetVersion,
                MaxQuestions = maxQuestions,
                CurrentQuestion = 1
            };
            await UserSessionRepository.CreateUserSession(Session);
        }

        public async Task UpdateSession()
        {
            if (!HasSession) throw new Exception("No session created");
            await UserSessionRepository.UpdateUserSession(Session);
        }

        public async Task Reload(string code)
        {
            var datetimeStamp = SessionIdHelper.Decode(SessionSalt, code);
            string partitionKey = SessionIdHelper.GetYearMonth(datetimeStamp);
            var userSession = await UserSessionRepository.GetUserSession(code, partitionKey);
            Session = userSession;
        }

        private async Task CheckForAnswer()
        {
            AnswerOption answerValue;
            HasInputError = false;
            if (Enum.TryParse(GetFormValue("selected_answer"), out answerValue))
            {
                string questionId = GetFormValue("questionId");
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
                    QuestionNumber = GetFormValue("questionNumber"),
                    QuestionText = GetFormValue("questionText"),
                    TraitCode = question.TraitCode,
                    IsNegative = question.IsNegative,
                };
                Session.RecordedAnswers.Add(answer);
                if (Session.RecordedAnswers.Count == Session.MaxQuestions)
                {
                    // We have complete the session as we have all the answers
                    Session.IsComplete = true;
                    Session.CompleteDt = DateTime.Now;
                    await UserSessionRepository.UpdateUserSession(Session);
                }
            }
            else
            {
                HasInputError = true;
            }
        }
    }
}

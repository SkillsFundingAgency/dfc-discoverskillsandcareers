using System;
using System.Threading.Tasks;
using System.Collections.Specialized;
using System.Net.Http;
using System.Linq;
using Dfc.DiscoverSkillsAndCareers.Models;
using Dfc.DiscoverSkillsAndCareers.Repositories;

namespace Dfc.DiscoverSkillsAndCareers.FunctionApp
{
    public class SessionHelper
    {
        private UserSessionRepository userSessionRepository;
        private AppSettings appSettings;

        public static async Task<SessionHelper> CreateWithInit(HttpRequestMessage request, AppSettings appSettings)
        {
            
            var sessionHelper = new SessionHelper();
            await sessionHelper.Init(request, appSettings);
            return sessionHelper;
        }

        public async Task Init(HttpRequestMessage request, AppSettings appSettings)
        {
            this.appSettings = appSettings;
            userSessionRepository = new UserSessionRepository(appSettings.CosmosSettings);
            UserSession userSession = null;

            if (request.Content.IsFormData())
            {
                FormData = await request.Content.ReadAsFormDataAsync();
                var formSessionId = FormData.GetValues("sessionId").FirstOrDefault();
                if (string.IsNullOrEmpty(formSessionId) == false)
                {
                    userSession = await userSessionRepository.GetUserSession(formSessionId);
                }
            }

            Session = userSession;

            if (HasSession && FormData != null)
            {
                CheckForAnswer();
            }
        }

        public NameValueCollection FormData { get; private set; }
        public UserSession Session { get; private set; }
        public bool HasSession => Session != null;

        public async Task CreateSession(string languageCode = "en")
        {
            string partitionKey = DateTime.Now.ToString("yyyyMM");
            Session = new UserSession()
            {
                UserSessionId = SessionIdHelper.GenerateSessionId(),
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

        private void CheckForAnswer()
        {
            // TODO: testable
            string questionId = FormData.GetValues("questionId").FirstOrDefault();
            AnswerOption answer;
            if (Enum.TryParse(FormData.GetValues("selected_answer")?.FirstOrDefault(), out answer))
            {
                Session.RecordedAnswers.Add(new Answer()
                {
                    AnsweredDt = DateTime.Now,
                    SelectedOption = answer,
                    QuestionId = questionId,
                    QuestionNumber = FormData.GetValues("questionNumber")?.FirstOrDefault(),
                    QuestionText = FormData.GetValues("questionText")?.FirstOrDefault(),
                    TraitCode = FormData.GetValues("traitCode")?.FirstOrDefault(),
                });
            }
        }
    }
}

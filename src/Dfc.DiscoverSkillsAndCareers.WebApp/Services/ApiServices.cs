using Dfc.DiscoverSkillsAndCareers.Models;
using Dfc.DiscoverSkillsAndCareers.WebApp.Config;
using Dfc.DiscoverSkillsAndCareers.WebApp.Models;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.Threading.Tasks;

namespace Dfc.DiscoverSkillsAndCareers.WebApp.Services
{
    public class ApiServices : IApiServices
    {
        private IHttpService HttpService;
        private IAppSettings AppSettings;

        public ApiServices(HttpService httpService, IOptions<AppSettings> appSettings)
        {
            HttpService = httpService;
            AppSettings = appSettings.Value;
        }

        public async Task<T> GetContentModel<T>(string contentType, Guid correlationId) where T : class
        {
            try
            {
                string url = $"{AppSettings.ContentApiRoot}/content/{contentType}";
                var json = await HttpService.GetString(url, correlationId);
                var content = JsonConvert.DeserializeObject<Content>(json);
                T model = Activator.CreateInstance<T>();
                if (content != null)
                {
                    model = JsonConvert.DeserializeObject<T>(content.ContentData);
                }
                return model;
            }
            catch (System.Net.Http.HttpRequestException)
            {
                return Activator.CreateInstance<T>();
            }
        }

        public async Task<NewSessionResponse> NewSession(Guid correlationId, string assessmentType)
        {
            string url = $"{AppSettings.SessionApiRoot}/assessment?assessmentType={assessmentType}";
            var json = await HttpService.PostData(url, "", correlationId);
            return JsonConvert.DeserializeObject<NewSessionResponse>(json);
        }

        public async Task<AssessmentQuestionResponse> Reload(string sessionId, Guid correlationId)
        {
            string url = $"{AppSettings.SessionApiRoot}/assessment/{sessionId}/reload";
            var json = await HttpService.GetString(url, correlationId);
            return JsonConvert.DeserializeObject<AssessmentQuestionResponse>(json);
        }

        public async Task<AssessmentQuestionResponse> Question(string sessionId, string assessment, int questionNumber, Guid correlationId)
        {
            string url = $"{AppSettings.SessionApiRoot}/assessment/{sessionId}/{assessment}/q/{questionNumber}";
            var json = await HttpService.GetString(url, correlationId);
            return JsonConvert.DeserializeObject<AssessmentQuestionResponse>(json);
        }

        public async Task<PostAnswerResponse> PostAnswer(string sessionId, PostAnswerRequest postAnswerRequest, Guid correlationId)
        {
            string url = $"{AppSettings.SessionApiRoot}/assessment/{sessionId}";
            var json = await HttpService.PostData(url, postAnswerRequest, correlationId);
            return JsonConvert.DeserializeObject<PostAnswerResponse>(json);
        }

        public async Task<ResultsResponse> Results(string sessionId, Guid correlationId)
        {
            string url = $"{AppSettings.ResultsApiRoot}/result/{sessionId}";
            var json = await HttpService.GetString(url, correlationId);
            return JsonConvert.DeserializeObject<ResultsResponse>(json);
        }

        public async Task<NewSessionResponse> StartFilteredForJobCategory(Guid correlationId, string sessionId, string jobCategory)
        {
            string url = $"{AppSettings.SessionApiRoot}/assessment/filtered/{sessionId}/{jobCategory}";
            var json = await HttpService.PostData(url, "", correlationId);
            return JsonConvert.DeserializeObject<NewSessionResponse>(json);
        }

        public async Task<ResultsJobCategoryResult> ResultsForJobCategory(string sessionId, string jobCategory, Guid correlationId)
        {
            string url = $"{AppSettings.ResultsApiRoot}/result/{sessionId}/{jobCategory}";
            var json = await HttpService.GetString(url, correlationId);
            return JsonConvert.DeserializeObject<ResultsJobCategoryResult>(json);
        }

        public async Task<NotifyResponse> SendEmail(string domain, string emailAddress, string templateId, string sessionId, Guid correlationId)
        {
            string url = $"{AppSettings.SessionApiRoot}/assessment/notify/email";
            var data = new
            {
                domain,
                emailAddress,
                templateId,
                sessionId,
            };
            var json = await HttpService.PostData(url, data, correlationId);
            return JsonConvert.DeserializeObject<NotifyResponse>(json);
        }

        public async Task<NotifyResponse> SendSms(string domain, string mobileNumber, string templateId, string sessionId, Guid correlationId)
        {
            string url = $"{AppSettings.SessionApiRoot}/assessment/notify/sms";
            var data = new
            {
                domain,
                mobileNumber,
                templateId,
                sessionId,
            };
            var json = await HttpService.PostData(url, data, correlationId);
            return JsonConvert.DeserializeObject<NotifyResponse>(json);
        }
    }
}

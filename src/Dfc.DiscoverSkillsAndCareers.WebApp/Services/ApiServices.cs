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

        public ApiServices(IHttpService httpService, IOptions<AppSettings> appSettings)
        {
            HttpService = httpService;
            AppSettings = appSettings.Value;
        }

        public async Task<T> GetContentModel<T>(string contentType) where T : class
        {
            try
            {
                string url = $"{AppSettings.ContentApiRoot}/content/{contentType}";
                var json = await HttpService.GetString(url);
                var content = JsonConvert.DeserializeObject<Content>(json);
                T model = Activator.CreateInstance<T>();
                if (content != null)
                {
                    model = JsonConvert.DeserializeObject<T>(content.ContentData);
                }
                return model;
            }
            catch (System.Net.Http.HttpRequestException ex)
            {
                return Activator.CreateInstance<T>();
            }
        }

        public async Task<NewSessionResponse> NewSession()
        {
            string url = $"{AppSettings.SessionApiRoot}/assessment";
            var json = await HttpService.PostData(url, "");
            return JsonConvert.DeserializeObject<NewSessionResponse>(json);
        }

        public async Task<NextQuestionResponse> NextQuestion(string sessionId)
        {
            string url = $"{AppSettings.SessionApiRoot}/assessment/{sessionId}/next";
            var json = await HttpService.GetString(url);
            return JsonConvert.DeserializeObject<NextQuestionResponse>(json);
        }

        public async Task<PostAnswerResponse> PostAnswer(string sessionId, PostAnswerRequest postAnswerRequest)
        {
            string url = $"{AppSettings.SessionApiRoot}/assessment/{sessionId}";
            var json = await HttpService.PostData(url, postAnswerRequest);
            return JsonConvert.DeserializeObject<PostAnswerResponse>(json);
        }
    }
}

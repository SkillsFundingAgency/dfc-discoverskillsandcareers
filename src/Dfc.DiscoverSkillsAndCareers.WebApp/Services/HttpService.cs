using Dfc.DiscoverSkillsAndCareers.WebApp.Config;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Net.Http;
using System.Threading.Tasks;

namespace Dfc.DiscoverSkillsAndCareers.WebApp.Services
{
    [ExcludeFromCodeCoverage]
    public class HttpService : IHttpService
    {
        HttpClient _httpClient;
        ILogger<HttpService> _logger;

        public HttpService(HttpClient httpClient, ILogger<HttpService> logger, IOptions<AppSettings> settings)
        {
            _httpClient =  httpClient;
            _logger = logger;
            
            if(!String.IsNullOrWhiteSpace(settings.Value.APIAuthorisationCode))
            {
                _httpClient.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", settings.Value.APIAuthorisationCode);
            }

        }

        public async Task<string> GetString(string url, Guid? dssCorrelationId)
        {
            _logger.LogInformation(url);
            AddCorrelationId(dssCorrelationId);
            using (HttpResponseMessage res = await _httpClient.GetAsync(url))
            {
                res.EnsureSuccessStatusCode();
                using (HttpContent content = res.Content)
                {
                    return await content.ReadAsStringAsync();
                }
            }
        }
        
        private void AddCorrelationId(Guid? dssCorrelationId)
        {
            _httpClient.DefaultRequestHeaders.Remove("DssCorrelationId");
            if (dssCorrelationId != null)
            {
                _httpClient.DefaultRequestHeaders.Add("DssCorrelationId", dssCorrelationId.Value.ToString());
            }
        }

        public async Task<string> PostData(string url, object data, Guid? dssCorrelationId)
        {
            _logger.LogInformation(url);
            AddCorrelationId(dssCorrelationId);
            using (HttpResponseMessage res = await _httpClient.PostAsync(url, new JsonContent(data)))
            {
                res.EnsureSuccessStatusCode();
                using (HttpContent content = res.Content)
                {
                    return await content.ReadAsStringAsync();
                }
            }
        }

        public class JsonContent : StringContent
        {
            public JsonContent(object obj) :
                base(JsonConvert.SerializeObject(obj), System.Text.Encoding.UTF8, "application/json")
            { }
        }
    }
}


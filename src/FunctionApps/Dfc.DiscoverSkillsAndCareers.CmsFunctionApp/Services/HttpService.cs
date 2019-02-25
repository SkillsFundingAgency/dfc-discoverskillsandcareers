using Newtonsoft.Json;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace Dfc.DiscoverSkillsAndCareers.CmsFunctionApp.Services
{
    public class HttpService : IHttpService
    {
        HttpClient _httpClient;
        ILogger<HttpService> _logger;

        public HttpService(ILogger<HttpService> logger)
        {
            _httpClient = new HttpClient();
            _logger = logger;
        }

        public async Task<string> GetString(string url)
        {
            _logger.LogInformation(url);
            //TODO: add header DssCorrelationId guid
            return await _httpClient.GetStringAsync(url);
        }

        public async Task<string> PostData(string url, object data)
        {
            _logger.LogInformation(url);
            //TODO: add header DssCorrelationId guid
            using (HttpResponseMessage res = await _httpClient.PostAsync(url, new JsonContent(data)))
            {
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

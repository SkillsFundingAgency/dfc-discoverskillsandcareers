using Newtonsoft.Json;
using System.Net.Http;
using System.Threading.Tasks;

namespace Dfc.DiscoverSkillsAndCareers.WebApp.Services
{
    public class HttpService : IHttpService
    {
        HttpClient _httpClient;

        public HttpService()
        {
            _httpClient = new HttpClient();
        }

        public async Task<string> GetString(string url)
        {
            return await _httpClient.GetStringAsync(url);
        }

        public async Task<string> PostData(string url, object data)
        {
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

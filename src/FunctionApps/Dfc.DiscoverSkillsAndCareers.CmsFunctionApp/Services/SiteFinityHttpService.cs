using Newtonsoft.Json;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using Microsoft.Extensions.Options;

namespace Dfc.DiscoverSkillsAndCareers.CmsFunctionApp.Services
{

    public class SiteFinityHttpService : ISiteFinityHttpService
    {
        public class AuthToken
        {
            [JsonProperty("access_token")]
            public string AccessToken { get; set; }

            [JsonProperty("expires_in")]
            public int ExpiresIn { get; set; }

            [JsonProperty("token_type")]
            public string TokenType { get; set; }

            [JsonProperty("refresh_token")]
            public string RefreshToken { get; set; }
        }

        protected HttpClient _httpClient;
        ILogger<SiteFinityHttpService> _logger;

        IOptions<AppSettings> _appSettings;

        public SiteFinityHttpService(ILogger<SiteFinityHttpService> logger, IOptions<AppSettings> appSettings)
        {
            _httpClient = new HttpClient();
            _logger = logger;
            _appSettings = appSettings;
        }

        public async Task Authenticate(string url)
        {
            var formData = new FormUrlEncodedContent(new [] {
                 new KeyValuePair<string, string>("client_id", _appSettings.Value.SiteFinityClientId),
                 new KeyValuePair<string, string>("client_secret", _appSettings.Value.SiteFinityClientSecret), 
                 new KeyValuePair<string, string>("username", _appSettings.Value.SiteFinityUsername), 
                 new KeyValuePair<string, string>("password", _appSettings.Value.SiteFinityPassword), 
                 new KeyValuePair<string, string>("grant_type", "password"), 
                 new KeyValuePair<string, string>("scope", _appSettings.Value.SiteFinityScope)
            });

            var tokenResponse = await _httpClient.PostAsync(url, formData);
            tokenResponse.EnsureSuccessStatusCode();

            var body = await tokenResponse.Content.ReadAsStringAsync();
            var token = JsonConvert.DeserializeObject<AuthToken>(body);
            
             _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue(token.TokenType, token.AccessToken);
        }

        public async Task<string> GetString(string url)
        {
            _logger.LogInformation(url);
            return await _httpClient.GetStringAsync(url);
        }

        public async Task<string> PostData(string url, object data)
        {
            _logger.LogInformation(url);
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
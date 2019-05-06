using Newtonsoft.Json;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using Microsoft.Extensions.Options;
using System;
using System.Text;

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

            public DateTime Expiry { get; set; }

            public bool HasExpired => DateTime.UtcNow >= Expiry;
        }

        private HttpClient _httpClient;
        private AuthToken _currentAuthToken;
        private object _syncObject = new object();
        ILogger<SiteFinityHttpService> _logger;

        IOptions<AppSettings> _appSettings;

        public SiteFinityHttpService(ILogger<SiteFinityHttpService> logger, IOptions<AppSettings> appSettings)
        {
            _httpClient = new HttpClient();
            _logger = logger;
            _appSettings = appSettings;
        }

        private async Task Authenticate()
        {
            
                var url = $"{_appSettings.Value.SiteFinityApiUrlbase}/{_appSettings.Value.SiteFinityApiAuthenicationEndpoint}";
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

                _logger.LogInformation($"Trying to acquire SiteFinity Auth Token: {url}");

                var body = await tokenResponse.Content.ReadAsStringAsync();

                lock(_syncObject) {
                    _currentAuthToken = JsonConvert.DeserializeObject<AuthToken>(body);
                    _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue(_currentAuthToken.TokenType, _currentAuthToken.AccessToken);
                    _currentAuthToken.Expiry = DateTime.UtcNow.AddSeconds(_currentAuthToken.ExpiresIn);
                    
                }

                _logger.LogInformation($"Auth token acquired: expires @ {_currentAuthToken.Expiry.ToString("dd/MM/yyyy HH:mm:ss")}");
            
        }

        private async Task TryAuthenticate() 
        {
            if(_appSettings.Value.SiteFinityRequiresAuthentication && (_currentAuthToken == null || _currentAuthToken.HasExpired)) {
                await Authenticate();
            }
        }

        public async Task<string> GetString(string url)
        {
            _logger.LogInformation($"GET: {url}");

            await TryAuthenticate();

            return await _httpClient.GetStringAsync(url);
        }

        public async Task<string> PostData(string url, object data)
        {
            _logger.LogInformation($"POST: {url}");
            await TryAuthenticate();
            
            using (HttpResponseMessage res = await _httpClient.PostAsync(url, new JsonContent(data)))
            {
                using (HttpContent content = res.Content)
                {
                    return await content.ReadAsStringAsync();
                }
            }
        }

        public async Task<string> PostData(string url, string data)
        {
            _logger.LogInformation($"POST: {url}");
            await TryAuthenticate();
            
            using (HttpResponseMessage res = await _httpClient.PostAsync(url, new StringContent(data, Encoding.UTF8, "application/json")))
            {
                using (HttpContent content = res.Content)
                {
                    return await content.ReadAsStringAsync();
                }
            }
        }

        public async Task Delete(string url)
        {
            _logger.LogInformation(url);
            await TryAuthenticate();
            
            using (HttpResponseMessage res = await _httpClient.DeleteAsync(url))
            {
                res.EnsureSuccessStatusCode();
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
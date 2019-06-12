using Newtonsoft.Json;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using Microsoft.Extensions.Options;
using System;
using System.Linq;
using System.Text;
using Dfc.DiscoverSkillsAndCareers.CmsFunctionApp.Models;
using Dfc.DiscoverSkillsAndCareers.Models.Extensions;
using Newtonsoft.Json.Linq;

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
            
            var url = $"{_appSettings.Value.SiteFinityApiUrlBase}/{_appSettings.Value.SiteFinityApiAuthenicationEndpoint}";
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
        
        private async Task<List<JObject>> GetContentTypeInstances(string contentType)
        {
            return await GetAll<JObject>(contentType);
        }

        private async Task<string> GetString(string url)
        {
            _logger.LogInformation($"GET: {url}");

            await TryAuthenticate();

            return await _httpClient.GetStringAsync(url);
        }

        public async Task<string> PostData(string contentType, object data)
        {
            var url = $"{_appSettings.Value.SiteFinityApiUrlBase}/api/{_appSettings.Value.SiteFinityApiWebService}/{contentType}";
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

        public async Task<string> PostData(string contentType, string data)
        {
            var url = $"{_appSettings.Value.SiteFinityApiUrlBase}/api/{_appSettings.Value.SiteFinityApiWebService}/{contentType}";
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

        public async Task Delete(string contentType)
        {
            var url =
                $"{_appSettings.Value.SiteFinityApiUrlBase}/api/{_appSettings.Value.SiteFinityApiWebService}/{contentType}";
            _logger.LogInformation(url);
            await TryAuthenticate();
            
            using (HttpResponseMessage res = await _httpClient.DeleteAsync(url))
            {
                res.EnsureSuccessStatusCode();
            }
        }

        public async Task<T> Get<T>(string contentType) where T : class
        {
            var data = await GetAll<T>(contentType);
            return data.Single();
        }
        
        public async Task<List<T>> GetAll<T>(string contentType) where T : class
        {
            var contentTypeUrl = new Uri($"{_appSettings.Value.SiteFinityApiUrlBase}/{contentType}");

            var isExhusted = false;
            var data = new List<T>();
            var page = 0;
            do
            {
                String url;
                if (String.IsNullOrWhiteSpace(contentTypeUrl.Query))
                {
                    url = $"{contentTypeUrl}?$top=50&$skip={50 * page}";
                }
                else
                {
                    url = $"{contentTypeUrl}&$top=50&$skip={50 * page}";
                }

                var results = await GetString(url).FromJson<SiteFinityDataFeed<T[]>>();
                if (results == null || results.Value.Length == 0)
                {
                    isExhusted = true;
                }
                else
                {

                    data.AddRange(results.Value);
                    page++;
                }

            } while (!isExhusted);

            return data.ToList();
        }
        
        
        public async Task<List<TaxonomyHierarchy>> GetTaxonomyInstances(string taxonomyInstance)
        {
            var taxonomies = await GetAll<JObject>("taxonomies");
            
            var taxaId =
                taxonomies
                    .Single(r => String.Equals(r.Value<string>("TaxonName"), taxonomyInstance, StringComparison.InvariantCultureIgnoreCase))
                    .Value<string>("Id");
            
            var taxonHierarcy = await GetAll<TaxonomyHierarchy>("hierarchy-taxa");

            var data =
                taxonHierarcy
                    .Where(x => String.Equals(x.TaxonomyId, taxaId, StringComparison.InvariantCultureIgnoreCase))
                    .ToList();

            return data;
        }

        private class JsonContent : StringContent
        {
            public JsonContent(object obj) :
                base(JsonConvert.SerializeObject(obj), System.Text.Encoding.UTF8, "application/json")
            { }
        }
    }
}
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
        public ILogger Logger { get; set; }

        IOptions<AppSettings> _appSettings;
        
        public SiteFinityHttpService(ILoggerFactory logger, IOptions<AppSettings> appSettings)
        {
            _httpClient = new HttpClient();
            Logger = logger.CreateLogger(typeof(SiteFinityHttpService));
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

            Logger.LogInformation($"Trying to acquire SiteFinity Auth Token: {url}");

            var body = await tokenResponse.Content.ReadAsStringAsync();

            lock(_syncObject) {
                _currentAuthToken = JsonConvert.DeserializeObject<AuthToken>(body);
                _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue(_currentAuthToken.TokenType, _currentAuthToken.AccessToken);
                _currentAuthToken.Expiry = DateTime.UtcNow.AddSeconds(_currentAuthToken.ExpiresIn);
                
            }

            Logger.LogInformation($"Auth token acquired: expires @ {_currentAuthToken.Expiry.ToString("dd/MM/yyyy HH:mm:ss")}");
            
        }

        private async Task TryAuthenticate() 
        {
            if(_appSettings.Value.SiteFinityRequiresAuthentication && (_currentAuthToken == null || _currentAuthToken.HasExpired)) {
                await Authenticate();
            }
        }
        

        private async Task<string> GetString(string url)
        {
            Logger.LogInformation($"GET: {url}");

            await TryAuthenticate();

            return await _httpClient.GetStringAsync(url);
        }

        public async Task<string> PostData(string contentType, object data)
        {
            var url = MakeAbsoluteUri(contentType);
            Logger.LogInformation($"POST: {url}");
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
            var url = MakeAbsoluteUri(contentType);
            Logger.LogInformation($"POST: {url}");
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
            var url = MakeAbsoluteUri(contentType);
            Logger.LogInformation(url);
            await TryAuthenticate();
            
            using (HttpResponseMessage res = await _httpClient.DeleteAsync(url))
            {
                if (!res.IsSuccessStatusCode)
                {
                    var body = await res.Content.ReadAsStringAsync();
                    throw new Exception($"Delete {contentType} failed: {body}");
                }
            }
        }

        public string MakeAbsoluteUri(string relativePortion)
        {
            return
                $"{_appSettings.Value.SiteFinityApiUrlBase}/api/{_appSettings.Value.SiteFinityApiWebService}/{relativePortion}";
        }

        public async Task<T> Get<T>(string contentType) where T : class
        {
            var url = $"{_appSettings.Value.SiteFinityApiUrlBase}/api/{_appSettings.Value.SiteFinityApiWebService}/{contentType}";

            var data = await GetString(url).FromJson<SiteFinityDataFeed<T>>();
            return data.Value;
        }
        
        public async Task<List<T>> GetAll<T>(string contentType) where T : class
        {
            var contentTypeUrl = new Uri($"{_appSettings.Value.SiteFinityApiUrlBase}/api/{_appSettings.Value.SiteFinityApiWebService}/{contentType}");

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
                    url = $"{contentTypeUrl.GetLeftPart(UriPartial.Path)}?$top=50&$skip={50 * page}&{contentTypeUrl.Query.TrimStart('?')}";
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
                    .Where(x => x.TaxonomyId.ToString() == taxaId)
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
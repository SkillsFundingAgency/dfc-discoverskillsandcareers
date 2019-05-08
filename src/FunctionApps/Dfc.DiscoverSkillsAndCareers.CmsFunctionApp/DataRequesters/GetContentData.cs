using Dfc.DiscoverSkillsAndCareers.CmsFunctionApp.Models;
using Dfc.DiscoverSkillsAndCareers.CmsFunctionApp.Services;
using Newtonsoft.Json;
using System.Linq;
using System.Threading.Tasks;

namespace Dfc.DiscoverSkillsAndCareers.CmsFunctionApp.DataRequesters
{
    public class GetContentData<T> : IGetContentData<T> where T : class
    {
        readonly ISiteFinityHttpService _httpService;

        public GetContentData(ISiteFinityHttpService httpService)
        {
            _httpService = httpService;
        }

        public async Task<T> GetData(string siteFinityApiUrlbase, string siteFinityService, string contentType)
        {
            var url = $"{siteFinityApiUrlbase}/api/{siteFinityService}/{contentType}";
            string json = await _httpService.GetString(url);
            var data = JsonConvert.DeserializeObject<SiteFinityDataFeed<T>>(json, JsonSettings.Instance);
            return data.Value;
        }
    }
}

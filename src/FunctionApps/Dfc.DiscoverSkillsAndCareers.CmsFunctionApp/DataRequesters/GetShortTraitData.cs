using Dfc.DiscoverSkillsAndCareers.CmsFunctionApp.Models;
using Dfc.DiscoverSkillsAndCareers.CmsFunctionApp.Services;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace Dfc.DiscoverSkillsAndCareers.CmsFunctionApp.DataRequesters
{
    public class GetShortTraitData : IGetShortTraitData
    {
        readonly ISiteFinityHttpService _httpService;

        public GetShortTraitData(ISiteFinityHttpService httpService)
        {
            _httpService = httpService;
        }

        public async Task<List<ShortTrait>> GetData(string siteFinityApiUrlbase, string siteFinityApiWebService)
        {
            string url = $"{siteFinityApiUrlbase}/api/{siteFinityApiWebService}/traits";
            string json = await _httpService.GetString(url);
            var data = JsonConvert.DeserializeObject<SiteFinityDataFeed<List<ShortTrait>>>(json, JsonSettings.Instance);
            return data.Value;
        }
    }
}

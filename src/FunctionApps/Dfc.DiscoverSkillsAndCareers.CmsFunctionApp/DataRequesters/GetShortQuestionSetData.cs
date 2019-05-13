using Dfc.DiscoverSkillsAndCareers.CmsFunctionApp.Models;
using Dfc.DiscoverSkillsAndCareers.CmsFunctionApp.Services;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Dfc.DiscoverSkillsAndCareers.CmsFunctionApp.DataRequesters
{
    public class GetShortQuestionSetData : IGetShortQuestionSetData
    {
        readonly ISiteFinityHttpService _httpService;

        public GetShortQuestionSetData(ISiteFinityHttpService httpService)
        {
            _httpService = httpService;
        }

        public async Task<List<ShortQuestionSet>> GetData(string siteFinityApiUrlbase, string siteFinityService)
        {
            string url = $"{siteFinityApiUrlbase}/api/{siteFinityService}/shortquestionsets";
            string json = await _httpService.GetString(url);
            var data = JsonConvert.DeserializeObject<SiteFinityDataFeed<List<ShortQuestionSet>>>(json, JsonSettings.Instance);
            return data.Value;
        }
    }
}

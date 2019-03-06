using Dfc.DiscoverSkillsAndCareers.CmsFunctionApp.Models;
using Dfc.DiscoverSkillsAndCareers.CmsFunctionApp.Services;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Dfc.DiscoverSkillsAndCareers.CmsFunctionApp.DataRequesters
{
    public class GetFilteringQuestionSetData : IGetFilteringQuestionSetData
    {
        readonly IHttpService HttpService;

        public GetFilteringQuestionSetData(IHttpService httpService)
        {
            HttpService = httpService;
        }

        public async Task<List<FilteringQuestionSet>> GetData(string siteFinityApiUrlbase)
        {
            string url = $"{siteFinityApiUrlbase}/api/default/filteringquestionsets";
            string json = await HttpService.GetString(url);
            var data = JsonConvert.DeserializeObject<SiteFinityDataFeed<List<FilteringQuestionSet>>>(json);
            return data.Value;
        }
    }
}

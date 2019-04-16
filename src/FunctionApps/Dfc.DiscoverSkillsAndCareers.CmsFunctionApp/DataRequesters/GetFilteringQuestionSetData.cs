using Dfc.DiscoverSkillsAndCareers.CmsFunctionApp.Models;
using Dfc.DiscoverSkillsAndCareers.CmsFunctionApp.Services;
using Dfc.DiscoverSkillsAndCareers.Repositories;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Dfc.DiscoverSkillsAndCareers.CmsFunctionApp.DataRequesters
{
    public class GetFilteringQuestionSetData : IGetFilteringQuestionSetData
    {
        readonly ISiteFinityHttpService HttpService;
        readonly IGetFilteringQuestionData GetFilteringQuestionData;

        public GetFilteringQuestionSetData(ISiteFinityHttpService httpService,
            IGetFilteringQuestionData getFilteringQuestionData)
        {
            HttpService = httpService;
            GetFilteringQuestionData = getFilteringQuestionData;
        }

        public async Task<List<FilteringQuestionSet>> GetData(string siteFinityApiUrlbase, string siteFinityService)
        {
            string url = $"{siteFinityApiUrlbase}/api/{siteFinityService}/filteringquestionsets";
            string json = await HttpService.GetString(url);
            var data = JsonConvert.DeserializeObject<SiteFinityDataFeed<List<FilteringQuestionSet>>>(json);
            foreach (var fqs in data.Value)
            {
                fqs.Questions = await GetFilteringQuestionData.GetData(siteFinityApiUrlbase, siteFinityService, fqs.Id);
            }
            return data.Value;
        }
    }
}

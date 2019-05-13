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
        private readonly ISiteFinityHttpService _httpService;
        private readonly IGetFilteringQuestionData _getFilteringQuestionData;

        public GetFilteringQuestionSetData(ISiteFinityHttpService httpService,
            IGetFilteringQuestionData getFilteringQuestionData)
        {
            _httpService = httpService;
            _getFilteringQuestionData = getFilteringQuestionData;
        }

        public async Task<List<FilteringQuestionSet>> GetData(string siteFinityApiUrlbase, string siteFinityService)
        {
            string url = $"{siteFinityApiUrlbase}/api/{siteFinityService}/filteringquestionsets?$expand=JobProfileTaxonomy";
            string json = await _httpService.GetString(url);
            var data = JsonConvert.DeserializeObject<SiteFinityDataFeed<List<FilteringQuestionSet>>>(json, JsonSettings.Instance);
            foreach (var fqs in data.Value)
            {
                fqs.Questions = await _getFilteringQuestionData.GetData(siteFinityApiUrlbase, siteFinityService, fqs.Id);
            }
            return data.Value;
        }
    }
}

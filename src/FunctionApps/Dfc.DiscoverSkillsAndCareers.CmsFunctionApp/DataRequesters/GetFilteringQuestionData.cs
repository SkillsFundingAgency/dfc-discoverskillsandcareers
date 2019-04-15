using Dfc.DiscoverSkillsAndCareers.CmsFunctionApp.Models;
using Dfc.DiscoverSkillsAndCareers.CmsFunctionApp.Services;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Dfc.DiscoverSkillsAndCareers.CmsFunctionApp.DataRequesters
{
    public class GetFilteringQuestionData : IGetFilteringQuestionData
    {
        readonly ISiteFinityHttpService HttpService;

        public GetFilteringQuestionData(ISiteFinityHttpService httpService)
        {
            HttpService = httpService;
        }

        public async Task<List<FilteringQuestion>> GetData(string siteFinityApiUrlbase, string siteFinityService, string questionSetId)
        {
            string getQuestionsUrl = $"{siteFinityApiUrlbase}/api/{siteFinityService}/filteringquestionsets({questionSetId})/Questions";
            string json = await HttpService.GetString(getQuestionsUrl);
            var data = JsonConvert.DeserializeObject<SiteFinityDataFeed<List<FilteringQuestion>>>(json);
            foreach (var question in data.Value)
            {
                question.ExcludesJobProfiles = new List<string>();
            }
            return data.Value;
        }
    }
}

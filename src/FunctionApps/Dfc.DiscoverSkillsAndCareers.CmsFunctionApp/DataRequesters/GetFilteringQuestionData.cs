using Dfc.DiscoverSkillsAndCareers.CmsFunctionApp.Models;
using Dfc.DiscoverSkillsAndCareers.CmsFunctionApp.Services;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
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
                string getJobProfilesUrl = $"{siteFinityApiUrlbase}/api/{siteFinityService}/filteringquestions({question.Id})/ExcludedJobProfiles";
                json = await HttpService.GetString(getJobProfilesUrl);
                var listJobProfiles = JsonConvert.DeserializeObject<SiteFinityDataFeed<List<JobProfile>>>(json);
                question.ExcludesJobProfiles = listJobProfiles.Value.Select(jp => jp.Title).ToList();
            }
            return data.Value;
        }
    }
}

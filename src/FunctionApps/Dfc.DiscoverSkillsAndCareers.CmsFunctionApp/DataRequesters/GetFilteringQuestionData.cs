using Dfc.DiscoverSkillsAndCareers.CmsFunctionApp.Models;
using Dfc.DiscoverSkillsAndCareers.CmsFunctionApp.Services;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;

namespace Dfc.DiscoverSkillsAndCareers.CmsFunctionApp.DataRequesters
{
    public class GetFilteringQuestionData : IGetFilteringQuestionData
    {
        readonly IHttpService HttpService;

        public GetFilteringQuestionData(IHttpService httpService)
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
                //string getJobProfileUrl = $"{siteFinityApiUrlbase}/api/{siteFinityService}/filteringquestions({question.Id})/ExcludesJobProfiles";
                //json = await HttpService.GetString(getJobProfileUrl);
                //var fakeJobProfiles = JsonConvert.DeserializeObject<SiteFinityDataFeed<List<FakeJobProfile>>>(json);
                //question.ExcludesJobProfiles = fakeJobProfiles.Value
                //                                    .Select(x => x.Title)
                //                                    .ToList();
                question.ExcludesJobProfiles = new List<string>();
            }
            return data.Value;
        }
    }
}

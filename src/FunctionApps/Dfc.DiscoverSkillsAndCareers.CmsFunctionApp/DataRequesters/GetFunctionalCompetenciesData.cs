using Dfc.DiscoverSkillsAndCareers.CmsFunctionApp.Models;
using Dfc.DiscoverSkillsAndCareers.CmsFunctionApp.Services;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;

namespace Dfc.DiscoverSkillsAndCareers.CmsFunctionApp.DataRequesters
{
    public class GetFunctionalCompetenciesData : IGetFunctionalCompetenciesData
    {
        readonly IHttpService HttpService;

        public GetFunctionalCompetenciesData(IHttpService httpService)
        {
            HttpService = httpService;
        }

        public async Task<List<FunctionalCompetency>> GetData(string siteFinityApiUrlbase, string siteFinityService)
        {
            string getCompetenciesUrl = $"{siteFinityApiUrlbase}/api/{siteFinityService}/functioncompetencies";
            string json = await HttpService.GetString(getCompetenciesUrl);
            var data = JsonConvert.DeserializeObject<SiteFinityDataFeed<List<FunctionalCompetency>>>(json);
            foreach (var functionalCompetency in data.Value)
            {
                string getQuestionUrl = $"{siteFinityApiUrlbase}/api/{siteFinityService}/functioncompetencies({functionalCompetency.Id})/Question";
                json = await HttpService.GetString(getQuestionUrl);
                var shortQuestion = JsonConvert.DeserializeObject<ShortQuestion>(json);
                functionalCompetency.Question = shortQuestion;

                string getJobProfilesUrl = $"{siteFinityApiUrlbase}/api/{siteFinityService}/functioncompetencies({functionalCompetency.Id})/test";
                json = await HttpService.GetString(getJobProfilesUrl);
                var listJobProfiles = JsonConvert.DeserializeObject<SiteFinityDataFeed<List<FakeJobProfile>>>(json);
                functionalCompetency.ExcludeJobProfiles = listJobProfiles.Value;
            }
            return data.Value;
        }
    }
}

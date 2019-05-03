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
        readonly ISiteFinityHttpService HttpService;

        public GetFunctionalCompetenciesData(ISiteFinityHttpService httpService)
        {
            HttpService = httpService;
        }

        public async Task<List<FunctionalCompetency>> GetData(string siteFinityApiUrlbase, string siteFinityService)
        {
            var fulllist = new List<FunctionalCompetency>();
            string json = string.Empty;
            bool exhusted = false;
            int page = 0;
            do
            {
                string getCompetenciesUrl = $"{siteFinityApiUrlbase}/api/{siteFinityService}/functioncompetencies?$skip={50 * page}";
                json = await HttpService.GetString(getCompetenciesUrl);
                var data = JsonConvert.DeserializeObject<SiteFinityDataFeed<List<FunctionalCompetency>>>(json);
                if (data.Value.Count == 0)
                {
                    exhusted = true;
                    break;
                }
                fulllist.AddRange(data.Value);
                page++;
            }
            while (!exhusted);

            foreach (var functionalCompetency in fulllist)
            {
                string getQuestionUrl = $"{siteFinityApiUrlbase}/api/{siteFinityService}/functioncompetencies({functionalCompetency.Id})/Question";
                json = await HttpService.GetString(getQuestionUrl);
                var shortQuestion = JsonConvert.DeserializeObject<ShortQuestion>(json);
                functionalCompetency.Question = shortQuestion;

                string getJobProfilesUrl = $"{siteFinityApiUrlbase}/api/{siteFinityService}/functioncompetencies({functionalCompetency.Id})/ExcludedJobProfiles";
                json = await HttpService.GetString(getJobProfilesUrl);
                var listJobProfiles = JsonConvert.DeserializeObject<SiteFinityDataFeed<List<FakeJobProfile>>>(json);
                functionalCompetency.ExcludeJobProfiles = listJobProfiles.Value;
            }
            return fulllist;
        }
    }
}

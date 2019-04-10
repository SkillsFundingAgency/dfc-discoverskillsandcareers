using Dfc.DiscoverSkillsAndCareers.CmsFunctionApp.Models;
using Dfc.DiscoverSkillsAndCareers.CmsFunctionApp.Services;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Dfc.DiscoverSkillsAndCareers.CmsFunctionApp.DataRequesters
{
    public class GetShortQuestionSetData : IGetShortQuestionSetData
    {
        readonly ISiteFinityHttpService HttpService;

        public GetShortQuestionSetData(ISiteFinityHttpService httpService)
        {
            HttpService = httpService;
        }

        public async Task<List<ShortQuestionSet>> GetData(string siteFinityApiUrlbase, string siteFinityService)
        {
            string url = $"{siteFinityApiUrlbase}/api/{siteFinityService}/shortquestionsets";
            string json = await HttpService.GetString(url);
            var data = JsonConvert.DeserializeObject<SiteFinityDataFeed<List<ShortQuestionSet>>>(json);
            return data.Value;
        }
    }
}

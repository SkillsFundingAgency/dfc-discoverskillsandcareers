using Dfc.DiscoverSkillsAndCareers.CmsFunctionApp.Models;
using Dfc.DiscoverSkillsAndCareers.CmsFunctionApp.Services;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Dfc.DiscoverSkillsAndCareers.CmsFunctionApp.DataRequesters
{
    public class GetJobProfileData : IGetJobProfileData
    {
        readonly ISiteFinityHttpService HttpService;

        public GetJobProfileData(ISiteFinityHttpService httpService)
        {
            HttpService = httpService;
        }

        public async Task<List<DfcJobProfile>> GetData(string siteFinityApiUrlbase, string siteFinityApiWebService)
        {
            string url = $"{siteFinityApiUrlbase}/api/{siteFinityApiWebService}/jobprofiles";
            string json = await HttpService.GetString(url);
            var data = JsonConvert.DeserializeObject<SiteFinityDataFeed<List<DfcJobProfile>>>(json);
            return data.Value;
        }
    }
}

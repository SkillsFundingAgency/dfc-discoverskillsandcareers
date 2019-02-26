using Dfc.DiscoverSkillsAndCareers.CmsFunctionApp.Models;
using Dfc.DiscoverSkillsAndCareers.CmsFunctionApp.Services;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace Dfc.DiscoverSkillsAndCareers.CmsFunctionApp.DataRequesters
{
    public class GetShortTraitData : IGetShortTraitData
    {
        readonly IHttpService HttpService;

        public GetShortTraitData(IHttpService httpService)
        {
            HttpService = httpService;
        }

        public async Task<List<ShortTrait>> GetData(string url)
        {
            string json = await HttpService.GetString(url);
            var data = JsonConvert.DeserializeObject<SiteFinityDataFeed<ShortTrait>>(json);
            return data.Value;
        }
    }
}

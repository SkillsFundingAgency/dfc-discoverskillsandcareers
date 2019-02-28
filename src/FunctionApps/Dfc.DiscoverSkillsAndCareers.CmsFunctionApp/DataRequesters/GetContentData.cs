using Dfc.DiscoverSkillsAndCareers.CmsFunctionApp.Models;
using Dfc.DiscoverSkillsAndCareers.CmsFunctionApp.Services;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Collections.Generic;
using Dfc.DiscoverSkillsAndCareers.Models;

namespace Dfc.DiscoverSkillsAndCareers.CmsFunctionApp.DataRequesters
{
    public class GetContentData<T> : IGetContentData<T> where T : class
    {
        readonly IHttpService HttpService;

        public GetContentData(IHttpService httpService)
        {
            HttpService = httpService;
        }

        public async Task<T> GetData(string url)
        {
            string json = await HttpService.GetString(url);
            var data = JsonConvert.DeserializeObject<SiteFinityDataFeed<T>>(json);
            return data.Value;
        }
    }
}

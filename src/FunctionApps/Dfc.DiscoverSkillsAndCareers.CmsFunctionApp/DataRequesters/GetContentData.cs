using Dfc.DiscoverSkillsAndCareers.CmsFunctionApp.Models;
using Dfc.DiscoverSkillsAndCareers.CmsFunctionApp.Services;
using Newtonsoft.Json;
using System.Linq;
using System.Threading.Tasks;

namespace Dfc.DiscoverSkillsAndCareers.CmsFunctionApp.DataRequesters
{
    public class GetContentData<T> : IGetContentData<T> where T : class
    {
        readonly ISiteFinityHttpService HttpService;

        public GetContentData(ISiteFinityHttpService httpService)
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

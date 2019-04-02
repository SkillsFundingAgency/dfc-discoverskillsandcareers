using Dfc.DiscoverSkillsAndCareers.CmsFunctionApp.Models;
using Dfc.DiscoverSkillsAndCareers.CmsFunctionApp.Services;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Dfc.DiscoverSkillsAndCareers.CmsFunctionApp.DataRequesters
{
    public class GetJobProfileData : IGetJobProfileData
    {
        readonly IHttpService HttpService;

        public GetJobProfileData(IHttpService httpService)
        {
            HttpService = httpService;
        }

        public async Task<List<DfcJobProfile>> GetData(string siteFinityApiUrlbase)
        {
            string path = @"C:\ncs\job_profiles.json";
            if (!System.IO.File.Exists(path))
            {
                return new List<DfcJobProfile>();
            }
            string json = await System.IO.File.ReadAllTextAsync(path);
            var data = JsonConvert.DeserializeObject<SiteFinityDataFeed<List<DfcJobProfile>>>(json);
            return data.Value;
        }
    }
}

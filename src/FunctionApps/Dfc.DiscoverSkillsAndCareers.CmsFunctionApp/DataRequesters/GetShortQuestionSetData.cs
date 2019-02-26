using Dfc.DiscoverSkillsAndCareers.CmsFunctionApp.Models;
using Dfc.DiscoverSkillsAndCareers.CmsFunctionApp.Services;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Dfc.DiscoverSkillsAndCareers.CmsFunctionApp.DataRequesters
{
    public class GetShortQuestionSetData : IGetShortQuestionSetData
    {
        readonly IHttpService HttpService;

        public GetShortQuestionSetData(IHttpService httpService)
        {
            HttpService = httpService;
        }

        public async Task<List<ShortQuestionSet>> GetData(string siteFinityApiUrlbase)
        {
            string url = $"{siteFinityApiUrlbase}/api/default/shortquestionsets";
            string json = await HttpService.GetString(url);
            var data = JsonConvert.DeserializeObject<SiteFinityDataFeed<List<ShortQuestionSet>>>(json);
            return data.Value;
        }
    }
}

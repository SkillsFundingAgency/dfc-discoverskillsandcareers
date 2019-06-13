using Dfc.DiscoverSkillsAndCareers.CmsFunctionApp.Models;
using Dfc.DiscoverSkillsAndCareers.CmsFunctionApp.Services;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Dfc.DiscoverSkillsAndCareers.CmsFunctionApp.DataRequesters
{
    public class GetShortQuestionSetData : IGetShortQuestionSetData
    {
        readonly ISiteFinityHttpService _httpService;

        public GetShortQuestionSetData(ISiteFinityHttpService httpService)
        {
            _httpService = httpService;
        }

        public async Task<List<SiteFinityShortQuestionSet>> GetData()
        {
            return await _httpService.GetAll<SiteFinityShortQuestionSet>("shortquestionsets");
        }
    }
}

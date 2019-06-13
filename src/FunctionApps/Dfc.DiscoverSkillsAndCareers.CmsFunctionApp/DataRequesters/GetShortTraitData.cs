using Dfc.DiscoverSkillsAndCareers.CmsFunctionApp.Models;
using Dfc.DiscoverSkillsAndCareers.CmsFunctionApp.Services;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace Dfc.DiscoverSkillsAndCareers.CmsFunctionApp.DataRequesters
{
    public class GetShortTraitData : IGetShortTraitData
    {
        readonly ISiteFinityHttpService _sitefinity;

        public GetShortTraitData(ISiteFinityHttpService sitefinity)
        {
            _sitefinity = sitefinity;
        }

        public async Task<List<SiteFinityTrait>> GetData()
        {
            return await _sitefinity.GetAll<SiteFinityTrait>("traits");
        }
    }
}

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

        public async Task<List<ShortTrait>> GetData()
        {
            return await _sitefinity.GetAll<ShortTrait>("traits");
        }
    }
}

using Dfc.DiscoverSkillsAndCareers.CmsFunctionApp.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Dfc.DiscoverSkillsAndCareers.CmsFunctionApp.DataRequesters
{
    public interface IGetShortQuestionSetData
    {
        Task<List<ShortQuestionSet>> GetData(string siteFinityApiUrlbase, string siteFinityService);
    }
}

using Dfc.DiscoverSkillsAndCareers.CmsFunctionApp.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Dfc.DiscoverSkillsAndCareers.CmsFunctionApp.DataRequesters
{
    public interface IGetFilteringQuestionSetData
    {
        Task<List<SiteFinityFilteringQuestionSet>> GetData();
    }
}

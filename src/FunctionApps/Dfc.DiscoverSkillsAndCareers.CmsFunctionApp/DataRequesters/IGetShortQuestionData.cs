using Dfc.DiscoverSkillsAndCareers.CmsFunctionApp.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Dfc.DiscoverSkillsAndCareers.CmsFunctionApp.DataRequesters
{
    public interface IGetShortQuestionData
    {
        Task<List<SiteFinityShortQuestion>> GetData(string questionSetId);
    }
}

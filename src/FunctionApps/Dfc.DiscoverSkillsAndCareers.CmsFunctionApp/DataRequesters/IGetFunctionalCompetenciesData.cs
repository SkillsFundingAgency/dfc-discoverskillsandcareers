using Dfc.DiscoverSkillsAndCareers.CmsFunctionApp.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Dfc.DiscoverSkillsAndCareers.CmsFunctionApp.DataRequesters
{
    public interface IGetFunctionalCompetenciesData
    {
        Task<List<FunctionalCompetency>> GetData(string siteFinityApiUrlbase, string siteFinityService);
    }
}

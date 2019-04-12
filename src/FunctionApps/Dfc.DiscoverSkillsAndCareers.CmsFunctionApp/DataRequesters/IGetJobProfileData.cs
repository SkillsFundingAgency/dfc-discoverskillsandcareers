using Dfc.DiscoverSkillsAndCareers.CmsFunctionApp.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Dfc.DiscoverSkillsAndCareers.CmsFunctionApp.DataRequesters
{
    public interface IGetJobProfileData
    {
        Task<List<DfcJobProfile>> GetData(string siteFinityApiUrlbase, string siteFinityApiWebService);
    }
}

using Dfc.DiscoverSkillsAndCareers.CmsFunctionApp.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Dfc.DiscoverSkillsAndCareers.CmsFunctionApp.DataRequesters
{
    public interface IGetJobCategoriesData
    {
        Task<List<JobCategory>> GetData(string url, string traitsUrl, string taxonomyId = "3b635a67-db48-43d2-b94b-332304775d37");
    }
}

using System.Collections.Generic;
using Dfc.DiscoverSkillsAndCareers.Models;
using Microsoft.Azure.Documents;
using System.Threading.Tasks;

namespace Dfc.DiscoverSkillsAndCareers.Repositories
{
    public interface IJobProfileRepository
    {
        Task<JobProfile[]> JobProfilesTitle(IDictionary<string, string> socCodeTitleMap);
        Task<JobProfile[]> JobProfilesForJobFamily(string jobFamily);
    }
}
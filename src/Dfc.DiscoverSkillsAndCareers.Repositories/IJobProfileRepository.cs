using Dfc.DiscoverSkillsAndCareers.Models;
using Microsoft.Azure.Documents;
using System.Threading.Tasks;

namespace Dfc.DiscoverSkillsAndCareers.Repositories
{
    public interface IJobProfileRepository
    {
        Task<JobProfile> GetJobProfile(string socCode, string partitionKey);
        Task<JobProfile[]> GetJobProfilesForJobFamily(string jobFamily);
        Task<Document> CreateJobProfile(JobProfile jobProfile);
    }
}

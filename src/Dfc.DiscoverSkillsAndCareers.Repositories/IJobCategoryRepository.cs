using Dfc.DiscoverSkillsAndCareers.Models;
using System.Threading.Tasks;

namespace Dfc.DiscoverSkillsAndCareers.Repositories
{
    public interface IJobCategoryRepository
    {
        Task<JobFamily> GetJobCategory(string socCode, string partitionKey);
        Task CreateJobCategory(JobFamily jobProfile);
        Task<JobFamily[]> GetJobCategories(string partitionKey);
        Task DeleteJobCategory(string partitionKey, JobFamily jobFamily);
        Task<JobFamily[]> GetJobCategoriesByName(string partitionKey, string jobFamilyName);
    }
}

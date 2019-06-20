using Dfc.DiscoverSkillsAndCareers.Models;
using System.Threading.Tasks;

namespace Dfc.DiscoverSkillsAndCareers.Repositories
{
    public interface IJobCategoryRepository
    {
        Task<JobCategory> GetJobCategory(string jobCategoryCode, string partitionKey = "job-categories");
        Task CreateOrUpdateJobCategory(JobCategory jobCategory);
        Task<JobCategory[]> GetJobCategories(string partitionKey = "job-categories");

    }
}

using Dfc.DiscoverSkillsAndCareers.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Dfc.DiscoverSkillsAndCareers.Repositories
{
    public interface IJobCategoryRepository
    {
        Task<JobFamily> GetJobCategory(string socCode, string partitionKey);
        Task CreateJobCategory(JobFamily jobProfile);
        Task<JobFamily[]> GetJobCategories(string partitionKey);
    }
}

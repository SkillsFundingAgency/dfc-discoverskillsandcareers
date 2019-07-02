using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Dfc.DiscoverSkillsAndCareers.Models;
using Dfc.DiscoverSkillsAndCareers.Models.Extensions;
using Newtonsoft.Json;

namespace Dfc.DiscoverSkillsAndCareers.Repositories
{
    public class LocalJobCategoryRepository : IJobCategoryRepository
    {
        private JobCategory[] Data { get; set; }
        
        public LocalJobCategoryRepository()
        {
            using (var stream = Assembly.GetAssembly(typeof(LocalShortTraitRepository)).GetManifestResourceStream("Dfc.DiscoverSkillsAndCareers.Repositories.data.JobCategories.json"))
            using(var sr = new StreamReader(stream))
            {
                Data = JsonConvert.DeserializeObject<JobCategory[]>(sr.ReadToEnd());
            }
        }

        public Task<JobCategory> GetJobCategory(string jobCategoryCode, string partitionKey = "job-categories")
        {
            return Task.FromResult(Data.FirstOrDefault(j => StringExtensions.EqualsIgnoreCase(j.Code, jobCategoryCode)));
        }

        public Task CreateOrUpdateJobCategory(JobCategory jobCategory)
        {
            return Task.CompletedTask;
        }

        public Task<JobCategory[]> GetJobCategories(string partitionKey = "job-categories")
        {
            return Task.FromResult(Data);
        }
    }
}
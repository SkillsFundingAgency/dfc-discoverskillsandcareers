using Dfc.DiscoverSkillsAndCareers.CmsFunctionApp.DataRequesters;
using Dfc.DiscoverSkillsAndCareers.CmsFunctionApp.Services;
using Dfc.DiscoverSkillsAndCareers.Repositories;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Threading.Tasks;
using Dfc.DiscoverSkillsAndCareers.Models;
using System.Linq;
using System.Collections.Generic;

namespace Dfc.DiscoverSkillsAndCareers.CmsFunctionApp.DataProcessors
{
    public class JobCategoryDataProcessor : IJobCategoryDataProcessor
    {
        readonly IGetJobCategoriesData _getJobCategoriesData;
        readonly AppSettings _appSettings;
        readonly IJobCategoryRepository _jobCategoryRepository;

        public JobCategoryDataProcessor(
            IGetJobCategoriesData getJobCategoriesData,
            IOptions<AppSettings> appSettings,
            IJobCategoryRepository jobCategoryRepository)
        {
            _getJobCategoriesData = getJobCategoriesData;
            _appSettings = appSettings.Value;
            _jobCategoryRepository = jobCategoryRepository;
        }

        public async Task RunOnce(ILogger logger)
        {
            logger.LogInformation("Begin poll for JobCategories");
            var data = await _getJobCategoriesData.GetData(_appSettings.SiteFinityJobCategoriesTaxonomyId);

            logger.LogInformation($"Have {data?.Count} job Categorys to save");

            var partitionKey = "jobfamily-cms";
            var codes = new List<string>();
            foreach (var jobCategory in data)
            {
                var code = JobCategoryHelper.GetCode(jobCategory.Title);
                // Remove any old job categories that have this title but will have a different code
                var existingJobCategories = await _jobCategoryRepository.GetJobCategoriesByName(partitionKey, jobCategory.Title);
                existingJobCategories = existingJobCategories.Where(x => x.JobFamilyCode != code).ToArray();
                for (var i = 0; i < existingJobCategories.Count(); i++)
                {
                    await _jobCategoryRepository.DeleteJobCategory(partitionKey, existingJobCategories[i]);
                }
                // Import the new job category
                await _jobCategoryRepository.CreateJobCategory(new JobFamily()
                {
                    JobFamilyName = jobCategory.Title,
                    Texts = new[]
                    {
                        new JobFamilyText()
                        {
                            LanguageCode = "en",
                            Text = jobCategory.Description,
                            Url = $"{jobCategory.UrlName}"
                        }
                    },
                    TraitCodes = jobCategory.Traits.Select(x => x.ToUpper()).ToArray(),
                    JobFamilyCode = code,
                    PartitionKey = partitionKey
                });
                codes.Add(code);
            }

            logger.LogInformation("End poll for JobCategories");
        }
    }
}

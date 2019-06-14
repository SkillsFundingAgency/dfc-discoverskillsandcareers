using Dfc.DiscoverSkillsAndCareers.CmsFunctionApp.Services;
using Dfc.DiscoverSkillsAndCareers.Repositories;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Threading.Tasks;
using Dfc.DiscoverSkillsAndCareers.Models;
using System.Linq;
using System.Collections.Generic;
using Dfc.DiscoverSkillsAndCareers.CmsFunctionApp.Models;

namespace Dfc.DiscoverSkillsAndCareers.CmsFunctionApp.DataProcessors
{
    public class JobCategoryDataProcessor : IJobCategoryDataProcessor
    {
        readonly AppSettings _appSettings;
        private readonly ISiteFinityHttpService _sitefinity;
        readonly IJobCategoryRepository _jobCategoryRepository;

        public JobCategoryDataProcessor(
            ISiteFinityHttpService sitefinity,
            IOptions<AppSettings> appSettings,
            IJobCategoryRepository jobCategoryRepository)
        {
            _appSettings = appSettings.Value;
            _sitefinity = sitefinity;
            _jobCategoryRepository = jobCategoryRepository;
        }
        
        public async Task<List<SiteFinityJobCategory>> GetData(string taxonomyName = "Job Profile Category")
        {
            var traits = await _sitefinity.GetAll<SiteFinityTrait>("traits");

            var taxonomies = await _sitefinity.GetTaxonomyInstances(taxonomyName);
            
            var jobCategories = taxonomies
                .Select(x => new SiteFinityJobCategory
                {
                    Id = x.Id,
                    TaxonomyId = x.TaxonomyId,
                    Title = x.Title,
                    Description = x.Description,
                    UrlName = x.UrlName,
                    Traits = new List<string>()
                }).ToList();
            
            foreach (var jobCategory in jobCategories)
            {
                foreach (var trait in traits)
                {
                    if (trait.JobProfileCategories.Contains(jobCategory.Id))
                    {
                        jobCategory.Traits.Add(trait.Name);
                    }
                }
            }
            return jobCategories.Where(jc => jc.Traits.Count > 0).ToList();
        }

        public async Task RunOnce(ILogger logger)
        {
            logger.LogInformation("Begin poll for JobCategories");
            var data = await GetData(_appSettings.SiteFinityJobCategoriesTaxonomyId);

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

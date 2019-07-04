using System.Threading.Tasks;
using Dfc.DiscoverSkillsAndCareers.CmsFunctionApp.Models;
using Dfc.DiscoverSkillsAndCareers.Models;
using Dfc.DiscoverSkillsAndCareers.Repositories;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Dfc.DiscoverSkillsAndCareers.CmsFunctionApp.DataProcessors
{
    public class JobProfileSkillsProcessor : IContentTypeProcessor<JobProfileSkillsProcessor>
    {
        private readonly ISiteFinityHttpService _sitefinity;
        private readonly IJobCategoryRepository _jobCategoryRepository;
        private readonly IOptions<AppSettings> _appSettings;
        private ILogger _logger;

        public JobProfileSkillsProcessor(
            ISiteFinityHttpService sitefinity,
            IJobCategoryRepository jobCategoryRepository,
            IOptions<AppSettings> appSettings)
        {
            _sitefinity = sitefinity;
            _jobCategoryRepository = jobCategoryRepository;
            _appSettings = appSettings;
        }
        
        public async Task RunOnce(ILogger logger)
        {
            _logger = logger;
            
            _logger.LogInformation($"Started updating job profile skills mapping");
            
            var jobProfiles = 
                await _sitefinity.GetAll<SiteFinityJobProfile>("jobprofiles?$select=Id,Title,JobProfileCategories&$expand=RelatedSkills&$orderby=Title");
            
            var jobCategories = 
                await _sitefinity.GetTaxonomyInstances("Job Profile Category");
                
            var categorySkillMappings = JobCategorySkillMapper.Map(jobProfiles, jobCategories,
                _appSettings.Value.MaxPercentageOfProfileOccurenceForSkill,
                _appSettings.Value.MaxPercentageDistributionOfJobProfiles);

            foreach (var categorySkill in categorySkillMappings)
            {
                _logger.LogInformation($"Updating job category skill mapping {categorySkill.JobCategory}");
                await CreateJobCategoryQuestionSet(categorySkill);
            }
        }
        
        private async Task CreateJobCategoryQuestionSet(JobCategorySkillMappingResult skillsMapping)
        {
            // Remove any old job categories that have this title but will have a different code
            var category = await _jobCategoryRepository.GetJobCategory(JobCategoryHelper.GetCode(skillsMapping.JobCategory));

            if (category != null)
            {
                category.Skills.Clear();
                category.Skills.AddRange(skillsMapping.SkillMappings);
            }
            else
            {
                _logger.LogError($"Unable to add skills to category {skillsMapping.JobCategory} - the category does not exist");
            }

            await _jobCategoryRepository.CreateOrUpdateJobCategory(category);
        }
    }
}
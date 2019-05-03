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
        readonly ISiteFinityHttpService HttpService;
        readonly IGetJobCategoriesData GetJobCategoriesData;
        readonly AppSettings AppSettings;
        readonly IJobCategoryRepository JobCategoryRepository;

        public JobCategoryDataProcessor(
            ISiteFinityHttpService httpService,
            IGetJobCategoriesData getJobCategoriesData,
            IOptions<AppSettings> appSettings,
            IJobCategoryRepository jobCategoryRepository)
        {
            HttpService = httpService;
            GetJobCategoriesData = getJobCategoriesData;
            AppSettings = appSettings.Value;
            JobCategoryRepository = jobCategoryRepository;
        }

        public async Task RunOnce(ILogger logger)
        {
            logger.LogInformation("Begin poll for JobCategories");
            var data = await GetJobCategoriesData.GetData(AppSettings.SiteFinityApiUrlbase, AppSettings.SiteFinityApiWebService, AppSettings.SiteFinityJobCategoriesTaxonomyId);

            logger.LogInformation($"Have {data?.Count} job Categorys to save");

            var codes = new List<string>();
            foreach (var jobCategory in data)
            {
                var code = JobCategoryHelper.GetCode(jobCategory.Title);
                await JobCategoryRepository.CreateJobCategory(new JobFamily()
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
                    JobFamilyCode = code
                });
                codes.Add(code);
            }

            logger.LogInformation("End poll for JobCategories");
        }
    }
}

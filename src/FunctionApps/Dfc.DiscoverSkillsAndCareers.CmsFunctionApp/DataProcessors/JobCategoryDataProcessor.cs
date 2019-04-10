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
        readonly ILogger<JobCategoryDataProcessor> Logger;
        readonly ISiteFinityHttpService HttpService;
        readonly IGetJobCategoriesData GetJobCategoriesData;
        readonly AppSettings AppSettings;
        readonly IJobCategoryRepository JobCategoryRepository;

        public JobCategoryDataProcessor(
            ILogger<JobCategoryDataProcessor> logger,
            ISiteFinityHttpService httpService,
            IGetJobCategoriesData getJobCategoriesData,
            IOptions<AppSettings> appSettings,
            IJobCategoryRepository jobCategoryRepository)
        {
            Logger = logger;
            HttpService = httpService;
            GetJobCategoriesData = getJobCategoriesData;
            AppSettings = appSettings.Value;
            JobCategoryRepository = jobCategoryRepository;
        }

        public async Task RunOnce()
        {
            Logger.LogInformation("Begin poll for JobCategories");

            string siteFinityApiUrlbase = AppSettings.SiteFinityApiUrlbase;
            string siteFinityService = AppSettings.SiteFinityApiWebService;

            string getJobCategoriesUrl = $"{siteFinityApiUrlbase}/api/{siteFinityService}/hierarchy-taxa";
            string getTraitsUrl = $"{siteFinityApiUrlbase}/api/{siteFinityService}/traits";
            var data = await GetJobCategoriesData.GetData(getJobCategoriesUrl, getTraitsUrl);

            Logger.LogInformation($"Have {data?.Count} job Categorys to save");

            var codes = new List<string>();
            foreach (var jobCategory in data)
            {
                var code = GetCode(jobCategory.Title);
                await JobCategoryRepository.CreateJobCategory(new JobFamily()
                {
                    JobFamilyName = jobCategory.Title,
                    Texts = new[]
                    {
                        new JobFamilyText()
                        {
                            LanguageCode = "en",
                            Text = jobCategory.Description,
                            Url = $"https://nationalcareers.service.gov.uk/job-categories/{jobCategory.UrlName}"
                        }
                    },
                    TraitCodes = jobCategory.Traits.Select(x => x.ToUpper()).ToArray(),
                    JobFamilyCode = code
                });
                codes.Add(code);
            }

            Logger.LogInformation("End poll for JobCategories");
        }

        public string GetCode(string input)
        {
            string code = "";
            var words = input.Split(" ");
            if (words.Count() > 1)
            {
                for (var i = 0; i < words.Count(); i++)
                {
                    code += words[i].Substring(0, 1).ToUpper();
                }
            }
            else
            {
                code = input.Substring(0, 3).ToUpper();
            }
            return code;
        }
    }
}

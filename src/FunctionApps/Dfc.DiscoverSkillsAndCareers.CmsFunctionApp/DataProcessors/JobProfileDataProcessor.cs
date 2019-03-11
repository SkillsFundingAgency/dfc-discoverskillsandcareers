using Dfc.DiscoverSkillsAndCareers.CmsFunctionApp.DataRequesters;
using Dfc.DiscoverSkillsAndCareers.CmsFunctionApp.Services;
using Dfc.DiscoverSkillsAndCareers.Repositories;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Threading.Tasks;
using Dfc.DiscoverSkillsAndCareers.Models;

namespace Dfc.DiscoverSkillsAndCareers.CmsFunctionApp.DataProcessors
{
    public class JobProfileDataProcessor : IJobProfileDataProcessor
    {
        readonly ILogger<JobProfileDataProcessor> Logger;
        readonly IHttpService HttpService;
        readonly IGetJobProfileData GetJobProfileData;
        readonly AppSettings AppSettings;
        readonly IJobProfileRepository JobProfileRepository;

        public JobProfileDataProcessor(
            ILogger<JobProfileDataProcessor> logger,
            IHttpService httpService,
            IGetJobProfileData getJobProfileData,
            IOptions<AppSettings> appSettings,
            IJobProfileRepository jobProfileRepository)
        {
            Logger = logger;
            HttpService = httpService;
            GetJobProfileData = getJobProfileData;
            AppSettings = appSettings.Value;
            JobProfileRepository = jobProfileRepository;
        }

        public async Task RunOnce()
        {
            Logger.LogInformation("Begin poll for JobProfiles");

            string siteFinityApiUrlbase = AppSettings.SiteFinityApiUrlbase;

            string url = $"{siteFinityApiUrlbase}/api/default/jobProfiles";
            var data = await GetJobProfileData.GetData(url);

            Logger.LogInformation($"Have {data?.Count} job profiles to save");

            foreach (var jobProfile in data)
            {
                await JobProfileRepository.CreateJobProfile(new JobProfile()
                {
                    CareerPathAndProgression = jobProfile.CareerPathAndProgression,
                    JobProfileCategories = jobProfile.JobProfileCategories,
                    Overview = jobProfile.Overview,
                    PartitionKey = "cms",
                    SalaryExperienced = jobProfile.SalaryExperienced,
                    SalaryStarter = jobProfile.SalaryStarter,
                    SocCode = jobProfile.SocCode,
                    Title = jobProfile.Title,
                    UrlName = jobProfile.UrlName,
                    WYDDayToDayTasks = jobProfile.WYDDayToDayTasks
                });
            }

            Logger.LogInformation("End poll for JobProfiles");
        }
    }
}

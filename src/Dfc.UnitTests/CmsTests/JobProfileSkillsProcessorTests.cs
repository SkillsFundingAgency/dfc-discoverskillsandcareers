using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Dfc.DiscoverSkillsAndCareers.CmsFunctionApp;
using Dfc.DiscoverSkillsAndCareers.CmsFunctionApp.DataProcessors;
using Dfc.DiscoverSkillsAndCareers.CmsFunctionApp.Models;
using Dfc.DiscoverSkillsAndCareers.CmsFunctionApp.Services;
using Dfc.DiscoverSkillsAndCareers.Models;
using Dfc.DiscoverSkillsAndCareers.Repositories;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using NSubstitute;
using Xunit;

namespace Dfc.UnitTests.CmsTests
{
    public class JobProfileSkillsProcessorTests
    {
        private ISiteFinityHttpService _siteFinityHttpService;
        private IJobCategoryRepository _jobCategoryRepository;
        private JobProfileSkillsProcessor _sut;
        private ILogger _logger;
        private IOptions<AppSettings> _appSettings;
        
        private List<SiteFinityJobProfile> JobProfiles { get; }

        private List<TaxonomyHierarchy> JobCategories { get; }

        public JobProfileSkillsProcessorTests()
        {
            _siteFinityHttpService = Substitute.For<ISiteFinityHttpService>();
            _jobCategoryRepository = Substitute.For<IJobCategoryRepository>();
            _appSettings = Substitute.For<IOptions<AppSettings>>();
            _logger = Substitute.For<ILogger>();

            JobCategories =
                JsonConvert.DeserializeObject<List<TaxonomyHierarchy>>(File.ReadAllText("Data/job-categories.json"));
            JobProfiles =
                JsonConvert.DeserializeObject<List<SiteFinityJobProfile>>(File.ReadAllText("Data/job-profiles.json"));
            
            _appSettings.Value.Returns(new AppSettings()
            {
                MaxPercentageDistributionOfJobProfiles = 0.75,
                MaxPercentageOfProfileOccurenceForSkill = 0.75
            });

            _sut = new JobProfileSkillsProcessor(_siteFinityHttpService, _jobCategoryRepository, _appSettings);
        }

        

        [Fact]
        public async Task CreatesAllOfTheCategoriesIfNotExist()
        {
            _siteFinityHttpService
                .GetAll<SiteFinityJobProfile>(
                    "jobprofiles?$select=Id,Title,JobProfileCategories&$expand=RelatedSkills&$orderby=Title")
                .Returns(Task.FromResult(JobProfiles));

            _siteFinityHttpService.GetTaxonomyInstances("Job Profile Category").Returns(Task.FromResult(JobCategories));

            await _sut.RunOnce(_logger);

            await _jobCategoryRepository.Received(JobCategories.Count).CreateOrUpdateJobCategory(Arg.Any<JobCategory>());
        }
        
        [Fact]
        public async Task UpdatesAllOfTheCategoriesIfNotExist()
        {
            _siteFinityHttpService
                .GetAll<SiteFinityJobProfile>(
                    "jobprofiles?$select=Id,Title,JobProfileCategories&$expand=RelatedSkills&$orderby=Title")
                .Returns(Task.FromResult(JobProfiles));

            _siteFinityHttpService.GetTaxonomyInstances("Job Profile Category").Returns(Task.FromResult(JobCategories));

            _jobCategoryRepository.GetJobCategory(Arg.Any<string>()).Returns(Task.FromResult(new JobCategory()));
            
            await _sut.RunOnce(_logger);

            await _jobCategoryRepository.Received(JobCategories.Count).CreateOrUpdateJobCategory(Arg.Any<JobCategory>());
        }
    }
}
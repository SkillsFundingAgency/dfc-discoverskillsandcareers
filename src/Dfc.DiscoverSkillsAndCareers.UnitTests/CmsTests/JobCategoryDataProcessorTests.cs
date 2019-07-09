using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Dfc.DiscoverSkillsAndCareers.CmsFunctionApp;
using Dfc.DiscoverSkillsAndCareers.CmsFunctionApp.DataProcessors;
using Dfc.DiscoverSkillsAndCareers.CmsFunctionApp.Models;
using Dfc.DiscoverSkillsAndCareers.Models;
using Dfc.DiscoverSkillsAndCareers.Models.SiteFinity;
using Dfc.DiscoverSkillsAndCareers.Repositories;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NSubstitute;
using Xunit;

namespace Dfc.UnitTests.CmsTests
{
    public class JobCategoryDataProcessorTests
    {
        private ISiteFinityHttpService _siteFinityHttpService;
        private IJobCategoryRepository _jobCategoryRepository;
        private JobCategoryDataProcessor _sut;
        private ILogger _logger;
        private IOptions<AppSettings> _appSettings;

        public JobCategoryDataProcessorTests()
        {
            _siteFinityHttpService = Substitute.For<ISiteFinityHttpService>();
            _jobCategoryRepository = Substitute.For<IJobCategoryRepository>();
            _appSettings = Substitute.For<IOptions<AppSettings>>();
            _logger = Substitute.For<ILogger>();

            _appSettings.Value.Returns(new AppSettings()
            {
                SiteFinityJobCategoriesTaxonomyId = "Job Profile Category"
            });

            _sut = new JobCategoryDataProcessor(_siteFinityHttpService, _appSettings, _jobCategoryRepository);
        }

        [Fact]
        public async Task GetData_LinksTraitsAndCateogriesCorrectly()
        {
            var jobCategoryGuid = Guid.NewGuid();
            
            _siteFinityHttpService.GetAll<SiteFinityTrait>("traits").Returns(Task.FromResult(new List<SiteFinityTrait>
            {
                new SiteFinityTrait { Name = "Leader", JobProfileCategories = new List<Guid> { jobCategoryGuid }}
            }));

            _siteFinityHttpService.GetTaxonomyInstances("Job Profile Category").Returns(Task.FromResult(
                new List<TaxonomyHierarchy>
                {
                    new TaxonomyHierarchy { Id = jobCategoryGuid, Title = "Animal care" }
                }));

            var result = await _sut.GetData("Job Profile Category");

            Assert.Single(result, r => r.Title == "Animal care" && r.Traits.Contains("Leader"));
        }

        [Fact]
        public async Task IfCategoryNotExistsTheItIsCreated()
        {
            var jobCategoryGuid = Guid.NewGuid();
            
            _siteFinityHttpService.GetAll<SiteFinityTrait>("traits").Returns(Task.FromResult(new List<SiteFinityTrait>
            {
                new SiteFinityTrait { Name = "Leader", JobProfileCategories = new List<Guid> { jobCategoryGuid }}
            }));

            _siteFinityHttpService.GetTaxonomyInstances("Job Profile Category").Returns(Task.FromResult(
            new List<TaxonomyHierarchy>
            {
                new TaxonomyHierarchy { Id = jobCategoryGuid, Title = "Animal care" }
            }));

            await _sut.RunOnce(_logger);

            await _jobCategoryRepository.Received(1).CreateOrUpdateJobCategory(Arg.Any<JobCategory>());

        }
        
        [Fact]
        public async Task IfCategoryExistsThenItIsUpdated()
        {
            var jobCategoryGuid = Guid.NewGuid();
            
            _siteFinityHttpService.GetAll<SiteFinityTrait>("traits").Returns(Task.FromResult(new List<SiteFinityTrait>
            {
                new SiteFinityTrait { Name = "Leader", JobProfileCategories = new List<Guid> { jobCategoryGuid }}
            }));

            _siteFinityHttpService.GetTaxonomyInstances("Job Profile Category").Returns(Task.FromResult(
                new List<TaxonomyHierarchy>
                {
                    new TaxonomyHierarchy { Id = jobCategoryGuid, Title = "Animal care" }
                }));

            _jobCategoryRepository.GetJobCategory("AC").Returns(Task.FromResult(new JobCategory
            {
                Name = "Animal care"
            }));
            
            await _sut.RunOnce(_logger);
            
            await _jobCategoryRepository.Received(1).CreateOrUpdateJobCategory(Arg.Any<JobCategory>());

        }
    }
}
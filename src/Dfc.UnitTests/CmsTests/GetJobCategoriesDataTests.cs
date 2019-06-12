using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Dfc.DiscoverSkillsAndCareers.CmsFunctionApp.DataRequesters;
using Dfc.DiscoverSkillsAndCareers.CmsFunctionApp.Models;
using Dfc.DiscoverSkillsAndCareers.CmsFunctionApp.Services;
using Newtonsoft.Json;
using NSubstitute;
using Xunit;

namespace Dfc.UnitTests.CmsTests
{
    public class GetJobCategoriesDataTests
    {
        private ISiteFinityHttpService _siteFinityHttpService;
        private GetJobCategoriesData _sut;
        
        public GetJobCategoriesDataTests()
        {
            _siteFinityHttpService = Substitute.For<ISiteFinityHttpService>();
            _sut = new GetJobCategoriesData(_siteFinityHttpService);
        }

        [Fact]
        public async Task GetData_ShouldHave_CorrectData()
        {
            var jobCategoryGuid = Guid.NewGuid();
            var jcTaxonId = Guid.NewGuid();
            
            _siteFinityHttpService.GetAll<ShortTrait>($"traits").Returns(Task.FromResult(new List<ShortTrait>
                {
                    new ShortTrait { Id = "trait1", Code = "Leader", Name = "Leader", JobProfileCategories = new List<Guid> { jobCategoryGuid }}
                }));
            
            _siteFinityHttpService.Get<ShortTrait>($"traits(trait1)").Returns(Task.FromResult(new ShortTrait
            {
                Code = "Leader", Name = "Leader", JobProfileCategories = new List<Guid> { jobCategoryGuid }
                
            }));

            _siteFinityHttpService.GetTaxonomyInstances("Job Profile Category").Returns(
                Task.FromResult( new List<TaxonomyHierarchy>
                    {
                        new TaxonomyHierarchy { Id = jobCategoryGuid.ToString(), TaxonomyId = jcTaxonId.ToString() }
                    }));
            
            var result = await _sut.GetData("Job Profile Category");
            
            Assert.Collection(result, fq => Assert.Single(fq.Traits, j => j == "LEADER"));
        }
    }
}
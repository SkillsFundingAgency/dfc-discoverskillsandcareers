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
        
        private const string SitefinityUrl = "https://localhost:8080";
        private const string SitefinityService = "dsac";
        
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
            
            _siteFinityHttpService.GetString($"{SitefinityUrl}/api/{SitefinityService}/traits").Returns(Task.FromResult(JsonConvert.SerializeObject(new SiteFinityDataFeed<List<ShortTrait>>
            {
                Value = new List<ShortTrait>
                {
                    new ShortTrait { Id = "trait1", Code = "Leader", Name = "Leader", JobProfileCategories = new List<Guid> { jobCategoryGuid }}
                }
            })));
            
            _siteFinityHttpService.GetString($"{SitefinityUrl}/api/{SitefinityService}/traits(trait1)").Returns(Task.FromResult(JsonConvert.SerializeObject(new ShortTrait
            {
                Code = "Leader", Name = "Leader", JobProfileCategories = new List<Guid> { jobCategoryGuid }
                
            })));

            _siteFinityHttpService.GetString($"{SitefinityUrl}/api/{SitefinityService}/taxonomies").Returns(
                Task.FromResult(
                    JsonConvert.SerializeObject(new SiteFinityDataFeed<List<object>> { 
                        Value = new List<object>
                        {
                            new { TaxonName = "Job Profile Category", Id = jcTaxonId.ToString() }
                        }
                    })));

            _siteFinityHttpService.GetString($"{SitefinityUrl}/api/{SitefinityService}/hierarchy-taxa").Returns(
                Task.FromResult(JsonConvert.SerializeObject(new SiteFinityDataFeed<List<TaxonomyHierarchy>>
                {
                    Value = new List<TaxonomyHierarchy>
                    {
                        new TaxonomyHierarchy { Id = jobCategoryGuid.ToString(), TaxonomyId = jcTaxonId.ToString() }
                    }
                })));
            
            var result = await _sut.GetData(SitefinityUrl, SitefinityService, "Job Profile Category");
            
            Assert.Collection(result, fq => Assert.Single(fq.Traits, j => j == "LEADER"));
        }
    }
}
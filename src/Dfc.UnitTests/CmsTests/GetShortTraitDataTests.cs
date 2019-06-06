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
    public class GetShortTraitDataTests
    {
        private ISiteFinityHttpService _siteFinityHttpService;
        private GetShortTraitData _sut;
        private const string SitefinityUrl = "https://localhost:8080";
        private const string SitefinityService = "dsac";
        
        public GetShortTraitDataTests()
        {
            _siteFinityHttpService = Substitute.For<ISiteFinityHttpService>();

            _sut = new GetShortTraitData(_siteFinityHttpService);
        }

        [Fact]
        public async Task GetData_ShouldHave_CorrectData()
        {
            var trait = "LEADER";
            var shortQuestionSetsUrl =
                $"{SitefinityUrl}/api/{SitefinityService}/traits";
            
            var shortTraitResponse = new SiteFinityDataFeed<List<ShortTrait>>
            {
                Value = new List<ShortTrait>
                {
                    new ShortTrait { Name = trait }
                }
            };

            _siteFinityHttpService.GetString(shortQuestionSetsUrl).Returns(Task.FromResult(JsonConvert.SerializeObject(shortTraitResponse)));
            
            var result = await _sut.GetData(SitefinityUrl, SitefinityService);
            
            Assert.Collection(result, fq => Assert.Equal(trait, fq.Name));
        }
        
    }
}
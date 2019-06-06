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
    public class GetShortQuestionSetDataTests
    {
        private ISiteFinityHttpService _siteFinityHttpService;
        private GetShortQuestionSetData _sut;
        private const string SitefinityUrl = "https://localhost:8080";
        private const string SitefinityService = "dsac";
        
        public GetShortQuestionSetDataTests()
        {
            _siteFinityHttpService = Substitute.For<ISiteFinityHttpService>();

            _sut = new GetShortQuestionSetData(_siteFinityHttpService);
        }

        [Fact]
        public async Task GetData_ShouldHave_CorrectData()
        {
            var questionSetId = "QS-1-1";
            var shortQuestionSetsUrl =
                $"{SitefinityUrl}/api/{SitefinityService}/shortquestionsets";
            
            var shortQuestionResponse = new SiteFinityDataFeed<List<ShortQuestionSet>>
            {
                Value = new List<ShortQuestionSet>
                {
                    new ShortQuestionSet { Id = questionSetId }
                }
            };

            _siteFinityHttpService.GetString(shortQuestionSetsUrl).Returns(Task.FromResult(JsonConvert.SerializeObject(shortQuestionResponse)));
            
            var result = await _sut.GetData(SitefinityUrl, SitefinityService);
            
            Assert.Collection(result, fq => Assert.Equal(questionSetId, fq.Id));
        }
        
    }
}
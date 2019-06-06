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
    public class GetShortQuestionDataTests
    {
        private ISiteFinityHttpService _siteFinityHttpService;
        private GetShortQuestionData _sut;
        private const string SitefinityUrl = "https://localhost:8080";
        private const string SitefinityService = "dsac";
        
        public GetShortQuestionDataTests()
        {
            _siteFinityHttpService = Substitute.For<ISiteFinityHttpService>();

            _sut = new GetShortQuestionData(_siteFinityHttpService);
        }

        [Fact]
        public async Task GetData_ShouldHave_CorrectData()
        {
            var questionSetId = "QS-1-1";
            var shortQuestionUrl =
                $"{SitefinityUrl}/api/{SitefinityService}/shortquestionsets({questionSetId})/Questions";
            
            var shortQuestionResponse = new SiteFinityDataFeed<List<ShortQuestion>>
            {
                Value = new List<ShortQuestion>
                {
                    new ShortQuestion { Id = "question1" }
                }
            };

            _siteFinityHttpService.GetString(shortQuestionUrl).Returns(Task.FromResult(JsonConvert.SerializeObject(shortQuestionResponse)));
            
            var questionTraitUrl =
                $"{SitefinityUrl}/api/{SitefinityService}/shortquestions(question1)/Trait";

            var traitsResponse = new ShortTrait {Name = "Leader"};
            
            _siteFinityHttpService.GetString(questionTraitUrl).Returns(Task.FromResult(JsonConvert.SerializeObject(traitsResponse)));

            var result = await _sut.GetData(SitefinityUrl, SitefinityService, questionSetId);
            
            Assert.Collection(result, fq => Assert.Equal("Leader", fq.Trait));
        }
        
    }
}
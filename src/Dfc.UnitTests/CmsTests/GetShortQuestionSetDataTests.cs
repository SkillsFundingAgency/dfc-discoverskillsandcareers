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
        
        public GetShortQuestionSetDataTests()
        {
            _siteFinityHttpService = Substitute.For<ISiteFinityHttpService>();

            _sut = new GetShortQuestionSetData(_siteFinityHttpService);
        }

        [Fact]
        public async Task GetData_ShouldHave_CorrectData()
        {
            var questionSetId = "QS-1-1";

            var shortQuestionResponse = new List<SiteFinityShortQuestionSet>
            {
                new SiteFinityShortQuestionSet {Id = questionSetId}
            };

            _siteFinityHttpService.GetAll<SiteFinityShortQuestionSet>("shortquestionsets").Returns(Task.FromResult(shortQuestionResponse));
            
            var result = await _sut.GetData();
            
            Assert.Collection(result, fq => Assert.Equal(questionSetId, fq.Id));
        }
        
    }
}
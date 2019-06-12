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
        
        public GetShortQuestionDataTests()
        {
            _siteFinityHttpService = Substitute.For<ISiteFinityHttpService>();

            _sut = new GetShortQuestionData(_siteFinityHttpService);
        }

        [Fact]
        public async Task GetData_ShouldHave_CorrectData()
        {
            var questionSetId = "QS-1-1";
            
            var shortQuestionResponse =  new List<ShortQuestion>
                {
                    new ShortQuestion { Id = "question1" }
                };

            _siteFinityHttpService.GetAll<ShortQuestion>($"shortquestionsets({questionSetId})/Questions").Returns(Task.FromResult(shortQuestionResponse));
            
            var questionTraitUrl =
                $"shortquestions(question1)/Trait";

            var traitsResponse = new ShortTrait {Name = "Leader"};
            
            _siteFinityHttpService.Get<ShortTrait>(questionTraitUrl).Returns(Task.FromResult(traitsResponse));

            var result = await _sut.GetData(questionSetId);
            
            Assert.Collection(result, fq => Assert.Equal("Leader", fq.Trait));
        }
        
    }
}
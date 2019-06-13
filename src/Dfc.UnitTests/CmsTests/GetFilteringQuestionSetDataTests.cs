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
    public class GetFilteringQuestionSetDataTests
    {
        private ISiteFinityHttpService _siteFinityHttpService;
        private GetFilteringQuestionSetData _sut;
        private IGetFilteringQuestionData _getFilteringQuestionData;

        
        public GetFilteringQuestionSetDataTests()
        {
            _siteFinityHttpService = Substitute.For<ISiteFinityHttpService>();
            _getFilteringQuestionData = Substitute.For<IGetFilteringQuestionData>();
            

            _sut = new GetFilteringQuestionSetData(_siteFinityHttpService, _getFilteringQuestionData);
        }

        [Fact]
        public async Task GetData_ShouldHave_CorrectData()
        {
            var questionSetId = "QS-1-1";

            _getFilteringQuestionData.GetData(questionSetId).Returns(Task.FromResult(
                new List<SiteFinityFilteringQuestion>
                {
                    new SiteFinityFilteringQuestion {Id = "question1"}
                }));
            
            var jobProfileResponse = new List<SiteFinityFilteringQuestionSet>
                {
                    new SiteFinityFilteringQuestionSet { Id = questionSetId, Title = "fq-set-1" }
                };
            
            _siteFinityHttpService.GetAll<SiteFinityFilteringQuestionSet>("filteringquestionsets?$expand=JobProfileTaxonomy").Returns(Task.FromResult(jobProfileResponse));

            var result = await _sut.GetData();
            
            Assert.Collection(result, fq => Assert.Single(fq.Questions, j => j.Id == "question1"));
        }
    }
}
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
        
        private const string SitefinityUrl = "https://localhost:8080";
        private const string SitefinityService = "dsac";
        
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

            _getFilteringQuestionData.GetData(SitefinityUrl, SitefinityService, questionSetId).Returns(Task.FromResult(
                new List<FilteringQuestion>
                {
                    new FilteringQuestion {Id = "question1"}
                }));

            var filteringQuestionSetUrl =
                $"{SitefinityUrl}/api/{SitefinityService}/filteringquestionsets?$expand=JobProfileTaxonomy";
            
            var jobProfileResponse = new SiteFinityDataFeed<List<FilteringQuestionSet>>
            {
                Value = new List<FilteringQuestionSet>
                {
                    new FilteringQuestionSet { Id = questionSetId, Title = "fq-set-1" }
                }
            };
            
            _siteFinityHttpService.GetString(filteringQuestionSetUrl).Returns(Task.FromResult(JsonConvert.SerializeObject(jobProfileResponse)));

            var result = await _sut.GetData(SitefinityUrl, SitefinityService);
            
            Assert.Collection(result, fq => Assert.Single(fq.Questions, j => j.Id == "question1"));
        }
    }
}
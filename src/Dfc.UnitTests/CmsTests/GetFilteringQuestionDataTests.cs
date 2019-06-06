using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Dfc.DiscoverSkillsAndCareers.CmsFunctionApp.DataRequesters;
using Dfc.DiscoverSkillsAndCareers.CmsFunctionApp.Models;
using NSubstitute;
using Dfc.DiscoverSkillsAndCareers.CmsFunctionApp.Services;
using Newtonsoft.Json;
using Xunit;

namespace Dfc.UnitTests.CmsTests
{
    public class GetFilteringQuestionDataTests
    {
        private ISiteFinityHttpService _siteFinityHttpService;
        private GetFilteringQuestionData _sut;
        private const string SitefinityUrl = "https://localhost:8080";
        private const string SitefinityService = "dsac";
        
        public GetFilteringQuestionDataTests()
        {
            _siteFinityHttpService = Substitute.For<ISiteFinityHttpService>();

            _sut = new GetFilteringQuestionData(_siteFinityHttpService);
        }

        [Fact]
        public async Task GetData_ShouldHave_CorrectData()
        {
            var questionSetId = "QS-1-1";
            var filteringQuestionUrl =
                $"{SitefinityUrl}/api/{SitefinityService}/filteringquestionsets({questionSetId})/Questions";
            var filteringQuestionResponse = new SiteFinityDataFeed<List<FilteringQuestion>>
            {
                Value = new List<FilteringQuestion>
                {
                    new FilteringQuestion { Id = "question1" }
                }
            };

            _siteFinityHttpService.GetString(filteringQuestionUrl).Returns(Task.FromResult(JsonConvert.SerializeObject(filteringQuestionResponse)));
            
            var jobProfileUrl =
                $"{SitefinityUrl}/api/{SitefinityService}/filteringquestions(question1)/ExcludedJobProfiles";
            
            var jobProfileResponse = new SiteFinityDataFeed<List<JobProfile>>
            {
                Value = new List<JobProfile>
                {
                    new JobProfile() { Id = "Jobprofile1", Title = "Jobprofile1" }
                }
            };
            
            _siteFinityHttpService.GetString(jobProfileUrl).Returns(Task.FromResult(JsonConvert.SerializeObject(jobProfileResponse)));

            var result = await _sut.GetData(SitefinityUrl, SitefinityService, questionSetId);
            
            Assert.Collection(result, fq => Assert.Single(fq.ExcludesJobProfiles, j => j == "Jobprofile1"));
        }
        
    }
}
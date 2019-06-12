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
        public GetFilteringQuestionDataTests()
        {
            _siteFinityHttpService = Substitute.For<ISiteFinityHttpService>();

            _sut = new GetFilteringQuestionData(_siteFinityHttpService);
        }

        [Fact]
        public async Task GetData_ShouldHave_CorrectData()
        {
            var questionSetId = "QS-1-1";
            var filteringQuestionUrl = $"filteringquestionsets({questionSetId})/Questions";
            var filteringQuestionResponse = new List<FilteringQuestion>
            {
                new FilteringQuestion {Id = "question1"}
            };

            _siteFinityHttpService.GetAll<FilteringQuestion>(filteringQuestionUrl).Returns(Task.FromResult(filteringQuestionResponse));
            
            var jobProfileUrl =
                $"filteringquestions(question1)/ExcludedJobProfiles";
            
            var jobProfileResponse = new List<JobProfile>
                {
                    new JobProfile() { Id = "Jobprofile1", Title = "Jobprofile1" }
                };
            
            _siteFinityHttpService.GetAll<JobProfile>(jobProfileUrl).Returns(Task.FromResult(jobProfileResponse));

            var result = await _sut.GetData(questionSetId);
            
            Assert.Collection(result, fq => Assert.Single(fq.ExcludesJobProfiles, j => j == "Jobprofile1"));
        }
        
    }
}
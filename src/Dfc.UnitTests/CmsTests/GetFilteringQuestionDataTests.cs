using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Dfc.DiscoverSkillsAndCareers.CmsFunctionApp;
using Dfc.DiscoverSkillsAndCareers.CmsFunctionApp.DataRequesters;
using Dfc.DiscoverSkillsAndCareers.CmsFunctionApp.Models;
using NSubstitute;
using Dfc.DiscoverSkillsAndCareers.CmsFunctionApp.Services;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Xunit;

namespace Dfc.UnitTests.CmsTests
{
    public class GetFilteringQuestionDataTests
    {
        private ISiteFinityHttpService _siteFinityHttpService;
        private GetFilteringQuestionData _sut;
        private IOptions<AppSettings> _appSettings;

        public GetFilteringQuestionDataTests()
        {
            _siteFinityHttpService = Substitute.For<ISiteFinityHttpService>();
            _appSettings = Substitute.For<IOptions<AppSettings>>();

            _appSettings.Value.Returns(new AppSettings
            {
                MaxQuestionsPerCategory = 4,
                MaxPercentageOfProfileOccurenceForSkill = 0.75
            });

            _sut = new GetFilteringQuestionData(_siteFinityHttpService, _appSettings);
        }

        [Fact]
        public async Task GetData_ShouldHave_CorrectData()
        {
            var questionSetId = "QS-1-1";
            var filteringQuestionUrl = $"filteringquestionsets({questionSetId})/Questions";
            var filteringQuestionResponse = new List<SiteFinityFilteringQuestion>
            {
                new SiteFinityFilteringQuestion {Id = "question1"}
            };

            _siteFinityHttpService.GetAll<SiteFinityFilteringQuestion>(filteringQuestionUrl).Returns(Task.FromResult(filteringQuestionResponse));
            
            var jobProfileUrl =
                $"filteringquestions(question1)/ExcludedJobProfiles";
            
            var jobProfileResponse = new List<SiteFinityJobProfile>
                {
                    new SiteFinityJobProfile() { Id = Guid.Empty, Title = "Jobprofile1" }
                };
            
            _siteFinityHttpService.GetAll<SiteFinityJobProfile>(jobProfileUrl).Returns(Task.FromResult(jobProfileResponse));

            var result = await _sut.GetData(questionSetId);
            
            Assert.Collection(result, fq => Assert.Single(fq.JobProfiles, j => j.JobProfile == "Jobprofile1"));
        }
        
    }
}
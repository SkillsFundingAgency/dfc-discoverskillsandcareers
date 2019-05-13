using Dfc.DiscoverSkillsAndCareers.AssessmentFunctionApp.Services;
using Dfc.DiscoverSkillsAndCareers.Models;
using Dfc.DiscoverSkillsAndCareers.Repositories;
using Microsoft.Extensions.Logging;
using NSubstitute;
using System;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Dfc.UnitTests
{
    public class FilteringAssessmentCalculationTests
    {
        private IQuestionRepository _questionRepository;
        private IJobProfileRepository _jobProfileRepository;
        private ILogger _logger;
        private IFilterAssessmentCalculationService _filterAssessmentCalculationService;

        public FilteringAssessmentCalculationTests()
        {
            _questionRepository = Substitute.For<IQuestionRepository>();
            _jobProfileRepository = Substitute.For<IJobProfileRepository>();
            _logger = Substitute.For<ILogger>();

            _filterAssessmentCalculationService = new FilterAssessmentCalculationService(_questionRepository, _jobProfileRepository);

            _questionRepository.GetQuestions(FilteringResultMockData.QuestionSetVersion).Returns(
                Task.FromResult(
                    FilteringResultMockData.FilteringQuestions.ToArray()
                )
            );

            _jobProfileRepository.JobProfilesForJobFamily("jobcategory1").Returns(
                Task.FromResult(
                    FilteringResultMockData.JobProfiles
                        .Where(x => x.JobProfileCategories.Contains("jobcategory1"))
                        .ToArray()
                )
            );
        }

        [Fact]
        public void CalculateAssessment_WithAnswersBothNo_ShouldHaveProfile5And6AndBothNegativeStatements()
        {
            string jobFamilyName = "jobcategory1";
            var userSession = new UserSession()
            {
                ResultData = new ResultData()
                {
                    JobFamilies = new[]
                    {
                        new JobFamilyResult { JobFamilyName = jobFamilyName }
                    }
                },
                FilteredAssessmentState = new FilteredAssessmentState()
                {
                    QuestionSetVersion = FilteringResultMockData.QuestionSetVersion,
                    JobFamilyName = jobFamilyName,
                    RecordedAnswers = new[]
                    {
                        new Answer() { AnsweredDt = DateTime.Now, SelectedOption = AnswerOption.No, QuestionSetVersion = FilteringResultMockData.QuestionSetVersion, QuestionId = "q1"  },
                        new Answer() { AnsweredDt = DateTime.Now, SelectedOption = AnswerOption.No, QuestionSetVersion = FilteringResultMockData.QuestionSetVersion, QuestionId = "q2" }
                    }
                }
            };
            _filterAssessmentCalculationService.CalculateAssessment(userSession, _logger);

            var jobFamily = userSession.ResultData.JobFamilies.First(jf => String.Equals(jf.JobFamilyName, userSession.FilteredAssessmentState.JobFamilyName, StringComparison.InvariantCultureIgnoreCase));

            var results = jobFamily.FilterAssessment.SuggestedJobProfiles;

            var whatYouToldUs = jobFamily.FilterAssessment.WhatYouToldUs;

            Assert.NotEmpty(results);
            Assert.Equal(2, results.Count);
            Assert.True(results.ContainsKey("jp5"));
            Assert.True(results.ContainsKey("jp6"));
            Assert.Equal(2, whatYouToldUs.Length);
            Assert.Equal("q1_negative", whatYouToldUs[0]);
            Assert.Equal("q2_negative", whatYouToldUs[1]);
        }

        [Fact]
        public void CalculateAssessment_WithAnswersNoYes_ShouldHaveProfile4And5And6And7AndBothNegativeAndPositiveStatements()
        {
            string jobFamilyName = "jobcategory1";
            var userSession = new UserSession()
            {
                ResultData = new ResultData()
                {
                    JobFamilies = new[]
                    {
                        new JobFamilyResult { JobFamilyName = jobFamilyName }
                    }
                },
                FilteredAssessmentState = new FilteredAssessmentState()
                {
                    QuestionSetVersion = FilteringResultMockData.QuestionSetVersion,
                    JobFamilyName = jobFamilyName,
                    RecordedAnswers = new[]
                    {
                        new Answer() { AnsweredDt = DateTime.Now, SelectedOption = AnswerOption.No, QuestionSetVersion = FilteringResultMockData.QuestionSetVersion, QuestionId = "q1"  },
                        new Answer() { AnsweredDt = DateTime.Now, SelectedOption = AnswerOption.Yes, QuestionSetVersion = FilteringResultMockData.QuestionSetVersion, QuestionId = "q2" }
                    }
                }
            };
            _filterAssessmentCalculationService.CalculateAssessment(userSession, _logger);

            var jobFamily = userSession.ResultData.JobFamilies.First(jf => String.Equals(jf.JobFamilyName, userSession.FilteredAssessmentState.JobFamilyName, StringComparison.InvariantCultureIgnoreCase));

            var results = jobFamily.FilterAssessment.SuggestedJobProfiles;

            var whatYouToldUs = jobFamily.FilterAssessment.WhatYouToldUs;

            Assert.NotEmpty(results);
            Assert.Equal(4, results.Count);
            Assert.True(results.ContainsKey("jp4"));
            Assert.True(results.ContainsKey("jp5"));
            Assert.True(results.ContainsKey("jp6"));
            Assert.True(results.ContainsKey("jp7"));
            Assert.Equal(2, whatYouToldUs.Length);
            Assert.Equal("q1_negative", whatYouToldUs[0]);
            Assert.Equal("q2_positive", whatYouToldUs[1]);
        }

        [Fact]
        public void CalculateAssessment_WithAnswersBothYes_ShouldHaveAllAndProfilesAndBothPositiveStatements()
        {
            string jobFamilyName = "jobcategory1";
            var userSession = new UserSession()
            {
                ResultData = new ResultData()
                {
                    JobFamilies = new[]
                    {
                        new JobFamilyResult { JobFamilyName = jobFamilyName }
                    }
                },
                FilteredAssessmentState = new FilteredAssessmentState()
                {
                    QuestionSetVersion = FilteringResultMockData.QuestionSetVersion,
                    JobFamilyName = jobFamilyName,
                    RecordedAnswers = new[]
                    {
                        new Answer() { AnsweredDt = DateTime.Now, SelectedOption = AnswerOption.Yes, QuestionSetVersion = FilteringResultMockData.QuestionSetVersion, QuestionId = "q1"  },
                        new Answer() { AnsweredDt = DateTime.Now, SelectedOption = AnswerOption.Yes, QuestionSetVersion = FilteringResultMockData.QuestionSetVersion, QuestionId = "q2" }
                    }
                }
            };
            _filterAssessmentCalculationService.CalculateAssessment(userSession, _logger);

            var jobFamily = userSession.ResultData.JobFamilies.First(jf => String.Equals(jf.JobFamilyName, userSession.FilteredAssessmentState.JobFamilyName, StringComparison.InvariantCultureIgnoreCase));

            var results = jobFamily.FilterAssessment.SuggestedJobProfiles;

            var whatYouToldUs = jobFamily.FilterAssessment.WhatYouToldUs;

            Assert.NotEmpty(results);
            Assert.Equal(7, results.Count);
            Assert.True(results.ContainsKey("jp1"));
            Assert.True(results.ContainsKey("jp2"));
            Assert.True(results.ContainsKey("jp3"));
            Assert.True(results.ContainsKey("jp4"));
            Assert.True(results.ContainsKey("jp5"));
            Assert.True(results.ContainsKey("jp6"));
            Assert.True(results.ContainsKey("jp7"));
            Assert.Equal(2, whatYouToldUs.Length);
            Assert.Equal("q1_positive", whatYouToldUs[0]);
            Assert.Equal("q2_positive", whatYouToldUs[1]);
        }

        public static class FilteringResultMockData
        {
            public static string QuestionSetVersion = "TEST";
            public static Question[] FilteringQuestions = new[]
            {
                new Question
                {
                    QuestionId = "q1",
                    FilterTrigger = "No",
                    NegativeResultDisplayText = "q1_negative",
                    PositiveResultDisplayText = "q1_positive",
                    ExcludesJobProfiles = new[]
                    {
                        "profile1",
                        "profile2",
                        "profile3"
                    }
                },
                new Question
                {
                    QuestionId = "q2",
                    FilterTrigger = "No",
                    NegativeResultDisplayText = "q2_negative",
                    PositiveResultDisplayText = "q2_positive",
                    ExcludesJobProfiles = new[]
                    {
                        "profile1",
                        "profile4",
                        "profile7"
                    }
                }
            };
            public static JobProfile[] JobProfiles = new[]
            {
                new JobProfile()
                {
                    Title = "profile1",
                    SocCode = "jp1",
                    JobProfileCategories = new[] { "jobcategory1" }
                },
                new JobProfile()
                {
                    Title = "profile2",
                    SocCode = "jp2",
                    JobProfileCategories = new[] { "jobcategory1" }
                },
                new JobProfile()
                {
                    Title = "profile3",
                    SocCode = "jp3",
                    JobProfileCategories = new[] { "jobcategory1" }
                },
                new JobProfile()
                {
                    Title = "profile4",
                    SocCode = "jp4",
                    JobProfileCategories = new[] { "jobcategory1" }
                },
                new JobProfile()
                {
                    Title = "profile5",
                    SocCode = "jp5",
                    JobProfileCategories = new[] { "jobcategory1" }
                },
                new JobProfile()
                {
                    Title = "profile6",
                    SocCode = "jp6",
                    JobProfileCategories = new[] { "jobcategory1" }
                },
                new JobProfile()
                {
                    Title = "profile7",
                    SocCode = "jp7",
                    JobProfileCategories = new[] { "jobcategory1" }
                }
            };
        }

    }
}

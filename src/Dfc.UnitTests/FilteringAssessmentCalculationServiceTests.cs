using Dfc.DiscoverSkillsAndCareers.AssessmentFunctionApp.Services;
using Dfc.DiscoverSkillsAndCareers.Models;
using Dfc.DiscoverSkillsAndCareers.Repositories;
using Microsoft.Extensions.Logging;
using NSubstitute;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Xunit;

namespace Dfc.UnitTests
{
    public class FilteringAssessmentCalculationServiceTests
    {
        private IQuestionRepository _questionRepository;
        private IJobCategoryRepository _jobCategoryRepository;
        private ILogger _logger;
        private FilterAssessmentCalculationService _sut;

        public FilteringAssessmentCalculationServiceTests()
        {
            _questionRepository = Substitute.For<IQuestionRepository>();
            _jobCategoryRepository = Substitute.For<IJobCategoryRepository>();
            _logger = Substitute.For<ILogger>();

            _sut = new FilterAssessmentCalculationService(_questionRepository, _jobCategoryRepository);
        }


        [Fact]
        public void WhatYouToldUs_Matches_Answers()
        {
            var result = _sut.ComputeWhatYouToldUs(new[]
            {
                new Answer {QuestionId = "q1", SelectedOption = AnswerOption.Yes},
                new Answer {QuestionId = "q2", SelectedOption = AnswerOption.No},
            }, new[]
            {
                new Question
                {
                    QuestionId = "q1", PositiveResultDisplayText = "Positive1", NegativeResultDisplayText = "Negative1"
                },
                new Question
                    {QuestionId = "q2", PositiveResultDisplayText = "Positive2", NegativeResultDisplayText = "Negative2"}
            });
            
            Assert.Collection(result, 
                s => Assert.Equal("Positive1",s), 
                s => Assert.Equal("Negative2",s) );
        }
        
        [Fact]
        public void WhatYouToldUs_SkipsNull()
        {
            var result = _sut.ComputeWhatYouToldUs(new[]
            {
                new Answer {QuestionId = "q1", SelectedOption = AnswerOption.Yes},
                new Answer {QuestionId = "q2", SelectedOption = AnswerOption.No},
            }, new[]
            {
                new Question
                {
                    QuestionId = "q1", PositiveResultDisplayText = "Positive1", NegativeResultDisplayText = "Negative1"
                },
                new Question
                    {QuestionId = "q2", PositiveResultDisplayText = "Positive2", NegativeResultDisplayText = null}
            });
            
            Assert.Collection(result, s => Assert.Equal("Positive1",s));
        }

        [Fact]
        public void WhatYouToldUs_SkipsEmpty()
        {
            var result = _sut.ComputeWhatYouToldUs(new[]
            {
                new Answer {QuestionId = "q1", SelectedOption = AnswerOption.Yes},
                new Answer {QuestionId = "q2", SelectedOption = AnswerOption.No},
            }, new[]
            {
                new Question
                {
                    QuestionId = "q1", PositiveResultDisplayText = "Positive1", NegativeResultDisplayText = "Negative1"
                },
                new Question
                    {QuestionId = "q2", PositiveResultDisplayText = "Positive2", NegativeResultDisplayText = "" }
            });
            
            Assert.Collection(result, s => Assert.Equal("Positive1",s));
        }

        [Theory]
        [InlineData(AnswerOption.Yes, AnswerOption.Yes, AnswerOption.Yes, AnswerOption.Yes, "JP3,JP5")]
        [InlineData(AnswerOption.Yes, AnswerOption.Yes, AnswerOption.No, AnswerOption.No, "JP1")]
        [InlineData(AnswerOption.Yes, AnswerOption.No, AnswerOption.No, AnswerOption.Yes, "JP2")]
        [InlineData(AnswerOption.No, AnswerOption.No, AnswerOption.No, AnswerOption.No, "JP4")]
        [InlineData(AnswerOption.Yes, AnswerOption.No, AnswerOption.No, AnswerOption.No, "")]
        public async Task CalculateAssessement_ShouldReturn_CorrectSelectedJobProfiles(AnswerOption a, AnswerOption b, AnswerOption c, AnswerOption d, string profiles)
        {
            _questionRepository.GetQuestions("qs-1").Returns(Task.FromResult(new []
            {
                new Question { QuestionId = "1", TraitCode = "A" },
                new Question { QuestionId = "2", TraitCode = "B" },
                new Question { QuestionId = "3", TraitCode = "C" },
                new Question { QuestionId = "4", TraitCode = "D" }
            }));
            
            _jobCategoryRepository.GetJobCategory("AC").Returns(Task.FromResult(new JobCategory
            {
                Name = "Animal Care",
                Skills = new List<JobProfileSkillMapping>
                {
                    new JobProfileSkillMapping
                    {
                        ONetAttribute = "A", 
                        JobProfiles = new List<JobProfileMapping>
                        {
                            new JobProfileMapping { JobProfile = "JP1", Included = true },
                            new JobProfileMapping { JobProfile = "JP2", Included = true },
                            new JobProfileMapping { JobProfile = "JP3", Included = true },
                            new JobProfileMapping { JobProfile = "JP4", Included = false },
                            new JobProfileMapping { JobProfile = "JP5", Included = true },
                        }
                    },
                    new JobProfileSkillMapping
                    {
                        ONetAttribute = "B", 
                        JobProfiles = new List<JobProfileMapping>
                        {
                            new JobProfileMapping { JobProfile = "JP1", Included = true },
                            new JobProfileMapping { JobProfile = "JP2", Included = false },
                            new JobProfileMapping { JobProfile = "JP3", Included = true },
                            new JobProfileMapping { JobProfile = "JP4", Included = false },
                            new JobProfileMapping { JobProfile = "JP5", Included = true },
                        }
                    },
                    new JobProfileSkillMapping
                    {
                        ONetAttribute = "C", 
                        JobProfiles = new List<JobProfileMapping>
                        {
                            new JobProfileMapping { JobProfile = "JP1", Included = false },
                            new JobProfileMapping { JobProfile = "JP2", Included = false },
                            new JobProfileMapping { JobProfile = "JP3", Included = true },
                            new JobProfileMapping { JobProfile = "JP4", Included = false },
                            new JobProfileMapping { JobProfile = "JP5", Included = true },
                        }
                    },
                    new JobProfileSkillMapping
                    {
                        ONetAttribute = "D", 
                        JobProfiles = new List<JobProfileMapping>
                        {
                            new JobProfileMapping { JobProfile = "JP1", Included = false },
                            new JobProfileMapping { JobProfile = "JP2", Included = true },
                            new JobProfileMapping { JobProfile = "JP3", Included = true },
                            new JobProfileMapping { JobProfile = "JP4", Included = false },
                            new JobProfileMapping { JobProfile = "JP5", Included = true },
                        }
                    }
                    
                }
            }));

            var categoryResult = new JobCategoryResult {JobCategoryName = "Animal Care"};
            var session = new UserSession
            {
                ResultData = new ResultData
                {
                    JobCategories = new [] { categoryResult }
                },
                AssessmentState = new AssessmentState("q-1", 1)
                {
                    RecordedAnswers = new []
                    {
                        new Answer { QuestionNumber = 1, SelectedOption = AnswerOption.Agree }, 
                    },
                    CurrentQuestion = 1
                    
                },
                FilteredAssessmentState = new FilteredAssessmentState
                {
                    CurrentFilterAssessmentCode = "AC",
                    RecordedAnswers = new[]
                    {
                        new Answer {TraitCode = "A", SelectedOption = a},
                        new Answer {TraitCode = "B", SelectedOption = b},
                        new Answer {TraitCode = "C", SelectedOption = c},
                        new Answer {TraitCode = "D", SelectedOption = d}
                    },
                    JobCategoryStates = new List<JobCategoryState>
                    {
                        new JobCategoryState("AC", "Animal Care", "qs-1", new[]
                        {
                            new JobCategorySkill {QuestionNumber = 1, Skill = "A"},
                            new JobCategorySkill {QuestionNumber = 2, Skill = "B"},
                            new JobCategorySkill {QuestionNumber = 3, Skill = "C"},
                            new JobCategorySkill {QuestionNumber = 4, Skill = "D"},
                        })
                    }
                }
            };

            await _sut.CalculateAssessment(session, _logger);

            if (String.IsNullOrEmpty(profiles))
            {
                Assert.Empty(categoryResult.FilterAssessmentResult.SuggestedJobProfiles);
            }
            else
            {
                foreach (var profile in profiles.Split(",", StringSplitOptions.RemoveEmptyEntries))
                {

                    Assert.Contains(profile, categoryResult.FilterAssessmentResult.SuggestedJobProfiles);
                }
            }
            
        }
    }
}

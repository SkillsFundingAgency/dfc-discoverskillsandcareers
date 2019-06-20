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
    public class FilteringAssessmentCalculationServiceTests
    {
        private IQuestionRepository _questionRepository;
        private IJobCategoryRepository _jobCategoryRepository;
        private FilterAssessmentCalculationService _sut;

        public FilteringAssessmentCalculationServiceTests()
        {
            _questionRepository = Substitute.For<IQuestionRepository>();
            _jobCategoryRepository = Substitute.For<IJobCategoryRepository>();

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
    }
}

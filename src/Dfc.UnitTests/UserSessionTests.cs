using System.Collections.Generic;
using Dfc.DiscoverSkillsAndCareers.Models;
using Microsoft.Azure.Documents;
using Xunit;

namespace Dfc.UnitTests
{
    public class UserSessionTests
    {
        [Fact]
        public void CanAddAnAnswer_ToAssessmentState()
        {
            var userSession = new UserSession
            {
                AssessmentState = new AssessmentState("qs-1", 5),
                FilteredAssessmentState = new FilteredAssessmentState()
            };
            
            userSession.AddAnswer(AnswerOption.Agree, new Question
            {
                QuestionId = "1", 
                Order = 1, 
                IsFilterQuestion = false, 
                Texts = new [] { new QuestionText { LanguageCode = "en", Text = "Question 1"} }
            });

            Assert.Single(userSession.AssessmentState.RecordedAnswers, a => a.QuestionId == "1" && a.SelectedOption == AnswerOption.Agree);
            Assert.Empty(userSession.FilteredAssessmentState.RecordedAnswers);
        }
        
        [Fact]
        public void CanAddAnAnswer_ToFilteredAssessmentState()
        {
            var userSession = new UserSession
            {
                AssessmentState = new AssessmentState("qs-1", 5),
                FilteredAssessmentState = new FilteredAssessmentState()
            };
            
            userSession.AddAnswer(AnswerOption.Yes, new Question
            {
                QuestionId = "1", 
                Order = 1, 
                IsFilterQuestion = true, 
                Texts = new [] { new QuestionText { LanguageCode = "en", Text = "Question 1"} }
            });

            Assert.Single(userSession.FilteredAssessmentState.RecordedAnswers, a => a.QuestionId == "1" && a.SelectedOption == AnswerOption.Yes);
            Assert.Empty(userSession.AssessmentState.RecordedAnswers);
        }
        
        [Fact]
        public void ManageIfComplete_WithCompleteState_ShouldBeComplete()
        {
            var userSession = new UserSession()
            {
                AssessmentState = new AssessmentState("qs-1",5) {
                    CurrentQuestion = 5,
                    RecordedAnswers = new []
                    {
                        new Answer() { QuestionNumber = 1 },
                        new Answer() { QuestionNumber = 2 },
                        new Answer() { QuestionNumber = 3 },
                        new Answer() { QuestionNumber = 4 },
                        new Answer() { QuestionNumber = 5 },
                    }
                },
                FilteredAssessmentState = new FilteredAssessmentState()
                {
                    RecordedAnswers = new []
                    {
                        new Answer { TraitCode = "A" }, 
                    },
                    JobCategoryStates = new List<JobCategoryState>
                    {
                        new JobCategoryState("CAT", "Construction and Trades", "QS-1", new []
                        {
                            new JobCategorySkill { Skill = "A", QuestionId = "1"}, 
                        })
                    },
                    CurrentFilterAssessmentCode = "CAT"
                }
            };
            
            Assert.True(userSession.IsComplete);
            Assert.True(userSession.AssessmentState.CompleteDt.HasValue);
        }

        [Fact]
        public void UpdateJobCategoryQuestionCount_UpdatesJobCategoryState()
        {
            var jc = new JobCategoryResult {JobCategoryName = "Animal Care", TotalQuestions = 3 };
            var sut = new UserSession
            {
                ResultData = new ResultData
                {
                  JobCategories  = new [] { jc}
                },
                FilteredAssessmentState = new FilteredAssessmentState
                {
                    RecordedAnswers = new []
                    {
                        new Answer { TraitCode = "A" },
                        new Answer { TraitCode = "B" },
                    },
                    JobCategoryStates = new List<JobCategoryState>
                    {
                        new JobCategoryState("AC", "Animal Care", "QS-1", new[]
                            {
                                new JobCategorySkill {Skill = "A", QuestionNumber = 1, QuestionId = "11" },
                                new JobCategorySkill {Skill = "B", QuestionNumber = 2, QuestionId = "12" },
                                new JobCategorySkill {Skill = "C", QuestionNumber = 3, QuestionId = "23" }
                            })
                    }
                }
            };
            
            sut.UpdateJobCategoryQuestionCount();
            
            Assert.Equal(1, jc.TotalQuestions);
        }
        
        [Fact]
        public void UpdateJobCategoryQuestionCount_DoesNotUpdateJobCategoryStateIfNoFilteredState()
        {
            var jc = new JobCategoryResult {JobCategoryName = "Animal Care", TotalQuestions = 3 };
            var sut = new UserSession
            {
                ResultData = new ResultData
                {
                    JobCategories  = new [] { jc}
                },
                FilteredAssessmentState = new FilteredAssessmentState
                {
                    RecordedAnswers = new []
                    {
                        new Answer { TraitCode = "A" },
                        new Answer { TraitCode = "B" },
                    },
                    JobCategoryStates = {}
                }
            };
            
            sut.UpdateJobCategoryQuestionCount();
            
            Assert.Equal(3, jc.TotalQuestions);
        }
    }
}
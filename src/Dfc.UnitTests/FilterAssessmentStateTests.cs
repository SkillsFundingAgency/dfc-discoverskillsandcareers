using System;
using System.Collections.Generic;
using Dfc.DiscoverSkillsAndCareers.Models;
using Xunit;

namespace Dfc.UnitTests
{
    public class FilterAssessmentStateTests
    {
        [Fact]
        public void RemoveAnswers_ForAJobCategory()
        {
            var sut = new FilteredAssessmentState
            {
                RecordedAnswers = new[]
                {
                    new Answer {TraitCode = "D"},
                    new Answer {TraitCode = "B"},
                    new Answer {TraitCode = "C"},
                    new Answer {TraitCode = "E"},
                    new Answer {TraitCode = "A"}
                },
                JobCategoryStates = new List<JobCategoryState>
                {
                    new JobCategoryState("GS", "Government Services", "QS-1", new[]
                    {
                        new JobCategorySkill {Skill = "A"},
                        new JobCategorySkill {Skill = "B"},
                        new JobCategorySkill {Skill = "C"}
                    })
                }
            };
            
            
            sut.RemoveAnswersForCategory("GS");
            
            Assert.DoesNotContain(sut.RecordedAnswers, a => a.TraitCode == "A" || a.TraitCode == "B" || a.TraitCode == "C");
        }
        
        [Fact]
        public void RemoveAnswers_ForAJobCategory_IfNoCategories()
        {
            var sut = new FilteredAssessmentState
            {
                RecordedAnswers = new []
                {
                    new Answer { TraitCode = "D" },
                    new Answer { TraitCode = "B" },
                    new Answer { TraitCode = "C" },
                    new Answer { TraitCode = "E" },
                    new Answer { TraitCode = "A" }
                },
                JobCategoryStates = {}
            };
            
            sut.RemoveAnswersForCategory("GS");
            
            Assert.Equal(sut.RecordedAnswers, sut.RecordedAnswers);
        }

        [Fact]
        public void MoveToNextQuestion_Throws_IfNoCurrentCode()
        {
            var sut = new FilteredAssessmentState
            {
                RecordedAnswers = new[]
                {
                    new Answer {TraitCode = "A", QuestionNumber = 1},
                    new Answer {TraitCode = "B", QuestionNumber = 2},
                },
                JobCategoryStates = new List<JobCategoryState>
                {
                    new JobCategoryState("GS", "Government Services", "QS-1", new[]
                    {
                        new JobCategorySkill {Skill = "A"},
                        new JobCategorySkill {Skill = "B"},
                        new JobCategorySkill {Skill = "C"}
                    })
                }
            };
            
            Assert.Throws<InvalidOperationException>(() => sut.MoveToNextQuestion());
            
        }
        
        [Fact]
        public void MoveToNextQuestion_Returns_NextQuestionIfNotComplete()
        {
            var sut = new FilteredAssessmentState
            {
                CurrentFilterAssessmentCode = "GS",
                RecordedAnswers = new[]
                {
                    new Answer {TraitCode = "A", QuestionNumber = 1},
                    new Answer {TraitCode = "B", QuestionNumber = 2},
                },
                JobCategoryStates = new List<JobCategoryState>
                {
                    new JobCategoryState("GS", "Government Service", "QS-1", new[]
                    {
                        new JobCategorySkill {Skill = "A", QuestionNumber = 1},
                        new JobCategorySkill {Skill = "B", QuestionNumber = 2},
                        new JobCategorySkill {Skill = "C", QuestionNumber = 3}
                    })
                }
            };
            

            var result = sut.MoveToNextQuestion();
            
            Assert.Equal(3, result);
        }
        
        [Fact]
        public void MoveToNextQuestion_Returns_FirstQuestionIfComplete()
        {
            var sut = new FilteredAssessmentState
            {
                CurrentFilterAssessmentCode = "GS",
                RecordedAnswers = new[]
                {
                    new Answer {TraitCode = "A", QuestionNumber = 1},
                    new Answer {TraitCode = "B", QuestionNumber = 2},
                    new Answer {TraitCode = "C", QuestionNumber = 3},
                },
                JobCategoryStates = new List<JobCategoryState>
                {
                    new JobCategoryState("GS", "Government Services", "QS-1", new[]
                    {
                        new JobCategorySkill {Skill = "A", QuestionNumber = 1},
                        new JobCategorySkill {Skill = "B", QuestionNumber = 2},
                        new JobCategorySkill {Skill = "C", QuestionNumber = 3}
                    })
                }
            };
            

            var result = sut.MoveToNextQuestion();
            
            Assert.Equal(1, result);
        }

        [Fact]
        public void CreateOrResetCategory_CreatesCategoryIfNotExist()
        {
            var questions = new[]
            {
                new Question {Order = 1, TraitCode = "A"},
                new Question {Order = 2, TraitCode = "B"},
                new Question {Order = 3, TraitCode = "C"},
                new Question {Order = 4, TraitCode = "D"}
            };
            
            var sut = new FilteredAssessmentState
            {
                CurrentFilterAssessmentCode = "AC"
            };
            
            sut.CreateOrResetCategoryState("QS-1", questions, new JobCategory
            {
                Name = "Animal Care", 
                Skills = new List<JobProfileSkillMapping>
                {
                    new JobProfileSkillMapping { ONetAttribute = "D"},
                    new JobProfileSkillMapping { ONetAttribute = "A"}
                }
            });

            Assert.Contains(sut.JobCategoryStates, jc => jc.JobCategoryCode == "AC");
            Assert.Equal("QS-1", sut.QuestionSetVersion);
            Assert.Equal(4, sut.CurrentQuestion);
        }
        
        [Fact]
        public void CreateOrResetCategory_OnlyUpdatesQuestionNumber()
        {
            var questions = new[]
            {
                new Question {Order = 1, TraitCode = "A"},
                new Question {Order = 2, TraitCode = "B"},
                new Question {Order = 3, TraitCode = "C"},
                new Question {Order = 4, TraitCode = "D"}
            };
            
            var sut = new FilteredAssessmentState
            {
                CurrentFilterAssessmentCode = "AC",
                JobCategoryStates = new List<JobCategoryState>
                {
                    new JobCategoryState("AC", "Animal Care", "QS-1",  new []
                    {
                        new JobCategorySkill { Skill = "D", QuestionNumber = 4, QuestionId = "4"},
                        new JobCategorySkill { Skill = "A", QuestionNumber = 1, QuestionId = "1"}
                    }, "1")
                }
            };
            
            sut.CreateOrResetCategoryState("QS-1", questions, new JobCategory
            {
                Name = "Animal Care", 
                Skills = new List<JobProfileSkillMapping>
                {
                    new JobProfileSkillMapping { ONetAttribute = "D"},
                    new JobProfileSkillMapping { ONetAttribute = "A"}
                }
            });

            Assert.Contains(sut.JobCategoryStates, jc => jc.JobCategoryCode == "AC");
            Assert.Equal("QS-1", sut.QuestionSetVersion);
            Assert.Equal(4, sut.CurrentQuestion);
        }

        [Fact]
        public void TryGetCategoryState_ReturnsFalse_IfNoCategory()
        {
            var sut = new FilteredAssessmentState();
            
            Assert.False(sut.TryGetJobCategoryState("GS", out var _));
        }
        
        
        [Fact]
        public void TryGetCategoryState_ReturnsTrue_IfCategory()
        {
            var sut = new FilteredAssessmentState
            {
                CurrentFilterAssessmentCode = "AC",
                JobCategoryStates = new List<JobCategoryState>
                {
                    new JobCategoryState("AC", "Animal Care", "QS-1", new []
                    {
                        new JobCategorySkill { Skill = "D", QuestionNumber = 4, QuestionId = "4"},
                        new JobCategorySkill { Skill = "A", QuestionNumber = 1, QuestionId = "1"}
                    }, "1")
                }
            };
            
            Assert.True(sut.TryGetJobCategoryState("AC", out var cat));
            Assert.NotNull(cat);
            Assert.Equal("QS-1",cat.QuestionSetVersion);
        }

        [Fact]
        public void GetAnswersForCategory_ReturnsEmptyArray_IfNoCategory()
        {
            var sut = new FilteredAssessmentState();

            Assert.Empty(sut.GetAnswersForCategory("AC"));
        }
        
        [Fact]
        public void GetAnswersForCategory_ReturnsArray_IfNoCategory()
        {
            var sut = new FilteredAssessmentState
            {
                CurrentFilterAssessmentCode = "AC",
                RecordedAnswers = new []
                {
                    new Answer { TraitCode = "A", QuestionNumber = 1 },
                    new Answer { TraitCode = "B", QuestionNumber = 2 },
                    new Answer { TraitCode = "C", QuestionNumber = 3 },
                },
                JobCategoryStates = new List<JobCategoryState>
                {
                    new JobCategoryState("AC", "Animal Care", "QS-1", new []
                    {
                        new JobCategorySkill { Skill = "D", QuestionNumber = 4, QuestionId = "4"},
                        new JobCategorySkill { Skill = "A", QuestionNumber = 1, QuestionId = "1"}
                    }, "1")
                }
            };

            Assert.Single(sut.GetAnswersForCategory("AC"), a => a.TraitCode == "A");
        }
    }
}
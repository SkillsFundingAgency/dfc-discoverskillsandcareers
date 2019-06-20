using Dfc.DiscoverSkillsAndCareers.Models;
using Xunit;

namespace Dfc.UnitTests
{
    public class JobCategoryStateTests
    {
        [Fact]
        public void IsComplete_ReturnsTrue_IfAllSkillsHaveMatchingAnswers()
        {
            var sut = new JobCategoryState
            {
                Skills = new[]
                {
                    new JobCategorySkill {Skill = "A"},
                    new JobCategorySkill {Skill = "B"},
                    new JobCategorySkill {Skill = "C"}
                }
            };
            
            Assert.True(sut.IsComplete(new []
            {
                new Answer { TraitCode =  "A"},
                new Answer { TraitCode =  "B"},
                new Answer { TraitCode =  "C"}
            }));
        }
        
        [Fact]
        public void IsComplete_ReturnsFalse_IfNotAllSkillsHaveMatchingAnswers()
        {
            var sut = new JobCategoryState
            {
                Skills = new[]
                {
                    new JobCategorySkill {Skill = "A"},
                    new JobCategorySkill {Skill = "B"},
                    new JobCategorySkill {Skill = "C"}
                }
            };
            
            Assert.False(sut.IsComplete(new []
            {
                new Answer { TraitCode =  "A"},
            }));
        }
        
        [Fact]
        public void IsComplete_ReturnsFalse_IfAnswersNull()
        {
            var sut = new JobCategoryState
            {
                Skills = new[]
                {
                    new JobCategorySkill {Skill = "A"},
                    new JobCategorySkill {Skill = "B"},
                    new JobCategorySkill {Skill = "C"}
                }
            };
            
            Assert.False(sut.IsComplete(null));
        }
        
        [Fact]
        public void UnansweredQuestions_Returns3()
        {
            var sut = new JobCategoryState
            {
                Skills = new[]
                {
                    new JobCategorySkill {Skill = "A"},
                    new JobCategorySkill {Skill = "B"},
                    new JobCategorySkill {Skill = "C"}
                }
            };
            
            Assert.Equal(3,sut.UnansweredQuestions(new Answer[] {}));
        }
        
        [Fact]
        public void UnansweredQuestions_Returns3_IfAnswersNull()
        {
            var sut = new JobCategoryState
            {
                Skills = new[]
                {
                    new JobCategorySkill {Skill = "A"},
                    new JobCategorySkill {Skill = "B"},
                    new JobCategorySkill {Skill = "C"}
                }
            };
            
            Assert.Equal(3,sut.UnansweredQuestions(new Answer[] {}));
        }
        
        [Fact]
        public void UnansweredQuestions_Returns2()
        {
            var sut = new JobCategoryState
            {
                Skills = new[]
                {
                    new JobCategorySkill {Skill = "A"},
                    new JobCategorySkill {Skill = "B"},
                    new JobCategorySkill {Skill = "C"}
                }
            };
            
            Assert.Equal(2,sut.UnansweredQuestions(new []
            {
                new Answer { TraitCode =  "A"}
            }));
        }
        
        [Fact]
        public void UnansweredQuestions_Returns0()
        {
            var sut = new JobCategoryState
            {
                Skills = new[]
                {
                    new JobCategorySkill {Skill = "A"},
                    new JobCategorySkill {Skill = "B"},
                    new JobCategorySkill {Skill = "C"}
                }
            };
            
            Assert.Equal(0,sut.UnansweredQuestions(new []
            {
                new Answer { TraitCode = "A" },
                new Answer { TraitCode = "B" },
                new Answer { TraitCode = "C" }
            }));
        }
    }
}
using Dfc.DiscoverSkillsAndCareers.Models;
using Xunit;

namespace Dfc.UnitTests
{
    public class JobCategoryStateTests
    {
        private JobCategoryState sut;

        public JobCategoryStateTests()
        {
            sut = new JobCategoryState("AC", "Animal Care", "QS-1", new[]
                {
                    new JobCategorySkill {Skill = "A", QuestionNumber = 1, QuestionId = "1" },
                    new JobCategorySkill {Skill = "B", QuestionNumber = 2, QuestionId = "2" },
                    new JobCategorySkill {Skill = "C", QuestionNumber = 3, QuestionId = "3" }
                }
            );
        }
        
        [Fact]
        public void IsComplete_ReturnsTrue_IfAllSkillsHaveMatchingAnswers()
        {
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
            
            Assert.False(sut.IsComplete(new []
            {
                new Answer { TraitCode =  "A"},
            }));
        }
        
        [Fact]
        public void IsComplete_ReturnsFalse_IfAnswersNull()
        {
            Assert.False(sut.IsComplete(null));
        }
        
        [Fact]
        public void UnansweredQuestions_Returns3()
        {
            Assert.Equal(3,sut.UnansweredQuestions(new Answer[] {}));
        }
        
        [Fact]
        public void UnansweredQuestions_Returns3_IfAnswersNull()
        {
            Assert.Equal(3,sut.UnansweredQuestions(new Answer[] {}));
        }
        
        [Fact]
        public void UnansweredQuestions_Returns2()
        {
            Assert.Equal(2,sut.UnansweredQuestions(new []
            {
                new Answer { TraitCode =  "A"}
            }));
        }
        
        [Fact]
        public void UnansweredQuestions_Returns0()
        {
            Assert.Equal(0,sut.UnansweredQuestions(new []
            {
                new Answer { TraitCode = "A" },
                new Answer { TraitCode = "B" },
                new Answer { TraitCode = "C" }
            }));
        }
    }
}
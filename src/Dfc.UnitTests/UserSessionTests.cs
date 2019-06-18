using System.Collections.Generic;
using Dfc.DiscoverSkillsAndCareers.Models;
using Xunit;

namespace Dfc.UnitTests
{
    public class UserSessionTests
    {
        [Fact]
        public void ManageIfComplete_WithCompleteState_ShouldBeComplete()
        {
            var userSession = new UserSession()
            {
                AssessmentState = new AssessmentState("qs-1",5) {
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
                    JobCategoryStates = new List<JobCategoryState>
                    {
                        new JobCategoryState{ JobCategoryCode = "CAT", Skills = new JobCategorySkill[]{}}
                    },
                    CurrentFilterAssessmentCode = "CAT"
                }
            };

            for(var i = 0; i < 5; i++)
            {
                userSession.AssessmentState.MoveToNextQuestion();
            }
            
            Assert.True(userSession.IsComplete);
            Assert.True(userSession.AssessmentState.CompleteDt.HasValue);
        }
    }
}
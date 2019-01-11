using System;
using Xunit;
using Dfc.DiscoverSkillsAndCareers.Models;
using Dfc.DiscoverSkillsAndCareers.FunctionApp.QuestionRouter;

namespace Dfc.UnitTests
{
    public class QuestionRouterTests
    {
        [Fact]
        public void ManageIfComplete_WithNotCompleteState_ShouldNotBeComplete()
        {
            var userSession = new UserSession()
            {
                MaxQuestions = 5,
                CurrentQuestion = 1
            };

            QuestionRouterFunction.ManageIfComplete(userSession);

            Assert.False(userSession.IsComplete);
            Assert.Null(userSession.CompleteDt);
        }

        [Fact]
        public void ManageIfComplete_WithCompleteState_ShouldBeComplete()
        {
            var userSession = new UserSession()
            {
                MaxQuestions = 5,
                CurrentQuestion = 5
            };

            QuestionRouterFunction.ManageIfComplete(userSession);

            Assert.True(userSession.IsComplete);
            Assert.NotNull(userSession.CompleteDt);
        }

        [Theory]
        [InlineData(1, false, "/q/2")]
        [InlineData(2, false, "/q/3")]
        [InlineData(3, false, "/q/4")]
        [InlineData(4, false, "/q/5")]
        [InlineData(5, true, "/results")]
        [InlineData(6, true, "/results")]
        public void GetNextRoute_WithSetup_ShouldHaveCorrectRoute(int question, bool isComplete, string expected)
        {
            var userSession = new UserSession()
            {
                MaxQuestions = 5,
                CurrentQuestion = question,
                IsComplete = isComplete,
            };

            string actual = BuildPageHtml.GetNextRoute(userSession);

            Assert.Equal(expected, actual);
        }
    }
}

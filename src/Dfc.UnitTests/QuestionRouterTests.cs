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
                CurrentQuestion = 5,
                RecordedAnswers = new System.Collections.Generic.List<Answer>()
                {
                    new Answer() { QuestionNumber = "1" },
                    new Answer() { QuestionNumber = "2" },
                    new Answer() { QuestionNumber = "3" },
                    new Answer() { QuestionNumber = "4" },
                    new Answer() { QuestionNumber = "5" },
                }
            };

            QuestionRouterFunction.ManageIfComplete(userSession);

            Assert.True(userSession.IsComplete);
            Assert.NotNull(userSession.CompleteDt);
        }

        [Fact]
        public void ManageIfComplete_WithMissingAnswers_ShouldNotBeComplete()
        {
            var userSession = new UserSession()
            {
                MaxQuestions = 5,
                CurrentQuestion = 5,
                RecordedAnswers = new System.Collections.Generic.List<Answer>()
                {
                    new Answer() { QuestionNumber = "1" },
                    new Answer() { QuestionNumber = "2" },
                    new Answer() { QuestionNumber = "5" },
                }
            };

            QuestionRouterFunction.ManageIfComplete(userSession);

            Assert.False(userSession.IsComplete);
            Assert.Null(userSession.CompleteDt);
        }

        [Theory]
        [InlineData(1, false, "/q/2")]
        [InlineData(2, false, "/q/3")]
        [InlineData(3, false, "/q/4")]
        [InlineData(4, false, "/q/5")]
        [InlineData(5, true, "/finish")]
        [InlineData(6, true, "/finish")]
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

        [Fact]
        public void GetNextRoute_WithRecordedAnswerLess1_ShouldBeResultsNextRoute()
        {
            var userSession = new UserSession()
            {
                MaxQuestions = 5,
                CurrentQuestion = 5,
                IsComplete = false,
                RecordedAnswers = new System.Collections.Generic.List<Answer>()
                {
                    new Answer() { QuestionNumber = "1" },
                    new Answer() { QuestionNumber = "2" },
                    new Answer() { QuestionNumber = "3" },
                    new Answer() { QuestionNumber = "4" }
                },
            };

            string actual = BuildPageHtml.GetNextRoute(userSession);

            Assert.Equal("/finish", actual);
        }

        [Fact]
        public void GetNextRoute_WithRecordedAnswersLess2_ShouldBeQuestionNextRoute()
        {
            var userSession = new UserSession()
            {
                MaxQuestions = 5,
                CurrentQuestion = 4,
                IsComplete = false,
                RecordedAnswers = new System.Collections.Generic.List<Answer>()
                {
                    new Answer() { QuestionNumber = "1" },
                    new Answer() { QuestionNumber = "2" },
                    new Answer() { QuestionNumber = "3" }
                },
            };

            string actual = BuildPageHtml.GetNextRoute(userSession);

            Assert.Equal("/q/5", actual);
        }

        [Theory]
        [InlineData(0, 40, 1)]
        [InlineData(1, 40, 1)]
        [InlineData(5, 40, 5)]
        [InlineData(40, 40, 40)]
        [InlineData(42, 40, 40)]
        public void GetCurrentQuestionNumber_WithTheory_ShouldHaveExpected(int question, int max, int expected)
        {
            int actual = QuestionRouterFunction.GetCurrentQuestionNumber(question, max);

            Assert.Equal(expected, actual);
        }
    }
}

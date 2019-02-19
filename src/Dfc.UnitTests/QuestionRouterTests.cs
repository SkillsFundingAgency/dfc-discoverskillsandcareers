using Dfc.DiscoverSkillsAndCareers.AssessmentFunctionApp;
using Dfc.DiscoverSkillsAndCareers.Models;
using Xunit;

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

            PostAnswerHttpTrigger.ManageIfComplete(userSession);

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

            PostAnswerHttpTrigger.ManageIfComplete(userSession);

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

            PostAnswerHttpTrigger.ManageIfComplete(userSession);

            Assert.False(userSession.IsComplete);
            Assert.Null(userSession.CompleteDt);
        }

        [Fact]
        public void GetNextQuestionNumber_WithNoAnswers_ShouldBeFirstQuestion()
        {
            var userSession = new UserSession()
            {
                MaxQuestions = 5,
                CurrentQuestion = 5,
                IsComplete = false,
                RecordedAnswers = new System.Collections.Generic.List<Answer>()
                {
                },
            };

            int actual = PostAnswerHttpTrigger.GetNextQuestionToAnswerNumber(userSession);

            Assert.Equal(1, actual);
        }

        [Fact]
        public void GetNextQuestionToAnswerNumber_WithGapInAnswers_ShouldBeQuestion3()
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

            var actual = PostAnswerHttpTrigger.GetNextQuestionToAnswerNumber(userSession);

            Assert.Equal(3, actual);
        }

        [Fact]
        public void GetNextQuestionNumber_WithRecordedAnswerLess1_ShouldBeLastQuestion()
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

            int actual = PostAnswerHttpTrigger.GetNextQuestionToAnswerNumber(userSession);

            Assert.Equal(5, actual);
        }

        [Theory]
        [InlineData(0, 40, 1)]
        [InlineData(1, 40, 1)]
        [InlineData(5, 40, 5)]
        [InlineData(40, 40, 40)]
        [InlineData(42, 40, 40)]
        public void GetNextQuestionNumber_WithTheory_ShouldHaveExpected(int question, int max, int expected)
        {
            int actual = NextQuestionHttpTrigger.GetNextQuestionNumber(question, max);

            Assert.Equal(expected, actual);
        }
    }
}

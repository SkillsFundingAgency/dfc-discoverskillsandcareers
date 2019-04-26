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
                AssessmentState = new AssessmentState {
                    MaxQuestions = 5,
                    CurrentQuestion = 1
                }
            };

            userSession.UpdateCompletionStatus();

            Assert.False(userSession.IsComplete);
            Assert.Null(userSession.CompleteDt);
        }

        [Fact]
        public void ManageIfComplete_WithCompleteState_ShouldBeComplete()
        {
            var userSession = new UserSession()
            {
                AssessmentState = new AssessmentState {
                    MaxQuestions = 5,
                    CurrentQuestion = 5,
                    RecordedAnswers = new []
                    {
                        new Answer() { QuestionNumber = "1" },
                        new Answer() { QuestionNumber = "2" },
                        new Answer() { QuestionNumber = "3" },
                        new Answer() { QuestionNumber = "4" },
                        new Answer() { QuestionNumber = "5" },
                    }
                }
            };

            userSession.UpdateCompletionStatus();

            Assert.True(userSession.IsComplete);
            Assert.NotNull(userSession.CompleteDt);
        }

        [Fact]
        public void ManageIfComplete_WithMissingAnswers_ShouldNotBeComplete()
        {
            var userSession = new UserSession()
            {
                AssessmentState = new AssessmentState {
                    MaxQuestions = 5,
                    CurrentQuestion = 5,
                    RecordedAnswers = new []
                    {
                        new Answer() { QuestionNumber = "1" },
                        new Answer() { QuestionNumber = "2" },
                        new Answer() { QuestionNumber = "5" },
                    }
                }
            };

            userSession.UpdateCompletionStatus();

            Assert.False(userSession.IsComplete);
            Assert.Null(userSession.CompleteDt);
        }

        [Fact]
        public void GetNextQuestionNumber_WithNoAnswers_ShouldBeFirstQuestion()
        {
            var userSession = new UserSession()
            {
                AssessmentState = new AssessmentState {
                    MaxQuestions = 5,
                    CurrentQuestion = 5,
                    IsComplete = false,
                    RecordedAnswers = {}
                }
            };

            userSession.MoveToNextQuestion();

            Assert.Equal(1, userSession.CurrentQuestion);
        }

        [Fact]
        public void GetNextQuestionToAnswerNumber_WithGapInAnswers_ShouldBeQuestion3()
        {
            var userSession = new UserSession()
            {
                AssessmentState = new AssessmentState {
                    MaxQuestions = 5,
                    CurrentQuestion = 5,
                    RecordedAnswers = new []
                    {
                        new Answer() { QuestionNumber = "1" },
                        new Answer() { QuestionNumber = "2" },
                        new Answer() { QuestionNumber = "5" },
                    }
                }
            };

            userSession.MoveToNextQuestion();

            Assert.Equal(3, userSession.CurrentQuestion);
        }

        [Fact]
        public void GetNextQuestionNumber_WithRecordedAnswerLess1_ShouldBeLastQuestion()
        {
            var userSession = new UserSession()
            {
                AssessmentState = new AssessmentState {
                    MaxQuestions = 5,
                    CurrentQuestion = 5,
                    IsComplete = false,
                    RecordedAnswers = new []
                    {
                        new Answer() { QuestionNumber = "1" },
                        new Answer() { QuestionNumber = "2" },
                        new Answer() { QuestionNumber = "3" },
                        new Answer() { QuestionNumber = "4" }
                    }
                }
            };

            userSession.MoveToNextQuestion();

            Assert.Equal(5, userSession.CurrentQuestion);
        }

        [Theory]
        [InlineData(0, 40, 1)]
        [InlineData(1, 40, 2)]
        [InlineData(5, 40, 6)]
        [InlineData(40, 40, null)]
        [InlineData(42, 40, null)]
        public void GetNextQuestionNumber_WithTheory_ShouldHaveExpected(int question, int max, int? expected)
        {
            int? actual = NextQuestionHttpTrigger.GetNextQuestionNumber(question, max);

            Assert.Equal(expected, actual);
        }
    }
}

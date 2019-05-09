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
                        new Answer() { QuestionNumber = 1 },
                        new Answer() { QuestionNumber = 2 },
                        new Answer() { QuestionNumber = 3 },
                        new Answer() { QuestionNumber = 4 },
                        new Answer() { QuestionNumber = 5 },
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
                        new Answer() { QuestionNumber = 1 },
                        new Answer() { QuestionNumber = 2 },
                        new Answer() { QuestionNumber = 5 },
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
                    RecordedAnswers = {}
                }
            };

            var question = userSession.FindNextUnansweredQuestion();

            Assert.Equal(1, question);
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
                        new Answer() { QuestionNumber = 1 },
                        new Answer() { QuestionNumber = 2 },
                        new Answer() { QuestionNumber = 5 },
                    }
                }
            };

            var question = userSession.FindNextUnansweredQuestion();

            Assert.Equal(3, question);
        }

        [Fact]
        public void GetNextQuestionNumber_WithRecordedAnswerLess1_ShouldBeLastQuestion()
        {
            var userSession = new UserSession()
            {
                AssessmentState = new AssessmentState {
                    MaxQuestions = 5,
                    CurrentQuestion = 5,
                    RecordedAnswers = new []
                    {
                        new Answer() { QuestionNumber = 1 },
                        new Answer() { QuestionNumber = 2 },
                        new Answer() { QuestionNumber = 3 },
                        new Answer() { QuestionNumber = 4 }
                    }
                }
            };

            var question = userSession.FindNextUnansweredQuestion();

            Assert.Equal(5, question);
        }

        [Theory]
        [InlineData(1, "01")]
        [InlineData(2, "02")]
        [InlineData(3, "03")]
        [InlineData(4, "04")]
        [InlineData(5, "05")]
        [InlineData(6, "06")]
        [InlineData(7, "07")]
        [InlineData(8, "08")]
        [InlineData(9, "09")]
        [InlineData(10, "10")]
        [InlineData(20, "20")]
        public void GetQuestionPageNumber_WithTheory_ShouldZeroPrefixLess10(int input, string expected)
        {
            var actual = Dfc.DiscoverSkillsAndCareers.WebApp.Controllers.QuestionController.GetQuestionPageNumber(input);

            Assert.Equal(expected, actual);
        }
    }
}

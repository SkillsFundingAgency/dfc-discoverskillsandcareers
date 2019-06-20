using Dfc.DiscoverSkillsAndCareers.Models;
using Dfc.DiscoverSkillsAndCareers.WebApp.Models;
using Xunit;

namespace Dfc.UnitTests
{
    public class AssessmentStateTests
    {
        [Fact]
        public void ManageIfComplete_WithNotCompleteState_ShouldNotBeComplete()
        {
            var assessmentState = new AssessmentState("QS-1", 5);
            
            Assert.False(assessmentState.IsComplete);
            Assert.False(assessmentState.CompleteDt.HasValue);
        }

        [Fact]
        public void ManageIfComplete_WithCompleteState_ShouldBeComplete()
        {
            var assessmentState = new AssessmentState("QS-1", 5) {
                    RecordedAnswers = new []
                    {
                        new Answer() { QuestionNumber = 1 },
                        new Answer() { QuestionNumber = 2 },
                        new Answer() { QuestionNumber = 3 },
                        new Answer() { QuestionNumber = 4 },
                        new Answer() { QuestionNumber = 5 },
                    }
                };

            assessmentState.SetCurrentQuestion(5);

            Assert.True(assessmentState.IsComplete);
            Assert.True(assessmentState.CompleteDt.HasValue);
        }

        [Fact]
        public void ManageIfComplete_WithMissingAnswers_ShouldNotBeComplete()
        {
            var assessmentState = new AssessmentState("QS-1", 5){
                    RecordedAnswers = new []
                    {
                        new Answer() { QuestionNumber = 1 },
                        new Answer() { QuestionNumber = 2 },
                        new Answer() { QuestionNumber = 5 },
                    }
                };
            

            Assert.False(assessmentState.IsComplete);
            Assert.False(assessmentState.CompleteDt.HasValue);
        }

        [Fact]
        public void GetNextQuestionNumber_WithNoAnswers_ShouldBeFirstQuestion()
        {
            var assessmentState = new AssessmentState("QA-1", 5){
                    RecordedAnswers = {}
                };

            var question = assessmentState.MoveToNextQuestion();

            Assert.Equal(1, question);
        }

        [Fact]
        public void GetNextQuestionToAnswerNumber_WithGapInAnswers_ShouldBeQuestion3()
        {
            var assessmentState = new AssessmentState("QS-1", 5) {
                    RecordedAnswers = new []
                    {
                        new Answer() { QuestionNumber = 1 },
                        new Answer() { QuestionNumber = 2 },
                        new Answer() { QuestionNumber = 5 },
                    }
                };
            
            assessmentState.SetCurrentQuestion(2);
            var question = assessmentState.MoveToNextQuestion();

            Assert.Equal(3, question);
        }

        [Fact]
        public void GetNextQuestionNumber_WithRecordedAnswerLess1_ShouldBeLastQuestion()
        {
            var assessmentState = new AssessmentState("QS-1", 5) {
                RecordedAnswers = new []
                    {
                        new Answer() { QuestionNumber = 1 },
                        new Answer() { QuestionNumber = 2 },
                        new Answer() { QuestionNumber = 3 },
                        new Answer() { QuestionNumber = 4 }
                    }
                };

            assessmentState.SetCurrentQuestion(4);
            
            var question = assessmentState.MoveToNextQuestion();

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
            var actual = input.ToQuestionPageNumber();
            Assert.Equal(expected, actual);
        }
    }
}
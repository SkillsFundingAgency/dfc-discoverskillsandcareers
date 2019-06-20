using System;
using System.Linq;
using Newtonsoft.Json;

namespace Dfc.DiscoverSkillsAndCareers.Models
{
    public class AssessmentState : AssessmentStateBase
    {
        public AssessmentState(string questionSetVersion, int maxQuestions)
        {
            QuestionSetVersion = questionSetVersion;
            MaxQuestions = maxQuestions;
        }

        [JsonProperty("questionSetVersion")]
        public override string QuestionSetVersion { get; }

        [JsonProperty("currentQuestion")] 
        public override int CurrentQuestion { get; protected set; } = 1;
        
        [JsonProperty("maxQuestions")]
        public override int MaxQuestions { get; }
        
        [JsonIgnore]
        public override bool IsComplete
        {
            get
            {
                var complete = RecordedAnswers.Length == MaxQuestions && CurrentQuestion == MaxQuestions;
                if (complete && !CompleteDt.HasValue)
                {
                    CompleteDt = DateTime.UtcNow;
                }

                return complete;
            }
        }

        [JsonProperty("recordedAnswers")]
        public Answer[] RecordedAnswers { get; set; } = {};

        public override int MoveToNextQuestion()
        {
            if (RecordedAnswers.Count() >= CurrentQuestion)
            {
                CurrentQuestion = FindNextQuestion();
                return CurrentQuestion;
            }

            CurrentQuestion = FindNextUnansweredQuestion();
            return CurrentQuestion;
        }

        public void SetCurrentQuestion(int questionNumber)
        {
            CurrentQuestion = Math.Min(questionNumber, MaxQuestions);
        }

        /// <summary>
        /// Gets the first answered question number.
        /// </summary>
        private int FindNextUnansweredQuestion()
        {
            for (int i = 1; i <= MaxQuestions; i++)
            {
                if (RecordedAnswers.All(x => x.QuestionNumber != i))
                {
                    
                    return i;
                }
            }
        
            return MaxQuestions;
        }
        
        private int FindNextQuestion()
        {
            return Math.Min(CurrentQuestion + 1, MaxQuestions);
        }
    }
}
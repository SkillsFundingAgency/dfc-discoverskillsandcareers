using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace Dfc.DiscoverSkillsAndCareers.Models
{
    public class AssessmentState
    {
        [JsonProperty("questionSetVersion")]
        public string QuestionSetVersion { get; set; }
        
        [JsonProperty("maxQuestions")]
        public int MaxQuestions { get; set; }
        
        [JsonProperty("currentQuestion")]
        public int CurrentQuestion { get; set; }
        
        [JsonProperty("isComplete")]
        public bool IsComplete { get; set; }
        
        [JsonProperty("completeDt")]
        public DateTime? CompleteDt { get; set; }
        
        [JsonProperty("recordedAnswers")]
        public Answer[] RecordedAnswers { get; set; } = {};
    }
    
    public class FilteredAssessmentState : AssessmentState
    {
        [JsonProperty("currentFilterAssessmentCode")]
        public string CurrentFilterAssessmentCode { get; set; }
        
        [JsonProperty("jobFamilyName")]
        public string JobFamilyName { get; set; }
        
        [JsonIgnore]
        public string JobFamilyNameUrlSafe => JobFamilyName?.ToLower()?.Replace(" ", "-");

    }
    
    public class UserSession
    {
        [JsonIgnore]
        public string PrimaryKey => $"{PartitionKey}-{UserSessionId}";

        [JsonProperty("partitionKey")]
        public string PartitionKey { get; set; }
        [JsonProperty("id")]
        public string UserSessionId { get; set; }
        [JsonProperty("languageCode")]
        public string LanguageCode { get; set; }
        [JsonProperty("salt")]
        public string Salt { get; set; }
        
        [JsonProperty("assessmentState")]
        public AssessmentState AssessmentState { get; set; }
        
        [JsonProperty("filteredAssessmentState")]
        public FilteredAssessmentState FilteredAssessmentState { get; set; }
        
        [JsonProperty("resultData")]
        public ResultData ResultData { get; set; } 

        [JsonProperty("startedDt")]
        public DateTime StartedDt { get; set; }
        
        [JsonProperty("assessmentType")]
        public string AssessmentType { get; set; }
        
        [JsonProperty("lastUpdatedDt")]
        public DateTime LastUpdatedDt { get; set; }

        [JsonIgnore]
        public AssessmentState CurrentState => IsFilterAssessment ? FilteredAssessmentState : AssessmentState;

        // Extension getters
        [JsonIgnore]
        public bool IsFilterAssessment => FilteredAssessmentState != null;

        [JsonIgnore] 
        public int CurrentMaxQuestions => CurrentState.MaxQuestions;

        [JsonIgnore] 
        public string CurrentQuestionSetVersion => CurrentState.QuestionSetVersion;

        [JsonIgnore] 
        public int CurrentQuestion => CurrentState.CurrentQuestion;
        
        [JsonIgnore] 
        public int MaxQuestions => CurrentState.MaxQuestions;

        [JsonIgnore] 
        public bool IsComplete => CurrentState.IsComplete;

        [JsonIgnore] 
        public DateTime? CompleteDt => CurrentState.CompleteDt;
        
        [JsonIgnore]
        public Answer[] CurrentRecordedAnswers => CurrentState.RecordedAnswers.Where(x => x.QuestionSetVersion == CurrentQuestionSetVersion).ToArray();
        
        
        /// <summary>
        /// Gets the next question number that should be answered.
        /// </summary>
        public void MoveToNextQuestion()
        {
            for (int i = 1; i <= CurrentMaxQuestions; i++)
            {
                if (CurrentRecordedAnswers.All(x => x.QuestionNumber != i.ToString()))
                {
                    CurrentState.CurrentQuestion = i;
                    return;
                }
            }
            throw new Exception("All questions answered");
        }
        
        /// <summary>
        /// Updates the IsComplete property on the UserSession based off the current answers and max questions.
        /// </summary>
        public void UpdateCompletionStatus()
        {
            bool allQuestionsAnswered = CurrentRecordedAnswers.Count() == CurrentMaxQuestions;
            var state = CurrentState;
                
            if(allQuestionsAnswered) {
                state.IsComplete = true;
                state.CompleteDt = DateTime.Now;
            }
            
        }
    }
}

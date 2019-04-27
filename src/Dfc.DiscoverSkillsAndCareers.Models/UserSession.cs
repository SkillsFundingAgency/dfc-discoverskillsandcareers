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

        [JsonIgnore] public bool IsComplete => MaxQuestions == RecordedAnswers?.Length;
        
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


        public bool TrySetStateToExistingSession(string assessment)
        {
            if (String.Equals("short", assessment, StringComparison.InvariantCultureIgnoreCase))
            {
                FilteredAssessmentState = null;
                return true;
            }
            
            var jobFamily = ResultData?.JobFamilies.SingleOrDefault(jf =>
                String.Equals(jf.JobFamilyNameUrlSafe, assessment, StringComparison.InvariantCultureIgnoreCase));

            
            //Check it we already have the correct filtered assessment
            //If this is the case then there is nothing else to do except potentially 
            //copy recorded answers forward. This helps dealing with scenarios where 
            //the url is directly navigated to.
            if (FilteredAssessmentState != null 
                && String.Equals(FilteredAssessmentState.JobFamilyNameUrlSafe, assessment, StringComparison.InvariantCultureIgnoreCase))
            {
                var answers = FilteredAssessmentState.RecordedAnswers?.Length;
                if (answers.GetValueOrDefault(0) == 0)
                {
                    FilteredAssessmentState.RecordedAnswers = jobFamily?.FilterAssessment?.RecordedAnswers ?? new Answer[] { };
                }

                return true;
            }
            
            //Otherwise hydrate the FilteredAssessment state from a previously answered job family result.
            
            if (jobFamily?.FilterAssessment == null) 
                return false;
            
            var filterAssessment = jobFamily.FilterAssessment;
            FilteredAssessmentState = new FilteredAssessmentState
            {
                MaxQuestions = filterAssessment.MaxQuestions,
                CurrentQuestion = filterAssessment?.RecordedAnswerCount ?? 1,
                QuestionSetVersion = filterAssessment.QuestionSetVersion,
                JobFamilyName = jobFamily.JobFamilyName,
                RecordedAnswers = filterAssessment.RecordedAnswers,
            };

            return true;

        }
        
        /// <summary>
        /// Gets the next question number that should be answered.
        /// </summary>
        public int FindNextUnansweredQuestion()
        {
            for (int i = 1; i <= CurrentMaxQuestions; i++)
            {
                if (CurrentRecordedAnswers.All(x => x.QuestionNumber != i))
                {
                    return i;
                }
            }

            return CurrentMaxQuestions;
        }
        
        /// <summary>
        /// Updates the IsComplete property on the UserSession based off the current answers and max questions.
        /// </summary>
        public void UpdateCompletionStatus()
        {                
            if(IsComplete) {
                CurrentState.CompleteDt = DateTime.Now;
            }   
        }
    }
}

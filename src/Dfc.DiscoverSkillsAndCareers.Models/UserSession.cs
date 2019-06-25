using System;
using System.Linq;
using Dfc.DiscoverSkillsAndCareers.Models.Extensions;
using Newtonsoft.Json;

namespace Dfc.DiscoverSkillsAndCareers.Models
{
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

        [JsonProperty("completeDt")] 
        public DateTime? CompleteDt => CurrentState.CompleteDt;

        [JsonIgnore] 
        public bool IsComplete => CurrentState.IsComplete;

        [JsonIgnore]
        public int MaxQuestions => CurrentState.MaxQuestions;

        [JsonIgnore]
        public AssessmentStateBase CurrentState =>
            (FilteredAssessmentState == null 
             || (String.IsNullOrWhiteSpace(FilteredAssessmentState.CurrentFilterAssessmentCode))) 
             || !AssessmentState.IsComplete
                ? (AssessmentStateBase)AssessmentState
                : (AssessmentStateBase)FilteredAssessmentState;

        [JsonIgnore]
        public int CurrentQuestion => CurrentState.CurrentQuestion;

        [JsonIgnore]
        public string CurrentQuestionSetVersion => CurrentState.QuestionSetVersion;
        
        [JsonIgnore]
        public bool IsFilterAssessment => (CurrentState is FilteredAssessmentState);
        
        [JsonIgnore]
        public Answer[] RecordedAnswers
        {
            get
            {
                switch (CurrentState)
                {
                    case AssessmentState a:
                        return a.RecordedAnswers;

                    case FilteredAssessmentState f:
                        return f.RecordedAnswers;

                    default:
                        throw new ArgumentException($"Unable to get recorded answers for assessmentType {CurrentState?.GetType().Name}");
                }
            }
        }
        
        public void UpdateJobCategoryQuestionCount()
        {
            if (FilteredAssessmentState != null)
            {
                foreach (var jobCategory in ResultData.JobCategories)
                {
                    var state = FilteredAssessmentState
                        .JobCategoryStates
                        .FirstOrDefault(jc => jc.JobCategoryCode.EqualsIgnoreCase(jobCategory.JobCategoryCode));

                    if (state != null)
                    {
                        jobCategory.TotalQuestions = state.UnansweredQuestions(FilteredAssessmentState.RecordedAnswers);
                    }
                }
            }
        }
        
        public void AddAnswer(AnswerOption answerValue, Question question)
        {
            var answer = new Answer()
            {
                AnsweredDt = DateTime.UtcNow,
                SelectedOption = answerValue,
                QuestionId = question.QuestionId,
                QuestionNumber = question.Order,
                QuestionText = question.Texts.FirstOrDefault(x => x.LanguageCode.ToLower() == "en")?.Text,
                TraitCode = question.TraitCode,
                IsNegative = question.IsNegative,
                QuestionSetVersion = CurrentQuestionSetVersion
            };

            if (question.IsFilterQuestion)
            {
                var newAnswerSet = FilteredAssessmentState.RecordedAnswers
                    .Where(x => x.QuestionId != question.QuestionId)
                    .ToList();
                
                newAnswerSet.Add(answer);
                FilteredAssessmentState.RecordedAnswers = newAnswerSet.ToArray();
            }
            else
            {
                var newAnswerSet = AssessmentState.RecordedAnswers
                    .Where(x => x.QuestionId != question.QuestionId)
                    .ToList();
                
                newAnswerSet.Add(answer);
                AssessmentState.RecordedAnswers = newAnswerSet.ToArray();
            }
        }

    }
}

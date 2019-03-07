using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace Dfc.DiscoverSkillsAndCareers.Models
{
    public class UserSession
    {
        [JsonIgnore]
        public string PrimaryKey {  get { return $"{PartitionKey}-{UserSessionId}"; } }
        [JsonProperty("partitionKey")]
        public string PartitionKey { get; set; }
        [JsonProperty("id")]
        public string UserSessionId { get; set; }
        [JsonProperty("languageCode")]
        public string LanguageCode { get; set; }
        [JsonProperty("salt")]
        public string Salt { get; set; }
        [JsonProperty("questionSetVersion")]
        public string QuestionSetVersion { get; set; }
        [JsonProperty("maxQuestions")]
        public int MaxQuestions { get; set; }
        [JsonProperty("currentQuestion")]
        public int CurrentQuestion { get; set; }
        [JsonProperty("recordedAnswers")]
        public Answer[] RecordedAnswers { get; set; } = {};
        [JsonProperty("resultData")]
        public ResultData ResultData { get; set; } 
        [JsonProperty("isComplete")]
        public bool IsComplete { get; set; }
        [JsonProperty("startedDt")]
        public DateTime StartedDt { get; set; }
        [JsonProperty("completeDt")]
        public DateTime? CompleteDt { get; set; }
        [JsonProperty("assessmentType")]
        public string AssessmentType { get; set; }
        [JsonProperty("currentFilterAssessmentCode")]
        public string CurrentFilterAssessmentCode { get; set; }

        // TODO: extentions
        [JsonIgnore]
        public bool IsFilterAssessment => !string.IsNullOrEmpty(CurrentFilterAssessmentCode);
        [JsonIgnore]
        public int CurrentMaxQuestions
        {
            get
            {
                if (IsFilterAssessment)
                {
                    return ResultData.JobFamilies.Where(x => x.JobFamilyCode == CurrentFilterAssessmentCode).First().FilterAssessment.MaxQuestions;
                }
                return MaxQuestions;
            }
        }
        [JsonIgnore]
        public string CurrentQuestionSetVersion
        {
            get
            {
                if (!IsFilterAssessment)
                {
                    return QuestionSetVersion;
                }
                return ResultData.JobFamilies.Where(x => x.JobFamilyCode == CurrentFilterAssessmentCode).First().FilterAssessment.QuestionSetVersion;
            }
        }
        [JsonIgnore]
        public IEnumerable<Answer> CurrentRecordedAnswers
        {
            get
            {
                return RecordedAnswers.Where(x => x.QuestionSetVersion == CurrentQuestionSetVersion);
            }
        }
        [JsonIgnore]
        public FilterAssessment CurrentFilterAssessment
        {
            get
            {
                if (string.IsNullOrEmpty(CurrentFilterAssessmentCode)) return null;
                return ResultData.JobFamilies.Where(x => x.JobFamilyCode == CurrentFilterAssessmentCode).First().FilterAssessment;
            }
        }
    }
}

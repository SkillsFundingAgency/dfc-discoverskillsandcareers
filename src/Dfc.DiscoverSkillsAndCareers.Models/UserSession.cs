using System;
using System.Collections.Generic;
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
        public List<Answer> RecordedAnswers { get; set; } = new List<Answer>();
        [JsonProperty("resultData")]
        public ResultData ResultData { get; set; } 
        [JsonProperty("isComplete")]
        public bool IsComplete { get; set; }
        [JsonProperty("startedDt")]
        public DateTime StartedDt { get; set; }
        [JsonProperty("completeDt")]
        public DateTime? CompleteDt { get; set; }
    }
}

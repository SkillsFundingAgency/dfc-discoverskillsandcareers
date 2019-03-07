using Newtonsoft.Json;
using System;

namespace Dfc.DiscoverSkillsAndCareers.Models
{
    public class FilterAssessment
    {
        [JsonProperty("jobFamilyName")]
        public string JobFamilyName { get; set; }
        [JsonProperty("createdDt")]
        public DateTime CreatedDt { get; set; }
        [JsonProperty("questionSetVersion")]
        public string QuestionSetVersion { get; set; }
        [JsonProperty("recordedAnswerCount")]
        public int RecordedAnswerCount { get; set; }
        [JsonProperty("maxQuestions")]
        public int MaxQuestions { get; set; }
        [JsonProperty("suggestedJobProfiles")]
        public string[] SuggestedJobProfiles { get; set; }

        [JsonIgnore]
        public string JobFamilyNameUrlSafe => JobFamilyName?.ToLower()?.Replace(" ", "-");
    }
}

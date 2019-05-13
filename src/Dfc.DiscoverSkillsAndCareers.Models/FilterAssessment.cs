using Newtonsoft.Json;
using System;
using System.Collections.Generic;

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

        [JsonProperty("recordedAnswers")]
        public Answer[] RecordedAnswers { get; set; } = { };

        [JsonProperty("suggestedJobProfiles")]
        public IDictionary<string, string> SuggestedJobProfiles { get; set; } = new Dictionary<string, string>();

        [JsonProperty("whatYouToldUs")]
        public string[] WhatYouToldUs { get; set; } = { };

        [JsonIgnore]
        public string JobFamilyNameUrlSafe => JobFamilyName?.ToLower()?.Replace(" ", "-");
    }
}

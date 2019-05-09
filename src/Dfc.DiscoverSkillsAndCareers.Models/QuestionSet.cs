using Newtonsoft.Json;
using System;

namespace Dfc.DiscoverSkillsAndCareers.Models
{
    public class QuestionSet
    {
        [JsonProperty("partitionKey")]
        public string PartitionKey { get; set; }
        
        [JsonProperty("id")]
        public string QuestionSetVersion { get; set; }
        
        [JsonProperty("version")]
        public int Version { get; set; }
        
        [JsonProperty("assessmentType")]
        public string AssessmentType { get; set; }
        
        [JsonProperty("maxQuestions")]
        public int MaxQuestions { get; set; }
        
        [JsonProperty("lastUpdated")]
        public DateTimeOffset LastUpdated { get; set; }
        
        [JsonProperty("description")]
        public string Description { get; set; }
        
        [JsonProperty("title")]
        public string Title { get; set; }
        
        [JsonProperty("titleLowercase")]
        public string QuestionSetKey { get; set; }
        
        [JsonProperty("isCurrent")]
        public bool IsCurrent { get; set; }
    }
}

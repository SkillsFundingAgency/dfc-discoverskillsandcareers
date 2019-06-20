using System;
using Newtonsoft.Json;

namespace Dfc.DiscoverSkillsAndCareers.ChangeFeed.Common
{
    //TODO: Delete job profiles in the new world because we record everyone every time this will get crazy
    
    public class QuestionText
    {
        [JsonProperty("languageCode")]
        public string LanguageCode { get; set; }
        
        [JsonProperty("text")]
        public string Text { get; set; }
    }

    public class QuestionJobProfile
    {
        [JsonProperty("jobProfile")]
        public string JobProfile { get; set; }
        
        [JsonProperty("included")]
        public bool Included { get; set; }
    }
    
    public class Question
    {
        [JsonProperty("partitionKey")]
        public string PartitionKey { get; set; }
        
        [JsonProperty("id")]
        public string QuestionId { get; set; }
        
        [JsonProperty("sfid")]
        public string SfId { get; set; }
        
        [JsonProperty("texts")]
        public QuestionText[] Texts { get; set; } = {};
        
        [JsonProperty("lastUpdatedDt")]
        public DateTimeOffset LastUpdatedDt { get; set; }
        
        [JsonProperty("jobProfiles")] 
        public QuestionJobProfile[] JobProfiles { get; set; } = { };
        
        [JsonProperty("traitCode")]
        public string TraitCode { get; set; }
        
        [JsonProperty("isNegative")]
        public bool IsNegative { get; set; }
        
        [JsonProperty("order")]
        public int Order { get; set; }

        [JsonProperty("positiveResultDisplayText")]
        public string PositiveResultDisplayText { get; set; }
        
        [JsonProperty("negativeResultDisplayText")]
        public string NegativeResultDisplayText { get; set; }
    }
}
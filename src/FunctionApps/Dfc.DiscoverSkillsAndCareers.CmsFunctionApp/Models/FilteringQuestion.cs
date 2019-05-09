using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace Dfc.DiscoverSkillsAndCareers.CmsFunctionApp.Models
{
    public class FilteringQuestion
    {
        [JsonProperty("Id")]
        public string Id { get; set; }
        
        [JsonProperty("QuestionText")]
        public string Title { get; set; }
        
        [JsonProperty("Description")]
        public string Description { get; set; }
        
        [JsonProperty("ExcludesJobProfiles")]
        public List<string> ExcludesJobProfiles { get; set; }
        
        [JsonProperty("IsYes")]
        public bool IsYes { get; set; }
        
        [JsonProperty("PositiveResultDisplayText")]
        public string PositiveResultDisplayText { get; set; }
        
        [JsonProperty("NegativeResultDisplayText")]
        public string NegativeResultDisplayText { get; set; }
        
        [JsonProperty("LastModified")]
        public DateTimeOffset LastUpdated { get; set; }
    }
}

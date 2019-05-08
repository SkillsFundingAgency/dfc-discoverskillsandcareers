using System;
using Newtonsoft.Json;

namespace Dfc.DiscoverSkillsAndCareers.CmsFunctionApp.Models
{
    public class ShortQuestion
    {
        [JsonProperty("Id")]
        public string Id { get; set; }
        
        [JsonProperty("QuestionText")]
        public string Title { get; set; }
        
        [JsonProperty("Description")]
        public string Description { get; set; }
        
        [JsonProperty("Trait")]
        public string Trait { get; set; }
        
        [JsonProperty("Order")]
        public int? Order { get; set; }
        
        [JsonProperty("IsNegative")]
        public bool IsNegative { get; set; }
        
        [JsonProperty("LastUpdatedDt")]
        public DateTimeOffset LastUpdatedDt { get; set; }
    }
}

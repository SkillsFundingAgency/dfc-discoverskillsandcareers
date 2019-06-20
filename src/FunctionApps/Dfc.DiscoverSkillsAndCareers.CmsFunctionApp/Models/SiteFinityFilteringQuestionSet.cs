using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace Dfc.DiscoverSkillsAndCareers.CmsFunctionApp.Models
{
    public class SiteFinityFilteringQuestionSet
    {
        [JsonProperty("Id")]
        public string Id { get; set; }
        
        [JsonProperty("Title")]
        public string Title { get; set; }
        
        [JsonProperty("Description")]
        public string Description { get; set; }
        
        [JsonProperty("Questions")]
        public List<SiteFinityFilteringQuestion> Questions { get; set; }
        
        [JsonProperty("LastModified")]
        public DateTimeOffset LastUpdated { get; set; }
    }
}

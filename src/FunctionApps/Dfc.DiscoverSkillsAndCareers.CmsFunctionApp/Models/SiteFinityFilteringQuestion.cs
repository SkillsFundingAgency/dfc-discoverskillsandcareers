using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace Dfc.DiscoverSkillsAndCareers.CmsFunctionApp.Models
{
    public class SiteFinityFilteringQuestionJobProfileMapping
    {
        public string JobProfile { get; set; }
        public bool Included { get; set; }
    }
    
    public class SiteFinityFilteringQuestion
    {
        [JsonProperty("Id")]
        public string Id { get; set; }
        
        [JsonProperty("QuestionText")]
        public string Title { get; set; }
        
        [JsonProperty("Description")]
        public string Description { get; set; }
        
        [JsonProperty]
        public SiteFinityONetSkill Skill { get; set; }
        
        public List<SiteFinityFilteringQuestionJobProfileMapping> JobProfiles { get; set; }
        
        
        [JsonProperty("LastModified")]
        public DateTimeOffset LastUpdated { get; set; }
    }
}

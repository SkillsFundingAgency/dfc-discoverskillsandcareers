using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;

namespace Dfc.DiscoverSkillsAndCareers.CmsFunctionApp.Models
{
    

    public class SiteFinityFilteringQuestionSkill
    {
        [JsonProperty("Id")]
        public Guid Id { get; set; }
        
        [JsonProperty("Title")]
        public string Title { get; set; }
    }
    
    public class SiteFinityFilteringQuestion
    {
        [JsonProperty("Id")]
        public Guid Id { get; set; }
        
        [JsonProperty("Title")]
        public string Title { get; set; }
        
        [JsonProperty("QuestionText")]
        public string QuestionText { get; set; }
        
        [JsonProperty("Description")]
        public string Description { get; set; }
      
        
        [JsonProperty("RelatedSkill")]
        public SiteFinityFilteringQuestionSkill RelatedSkill { get; set; }
        
        [JsonProperty("LastModified")]
        public DateTimeOffset LastUpdated { get; set; }
        
        
        
    }
}

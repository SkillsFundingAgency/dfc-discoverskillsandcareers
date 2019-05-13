using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace Dfc.DiscoverSkillsAndCareers.CmsFunctionApp.Models
{
    public class FilteringQuestionSet
    {
        [JsonProperty("Id")]
        public string Id { get; set; }
        [JsonProperty("Title")]
        public string Title { get; set; }
        [JsonProperty("Description")]
        public string Description { get; set; }
        public List<FilteringQuestion> Questions { get; set; }
        
        public string JobCategory => JobProfileCategories?.First();
        
        public string[] JobProfileCategories { get; set; }
        
        public TaxonomyHierarchy[] JobProfileTaxonomy { get; set; }
        
        [JsonProperty("LastModified")]
        public DateTimeOffset LastUpdated { get; set; }
    }
}

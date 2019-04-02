using System;
using System.Collections.Generic;
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
        [JsonProperty("LastModified")]
        public DateTime LastUpdated { get; set; }
    }
}

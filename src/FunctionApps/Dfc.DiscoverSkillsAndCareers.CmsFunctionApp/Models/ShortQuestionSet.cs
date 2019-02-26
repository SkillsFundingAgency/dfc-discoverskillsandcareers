using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Dfc.DiscoverSkillsAndCareers.CmsFunctionApp.Models
{
    public class ShortQuestionSet
    {
        [JsonProperty("Id")]
        public string Id { get; set; }
        [JsonProperty("Title")]
        public string Title { get; set; }
        [JsonProperty("Description")]
        public string Description { get; set; }
        [JsonIgnore()]
        public List<ShortQuestion> Questions { get; set; }
        [JsonProperty("LastModified")]
        public DateTime LastUpdated { get; set; }
    }
}

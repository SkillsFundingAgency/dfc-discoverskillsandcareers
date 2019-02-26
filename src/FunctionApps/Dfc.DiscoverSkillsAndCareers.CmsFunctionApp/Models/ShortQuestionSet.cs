using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Dfc.DiscoverSkillsAndCareers.CmsFunctionApp.Models
{
    public class ShortQuestionSet
    {
        [JsonProperty("Title")]
        public string Title { get; set; }
        [JsonProperty("Description")]
        public string Description { get; set; }
        [JsonIgnore()]
        public List<ShortQuestion> Questions { get; set; }
        [JsonProperty("PublicationDate")]
        public DateTime LastUpdated { get; set; }
    }
}

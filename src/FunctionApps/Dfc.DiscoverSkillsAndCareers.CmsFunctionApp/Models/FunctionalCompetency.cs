using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace Dfc.DiscoverSkillsAndCareers.CmsFunctionApp.Models
{
    public class FunctionalCompetency
    {
        [JsonProperty("Id")]
        public string Id { get; set; }
        [JsonProperty("Title")]
        public string Title { get; set; }
        [JsonProperty("JobCategory")]
        public string JobCategory { get; set; }
        [JsonProperty("Description")]
        public string Description { get; set; }
        public ShortQuestion Question { get; set; }
        public List<FakeJobProfile> ExcludeJobProfiles { get; set; }
        [JsonProperty("Order")]
        public int? Order { get; set; }
        public bool IsNegative { get; set; }
        [JsonProperty("LastModified")]
        public DateTime LastUpdated { get; set; }
    }
}

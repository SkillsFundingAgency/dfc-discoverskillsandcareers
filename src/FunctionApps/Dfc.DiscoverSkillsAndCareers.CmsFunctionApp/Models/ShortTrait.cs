using Newtonsoft.Json;
using System;

namespace Dfc.DiscoverSkillsAndCareers.CmsFunctionApp.Models
{
    public class ShortTrait
    {
        [JsonProperty("Name")]
        public string Name { get; set; }
        [JsonProperty("LastModified")]
        public DateTime LastUpdated { get; set; }
        [JsonProperty("Code")]
        public string Code { get; set; }
        [JsonProperty("ResultDisplayText")]
        public string ResultDisplayText { get; set; }
    }
}

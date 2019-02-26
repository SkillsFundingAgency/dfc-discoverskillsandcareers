using Newtonsoft.Json;
using System;

namespace Dfc.DiscoverSkillsAndCareers.CmsFunctionApp.Models
{
    public class ShortTrait
    {
        [JsonProperty("Name")]
        public string Name { get; set; }
        [JsonProperty("PublicationDate")]
        public DateTime LastUpdated { get; set; }
    }
}

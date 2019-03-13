using Newtonsoft.Json;

namespace Dfc.DiscoverSkillsAndCareers.Models
{
    public class Trait
    {
        [JsonProperty("partitionKey")]
        public string PartitionKey { get; set; }
        [JsonProperty("id")]
        public string TraitCode { get; set; }
        [JsonProperty("traitName")]
        public string TraitName { get; set; }
        [JsonProperty("texts")]
        public TraitText[] Texts { get; set; } = {};
    }
}

using Newtonsoft.Json;

namespace Dfc.DiscoverSkillsAndCareers.Models
{
    public class TraitValue
    {
        [JsonProperty("traitCode")]
        public string TraitCode { get; set; }
        
        [JsonProperty("total")]
        public int Total { get; set; }
        
        [JsonProperty("normalizedTotal")]
        public decimal NormalizedTotal { get; set; }
    }
}
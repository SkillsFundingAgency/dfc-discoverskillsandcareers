using Newtonsoft.Json;

namespace Dfc.DiscoverSkillsAndCareers.Models
{
    public class ResultData
    {
        [JsonProperty("traits")]
        public TraitResult[] Traits { get; set; }
        [JsonProperty("jobFamilies")]
        public JobFamilyResult[] JobFamilies { get; set; }
    }
}

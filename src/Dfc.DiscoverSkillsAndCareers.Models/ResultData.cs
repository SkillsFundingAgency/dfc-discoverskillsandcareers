using Newtonsoft.Json;

namespace Dfc.DiscoverSkillsAndCareers.Models
{
    public class ResultData
    {
        [JsonProperty("traits")]
        public TraitResult[] Traits { get; set; }
        
        [JsonProperty("jobFamilies")]
        public JobCategoryResult[] JobCategories { get; set; }
        
        [JsonProperty("traitsscores")]
        public TraitResult[] TraitScores { get; set; }
    }
}

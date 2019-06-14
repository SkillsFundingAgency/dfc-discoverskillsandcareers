using Newtonsoft.Json;

namespace Dfc.DiscoverSkillsAndCareers.Models
{
    public class QuestionJobProfile
    {
        [JsonProperty("jobProfile")]
        public string JobProfile { get; set; }
        
        [JsonProperty("included")]
        public bool Included { get; set; }
    }
}
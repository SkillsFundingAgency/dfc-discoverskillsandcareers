using Newtonsoft.Json;

namespace Dfc.DiscoverSkillsAndCareers.WebApp.Models
{
    public class ResultsJobCategoryResult
    {
        [JsonProperty("sessionId")]
        public string SessionId { get; set; }
        [JsonProperty("jobProfiles")]
        public JobProfile[] JobProfiles { get; set; }
    }
}

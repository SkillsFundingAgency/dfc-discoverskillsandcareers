using Newtonsoft.Json;

namespace Dfc.DiscoverSkillsAndCareers.ResultsFunctionApp.Models
{
    public class ResultsJobCategoryResult
    {
        [JsonProperty("sessionId")]
        public string SessionId { get; set; }

        [JsonProperty("jobProfiles")]
        public JobProfileResult[] JobProfiles { get; set; }
    }
}
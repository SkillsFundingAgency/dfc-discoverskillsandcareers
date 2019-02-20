using Newtonsoft.Json;

namespace Dfc.DiscoverSkillsAndCareers.WebApp.Models
{
    public class PostAnswerResponse
    {
        [JsonProperty("isSuccess")]
        public bool IsSuccess { get; set; }
        [JsonProperty("isComplete")]
        public bool IsComplete { get; set; }
    }
}

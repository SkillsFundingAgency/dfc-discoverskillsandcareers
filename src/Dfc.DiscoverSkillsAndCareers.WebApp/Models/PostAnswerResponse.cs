using Newtonsoft.Json;

namespace Dfc.DiscoverSkillsAndCareers.WebApp.Models
{
    public class PostAnswerResponse
    {
        [JsonProperty("isSuccess")]
        public bool IsSuccess { get; set; }
        [JsonProperty("isComplete")]
        public bool IsComplete { get; set; }
        public string JobCategorySafeUrl { get; set; }
        public bool IsFilterAssessment { get; set; }
        public int NextQuestionNumber { get; set; }
    }
}

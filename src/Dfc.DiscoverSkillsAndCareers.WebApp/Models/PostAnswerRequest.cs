using Newtonsoft.Json;

namespace Dfc.DiscoverSkillsAndCareers.WebApp.Models
{
    public class PostAnswerRequest
    {
        [JsonProperty("questionId")]
        public string QuestionId { get; set; }
        [JsonProperty("selectedOption")]
        public string SelectedOption { get; set; }
    }
}

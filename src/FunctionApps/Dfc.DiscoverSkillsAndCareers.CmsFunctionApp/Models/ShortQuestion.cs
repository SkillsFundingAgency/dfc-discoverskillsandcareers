using Newtonsoft.Json;

namespace Dfc.DiscoverSkillsAndCareers.CmsFunctionApp.Models
{
    public class ShortQuestion
    {
        [JsonProperty("Id")]
        public string Id { get; set; }
        [JsonProperty("QuestionText")]
        public string Title { get; set; }
        [JsonProperty("Description")]
        public string Description { get; set; }
        public string Trait { get; set; }
        [JsonProperty("Order")]
        public int? Order { get; set; }
        public bool IsNegative { get; set; }
    }
}

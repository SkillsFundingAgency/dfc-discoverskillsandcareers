using Newtonsoft.Json;
using System.Collections.Generic;

namespace Dfc.DiscoverSkillsAndCareers.Models
{
    public class FilteringQuestion
    {
        [JsonProperty("partitionKey")]
        public string PartitionKey { get; set; }
        [JsonProperty("id")]
        public string FilteringQuestionId { get; set; }
        [JsonProperty("texts")]
        public QuestionText[] Texts { get; set; } = { };
        [JsonProperty("jobFamily")]
        public string JobFamily { get; set; }
        [JsonProperty("order")]
        public int Order { get; set; }
        [JsonProperty("excludesJobProfiles")]
        public string[] ExcludesJobProfiles { get; set; } = { };
        [JsonProperty("excludeAnswerTrigger")]
        public string ExcludeAnswerTrigger { get; set; } = "No";
        [JsonProperty("answerOptions")]
        public string[] AnswerOptions { get; set; } = new [] { "Yes", "No" };
    }
}

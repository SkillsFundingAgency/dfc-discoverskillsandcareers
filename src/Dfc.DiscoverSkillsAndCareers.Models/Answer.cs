using System;
using Newtonsoft.Json;

namespace Dfc.DiscoverSkillsAndCareers.Models
{
    public class Answer
    {
        [JsonProperty("questionId")]
        public string QuestionId { get; set; }
        [JsonProperty("questionNumber")]
        public string QuestionNumber { get; set; }
        [JsonProperty("questionText")]
        public string QuestionText { get; set; }
        [JsonProperty("selectedOption")]
        public string SelectedOption { get; set; }
        [JsonProperty("answeredDt")]
        public DateTime AnsweredDt { get; set; }
    }
}

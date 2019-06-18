using System;
using Newtonsoft.Json;

namespace Dfc.DiscoverSkillsAndCareers.Models
{
    public class Answer
    {
        [JsonProperty("questionId")]
        public string QuestionId { get; set; }
        
        [JsonProperty("questionNumber")]
        public int QuestionNumber { get; set; }
        
        [JsonProperty("questionText")]
        public string QuestionText { get; set; }
        
        [JsonProperty("traitCode")]
        public string TraitCode { get; set; }
        
        [JsonProperty("selectedOption")]
        public AnswerOption SelectedOption { get; set; }
        
        [JsonProperty("answeredDt")]
        public DateTime AnsweredDt { get; set; }
        
        [JsonProperty("isNegative")]
        public bool IsNegative { get; set; }
        
        [JsonProperty("questionSetVersion")]
        public string QuestionSetVersion { get; set; }
    }
}

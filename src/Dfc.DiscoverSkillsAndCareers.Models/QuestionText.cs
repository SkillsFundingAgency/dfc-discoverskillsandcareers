using System;
using Newtonsoft.Json;

namespace Dfc.DiscoverSkillsAndCareers.Models
{
    public class QuestionText
    {
        [JsonProperty("languageCode")]
        public string LanguageCode { get; set; }
        [JsonProperty("text")]
        public string Text { get; set; }
    }
}

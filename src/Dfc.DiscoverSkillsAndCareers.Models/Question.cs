using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Dfc.DiscoverSkillsAndCareers.Models
{
    public class Question
    {
        [JsonProperty("partitionKey")]
        public string PartitionKey { get; set; }
        [JsonProperty("id")]
        public string QuestionId { get; set; }
        [JsonProperty("texts")]
        public QuestionText[] Texts { get; set; } = {};
        [JsonProperty("traitCode")]
        public string TraitCode { get; set; }
        [JsonProperty("isNegative")]
        public bool IsNegative { get; set; }
        [JsonProperty("order")]
        public int Order { get; set; }
        [JsonProperty("excludesJobProfiles")]
        public List<string> ExcludesJobProfiles { get; set; } = new List<string>();
    }
}

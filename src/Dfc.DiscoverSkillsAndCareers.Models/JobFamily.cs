using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Dfc.DiscoverSkillsAndCareers.Models
{
    public class JobFamily
    {
        [JsonProperty("id")]
        public string JobFamilyCode { get; set; }
        [JsonProperty("jobFamilyName")]
        public string JobFamilyName { get; set; }
        [JsonProperty("texts")]
        public JobFamilyText[] Texts { get; set; } = {};
        [JsonProperty("traitCodes")]
        public string[] TraitCodes { get; set; } = {};
        [JsonIgnore]
        public decimal ResultMultiplier {  get { return 1m / TraitCodes.Length; } }
        [JsonProperty("partitionKey")]
        public string PartitionKey { get; set; }
    }
}

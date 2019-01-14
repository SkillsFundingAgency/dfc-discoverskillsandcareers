using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Dfc.DiscoverSkillsAndCareers.Models
{
    public class JobFamily
    {
        [JsonProperty("jobFamilyCode")]
        public string JobFamilyCode { get; set; }
        [JsonProperty("jobFamilyName")]
        public string JobFamilyName { get; set; }
        [JsonProperty("texts")]
        public List<JobFamilyText> Texts { get; set; } = new List<JobFamilyText>();
        [JsonProperty("traitCodes")]
        public List<string> TraitCodes { get; set; } = new List<string>();
        [JsonIgnore]
        public decimal ResultMultiplier {  get { return 1m / TraitCodes.Count; } }
    }
}

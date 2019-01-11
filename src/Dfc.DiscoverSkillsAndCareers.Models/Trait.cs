using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Dfc.DiscoverSkillsAndCareers.Models
{
    public class Trait
    {
        [JsonProperty("traitCode")]
        public string TraitCode { get; set; }
        [JsonProperty("traitName")]
        public string TraitName { get; set; }
        [JsonProperty("texts")]
        public List<TraitText> Texts { get; set; } = new List<TraitText>();
    }
}

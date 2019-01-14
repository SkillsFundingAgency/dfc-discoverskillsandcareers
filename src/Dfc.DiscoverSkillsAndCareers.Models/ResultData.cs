using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Dfc.DiscoverSkillsAndCareers.Models
{
    public class ResultData
    {
        [JsonProperty("traits")]
        public List<TraitResult> Traits { get; set; }
        [JsonProperty("jobFamilyCodes")]
        public List<string> JobFamilyCodes { get; set; }
    }
}

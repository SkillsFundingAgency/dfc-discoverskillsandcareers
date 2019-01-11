using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Dfc.DiscoverSkillsAndCareers.Models
{
    public class ResultData
    {
        [JsonProperty("traitCodes")]
        public List<string> TraitCodes { get; set; }
        [JsonProperty("jobFamilyCodes")]
        public List<string> JobFamilyCodes { get; set; }
    }
}

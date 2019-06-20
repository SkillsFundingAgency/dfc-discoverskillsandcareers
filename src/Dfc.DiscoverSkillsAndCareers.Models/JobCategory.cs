using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Dfc.DiscoverSkillsAndCareers.Models
{
    public class JobCategory
    {
        [JsonProperty("id")] 
        public string Code => JobCategoryHelper.GetCode(Name);
        
        [JsonProperty("jobFamilyName")]
        public string Name { get; set; }
        
        [JsonProperty("texts")]
        public JobCategoryText[] Texts { get; set; } = {};
        
        [JsonProperty("traitCodes")]
        public string[] Traits { get; set; } = {};
        
        [JsonIgnore]
        public decimal ResultMultiplier {  get { return 1m / Traits.Length; } }
        
        [JsonProperty("partitionKey")]
        public string PartitionKey { get; set; }

        public List<JobProfileSkillMapping> Skills { get; set; } = new List<JobProfileSkillMapping>();
    }
    
}

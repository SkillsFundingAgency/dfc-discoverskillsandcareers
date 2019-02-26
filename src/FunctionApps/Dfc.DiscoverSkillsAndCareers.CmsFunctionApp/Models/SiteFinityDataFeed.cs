using Newtonsoft.Json;
using System.Collections.Generic;

namespace Dfc.DiscoverSkillsAndCareers.CmsFunctionApp.Models
{
    public class SiteFinityDataFeed<T> where T : class
    {
        [JsonProperty("value")]
        public List<T> Value { get; set; }
    }
}

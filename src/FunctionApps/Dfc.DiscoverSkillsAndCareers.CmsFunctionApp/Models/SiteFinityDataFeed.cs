using Newtonsoft.Json;

namespace Dfc.DiscoverSkillsAndCareers.CmsFunctionApp.Models
{
    public class SiteFinityDataFeed<T> where T : class
    {
        [JsonProperty("value")]
        public T Value { get; set; }
    }
}

using Newtonsoft.Json;

namespace Dfc.DiscoverSkillsAndCareers.Models.SiteFinity
{
    public class SiteFinityDataFeed<T> where T : class
    {
        [JsonProperty("value")]
        public T Value { get; set; }
    }
}
using System.Diagnostics.CodeAnalysis;
using Newtonsoft.Json;

namespace Dfc.DiscoverSkillsAndCareers.Models.SiteFinity
{
    [ExcludeFromCodeCoverage]
    public class SiteFinityDataFeed<T> where T : class
    {
        [JsonProperty("value")]
        public T Value { get; set; }
    }
}

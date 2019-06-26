using System.Diagnostics.CodeAnalysis;
using Newtonsoft.Json;

namespace Dfc.DiscoverSkillsAndCareers.CmsFunctionApp.Models
{
    [ExcludeFromCodeCoverage]
    public class SiteFinityDataFeed<T> where T : class
    {
        [JsonProperty("value")]
        public T Value { get; set; }
    }
}

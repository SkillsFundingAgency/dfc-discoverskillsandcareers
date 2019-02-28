using System;
using Newtonsoft.Json;

namespace Dfc.DiscoverSkillsAndCareers.Models
{
    public class Content
    {
        [JsonProperty("id")]
        public string Id { get; set; }
        [JsonProperty("contentType")]
        public string ContentType { get; set; }
        [JsonProperty("contentData")]
        public string ContentData { get; set; }
        [JsonProperty("lastUpdated")]
        public DateTime LastUpdated { get; set; }
    }
}
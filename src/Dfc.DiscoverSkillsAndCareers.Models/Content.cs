using System;
using Newtonsoft.Json;

namespace Dfc.DiscoverSkillsAndCareers.Models
{
    public class Content
    {
        [JsonProperty("contentType")]
        public string ContentType { get; set; }
        [JsonProperty("contentData")]
        public string ContentData { get; set; }
    }
}
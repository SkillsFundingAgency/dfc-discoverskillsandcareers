using Newtonsoft.Json;

namespace Dfc.DiscoverSkillsAndCareers.FunctionApp.Models
{
    public class DscSession
    {
        [JsonProperty("sessionId")]
        public string SessionId { get; set; }
    }
}

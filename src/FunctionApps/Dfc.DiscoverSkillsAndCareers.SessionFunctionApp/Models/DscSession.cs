using Newtonsoft.Json;

namespace Dfc.DiscoverSkillsAndCareers.SessionFunctionApp.Models
{
    public class DscSession
    {
        [JsonProperty("sessionId")]
        public string SessionId { get; set; }
    }
}

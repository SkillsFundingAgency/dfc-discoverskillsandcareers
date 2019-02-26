using Newtonsoft.Json;

namespace Dfc.DiscoverSkillsAndCareers.AssessmentFunctionApp.Models
{
    public class DscSession
    {
        [JsonProperty("sessionId")]
        public string SessionId { get; set; }
    }
}

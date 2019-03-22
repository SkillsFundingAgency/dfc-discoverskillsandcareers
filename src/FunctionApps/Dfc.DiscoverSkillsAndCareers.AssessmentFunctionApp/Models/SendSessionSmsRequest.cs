using Newtonsoft.Json;

namespace Dfc.DiscoverSkillsAndCareers.AssessmentFunctionApp.Models
{
    public class SendSessionSmsRequest
    {
        [JsonProperty("sessionId")]
        public string SessionId { get; set; }
        [JsonProperty("mobileNumber")]
        public string MobileNumber { get; set; }
        [JsonProperty("domain")]
        public string Domain { get; set; }
        [JsonProperty("templateId")]
        public string TemplateId { get; set; }
    }
}

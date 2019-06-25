using System.Diagnostics.CodeAnalysis;
using Newtonsoft.Json;

namespace Dfc.DiscoverSkillsAndCareers.AssessmentFunctionApp.Models
{
    [ExcludeFromCodeCoverage]
    public class SendSessionEmailRequest
    {
        [JsonProperty("sessionId")]
        public string SessionId { get; set; }
        [JsonProperty("emailAddress")]
        public string EmailAddress { get; set; }
        [JsonProperty("domain")]
        public string Domain { get; set; }
        [JsonProperty("templateId")]
        public string TemplateId { get; set; }
    }
}

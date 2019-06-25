using System.Diagnostics.CodeAnalysis;
using Newtonsoft.Json;

namespace Dfc.DiscoverSkillsAndCareers.AssessmentFunctionApp.Models
{
    [ExcludeFromCodeCoverage]
    public class FilterSessionResponse
    {
        [JsonProperty("sessionId")]
        public string SessionId { get; set; }

        [JsonProperty("questionNumber")]
        public int QuestionNumber { get; set; }
    }
}

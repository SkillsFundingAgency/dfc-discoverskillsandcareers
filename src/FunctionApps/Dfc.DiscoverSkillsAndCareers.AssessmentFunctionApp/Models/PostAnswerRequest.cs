using System.Diagnostics.CodeAnalysis;
using Newtonsoft.Json;

namespace Dfc.DiscoverSkillsAndCareers.AssessmentFunctionApp.Models
{
    [ExcludeFromCodeCoverage]
    public class PostAnswerRequest
    {
        [JsonProperty("questionId")]
        public string QuestionId { get; set; }

        [JsonProperty("selectedOption")] 
        public string SelectedOption { get; set; }
    }
}

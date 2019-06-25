using System.Diagnostics.CodeAnalysis;
using Newtonsoft.Json;

namespace Dfc.DiscoverSkillsAndCareers.AssessmentFunctionApp.Models
{
    [ExcludeFromCodeCoverage]
    public class PostAnswerResponse
    {
        [JsonProperty("isSuccess")]
        public bool IsSuccess { get; set; }
        [JsonProperty("isComplete")]
        public bool IsComplete { get; set; }
        [JsonProperty("jobCategorySafeUrl")]
        public string JobCategorySafeUrl { get; set; }
        [JsonProperty("isFilterAssessment")]
        public bool IsFilterAssessment { get; set; }
        [JsonProperty("NextQuestionNumber")]
        public int NextQuestionNumber { get; set; }
    }
}

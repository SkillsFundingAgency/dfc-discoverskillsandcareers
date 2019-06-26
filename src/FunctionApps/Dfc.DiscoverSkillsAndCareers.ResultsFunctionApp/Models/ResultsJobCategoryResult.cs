using System;
using System.Diagnostics.CodeAnalysis;
using Newtonsoft.Json;

namespace Dfc.DiscoverSkillsAndCareers.ResultsFunctionApp.Models
{
    [ExcludeFromCodeCoverage]
    public class ResultsJobCategoryResult
    {
        [JsonProperty("sessionId")]
        public string SessionId { get; set; }
        [JsonProperty("jobProfiles")]
        public JobProfileResult[] JobProfiles { get; set; }
    }
}

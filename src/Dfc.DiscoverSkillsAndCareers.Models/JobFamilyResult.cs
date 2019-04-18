using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Dfc.DiscoverSkillsAndCareers.Models
{
    public class JobFamilyResult
    {
        [JsonProperty("jobFamilyCode")]
        public string JobFamilyCode { get; set; }
        [JsonProperty("jobFamilyName")]
        public string JobFamilyName { get; set; }
        [JsonProperty("jobFamilyText")]
        public string JobFamilyText { get; set; }
        [JsonProperty("jobFamilyUrl")]
        public string Url { get; set; }
        [JsonProperty("traitsTotal")]
        public int TraitsTotal { get; set; }
        [JsonProperty("total")]
        public decimal Total { get; set; }
        [JsonProperty("normalizedTotal")]
        public decimal NormalizedTotal { get; set; }
        public TraitValue[] TraitValues { get; set; } = {};
        [JsonProperty("filterAssessment")]
        public FilterAssessment FilterAssessment { get; set; }
        [JsonProperty("totalQuestions")]
        public int TotalQuestions { get; set; }
    }

    public class TraitValue
    {
        [JsonProperty("traitCode")]
        public string TraitCode { get; set; }
        [JsonProperty("total")]
        public decimal Total { get; set; }
        [JsonProperty("normalizedTotal")]
        public decimal NormalizedTotal { get; set; }
    }
}

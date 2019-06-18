using System;
using System.Collections.Generic;
using System.Diagnostics;
using Newtonsoft.Json;

namespace Dfc.DiscoverSkillsAndCareers.Models
{
    [DebuggerDisplay("JobFamily = {JobCategoryCode}")]
    public class JobCategoryResult
    {
        [JsonProperty("jobFamilyCode")] 
        public string JobCategoryCode => JobCategoryHelper.GetCode(JobCategoryName);
        
        [JsonProperty("jobFamilyName")]
        public string JobCategoryName { get; set; }
        
        [JsonProperty("jobFamilyText")]
        public string JobCategoryText { get; set; }
        
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
        public FilterAssessmentResult FilterAssessmentResult { get; set; }
        
        [JsonProperty("totalQuestions")]
        public int TotalQuestions { get; set; }
        
        [JsonIgnore]
        public string JobCategoryNameUrl => JobCategoryName?.ToLower()?.Replace(" ", "-");
    }
}

using Dfc.DiscoverSkillsAndCareers.Models;

namespace Dfc.DiscoverSkillsAndCareers.ResultsFunctionApp.Models
{
    public class ResultsResponse
    {
        public string SessionId { get; set; }
        public JobFamilyResult[] JobFamilies { get; set; }
        public string[] Traits { get; set; }
        public int JobFamilyCount { get; set; }
        public int JobFamilyMoreCount { get; set; }
        public string AssessmentType { get; set; }
        public JobProfileResult[] JobProfiles { get; set; }
        public string[] WhatYouToldUs { get; set; }
    }
}

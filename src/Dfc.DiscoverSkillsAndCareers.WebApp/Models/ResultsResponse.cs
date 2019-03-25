using Dfc.DiscoverSkillsAndCareers.Models;
using System.Collections.Generic;

namespace Dfc.DiscoverSkillsAndCareers.WebApp.Models
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
    }
}

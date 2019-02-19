using System.Collections.Generic;
using Dfc.DiscoverSkillsAndCareers.Models;

namespace Dfc.DiscoverSkillsAndCareers.WebApp.Models
{
    public class ResultsViewModel
    {
        public string SessionId { get; set; }
        public List<JobFamilyResult> JobFamilies { get; set; }
        public List<string> Traits { get; set; }
        public int JobFamilyCount { get; set; }
        public int JobFamilyMoreCount { get; set; }
        public string AssessmentType { get; set; }
    }
}

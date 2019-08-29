using Dfc.DiscoverSkillsAndCareers.Models;
using System.Collections.Generic;

namespace Dfc.DiscoverSkillsAndCareers.CmsFunctionApp.Models
{
    public class JobCategorySkillMappingResult
    {
        public string JobCategory { get; set; }

        public List<JobProfileSkillMapping> SkillMappings { get; set; } = new List<JobProfileSkillMapping>();
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using Dfc.DiscoverSkillsAndCareers.Models;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Dfc.DiscoverSkillsAndCareers.CmsFunctionApp.Models
{
    public class JobCategorySkillMappingResult
    {
        public string JobCategory { get; set; }
        
        public List<JobProfileSkillMapping> SkillMappings { get; set; } = new List<JobProfileSkillMapping>();
        
    }
}
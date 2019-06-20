using System.Collections.Generic;

namespace Dfc.DiscoverSkillsAndCareers.Models
{
    public class JobProfileSkillMapping
    {
        public string ONetAttribute { get; set; }
        public List<JobProfileMapping> JobProfiles { get; set; } = new List<JobProfileMapping>();

    }
}
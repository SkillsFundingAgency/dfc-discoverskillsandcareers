using System;
using System.Collections.Generic;
using System.Linq;

namespace Dfc.DiscoverSkillsAndCareers.CmsFunctionApp.Models
{
    public class SiteFinityJobProfile
    {
        public Guid Id { get; set; }
        
        public string Title { get; set; }

        public Guid[] JobProfileCategories { get; set; }
        
        public SiteFinityONetSkill[] RelatedSkills { get; set; }
        
        public SiteFinityONetSkill[] Skills(HashSet<string> skillsToRemove)
        {
            return RelatedSkills
                .Where(o =>
                    !o.ONetAttributeType.Equals("knowledge", StringComparison.InvariantCultureIgnoreCase)
                    && !skillsToRemove.Contains(o.Skill))
                .Take(8).ToArray();
        }

        public bool HasSkill(HashSet<string> skillsToRemove, string skill)
        {
            return Skills(skillsToRemove).Any(s => s.Skill.Equals(skill, StringComparison.InvariantCultureIgnoreCase));
        }
    }
}
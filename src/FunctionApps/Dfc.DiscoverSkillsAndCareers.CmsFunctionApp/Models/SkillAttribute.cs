using System;
using System.Collections.Generic;

namespace Dfc.DiscoverSkillsAndCareers.CmsFunctionApp.Models
{
    public class SkillAttribute
    {
        public string ONetAttribute { get; set; }
        public string ONetAttributeType { get; set; }
        public double CompositeRank { get; set; }
        public double ONetRank { get; set; }
        public double NcsRank { get; set; }
        public int TotalProfilesWithSkill { get; set; }
        public double PercentageProfileWithSkill { get; set; }

        public HashSet<string> ProfilesWithSkill { get; set;  } 

    }
}
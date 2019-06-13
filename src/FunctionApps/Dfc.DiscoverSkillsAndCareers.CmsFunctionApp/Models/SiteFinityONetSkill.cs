using System;
using System.Linq;
using System.Text;

namespace Dfc.DiscoverSkillsAndCareers.CmsFunctionApp.Models
{
    public class SiteFinityONetSkill : IEquatable<SiteFinityONetSkill>
    {
        public string Title { get; set; }
        
        public string ONetAttributeType { get; set; }
        
        public int Rank { get; set; }
        
        public double ONetRank { get; set; }

        public string Skill => Title?.Split(new[] {'-'}, StringSplitOptions.RemoveEmptyEntries).ElementAtOrDefault(1);

        public bool Equals(SiteFinityONetSkill other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return string.Equals(Skill, other.Skill);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((SiteFinityONetSkill) obj);
        }

        public override int GetHashCode()
        {
            return (Skill != null ? Skill.GetHashCode() : 0);
        }
    }
}

using System.Diagnostics.CodeAnalysis;

namespace Dfc.DiscoverSkillsAndCareers.CmsFunctionApp.Models
{
    [ExcludeFromCodeCoverage]
    public class JobCategorySkills
    {
        public int JobCategoryProfileCount { get; set; }
            
        public SkillAttribute[] SkillAttributes { get; set; }
        public string CategoryName { get; set; }
    }
}
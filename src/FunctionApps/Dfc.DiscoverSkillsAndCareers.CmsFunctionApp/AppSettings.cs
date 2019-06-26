using System;
using System.Diagnostics.CodeAnalysis;
using Dfc.DiscoverSkillsAndCareers.Repositories;

namespace Dfc.DiscoverSkillsAndCareers.CmsFunctionApp
{

    [ExcludeFromCodeCoverage]
    public class AppSettings
    {
        public double MaxPercentageOfProfileOccurenceForSkill { get; set; } = 0.75;
        public double MaxPercentageDistributionOfJobProfiles { get; set; } = 0.75;

        public string SiteFinityJobCategoriesTaxonomyId { get; set; } = "Job Profile Category";
    }
}

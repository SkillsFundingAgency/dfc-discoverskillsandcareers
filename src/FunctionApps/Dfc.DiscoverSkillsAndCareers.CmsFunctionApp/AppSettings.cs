namespace Dfc.DiscoverSkillsAndCareers.CmsFunctionApp
{
    public class AppSettings
    {
        public double MaxPercentageOfProfileOccurenceForSkill { get; set; } = 0.75;
        public double MaxPercentageDistributionOfJobProfiles { get; set; } = 0.75;

        public string SiteFinityJobCategoriesTaxonomyId { get; set; } = "Job Profile Category";
    }
}
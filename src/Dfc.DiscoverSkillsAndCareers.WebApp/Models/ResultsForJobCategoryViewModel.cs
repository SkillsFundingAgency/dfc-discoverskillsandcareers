namespace Dfc.DiscoverSkillsAndCareers.WebApp.Models
{
    public class ResultsForJobCategoryViewModel
    {
        public string SessionId { get; set; }
        public JobProfile[] JobProfiles { get; set; } = {}
        public string Title { get; set; } = "National Careers Service - Results";
        public string TraitTitle { get; set; } = "What you told us";
        public string TraitSummaryText { get; set; } = "Because of your answers, we have provided the following job profiles. You can choose to view more results at the bottom of the page.";
        public string Headline { get; set; } = "Job profiles you might be suited to";
        public string SaveProgressLinkText { get; set; } = "Save my progress";
        public string SeeLinkButtonText { get; set; } = "See job profile";
        public string SaveProgressTitle { get; set; } = "Return to this later";
    }
}

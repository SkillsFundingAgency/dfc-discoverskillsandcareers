namespace Dfc.DiscoverSkillsAndCareers.WebApp.Models
{
    public class ResultsForJobCategoryViewModel
    {
        public string SessionId { get; set; }
        public JobProfile[] JobProfiles { get; set; } = {};
        public string Title { get; set; } 
        public string TraitTitle { get; set; } 
        public string TraitSummaryText { get; set; } 
        public string Headline { get; set; } 
        public string SaveProgressText { get; set; } 
        public string SeeLinkButtonText { get; set; } 
        public string SaveProgressTitle { get; set; } 
    }
}

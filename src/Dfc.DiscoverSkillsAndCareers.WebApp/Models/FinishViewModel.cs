namespace Dfc.DiscoverSkillsAndCareers.WebApp.Models
{
    public class FinishViewModel
    {
        public string SessionId { get; set; }
        public string JobCategorySafeUrl { get; set; }
        public bool IsFilterAssessment { get; set; }
        public string Title { get; set; } = "Assessment complete";
        public string BreadcrumbText { get; set; } = "Assessment";
        public string Content { get; set; } = "See the types of jobs that might suit you. Your results are based on your responses.";
        public string Headline { get; set; } = "Assessment complete";
        public string ViewResultsButtonText { get; set; } = "See results";
        public string ContentTitle { get; set; } = "What to do next";
    }
}

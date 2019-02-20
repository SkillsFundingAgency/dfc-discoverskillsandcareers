namespace Dfc.DiscoverSkillsAndCareers.WebApp.Models
{
    public class QuestionViewModel
    {
        public string QuestionId { get; set; }
        public string QuestionText { get; set; }
        public int QuestionNumber { get; set; }
        public string TraitCode { get; set; }
        public string FormRoute { get; set; }
        public string SessionId { get; set; }
        public string ErrorMessage { get; set; }
        public string Percentage { get; set; }
        public string PercentrageLeft { get; set; }
        public string Code { get; set; }
        public string Title { get; set; } = "National Careers Service - Statement";
        public string SaveProgressLinkText { get; set; } = "Save my progress";
        public string StronglyDisagree { get; set; } = "Strongly disagree";
        public string StronglyAgree { get; set; } = "Strongly agree";
        public string SaveProgressTitle { get; set; } = "Return to this later";
        public string Neutral { get; set; } = "This doesn't apply to me";
        public string Agree { get; set; } = "Agree";
        public string NextButtonText { get; set; } = "Next";
        public string Disagree { get; set; } = "Disagree";

    }
}

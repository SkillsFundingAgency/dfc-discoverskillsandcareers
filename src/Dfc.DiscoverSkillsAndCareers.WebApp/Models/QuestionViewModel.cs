using Dfc.DiscoverSkillsAndCareers.Models;

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
        public bool IsFilterAssessment { get; set; }
        public AnswerOption? RecordedAnswer { get; set; }
        public string NoAnswerErrorMessage { get; set; } = "Choose an answer to the statement";
        public string AssessmentType { get; set; }

        public string PageTitle => $"Q{QuestionNumber}";
    }
}

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
        public string Title { get; set; } = "National Careers Service - Assessment Statement";
        public string Breadcrumb { get; set; } = "Assessment";
        public string SaveProgressText { get; set; } = "Save my progress";
        public string StronglyDisagreeText { get; set; } = "Strongly disagree";
        public string StronglyAgreeText { get; set; } = "Strongly agree";
        public string SaveProgressTitle { get; set; } = "Return to this later";
        public string NeutralText { get; set; } = "It depends";
        public string AgreeText { get; set; } = "Agree";
        public string NextButtonText { get; set; } = "Next";
        public string DisagreeText { get; set; } = "Disagree";
        public string YesText { get; set; } = "Yes";
        public string NoText { get; set; } = "No";
        public string NoAnswerErrorMessage { get; set; } = "Please select an option below to continue";
        public string AssessmentType { get; set; }
    }
}

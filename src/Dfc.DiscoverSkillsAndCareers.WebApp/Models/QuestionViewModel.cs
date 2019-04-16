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
        public string Title { get; set; }
        public string Breadcrumb { get; set; }
        public string SaveProgressText { get; set; }
        public string StronglyDisagreeText { get; set; }
        public string StronglyAgreeText { get; set; }
        public string SaveProgressTitle { get; set; }
        public string NeutralText { get; set; }
        public string AgreeText { get; set; }
        public string NextButtonText { get; set; }
        public string DisagreeText { get; set; }
        public string YesText { get; set; }
        public string NoText { get; set; }
        public string NoAnswerErrorMessage { get; set; }
    }
}

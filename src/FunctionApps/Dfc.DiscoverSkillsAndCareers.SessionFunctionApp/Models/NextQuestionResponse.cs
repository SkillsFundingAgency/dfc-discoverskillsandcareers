namespace Dfc.DiscoverSkillsAndCareers.SessionFunctionApp.Models
{
    public class NextQuestionResponse
    {
        public string QuestionText { get; set; }
        public string TraitCode { get; set; }
        public string QuestionId { get; set; }
        public int QuestionNumber { get; set; }
        public string SessionId { get; set; }
        public int PercentComplete { get; set; }
        public int? NextQuestionNumber { get; set; }
        public bool IsComplete { get; set; }
    }
}

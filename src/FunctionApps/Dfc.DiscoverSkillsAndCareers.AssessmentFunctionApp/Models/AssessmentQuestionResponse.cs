using System;
using Dfc.DiscoverSkillsAndCareers.Models;

namespace Dfc.DiscoverSkillsAndCareers.AssessmentFunctionApp.Models
{
    public class AssessmentQuestionResponse
    {
        public string QuestionText { get; set; }
        public string TraitCode { get; set; }
        public string QuestionId { get; set; }
        public int QuestionNumber { get; set; }
        public string SessionId { get; set; }
        public int PercentComplete { get; set; }
        public int? NextQuestionNumber { get; set; }
        public bool IsComplete { get; set; }
        public string ReloadCode { get; set; }
        public DateTime StartedDt { get; set; }
        public int RecordedAnswersCount { get; set; }
        public int MaxQuestionsCount { get; set; }
        public string CurrentFilterAssessmentCode { get; set; }
        public bool IsFilterAssessment { get; set; }
        public string JobCategorySafeUrl { get; set; }
        public AnswerOption? RecordedAnswer { get; set; }
    }
}

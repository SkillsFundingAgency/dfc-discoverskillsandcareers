using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Dfc.DiscoverSkillsAndCareers.ChangeFeed.Data.Entities
{
    [Table("UserSession_Answer")]
    public class UmAnswer
    {
        public string Id { get; set; }
        public string UserSessionId { get; set; }
        public string QuestionId { get; set; }
        public string QuestionNumber { get; set; }
        public string QuestionText { get; set; }
        public string TraitCode { get; set; }
        public string SelectedOption { get; set; }
        public DateTime AnsweredDt { get; set; }
        public bool IsNegative { get; set; }
        public string QuestionSetVersion { get; set; }
    }
}

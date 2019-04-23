using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Dfc.DiscoverSkillsAndCareers.ChangeFeed.Data.Entities
{
    [Table("UserSession")]
    public class UmUserSession
    {
        [Key]
        public string Id { get; set; }
        public DateTime LastUpdatedDt { get; set; }
        public string LanguageCode { get; set; }
        public string QuestionSetVersion { get; set; }
        public int MaxQuestions { get; set; }
        public int CurrentQuestion { get; set; }
        public bool IsComplete { get; set; }
        public DateTime StartedDt { get; set; }
        public DateTime? CompleteDt { get; set; }
        public string AssessmentType { get; set; }
        public string CurrentFilterAssessmentCode { get; set; }
    }
}

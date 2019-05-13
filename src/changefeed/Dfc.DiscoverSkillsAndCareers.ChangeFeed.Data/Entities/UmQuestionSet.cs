using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Dfc.DiscoverSkillsAndCareers.ChangeFeed.Data.Entities
{
    [Table("QuestionSet")]
    public class UmQuestionSet
    {
        [Key]
        public string Id { get; set; }
        public int Version { get; set; }
        public string AssessmentType { get; set; }
        public int MaxQuestions { get; set; }
        public string Title { get; set; }
        public DateTimeOffset LastUpdatedDt { get; set; }
    }
}

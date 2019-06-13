using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Dfc.DiscoverSkillsAndCareers.ChangeFeed.Data.Entities
{
    [Table("Question_JobProfile")]
    public class UmQuestionJobProfile
    {
        [Key]
        public string Id { get; set; }
        public string QuestionId { get; set; }
        public string JobProfile { get; set; }
        
        public bool Included { get; set; }
    }
}

using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Dfc.DiscoverSkillsAndCareers.ChangeFeed.Data.Entities
{
    [Table("UserSession_SuggestedJobCategory")]
    public class UmSuggestedJobCategory
    {
        public string Id { get; set; }
        public string UserSessionId { get; set; }
        public string JobCategoryCode { get; set; }
        public string JobCategory { get; set; }
        public int TraitTotal { get; set; }
        public decimal JobCategoryScore { get; set; }
        public bool HasCompletedFilterAssessment { get; set; }
        public int FilterAssessmentQuestions { get; set; }
    }
}

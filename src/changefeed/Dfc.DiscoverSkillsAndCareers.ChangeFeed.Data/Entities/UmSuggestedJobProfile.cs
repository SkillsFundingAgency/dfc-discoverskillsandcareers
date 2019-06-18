using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Dfc.DiscoverSkillsAndCareers.ChangeFeed.Data.Entities
{
    [Table("UserSession_SuggestedJobProfile")]
    public class UmSuggestedJobProfile
    {
        public string Id { get; set; }
        public string UserSessionId { get; set; }
        public string JobCategoryCode { get; set; }
        public string JobProfile { get; set; }
    }
}

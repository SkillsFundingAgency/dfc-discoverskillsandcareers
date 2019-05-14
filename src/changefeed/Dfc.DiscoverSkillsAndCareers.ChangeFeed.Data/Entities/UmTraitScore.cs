using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Dfc.DiscoverSkillsAndCareers.ChangeFeed.Data.Entities
{
    [Table("UserSession_TraitScore")]
    public class UmTraitScore
    {
        public string Id { get; set; }
        public string UserSessionId { get; set; }
        public string Trait { get; set; }
        public int Score { get; set; }
    }
}

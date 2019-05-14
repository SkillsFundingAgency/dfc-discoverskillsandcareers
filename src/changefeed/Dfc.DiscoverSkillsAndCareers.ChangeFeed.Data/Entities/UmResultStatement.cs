using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Dfc.DiscoverSkillsAndCareers.ChangeFeed.Data.Entities
{
    [Table("UserSession_ResultStatement")]
    public class UmResultStatement
    {
        public string Id { get; set; }
        public string UserSessionId { get; set; }
        public string TextDisplayed { get; set; }
        public bool IsTrait { get; set; }
        public bool IsFilter { get; set; }
    }
}

using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Dfc.DiscoverSkillsAndCareers.ChangeFeed.Data.Entities
{
    [Table("Question")]
    public class UmQuestion
    {
        [Key]
        public string Id { get; set; }
        public string Text { get; set; }
        public string TraitCode { get; set; }
        public bool IsNegative { get; set; }
        public int Order { get; set; }
        public string FilterTrigger { get; set; }
        public string SfId { get; set; }
        public string PositiveResultDisplayText { get; set; }
        public string NegativeResultDisplayText { get; set; }
        public DateTime LastUpdatedDt { get; set; }
    }
}

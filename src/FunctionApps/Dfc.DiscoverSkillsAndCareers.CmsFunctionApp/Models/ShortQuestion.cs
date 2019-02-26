namespace Dfc.DiscoverSkillsAndCareers.CmsFunctionApp.Models
{
    public class ShortQuestion
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public string Trait { get; set; }
        public int Order { get; set; }
        public bool IsNegative { get; set; }
    }
}

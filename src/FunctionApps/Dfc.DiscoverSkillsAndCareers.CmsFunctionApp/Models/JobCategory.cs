using System.Collections.Generic;

namespace Dfc.DiscoverSkillsAndCareers.CmsFunctionApp.Models
{
    public class JobCategory
    {
        public string Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string UrlName { get; set; }
        public List<string> Traits { get; set; }
    }
}

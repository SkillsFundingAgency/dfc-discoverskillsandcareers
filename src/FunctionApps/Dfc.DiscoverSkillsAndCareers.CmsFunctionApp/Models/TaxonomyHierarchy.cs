using System;

namespace Dfc.DiscoverSkillsAndCareers.CmsFunctionApp.Models
{
    public class TaxonomyHierarchy
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string UrlName { get; set; }
        public Guid TaxonomyId { get; set; }
    }
}

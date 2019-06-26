using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Dfc.DiscoverSkillsAndCareers.CmsFunctionApp.Models
{
    [ExcludeFromCodeCoverage]
    public class SiteFinityJobCategory
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string UrlName { get; set; }
        public List<string> Traits { get; set; }
        public Guid TaxonomyId { get; set; }
    }
}

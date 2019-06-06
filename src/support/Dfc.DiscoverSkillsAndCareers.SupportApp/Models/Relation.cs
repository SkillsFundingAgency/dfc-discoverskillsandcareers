using System;

namespace Dfc.DiscoverSkillsAndCareers.SupportApp.Models
{
    public class Relation
    {
        public RelationType RelatedType { get; set; }
        
        
        public string[] Values { get; set; }

        public string Property => RelatedType.Property;
        
        public string ContentType => RelatedType.ContentType;
        public bool IsTaxonomy => RelatedType.Type.Equals("taxonomies", StringComparison.InvariantCultureIgnoreCase);
        

    }
}
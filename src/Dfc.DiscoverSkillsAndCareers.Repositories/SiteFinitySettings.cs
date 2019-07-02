using System.Diagnostics.CodeAnalysis;

namespace Dfc.DiscoverSkillsAndCareers.Repositories
{
    [ExcludeFromCodeCoverage]
    public class SiteFinitySettings
    {
        public string SiteFinityApiUrlBase { get; set; }
        public string SiteFinityApiWebService { get; set; }
        public bool SiteFinityRequiresAuthentication { get; set; }
        public string SiteFinityApiAuthenicationEndpoint { get; set;}
        public string SiteFinityScope { get; set; }
        public string SiteFinityUsername { get; set; }
        public string SiteFinityPassword { get; set; }
        public string SiteFinityClientId { get; set; }
        public string SiteFinityClientSecret { get; set; }
    }
}
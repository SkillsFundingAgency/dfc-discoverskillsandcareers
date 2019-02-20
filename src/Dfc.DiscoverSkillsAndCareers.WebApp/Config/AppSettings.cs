using Dfc.DiscoverSkillsAndCareers.Repositories;

namespace Dfc.DiscoverSkillsAndCareers.WebApp.Config
{
    public class AppSettings : IAppSettings
    {
        public CosmosSettings CosmosSettings { get; set; } = new CosmosSettings();
        public BlobStorageSettings BlobStorage { get; set; } = new BlobStorageSettings();
        public string StaticSiteDomain { get; set; }
        public string ContentApiRoot { get; set; }
        public string SessionApiRoot { get; set; }
        public string ResultsApiRoot { get; set; }
    }
}

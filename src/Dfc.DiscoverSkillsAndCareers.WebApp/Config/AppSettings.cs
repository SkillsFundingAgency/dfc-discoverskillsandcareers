using Dfc.DiscoverSkillsAndCareers.Repositories;

namespace Dfc.DiscoverSkillsAndCareers.WebApp.Config
{
    public class AppSettings : IAppSettings
    {
        public CosmosSettings CosmosSettings { get; set; } = new CosmosSettings();
        public BlobStorageSettings BlobStorage { get; set; } = new BlobStorageSettings();
        public string StaticSiteDomain { get; set; }
    }
}

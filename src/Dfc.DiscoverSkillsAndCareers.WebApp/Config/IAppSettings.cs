using Dfc.DiscoverSkillsAndCareers.Repositories;

namespace Dfc.DiscoverSkillsAndCareers.WebApp.Config
{
    public interface IAppSettings
    {
        CosmosSettings CosmosSettings { get; set; }
        BlobStorageSettings BlobStorage { get; set; }
        string StaticSiteDomain { get; set; } // TODO: no longer needed
    }
}

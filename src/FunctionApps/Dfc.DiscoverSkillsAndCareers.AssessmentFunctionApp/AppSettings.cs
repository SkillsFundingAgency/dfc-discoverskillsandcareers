using System;
using Dfc.DiscoverSkillsAndCareers.Repositories;

namespace Dfc.DiscoverSkillsAndCareers.AssessmentFunctionApp
{
    public class AppSettings
    {
        [Obsolete()]
        public CosmosSettings CosmosSettings { get; set; } = new CosmosSettings();
        [Obsolete()]
        public BlobStorageSettings BlobStorage { get; set; } = new BlobStorageSettings();
        [Obsolete()]
        public string StaticSiteDomain { get; set; }
        public string SessionSalt { get; set; }
    }

    [Obsolete()]
    public class BlobStorageSettings
    {
        public string StorageConnectionString { get; set; }
        public string ContainerName { get; set; }
    }
}

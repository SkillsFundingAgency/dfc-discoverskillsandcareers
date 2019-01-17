using System;
using Dfc.DiscoverSkillsAndCareers.Repositories;

namespace Dfc.DiscoverSkillsAndCareers.FunctionApp
{
    public class AppSettings
    {
        public CosmosSettings CosmosSettings { get; set; } = new CosmosSettings();
        public BlobStorageSettings BlobStorage { get; set; } = new BlobStorageSettings();
    }

    public class BlobStorageSettings
    {
        public string StorageConnectionString { get; set; }
        public string ContainerName { get; set; }
    }
}

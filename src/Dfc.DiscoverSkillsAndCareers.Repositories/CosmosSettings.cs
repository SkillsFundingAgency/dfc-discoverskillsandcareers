namespace Dfc.DiscoverSkillsAndCareers.Repositories
{
    public class CosmosSettings : ICosmosSettings
    {
        public string Endpoint { get; set; }
        public string Key { get; set; }
        public string DatabaseName { get; set; }
    }
}
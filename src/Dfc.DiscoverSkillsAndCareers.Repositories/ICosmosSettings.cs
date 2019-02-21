namespace Dfc.DiscoverSkillsAndCareers.Repositories
{
    public interface ICosmosSettings
    {
        string Endpoint { get; set; }
        string Key { get; set; }
        string DatabaseName { get; set; }
    }
}

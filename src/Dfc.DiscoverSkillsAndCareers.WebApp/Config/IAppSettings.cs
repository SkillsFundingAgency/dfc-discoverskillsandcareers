namespace Dfc.DiscoverSkillsAndCareers.WebApp.Config
{
    public interface IAppSettings
    {
        string ContentApiRoot { get; set; }
        string SessionApiRoot { get; set; }
        string ResultsApiRoot { get; set; }
    }
}

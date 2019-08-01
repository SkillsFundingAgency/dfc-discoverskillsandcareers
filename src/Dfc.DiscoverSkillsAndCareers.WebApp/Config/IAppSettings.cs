namespace Dfc.DiscoverSkillsAndCareers.WebApp.Config
{
    public interface IAppSettings
    {
        string ContentApiRoot { get; set; }
        string SessionApiRoot { get; set; }
        string ResultsApiRoot { get; set; }
        bool UseFilteringQuestions { get; set; }
        string NotifyEmailTemplateId { get; set; }
        string NotifySmsTemplateId { get; set; }
        string APIAuthorisationCode { get; set; }
        string ExploreCareersBaseUrl { get; set; }
        string APIRootSegment { get; set; }
    }
}

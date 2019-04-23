namespace Dfc.DiscoverSkillsAndCareers.WebApp.Config
{
    public interface IAppSettings
    {
        string ContentApiRoot { get; set; }
        string SessionApiRoot { get; set; }
        string ResultsApiRoot { get; set; }
        string AssessmentQuestionSetNames { get; set; }
        bool UseFilteringQuestions { get; set; }
        string NotifyEmailTemplateId { get; set; }
        string NotifySmsTemplateId { get; set; }
        string ContentApiAuthorisationCode { get; set; }
        string SessionApiAuthorisationCode { get; set; }
        string ResultsApiAuthorisationCode { get; set; }
    }
}

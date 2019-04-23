namespace Dfc.DiscoverSkillsAndCareers.WebApp.Config
{
    public class AppSettings : IAppSettings
    {
        public string ContentApiRoot { get; set; }
        public string SessionApiRoot { get; set; }
        public string ResultsApiRoot { get; set; }
        public string AssessmentQuestionSetNames { get; set; }
        public bool UseFilteringQuestions { get; set; }
        public string NotifyEmailTemplateId { get; set; }
        public string NotifySmsTemplateId { get; set; }
        public string ContentApiAuthorisationCode { get; set; }
        public string SessionApiAuthorisationCode { get; set; }
        public string ResultsApiAuthorisationCode { get; set; }
    }
}

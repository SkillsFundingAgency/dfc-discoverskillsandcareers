namespace Dfc.DiscoverSkillsAndCareers.WebApp.Config
{
    public class AppSettings : IAppSettings
    {
        public string ContentApiRoot { get; set; }
        public string SessionApiRoot { get; set; }
        public string ResultsApiRoot { get; set; }
        public string AssessmentQuestionSetNames { get; set; }
        public bool UseFilteringQuestions { get; set; }
    }
}

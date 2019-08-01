using System.Diagnostics.CodeAnalysis;

namespace Dfc.DiscoverSkillsAndCareers.WebApp.Config
{
    [ExcludeFromCodeCoverage]
    public class AppSettings : IAppSettings
    {
        public string ContentApiRoot { get; set; }
        public string SessionApiRoot { get; set; }
        public string ResultsApiRoot { get; set; }
        public bool UseFilteringQuestions { get; set; }
        public string NotifyEmailTemplateId { get; set; }
        public string NotifySmsTemplateId { get; set; }
        public string APIAuthorisationCode { get; set; }
        public string ExploreCareersBaseUrl { get; set; }
        public string AssetsCDNUrl { get; set; }
        public string APIRootSegment { get; set; }
    }
}
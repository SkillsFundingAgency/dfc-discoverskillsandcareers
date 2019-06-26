using System.Diagnostics.CodeAnalysis;

namespace Dfc.DiscoverSkillsAndCareers.AssessmentFunctionApp
{
    [ExcludeFromCodeCoverage]
    public class AppSettings
    {
        public string SessionSalt { get; set; }
        public string NotifyApiKey { get; set; }
    }

}

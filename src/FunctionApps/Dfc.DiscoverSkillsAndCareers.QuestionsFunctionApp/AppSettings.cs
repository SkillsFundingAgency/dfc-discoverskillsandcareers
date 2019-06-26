using System;
using System.Diagnostics.CodeAnalysis;
using Dfc.DiscoverSkillsAndCareers.Repositories;

namespace Dfc.DiscoverSkillsAndCareers.QuestionsFunctionApp
{
    [ExcludeFromCodeCoverage]
    public class AppSettings
    {
        public string SessionSalt { get; set; }
    }

}

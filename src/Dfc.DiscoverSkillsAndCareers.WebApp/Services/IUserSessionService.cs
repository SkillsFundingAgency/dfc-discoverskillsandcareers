using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace Dfc.DiscoverSkillsAndCareers.Services
{
    public interface IUserSessionService
    {
        string TryGetSessionId(HttpRequestMessage request);
        Task CreateSession(string questionSetVersion, int maxQuestions, string languageCode = "en");
        Task UpdateSession();
        Task Reload(string code);
        Task Init(HttpRequestMessage request);
    }
}

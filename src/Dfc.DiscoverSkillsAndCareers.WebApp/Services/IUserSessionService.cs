using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Dfc.DiscoverSkillsAndCareers.Models;
using Microsoft.AspNetCore.Http;

namespace Dfc.DiscoverSkillsAndCareers.Services
{
    public interface IUserSessionService
    {
        string TryGetSessionId(HttpRequest request);
        Task CreateSession(string questionSetVersion, int maxQuestions, string languageCode = "en");
        Task UpdateSession();
        Task Reload(string code);
        Task Init(HttpRequest request);
        bool HasSession { get; }
        bool HasInputError { get; }
        UserSession Session { get; }
        string GetFormValue(string key);
        int? GetFormNumberValue(string key);
    }
}

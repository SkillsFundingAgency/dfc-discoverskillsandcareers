using System;
using System.Threading.Tasks;
using Dfc.DiscoverSkillsAndCareers.Models;

namespace Dfc.DiscoverSkillsAndCareers.Repositories
{
    public class UserSessionRepository
    {
        public async Task<UserSession> GetUserSessionAsync(string sessionId)
        {
            return new UserSession()
            {
                SessionId = sessionId
            };
        }
    }
}

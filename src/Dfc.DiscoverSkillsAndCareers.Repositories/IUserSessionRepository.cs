using System.Threading.Tasks;
using Dfc.DiscoverSkillsAndCareers.Models;
using Microsoft.Azure.Documents;

namespace Dfc.DiscoverSkillsAndCareers.Repositories
{
    public interface IUserSessionRepository
    {
        Task<UserSession> GetUserSession(string primaryKey);
        Task<UserSession> GetUserSession(string userSessionId, string partitionKey);
        Task CreateUserSession(UserSession userSession);
        Task UpdateUserSession(UserSession userSession);
    }
}

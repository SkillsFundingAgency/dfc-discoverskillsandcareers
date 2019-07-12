using System.Threading.Tasks;
using Dfc.DiscoverSkillsAndCareers.Models;
using Microsoft.Azure.Documents;

namespace Dfc.DiscoverSkillsAndCareers.Repositories
{
    public interface IUserSessionRepository
    {
        Task<UserSession> GetUserSession(string primaryKey, bool byPassCache = true);
        Task CreateUserSession(UserSession userSession);
        Task UpdateUserSession(UserSession userSession);
    }
}

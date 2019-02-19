using System.Threading.Tasks;
using Dfc.DiscoverSkillsAndCareers.Models;
using Dfc.DiscoverSkillsAndCareers.Repositories;
using Microsoft.Azure.Documents;

namespace Dfc.UnitTests.Fakes
{
    public class FakeUserSessionRepository : IUserSessionRepository
    {
        public Task CreateUserSession(UserSession userSession)
        {
            return Task.CompletedTask;
        }

        public Task<UserSession> GetUserSession(string primaryKey)
        {
            var result = new UserSession()
            {

            };
            return Task.FromResult<UserSession>(result);
        }

        public Task<UserSession> GetUserSession(string userSessionId, string partitionKey)
        {
            throw new System.NotImplementedException();
        }

        public Task<UserSession> UpdateUserSession(UserSession userSession)
        {
            return Task.FromResult<UserSession>(userSession);
        }
    }
}

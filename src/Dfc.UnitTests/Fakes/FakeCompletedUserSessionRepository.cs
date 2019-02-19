using System.Threading.Tasks;
using Dfc.DiscoverSkillsAndCareers.Models;
using Dfc.DiscoverSkillsAndCareers.Repositories;
using Microsoft.Azure.Documents;

namespace Dfc.UnitTests.Fakes
{
    public class FakeCompletedUserSessionRepository : IUserSessionRepository
    {
        public Task<Document> CreateUserSession(UserSession userSession)
        {
            throw new System.NotImplementedException();
        }

        public Task<UserSession> GetUserSession(string primaryKey)
        {
            var result = new UserSession()
            {
                ResultData = new ResultData()
                {
                    
                }
            };
            return Task.FromResult<UserSession>(result);
        }

        public Task<UserSession> GetUserSession(string userSessionId, string partitionKey)
        {
            throw new System.NotImplementedException();
        }

        public Task<UserSession> UpdateUserSession(UserSession userSession)
        {
            throw new System.NotImplementedException();
        }
    }
}

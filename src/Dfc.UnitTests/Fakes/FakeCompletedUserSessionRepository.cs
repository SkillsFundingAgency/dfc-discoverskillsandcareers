using Dfc.DiscoverSkillsAndCareers.Models;
using Dfc.DiscoverSkillsAndCareers.Repositories;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Dfc.UnitTests.Fakes
{
    public class FakeCompletedUserSessionRepository : IUserSessionRepository
    {
        public Task CreateUserSession(UserSession userSession)
        {
            return Task.CompletedTask;
        }

        public Task<UserSession> GetUserSession(string primaryKey)
        {
            var result = new UserSession()
            {
                ResultData = new ResultData()
                {
                    Traits = new TraitResult[]{},
                    JobFamilies = new JobFamilyResult[]{},
                }
            };
            return Task.FromResult<UserSession>(result);
        }

        public Task<UserSession> GetUserSession(string userSessionId, string partitionKey)
        {
            throw new System.NotImplementedException();
        }

        public Task UpdateUserSession(UserSession userSession)
        {
            throw new System.NotImplementedException();
        }
    }
}

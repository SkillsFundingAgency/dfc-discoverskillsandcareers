using System;

namespace Dfc.DiscoverSkillsAndCareers.FunctionApp
{
    public static class SessionIdHelper
    {
        public static string GenerateSessionId()
        {
            // TODO: 
            return Guid.NewGuid().ToString();
        }
    }
}

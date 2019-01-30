using System;
using HashidsNet;

namespace Dfc.DiscoverSkillsAndCareers.FunctionApp.Helpers
{
    public static class SessionIdHelper
    {
        public static string GenerateSessionId(string salt)
        {
            var hashids = new Hashids(salt, 4, "acefghjkmnrstwxyz23456789");
            long digits = Convert.ToInt64(DateTime.Now.ToString("yyMMddHHmmssf"));
            var hash = hashids.EncodeLong(digits);
            return hash;
        }
    }
}

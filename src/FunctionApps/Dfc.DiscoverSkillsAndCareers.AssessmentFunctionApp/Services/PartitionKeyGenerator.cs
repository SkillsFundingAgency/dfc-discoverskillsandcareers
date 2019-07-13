using System;
using System.Security.Cryptography;
using System.Text;

namespace Dfc.DiscoverSkillsAndCareers.AssessmentFunctionApp.Services
{
    public static class PartitionKeyGenerator
    {
        private static readonly MD5 _md5 = MD5.Create();
        private static string Create(string prefix, string id, int numberOfPartitions)
        {
            var hashedValue = _md5.ComputeHash(Encoding.UTF8.GetBytes(id));
            var asInt = BitConverter.ToInt32(hashedValue, 0);
            asInt = asInt == int.MinValue ? asInt + 1 : asInt;
            return $"{prefix}{Math.Abs(asInt) % numberOfPartitions}";
        }

        public static string UserSession(string id)
        {
            return Create("session", id, 20);
        }
    }
}
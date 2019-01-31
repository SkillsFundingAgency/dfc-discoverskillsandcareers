using System;
using Xunit;
using HashidsNet;
using System.Threading;
using System.Collections.Generic;

namespace Dfc.UnitTests
{
    public class SessionIdTests
    {
        [Fact]
        public void GenerateHash_ShouldAllBeUnique()
        {
            int amount = 100;
            var codes = new List<string>();
            for (var i = 0; i < amount; i++)
            {
                string salt = Guid.NewGuid().ToString();
                var hash = Dfc.DiscoverSkillsAndCareers.FunctionApp.Helpers.SessionIdHelper.GenerateSessionId(salt);
                if (codes.Contains(hash) == true)
                {
                    throw new Exception($"duplicate {i}");
                }
                Assert.True(hash.Length < 14);
                codes.Add(hash);
            }
            Assert.Equal(amount, codes.Count);
        }
    }
}

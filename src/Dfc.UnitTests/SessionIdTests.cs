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
            var codes = new List<string>();
            for (var i = 0; i < 100; i++)
            {
                string salt = Guid.NewGuid().ToString();
                var hashids = new Hashids(salt, 8);
                long l = Convert.ToInt64(DateTime.Now.ToString("yyMMddHHmmssf"));
                var hash = hashids.EncodeLong(l);
                if (codes.Contains(hash) == true)
                {
                    throw new Exception($"duplicate {i}");
                }
                Assert.True(hash.Length < 10);
                codes.Add(hash);
            }
            Assert.Equal(100, codes.Count);
        }
    }
}

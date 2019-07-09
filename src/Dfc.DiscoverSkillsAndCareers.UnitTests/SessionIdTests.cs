using Dfc.DiscoverSkillsAndCareers.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using Dfc.DiscoverSkillsAndCareers.CmsFunctionApp.DataProcessors;
using Xunit;
using Xunit.Abstractions;

namespace Dfc.UnitTests
{
    public class SessionIdTests
    {
        private readonly ITestOutputHelper Output;

        public SessionIdTests(ITestOutputHelper output)
        {
            Output = output;
        }

        private const string SESSION_SALT = "ncs";

        [Fact]
        public void GenerateSessionId_With1000_ShouldAllBeUnique()
        {
            int amount = 1000;
            var codes = new List<string>();
            for (var i = 0; i < amount; i++)
            {
                var index = new Random().Next(0);
                var hash = SessionIdHelper.GenerateSessionId(SESSION_SALT);
                if (codes.Contains(hash) == true)
                {
                    throw new Exception($"duplicate {i}");
                }
                codes.Add(hash);
            }
            int maxLength = codes.Select(x => x.Length).OrderByDescending(x => x).First();
            Output.WriteLine($"Code max length {maxLength}");
            Assert.Equal(amount, codes.Count);
        }

        [Fact]
        public void PartitionKey_WithNewSession_ShouldDecodeToCurrentMonthYear()
        {
            var expected = DateTime.Now.ToString("yyyyMM");
            var sessionId = SessionIdHelper.GenerateSessionId(SESSION_SALT);
            var datetimeStamp = SessionIdHelper.Decode(SESSION_SALT, sessionId);

            string partitionKey = SessionIdHelper.GetYearMonth(datetimeStamp);

            Assert.Equal(expected, partitionKey);
        }

        [Theory]
        [InlineData(2019, 1, "201901")]
        [InlineData(2019, 2, "201902")]
        [InlineData(2019, 6, "201906")]
        [InlineData(2019, 12, "201912")]
        [InlineData(2020, 1, "202001")]
        [InlineData(2020, 6, "202006")]
        [InlineData(2020, 12, "202012")]
        [InlineData(2021, 1, "202101")]
        [InlineData(2021, 6, "202106")]
        [InlineData(2021, 12, "202112")]
        [InlineData(2027, 1, "202701")]
        [InlineData(2027, 6, "202706")]
        [InlineData(2027, 12, "202712")]

        public void PartitionKey_WithVariousDates_ShouldMatchTheory(int year, int month, string expectedPartitionKey)
        {
            var creationDate = new DateTime(year, month, 1);
            var sessionId = SessionIdHelper.GenerateSessionId(SESSION_SALT, creationDate);
            var datetimeStamp = SessionIdHelper.Decode(SESSION_SALT, sessionId);

            string partitionKey = SessionIdHelper.GetYearMonth(datetimeStamp);

            Assert.Equal(expectedPartitionKey, partitionKey);
        }
    }
}

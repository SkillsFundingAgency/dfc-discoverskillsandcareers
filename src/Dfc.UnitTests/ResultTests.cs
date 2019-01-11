using System;
using Xunit;
using Dfc.DiscoverSkillsAndCareers.FunctionApp.Results;
using Dfc.DiscoverSkillsAndCareers.Models;

namespace Dfc.UnitTests
{
    public class ResultTests
    {
        [Fact]
        public void CalcuateResult_WithSession_ShouldHaveResultsData()
        {
            var userSession = new UserSession();

            CalculateResult.Run(userSession);

            Assert.NotNull(userSession.ResultData);
        }
    }
}

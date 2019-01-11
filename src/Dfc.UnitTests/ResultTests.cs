using System;
using System.Linq;
using System.Collections.Generic;
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

        [Fact]
        public void CalcuateResult_WithAnswers_ShouldGetSomeTraits()
        {
            var userSession = new UserSession()
            {
                LanguageCode = "en",
                RecordedAnswers = new List<Answer>()
                {
                    new Answer() { AnsweredDt = DateTime.Now, SelectedOption = AnswerOption.StronglyAgree, TraitCode = "LEADER" }
                }
            };

            CalculateResult.Run(userSession);

            Assert.NotNull(userSession.ResultData.Traits);
            Assert.True(userSession.ResultData.Traits.Count == 1);
            Assert.True(userSession.ResultData.Traits.First().TotalScore == 2);
        }
    }
}

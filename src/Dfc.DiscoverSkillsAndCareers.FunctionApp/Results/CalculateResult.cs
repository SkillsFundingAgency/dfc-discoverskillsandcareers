using System;
using System.Linq;
using System.Collections.Generic;
using Dfc.DiscoverSkillsAndCareers.Models;

namespace Dfc.DiscoverSkillsAndCareers.FunctionApp.Results
{
    public class CalculateResult
    {
        public static void Run(UserSession userSession)
        {
            var answerOptions = new Dictionary<AnswerOption, int>()
            {
                { AnswerOption.StronglyDisagree, -2 },
                { AnswerOption.Disagree, -1 },
                { AnswerOption.Neutral, 0 },
                { AnswerOption.Agree, 1 },
                { AnswerOption.StronglyAgree, 2 },
            };

            // TODO: move/store
            var traits = new List<Trait>()
            {
                new Trait() { TraitCode = "LEADER", TraitName = "Leader", Texts = new List<TraitText>() { new TraitText() { LanguageCode = "en", Text = "Some text about the Leader trait" } } },
                new Trait() { TraitCode = "DRIVER", TraitName = "Driver", Texts = new List<TraitText>() { new TraitText() { LanguageCode = "en", Text = "Some text about the Driver trait" } } },
                new Trait() { TraitCode = "DOER", TraitName = "Doer", Texts = new List<TraitText>() { new TraitText() { LanguageCode = "en", Text = "Some text about the Doer trait" } } },
                new Trait() { TraitCode = "ORGANISER", TraitName = "Organiser", Texts = new List<TraitText>() { new TraitText() { LanguageCode = "en", Text = "Some text about the Organiser trait" } } },
                new Trait() { TraitCode = "HELPER", TraitName = "Helper", Texts = new List<TraitText>() { new TraitText() { LanguageCode = "en", Text = "Some text about the Helper trait" } } },
                new Trait() { TraitCode = "ANALYST", TraitName = "Analyst", Texts = new List<TraitText>() { new TraitText() { LanguageCode = "en", Text = "Some text about the Analyst trait" } } },
                new Trait() { TraitCode = "CREATOR", TraitName = "Creator", Texts = new List<TraitText>() { new TraitText() { LanguageCode = "en", Text = "Some text about the Creator trait" } } },
                new Trait() { TraitCode = "INFLUENCER", TraitName = "Influencer", Texts = new List<TraitText>() { new TraitText() { LanguageCode = "en", Text = "Some text about the Influencer trait" } } }
            };

            var userTraits = userSession.RecordedAnswers
                .Select(x => new
                {
                    x.TraitCode,
                    Score = answerOptions.Where(a => a.Key == x.SelectedOption).First().Value
                })
                .GroupBy(x => x.TraitCode)
                .Select(g => new TraitResult()
                {
                    TraitCode = g.First().TraitCode,
                    TraitName = traits.Where(x => x.TraitCode == g.First().TraitCode).First().TraitName,
                    TraitText = traits.Where(x => x.TraitCode == g.First().TraitCode).First().Texts.Where(x => x.LanguageCode == userSession.LanguageCode).First().Text,
                    TotalScore = g.Sum(x => x.Score)
                })
                .OrderByDescending(x => x.TotalScore)
                .ToList();

            var resultData = new ResultData()
            {
                Traits = userTraits
            };
            
           userSession.ResultData = resultData;
        }
    }
}

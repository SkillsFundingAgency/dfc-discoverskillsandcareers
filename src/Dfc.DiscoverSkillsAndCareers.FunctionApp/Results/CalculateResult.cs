using System;
using System.Linq;
using System.Collections.Generic;
using Dfc.DiscoverSkillsAndCareers.Models;

namespace Dfc.DiscoverSkillsAndCareers.FunctionApp.Results
{
    public class CalculateResult
    {
        // TODO: store
        private static Dictionary<AnswerOption, int> AnswerOptions = new Dictionary<AnswerOption, int>()
            {
                { AnswerOption.StronglyDisagree, -2 },
                { AnswerOption.Disagree, -1 },
                { AnswerOption.Neutral, 0 },
                { AnswerOption.Agree, 1 },
                { AnswerOption.StronglyAgree, 2 },
            };
        // TODO: store
        private static List<Trait> Traits = new List<Trait>()
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
        // TODO: store
        private static List<JobFamily> JobFamilies = new List<JobFamily>()
            {
                new JobFamily()
                {
                    JobFamilyCode = "MAN",
                    JobFamilyName = "Managerial",
                    TraitCodes = new List<string>() { "LEADER", "DRIVER" },
                    Texts = new List<JobFamilyText>()
                    {
                        new JobFamilyText() { LanguageCode = "en", Text = "Managerial result text" }
                    }
                },
                new JobFamily()
                {
                    JobFamilyCode = "BAW",
                    JobFamilyName = "Beauty and wellbeing",
                    TraitCodes = new List<string>() { "DRIVER", "DOER" },
                    Texts = new List<JobFamilyText>()
                    {
                        new JobFamilyText() { LanguageCode = "en", Text = "Beauty and wellbeing result text" }
                    }
                },
                new JobFamily()
                {
                    JobFamilyCode = "SAR",
                    JobFamilyName = "Science and research",
                    TraitCodes = new List<string>() { "DRIVER", "ANALYST", "ORGANISER" },
                    Texts = new List<JobFamilyText>()
                    {
                        new JobFamilyText() { LanguageCode = "en", Text = "Science and research result text" }
                    }
                },
                new JobFamily()
                {
                    JobFamilyCode = "MAU",
                    JobFamilyName = "Manufacturing",
                    TraitCodes = new List<string>() { "DRIVER", "ANALYST", "ORGANISER" },
                    Texts = new List<JobFamilyText>()
                    {
                        new JobFamilyText() { LanguageCode = "en", Text = "Manufacturing result text" }
                    }
                },
                new JobFamily()
                {
                    JobFamilyCode = "TAE",
                    JobFamilyName = "Teaching and education",
                    TraitCodes = new List<string>() { "LEADER", "HELPER", "ORGANISER" },
                    Texts = new List<JobFamilyText>()
                    {
                        new JobFamilyText() { LanguageCode = "en", Text = "Teaching and education result text" }
                    }
                },
                new JobFamily()
                {
                    JobFamilyCode = "BAF",
                    JobFamilyName = "Business and finance",
                    TraitCodes = new List<string>() { "DRIVER", "ORGANISER", "DOER" },
                    Texts = new List<JobFamilyText>()
                    {
                        new JobFamilyText() { LanguageCode = "en", Text = "Business and finance result text" }
                    }
                },
                new JobFamily()
                {
                    JobFamilyCode = "LAL",
                    JobFamilyName = "Law and legal",
                    TraitCodes = new List<string>() { "DRIVER", "ORGANISER", "DOER" },
                    Texts = new List<JobFamilyText>()
                    {
                        new JobFamilyText() { LanguageCode = "en", Text = "Law and legal result text" }
                    }
                },
                new JobFamily()
                {
                    JobFamilyCode = "CTD",
                    JobFamilyName = "Computing, technology and digital",
                    TraitCodes = new List<string>() { "ANALYST", "CREATOR" },
                    Texts = new List<JobFamilyText>()
                    {
                        new JobFamilyText() { LanguageCode = "en", Text = "Computing, technology and digital result text" }
                    }
                },
                new JobFamily()
                {
                    JobFamilyCode = "SOC",
                    JobFamilyName = "Social care",
                    TraitCodes = new List<string>() { "HELPER" },
                    Texts = new List<JobFamilyText>()
                    {
                        new JobFamilyText() { LanguageCode = "en", Text = "Social care result text" }
                    }
                },
                new JobFamily()
                {
                    JobFamilyCode = "HEC",
                    JobFamilyName = "Healthcare",
                    TraitCodes = new List<string>() { "HELPER", "ANALYST", "DOER" },
                    Texts = new List<JobFamilyText>()
                    {
                        new JobFamilyText() { LanguageCode = "en", Text = "Healthcare result text" }
                    }
                },
                new JobFamily()
                {
                    JobFamilyCode = "ANC",
                    JobFamilyName = "Animal care",
                    TraitCodes = new List<string>() { "HELPER", "ANALYST", "DOER" },
                    Texts = new List<JobFamilyText>()
                    {
                        new JobFamilyText() { LanguageCode = "en", Text = "Animal care result text" }
                    }
                },
                new JobFamily()
                {
                    JobFamilyCode = "EUS",
                    JobFamilyName = "Emergency and uniform services",
                    TraitCodes = new List<string>() { "LEADER", "HELPER", "DOER" },
                    Texts = new List<JobFamilyText>()
                    {
                        new JobFamilyText() { LanguageCode = "en", Text = "Emergency and uniform services result text" }
                    }
                },
                new JobFamily()
                {
                    JobFamilyCode = "SAL",
                    JobFamilyName = "Sports and leisure",
                    TraitCodes = new List<string>() { "DRIVER", "CREATOR" },
                    Texts = new List<JobFamilyText>()
                    {
                        new JobFamilyText() { LanguageCode = "en", Text = "Sports and leisure result text" }
                    }
                },
                new JobFamily()
                {
                    JobFamilyCode = "TAT",
                    JobFamilyName = "Travel and tourism",
                    TraitCodes = new List<string>() { "HELPER", "ORGANISER", "DOER" },
                    Texts = new List<JobFamilyText>()
                    {
                        new JobFamilyText() { LanguageCode = "en", Text = "Travel and tourism result text" }
                    }
                },
                new JobFamily()
                {
                    JobFamilyCode = "ADM",
                    JobFamilyName = "Administration",
                    TraitCodes = new List<string>() { "ANALYST", "ORGANISER" },
                    Texts = new List<JobFamilyText>()
                    {
                        new JobFamilyText() { LanguageCode = "en", Text = "Administration result text" }
                    }
                },
                new JobFamily()
                {
                    JobFamilyCode = "GOV",
                    JobFamilyName = "Government services",
                    TraitCodes = new List<string>() { "ORGANISER" },
                    Texts = new List<JobFamilyText>()
                    {
                        new JobFamilyText() { LanguageCode = "en", Text = "Government services result text" }
                    }
                },
                new JobFamily()
                {
                    JobFamilyCode = "HOM",
                    JobFamilyName = "Home services",
                    TraitCodes = new List<string>() { "ORGANISER", "DOER" },
                    Texts = new List<JobFamilyText>()
                    {
                        new JobFamilyText() { LanguageCode = "en", Text = "Home services result text" }
                    }
                },
                new JobFamily()
                {
                    JobFamilyCode = "ENV",
                    JobFamilyName = "Environment and land",
                    TraitCodes = new List<string>() { "DOER" },
                    Texts = new List<JobFamilyText>()
                    {
                        new JobFamilyText() { LanguageCode = "en", Text = "Environment and land result text" }
                    }
                },
                new JobFamily()
                {
                    JobFamilyCode = "CAT",
                    JobFamilyName = "Construction and trades",
                    TraitCodes = new List<string>() { "ANALYST", "CREATOR", "DOER" },
                    Texts = new List<JobFamilyText>()
                    {
                        new JobFamilyText() { LanguageCode = "en", Text = "Construction and trades result text" }
                    }
                },
                new JobFamily()
                {
                    JobFamilyCode = "CAM",
                    JobFamilyName = "Creative and media",
                    TraitCodes = new List<string>() { "ANALYST", "CREATOR", "DOER" },
                    Texts = new List<JobFamilyText>()
                    {
                        new JobFamilyText() { LanguageCode = "en", Text = "Creative and media result text" }
                    }
                },
                new JobFamily()
                {
                    JobFamilyCode = "RAS",
                    JobFamilyName = "Retail and sales",
                    TraitCodes = new List<string>() { "INFLUENCER", "HELPER" },
                    Texts = new List<JobFamilyText>()
                    {
                        new JobFamilyText() { LanguageCode = "en", Text = "Retail and sales result text" }
                    }
                },
                new JobFamily()
                {
                    JobFamilyCode = "HAF",
                    JobFamilyName = "Hospitality and food",
                    TraitCodes = new List<string>() { "INFLUENCER", "HELPER" },
                    Texts = new List<JobFamilyText>()
                    {
                        new JobFamilyText() { LanguageCode = "en", Text = "Hospitality and food result text" }
                    }
                },
                new JobFamily()
                {
                    JobFamilyCode = "EAM",
                    JobFamilyName = "Engineering and maintenance",
                    TraitCodes = new List<string>() { "DOER" },
                    Texts = new List<JobFamilyText>()
                    {
                        new JobFamilyText() { LanguageCode = "en", Text = "Engineering and maintenance result text" }
                    }
                },
                new JobFamily()
                {
                    JobFamilyCode = "TRA",
                    JobFamilyName = "Transport",
                    TraitCodes = new List<string>() { "ORGANISER", "DOER" },
                    Texts = new List<JobFamilyText>()
                    {
                        new JobFamilyText() { LanguageCode = "en", Text = "Transport result text" }
                    }
                },
                new JobFamily()
                {
                    JobFamilyCode = "DAS",
                    JobFamilyName = "Delivery and storage",
                    TraitCodes = new List<string>() { "ORGANISER", "DOER" },
                    Texts = new List<JobFamilyText>()
                    {
                        new JobFamilyText() { LanguageCode = "en", Text = "Delivery and storage result text" }
                    }
                },
            };

        public static void Run(UserSession userSession)
        {
            var answerOptions = AnswerOptions;
            var traits = Traits;

            // User traits
            var userTraits = userSession.RecordedAnswers
                .Select(x => new
                {
                    x.TraitCode,
                    Score = !x.IsNegative ? answerOptions.Where(a => a.Key == x.SelectedOption).First().Value
                        : answerOptions.Where(a => a.Key == x.SelectedOption).First().Value * -1
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
                Traits = userTraits,
                JobFamilyCodes = CalculateJobFamilyRelevance(userTraits)
            };
            
            userSession.ResultData = resultData;
        }

        public static  List<JobFamilyResult> CalculateJobFamilyRelevance(List<TraitResult> userTraits)
        {
            var jobFamilies = JobFamilies;

            var userJobFamilies = jobFamilies
              .Select(x => new JobFamilyResult()
              {
                  JobFamilyCode = x.JobFamilyCode,
                  JobFamilyName = x.JobFamilyName,
                  TraitsTotal = userTraits.Where(t => x.TraitCodes.Contains(t.TraitCode)).Sum(t => t.TotalScore),
                  TraitValues = userTraits
                      .Where(t => x.TraitCodes.Contains(t.TraitCode))
                      .Select(v => new TraitValue()
                      {
                          TraitCode = v.TraitCode,
                          Total = v.TotalScore,
                          NormalizedTotal = x.ResultMultiplier * v.TotalScore
                      }).ToList(),
                    NormalizedTotal = x.ResultMultiplier
              })
              .Where(x => !x.TraitValues.Any(v => v.NormalizedTotal < 0.5m))
              .ToList();
            userJobFamilies.ForEach(x =>
            {
                x.Total = x.TraitValues.Where(v => v.NormalizedTotal >= 1m).Sum(v => v.NormalizedTotal);
                x.NormalizedTotal = x.NormalizedTotal * x.TraitValues.Sum(v => v.Total);
            });
            var result = userJobFamilies
                .Where(x => x.NormalizedTotal >= 1)
                .OrderByDescending(x => x.Total).ToList();
            return result;
        }
    }
}

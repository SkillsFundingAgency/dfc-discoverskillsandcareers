using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;
using Dfc.DiscoverSkillsAndCareers.ResultsFunctionApp;
using Dfc.DiscoverSkillsAndCareers.Models;
using Dfc.DiscoverSkillsAndCareers.Services;

namespace Dfc.UnitTests
{
    public class ResultTests
    {
        [Fact]
        public async Task CalcuateResult_WithSession_ShouldHaveResultsData()
        {
            var userSession = new UserSession();

            var AssessmentCalculationService = new AssessmentCalculationService();
            await AssessmentCalculationService.CalculateShortAssessment(userSession);

            Assert.NotNull(userSession.ResultData);
        }

        [Fact]
        public async Task CalcuateResult_WithAnswers_ShouldGetSomeTraits()
        {
            var userSession = new UserSession()
            {
                LanguageCode = "en",
                RecordedAnswers = new List<Answer>()
                {
                    new Answer() { AnsweredDt = DateTime.Now, SelectedOption = AnswerOption.StronglyAgree, TraitCode = "LEADER" }
                }
            };

            var AssessmentCalculationService = new AssessmentCalculationService();
            await AssessmentCalculationService.CalculateShortAssessment(userSession);

            Assert.NotNull(userSession.ResultData.Traits);
            Assert.True(userSession.ResultData.Traits.Count == 1);
            Assert.True(userSession.ResultData.Traits.First().TotalScore == 2);
        }

        [Fact]
        public async Task CalcuateResult_WithOnlyPositiveLeader_ShouldTotalLeader3()
        {
            var userSession = new UserSession()
            {
                LanguageCode = "en",
                RecordedAnswers = new List<Answer>()
                {
                    new Answer() { AnsweredDt = DateTime.Now, SelectedOption = AnswerOption.StronglyAgree, TraitCode = "LEADER" },
                    new Answer() { AnsweredDt = DateTime.Now, SelectedOption = AnswerOption.Agree, TraitCode = "LEADER" }

                }
            };

            var AssessmentCalculationService = new AssessmentCalculationService();
            await AssessmentCalculationService.CalculateShortAssessment(userSession);

            Assert.NotNull(userSession.ResultData.Traits);
            Assert.True(userSession.ResultData.Traits.Count == 1);
            Assert.True(userSession.ResultData.Traits.First().TotalScore == 3);
            Assert.True(userSession.ResultData.Traits.First().TraitCode == "LEADER");
        }

        [Fact]
        public async Task CalcuateResult_WithOnlyPositiveNegativeLeader_ShouldTotalLeader1()
        {
            var userSession = new UserSession()
            {
                LanguageCode = "en",
                RecordedAnswers = new List<Answer>()
                {
                    new Answer() { AnsweredDt = DateTime.Now, SelectedOption = AnswerOption.StronglyAgree, TraitCode = "LEADER" },
                    new Answer() { AnsweredDt = DateTime.Now, SelectedOption = AnswerOption.Agree, TraitCode = "LEADER" },
                    new Answer() { AnsweredDt = DateTime.Now, SelectedOption = AnswerOption.StronglyDisagree, TraitCode = "LEADER" },

                }
            };

            var AssessmentCalculationService = new AssessmentCalculationService();
            await AssessmentCalculationService.CalculateShortAssessment(userSession);

            Assert.NotNull(userSession.ResultData.Traits);
            Assert.True(userSession.ResultData.Traits.Count == 1);
            Assert.True(userSession.ResultData.Traits.First().TotalScore == 1);
            Assert.True(userSession.ResultData.Traits.First().TraitCode == "LEADER");
        }

        [Fact]
        public async Task CalcuateResult_WithOnlyOrganiser_ShouldHaveJobFamilyGovServices()
        {
            var userSession = new UserSession()
            {
                LanguageCode = "en",
                RecordedAnswers = new List<Answer>()
                {
                    new Answer() { AnsweredDt = DateTime.Now, SelectedOption = AnswerOption.StronglyAgree, TraitCode = "ORGANISER" },
                }
            };

            var AssessmentCalculationService = new AssessmentCalculationService();
            await AssessmentCalculationService.CalculateShortAssessment(userSession);

            Assert.NotNull(userSession.ResultData.Traits);
            Assert.True(userSession.ResultData.Traits.Count == 1);
            Assert.True(userSession.ResultData.Traits.First().TotalScore == 2);
            Assert.True(userSession.ResultData.Traits.First().TraitCode == "ORGANISER");
            var govServices = userSession.ResultData.JobFamilies.Where(x => x.Total > 0).FirstOrDefault();
            Assert.NotNull(govServices);
            Assert.True(govServices.JobFamilyCode == "GOV");
            Assert.True(govServices.Total == 2m);
        }

        [Fact]
        public void CalculateJobFamilyRelevance_WithRobExample_ShouldBeAsResult()
        {
            var userTraits = new List<TraitResult>()
            {
                new TraitResult() { TraitCode = "LEADER", TotalScore = 7 },
                new TraitResult() { TraitCode = "DRIVER", TotalScore = 1 },
                new TraitResult() { TraitCode = "INFLUENCER", TotalScore = 8 },
                new TraitResult() { TraitCode = "HELPER", TotalScore = 5 },
                new TraitResult() { TraitCode = "ANALYST", TotalScore = 9 },
                new TraitResult() { TraitCode = "CREATOR", TotalScore = 7 },
                new TraitResult() { TraitCode = "ORGANISER", TotalScore = -1 },
                new TraitResult() { TraitCode = "DOER", TotalScore = 0 }
            };


            var result = AssessmentCalculationService.CalculateJobFamilyRelevance(AssessmentCalculationService.JobFamilies, userTraits, "en");

            var jobs = FriendlyJobsString(result);
            Assert.True(result.Count == 10);
            Assert.True(result[0].JobFamilyCode == "CTD");
            Assert.True(result[1].JobFamilyCode == "RAS");
            Assert.True(result[2].JobFamilyCode == "HAF");
            Assert.True(result[3].JobFamilyCode == "CAT");
            Assert.True(result[4].JobFamilyCode == "CAM");
            Assert.True(result[5].JobFamilyCode == "SOC");
            Assert.True(result[6].JobFamilyCode == "HEC");
            Assert.True(result[7].JobFamilyCode == "ANC");
            Assert.True(result[8].JobFamilyCode == "ADM");
            Assert.True(result[9].JobFamilyCode == "TAE");
        }

        [Fact]
        public void CalculateJobFamilyRelevance_WithColinExample_ShouldBeAsResult()
        {
            var userTraits = new List<TraitResult>()
            {
                new TraitResult() { TraitCode = "LEADER", TotalScore = 7 },
                new TraitResult() { TraitCode = "DRIVER", TotalScore = 0 },
                new TraitResult() { TraitCode = "INFLUENCER", TotalScore = 5 },
                new TraitResult() { TraitCode = "HELPER", TotalScore = 4 },
                new TraitResult() { TraitCode = "ANALYST", TotalScore = 10 },
                new TraitResult() { TraitCode = "CREATOR", TotalScore = 4 },
                new TraitResult() { TraitCode = "ORGANISER", TotalScore = 1 },
                new TraitResult() { TraitCode = "DOER", TotalScore = 6 }
            };

            var result = AssessmentCalculationService.CalculateJobFamilyRelevance(AssessmentCalculationService.JobFamilies, userTraits, "en");

            var jobs = FriendlyJobsString(result);
            Assert.True(result.Count == 10);
            Assert.True(result[0].JobFamilyCode == "CTD");
            Assert.True(result[1].JobFamilyCode == "HEC");
            Assert.True(result[2].JobFamilyCode == "ANC");
            Assert.True(result[3].JobFamilyCode == "CAT");
            Assert.True(result[4].JobFamilyCode == "CAM");
            Assert.True(result[5].JobFamilyCode == "ENV");
            Assert.True(result[6].JobFamilyCode == "EAM");
            Assert.True(result[7].JobFamilyCode == "EUS");
            Assert.True(result[8].JobFamilyCode == "ADM");
            Assert.True(result[9].JobFamilyCode == "RAS");
        }

        [Fact]
        public void CalculateJobFamilyRelevance_WithAllNegative_ShouldHaveNoFamilies()
        {
            var userTraits = new List<TraitResult>()
            {
                new TraitResult() { TraitCode = "LEADER", TotalScore = -8 },
                new TraitResult() { TraitCode = "DRIVER", TotalScore = -8 },
                new TraitResult() { TraitCode = "INFLUENCER", TotalScore = -8 },
                new TraitResult() { TraitCode = "HELPER", TotalScore = -8 },
                new TraitResult() { TraitCode = "ANALYST", TotalScore = -8 },
                new TraitResult() { TraitCode = "CREATOR", TotalScore = -8 },
                new TraitResult() { TraitCode = "ORGANISER", TotalScore = -8 },
                new TraitResult() { TraitCode = "DOER", TotalScore = -8 }
            };

            var result = AssessmentCalculationService.CalculateJobFamilyRelevance(AssessmentCalculationService.JobFamilies, userTraits, "en");

            Assert.True(result.Count == 0);
        }

        [Fact]
        public void CalculateJobFamilyRelevance_WithAllNeutral_ShouldHaveNoFamilies()
        {
            var userTraits = new List<TraitResult>()
            {
                new TraitResult() { TraitCode = "LEADER", TotalScore = 0 },
                new TraitResult() { TraitCode = "DRIVER", TotalScore = 0 },
                new TraitResult() { TraitCode = "INFLUENCER", TotalScore = 0 },
                new TraitResult() { TraitCode = "HELPER", TotalScore = 0 },
                new TraitResult() { TraitCode = "ANALYST", TotalScore = 0 },
                new TraitResult() { TraitCode = "CREATOR", TotalScore = 0 },
                new TraitResult() { TraitCode = "ORGANISER", TotalScore = 0 },
                new TraitResult() { TraitCode = "DOER", TotalScore = 0 }
            };

            var result = AssessmentCalculationService.CalculateJobFamilyRelevance(AssessmentCalculationService.JobFamilies, userTraits, "en");

            Assert.True(result.Count == 0);
        }

        [Fact]
        public void CalculateJobFamilyRelevance_WithLeaderDriverInfluencer_Should()
        {
            var userTraits = new List<TraitResult>()
            {
                new TraitResult() { TraitCode = "LEADER", TotalScore = 6 },
                new TraitResult() { TraitCode = "DRIVER", TotalScore = 6 },
                new TraitResult() { TraitCode = "INFLUENCER", TotalScore = 6 },
                new TraitResult() { TraitCode = "HELPER", TotalScore = -6 },
                new TraitResult() { TraitCode = "ANALYST", TotalScore = -6 },
                new TraitResult() { TraitCode = "CREATOR", TotalScore = -6 },
                new TraitResult() { TraitCode = "ORGANISER", TotalScore = -6 },
                new TraitResult() { TraitCode = "DOER", TotalScore = -6 }
            };

            var result = AssessmentCalculationService.CalculateJobFamilyRelevance(AssessmentCalculationService.JobFamilies, userTraits, "en");

            var jobs = FriendlyJobsString(result);
            Assert.True(result.Count == 10);
            Assert.True(result[0].JobFamilyCode == "MAN");
            Assert.True(result[1].JobFamilyCode == "BAW");
            Assert.True(result[2].JobFamilyCode == "SAL");
            Assert.True(result[3].JobFamilyCode == "RAS");
            Assert.True(result[4].JobFamilyCode == "HAF");
            Assert.True(result[5].JobFamilyCode == "SAR");
            Assert.True(result[6].JobFamilyCode == "MAU");
            Assert.True(result[7].JobFamilyCode == "TAE");
            Assert.True(result[8].JobFamilyCode == "BAF");
            Assert.True(result[9].JobFamilyCode == "LAL");
        }

        private string FriendlyJobsString(List<JobFamilyResult> result)
        {
            return string.Join(Environment.NewLine, result.Select(x => $"{x.JobFamilyName} {x.Total.ToString("N2")} {x.JobFamilyCode} {x.TraitsTotal.ToString()}").ToArray());
        }

        [Fact]
        public void CalculateJobFamilyRelevance_WithHelperAnalystCreator_Should()
        {
            var userTraits = new List<TraitResult>()
            {
                new TraitResult() { TraitCode = "LEADER", TotalScore = -6 },
                new TraitResult() { TraitCode = "DRIVER", TotalScore = -6 },
                new TraitResult() { TraitCode = "INFLUENCER", TotalScore = -6 },
                new TraitResult() { TraitCode = "HELPER", TotalScore = 6 },
                new TraitResult() { TraitCode = "ANALYST", TotalScore = 6 },
                new TraitResult() { TraitCode = "CREATOR", TotalScore = 6 },
                new TraitResult() { TraitCode = "ORGANISER", TotalScore = -6 },
                new TraitResult() { TraitCode = "DOER", TotalScore = -6 }
            };

            var result = AssessmentCalculationService.CalculateJobFamilyRelevance(AssessmentCalculationService.JobFamilies, userTraits, "en");

            var jobs = FriendlyJobsString(result);
            Assert.True(result.Count == 10);
            Assert.True(result[0].JobFamilyCode == "CTD");
            Assert.True(result[1].JobFamilyCode == "SOC");
            Assert.True(result[2].JobFamilyCode == "HEC");
            Assert.True(result[3].JobFamilyCode == "ANC");
            Assert.True(result[4].JobFamilyCode == "CAT");
            Assert.True(result[5].JobFamilyCode == "CAM");
            Assert.True(result[6].JobFamilyCode == "SAL");
            Assert.True(result[7].JobFamilyCode == "ADM");
            Assert.True(result[8].JobFamilyCode == "RAS");
            Assert.True(result[9].JobFamilyCode == "HAF");
        }

        [Fact]
        public void CalculateJobFamilyRelevance_WithOrganiserDoer_Should()
        {
            var userTraits = new List<TraitResult>()
            {
                new TraitResult() { TraitCode = "LEADER", TotalScore = -6 },
                new TraitResult() { TraitCode = "DRIVER", TotalScore = -6 },
                new TraitResult() { TraitCode = "INFLUENCER", TotalScore = -6 },
                new TraitResult() { TraitCode = "HELPER", TotalScore = -6 },
                new TraitResult() { TraitCode = "ANALYST", TotalScore = -6 },
                new TraitResult() { TraitCode = "CREATOR", TotalScore = -6 },
                new TraitResult() { TraitCode = "ORGANISER", TotalScore = 6 },
                new TraitResult() { TraitCode = "DOER", TotalScore = 6 }
            };

            var result = AssessmentCalculationService.CalculateJobFamilyRelevance(AssessmentCalculationService.JobFamilies, userTraits, "en");

            var jobs = FriendlyJobsString(result);
            Assert.True(result.Count == 10);
            Assert.True(result[0].JobFamilyCode == "GOV");
            Assert.True(result[1].JobFamilyCode == "HOM");
            Assert.True(result[2].JobFamilyCode == "ENV");
            Assert.True(result[3].JobFamilyCode == "EAM");
            Assert.True(result[4].JobFamilyCode == "TRA");
            Assert.True(result[5].JobFamilyCode == "DAS");
            Assert.True(result[6].JobFamilyCode == "BAF");
            Assert.True(result[7].JobFamilyCode == "LAL");
            Assert.True(result[8].JobFamilyCode == "TAT");
            Assert.True(result[9].JobFamilyCode == "BAW");
        }

        [Fact]
        public void CalculateJobFamilyRelevance_WithAnalystOrganiserDriver_Should()
        {
            var userTraits = new List<TraitResult>()
            {
                new TraitResult() { TraitCode = "LEADER", TotalScore = -6 },
                new TraitResult() { TraitCode = "DRIVER", TotalScore = 6 },
                new TraitResult() { TraitCode = "INFLUENCER", TotalScore = -6 },
                new TraitResult() { TraitCode = "HELPER", TotalScore = -6 },
                new TraitResult() { TraitCode = "ANALYST", TotalScore = 6 },
                new TraitResult() { TraitCode = "CREATOR", TotalScore = -6 },
                new TraitResult() { TraitCode = "ORGANISER", TotalScore = 6 },
                new TraitResult() { TraitCode = "DOER", TotalScore = -6 }
            };

            var result = AssessmentCalculationService.CalculateJobFamilyRelevance(AssessmentCalculationService.JobFamilies, userTraits, "en");

            var jobs = FriendlyJobsString(result);
            Assert.True(result.Count == 10);
            Assert.True(result[0].JobFamilyCode == "ADM");
            Assert.True(result[1].JobFamilyCode == "GOV");
            Assert.True(result[2].JobFamilyCode == "SAR");
            Assert.True(result[3].JobFamilyCode == "MAU");
            Assert.True(result[4].JobFamilyCode == "BAF");
            Assert.True(result[5].JobFamilyCode == "LAL");
            Assert.True(result[6].JobFamilyCode == "MAN");
            Assert.True(result[7].JobFamilyCode == "BAW");
            Assert.True(result[8].JobFamilyCode == "CTD");
            Assert.True(result[9].JobFamilyCode == "SAL");
        }

        [Fact]
        public void CalculateJobFamilyRelevance_WithAllAgree_Should()
        {
            var userTraits = new List<TraitResult>()
            {
                new TraitResult() { TraitCode = "LEADER", TotalScore = 3 },
                new TraitResult() { TraitCode = "DRIVER", TotalScore = 3 },
                new TraitResult() { TraitCode = "INFLUENCER", TotalScore = 3 },
                new TraitResult() { TraitCode = "HELPER", TotalScore = 3 },
                new TraitResult() { TraitCode = "ANALYST", TotalScore = 3 },
                new TraitResult() { TraitCode = "CREATOR", TotalScore = 3 },
                new TraitResult() { TraitCode = "ORGANISER", TotalScore = 3 },
                new TraitResult() { TraitCode = "DOER", TotalScore = 3 }
            };

            var result = AssessmentCalculationService.CalculateJobFamilyRelevance(AssessmentCalculationService.JobFamilies, userTraits, "en");

            Assert.True(result.Count == 10);
        }
    }
}

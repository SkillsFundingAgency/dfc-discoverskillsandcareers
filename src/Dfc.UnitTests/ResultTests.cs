using Dfc.DiscoverSkillsAndCareers.AssessmentFunctionApp.Services;
using Dfc.DiscoverSkillsAndCareers.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using Dfc.DiscoverSkillsAndCareers.Repositories;
using Microsoft.Build.Utilities;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Xunit;
using Task = System.Threading.Tasks.Task;

namespace Dfc.UnitTests
{
    public class ResultTests
    {
        private IQuestionSetRepository questionSetRepository;
        private ILogger logger;

        public ResultTests()
        {
            questionSetRepository = Substitute.For<IQuestionSetRepository>();
            logger = Substitute.For<ILogger>();

            questionSetRepository.GetCurrentFilteredQuestionSets().Returns(Task.FromResult(
                AssessmentCalculationService.JobFamilies.Select(jf => new QuestionSet
                {
                    Title = jf.JobFamilyName,
                    MaxQuestions = 3
                }).ToList()));
        }
        
        [Fact]
        public async Task CalcuateResult_WithSession_ShouldHaveResultsData()
        {
            var userSession = new UserSession()
            {
                LanguageCode = "en",
                AssessmentState = new AssessmentState {
                    RecordedAnswers = new []
                    {
                        new Answer() { AnsweredDt = DateTime.Now, SelectedOption = AnswerOption.StronglyAgree, TraitCode = "LEADER" }
                    }
                }
            };

            var assessmentCalculationService = new AssessmentCalculationService(questionSetRepository);
            await assessmentCalculationService.CalculateAssessment(userSession, logger);

            Assert.NotNull(userSession.ResultData);
        }

        [Fact]
        public async Task CalcuateResult_WithAnswers_ShouldGetSomeTraits()
        {
            var userSession = new UserSession()
            {
                LanguageCode = "en",
                AssessmentState = new AssessmentState {
                    RecordedAnswers = new []
                    {
                        new Answer() { AnsweredDt = DateTime.Now, SelectedOption = AnswerOption.StronglyAgree, TraitCode = "LEADER" }
                    }
                }
            };

            var assessmentCalculationService = new AssessmentCalculationService(questionSetRepository);
            await assessmentCalculationService.CalculateAssessment(userSession, logger);

            Assert.NotNull(userSession.ResultData.Traits);
            Assert.True(userSession.ResultData.Traits.Length == 1);
            Assert.True(userSession.ResultData.Traits.First().TotalScore == 2);
        }

        [Fact]
        public async Task CalcuateResult_WithOnlyPositiveLeader_ShouldTotalLeader3()
        {
            var userSession = new UserSession()
            {
                LanguageCode = "en",
                AssessmentState = new AssessmentState {
                    RecordedAnswers = new []
                    {
                        new Answer() { AnsweredDt = DateTime.Now, SelectedOption = AnswerOption.StronglyAgree, TraitCode = "LEADER" },
                        new Answer() { AnsweredDt = DateTime.Now, SelectedOption = AnswerOption.Agree, TraitCode = "LEADER" }
    
                    }
                }
            };

            var assessmentCalculationService = new AssessmentCalculationService(questionSetRepository);
            await assessmentCalculationService.CalculateAssessment(userSession, logger);

            Assert.NotNull(userSession.ResultData.Traits);
            Assert.True(userSession.ResultData.Traits.Length == 1);
            Assert.True(userSession.ResultData.Traits.First().TotalScore == 3);
            Assert.True(userSession.ResultData.Traits.First().TraitCode == "LEADER");
        }

        [Fact]
        public async Task CalcuateResult_WithOnlyPositiveNegativeLeader_ShouldTotalLeader1()
        {
            var userSession = new UserSession()
            {
                LanguageCode = "en",
                AssessmentState = new AssessmentState {
                    RecordedAnswers = new []
                    {
                        new Answer() { AnsweredDt = DateTime.Now, SelectedOption = AnswerOption.StronglyAgree, TraitCode = "LEADER" },
                        new Answer() { AnsweredDt = DateTime.Now, SelectedOption = AnswerOption.Agree, TraitCode = "LEADER" },
                        new Answer() { AnsweredDt = DateTime.Now, SelectedOption = AnswerOption.StronglyDisagree, TraitCode = "LEADER" },
    
                    }
                }
            };

            var AssessmentCalculationService = new AssessmentCalculationService(questionSetRepository);
            await AssessmentCalculationService.CalculateAssessment(userSession, logger);

            Assert.NotNull(userSession.ResultData.Traits);
            Assert.True(userSession.ResultData.Traits.Length == 1);
            Assert.True(userSession.ResultData.Traits.First().TotalScore == 1);
            Assert.True(userSession.ResultData.Traits.First().TraitCode == "LEADER");
        }

        [Fact]
        public async Task CalcuateResult_WithOnlyOrganiser_ShouldHaveJobFamilyGovServices()
        {
            var userSession = new UserSession()
            {
                LanguageCode = "en",
                AssessmentState = new AssessmentState {
                    RecordedAnswers = new []
                    {
                        new Answer() { AnsweredDt = DateTime.Now, SelectedOption = AnswerOption.StronglyAgree, TraitCode = "ORGANISER" },
                    }
                }
            };

            var assessmentCalculationService = new AssessmentCalculationService(questionSetRepository);
            await assessmentCalculationService.CalculateAssessment(userSession, logger);

            Assert.NotNull(userSession.ResultData.Traits);
            Assert.True(userSession.ResultData.Traits.Length == 1);
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
            var userTraits = new []
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

            var assessmentCalculationService = new AssessmentCalculationService(questionSetRepository);

            var result = assessmentCalculationService.CalculateJobFamilyRelevance(AssessmentCalculationService.JobFamilies, userTraits, "en");

            var jobs = FriendlyJobsString(result);
            Assert.Equal(6, result.Count);
            Assert.Equal("CTD", result[0].JobFamilyCode);
            Assert.Equal("RAS", result[1].JobFamilyCode);
            Assert.Equal("HAF", result[2].JobFamilyCode);
            Assert.Equal("SOC", result[3].JobFamilyCode);
            Assert.Equal("MAN", result[4].JobFamilyCode);
            Assert.Equal("SAL", result[5].JobFamilyCode);
    
        }

        [Fact]
        public void CalculateJobFamilyRelevance_WithColinExample_ShouldBeAsResult()
        {
            var userTraits = new []
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

            var assessmentCalculationService = new AssessmentCalculationService(questionSetRepository);
            var result = assessmentCalculationService.CalculateJobFamilyRelevance(AssessmentCalculationService.JobFamilies, userTraits, "en");

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
            var userTraits = new []
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

            var assessmentCalculationService = new AssessmentCalculationService(questionSetRepository);
            var result = assessmentCalculationService.CalculateJobFamilyRelevance(AssessmentCalculationService.JobFamilies, userTraits, "en");

            Assert.True(result.Count == 0);
        }

        [Fact]
        public void CalculateJobFamilyRelevance_WithAllNeutral_ShouldHaveNoFamilies()
        {
            var userTraits = new []
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

            var assessmentCalculationService = new AssessmentCalculationService(questionSetRepository);
            var result = assessmentCalculationService.CalculateJobFamilyRelevance(AssessmentCalculationService.JobFamilies, userTraits, "en");

            Assert.True(result.Count == 0);
        }

        [Fact]
        public void CalculateJobFamilyRelevance_WithLeaderDriverInfluencer_Should()
        {
            var userTraits = new []
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

            var assessmentCalculationService = new AssessmentCalculationService(questionSetRepository);
            var result = assessmentCalculationService.CalculateJobFamilyRelevance(AssessmentCalculationService.JobFamilies, userTraits, "en");

            var jobs = FriendlyJobsString(result);
            Assert.Single(result);
            Assert.Equal("MAN", result[0].JobFamilyCode);
        }

        private string FriendlyJobsString(IEnumerable<JobFamilyResult> result)
        {
            return string.Join(Environment.NewLine, result.Select(x => $"{x.JobFamilyName} {x.Total.ToString("N2")} {x.JobFamilyCode} {x.TraitsTotal.ToString()}").ToArray());
        }

        [Fact]
        public void CalculateJobFamilyRelevance_WithHelperAnalystCreator_Should()
        {
            var userTraits = new []
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

            var assessmentCalculationService = new AssessmentCalculationService(questionSetRepository);
            var result = assessmentCalculationService.CalculateJobFamilyRelevance(AssessmentCalculationService.JobFamilies, userTraits, "en");

            var jobs = FriendlyJobsString(result);
            Assert.Equal(2, result.Count);
            Assert.Equal("CTD",result[0].JobFamilyCode);
            Assert.Equal("SOC",result[1].JobFamilyCode);
        }

        [Fact]
        public void CalculateJobFamilyRelevance_WithOrganiserDoer_Should()
        {
            var userTraits = new []
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

            var assessmentCalculationService = new AssessmentCalculationService(questionSetRepository);
            var result = assessmentCalculationService.CalculateJobFamilyRelevance(AssessmentCalculationService.JobFamilies, userTraits, "en");

            var jobs = FriendlyJobsString(result);
            Assert.Equal(6,result.Count);
            Assert.Equal("GOV",result[0].JobFamilyCode);
            Assert.Equal("HOM",result[1].JobFamilyCode);
            Assert.Equal("ENV",result[2].JobFamilyCode);
            Assert.Equal("EAM",result[3].JobFamilyCode);
            Assert.Equal("TRA",result[4].JobFamilyCode);
            Assert.Equal("DAS",result[5].JobFamilyCode);
        }

        [Fact]
        public void CalculateJobFamilyRelevance_WithAnalystOrganiserDriver_Should()
        {
            var userTraits = new []
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

            var assessmentCalculationService = new AssessmentCalculationService(questionSetRepository);
            var result = assessmentCalculationService.CalculateJobFamilyRelevance(AssessmentCalculationService.JobFamilies, userTraits, "en");

            var jobs = FriendlyJobsString(result);
            Assert.Equal(4,result.Count);
            Assert.Equal("ADM", result[0].JobFamilyCode);
            Assert.Equal("GOV", result[1].JobFamilyCode);
            Assert.Equal("SAR", result[2].JobFamilyCode);
            Assert.Equal("MAU", result[3].JobFamilyCode);
        }

        [Fact]
        public void CalculateJobFamilyRelevance_WithAllAgree_Should()
        {
            var userTraits = new []
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

            var assessmentCalculationService = new AssessmentCalculationService(questionSetRepository);
            var result = assessmentCalculationService.CalculateJobFamilyRelevance(AssessmentCalculationService.JobFamilies, userTraits, "en");

            Assert.True(result.Count == 10);
        }
    }
}

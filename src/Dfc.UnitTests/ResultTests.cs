using Dfc.DiscoverSkillsAndCareers.AssessmentFunctionApp.Services;
using Dfc.DiscoverSkillsAndCareers.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dfc.DiscoverSkillsAndCareers.Repositories;
using NSubstitute;
using Xunit;
using Microsoft.Extensions.Logging;

namespace Dfc.UnitTests
{
    public class ResultTests
    {

        private IJobCategoryRepository _jobFamilyRepository;
        private IShortTraitRepository _shortTraitRepository;
        private IQuestionSetRepository _questionSetRepository;
        private ILogger _logger;
        private AssessmentCalculationService _assessmentCalculationService;

        public ResultTests()
        {
            _jobFamilyRepository = Substitute.For<IJobCategoryRepository>();
            _shortTraitRepository = Substitute.For<IShortTraitRepository>();
            _questionSetRepository = Substitute.For<IQuestionSetRepository>();
            _logger = Substitute.For<ILogger>();

            _assessmentCalculationService = new AssessmentCalculationService(
                _jobFamilyRepository,
                _shortTraitRepository,
                _questionSetRepository);

            _shortTraitRepository.GetTraits("shorttrait-cms").Returns(
                Task.FromResult(ResultTestMockData.Traits.ToArray())
            );

            _jobFamilyRepository.GetJobCategories("jobfamily-cms").Returns(
                Task.FromResult(ResultTestMockData.JobFamilies.ToArray())
            );

            _questionSetRepository.GetCurrentFilteredQuestionSets().Returns(
                Task.FromResult(
                ResultTestMockData.JobFamilies.Select(jf => new QuestionSet
                {
                    Title = jf.JobFamilyName,
                    QuestionSetKey = jf.JobFamilyName.Replace(" ", "-").ToLower(),
                    MaxQuestions = 3
                }).ToList())
            );
        }

        [Fact]
        public async Task CalcuateResult_WithSession_ShouldHaveResultsData()
        {
            var userSession = new UserSession();

            var exception = await Assert.ThrowsAsync<Exception>(async () =>
            {
                await _assessmentCalculationService.CalculateAssessment(userSession,_logger);
            });
            Assert.Equal("AssessmentState is not set!", exception.Message);
        }

        [Fact]
        public async Task CalcuateResult_WithAnswers_ShouldGetSomeTraits()
        {
            var userSession = new UserSession()
            {
                LanguageCode = "en",
                AssessmentState = new AssessmentState()
                {
                    RecordedAnswers = new[]
                    {
                        new Answer() { AnsweredDt = DateTime.Now, SelectedOption = AnswerOption.StronglyAgree, TraitCode = "LEADER" }
                    }
                }
            };

            await _assessmentCalculationService.CalculateAssessment(userSession,_logger);

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
                AssessmentState = new AssessmentState()
                {
                    RecordedAnswers = new[]
                    {
                        new Answer() { AnsweredDt = DateTime.Now, SelectedOption = AnswerOption.StronglyAgree, TraitCode = "LEADER" },
                        new Answer() { AnsweredDt = DateTime.Now, SelectedOption = AnswerOption.Agree, TraitCode = "LEADER" }
                    }
                }
            };

            await _assessmentCalculationService.CalculateAssessment(userSession,_logger);

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
                AssessmentState = new AssessmentState()
                {
                    RecordedAnswers = new[]
                    {
                        new Answer() { AnsweredDt = DateTime.Now, SelectedOption = AnswerOption.StronglyAgree, TraitCode = "LEADER" },
                        new Answer() { AnsweredDt = DateTime.Now, SelectedOption = AnswerOption.Agree, TraitCode = "LEADER" },
                        new Answer() { AnsweredDt = DateTime.Now, SelectedOption = AnswerOption.StronglyDisagree, TraitCode = "LEADER" },
                    }
                }
            };
            
            await _assessmentCalculationService.CalculateAssessment(userSession,_logger);

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
                AssessmentState = new AssessmentState()
                {
                    RecordedAnswers = new[]
                    {
                        new Answer() { AnsweredDt = DateTime.Now, SelectedOption = AnswerOption.StronglyAgree, TraitCode = "ORGANISER" },
                    }
                }
            };

            await _assessmentCalculationService.CalculateAssessment(userSession,_logger);

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
            var userTraits = new[]
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

            var result = _assessmentCalculationService.CalculateJobFamilyRelevance(ResultTestMockData.JobFamilies, userTraits, "en");

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
            var userTraits = new[]
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

            var result = _assessmentCalculationService.CalculateJobFamilyRelevance(ResultTestMockData.JobFamilies, userTraits, "en");

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
            var userTraits = new[]
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

            var result = _assessmentCalculationService.CalculateJobFamilyRelevance(ResultTestMockData.JobFamilies, userTraits, "en");

            Assert.True(result.Count == 0);
        }

        [Fact]
        public void CalculateJobFamilyRelevance_WithAllNeutral_ShouldHaveNoFamilies()
        {
            var userTraits = new[]
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

            var result = _assessmentCalculationService.CalculateJobFamilyRelevance(ResultTestMockData.JobFamilies, userTraits, "en");

            Assert.True(result.Count == 0);
        }

        [Fact]
        public void CalculateJobFamilyRelevance_WithLeaderDriverInfluencer_Should()
        {
            var userTraits = new[]
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

            var result = _assessmentCalculationService.CalculateJobFamilyRelevance(ResultTestMockData.JobFamilies, userTraits, "en");

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

        private string FriendlyJobsString(IEnumerable<JobFamilyResult> result)
        {
            return string.Join(Environment.NewLine, result.Select(x => $"{x.JobFamilyName} {x.Total.ToString("N2")} {x.JobFamilyCode} {x.TraitsTotal.ToString()}").ToArray());
        }

        [Fact]
        public void CalculateJobFamilyRelevance_WithHelperAnalystCreator_Should()
        {
            var userTraits = new[]
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

            var result = _assessmentCalculationService.CalculateJobFamilyRelevance(ResultTestMockData.JobFamilies, userTraits, "en");

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
            var userTraits = new[]
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

            var result = _assessmentCalculationService.CalculateJobFamilyRelevance(ResultTestMockData.JobFamilies, userTraits, "en");

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
            var userTraits = new[]
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

            var result = _assessmentCalculationService.CalculateJobFamilyRelevance(ResultTestMockData.JobFamilies, userTraits, "en");

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
            var userTraits = new[]
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

            var result = _assessmentCalculationService.CalculateJobFamilyRelevance(ResultTestMockData.JobFamilies, userTraits, "en");

            Assert.True(result.Count == 10);
        }
    }

    public static class ResultTestMockData
    {
        public static List<Trait> Traits = new List<Trait>()
        {
            new Trait() { TraitCode = "LEADER", TraitName = "Leader", Texts = new [] { new TraitText() { LanguageCode = "en", Text = "You like to lead other people and are good at taking control of situations." } } },
            new Trait() { TraitCode = "DRIVER", TraitName = "Driver", Texts = new [] { new TraitText() { LanguageCode = "en", Text = "You enjoy setting targets and are comfortable competing with other people." } } },
            new Trait() { TraitCode = "DOER", TraitName = "Doer", Texts = new [] { new TraitText() { LanguageCode = "en", Text = "You’re a practical person and enjoy working with your hands." } } },
            new Trait() { TraitCode = "ORGANISER", TraitName = "Organiser", Texts = new [] { new TraitText() { LanguageCode = "en", Text = "You like to plan things and are well organised." } } },
            new Trait() { TraitCode = "HELPER", TraitName = "Helper", Texts = new [] { new TraitText() { LanguageCode = "en", Text = "You enjoy helping and listening to other people." } } },
            new Trait() { TraitCode = "ANALYST", TraitName = "Analyst", Texts = new [] { new TraitText() { LanguageCode = "en", Text = "You like dealing with complicated problems or working with numbers." } } },
            new Trait() { TraitCode = "CREATOR", TraitName = "Creator", Texts = new [] { new TraitText() { LanguageCode = "en", Text = "You’re a creative person and enjoy coming up with new ways of doing things." } } },
            new Trait() { TraitCode = "INFLUENCER", TraitName = "Influencer", Texts = new [] { new TraitText() { LanguageCode = "en", Text = "You are sociable and find it easy to understand people." } } }
        };


        public static List<JobFamily> JobFamilies = new List<JobFamily>()
        {
            new JobFamily()
            {
                JobFamilyCode = "MAN",
                JobFamilyName = "Managerial",
                TraitCodes = new [] { "LEADER", "DRIVER" },
                Texts = new []
                {
                    new JobFamilyText()
                    {
                        LanguageCode = "en",
                        Text = "To do managerial jobs, you might need leadership skills, the ability to motivate and manage staff, and the ability to monitor your own performance and that of your colleagues.",
                        Url = "managerial"
                    }
                }
            },
            new JobFamily()
            {
                JobFamilyCode = "BAW",
                JobFamilyName = "Beauty and wellbeing",
                TraitCodes = new []{ "DRIVER", "DOER" },
                Texts = new []
                {
                    new JobFamilyText()
                    {
                        LanguageCode = "en",
                        Text = "To do beauty and wellbeing jobs, you might need customer service skills, sensitivity and understanding, or the ability to work well with your hands.",
                        Url = "beauty-and-wellbeing"
                    }
                }
            },
            new JobFamily()
            {
                JobFamilyCode = "SAR",
                JobFamilyName = "Science and research",
                TraitCodes = new [] { "DRIVER", "ANALYST", "ORGANISER" },
                Texts = new []
                {
                    new JobFamilyText()
                    {
                        LanguageCode = "en",
                        Text = "To do science and research jobs, you might need the ability to operate and control equipment, or to be thorough and pay attention to detail, or observation and recording skills.",
                        Url = "science-and-research"
                    }
                }
            },
            new JobFamily()
            {
                JobFamilyCode = "MAU",
                JobFamilyName = "Manufacturing",
                TraitCodes = new [] { "DRIVER", "ANALYST", "ORGANISER" },
                Texts = new []
                {
                    new JobFamilyText()
                    {
                        LanguageCode = "en",
                        Text = "To do manufacturing jobs, you might need to be thorough and pay attention to detail, physical skills like movement, coordination, dexterity and grace, or the ability to work well with your hands.",
                        Url = "manufacturing"
                    }
                }
            },
            new JobFamily()
            {
                JobFamilyCode = "TAE",
                JobFamilyName = "Teaching and education",
                TraitCodes = new [] { "LEADER", "HELPER", "ORGANISER" },
                Texts = new []
                {
                    new JobFamilyText()
                    {
                        LanguageCode = "en",
                        Text = "To do teaching and education jobs, you might need counselling skills including active listening and a non-judgemental approach, knowledge of teaching and the ability to design courses, or sensitivity and understanding.",
                        Url = "teaching-and-education"
                    }
                }
            },
            new JobFamily()
            {
                JobFamilyCode = "BAF",
                JobFamilyName = "Business and finance",
                TraitCodes = new [] { "DRIVER", "ORGANISER", "DOER" },
                Texts = new []
                {
                    new JobFamilyText()
                    {
                        LanguageCode = "en",
                        Text = "To do business and finance jobs, you might need to be thorough and pay attention to detail, administration skills, or maths knowledge.",
                        Url = "business-and-finance"
                    }
                }
            },
            new JobFamily()
            {
                JobFamilyCode = "LAL",
                JobFamilyName = "Law and legal",
                TraitCodes = new [] { "DRIVER", "ORGANISER", "DOER" },
                Texts = new []
                {
                    new JobFamilyText()
                    {
                        LanguageCode = "en",
                        Text = "To do law and legal jobs, you might need persuading and negotiating skills, active listening skills the ability to accept criticism and work well under pressure, or to be thorough and pay attention to detail.",
                        Url = "law-and-legal"
                    }
                }
            },
            new JobFamily()
            {
                JobFamilyCode = "CTD",
                JobFamilyName = "Computing, technology and digital",
                TraitCodes = new [] { "ANALYST", "CREATOR" },
                Texts = new []
                {
                    new JobFamilyText()
                    {
                        LanguageCode = "en",
                        Text = "To do computing, technology and digital jobs, you might need analytical thinking skills, the ability to come up with new ways of doing things, or a thorough understanding of computer systems and applications.",
                        Url = "computing-technology-and-digital"
                    }
                }
            },
            new JobFamily()
            {
                JobFamilyCode = "SOC",
                JobFamilyName = "Social care",
                TraitCodes = new [] { "HELPER" },
                Texts = new []
                {
                    new JobFamilyText()
                    {
                        LanguageCode = "en",
                        Text = "To do social care jobs, you might need sensitivity and understanding patience and the ability to remain calm in stressful situations, the ability to work well with others, or excellent verbal communication skills.",
                        Url = "social-care"
                    }
                }
            },
            new JobFamily()
            {
                JobFamilyCode = "HEC",
                JobFamilyName = "Healthcare",
                TraitCodes = new [] { "HELPER", "ANALYST", "DOER" },
                Texts = new []
                {
                    new JobFamilyText()
                    {
                        LanguageCode = "en",
                        Text = "To do healthcare jobs, you might need sensitivity and understanding, the ability to work well with others, or excellent verbal communication skills.",
                        Url = "healthcare"
                    }
                }
            },
            new JobFamily()
            {
                JobFamilyCode = "ANC",
                JobFamilyName = "Animal care",
                TraitCodes = new [] { "HELPER", "ANALYST", "DOER" },
                Texts = new []
                {
                    new JobFamilyText()
                    {
                        LanguageCode = "en",
                        Text = "To do animal care jobs, you might need the ability to use your initiative, patience and the ability to remain calm in stressful situations, or the ability to accept criticism and work well under pressure.",
                        Url = "animal-care"
                    }
                }
            },
            new JobFamily()
            {
                JobFamilyCode = "EUS",
                JobFamilyName = "Emergency and uniform services",
                TraitCodes = new [] { "LEADER", "HELPER", "DOER" },
                Texts = new []
                {
                    new JobFamilyText()
                    {
                        LanguageCode = "en",
                        Text = "To do emergency and uniform service jobs, you might need knowledge of public safety and security, the ability to accept criticism and work well under pressure, or patience and the ability to remain calm in stressful situations.",
                        Url = "emergency-and-uniform-services"
                    }
                }
            },
            new JobFamily()
            {
                JobFamilyCode = "SAL",
                JobFamilyName = "Sports and leisure",
                TraitCodes = new [] { "DRIVER", "CREATOR" },
                Texts = new []
                {
                    new JobFamilyText()
                    {
                        LanguageCode = "en",
                        Text = "To do sports and leisure jobs, you might need the ability to work well with others, to enjoy working with other people, or knowledge of teaching and the ability to design courses.",
                        Url = "sports-and-leisure"
                    }
                }
            },
            new JobFamily()
            {
                JobFamilyCode = "TAT",
                JobFamilyName = "Travel and tourism",
                TraitCodes = new [] { "HELPER", "ORGANISER", "DOER" },
                Texts = new []
                {
                    new JobFamilyText()
                    {
                        LanguageCode = "en",
                        Text = "To do travel and tourism jobs, you might need excellent verbal communication skills, the ability to sell products and services, or active listening skills.",
                        Url = "travel-and-tourism"
                    }
                }
            },
            new JobFamily()
            {
                JobFamilyCode = "ADM",
                JobFamilyName = "Administration",
                TraitCodes = new [] { "ANALYST", "ORGANISER" },
                Texts = new []
                {
                    new JobFamilyText()
                    {
                        LanguageCode = "en",
                        Text = "To do administration jobs, you might need administration skills, the ability to work well with others, or customer service skills.",
                        Url = "administration"
                    }
                }
            },
            new JobFamily()
            {
                JobFamilyCode = "GOV",
                JobFamilyName = "Government services",
                TraitCodes = new [] { "ORGANISER" },
                Texts = new []
                {
                    new JobFamilyText()
                    {
                        LanguageCode = "en",
                        Text = "To do government services jobs, you might need the ability to accept criticism and work well under pressure, to be thorough and pay attention to detail, and customer service skills.",
                        Url = "government-services"
                    }
                }
            },
            new JobFamily()
            {
                JobFamilyCode = "HOM",
                JobFamilyName = "Home services",
                TraitCodes = new [] { "ORGANISER", "DOER" },
                Texts = new []
                {
                    new JobFamilyText()
                    {
                        LanguageCode = "en",
                        Text = "To do home services jobs, you might need customer service skills, business management skills, or administration skills, or the ability to accept criticism and work well under pressure.",
                        Url = "home-services"
                    }
                }
            },
            new JobFamily()
            {
                JobFamilyCode = "ENV",
                JobFamilyName = "Environment and land",
                TraitCodes = new [] { "DOER" },
                Texts = new []
                {
                    new JobFamilyText()
                    {
                        LanguageCode = "en",
                        Text = "To do environment and land jobs, you might need thinking and reasoning skills, to be thorough and pay attention to detail, or analytical thinking skills.",
                        Url = "environment-and-land"
                    }
                }
            },
            new JobFamily()
            {
                JobFamilyCode = "CAT",
                JobFamilyName = "Construction and trades",
                TraitCodes = new [] { "ANALYST", "CREATOR", "DOER" },
                Texts = new []
                {
                    new JobFamilyText()
                    {
                        LanguageCode = "en",
                        Text = "To do construction and trades jobs, you might need knowledge of building and construction, patience and the ability to remain calm in stressful situations, and the ability to work well with your hands.",
                        Url = "construction-and-trades"
                    }
                }
            },
            new JobFamily()
            {
                JobFamilyCode = "CAM",
                JobFamilyName = "Creative and media",
                TraitCodes = new [] { "ANALYST", "CREATOR", "DOER" },
                Texts = new []
                {
                    new JobFamilyText()
                    {
                        LanguageCode = "en",
                        Text = "To do creative and media jobs, you might need the ability to come up with new ways of doing things, the ability to use your initiative, or the ability to organise your time and workload.",
                        Url = "creative-and-media"
                    }
                }
            },
            new JobFamily()
            {
                JobFamilyCode = "RAS",
                JobFamilyName = "Retail and sales",
                TraitCodes = new [] { "INFLUENCER", "HELPER" },
                Texts = new []
                {
                    new JobFamilyText()
                    {
                        LanguageCode = "en",
                        Text = "To do retail and sales jobs, you might need customer service skills, the ability to work well with others, or the ability to sell products and services.",
                        Url = "retail-and-sales"
                    }
                }
            },
            new JobFamily()
            {
                JobFamilyCode = "HAF",
                JobFamilyName = "Hospitality and food",
                TraitCodes = new [] { "INFLUENCER", "HELPER" },
                Texts = new []
                {
                    new JobFamilyText()
                    {
                        LanguageCode = "en",
                        Text = "To do hospitality and food jobs, you might need customer service skills, the ability to sell products and services, or to enjoy working with other people.",
                        Url = "hospitality-and-food"
                    }
                }
            },
            new JobFamily()
            {
                JobFamilyCode = "EAM",
                JobFamilyName = "Engineering and maintenance",
                TraitCodes = new [] { "DOER" },
                Texts = new []
                {
                    new JobFamilyText()
                    {
                        LanguageCode = "en",
                        Text = "To do engineering and maintenance jobs, you might need knowledge of engineering science and technology, to be thorough and pay attention to detail, or analytical thinking skills.",
                        Url = "engineering-and-maintenance"
                    }
                }
            },
            new JobFamily()
            {
                JobFamilyCode = "TRA",
                JobFamilyName = "Transport",
                TraitCodes = new [] { "ORGANISER", "DOER" },
                Texts = new []
                {
                    new JobFamilyText()
                    {
                        LanguageCode = "en",
                        Text = "To do transport jobs, you might need customer service skills, knowledge of public safety and security, or the ability to operate and control equipment.",
                        Url = "transport"
                    }
                }
            },
            new JobFamily()
            {
                JobFamilyCode = "DAS",
                JobFamilyName = "Delivery and storage",
                TraitCodes = new [] { "ORGANISER", "DOER" },
                Texts = new []
                {
                    new JobFamilyText()
                    {
                        LanguageCode = "en",
                        Text = "To do delivery and storage jobs, you might need the ability to work well with others, customer service skills, or knowledge of transport methods, costs and benefits.",
                        Url = "delivery-and-storage"
                    }
                }
            },
        };
    }
}
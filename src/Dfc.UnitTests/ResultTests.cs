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
        private IQuestionRepository _questionRepository;

        public ResultTests()
        {
            _jobFamilyRepository = Substitute.For<IJobCategoryRepository>();
            _shortTraitRepository = Substitute.For<IShortTraitRepository>();
            _questionSetRepository = Substitute.For<IQuestionSetRepository>();
            _questionRepository = Substitute.For<IQuestionRepository>();
            
            _logger = Substitute.For<ILogger>();

            _assessmentCalculationService = new AssessmentCalculationService(
                _jobFamilyRepository,
                _shortTraitRepository,
                _questionRepository,
                _questionSetRepository);

            _shortTraitRepository.GetTraits().Returns(
                Task.FromResult(ResultTestMockData.Traits.ToArray())
            );

            _jobFamilyRepository.GetJobCategories("jobfamily-cms").Returns(
                Task.FromResult(ResultTestMockData.JobFamilies.ToArray())
            );

            _questionSetRepository.GetCurrentFilteredQuestionSets().Returns(
                Task.FromResult(
                ResultTestMockData.JobFamilies.Select(jf => new QuestionSet
                {
                    Title = jf.Name,
                    QuestionSetKey = jf.Name.Replace(" ", "-").ToLower(),
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
                AssessmentState = new AssessmentState("qs-1",1)
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
                AssessmentState = new AssessmentState("qs-1",2)
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
                AssessmentState = new AssessmentState("qs-1",3)
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
                AssessmentState = new AssessmentState("qs-1",1)
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
            var govServices = userSession.ResultData.JobCategories.Where(x => x.Total > 0).FirstOrDefault();
            Assert.NotNull(govServices);
            Assert.True(govServices.JobCategoryCode == "GS");
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

            var result = _assessmentCalculationService.CalculateJobFamilyRelevance(ResultTestMockData.JobFamilies, userTraits, "en").ToList();

            Assert.Equal(6,result.Count);
            Assert.Equal("CTAD",result[0].JobCategoryCode);
            Assert.Equal("RAS",result[1].JobCategoryCode);
            Assert.Equal("HAF",result[2].JobCategoryCode);
            Assert.Equal("MANAG",result[3].JobCategoryCode);
            Assert.Equal("SAL",result[4].JobCategoryCode);
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

            var result = _assessmentCalculationService.CalculateJobFamilyRelevance(ResultTestMockData.JobFamilies, userTraits, "en").ToList();

            Assert.Equal(10,result.Count);
            Assert.Equal("HEALT",result[0].JobCategoryCode);
            Assert.Equal("AC",result[1].JobCategoryCode);
            Assert.Equal("CAT",result[2].JobCategoryCode);
            Assert.Equal("CAM",result[3].JobCategoryCode);
            Assert.Equal("EAUS",result[4].JobCategoryCode);
            Assert.Equal("CTAD",result[5].JobCategoryCode);
            Assert.Equal("TAE",result[6].JobCategoryCode);
            Assert.Equal("TAT",result[7].JobCategoryCode);
            Assert.Equal("ADMIN",result[8].JobCategoryCode);
            Assert.Equal("RAS",result[9].JobCategoryCode);
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

            var result = _assessmentCalculationService.CalculateJobFamilyRelevance(ResultTestMockData.JobFamilies, userTraits, "en").ToList();

            Assert.Empty(result);
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

            var result = _assessmentCalculationService.CalculateJobFamilyRelevance(ResultTestMockData.JobFamilies, userTraits, "en").ToList();

            Assert.Empty(result);
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

            var result = _assessmentCalculationService.CalculateJobFamilyRelevance(ResultTestMockData.JobFamilies, userTraits, "en").ToList();

            Assert.Single(result);
            Assert.Equal("MANAG",result[0].JobCategoryCode);
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

            var result = _assessmentCalculationService.CalculateJobFamilyRelevance(ResultTestMockData.JobFamilies, userTraits, "en").ToList();
            
            Assert.Equal(2, result.Count);
            Assert.Equal("CTAD",result[0].JobCategoryCode);
            Assert.Equal("SC",result[1].JobCategoryCode);
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

            var result = _assessmentCalculationService.CalculateJobFamilyRelevance(ResultTestMockData.JobFamilies, userTraits, "en").ToList();

            Assert.Equal(6,result.Count);
            Assert.Equal("HS",result[0].JobCategoryCode);
            Assert.Equal("TRANS",result[1].JobCategoryCode);
            Assert.Equal("DAS",result[2].JobCategoryCode);
            Assert.Equal("GS",result[3].JobCategoryCode);
            Assert.Equal("EAL",result[4].JobCategoryCode);
            Assert.Equal("EAM",result[5].JobCategoryCode);

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

            var result = _assessmentCalculationService.CalculateJobFamilyRelevance(ResultTestMockData.JobFamilies, userTraits, "en").ToList();
            
            Assert.Equal(4, result.Count);
            Assert.Equal("SAR",result[0].JobCategoryCode);
            Assert.Equal("MANUF",result[1].JobCategoryCode);
            Assert.Equal("ADMIN",result[2].JobCategoryCode);
            Assert.Equal("GS",result[3].JobCategoryCode);
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

            var result = _assessmentCalculationService.CalculateJobFamilyRelevance(ResultTestMockData.JobFamilies, userTraits, "en").ToList();

            Assert.Equal(10, result.Count);
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


        public static List<JobCategory> JobFamilies = new List<JobCategory>()
        {
            new JobCategory()
            {
               
                Name = "Managerial",
                Traits = new [] { "LEADER", "DRIVER" },
                Texts = new []
                {
                    new JobCategoryText()
                    {
                        LanguageCode = "en",
                        Text = "To do managerial jobs, you might need leadership skills, the ability to motivate and manage staff, and the ability to monitor your own performance and that of your colleagues.",
                        Url = "managerial"
                    }
                }
            },
            new JobCategory()
            {
               
                Name = "Beauty and wellbeing",
                Traits = new []{ "DRIVER", "DOER" },
                Texts = new []
                {
                    new JobCategoryText()
                    {
                        LanguageCode = "en",
                        Text = "To do beauty and wellbeing jobs, you might need customer service skills, sensitivity and understanding, or the ability to work well with your hands.",
                        Url = "beauty-and-wellbeing"
                    }
                }
            },
            new JobCategory()
            {
               
                Name = "Science and research",
                Traits = new [] { "DRIVER", "ANALYST", "ORGANISER" },
                Texts = new []
                {
                    new JobCategoryText()
                    {
                        LanguageCode = "en",
                        Text = "To do science and research jobs, you might need the ability to operate and control equipment, or to be thorough and pay attention to detail, or observation and recording skills.",
                        Url = "science-and-research"
                    }
                }
            },
            new JobCategory()
            {
               
                Name = "Manufacturing",
                Traits = new [] { "DRIVER", "ANALYST", "ORGANISER" },
                Texts = new []
                {
                    new JobCategoryText()
                    {
                        LanguageCode = "en",
                        Text = "To do manufacturing jobs, you might need to be thorough and pay attention to detail, physical skills like movement, coordination, dexterity and grace, or the ability to work well with your hands.",
                        Url = "manufacturing"
                    }
                }
            },
            new JobCategory()
            {
               
                Name = "Teaching and education",
                Traits = new [] { "LEADER", "HELPER", "ORGANISER" },
                Texts = new []
                {
                    new JobCategoryText()
                    {
                        LanguageCode = "en",
                        Text = "To do teaching and education jobs, you might need counselling skills including active listening and a non-judgemental approach, knowledge of teaching and the ability to design courses, or sensitivity and understanding.",
                        Url = "teaching-and-education"
                    }
                }
            },
            new JobCategory()
            {
               
                Name = "Business and finance",
                Traits = new [] { "DRIVER", "ORGANISER", "DOER" },
                Texts = new []
                {
                    new JobCategoryText()
                    {
                        LanguageCode = "en",
                        Text = "To do business and finance jobs, you might need to be thorough and pay attention to detail, administration skills, or maths knowledge.",
                        Url = "business-and-finance"
                    }
                }
            },
            new JobCategory()
            {
               
                Name = "Law and legal",
                Traits = new [] { "DRIVER", "ORGANISER", "DOER" },
                Texts = new []
                {
                    new JobCategoryText()
                    {
                        LanguageCode = "en",
                        Text = "To do law and legal jobs, you might need persuading and negotiating skills, active listening skills the ability to accept criticism and work well under pressure, or to be thorough and pay attention to detail.",
                        Url = "law-and-legal"
                    }
                }
            },
            new JobCategory()
            {
               
                Name = "Computing, technology and digital",
                Traits = new [] { "ANALYST", "CREATOR" },
                Texts = new []
                {
                    new JobCategoryText()
                    {
                        LanguageCode = "en",
                        Text = "To do computing, technology and digital jobs, you might need analytical thinking skills, the ability to come up with new ways of doing things, or a thorough understanding of computer systems and applications.",
                        Url = "computing-technology-and-digital"
                    }
                }
            },
            new JobCategory()
            {
               
                Name = "Social care",
                Traits = new [] { "HELPER" },
                Texts = new []
                {
                    new JobCategoryText()
                    {
                        LanguageCode = "en",
                        Text = "To do social care jobs, you might need sensitivity and understanding patience and the ability to remain calm in stressful situations, the ability to work well with others, or excellent verbal communication skills.",
                        Url = "social-care"
                    }
                }
            },
            new JobCategory()
            {
               
                Name = "Healthcare",
                Traits = new [] { "HELPER", "ANALYST", "DOER" },
                Texts = new []
                {
                    new JobCategoryText()
                    {
                        LanguageCode = "en",
                        Text = "To do healthcare jobs, you might need sensitivity and understanding, the ability to work well with others, or excellent verbal communication skills.",
                        Url = "healthcare"
                    }
                }
            },
            new JobCategory()
            {
               
                Name = "Animal care",
                Traits = new [] { "HELPER", "ANALYST", "DOER" },
                Texts = new []
                {
                    new JobCategoryText()
                    {
                        LanguageCode = "en",
                        Text = "To do animal care jobs, you might need the ability to use your initiative, patience and the ability to remain calm in stressful situations, or the ability to accept criticism and work well under pressure.",
                        Url = "animal-care"
                    }
                }
            },
            new JobCategory()
            {
               
                Name = "Emergency and uniform services",
                Traits = new [] { "LEADER", "HELPER", "DOER" },
                Texts = new []
                {
                    new JobCategoryText()
                    {
                        LanguageCode = "en",
                        Text = "To do emergency and uniform service jobs, you might need knowledge of public safety and security, the ability to accept criticism and work well under pressure, or patience and the ability to remain calm in stressful situations.",
                        Url = "emergency-and-uniform-services"
                    }
                }
            },
            new JobCategory()
            {
               
                Name = "Sports and leisure",
                Traits = new [] { "DRIVER", "CREATOR" },
                Texts = new []
                {
                    new JobCategoryText()
                    {
                        LanguageCode = "en",
                        Text = "To do sports and leisure jobs, you might need the ability to work well with others, to enjoy working with other people, or knowledge of teaching and the ability to design courses.",
                        Url = "sports-and-leisure"
                    }
                }
            },
            new JobCategory()
            {
               
                Name = "Travel and tourism",
                Traits = new [] { "HELPER", "ORGANISER", "DOER" },
                Texts = new []
                {
                    new JobCategoryText()
                    {
                        LanguageCode = "en",
                        Text = "To do travel and tourism jobs, you might need excellent verbal communication skills, the ability to sell products and services, or active listening skills.",
                        Url = "travel-and-tourism"
                    }
                }
            },
            new JobCategory()
            {
               
                Name = "Administration",
                Traits = new [] { "ANALYST", "ORGANISER" },
                Texts = new []
                {
                    new JobCategoryText()
                    {
                        LanguageCode = "en",
                        Text = "To do administration jobs, you might need administration skills, the ability to work well with others, or customer service skills.",
                        Url = "administration"
                    }
                }
            },
            new JobCategory()
            {
               
                Name = "Government services",
                Traits = new [] { "ORGANISER" },
                Texts = new []
                {
                    new JobCategoryText()
                    {
                        LanguageCode = "en",
                        Text = "To do government services jobs, you might need the ability to accept criticism and work well under pressure, to be thorough and pay attention to detail, and customer service skills.",
                        Url = "government-services"
                    }
                }
            },
            new JobCategory()
            {
               
                Name = "Home services",
                Traits = new [] { "ORGANISER", "DOER" },
                Texts = new []
                {
                    new JobCategoryText()
                    {
                        LanguageCode = "en",
                        Text = "To do home services jobs, you might need customer service skills, business management skills, or administration skills, or the ability to accept criticism and work well under pressure.",
                        Url = "home-services"
                    }
                }
            },
            new JobCategory()
            {
               
                Name = "Environment and land",
                Traits = new [] { "DOER" },
                Texts = new []
                {
                    new JobCategoryText()
                    {
                        LanguageCode = "en",
                        Text = "To do environment and land jobs, you might need thinking and reasoning skills, to be thorough and pay attention to detail, or analytical thinking skills.",
                        Url = "environment-and-land"
                    }
                }
            },
            new JobCategory()
            {
               
                Name = "Construction and trades",
                Traits = new [] { "ANALYST", "CREATOR", "DOER" },
                Texts = new []
                {
                    new JobCategoryText()
                    {
                        LanguageCode = "en",
                        Text = "To do construction and trades jobs, you might need knowledge of building and construction, patience and the ability to remain calm in stressful situations, and the ability to work well with your hands.",
                        Url = "construction-and-trades"
                    }
                }
            },
            new JobCategory()
            {
               
                Name = "Creative and media",
                Traits = new [] { "ANALYST", "CREATOR", "DOER" },
                Texts = new []
                {
                    new JobCategoryText()
                    {
                        LanguageCode = "en",
                        Text = "To do creative and media jobs, you might need the ability to come up with new ways of doing things, the ability to use your initiative, or the ability to organise your time and workload.",
                        Url = "creative-and-media"
                    }
                }
            },
            new JobCategory()
            {
               
                Name = "Retail and sales",
                Traits = new [] { "INFLUENCER", "HELPER" },
                Texts = new []
                {
                    new JobCategoryText()
                    {
                        LanguageCode = "en",
                        Text = "To do retail and sales jobs, you might need customer service skills, the ability to work well with others, or the ability to sell products and services.",
                        Url = "retail-and-sales"
                    }
                }
            },
            new JobCategory()
            {
               
                Name = "Hospitality and food",
                Traits = new [] { "INFLUENCER", "HELPER" },
                Texts = new []
                {
                    new JobCategoryText()
                    {
                        LanguageCode = "en",
                        Text = "To do hospitality and food jobs, you might need customer service skills, the ability to sell products and services, or to enjoy working with other people.",
                        Url = "hospitality-and-food"
                    }
                }
            },
            new JobCategory()
            {
               
                Name = "Engineering and maintenance",
                Traits = new [] { "DOER" },
                Texts = new []
                {
                    new JobCategoryText()
                    {
                        LanguageCode = "en",
                        Text = "To do engineering and maintenance jobs, you might need knowledge of engineering science and technology, to be thorough and pay attention to detail, or analytical thinking skills.",
                        Url = "engineering-and-maintenance"
                    }
                }
            },
            new JobCategory()
            {
               
                Name = "Transport",
                Traits = new [] { "ORGANISER", "DOER" },
                Texts = new []
                {
                    new JobCategoryText()
                    {
                        LanguageCode = "en",
                        Text = "To do transport jobs, you might need customer service skills, knowledge of public safety and security, or the ability to operate and control equipment.",
                        Url = "transport"
                    }
                }
            },
            new JobCategory()
            {
               
                Name = "Delivery and storage",
                Traits = new [] { "ORGANISER", "DOER" },
                Texts = new []
                {
                    new JobCategoryText()
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
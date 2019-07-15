﻿using Dfc.DiscoverSkillsAndCareers.AssessmentFunctionApp.Services;
using Dfc.DiscoverSkillsAndCareers.Models;
using System;
using System.Collections.Generic;
using Xunit;
using System.Linq;
using Dfc.DiscoverSkillsAndCareers.Repositories;
using Microsoft.Extensions.Logging;
using NSubstitute;
using System.Threading.Tasks;

namespace Dfc.UnitTests
{
    public class AssessmentCalculationServiceTests
    {
        private static List<Trait> Traits;
        private static List<JobCategory> JobFamilies;
        private static List<QuestionSet> QuestionSets;
        private static Dictionary<AnswerOption, int> AnswerOptions;

        private IJobCategoryRepository _jobCategoryRepository;
        private IShortTraitRepository _shortTraitRepository;
        private IQuestionSetRepository _questionSetRepository;
        private ILogger _logger;
        private AssessmentCalculationService _sut;
        private IQuestionRepository _questionRepository;

        public AssessmentCalculationServiceTests()
        {
            _jobCategoryRepository = Substitute.For<IJobCategoryRepository>();
            _shortTraitRepository = Substitute.For<IShortTraitRepository>();
            _questionRepository = Substitute.For<IQuestionRepository>();
            _questionSetRepository = Substitute.For<IQuestionSetRepository>();
            _logger = Substitute.For<ILogger>();

            _sut = new AssessmentCalculationService(
                _jobCategoryRepository,
                _shortTraitRepository,
                _questionRepository,
                _questionSetRepository);

            Traits = new List<Trait>()
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
            JobFamilies = new List<JobCategory>()
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
                            Url = "https://nationalcareers.service.gov.uk/job-categories/managerial"
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
                            Url = "https://nationalcareers.service.gov.uk/job-categories/beauty-and-wellbeing"
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
                            Url = "https://nationalcareers.service.gov.uk/job-categories/science-and-research"
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
                            Url = "https://nationalcareers.service.gov.uk/job-categories/manufacturing"
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
                            Url = "https://nationalcareers.service.gov.uk/job-categories/teaching-and-education"
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
                            Url = "https://nationalcareers.service.gov.uk/job-categories/business-and-finance"
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
                            Url = "https://nationalcareers.service.gov.uk/job-categories/law-and-legal"
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
                            Url = "https://nationalcareers.service.gov.uk/job-categories/computing-technology-and-digital"
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
                            Url = "https://nationalcareers.service.gov.uk/job-categories/social-care"
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
                            Url = "https://nationalcareers.service.gov.uk/job-categories/healthcare"
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
                            Url = "https://nationalcareers.service.gov.uk/job-categories/animal-care"
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
                            Url = "https://nationalcareers.service.gov.uk/job-categories/emergency-and-uniform-services"
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
                            Url = "https://nationalcareers.service.gov.uk/job-categories/sports-and-leisure"
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
                            Url = "https://nationalcareers.service.gov.uk/job-categories/travel-and-tourism"
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
                            Url = "https://nationalcareers.service.gov.uk/job-categories/administration"
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
                            Url = "https://nationalcareers.service.gov.uk/job-categories/government-services"
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
                            Url = "https://nationalcareers.service.gov.uk/job-categories/home-services"
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
                            Url = "https://nationalcareers.service.gov.uk/job-categories/environment-and-land"
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
                            Url = "https://nationalcareers.service.gov.uk/job-categories/construction-and-trades"
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
                            Url = "https://nationalcareers.service.gov.uk/job-categories/creative-and-media"
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
                            Url = "https://nationalcareers.service.gov.uk/job-categories/retail-and-sales"
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
                            Url = "https://nationalcareers.service.gov.uk/job-categories/hospitality-and-food"
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
                            Url = "https://nationalcareers.service.gov.uk/job-categories/engineering-and-maintenance"
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
                            Url = "https://nationalcareers.service.gov.uk/job-categories/transport"
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
                            Url = "https://nationalcareers.service.gov.uk/job-categories/delivery-and-storage"
                        }
                    }
                },
            };
            AnswerOptions = new Dictionary<AnswerOption, int>()
            {
                { AnswerOption.StronglyDisagree, -2 },
                { AnswerOption.Disagree, -1 },
                { AnswerOption.Neutral, 0 },
                { AnswerOption.Agree, 1 },
                { AnswerOption.StronglyAgree, 2 },
            };
            QuestionSets = 
                JobFamilies
                       .Select(jf => new QuestionSet
                       {
                           Title = jf.Name,
                           MaxQuestions = 3,
                           QuestionSetKey = jf.Name.Replace(" ", "-").ToLower(),
                       })
                        .ToList();
        }

        [Fact]
        public async Task CalculateAssessment_ShouldThrow_ExceptionOnNoJobFamilies()
        {
            _jobCategoryRepository.GetJobCategories().Returns(Task.FromResult(new JobCategory[] {}));

            await Assert.ThrowsAsync<Exception>(() => _sut.CalculateAssessment(new UserSession(), _logger));
        }
        
        [Fact]
        public async Task CalculateAssessment_ShouldThrow_ExceptionOnNoTraits()
        {
            _jobCategoryRepository.GetJobCategories().Returns(Task.FromResult(new []
            {
                new JobCategory(),
            }));
            
            _shortTraitRepository.GetTraits().Returns(Task.FromResult(new Trait[] {}));
            
            await Assert.ThrowsAsync<Exception>(() => _sut.CalculateAssessment(new UserSession(), _logger));
        }
        
        [Fact]
        public async Task CalculateAssessment_ShouldThrow_ExceptionOnNoQuestions()
        {
            _jobCategoryRepository.GetJobCategories().Returns(Task.FromResult(new []
            {
                new JobCategory(),
            }));

            _shortTraitRepository.GetTraits().Returns(Task.FromResult(new[]
            {
                new Trait(),
            }));
            
            _questionSetRepository.GetCurrentQuestionSet("filtered").Returns(Task.FromResult(new QuestionSet
            {
                QuestionSetVersion = "qs-1"
            }));
            
            _questionRepository.GetQuestions("qs-1").Returns(Task.FromResult(new Question[] {}));
            
            await Assert.ThrowsAsync<Exception>(() => _sut.CalculateAssessment(new UserSession(), _logger));
        }
        
        [Fact]
        public async Task CalculateAssessment_ShouldThrow_ExceptionOnNoUserSessionAssessmentState()
        {
            _jobCategoryRepository.GetJobCategories().Returns(Task.FromResult(new []
            {
                new JobCategory(),
            }));

            _shortTraitRepository.GetTraits().Returns(Task.FromResult(new[]
            {
                new Trait(),
            }));

            _questionSetRepository.GetCurrentQuestionSet("filtered").Returns(Task.FromResult(new QuestionSet
            {
                QuestionSetVersion = "qs-1"
            }));

            _questionRepository.GetQuestions("qs-1").Returns(Task.FromResult(new[]
            {
                new Question(),
            }));
            
            await Assert.ThrowsAsync<Exception>(() => _sut.CalculateAssessment(new UserSession(), _logger));
        }

        [Fact]
        public void PrepareFilterAssessmentState_ShouldAdd_CorrectCategories()
        {
            var questions = new []
            {
                new Question {TraitCode = "A", Order = 1},
                new Question {TraitCode = "B", Order = 1},
            };

            var categories = new[]
            {
                new JobCategory
                {
                    Name = "Animal Care", Skills = new List<JobProfileSkillMapping>
                    {
                        new JobProfileSkillMapping {ONetAttribute = "A"}
                    }
                },
            };
            
            var session = new UserSession
            {
                ResultData = new ResultData
                {
                    JobCategories = new[]
                    {
                        new JobCategoryResult {JobCategoryName = "Animal Care"},
                    }
                }
            };
            
            _sut.PrepareFilterAssessmentState("QS-1", session, categories, questions);

            Assert.Contains(session.FilteredAssessmentState.JobCategoryStates, a => a.JobCategoryCode == "AC");
        }
        
        [Fact]
        public void RunShortAssessment_WithAllNeutral_SHouldHaveNoTraits()
        {
            var userSession = new UserSession()
            {
                LanguageCode = "en",
                AssessmentState = new AssessmentState("qs-1",8) {
                    RecordedAnswers = new []
                    {
                        new Answer() { TraitCode = "LEADER", SelectedOption = AnswerOption.Neutral },
                        new Answer() { TraitCode = "DRIVER", SelectedOption = AnswerOption.Neutral },
                        new Answer() { TraitCode = "INFLUENCER", SelectedOption = AnswerOption.Neutral },
                        new Answer() { TraitCode = "HELPER", SelectedOption = AnswerOption.Neutral },
                        new Answer() { TraitCode = "ANALYST", SelectedOption = AnswerOption.Neutral },
                        new Answer() { TraitCode = "CREATOR", SelectedOption = AnswerOption.Neutral },
                        new Answer() { TraitCode = "ORGANISER", SelectedOption = AnswerOption.Neutral },
                        new Answer() { TraitCode = "DOER", SelectedOption = AnswerOption.Neutral }
                    }
                }
            };

            
            _sut.RunShortAssessment(userSession, JobFamilies, AnswerOptions, Traits);

            var traits = userSession.ResultData.Traits.ToList();
            Assert.Empty(traits);
        }

        [Fact]
        public void RunShortAssessment_WithDoer_SHouldHaveDoerTrait()
        {
            var userSession = new UserSession()
            {
                LanguageCode = "en",
                AssessmentState = new AssessmentState("qs-1",8) {
                    RecordedAnswers = new[]
                    {
                        new Answer() { TraitCode = "LEADER", SelectedOption = AnswerOption.Neutral },
                        new Answer() { TraitCode = "DRIVER", SelectedOption = AnswerOption.Neutral },
                        new Answer() { TraitCode = "INFLUENCER", SelectedOption = AnswerOption.Neutral },
                        new Answer() { TraitCode = "HELPER", SelectedOption = AnswerOption.Neutral },
                        new Answer() { TraitCode = "ANALYST", SelectedOption = AnswerOption.Neutral },
                        new Answer() { TraitCode = "CREATOR", SelectedOption = AnswerOption.Neutral },
                        new Answer() { TraitCode = "ORGANISER", SelectedOption = AnswerOption.Neutral },
                        new Answer() { TraitCode = "DOER", SelectedOption = AnswerOption.Agree }
                    }
                }
            };

            _sut.RunShortAssessment(userSession, JobFamilies, AnswerOptions, Traits);

            var traits = userSession.ResultData.Traits.ToList();
            Assert.Single(traits);
        }

        [Fact]
        public void RunShortAssessment_WithAllAgree_ShouldHaveAllTraits()
        {
            var userSession = new UserSession()
            {
                LanguageCode = "en",
                AssessmentState = new AssessmentState("qs-1",8) {
                    RecordedAnswers = new[]
                    {
                        new Answer() { TraitCode = "LEADER", SelectedOption = AnswerOption.Agree },
                        new Answer() { TraitCode = "DRIVER", SelectedOption = AnswerOption.Agree },
                        new Answer() { TraitCode = "INFLUENCER", SelectedOption = AnswerOption.Agree },
                        new Answer() { TraitCode = "HELPER", SelectedOption = AnswerOption.Agree },
                        new Answer() { TraitCode = "ANALYST", SelectedOption = AnswerOption.Agree },
                        new Answer() { TraitCode = "CREATOR", SelectedOption = AnswerOption.Agree },
                        new Answer() { TraitCode = "ORGANISER", SelectedOption = AnswerOption.Agree },
                        new Answer() { TraitCode = "DOER", SelectedOption = AnswerOption.Agree }
                    }
                }
            };

            _sut.RunShortAssessment(userSession, JobFamilies, AnswerOptions, Traits);

            var traits = userSession.ResultData.Traits.ToList();
            Assert.Equal(8, traits.Count);
        }

        [Fact]
        public void RunShortAssessment_WithAllDisagree_ShouldHaveNoTraits()
        {
            var userSession = new UserSession()
            {
                LanguageCode = "en",
                AssessmentState = new AssessmentState("qs-1",8) {
                    RecordedAnswers = new[]
                    {
                        new Answer() { TraitCode = "LEADER", SelectedOption = AnswerOption.Disagree },
                        new Answer() { TraitCode = "DRIVER", SelectedOption = AnswerOption.Disagree },
                        new Answer() { TraitCode = "INFLUENCER", SelectedOption = AnswerOption.Disagree },
                        new Answer() { TraitCode = "HELPER", SelectedOption = AnswerOption.Disagree },
                        new Answer() { TraitCode = "ANALYST", SelectedOption = AnswerOption.Disagree },
                        new Answer() { TraitCode = "CREATOR", SelectedOption = AnswerOption.Disagree },
                        new Answer() { TraitCode = "ORGANISER", SelectedOption = AnswerOption.Disagree },
                        new Answer() { TraitCode = "DOER", SelectedOption = AnswerOption.Disagree }
                    }
                }
            };

            _sut.RunShortAssessment(userSession, JobFamilies, AnswerOptions, Traits);

            var traits = userSession.ResultData.Traits.ToList();
            Assert.Empty(traits);
        }

        [Fact]
        public void RunShortAssessment_WithAllStronglyDisagree_ShouldHaveNoTraits()
        {
            var userSession = new UserSession()
            {
                LanguageCode = "en",
                AssessmentState = new AssessmentState("qs-1",8) {
                    RecordedAnswers = new[]
                    {
                        new Answer() { TraitCode = "LEADER", SelectedOption = AnswerOption.StronglyDisagree },
                        new Answer() { TraitCode = "DRIVER", SelectedOption = AnswerOption.StronglyDisagree },
                        new Answer() { TraitCode = "INFLUENCER", SelectedOption = AnswerOption.StronglyDisagree },
                        new Answer() { TraitCode = "HELPER", SelectedOption = AnswerOption.StronglyDisagree },
                        new Answer() { TraitCode = "ANALYST", SelectedOption = AnswerOption.StronglyDisagree },
                        new Answer() { TraitCode = "CREATOR", SelectedOption = AnswerOption.StronglyDisagree },
                        new Answer() { TraitCode = "ORGANISER", SelectedOption = AnswerOption.StronglyDisagree },
                        new Answer() { TraitCode = "DOER", SelectedOption = AnswerOption.StronglyDisagree }
                    }
                }
            };

            _sut.RunShortAssessment(userSession, JobFamilies, AnswerOptions, Traits);

            var traits = userSession.ResultData.Traits.ToList();
            Assert.Empty(traits);
        }

        [Fact]
        public void RunShortAssessment_WithAllStronglyDisagreeWithDoerNegative_ShouldHaveNoTraits()
        {
            var userSession = new UserSession()
            {
                LanguageCode = "en",
                AssessmentState = new AssessmentState("qs-1",12) {
                    RecordedAnswers = new[]
                    {
                        new Answer() { TraitCode = "LEADER", SelectedOption = AnswerOption.StronglyDisagree },
                        new Answer() { TraitCode = "DRIVER", SelectedOption = AnswerOption.StronglyDisagree },
                        new Answer() { TraitCode = "INFLUENCER", SelectedOption = AnswerOption.StronglyDisagree },
                        new Answer() { TraitCode = "HELPER", SelectedOption = AnswerOption.StronglyDisagree },
                        new Answer() { TraitCode = "ANALYST", SelectedOption = AnswerOption.StronglyDisagree },
                        new Answer() { TraitCode = "CREATOR", SelectedOption = AnswerOption.StronglyDisagree },
                        new Answer() { TraitCode = "ORGANISER", SelectedOption = AnswerOption.StronglyDisagree },
                        new Answer() { TraitCode = "DOER", SelectedOption = AnswerOption.StronglyDisagree },
                        new Answer() { TraitCode = "DOER", SelectedOption = AnswerOption.StronglyDisagree },
                        new Answer() { TraitCode = "DOER", SelectedOption = AnswerOption.StronglyDisagree },
                        new Answer() { TraitCode = "DOER", SelectedOption = AnswerOption.StronglyDisagree },
                        new Answer() { TraitCode = "DOER", SelectedOption = AnswerOption.StronglyDisagree, IsNegative = true },
                    }
                }
            };

            _sut.RunShortAssessment(userSession, JobFamilies, AnswerOptions, Traits);

            var traits = userSession.ResultData.Traits.ToList();
            Assert.Empty(traits);
            var doerScore = userSession.ResultData.TraitScores.Where(x => x.TraitCode == "DOER").Sum(x => x.TotalScore);
            Assert.Equal(-6, doerScore);
        }

        [Fact]
        public void RunShortAssessment_UMB337_DiametricallyOpposedAnswers_StronglyAgreeToStronglyDisagree_ShouldEqualZero()
        {
           
            var session = new UserSession()
            {
                LanguageCode = "en",
                AssessmentState = new AssessmentState("qs-1",40) {
                    RecordedAnswers = new []
                    {
                        new Answer { TraitCode = "LEADER", SelectedOption = AnswerOption.StronglyAgree, IsNegative = false},
                        new Answer { TraitCode = "LEADER", SelectedOption = AnswerOption.Agree, IsNegative = false},
                        new Answer { TraitCode = "LEADER", SelectedOption = AnswerOption.Neutral, IsNegative = false},
                        new Answer { TraitCode = "LEADER", SelectedOption = AnswerOption.Agree, IsNegative = true},
                        new Answer { TraitCode = "LEADER", SelectedOption = AnswerOption.StronglyDisagree, IsNegative = false},
                        new Answer { TraitCode = "DRIVER", SelectedOption = AnswerOption.StronglyAgree, IsNegative = false},
                        new Answer { TraitCode = "DRIVER", SelectedOption = AnswerOption.Agree, IsNegative = false},
                        new Answer { TraitCode = "DRIVER", SelectedOption = AnswerOption.Neutral, IsNegative = false},
                        new Answer { TraitCode = "DRIVER", SelectedOption = AnswerOption.Disagree, IsNegative = false},
                        new Answer { TraitCode = "DRIVER", SelectedOption = AnswerOption.StronglyDisagree, IsNegative = false},
                        new Answer { TraitCode = "INFLUENCER", SelectedOption = AnswerOption.StronglyAgree, IsNegative = false},
                        new Answer { TraitCode = "INFLUENCER", SelectedOption = AnswerOption.Agree, IsNegative = false},
                        new Answer { TraitCode = "INFLUENCER", SelectedOption = AnswerOption.Neutral, IsNegative = false},
                        new Answer { TraitCode = "INFLUENCER", SelectedOption = AnswerOption.Disagree, IsNegative = false},
                        new Answer { TraitCode = "INFLUENCER", SelectedOption = AnswerOption.StronglyAgree, IsNegative = true},
                        new Answer { TraitCode = "HELPER", SelectedOption = AnswerOption.StronglyAgree, IsNegative = false},
                        new Answer { TraitCode = "HELPER", SelectedOption = AnswerOption.Agree, IsNegative = false},
                        new Answer { TraitCode = "HELPER", SelectedOption = AnswerOption.Neutral, IsNegative = false},
                        new Answer { TraitCode = "HELPER", SelectedOption = AnswerOption.Disagree, IsNegative = false},
                        new Answer { TraitCode = "HELPER", SelectedOption = AnswerOption.StronglyDisagree, IsNegative = false},
                        new Answer { TraitCode = "ANALYST", SelectedOption = AnswerOption.StronglyAgree, IsNegative = false},
                        new Answer { TraitCode = "ANALYST", SelectedOption = AnswerOption.Agree, IsNegative = false},
                        new Answer { TraitCode = "ANALYST", SelectedOption = AnswerOption.Neutral, IsNegative = false},
                        new Answer { TraitCode = "ANALYST", SelectedOption = AnswerOption.Disagree, IsNegative = false},
                        new Answer { TraitCode = "ANALYST", SelectedOption = AnswerOption.StronglyDisagree, IsNegative = false},
                        new Answer { TraitCode = "CREATOR", SelectedOption = AnswerOption.StronglyAgree, IsNegative = false},
                        new Answer { TraitCode = "CREATOR", SelectedOption = AnswerOption.Agree, IsNegative = false},
                        new Answer { TraitCode = "CREATOR", SelectedOption = AnswerOption.Neutral, IsNegative = false},
                        new Answer { TraitCode = "CREATOR", SelectedOption = AnswerOption.Disagree, IsNegative = false},
                        new Answer { TraitCode = "CREATOR", SelectedOption = AnswerOption.StronglyDisagree, IsNegative = false},
                        new Answer { TraitCode = "ORGANISER", SelectedOption = AnswerOption.StronglyAgree, IsNegative = false},
                        new Answer { TraitCode = "ORGANISER", SelectedOption = AnswerOption.Agree, IsNegative = false},
                        new Answer { TraitCode = "ORGANISER", SelectedOption = AnswerOption.Neutral, IsNegative = false},
                        new Answer { TraitCode = "ORGANISER", SelectedOption = AnswerOption.Disagree, IsNegative = false},
                        new Answer { TraitCode = "ORGANISER", SelectedOption = AnswerOption.StronglyAgree, IsNegative = true},
                        new Answer { TraitCode = "DOER", SelectedOption = AnswerOption.StronglyAgree, IsNegative = false},
                        new Answer { TraitCode = "DOER", SelectedOption = AnswerOption.Agree, IsNegative = false},
                        new Answer { TraitCode = "DOER", SelectedOption = AnswerOption.Neutral, IsNegative = false},
                        new Answer { TraitCode = "DOER", SelectedOption = AnswerOption.Disagree, IsNegative = false},
                        new Answer { TraitCode = "DOER", SelectedOption = AnswerOption.StronglyAgree, IsNegative = true},
                    }
                }
                    
            };

            _sut.RunShortAssessment(session, JobFamilies, AnswerOptions, Traits);

            int? getTraitScore(string trait)
            {
                return session.ResultData.TraitScores.SingleOrDefault(t => t.TraitCode == trait)?.TotalScore;
            }
            
            Assert.Equal(0, getTraitScore("LEADER"));
            Assert.Equal(0, getTraitScore("DRIVER"));
            Assert.Equal(0, getTraitScore("INFLUENCER"));
            Assert.Equal(0, getTraitScore("HELPER"));
            Assert.Equal(0, getTraitScore("ANALYST"));
            Assert.Equal(0, getTraitScore("CREATOR"));
            Assert.Equal(0, getTraitScore("ORGANISER"));
            Assert.Equal(0, getTraitScore("DOER"));
        }
        
        [Fact]
        public void RunShortAssessment_UMB337_DiametricallyOpposedAnswers_StronglyDisagreeToStronglyAgree_ShouldEqualZero()
        {
            var session = new UserSession()
            {
                LanguageCode = "en",
                AssessmentState = new AssessmentState("qs-1",40) {
                    RecordedAnswers = new []
                    {
                        new Answer { TraitCode = "LEADER", SelectedOption = AnswerOption.StronglyDisagree, IsNegative = false},
                        new Answer { TraitCode = "LEADER", SelectedOption = AnswerOption.Disagree, IsNegative = false},
                        new Answer { TraitCode = "LEADER", SelectedOption = AnswerOption.Neutral, IsNegative = false},
                        new Answer { TraitCode = "LEADER", SelectedOption = AnswerOption.Disagree, IsNegative = true},
                        new Answer { TraitCode = "LEADER", SelectedOption = AnswerOption.StronglyAgree, IsNegative = false},
                        new Answer { TraitCode = "DRIVER", SelectedOption = AnswerOption.StronglyDisagree, IsNegative = false},
                        new Answer { TraitCode = "DRIVER", SelectedOption = AnswerOption.Disagree, IsNegative = false},
                        new Answer { TraitCode = "DRIVER", SelectedOption = AnswerOption.Neutral, IsNegative = false},
                        new Answer { TraitCode = "DRIVER", SelectedOption = AnswerOption.Agree, IsNegative = false},
                        new Answer { TraitCode = "DRIVER", SelectedOption = AnswerOption.StronglyAgree, IsNegative = false},
                        new Answer { TraitCode = "INFLUENCER", SelectedOption = AnswerOption.StronglyDisagree, IsNegative = false},
                        new Answer { TraitCode = "INFLUENCER", SelectedOption = AnswerOption.Disagree, IsNegative = false},
                        new Answer { TraitCode = "INFLUENCER", SelectedOption = AnswerOption.Neutral, IsNegative = false},
                        new Answer { TraitCode = "INFLUENCER", SelectedOption = AnswerOption.Agree, IsNegative = false},
                        new Answer { TraitCode = "INFLUENCER", SelectedOption = AnswerOption.StronglyDisagree, IsNegative = true},
                        new Answer { TraitCode = "HELPER", SelectedOption = AnswerOption.StronglyDisagree, IsNegative = false},
                        new Answer { TraitCode = "HELPER", SelectedOption = AnswerOption.Disagree, IsNegative = false},
                        new Answer { TraitCode = "HELPER", SelectedOption = AnswerOption.Neutral, IsNegative = false},
                        new Answer { TraitCode = "HELPER", SelectedOption = AnswerOption.Agree, IsNegative = false},
                        new Answer { TraitCode = "HELPER", SelectedOption = AnswerOption.StronglyAgree, IsNegative = false},
                        new Answer { TraitCode = "ANALYST", SelectedOption = AnswerOption.StronglyDisagree, IsNegative = false},
                        new Answer { TraitCode = "ANALYST", SelectedOption = AnswerOption.Disagree, IsNegative = false},
                        new Answer { TraitCode = "ANALYST", SelectedOption = AnswerOption.Neutral, IsNegative = false},
                        new Answer { TraitCode = "ANALYST", SelectedOption = AnswerOption.Agree, IsNegative = false},
                        new Answer { TraitCode = "ANALYST", SelectedOption = AnswerOption.StronglyAgree, IsNegative = false},
                        new Answer { TraitCode = "CREATOR", SelectedOption = AnswerOption.StronglyDisagree, IsNegative = false},
                        new Answer { TraitCode = "CREATOR", SelectedOption = AnswerOption.Disagree, IsNegative = false},
                        new Answer { TraitCode = "CREATOR", SelectedOption = AnswerOption.Neutral, IsNegative = false},
                        new Answer { TraitCode = "CREATOR", SelectedOption = AnswerOption.Agree, IsNegative = false},
                        new Answer { TraitCode = "CREATOR", SelectedOption = AnswerOption.StronglyAgree, IsNegative = false},
                        new Answer { TraitCode = "ORGANISER", SelectedOption = AnswerOption.StronglyDisagree, IsNegative = false},
                        new Answer { TraitCode = "ORGANISER", SelectedOption = AnswerOption.Disagree, IsNegative = false},
                        new Answer { TraitCode = "ORGANISER", SelectedOption = AnswerOption.Neutral, IsNegative = false},
                        new Answer { TraitCode = "ORGANISER", SelectedOption = AnswerOption.Agree, IsNegative = false},
                        new Answer { TraitCode = "ORGANISER", SelectedOption = AnswerOption.StronglyDisagree, IsNegative = true},
                        new Answer { TraitCode = "DOER", SelectedOption = AnswerOption.StronglyDisagree, IsNegative = false},
                        new Answer { TraitCode = "DOER", SelectedOption = AnswerOption.Disagree, IsNegative = false},
                        new Answer { TraitCode = "DOER", SelectedOption = AnswerOption.Neutral, IsNegative = false},
                        new Answer { TraitCode = "DOER", SelectedOption = AnswerOption.Agree, IsNegative = false},
                        new Answer { TraitCode = "DOER", SelectedOption = AnswerOption.StronglyDisagree, IsNegative = true},
                    }
                }
                    
            };

            _sut.RunShortAssessment(session, JobFamilies, AnswerOptions, Traits);

            int? getTraitScore(string trait)
            {
                return session.ResultData.TraitScores.SingleOrDefault(t => t.TraitCode == trait)?.TotalScore;
            }
            
            Assert.Equal(0, getTraitScore("LEADER"));
            Assert.Equal(0, getTraitScore("DRIVER"));
            Assert.Equal(0, getTraitScore("INFLUENCER"));
            Assert.Equal(0, getTraitScore("HELPER"));
            Assert.Equal(0, getTraitScore("ANALYST"));
            Assert.Equal(0, getTraitScore("CREATOR"));
            Assert.Equal(0, getTraitScore("ORGANISER"));
            Assert.Equal(0, getTraitScore("DOER"));
        }
        
        [Fact]
        public void RunShortAssessment_UMB335_MissingOrganiserTrait()
        {
            var session = new UserSession()
            {
                LanguageCode = "en",
                AssessmentState = new AssessmentState("qs-1",40) {
                    RecordedAnswers = new []
                    {
                        new Answer { TraitCode = "LEADER", SelectedOption = AnswerOption.StronglyDisagree, IsNegative = false},
                        new Answer { TraitCode = "LEADER", SelectedOption = AnswerOption.StronglyDisagree, IsNegative = false},
                        new Answer { TraitCode = "LEADER", SelectedOption = AnswerOption.StronglyDisagree, IsNegative = false},
                        new Answer { TraitCode = "LEADER", SelectedOption = AnswerOption.StronglyDisagree, IsNegative = true},
                        new Answer { TraitCode = "LEADER", SelectedOption = AnswerOption.StronglyDisagree, IsNegative = false},
                        new Answer { TraitCode = "DRIVER", SelectedOption = AnswerOption.StronglyDisagree, IsNegative = false},
                        new Answer { TraitCode = "DRIVER", SelectedOption = AnswerOption.StronglyDisagree, IsNegative = false},
                        new Answer { TraitCode = "DRIVER", SelectedOption = AnswerOption.StronglyDisagree, IsNegative = false},
                        new Answer { TraitCode = "DRIVER", SelectedOption = AnswerOption.StronglyDisagree, IsNegative = false},
                        new Answer { TraitCode = "DRIVER", SelectedOption = AnswerOption.StronglyDisagree, IsNegative = false},
                        new Answer { TraitCode = "INFLUENCER", SelectedOption = AnswerOption.StronglyDisagree, IsNegative = false},
                        new Answer { TraitCode = "INFLUENCER", SelectedOption = AnswerOption.StronglyDisagree, IsNegative = false},
                        new Answer { TraitCode = "INFLUENCER", SelectedOption = AnswerOption.StronglyDisagree, IsNegative = false},
                        new Answer { TraitCode = "INFLUENCER", SelectedOption = AnswerOption.StronglyDisagree, IsNegative = false},
                        new Answer { TraitCode = "INFLUENCER", SelectedOption = AnswerOption.StronglyDisagree, IsNegative = true},
                        new Answer { TraitCode = "HELPER", SelectedOption = AnswerOption.StronglyDisagree, IsNegative = false},
                        new Answer { TraitCode = "HELPER", SelectedOption = AnswerOption.StronglyDisagree, IsNegative = false},
                        new Answer { TraitCode = "HELPER", SelectedOption = AnswerOption.StronglyDisagree, IsNegative = false},
                        new Answer { TraitCode = "HELPER", SelectedOption = AnswerOption.StronglyDisagree, IsNegative = false},
                        new Answer { TraitCode = "HELPER", SelectedOption = AnswerOption.StronglyDisagree, IsNegative = false},
                        new Answer { TraitCode = "ANALYST", SelectedOption = AnswerOption.StronglyAgree, IsNegative = false},
                        new Answer { TraitCode = "ANALYST", SelectedOption = AnswerOption.StronglyAgree, IsNegative = false},
                        new Answer { TraitCode = "ANALYST", SelectedOption = AnswerOption.StronglyAgree, IsNegative = false},
                        new Answer { TraitCode = "ANALYST", SelectedOption = AnswerOption.StronglyAgree, IsNegative = false},
                        new Answer { TraitCode = "ANALYST", SelectedOption = AnswerOption.StronglyAgree, IsNegative = false},
                        new Answer { TraitCode = "CREATOR", SelectedOption = AnswerOption.StronglyAgree, IsNegative = false},
                        new Answer { TraitCode = "CREATOR", SelectedOption = AnswerOption.StronglyAgree, IsNegative = false},
                        new Answer { TraitCode = "CREATOR", SelectedOption = AnswerOption.StronglyAgree, IsNegative = false},
                        new Answer { TraitCode = "CREATOR", SelectedOption = AnswerOption.StronglyAgree, IsNegative = false},
                        new Answer { TraitCode = "CREATOR", SelectedOption = AnswerOption.StronglyAgree, IsNegative = false},
                        new Answer { TraitCode = "ORGANISER", SelectedOption = AnswerOption.StronglyAgree, IsNegative = false},
                        new Answer { TraitCode = "ORGANISER", SelectedOption = AnswerOption.StronglyAgree, IsNegative = false},
                        new Answer { TraitCode = "ORGANISER", SelectedOption = AnswerOption.StronglyAgree, IsNegative = false},
                        new Answer { TraitCode = "ORGANISER", SelectedOption = AnswerOption.StronglyAgree, IsNegative = false},
                        new Answer { TraitCode = "ORGANISER", SelectedOption = AnswerOption.StronglyAgree, IsNegative = true},
                        new Answer { TraitCode = "DOER", SelectedOption = AnswerOption.StronglyAgree, IsNegative = false},
                        new Answer { TraitCode = "DOER", SelectedOption = AnswerOption.StronglyAgree, IsNegative = false},
                        new Answer { TraitCode = "DOER", SelectedOption = AnswerOption.StronglyAgree, IsNegative = false},
                        new Answer { TraitCode = "DOER", SelectedOption = AnswerOption.StronglyAgree, IsNegative = false},
                        new Answer { TraitCode = "DOER", SelectedOption = AnswerOption.StronglyAgree, IsNegative = true},
                    }
                }
            };

            _sut.RunShortAssessment(session, JobFamilies, AnswerOptions, Traits);

            IDictionary<string, TraitResult> traitLookup = session.ResultData.Traits.ToDictionary(r => r.TraitCode, r => r);
            Assert.Contains("DOER", traitLookup);
            Assert.Contains("ORGANISER", traitLookup);
            Assert.Contains("CREATOR", traitLookup);
            Assert.Contains("ANALYST", traitLookup);
            Assert.Equal(4, session.ResultData.Traits.Length);
        }
    }
}

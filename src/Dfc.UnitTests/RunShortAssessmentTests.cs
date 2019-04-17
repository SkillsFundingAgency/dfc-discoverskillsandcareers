using Dfc.DiscoverSkillsAndCareers.AssessmentFunctionApp.Services;
using Dfc.DiscoverSkillsAndCareers.Models;
using System;
using System.Collections.Generic;
using Xunit;
using System.Linq;

namespace Dfc.UnitTests
{
    public class RunShortAssessmentTests
    {
        private static List<Trait> Traits;
        private static List<JobFamily> JobFamilies;
        private static Dictionary<AnswerOption, int> AnswerOptions;

        public RunShortAssessmentTests()
        {
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
            JobFamilies = new List<JobFamily>()
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
                            Url = "https://nationalcareers.service.gov.uk/job-categories/managerial"
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
                            Url = "https://nationalcareers.service.gov.uk/job-categories/beauty-and-wellbeing"
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
                            Url = "https://nationalcareers.service.gov.uk/job-categories/science-and-research"
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
                            Url = "https://nationalcareers.service.gov.uk/job-categories/manufacturing"
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
                            Url = "https://nationalcareers.service.gov.uk/job-categories/teaching-and-education"
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
                            Url = "https://nationalcareers.service.gov.uk/job-categories/business-and-finance"
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
                            Url = "https://nationalcareers.service.gov.uk/job-categories/law-and-legal"
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
                            Url = "https://nationalcareers.service.gov.uk/job-categories/computing-technology-and-digital"
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
                            Url = "https://nationalcareers.service.gov.uk/job-categories/social-care"
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
                            Url = "https://nationalcareers.service.gov.uk/job-categories/healthcare"
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
                            Url = "https://nationalcareers.service.gov.uk/job-categories/animal-care"
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
                            Url = "https://nationalcareers.service.gov.uk/job-categories/emergency-and-uniform-services"
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
                            Url = "https://nationalcareers.service.gov.uk/job-categories/sports-and-leisure"
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
                            Url = "https://nationalcareers.service.gov.uk/job-categories/travel-and-tourism"
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
                            Url = "https://nationalcareers.service.gov.uk/job-categories/administration"
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
                            Url = "https://nationalcareers.service.gov.uk/job-categories/government-services"
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
                            Url = "https://nationalcareers.service.gov.uk/job-categories/home-services"
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
                            Url = "https://nationalcareers.service.gov.uk/job-categories/environment-and-land"
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
                            Url = "https://nationalcareers.service.gov.uk/job-categories/construction-and-trades"
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
                            Url = "https://nationalcareers.service.gov.uk/job-categories/creative-and-media"
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
                            Url = "https://nationalcareers.service.gov.uk/job-categories/retail-and-sales"
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
                            Url = "https://nationalcareers.service.gov.uk/job-categories/hospitality-and-food"
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
                            Url = "https://nationalcareers.service.gov.uk/job-categories/engineering-and-maintenance"
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
                            Url = "https://nationalcareers.service.gov.uk/job-categories/transport"
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
        }

        [Fact]
        public void RunShortAssessment_WithAllNeutral_SHouldHaveNoTraits()
        {
            var userSession = new UserSession()
            {
                LanguageCode = "en",
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
            };

            AssessmentCalculationService.RunShortAssessment(userSession, JobFamilies, AnswerOptions, Traits);

            var traits = userSession.ResultData.Traits.ToList();

            Assert.Equal(0, traits.Count);
        }

        [Fact]
        public void RunShortAssessment_WithDoer_SHouldHaveDoerTrait()
        {
            var userSession = new UserSession()
            {
                LanguageCode = "en",
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
            };

            AssessmentCalculationService.RunShortAssessment(userSession, JobFamilies, AnswerOptions, Traits);

            var traits = userSession.ResultData.Traits.ToList();

            Assert.Equal(1, traits.Count);
        }

        [Fact]
        public void RunShortAssessment_WithAllAgree_ShouldHaveAllTraits()
        {
            var userSession = new UserSession()
            {
                LanguageCode = "en",
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
            };

            AssessmentCalculationService.RunShortAssessment(userSession, JobFamilies, AnswerOptions, Traits);

            var traits = userSession.ResultData.Traits.ToList();

            Assert.Equal(8, traits.Count);
        }

        [Fact]
        public void RunShortAssessment_WithAllDisagree_ShouldHaveNoTraits()
        {
            var userSession = new UserSession()
            {
                LanguageCode = "en",
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
            };

            AssessmentCalculationService.RunShortAssessment(userSession, JobFamilies, AnswerOptions, Traits);

            var traits = userSession.ResultData.Traits.ToList();

            Assert.Equal(0, traits.Count);
        }

        [Fact]
        public void RunShortAssessment_WithAllStronglyDisagree_ShouldHaveNoTraits()
        {
            var userSession = new UserSession()
            {
                LanguageCode = "en",
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
            };

            AssessmentCalculationService.RunShortAssessment(userSession, JobFamilies, AnswerOptions, Traits);

            var traits = userSession.ResultData.Traits.ToList();

            Assert.Equal(0, traits.Count);
        }

        [Fact]
        public void RunShortAssessment_WithAllStronglyDisagreeWithDoerNegative_ShouldHaveNoTraits()
        {
            var userSession = new UserSession()
            {
                LanguageCode = "en",
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
            };

            AssessmentCalculationService.RunShortAssessment(userSession, JobFamilies, AnswerOptions, Traits);

            var traits = userSession.ResultData.Traits.ToList();

            Assert.Equal(0, traits.Count);
            var doerScore = userSession.ResultData.TraitScores.Where(x => x.TraitCode == "DOER").Sum(x => x.TotalScore);
            Assert.Equal(-6, doerScore);
        }
    }
}


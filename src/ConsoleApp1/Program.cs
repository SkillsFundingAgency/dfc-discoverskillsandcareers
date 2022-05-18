using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Dfc.DiscoverSkillsAndCareers.AssessmentFunctionApp.Services;
using Dfc.DiscoverSkillsAndCareers.CmsFunctionApp.Models;
using Dfc.DiscoverSkillsAndCareers.Models;
using Dfc.DiscoverSkillsAndCareers.Repositories;
using Microsoft.Azure.Documents.Client;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;

namespace ConsoleApp1
{
    class Program
    {
        async static Task Main(string[] args)
        {
            await Main2(args);
        }
        
        async static Task Main1(string[] args)
        {
            var options = new SiteFinitySettings
            {
                SiteFinityApiUrlBase = "https://dev-service.nationalcareersservice.org.uk",
                SiteFinityApiWebService = "dsac",
                SiteFinityRequiresAuthentication = true,
                SiteFinityApiAuthenicationEndpoint = "SiteFinity/Authenticate/openid/connect/token",
                SiteFinityScope = "offline_access openid",
                SiteFinityUsername = "dysac@apiusers.local",
                SiteFinityPassword = "",
                SiteFinityClientId = "dsac",
                SiteFinityClientSecret = "",
            };

            ISiteFinityHttpService _sitefinity = new SiteFinityHttpService(
                new NullLoggerFactory(),
                new OptionsWrapper<SiteFinitySettings>(options));
            
            var jobProfiles = 
                await _sitefinity.GetAll<SiteFinityJobProfile>("jobprofiles?$select=Id,Title,JobProfileCategories&$expand=RelatedSkills&$orderby=Title");

            var jobCategories = 
                await _sitefinity.GetTaxonomyInstances("Job Profile Category");
            
            var categorySkillMappings = JobCategorySkillMapper.Map(jobProfiles, jobCategories,
                0.75,
                0.75);
        }

        async static Task Main2(string[] args)
        {
            var endpoint = "";
            var key = "";
            
            var questionDocumentClient = new DocumentClient(new Uri(endpoint), key);
            var jobCategoryDocumentClient = new DocumentClient(new Uri(endpoint), key);

            var options = Options.Create(new CosmosSettings
            {
                DatabaseName = "DiscoverMySkillsAndCareers",
                Endpoint = endpoint,
                Key = key
            });
            
            var questionRepository = new QuestionRepository(questionDocumentClient, options);
            var jobCategoryRepository = new JobCategoryRepository(jobCategoryDocumentClient, options);

            var questionSetVersion = "201901-1";
            var traitCode1 = "Self Control";
            var traitCode2 = "Stress Tolerance";
            var traitCode3 = "Speaking, Verbal Abilities";
            
            var userSession = new UserSession
            {
                FilteredAssessmentState = new FilteredAssessmentState
                {
                    JobCategoryStates = new List<JobCategoryState>
                    {
                        new JobCategoryState(
                            "EAUS",
                            "Emergency and uniform services",
                            questionSetVersion,
                            new[]
                            {
                                new JobCategorySkill
                                {
                                    Skill = traitCode1
                                },
                                new JobCategorySkill
                                {
                                    Skill = traitCode2
                                },
                                new JobCategorySkill
                                {
                                    Skill = traitCode3
                                }
                            })
                    },
                    RecordedAnswers = new[]
                    {
                        new Answer { TraitCode = traitCode1, QuestionId = "1", QuestionNumber = 0, SelectedOption = AnswerOption.Yes, QuestionSetVersion = questionSetVersion},
                        new Answer { TraitCode = traitCode2, QuestionId = "2", QuestionNumber = 1, SelectedOption = AnswerOption.Yes, QuestionSetVersion = questionSetVersion},
                        new Answer { TraitCode = traitCode3, QuestionId = "3", QuestionNumber = 2, SelectedOption = AnswerOption.Yes, QuestionSetVersion = questionSetVersion}
                    }
                }
            };
            
            var log = NullLogger<FilterAssessmentCalculationService>.Instance;
            
            var filterAssessmentCalculationService = new FilterAssessmentCalculationService(questionRepository, jobCategoryRepository);
            await filterAssessmentCalculationService.CalculateAssessment(userSession, log);
        }
    }
}
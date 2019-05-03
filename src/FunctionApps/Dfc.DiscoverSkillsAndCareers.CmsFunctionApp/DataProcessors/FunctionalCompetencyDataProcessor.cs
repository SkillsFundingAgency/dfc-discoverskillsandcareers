using Dfc.DiscoverSkillsAndCareers.CmsFunctionApp.DataRequesters;
using Dfc.DiscoverSkillsAndCareers.CmsFunctionApp.Services;
using Dfc.DiscoverSkillsAndCareers.Repositories;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Threading.Tasks;
using Dfc.DiscoverSkillsAndCareers.Models;
using System.Collections.Generic;
using System;
using System.Linq;
using Dfc.DiscoverSkillsAndCareers.CmsFunctionApp.Models;
using Newtonsoft.Json;

namespace Dfc.DiscoverSkillsAndCareers.CmsFunctionApp.DataProcessors
{
    public class FunctionalCompetencyDataProcessor : IFunctionalCompetencyDataProcessor
    {
        readonly ISiteFinityHttpService HttpService;
        readonly IGetFunctionalCompetenciesData GetFunctionalCompetenciesData;
        readonly IGetJobCategoriesData GetJobCategoriesData;
        readonly AppSettings AppSettings;
        readonly IQuestionRepository QuestionRepository;
        readonly IQuestionSetRepository QuestionSetRepository;

        public FunctionalCompetencyDataProcessor(
            ISiteFinityHttpService httpService,
            IGetFunctionalCompetenciesData getFunctionalCompetenciesData,
            IGetJobCategoriesData getJobCategoriesData,
            IOptions<AppSettings> appSettings,
            IQuestionRepository questionRepository,
            IQuestionSetRepository questionSetRepository)
        {
            HttpService = httpService;
            GetFunctionalCompetenciesData = getFunctionalCompetenciesData;
            GetJobCategoriesData = getJobCategoriesData;
            AppSettings = appSettings.Value;
            QuestionRepository = questionRepository;
            QuestionSetRepository = questionSetRepository;
        }

        public async Task RunOnce(ILogger logger)
        {
            try
            {
                logger.LogInformation("Begin poll for FunctionalCompetencies");

                string siteFinityApiUrlbase = AppSettings.SiteFinityApiUrlbase;
                string siteFinityService = AppSettings.SiteFinityApiWebService;

                var functionalCompetencies = await GetFunctionalCompetenciesData.GetData(siteFinityApiUrlbase, siteFinityService);
                var jobCategories =
                    (await GetJobCategoriesData.GetData(siteFinityApiUrlbase, siteFinityService, AppSettings.SiteFinityJobCategoriesTaxonomyId))
                        .ToDictionary(x => x.Id, x => x.Title, StringComparer.InvariantCultureIgnoreCase);

                logger.LogInformation($"Have {functionalCompetencies?.Count} functional competencies to process");
                var sets = new List<QuestionSet>();
                foreach (var functionalCompetency in functionalCompetencies.Where(fc => fc.JobProfileCategories != null && jobCategories.ContainsKey(fc.JobCategory)))
                {
                    if (functionalCompetency.JobProfileCategories == null || functionalCompetency.JobProfileCategories.Count() == 0)
                    {
                        throw new Exception($"JobProfileCategories have not been determined for functionalCompetency {functionalCompetency.Id}");
                    }

                    var questionSet = await QuestionSetRepository.GetCurrentQuestionSet("filtered", jobCategories[functionalCompetency.JobCategory]);

                    if (questionSet == null)
                    {
                        throw new Exception($"Questionset for {functionalCompetency.JobCategory} could not be found - maybe a typo in sitefinity?");
                    }

                    if (!sets.Any(x => x.QuestionSetVersion == questionSet.QuestionSetVersion))
                    {
                        sets.Add(questionSet);
                    }
                    var questions = await QuestionRepository.GetQuestions(questionSet.QuestionSetVersion);
                    var question = questions.ToList().FirstOrDefault(x => x.SfId == functionalCompetency.Question?.Id);
                    if (question == null)
                    {
                        throw new Exception($"Question {functionalCompetency.Question.Id} exists in sitefinity but not locally");
                    }
                    if (functionalCompetency.ExcludeJobProfiles != null)
                    {
                        var excludeJobProfiles = new List<string>();
                        foreach (var jobProfile in functionalCompetency.ExcludeJobProfiles)
                        {
                            excludeJobProfiles.Add(jobProfile.Title);
                        }
                        question.ExcludesJobProfiles = excludeJobProfiles.ToArray();
                        logger.LogInformation($"Question {question.QuestionId} has {excludeJobProfiles.Count} ExcludesJobProfiles");
                    }

                    await QuestionRepository.CreateQuestion(question);
                }

                logger.LogInformation("End poll for FunctionalCompetencies");

                var output = new List<FilteringQuestionSet>();
                foreach (var fqs in sets)
                {
                    var questions = await QuestionRepository.GetQuestions(fqs.QuestionSetVersion);
                    output.Add(new FilteringQuestionSet()
                    {
                        Id = fqs.QuestionSetVersion,
                        LastUpdated = fqs.LastUpdated,
                        Description = fqs.Description,
                        Title = fqs.Title,
                        Questions = questions.Select(x => new Models.FilteringQuestion()
                        {
                            Id = x.SfId,
                            Title = x.Texts.First().Text,
                            ExcludesJobProfiles = x.ExcludesJobProfiles.OrderBy(p => p).ToList(),
                            IsYes = x.IsNegative,
                            NegativeResultDisplayText = x.NegativeResultDisplayText,
                            PositiveResultDisplayText = x.PositiveResultDisplayText,
                            Order = x.Order

                        }).ToList()
                    });
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}

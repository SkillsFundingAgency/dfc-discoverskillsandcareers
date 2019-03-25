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

namespace Dfc.DiscoverSkillsAndCareers.CmsFunctionApp.DataProcessors
{
    public class FunctionalCompetencyDataProcessor : IFunctionalCompetencyDataProcessor
    {
        readonly ILogger<FunctionalCompetencyDataProcessor> Logger;
        readonly IHttpService HttpService;
        readonly IGetFunctionalCompetenciesData GetFunctionalCompetenciesData;
        readonly AppSettings AppSettings;
        readonly IQuestionRepository QuestionRepository;
        readonly IQuestionSetRepository QuestionSetRepository;

        public FunctionalCompetencyDataProcessor(
            ILogger<FunctionalCompetencyDataProcessor> logger,
            IHttpService httpService,
            IGetFunctionalCompetenciesData getFunctionalCompetenciesData,
            IOptions<AppSettings> appSettings,
            IQuestionRepository questionRepository,
            IQuestionSetRepository questionSetRepository)
        {
            Logger = logger;
            HttpService = httpService;
            GetFunctionalCompetenciesData = getFunctionalCompetenciesData;
            AppSettings = appSettings.Value;
            QuestionRepository = questionRepository;
            QuestionSetRepository = questionSetRepository;
        }

        public async Task RunOnce()
        {
            Logger.LogInformation("Begin poll for FunctionalCompetencies");

            string siteFinityApiUrlbase = AppSettings.SiteFinityApiUrlbase;
            string siteFinityService = AppSettings.SiteFinityApiWebService;

            var data = await GetFunctionalCompetenciesData.GetData(siteFinityApiUrlbase, siteFinityService);

            Logger.LogInformation($"Have {data?.Count} functional competencies to process");

            foreach (var functionalCompetency in data)
            {
                var questionSet = await QuestionSetRepository.GetCurrentQuestionSet("filtered", functionalCompetency.JobCategory);

                var questions = await QuestionRepository.GetQuestions(questionSet.QuestionSetVersion);
                var question = questions.ToList().FirstOrDefault(x => x.SfId == functionalCompetency.Question.Id);
                if (question == null)
                {
                    throw new Exception($"Question {functionalCompetency.Question.Id} exists in sitefinity but not locally");
                }
                var excludeJobProfiles = new List<string>();
                foreach (var jobProfile in functionalCompetency.ExcludeJobProfiles)
                {
                    excludeJobProfiles.Add(jobProfile.Title);
                }
                question.ExcludesJobProfiles = excludeJobProfiles.ToArray();
                Logger.LogInformation($"Question {question.QuestionId} has {excludeJobProfiles.Count} ExcludesJobProfiles");

                await QuestionRepository.CreateQuestion(question);
            }

            Logger.LogInformation("End poll for FunctionalCompetencies");
        }
    }
}

using Dfc.DiscoverSkillsAndCareers.CmsFunctionApp.DataRequesters;
using Dfc.DiscoverSkillsAndCareers.CmsFunctionApp.Services;
using Dfc.DiscoverSkillsAndCareers.Models;
using Dfc.DiscoverSkillsAndCareers.Repositories;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Dfc.DiscoverSkillsAndCareers.CmsFunctionApp.DataProcessors
{
    public class ShortQuestionSetDataProcessor : IShortQuestionSetDataProcessor
    {
        readonly ILogger<ShortQuestionSetDataProcessor> Logger;
        readonly IHttpService HttpService;
        readonly IQuestionRepository QuestionRepository;
        readonly IQuestionSetRepository QuestionSetRepository;
        readonly IGetShortQuestionSetData GetShortQuestionSetData;
        readonly IGetShortQuestionData GetShortQuestionData;
        readonly AppSettings AppSettings;

        public ShortQuestionSetDataProcessor(
            ILogger<ShortQuestionSetDataProcessor> logger, 
            IHttpService httpService, 
            IQuestionRepository questionRepository,
            IQuestionSetRepository questionSetRepository,
            IGetShortQuestionSetData getShortQuestionSetData,
            IGetShortQuestionData getShortQuestionData,
            IOptions<AppSettings> appSettings)
        {
            Logger = logger;
            HttpService = httpService;
            QuestionRepository = questionRepository;
            QuestionSetRepository = questionSetRepository;
            GetShortQuestionSetData = getShortQuestionSetData;
            GetShortQuestionData = getShortQuestionData;
            AppSettings = appSettings.Value;
        }

        public async Task RunOnce()
        {
            Logger.LogInformation("Begin poll for ShortQuestionSet");

            string siteFinityApiUrlbase = AppSettings.SiteFinityApiUrlbase;
            string siteFinityService = AppSettings.SiteFinityApiWebService;
            string assessmentType = "short";

            var questionSets = await GetShortQuestionSetData.GetData(siteFinityApiUrlbase, siteFinityService);
            Logger.LogInformation($"Have {questionSets?.Count} question sets to review");

            foreach (var data in questionSets)
            {
                Logger.LogInformation($"Getting cms data for questionset {data.Id} {data.Title}");

                // Attempt to load the current version for this assessment type and title
                var questionSet = await QuestionSetRepository.GetCurrentQuestionSet("short", data.Title);

                // Determine if an update is required i.e. the last updated datetime stamp has changed
                bool updateRequired = questionSet == null || (data.LastUpdated != questionSet.LastUpdated);

                // Nothing to do so log and exit
                if (!updateRequired)
                {
                    Logger.LogInformation($"Questionset {data.Id} {data.Title} is upto date - no changes to be done");
                    return;
                }

                // Attempt to get the questions for this questionset
                Logger.LogInformation($"Getting cms questions for questionset {data.Id} {data.Title}");
                data.Questions = await GetShortQuestionData.GetData(siteFinityApiUrlbase, siteFinityService, data.Id);
                if (data.Questions.Count == 0)
                {
                    Logger.LogInformation($"Questionset {data.Id} doesn't have any questions");
                    return;
                }
                Logger.LogInformation($"Received {data.Questions?.Count} questions for questionset {data.Id} {data.Title}");

                if (questionSet != null)
                {
                    // Change the current question set to be not current
                    questionSet.IsCurrent = false;
                    await QuestionSetRepository.CreateQuestionSet(questionSet);
                }

                // Create the new current version
                int newVersionNumber = questionSet == null ? 1 : questionSet.Version + 1;
                var newQuestionSet = new QuestionSet()
                {
                    PartitionKey = "ncs",
                    Title = data.Title,
                    TitleLowercase = data.Title.ToLower(),
                    Description = data.Description,
                    Version = newVersionNumber,
                    QuestionSetVersion = $"{assessmentType.ToLower()}-{data.Title.ToLower()}-{newVersionNumber}",
                    AssessmentType = assessmentType,
                    IsCurrent = true,
                    LastUpdated = data.LastUpdated,                    
                };

                string questionPartitionKey = newQuestionSet.QuestionSetVersion;
                int questionNumber = 1;
                foreach (var dataQuestion in data.Questions.OrderBy(x => x.Order))
                {
                    var newQuestion = new Question
                    {
                        IsNegative = dataQuestion.IsNegative,
                        Order = questionNumber,
                        QuestionId = questionPartitionKey + "-" + questionNumber,
                        TraitCode = dataQuestion.Trait.ToUpper(),
                        PartitionKey = questionPartitionKey,
                        Texts = new []
                    {
                        new QuestionText { LanguageCode = "EN", Text = dataQuestion.Title }
                    }
                    };
                    newQuestionSet.MaxQuestions = questionNumber;
                    questionNumber++;
                    await QuestionRepository.CreateQuestion(newQuestion);
                    Logger.LogInformation($"Created question {newQuestion.QuestionId}");
                }
                await QuestionSetRepository.CreateQuestionSet(newQuestionSet);
                Logger.LogInformation($"Created questionset {newQuestionSet.Version}");
            }
            Logger.LogInformation($"End poll for ShortQuestionSet");
        }
    }
}

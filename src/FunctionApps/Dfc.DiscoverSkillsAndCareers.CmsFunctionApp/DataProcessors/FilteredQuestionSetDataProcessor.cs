using Dfc.DiscoverSkillsAndCareers.CmsFunctionApp.DataRequesters;
using Dfc.DiscoverSkillsAndCareers.CmsFunctionApp.Services;
using Dfc.DiscoverSkillsAndCareers.Models;
using Dfc.DiscoverSkillsAndCareers.Repositories;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Dfc.DiscoverSkillsAndCareers.CmsFunctionApp.DataProcessors
{
    public class FilteredQuestionSetDataProcessor : IFilteredQuestionSetDataProcessor
    {
        private readonly ISiteFinityHttpService _httpService;
        private readonly IQuestionRepository _questionRepository;
        private readonly IQuestionSetRepository _questionSetRepository;
        private readonly IGetFilteringQuestionSetData _getFilteringQuestionSetData;
        private readonly AppSettings _appSettings;

        public FilteredQuestionSetDataProcessor(
            ISiteFinityHttpService httpService,
            IQuestionRepository questionRepository,
            IQuestionSetRepository questionSetRepository,
            IGetFilteringQuestionSetData getFilteringQuestionSetData,
            IOptions<AppSettings> appSettings)
        {
            _httpService = httpService;
            _questionRepository = questionRepository;
            _questionSetRepository = questionSetRepository;
            _getFilteringQuestionSetData = getFilteringQuestionSetData;
            _appSettings = appSettings.Value;
        }

        public async Task RunOnce(ILogger logger)
        {
            logger.LogInformation("Begin poll for FilteredQuestionSet");

            string siteFinityApiUrlbase = _appSettings.SiteFinityApiUrlbase;
            string siteFinityService = _appSettings.SiteFinityApiWebService;
            string assessmentType = "filtered";

            var questionSets = await _getFilteringQuestionSetData.GetData(siteFinityApiUrlbase, siteFinityService);

            if (questionSets == null)
            {
                logger.LogWarning("No data returned from CMS while trying to extract filtering quesition sets");
                return;
            }
            
            logger.LogInformation($"Have {questionSets?.Count} question sets to review");

            foreach (var data in questionSets)
            {
                logger.LogInformation($"Getting cms data for question set {data.Id} {data.Title}");

                // Attempt to load the current version for this assessment type and title
                var questionSet = await _questionSetRepository.GetCurrentQuestionSet("filtered", data.Title);

                // Determine if an update is required i.e. the last updated datetime stamp has changed
                bool updateRequired = questionSet == null || (data.LastUpdated > questionSet.LastUpdated);

                // Nothing to do so log and exit
                if (!updateRequired)
                {
                    logger.LogInformation($"Filtering Question Set {data.Title} is upto date - no changes to be done");
                    return;
                }

                // Attempt to get the questions for this questionset
                if (data.Questions == null || data.Questions.Count == 0)
                {
                    logger.LogInformation($"Filtering Question Set {data.Title} doesn't have any questions");
                    return;
                }

                logger.LogInformation($"Received {data.Questions?.Count} questions for filtering question set {data.Id} {data.Title}");

                if (questionSet != null)
                {
                    // Change the current question set to be not current
                    questionSet.IsCurrent = false;
                }

                // Create the new current version
                int newVersionNumber = questionSet == null ? 1 : questionSet.Version + 1;
                var titleLowercase = data.Title.ToLower().Replace(" ", "-");
                var newQuestionSet = new QuestionSet()
                {
                    PartitionKey = "ncs",
                    Title = data.Title,
                    TitleLowercase = titleLowercase,
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
                        Order = questionNumber,
                        QuestionId = questionPartitionKey + "-" + questionNumber,
                        PartitionKey = questionPartitionKey,
                        Texts = new[]
                        {
                            new QuestionText { LanguageCode = "EN", Text = dataQuestion.Title }
                        },
                        ExcludesJobProfiles = dataQuestion.ExcludesJobProfiles.ToArray(),
                        FilterTrigger = dataQuestion.IsYes ? "Yes" : "No",
                        SfId = dataQuestion.Id,
                        PositiveResultDisplayText = dataQuestion.PositiveResultDisplayText,
                        NegativeResultDisplayText = dataQuestion.NegativeResultDisplayText,
                        LastUpdatedDt = dataQuestion.LastUpdated == new DateTimeOffset() ? DateTimeOffset.UtcNow : dataQuestion.LastUpdated
                    };
                    newQuestionSet.MaxQuestions = questionNumber;
                    questionNumber++;
                    await _questionRepository.CreateQuestion(newQuestion);
                    logger.LogInformation($"Created question {newQuestion.QuestionId}");
                }
                await _questionSetRepository.CreateOrUpdateQuestionSet(newQuestionSet);
                await _questionSetRepository.CreateOrUpdateQuestionSet(questionSet);
                logger.LogInformation($"Created Filtering Question Set - {newQuestionSet.Version}");
            }

            logger.LogInformation($"End poll for Filterin gQuestion Set");
        }
    }
}

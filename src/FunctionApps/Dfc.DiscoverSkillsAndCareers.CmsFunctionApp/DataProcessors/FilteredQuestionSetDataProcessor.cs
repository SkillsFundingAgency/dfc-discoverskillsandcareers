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
        private readonly IQuestionRepository _questionRepository;
        private readonly IQuestionSetRepository _questionSetRepository;
        private readonly IGetFilteringQuestionSetData _getFilteringQuestionSetData;

        public FilteredQuestionSetDataProcessor(
            IQuestionRepository questionRepository,
            IQuestionSetRepository questionSetRepository,
            IGetFilteringQuestionSetData getFilteringQuestionSetData)
        {
            _questionRepository = questionRepository;
            _questionSetRepository = questionSetRepository;
            _getFilteringQuestionSetData = getFilteringQuestionSetData;
        }

        public async Task RunOnce(ILogger logger)
        {
            logger.LogInformation("Begin poll for FilteredQuestionSet");
            
            string assessmentType = "filtered";

            var questionSets = await _getFilteringQuestionSetData.GetData();

            if (questionSets == null)
            {
                logger.LogError("No data returned from CMS while trying to extract filtering question sets");
                return;
            }
            
            logger.LogInformation($"Have {questionSets?.Count} question sets to review");

            foreach (var data in questionSets)
            {
                logger.LogInformation($"Getting cms data for question set {data.Id} {data.Title}");
                var jobCategory = data.JobProfileTaxonomy.SingleOrDefault()?.Title;

                if (String.IsNullOrWhiteSpace(jobCategory))
                {
                    logger.LogWarning($"No job category assigned to filtering question set {data.Title}, skipping.");
                    continue;
                }
                
                // Attempt to load the current version for this assessment type and title
                var questionSet = await _questionSetRepository.GetLatestQuestionSetByTypeAndKey("filtered", jobCategory);

                // Determine if an update is required i.e. the last updated datetime stamp has changed
                bool updateRequired = questionSet == null || (data.LastUpdated > questionSet.LastUpdated);

                // Nothing to do so log and exit
                if (!updateRequired)
                {
                    logger.LogInformation($"Filtering Question Set {data.Title} is upto date - no changes to be done");
                    continue;
                }

                // Attempt to get the questions for this questionset
                if (data.Questions == null || data.Questions.Count == 0)
                {
                    logger.LogInformation($"Filtering Question Set {data.Title} doesn't have any questions");
                    continue;
                }

                logger.LogInformation($"Received {data.Questions?.Count} questions for filtering question set {data.Id} {data.Title}");

                if (questionSet != null)
                {
                    // Change the current question set to be not current
                    logger.LogInformation(
                        $"Demoting filtering question set {questionSet.QuestionSetVersion} from current");
                    questionSet.IsCurrent = false;
                    await _questionSetRepository.CreateOrUpdateQuestionSet(questionSet);
                }

                // Create the new current version
                int newVersionNumber = questionSet == null ? 1 : questionSet.Version + 1;
                var titleLowercase = jobCategory.ToLower().Replace(" ", "-");
                var newQuestionSet = new QuestionSet()
                {
                    PartitionKey = "ncs",
                    Title = data.Title,
                    QuestionSetKey = titleLowercase,
                    Description = data.Description,
                    Version = newVersionNumber,
                    QuestionSetVersion = $"{assessmentType.ToLower()}-{titleLowercase}-{newVersionNumber}",
                    AssessmentType = assessmentType,
                    IsCurrent = true,
                    LastUpdated = data.LastUpdated,
                };

                string questionPartitionKey = newQuestionSet.QuestionSetVersion;
                int questionNumber = 1;
                foreach (var dataQuestion in data.Questions)
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
                        JobProfiles = dataQuestion.JobProfiles.Select(j => new QuestionJobProfile
                        {
                            JobProfile = j.JobProfile,
                            Included = j.Included
                        }).ToArray(),
                        SfId = dataQuestion.Id,
                        LastUpdatedDt = dataQuestion.LastUpdated == new DateTimeOffset() ? DateTimeOffset.UtcNow : dataQuestion.LastUpdated
                    };
                    newQuestionSet.MaxQuestions = questionNumber;
                    questionNumber++;
                    await _questionRepository.CreateQuestion(newQuestion);
                    logger.LogInformation($"Created filtering question {newQuestion.QuestionId}");
                }
                await _questionSetRepository.CreateOrUpdateQuestionSet(newQuestionSet);
                if (questionSet != null)
                {
                    
                }
                logger.LogInformation($"Created Filtering Question Set - {newQuestionSet.Version}");
            }

            logger.LogInformation($"End poll for Filtering Question Set");
        }
    }
}

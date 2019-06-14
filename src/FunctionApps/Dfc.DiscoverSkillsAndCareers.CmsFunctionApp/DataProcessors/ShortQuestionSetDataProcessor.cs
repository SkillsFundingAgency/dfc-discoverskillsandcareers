using System.Collections.Generic;
using Dfc.DiscoverSkillsAndCareers.CmsFunctionApp.Services;
using Dfc.DiscoverSkillsAndCareers.Models;
using Dfc.DiscoverSkillsAndCareers.Repositories;
using Microsoft.Extensions.Logging;
using System.Linq;
using System.Threading.Tasks;
using Dfc.DiscoverSkillsAndCareers.CmsFunctionApp.Models;

namespace Dfc.DiscoverSkillsAndCareers.CmsFunctionApp.DataProcessors
{
    public class ShortQuestionSetDataProcessor : IShortQuestionSetDataProcessor
    {
        private readonly ISiteFinityHttpService _sitefinity;
        readonly IQuestionRepository _questionRepository;
        readonly IQuestionSetRepository _questionSetRepository;

        public ShortQuestionSetDataProcessor(
            ISiteFinityHttpService sitefinity,
            IQuestionRepository questionRepository,
            IQuestionSetRepository questionSetRepository)
        {
            _sitefinity = sitefinity;
            _questionRepository = questionRepository;
            _questionSetRepository = questionSetRepository;
        }

        public async Task RunOnce(ILogger logger)
        {
            logger.LogInformation("Begin poll for ShortQuestionSet");

            string assessmentType = "short";

            var questionSets = await _sitefinity.GetAll<SiteFinityShortQuestionSet>("shortquestionsets");
            logger.LogInformation($"Have {questionSets?.Count} question sets to review");

            if (questionSets != null)
            {
                foreach (var data in questionSets.OrderBy(x => x.LastUpdated))
                {
                    logger.LogInformation($"Getting cms data for question set {data.Id} {data.Title}");
                    
                    
                    // Attempt to load the current version for this assessment type and title
                    var questionSet = await _questionSetRepository.GetLatestQuestionSetByTypeAndKey("short", data.Title);
                    
                    // Determine if an update is required i.e. the last updated datetime stamp has changed
                    bool updateRequired = questionSet == null || (data.LastUpdated > questionSet.LastUpdated);

                    // Nothing to do so log and exit
                    if (!updateRequired)
                    {
                        logger.LogInformation($"Question set {data.Id} {data.Title} is upto date - no changes to be done");
                        continue;
                    }

                    // Attempt to get the questions for this question set
                    logger.LogInformation($"Getting cms questions for question set {data.Id} {data.Title}");
                    
                    data.Questions = await _sitefinity.Get<List<SiteFinityShortQuestion>>($"shortquestionsets({data.Id})/Questions?$expand=Trait");
                    
                    if (data.Questions.Count == 0)
                    {
                        logger.LogInformation($"Question set {data.Id} doesn't have any questions");
                        continue;
                    }

                    logger.LogInformation(
                        $"Received {data.Questions?.Count} questions for question set {data.Id} {data.Title}");
                    
                    var latestQuestionSet = await _questionSetRepository.GetCurrentQuestionSet("short");

                    if (latestQuestionSet != null)
                    {
                        // Change the current question set to be not current
                        logger.LogInformation(
                            $"Demoting question set {latestQuestionSet.QuestionSetVersion} from current");
                        latestQuestionSet.IsCurrent = false;
                        await _questionSetRepository.CreateOrUpdateQuestionSet(latestQuestionSet);
                    }
                    
                    if (questionSet != null && questionSet.IsCurrent)
                    {
                        // Change the current question set to be not current
                        logger.LogInformation(
                            $"Demoting question set {questionSet.QuestionSetVersion} from current");
                        questionSet.IsCurrent = false;
                        await _questionSetRepository.CreateOrUpdateQuestionSet(questionSet);
                    }


                    // Create the new current version
                    int newVersionNumber = questionSet == null ? 1 : questionSet.Version + 1;
                    var newQuestionSet = new QuestionSet()
                    {
                        PartitionKey = "ncs",
                        Title = data.Title,
                        QuestionSetKey = data.Title.ToLower(),
                        Description = data.Description,
                        Version = newVersionNumber,
                        QuestionSetVersion = $"{assessmentType.ToLower()}-{data.Title.ToLower()}-{newVersionNumber}",
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
                            IsNegative = dataQuestion.IsNegative,
                            Order = questionNumber,
                            QuestionId = questionPartitionKey + "-" + questionNumber,
                            TraitCode = dataQuestion.Trait.Name.ToUpper(),
                            PartitionKey = questionPartitionKey,
                            LastUpdatedDt = dataQuestion.LastUpdatedDt,
                            Texts = new[]
                            {
                                new QuestionText {LanguageCode = "EN", Text = dataQuestion.Title}
                            }
                        };
                        newQuestionSet.MaxQuestions = questionNumber;
                        questionNumber++;
                        await _questionRepository.CreateQuestion(newQuestion);
                        logger.LogInformation($"Created short question {newQuestion.QuestionId}");
                    }

                    await _questionSetRepository.CreateOrUpdateQuestionSet(newQuestionSet);
                    logger.LogInformation($"Created Short Question Set {newQuestionSet.Version}");
                }
            }
            else
            {
                logger.LogError("No Short Question sets available");
            }

            logger.LogInformation($"End poll for ShortQuestionSet");
        }
    }
}

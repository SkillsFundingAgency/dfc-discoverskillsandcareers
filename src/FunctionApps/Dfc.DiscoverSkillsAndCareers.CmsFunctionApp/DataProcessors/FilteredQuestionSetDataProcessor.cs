using Dfc.DiscoverSkillsAndCareers.CmsFunctionApp.Services;
using Dfc.DiscoverSkillsAndCareers.Models;
using Dfc.DiscoverSkillsAndCareers.Repositories;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dfc.DiscoverSkillsAndCareers.CmsFunctionApp.Models;

namespace Dfc.DiscoverSkillsAndCareers.CmsFunctionApp.DataProcessors
{
    public class FilteredQuestionSetDataProcessor : IContentTypeProcessor<FilteredQuestionSetDataProcessor>
    {
        private readonly IQuestionRepository _questionRepository;
        private readonly IQuestionSetRepository _questionSetRepository;
        private readonly IJobCategoryRepository _jobCategoryRepository;
        private readonly ISiteFinityHttpService _sitefinity;
        private readonly IOptions<AppSettings> _appSettings;
        private ILogger _logger;
        private const string assessmentType = "filtered";

        public FilteredQuestionSetDataProcessor(
            IQuestionRepository questionRepository,
            IQuestionSetRepository questionSetRepository,
            IJobCategoryRepository jobCategoryRepository,
            ISiteFinityHttpService sitefinity,
            IOptions<AppSettings> appSettings)
        {
            _questionRepository = questionRepository;
            _jobCategoryRepository = jobCategoryRepository;
            _questionSetRepository = questionSetRepository;
            _sitefinity = sitefinity;
            _appSettings = appSettings;
        }
        
        public async Task RunOnce(ILogger logger)
        {
            _logger = logger;
            
            var questionSet = await UpdateFilteredQuestionSets();

            if (questionSet != null)
            {
                
            }
        }

        

        private async Task<QuestionSet> UpdateFilteredQuestionSets()
        {
            _logger.LogInformation("Begin poll for Filtered Question Sets");

            var questionSets = await _sitefinity.GetAll<SiteFinityFilteringQuestionSet>("filteringquestionsets");
            _logger.LogInformation($"Have {questionSets?.Count} question sets to review");

            QuestionSet questionSet = null;
            
            if (questionSets != null)
            {
                foreach (var data in questionSets.OrderBy(x => x.LastUpdated))
                {
                    _logger.LogInformation($"Getting cms data for question set {data.Id} {data.Title}");

                    // Attempt to load the current version for this assessment type and title
                    questionSet = await _questionSetRepository.GetCurrentQuestionSet(assessmentType);
             
                    // Determine if an update is required i.e. the last updated datetime stamp has changed
                    bool updateRequired = questionSet == null || (data.LastUpdated > questionSet.LastUpdated);

                    // Nothing to do so log and exit
                    if (!updateRequired)
                    {
                        _logger.LogInformation($"Filtered Question set {data.Id} {data.Title} is upto date - no changes to be done");
                        questionSet = null;
                        continue;
                    }

                    // Attempt to get the questions for this question set
                    _logger.LogInformation($"Getting cms questions for question set {data.Id} {data.Title}");

                    data.Questions =
                        await _sitefinity.Get<List<SiteFinityFilteringQuestion>>($"filteringquestionsets({data.Id})/Questions?$expand=RelatedSkill");

                    if (data.Questions.Count == 0)
                    {
                        _logger.LogInformation($"Question set {data.Id} doesn't have any questions");
                        questionSet = null;
                        continue;
                    }

                    _logger.LogInformation(
                        $"Received {data.Questions?.Count} questions for question set {data.Id} {data.Title}");

                    if (questionSet != null && questionSet.IsCurrent)
                    {
                        // Change the current question set to be not current
                        _logger.LogInformation($"Demoting question set {questionSet.QuestionSetVersion} from current");
                        questionSet.IsCurrent = false;
                        await _questionSetRepository.CreateOrUpdateQuestionSet(questionSet);
                    }

                    // Create the new current version
                    int newVersionNumber = questionSet == null ? 1 : questionSet.Version + 1;
                    var newQuestionSet = new QuestionSet
                    {
                        PartitionKey = "filtered-" + data.Title.ToLower().Replace(" ", "-"),
                        Title = data.Title,
                        QuestionSetKey = data.Title.ToLower(),
                        Description = data.Description,
                        Version = newVersionNumber,
                        QuestionSetVersion = $"{assessmentType.ToLower()}-{data.Title.ToLower()}-{newVersionNumber.ToString()}",
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
                            IsNegative = false,
                            Order = questionNumber,
                            QuestionId = questionPartitionKey + "-" + questionNumber.ToString(),
                            PartitionKey = questionPartitionKey,
                            TraitCode = dataQuestion.RelatedSkill.Title.ToLower(),
                            LastUpdatedDt = dataQuestion.LastUpdated,
                            SfId = dataQuestion.Id,
                            IsFilterQuestion = true,
                            Texts = new[]
                            {
                                new QuestionText {LanguageCode = "EN", Text = dataQuestion.QuestionText}
                            }
                        };
                        newQuestionSet.MaxQuestions = questionNumber;
                        questionNumber++;
                        await _questionRepository.CreateQuestion(newQuestion);
                        
                        _logger.LogInformation($"Created filtering question {newQuestion.QuestionId}");
                    }
                    
                    questionSet = newQuestionSet;
                    
                    await _questionSetRepository.CreateOrUpdateQuestionSet(newQuestionSet);
                    _logger.LogInformation($"Created Filtering Question Set {newQuestionSet.Version.ToString()}");

                }
            }
            else
            {
                _logger.LogError("No Filtered Question sets available");
            }

            _logger.LogInformation($"End poll for Filtered Question Sets");
            return questionSet;
        }
    }
}

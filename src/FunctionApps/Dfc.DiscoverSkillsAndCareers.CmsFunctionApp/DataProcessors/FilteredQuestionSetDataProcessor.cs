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
    public class FilteredQuestionSetDataProcessor : IFilteredQuestionSetDataProcessor
    {
        private readonly IQuestionRepository _questionRepository;
        private readonly IQuestionSetRepository _questionSetRepository;
        private readonly ISiteFinityHttpService _sitefinity;
        private readonly IOptions<AppSettings> _appSettings;
        private const string assessmentType = "filtered";

        public FilteredQuestionSetDataProcessor(
            IQuestionRepository questionRepository,
            IQuestionSetRepository questionSetRepository,
            ISiteFinityHttpService sitefinity,
            IOptions<AppSettings> appSettings)
        {
            _questionRepository = questionRepository;
            _questionSetRepository = questionSetRepository;
            _sitefinity = sitefinity;
            _appSettings = appSettings;
        }

        public async Task RunOnce(ILogger logger)
        {
            var questionSets =
                await _sitefinity.GetAll<SiteFinityFilteringQuestionSet>($"filteringquestionsets");

            foreach (var questionSet in questionSets)
            {
                logger.LogInformation($"Getting cms data for question set {questionSet.Id} {questionSet.Title}");
                
                var currentQuestionSet = await _questionSetRepository.GetLatestQuestionSetByTypeAndKey("filtered", questionSet.Title);

                // Determine if an update is required i.e. the last updated datetime stamp has changed
                bool updateRequired = currentQuestionSet == null || (currentQuestionSet.LastUpdated > questionSet.LastUpdated);
                // Nothing to do so log and exit
                if (!updateRequired)
                {
                    logger.LogInformation($"Filtering Question Set for job category {questionSet.Title} is upto date - no changes to be done");
                    return;
                }

                var questions = 
                    await _sitefinity.Get<List<SiteFinityFilteringQuestion>>($"filteringquestionsets({questionSet.Id})/Questions?$expand=RelatedSkill");
            
                var jobProfiles = 
                    await _sitefinity.GetAll<SiteFinityJobProfile>("jobprofiles?$select=Id,Title,JobProfileCategories&$expand=RelatedSkills&$orderby=Title");
            
                var jobCategories = 
                    await _sitefinity.GetTaxonomyInstances("Job Profile Category");

                var categoryQuestions = JobCategoryQuestionBuilder.Build(questions, jobProfiles, jobCategories,
                    _appSettings.Value.MaxPercentageOfProfileOccurenceForSkill,
                    _appSettings.Value.MaxPercentageDistributionOfJobProfiles);

                
                foreach (var category in categoryQuestions)
                {
                    if (category.HasMessages)
                    {
                        foreach (var msg in category.Messages)
                        {
                            logger.Log(msg.Item1, msg.Item2);
                        }
                    }

                    await UpdateFilteringQuestionSet(logger, questionSet, category);
                }   
            }
        }
        
        public async Task UpdateFilteringQuestionSet(ILogger logger, SiteFinityFilteringQuestionSet questionSet, JobCategoryQuestionsResult jobCategory)
        {
            var currentQuestionSet = await _questionSetRepository.GetLatestQuestionSetByTypeAndKey("filtered", jobCategory.JobCategory);

            // Determine if an update is required i.e. the last updated datetime stamp has changed
            bool updateRequired = currentQuestionSet == null || (currentQuestionSet.LastUpdated > questionSet.LastUpdated);

            // Nothing to do so log and exit
            if (!updateRequired)
            {
                logger.LogInformation($"Filtering Question Set for job category {jobCategory.JobCategory} is upto date - no changes to be done");
                return;
            }
            
            int newVersionNumber = currentQuestionSet == null ? 1 : currentQuestionSet.Version + 1;
            var titleLowercase = jobCategory.JobCategory.ToLower().Replace(" ", "-");
            
            var newQuestionSet = new QuestionSet()
            {
                PartitionKey = questionSet.Title.ToLower().Replace(" ", "-"),
                Title = jobCategory.JobCategory,
                QuestionSetKey = titleLowercase,
                Description = "",
                Version = newVersionNumber,
                QuestionSetVersion = $"{assessmentType.ToLower()}-{titleLowercase}-{newVersionNumber}",
                AssessmentType = assessmentType,
                IsCurrent = true,
                LastUpdated = questionSet.LastUpdated,
            };

            string questionPartitionKey = newQuestionSet.QuestionSetVersion;
            int questionNumber = 1;
            
            foreach (var dataQuestion in jobCategory.Questions)
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
            if (currentQuestionSet != null)
            {
                // Change the current question set to be not current
                logger.LogInformation($"Demoting filtering question set {currentQuestionSet.QuestionSetVersion} from current");
                currentQuestionSet.IsCurrent = false;
                await _questionSetRepository.CreateOrUpdateQuestionSet(currentQuestionSet);
            }
            
            
        }
    }
}

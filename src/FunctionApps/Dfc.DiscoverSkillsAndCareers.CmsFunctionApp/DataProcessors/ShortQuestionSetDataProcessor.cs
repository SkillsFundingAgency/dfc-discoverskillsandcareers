using Dfc.DiscoverSkillsAndCareers.CmsFunctionApp.DataRequesters;
using Dfc.DiscoverSkillsAndCareers.CmsFunctionApp.Services;
using Dfc.DiscoverSkillsAndCareers.Models;
using Dfc.DiscoverSkillsAndCareers.Repositories;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Dfc.DiscoverSkillsAndCareers.CmsFunctionApp.DataProcessors
{
    public class ShortQuestionSetDataProcessor : IShortQuestionSetPoller
    {
        readonly ILogger<ShortQuestionSetDataProcessor> Logger;
        readonly IHttpService HttpService;
        readonly IQuestionRepository QuestionRepository;
        readonly IQuestionSetRepository QuestionSetRepository;
        readonly IGetShortQuestionSetData GetShortQuestionSetData;

        public ShortQuestionSetDataProcessor(
            ILogger<ShortQuestionSetDataProcessor> logger, 
            IHttpService httpService, 
            IQuestionRepository questionRepository,
            IQuestionSetRepository questionSetRepository,
            IGetShortQuestionSetData getShortQuestionSetData)
        {
            Logger = logger;
            HttpService = httpService;
            QuestionRepository = questionRepository;
            QuestionSetRepository = questionSetRepository;
            GetShortQuestionSetData = getShortQuestionSetData;
        }

        public async Task RunOnce()
        {
            string assessmentType = "short";

            Logger.LogInformation("Begin poll for ShortQuestionSet");

            string url = "https://www.google.com"; // TODO: CMS endpoint
            var data = await GetShortQuestionSetData.GetData(url);

            // Attempt to load the current version for this assessment type and title
            var questionSet = await QuestionSetRepository.GetCurrentQuestionSet("short", data.Title);

            // Determine if an update is required i.e. the last updated datetime stamp has changed
            bool updateRequired = questionSet == null || (data.LastUpdated != questionSet.LastUpdated); // TODO:

            // Nothing to do so log and exit
            if (!updateRequired)
            {
                Console.WriteLine($"Questionset is upto date");
                return;
            }

            if (questionSet != null)
            {
                // Change the current question set to be not current
                questionSet.IsCurrent = false;
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
                LastUpdated = data.LastUpdated
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
                    Texts = new List<QuestionText>
                    {
                        new QuestionText { LanguageCode = "EN", Text = dataQuestion.Title }
                    }
                };
                newQuestionSet.MaxQuestions = questionNumber;
                questionNumber++;
                await QuestionRepository.CreateQuestion(newQuestion);
            }
            await QuestionSetRepository.CreateQuestionSet(newQuestionSet);
        }
    }
}

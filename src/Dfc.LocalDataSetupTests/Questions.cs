using System.Collections.Generic;
using Xunit;
using Dfc.DiscoverSkillsAndCareers.Models;
using Dfc.DiscoverSkillsAndCareers.Repositories;

namespace Dfc.LocalDataSetupTests
{
    public class Questions
    {

        public async void SetupQuestions()
        {
            string QuestionSetVersion = "201901";
            List<string> traits = new List<string>() { "LEADER", "DOER", "DRIVER", "INFLUENCER", "HELPER", "ORGANISER", "ANALYST", "CREATOR" };
            var cosmosSettings = new CosmosSettings()
            {
                Key = "C2y6yDjf5/R+ob0N8A7Cgv30VRDJIWEHLM+4QDU5DE2nQ9nDuVTqobD4b8mGGyPMbIZnqyMsEcaGQy67XIw/Jw==",
                DatabaseName = "TestDatabase",
                Endpoint = "https://localhost:8081"
            };
            var questionRepository = new QuestionRepository(cosmosSettings);

            for (int questionNumber = 1; questionNumber <= 20; questionNumber++)
            {
                var question = new Question()
                {
                    QuestionId = $"{QuestionSetVersion}-{questionNumber}",
                    Texts = new List<QuestionText>()
                    {
                        new QuestionText() { LanguageCode = "en", Text = $"English question {questionNumber}" },
                        new QuestionText() { LanguageCode = "cy", Text = $"Welsh question {questionNumber}"}
                    },
                    Order = questionNumber,
                    PartitionKey = QuestionSetVersion,
                    TraitCode = traits[new System.Random().Next(0, 7)]
                };

                await questionRepository.CreateQuestion(question);
            }
        }
    }
}

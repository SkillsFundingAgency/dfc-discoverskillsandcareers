using Dfc.DiscoverSkillsAndCareers.Models;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace Dfc.UnitTests
{
    public class FilteringQuestion
    {
        [JsonProperty("partitionKey")]
        public string PartitionKey { get; set; }
        [JsonProperty("id")]
        public string FilteringQuestionId { get; set; }
        [JsonProperty("texts")]
        public QuestionText[] Texts { get; set; } = { };
        [JsonProperty("jobFamily")]
        public string JobFamily { get; set; }
        [JsonProperty("order")]
        public int Order { get; set; }
        [JsonProperty("excludesJobProfiles")]
        public string[] ExcludesJobProfiles { get; set; } = { };
        [JsonProperty("excludeAnswerTrigger")]
        public string ExcludeAnswerTrigger { get; set; } = "No";
        [JsonProperty("answerOptions")]
        public string[] AnswerOptions { get; set; } = new[] { "Yes", "No" };
    }
    
    public class FilteringQuestionTests
    {
        public FilteringQuestionTests()
        {
            FilteringQuestionsSample = new List<FilteringQuestion>()
            {
                new FilteringQuestion()
                {
                    JobFamily = "ANC",
                    Texts = new [] { new QuestionText() { Text = "", LanguageCode = "en" } },
                    Order = 1,
                    ExcludesJobProfiles = new []
                    {
                        "Beekeeper",
                        "Farm worker",
                        "Farmer",
                        "Gamekeeper"
                    }
                },
                new FilteringQuestion()
                {
                    JobFamily = "ANC",
                    Texts = new [] { new QuestionText() { Text = "", LanguageCode = "en" } },
                    Order = 2,
                    ExcludesJobProfiles = new []
                    {
                        "Farmer",
                        "Gamekeeper",
                        "Horse groom",
                        "Horse riding instructor",
                    }
                }
            };
        }

        List<FilteringQuestion> FilteringQuestionsSample;

        [Theory]
        [InlineData("No", 0)]
        [InlineData("Yes", 6)]
        public void AnswerRemoval_WithTheory_HasExpectedProfileCount(string answer, int expectedRemainingProfiles)
        {
            var jobFamily = "ANC";

            var allJobProfiles = GetAllJobProfilesByJobFamilyCode(jobFamily);
            var suggestedJobProfiles = GetAllJobProfilesByJobFamilyCode(jobFamily);

            var relevantQuestions = FilteringQuestionsSample
                .Where(x => x.JobFamily == jobFamily)
                .OrderBy(x => x.Order)
                .ToList();

            for (int i = 1; i <= relevantQuestions.Count; i++)
            {
                var question = relevantQuestions
                    .Where(x => x.Order == i)
                    .First();

                var toremove = relevantQuestions
                    .Where(x => x.Order == i
                        && x.ExcludeAnswerTrigger == answer)
                    .SelectMany(x => x.ExcludesJobProfiles)
                    .ToList();
                int expected = suggestedJobProfiles.Count;
                var removedCount = suggestedJobProfiles.RemoveAll(x => toremove.Contains(x));
                expected -= removedCount;

                Assert.Equal(expected, suggestedJobProfiles.Count);
            }

            Assert.Equal(expectedRemainingProfiles, suggestedJobProfiles.Count);
        }

        private List<string> GetAllJobProfilesByJobFamilyCode(string code) => 
            FilteringQuestionsSample
                .Where(x => x.JobFamily == code)
                .Select(x => x.ExcludesJobProfiles)
                .SelectMany(x => x)
                .Distinct()
                .ToList();
    }
}

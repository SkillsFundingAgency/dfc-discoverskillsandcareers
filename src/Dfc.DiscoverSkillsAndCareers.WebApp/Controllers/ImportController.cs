using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dfc.DiscoverSkillsAndCareers.Models;
using Dfc.DiscoverSkillsAndCareers.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Dfc.DiscoverSkillsAndCareers.WebApp.Controllers
{
    public class ImportController : Controller
    {
        readonly ILogger<ImportController> Logger;
        readonly IQuestionRepository QuestionRepository;
        readonly IQuestionSetRepository QuestionSetRepository;

        public ImportController(
            ILogger<ImportController> logger,
            IQuestionRepository questionRepository,
            IQuestionSetRepository questionSetRepository)
        {
            Logger = logger;
            QuestionRepository = questionRepository;
            QuestionSetRepository = questionSetRepository;
        }

        public class FilteringQuestionSet
        {
            [JsonProperty("Id")]
            public string Id { get; set; }
            [JsonProperty("Title")]
            public string Title { get; set; }
            [JsonProperty("Description")]
            public string Description { get; set; }
            public List<FilteringQuestion> Questions { get; set; }
            [JsonProperty("LastModified")]
            public DateTime LastUpdated { get; set; }
        }

        public class FilteringQuestion
        {
            [JsonProperty("Id")]
            public string Id { get; set; }
            [JsonProperty("QuestionText")]
            public string Title { get; set; }
            [JsonProperty("Description")]
            public string Description { get; set; }
            [JsonProperty("Order")]
            public int? Order { get; set; }
            [JsonProperty("ExcludesJobProfiles")]
            public List<string> ExcludesJobProfiles { get; set; }
            [JsonProperty("IsYes")]
            public bool IsYes { get; set; }
        }

        [HttpPost("import/filtering-questions")]
        public async Task<IActionResult> FilteringQuestions([FromBody]string uploadJson)
        {
            try
            {
                Request.Headers.TryGetValue("import-key", out var headerValue);
                if (headerValue.ToString() != "Hgfy-gYh3") return NotFound();

                var questionSets = JsonConvert.DeserializeObject<List<FilteringQuestionSet>>(uploadJson);

                string assessmentType = "filtered";
                var createdQuestionSets = new List<QuestionSet>();
                foreach (var data in questionSets)
                {
                    Logger.LogInformation($"Getting cms data for questionset {data.Id} {data.Title}");

                    // Attempt to load the current version for this assessment type and title
                    var questionSet = await QuestionSetRepository.GetCurrentQuestionSet("filtered", data.Title);

                    // Determine if an update is required i.e. the last updated datetime stamp has changed
                    bool updateRequired = questionSet == null || (data.LastUpdated != questionSet.LastUpdated);

                    // Nothing to do so log and exit
                    if (!updateRequired)
                    {
                        return new JsonResult(new
                        {
                            message = $"Filteringquestionset {data.Id} {data.Title} is upto date - no changes to be done"
                        });
                    }

                    // Attempt to get the questions for this questionset
                    if (data.Questions.Count == 0)
                    {
                        return new JsonResult(new
                        {
                            message = $"Filteringquestionset {data.Id} doesn't have any questions"
                        });
                    }
                    Logger.LogInformation($"Received {data.Questions?.Count} questions for questionset {data.Id} {data.Title}");

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
                            FilterTrigger = dataQuestion.IsYes ? "Yes" : "No"

                        };
                        newQuestionSet.MaxQuestions = questionNumber;
                        questionNumber++;
                        await QuestionRepository.CreateQuestion(newQuestion);
                        Logger.LogInformation($"Created question {newQuestion.QuestionId}");
                    }
                    await QuestionSetRepository.CreateQuestionSet(newQuestionSet);
                    Logger.LogInformation($"Created filteringquestionset {newQuestionSet.Version}");
                    createdQuestionSets.Add(newQuestionSet);
                }

                return new JsonResult(new
                {
                    message = "Ok",
                    createdQuestionSets
                });
            }

            catch (Exception ex)
            {
                return new JsonResult(new
                {
                    message = ex.ToString()
                });
            }
        }
    }
}
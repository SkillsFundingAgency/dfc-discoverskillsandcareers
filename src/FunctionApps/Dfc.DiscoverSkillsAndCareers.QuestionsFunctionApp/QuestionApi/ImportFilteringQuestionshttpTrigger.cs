using Dfc.DiscoverSkillsAndCareers.Models;
using Dfc.DiscoverSkillsAndCareers.Repositories;
using DFC.Common.Standard.Logging;
using DFC.Functions.DI.Standard.Attributes;
using DFC.HTTP.Standard;
using DFC.Swagger.Standard.Annotations;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace Dfc.DiscoverSkillsAndCareers.QuestionsFunctionApp
{
    public static class ImportFilteringQuestionsHttpTrigger
    {
        [FunctionName("ImportFilteringQuestionsHttpTrigger")]
        [ProducesResponseType(typeof(Question), (int)HttpStatusCode.OK)]
        [Response(HttpStatusCode = (int)HttpStatusCode.OK, Description = "Import complete", ShowSchema = true)]
        [Response(HttpStatusCode = (int)HttpStatusCode.NoContent, Description = "Question set does not exist", ShowSchema = false)]
        [Response(HttpStatusCode = (int)HttpStatusCode.BadRequest, Description = "Request was malformed", ShowSchema = false)]
        [Response(HttpStatusCode = (int)HttpStatusCode.Unauthorized, Description = "API key is unknown or invalid", ShowSchema = false)]
        [Response(HttpStatusCode = (int)HttpStatusCode.Forbidden, Description = "Insufficient access", ShowSchema = false)]
        [Display(Name = "ImportFilteringQuestions", Description = "Imports the filtering questions")]

        public static async Task<HttpResponseMessage> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "import/filtering-questions")]HttpRequest req,
            ILogger log,
            [Inject]ILoggerHelper loggerHelper,
            [Inject]IHttpRequestHelper httpRequestHelper,
            [Inject]IHttpResponseMessageHelper httpResponseMessageHelper,
            [Inject]IQuestionRepository questionRepository,
            [Inject]IQuestionSetRepository questionSetRepository
            )
        {

            string inputJson = "";
            using (var streamReader = new StreamReader(req.Body))
            {
                inputJson = streamReader.ReadToEnd();
            }

            try
            {
                req.Headers.TryGetValue("import-key", out var headerValue);
                if (headerValue.ToString() != "Hgfy-gYh3") return httpResponseMessageHelper.BadRequest();

                var questionSets = JsonConvert.DeserializeObject<List<FilteringQuestionSet>>(inputJson);

                string assessmentType = "filtered";
                var createdQuestionSets = new List<QuestionSet>();
                foreach (var data in questionSets)
                {
                    log.LogInformation($"Getting cms data for questionset {data.Id} {data.Title}");

                    // Attempt to load the current version for this assessment type and title
                    var questionSet = await questionSetRepository.GetCurrentQuestionSet("filtered", data.Title);

                    // Determine if an update is required i.e. the last updated datetime stamp has changed
                    bool updateRequired = questionSet == null || (data.LastUpdated != questionSet.LastUpdated);

                    // Nothing to do so log and exit
                    if (!updateRequired)
                    {
                        return httpResponseMessageHelper.Ok(JsonConvert.SerializeObject(new
                        {
                            message = $"Filteringquestionset {data.Id} {data.Title} is upto date - no changes to be done"
                        }));
                    }

                    // Attempt to get the questions for this questionset
                    if (data.Questions.Count == 0)
                    {
                        return httpResponseMessageHelper.Ok(JsonConvert.SerializeObject(new
                        {
                            message = $"Filteringquestionset {data.Id} doesn't have any questions"
                        }));
                    }
                    log.LogInformation($"Received {data.Questions?.Count} questions for questionset {data.Id} {data.Title}");

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
                        await questionRepository.CreateQuestion(newQuestion);
                        log.LogInformation($"Created question {newQuestion.QuestionId}");
                    }
                    await questionSetRepository.CreateQuestionSet(newQuestionSet);
                    log.LogInformation($"Created filteringquestionset {newQuestionSet.Version}");
                    createdQuestionSets.Add(newQuestionSet);
                }

                return httpResponseMessageHelper.Ok(JsonConvert.SerializeObject(new
                {
                    message = "Ok",
                    createdQuestionSets
                }));
            }

            catch (Exception ex)
            {
                return httpResponseMessageHelper.Ok(JsonConvert.SerializeObject(new
                {
                    message = ex.ToString()
                }));
            }
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

    }
}

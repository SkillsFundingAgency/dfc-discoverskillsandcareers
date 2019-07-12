using Dfc.DiscoverSkillsAndCareers.Models;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Dfc.DiscoverSkillsAndCareers.Models.Extensions;
using Microsoft.Extensions.Caching.Memory;

namespace Dfc.DiscoverSkillsAndCareers.Repositories
{
    [ExcludeFromCodeCoverage]
    public class QuestionRepository : IQuestionRepository
    {
        readonly TimeSpan cacheExpiration = TimeSpan.FromMinutes(30);
        readonly ICosmosSettings cosmosSettings;
        readonly IMemoryCache cache;
        readonly string collectionName;
        readonly DocumentClient client;

        public QuestionRepository(DocumentClient client, IOptions<CosmosSettings> cosmosSettings, IMemoryCache cache)
        {
            this.cosmosSettings = cosmosSettings?.Value;
            this.collectionName = "Questions";
            this.cache = cache;
            this.client = client;
        }

        public async Task<Question> GetQuestion(int questionNumber, string questionSetVersion)
        {
            var questionId = $"{questionSetVersion}-{questionNumber}";
            return await GetQuestion(questionId);
        }

        public async Task<Question> GetQuestion(string questionId)
        {
            try
            {
                int pos = questionId.LastIndexOf('-');
                string questionSetVersion = questionId.Substring(0, pos);
                
                var questions = await GetQuestions(questionSetVersion);
                return questions?.FirstOrDefault(q => q.QuestionId.EqualsIgnoreCase(questionId));
            }
            catch (DocumentClientException ex)
            {
                if (ex.StatusCode == HttpStatusCode.NotFound)
                {
                    return null;
                }
                else
                {
                    throw;
                }
            }
        }

        public async Task<Document> CreateQuestion(Question question)
        {
            var uri = UriFactory.CreateDocumentCollectionUri(cosmosSettings.DatabaseName, collectionName);
            try
            {
                if (cache.TryGetValue<Question[]>(question.PartitionKey, out var questions))
                {
                    var index = Array.FindIndex(questions, q => q.QuestionId.EqualsIgnoreCase(question.QuestionId));

                    if (index == -1)
                    {
                        var x = questions.ToList();
                        x.Add(question);
                        questions = x.ToArray();
                    }
                    else
                    {
                        questions[index] = question;
                    }
                    
                    cache.Set(question.PartitionKey, questions, cacheExpiration);
                }
                
                return await client.CreateDocumentAsync(uri, question);
            }
            catch
            {
                return await client.UpsertDocumentAsync(uri, question);
            }
        }

        public async Task<Question[]> GetQuestions(string assessmentType, string title, int version)
        {
            var partitionKey = $"{assessmentType.ToLower()}-{title.ToLower()}-{version}";
            return await GetQuestions(partitionKey);
        }

        public async Task<Question[]> GetQuestions(string questionSetVersion)
        {
            try
            {
                var uri = UriFactory.CreateDocumentCollectionUri(cosmosSettings.DatabaseName, collectionName);
                FeedOptions feedOptions = new FeedOptions() { EnableCrossPartitionQuery = true };
                
                if (!cache.TryGetValue<Question[]>(questionSetVersion, out var queryQuestions))
                {
                    queryQuestions = client.CreateDocumentQuery<Question>(uri, feedOptions)
                        .Where(x => x.PartitionKey == questionSetVersion)
                        .AsEnumerable()
                        .ToArray();

                    cache.Set(questionSetVersion, queryQuestions, cacheExpiration);
                }
                
                return await Task.FromResult(queryQuestions?.OrderBy(q => q.Order).ToArray());
            }
            catch (DocumentClientException ex)
            {
                if (ex.StatusCode == HttpStatusCode.NotFound)
                {
                    return null;
                }

                throw;
            }
        }
    }
}

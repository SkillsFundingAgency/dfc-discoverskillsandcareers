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
using Microsoft.Extensions.Caching.Memory;

namespace Dfc.DiscoverSkillsAndCareers.Repositories
{
    [ExcludeFromCodeCoverage]
    public class QuestionRepository : IQuestionRepository
    {
        private readonly Func<MemoryCacheEntryOptions> getCacheExpiration;
        readonly ICosmosSettings cosmosSettings;
        readonly string collectionName;
        readonly DocumentClient client;
        readonly IMemoryCache cache;

        public QuestionRepository(DocumentClient client, IOptions<CosmosSettings> cosmosSettings, IMemoryCache cache)
        {
            this.cosmosSettings = cosmosSettings?.Value;
            this.collectionName = "Questions";
            this.client = client;
            this.cache = cache;
            
            this.getCacheExpiration = () => new MemoryCacheEntryOptions()
                .SetSlidingExpiration(TimeSpan.FromMinutes(5));
            
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
                if (!cache.TryGetValue<Question>(questionId, out var question))
                {

                    var uri = UriFactory.CreateDocumentUri(cosmosSettings.DatabaseName, collectionName, questionId);
                    int pos = questionId.LastIndexOf('-');
                    string partitionKey = questionId.Substring(0, pos);
                    var requestOptions = new RequestOptions {PartitionKey = new PartitionKey(partitionKey)};
                    var document = await client.ReadDocumentAsync<Question>(uri, requestOptions);
                    cache.Set(questionId, document, getCacheExpiration());
                    return document;
                }

                return question;
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
            cache.Set(question.QuestionId, question, getCacheExpiration());
            return await client.UpsertDocumentAsync(uri, question);
        }

        public async Task<Question[]> GetQuestions(string assessmentType, string title, int version)
        {
            return await GetQuestions($"{assessmentType.ToLower()}-{title.ToLower()}-{version}");
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

                    cache.Set(questionSetVersion, queryQuestions, getCacheExpiration());
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

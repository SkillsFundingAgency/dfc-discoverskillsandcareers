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

namespace Dfc.DiscoverSkillsAndCareers.Repositories
{
    [ExcludeFromCodeCoverage]
    public class QuestionRepository : IQuestionRepository
    {
        readonly ICosmosSettings cosmosSettings;
        readonly string collectionName;
        readonly DocumentClient client;

        public QuestionRepository(DocumentClient client, IOptions<CosmosSettings> cosmosSettings)
        {
            this.cosmosSettings = cosmosSettings?.Value;
            this.collectionName = "Questions";
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
                var uri = UriFactory.CreateDocumentUri(cosmosSettings.DatabaseName, collectionName, questionId);
                int pos = questionId.LastIndexOf('-');
                string partitionKey = questionId.Substring(0, pos);
                var requestOptions = new RequestOptions { PartitionKey = new PartitionKey(partitionKey) };
                var document = await client.ReadDocumentAsync<Question>(uri, requestOptions);
                return document;
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
                return await client.CreateDocumentAsync(uri, question);
            }
            catch
            {
                return await client.UpsertDocumentAsync(uri, question);
            }
        }

        public async Task<Question[]> GetQuestions(string assessmentType, string title, int version)
        {
            try
            {
                var uri = UriFactory.CreateDocumentCollectionUri(cosmosSettings.DatabaseName, collectionName);
                FeedOptions feedOptions = new FeedOptions() { EnableCrossPartitionQuery = true };
                var queryQuestions = client.CreateDocumentQuery<Question>(uri, feedOptions)
                                       .Where(x => x.PartitionKey == $"{assessmentType.ToLower()}-{title.ToLower()}-{version}")
                                       .AsEnumerable()
                                       .ToArray();
                return await Task.FromResult(queryQuestions);
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

        public async Task<Question[]> GetQuestions(string questionSetVersion)
        {
            try
            {
                var uri = UriFactory.CreateDocumentCollectionUri(cosmosSettings.DatabaseName, collectionName);
                FeedOptions feedOptions = new FeedOptions() { EnableCrossPartitionQuery = true };
                var queryQuestions = client.CreateDocumentQuery<Question>(uri, feedOptions)
                                       .Where(x => x.PartitionKey == questionSetVersion)
                                       .AsEnumerable()
                                       .ToArray();
                return await Task.FromResult(queryQuestions);
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
    }
}

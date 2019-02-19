using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Dfc.DiscoverSkillsAndCareers.Models;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Microsoft.Extensions.Options;

namespace Dfc.DiscoverSkillsAndCareers.Repositories
{
    public class QuestionRepository : IQuestionRepository
    {
        readonly ICosmosSettings cosmosSettings;
        readonly string collectionName;
        readonly DocumentClient client;

        //public QuestionRepository(ICosmosSettings cosmosSettings, string collectionName = "Questions")
        //{
        //    this.cosmosSettings = cosmosSettings;
        //    this.collectionName = collectionName;
        //    client = new DocumentClient(new Uri(cosmosSettings.Endpoint), cosmosSettings.Key);
        //}

        public QuestionRepository(IOptions<CosmosSettings> cosmosSettings, string collectionName)
        {
            this.cosmosSettings = cosmosSettings?.Value;
            this.collectionName = collectionName ?? "Questions";
            client = new DocumentClient(new Uri(this.cosmosSettings.Endpoint), this.cosmosSettings.Key);
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
                string partitionKey = questionId.Split('-').FirstOrDefault();
                var requestOptions = new RequestOptions { PartitionKey = new PartitionKey(partitionKey) };
                Document document = await client.ReadDocumentAsync(uri, requestOptions);
                return (Question)(dynamic)document;
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

        public async Task<QuestionSetInfo> GetCurrentQuestionSetVersion()
        {
            // TODO: lookup
            return new QuestionSetInfo()
            {
                QuestionSetVersion = "201901",
                MaxQuestions = 4,
                AssessmentType = "short"
            };
        }

        public async Task<QuestionSetInfo> GetQuestionSetInfo(string version)
        {
            // TODO: lookup
            return new QuestionSetInfo()
            {
                QuestionSetVersion = "201901",
                MaxQuestions = 4,
                AssessmentType = "short"
            };
        }

        public async Task<List<Question>> GetQuestions(string assessmentType, string version)
        {
            try
            {
                // TODO query
                //var uri = UriFactory.CreateDocumentUri(cosmosSettings.DatabaseName, collectionName, );
                //string partitionKey = questionId.Split('-').FirstOrDefault();
                //var requestOptions = new RequestOptions { PartitionKey = new PartitionKey(partitionKey) };
                //Document document = await client.ReadDocumentAsync(uri, requestOptions);
                //return (Question)(dynamic)document;
                return new List<Question>();
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

using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Dfc.DiscoverSkillsAndCareers.Models;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;

namespace Dfc.DiscoverSkillsAndCareers.Repositories
{
    public class QuestionRepository
    {
        readonly ICosmosSettings cosmosSettings;
        readonly string collectionName;
        readonly DocumentClient client;

        public QuestionRepository(ICosmosSettings cosmosSettings, string collectionName = "Questions")
        {
            this.cosmosSettings = cosmosSettings;
            this.collectionName = collectionName;
            client = new DocumentClient(new Uri(cosmosSettings.Endpoint), cosmosSettings.Key);
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
            return await client.CreateDocumentAsync(uri, question);
        }

        public async Task<QuestionSetInfo> GetCurrentQuestionSetVersion()
        {
            // TODO: lookup
            return new QuestionSetInfo()
            {
                QuestionSetVersion = "201901",
                MaxQuestions = 40
            };
        }
    }
}

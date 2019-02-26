using Dfc.DiscoverSkillsAndCareers.Models;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Microsoft.Extensions.Options;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Dfc.DiscoverSkillsAndCareers.Repositories
{
    public class QuestionSetRepository : IQuestionSetRepository
    {
        readonly ICosmosSettings cosmosSettings;
        readonly string collectionName;
        readonly DocumentClient client;

        public QuestionSetRepository(IOptions<CosmosSettings> cosmosSettings)
        {
            this.cosmosSettings = cosmosSettings?.Value;
            this.collectionName = "QuestionSets";
            client = new DocumentClient(new Uri(this.cosmosSettings.Endpoint), this.cosmosSettings.Key);
        }

        public async Task<Document> CreateQuestionSet(QuestionSet questionSet)
        {
            var uri = UriFactory.CreateDocumentCollectionUri(cosmosSettings.DatabaseName, collectionName);
            try
            {
                return await client.CreateDocumentAsync(uri, questionSet);
            }
            catch
            {
                return await client.UpsertDocumentAsync(uri, questionSet);
            }
        }
        
        public async Task<QuestionSet> GetCurrentQuestionSet(string assessmentType, string title)
        {
            var uri = UriFactory.CreateDocumentCollectionUri(cosmosSettings.DatabaseName, collectionName);
            FeedOptions feedOptions = new FeedOptions() { EnableCrossPartitionQuery = true };
            QuestionSet queryQuestionSet = client.CreateDocumentQuery<QuestionSet>(uri, feedOptions)
                                   .Where(x => x.AssessmentType == assessmentType && x.TitleLowercase == title.ToLower())
                                   .OrderByDescending(x => x.Version)
                                   .AsEnumerable()
                                   .FirstOrDefault();
            return queryQuestionSet;
        }

        public async Task<QuestionSet> GetQuestionSetVersion(string assessmentType, string title, int version)
        {
            var uri = UriFactory.CreateDocumentCollectionUri(cosmosSettings.DatabaseName, collectionName);
            FeedOptions feedOptions = new FeedOptions() { EnableCrossPartitionQuery = true };
            QuestionSet queryQuestionSet = client.CreateDocumentQuery<QuestionSet>(uri, feedOptions)
                                   .Where(x => x.AssessmentType == assessmentType && x.TitleLowercase == title.ToLower() && x.Version == version)
                                   .AsEnumerable()
                                   .FirstOrDefault();
            return queryQuestionSet;
        }
    }
}

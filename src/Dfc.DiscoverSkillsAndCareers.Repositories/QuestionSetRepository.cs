using Dfc.DiscoverSkillsAndCareers.Models;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Microsoft.Azure.Documents.Linq;
using Microsoft.Extensions.Options;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;

namespace Dfc.DiscoverSkillsAndCareers.Repositories
{
    [ExcludeFromCodeCoverage]
    public class QuestionSetRepository : IQuestionSetRepository
    {
        readonly ICosmosSettings cosmosSettings;
        readonly string collectionName;
        readonly DocumentClient client;

        public QuestionSetRepository(DocumentClient client, IOptions<CosmosSettings> cosmosSettings)
        {
            this.cosmosSettings = cosmosSettings?.Value;
            this.collectionName = "QuestionSets";
            this.client = client;
        }

        public async Task<Document> CreateOrUpdateQuestionSet(QuestionSet questionSet)
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
        
        public async Task<QuestionSet> GetCurrentQuestionSet(string assessmentType)
        {
            var uri = UriFactory.CreateDocumentCollectionUri(cosmosSettings.DatabaseName, collectionName);
            FeedOptions feedOptions = new FeedOptions() { EnableCrossPartitionQuery = true };
            QuestionSet queryQuestionSet = client.CreateDocumentQuery<QuestionSet>(uri, feedOptions)
                                   .Where(x => x.AssessmentType == assessmentType && x.IsCurrent)
                                   .OrderByDescending(x => x.Version)
                                   .AsEnumerable()
                                   .FirstOrDefault();
            return await Task.FromResult(queryQuestionSet);
        }
        
        public async Task<QuestionSet> GetLatestQuestionSetByTypeAndKey(string assessmentType, string key)
        {
            var keyLowerCase = key.Replace(" ", "-").ToLower();
            var uri = UriFactory.CreateDocumentCollectionUri(cosmosSettings.DatabaseName, collectionName);
            FeedOptions feedOptions = new FeedOptions() { EnableCrossPartitionQuery = true };
            QuestionSet queryQuestionSet = client.CreateDocumentQuery<QuestionSet>(uri, feedOptions)
                .Where(x => x.AssessmentType == assessmentType && x.QuestionSetKey == keyLowerCase)
                .OrderByDescending(x => x.Version)
                .AsEnumerable()
                .FirstOrDefault();
            return await Task.FromResult(queryQuestionSet);
        }

        public async Task<QuestionSet> GetQuestionSetVersion(string assessmentType, string title, int version)
        {
            var titleLowercase = title.ToLower().Replace(" ", "-");
            var uri = UriFactory.CreateDocumentCollectionUri(cosmosSettings.DatabaseName, collectionName);
            
            FeedOptions feedOptions = new FeedOptions() { EnableCrossPartitionQuery = true };
            
            QuestionSet queryQuestionSet = client.CreateDocumentQuery<QuestionSet>(uri, feedOptions)
                                   .Where(x => x.AssessmentType == assessmentType && x.QuestionSetKey == titleLowercase && x.Version == version)
                                   .AsEnumerable()
                                   .FirstOrDefault();
            return await Task.FromResult(queryQuestionSet);
        }

        public async Task<List<QuestionSet>> GetCurrentFilteredQuestionSets()
        {
            var uri = UriFactory.CreateDocumentCollectionUri(cosmosSettings.DatabaseName, collectionName);
            FeedOptions feedOptions = new FeedOptions() { EnableCrossPartitionQuery = true };
            List<QuestionSet> queryQuestionSet = client.CreateDocumentQuery<QuestionSet>(uri, feedOptions)
                .Where(x => x.AssessmentType == "filtered" && x.IsCurrent == true)
                .ToList();
            return await Task.FromResult(queryQuestionSet);
        }
        
    }
}

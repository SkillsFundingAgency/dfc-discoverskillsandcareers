using Dfc.DiscoverSkillsAndCareers.Models;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Microsoft.Extensions.Options;
using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace Dfc.DiscoverSkillsAndCareers.Repositories
{
    public class QuestionSetRepository : IQuestionSetRepository
    {
        private readonly ICosmosSettings cosmosSettings;
        private readonly string collectionName;
        private readonly DocumentClient client;

        public QuestionSetRepository(DocumentClient client, IOptions<CosmosSettings> cosmosSettings)
        {
            this.cosmosSettings = cosmosSettings?.Value;

            if (this.cosmosSettings == null)
                throw new ArgumentNullException(nameof(cosmosSettings));

            this.collectionName = "QuestionSets";
            this.client = client;
        }

        public async Task<Document> CreateOrUpdateQuestionSet(QuestionSet questionSet)
        {
            var uri = UriFactory.CreateDocumentCollectionUri(this.cosmosSettings.DatabaseName, collectionName);
            var doc = await client.UpsertDocumentAsync(uri, questionSet);

            await CreateUpdateLatestQuestionSet(questionSet);

            return doc;
        }

        private async Task CreateUpdateLatestQuestionSet(QuestionSet questionSet)
        {
            if (questionSet?.IsCurrent == true)
            {
                questionSet.PartitionKey = "latest-questionset";
                questionSet.QuestionSetVersion = $"latest-{questionSet.AssessmentType}";

                var uri = UriFactory.CreateDocumentCollectionUri(cosmosSettings.DatabaseName, collectionName);
                await client.UpsertDocumentAsync(uri, questionSet);
            }
        }

        public async Task<QuestionSet> GetCurrentQuestionSet(string assessmentType)
        {
            var feedOptions = new RequestOptions()
            {
                PartitionKey = new PartitionKey("latest-questionset")
            };

            var uri = UriFactory.CreateDocumentUri(cosmosSettings.DatabaseName, collectionName, $"latest-{assessmentType}");
            QuestionSet qs = null;

            try
            {
                var result = await client.ReadDocumentAsync<QuestionSet>(uri, feedOptions);
                qs = result.Document;
            }
            catch (DocumentClientException ex)
            {
                if (ex.StatusCode == HttpStatusCode.NotFound)
                {
                    var collectionUri = UriFactory.CreateDocumentCollectionUri(cosmosSettings.DatabaseName, collectionName);
                    QuestionSet latestQs = client.CreateDocumentQuery<QuestionSet>(collectionUri, new FeedOptions() { EnableCrossPartitionQuery = true })
                                   .Where(x => x.AssessmentType == assessmentType && x.IsCurrent)
                                   .OrderByDescending(x => x.Version)
                                   .AsEnumerable()
                                   .FirstOrDefault();

                    await CreateUpdateLatestQuestionSet(latestQs);

                    qs = latestQs;
                }
            }

            if (qs != null)
            {
                qs.QuestionSetVersion = $"{assessmentType.ToLower()}-{qs.Title.ToLower()}-{qs.Version.ToString()}";
                qs.PartitionKey = "ncs";
            }

            return qs;
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
    }
}
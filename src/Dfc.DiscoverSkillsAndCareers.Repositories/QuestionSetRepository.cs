using System;
using Dfc.DiscoverSkillsAndCareers.Models;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Microsoft.Azure.Documents.Linq;
using Microsoft.Extensions.Options;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;
using Dfc.DiscoverSkillsAndCareers.Models.Extensions;
using Microsoft.Extensions.Caching.Memory;

namespace Dfc.DiscoverSkillsAndCareers.Repositories
{
    [ExcludeFromCodeCoverage]
    public class QuestionSetRepository : IQuestionSetRepository
    {
        readonly TimeSpan cacheExpiration = TimeSpan.FromMinutes(30);
        readonly ICosmosSettings cosmosSettings;
        readonly string collectionName;
        readonly DocumentClient client;
        private readonly IMemoryCache cache;
        private QuestionSet latestQuestionSetShort;
        private QuestionSet latestQuestionSetFiltered;

        public QuestionSetRepository(DocumentClient client, IOptions<CosmosSettings> cosmosSettings, IMemoryCache cache)
        {
            this.cosmosSettings = cosmosSettings?.Value;
            this.collectionName = "QuestionSets";
            this.client = client;
            this.cache = cache;
        }

        public async Task<Document> CreateOrUpdateQuestionSet(QuestionSet questionSet)
        {
            var uri = UriFactory.CreateDocumentCollectionUri(cosmosSettings.DatabaseName, collectionName);
            try
            {
                cache.Set(questionSet.QuestionSetKey, questionSet, cacheExpiration);

                if (questionSet.IsCurrent)
                {
                    if (questionSet.AssessmentType.EqualsIgnoreCase("short"))
                    {
                        latestQuestionSetShort = questionSet;
                    }
                    else
                    {
                        latestQuestionSetFiltered = questionSet;
                    }
                }

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

            var latestQuestionSet = assessmentType.EqualsIgnoreCase("short")
                ? latestQuestionSetShort
                : latestQuestionSetFiltered;
            
            if (latestQuestionSet == null)
            {
                QuestionSet questionSet = client.CreateDocumentQuery<QuestionSet>(uri, feedOptions)
                    .Where(x => x.AssessmentType == assessmentType && x.IsCurrent)
                    .OrderByDescending(x => x.Version)
                    .AsEnumerable()
                    .FirstOrDefault();

                if (questionSet != null)
                {
                    if (questionSet.AssessmentType.EqualsIgnoreCase("short"))
                    {
                        latestQuestionSetShort = questionSet;
                    }
                    else
                    {
                        latestQuestionSetFiltered = questionSet;
                    }

                    latestQuestionSet = questionSet;
                    
                    cache.Set(latestQuestionSet.QuestionSetKey, latestQuestionSet, cacheExpiration);
                }
            }

            return await Task.FromResult(latestQuestionSet);
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

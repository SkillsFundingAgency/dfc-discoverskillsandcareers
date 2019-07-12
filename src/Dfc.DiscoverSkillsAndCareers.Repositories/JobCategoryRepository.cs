using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using Dfc.DiscoverSkillsAndCareers.Models;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Microsoft.Extensions.Options;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Dfc.DiscoverSkillsAndCareers.Models.Extensions;
using Microsoft.Extensions.Caching.Memory;

namespace Dfc.DiscoverSkillsAndCareers.Repositories
{
    [ExcludeFromCodeCoverage]
    public class JobCategoryRepository : IJobCategoryRepository
    {
        readonly TimeSpan cacheExpiration = TimeSpan.FromMinutes(30);
        private readonly IMemoryCache cache;
        readonly ICosmosSettings cosmosSettings;
        readonly string collectionName;
        readonly DocumentClient client;

        public JobCategoryRepository(DocumentClient client, IOptions<CosmosSettings> cosmosSettings, IMemoryCache cache)
        {
            this.cosmosSettings = cosmosSettings?.Value;
            this.collectionName = "QuestionSets";
            this.client = client;
            this.cache = cache;

        }
        
        public async Task<JobCategory> GetJobCategory(string jobCategoryCode, string partitionKey = "job-categories")
        {
            try
            {
                var jobCategories = await GetJobCategories(partitionKey);
                return jobCategories?.FirstOrDefault(j => j.Code.EqualsIgnoreCase(jobCategoryCode));
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

        public async Task CreateOrUpdateJobCategory(JobCategory jobCategory)
        {
            var uri = UriFactory.CreateDocumentCollectionUri(cosmosSettings.DatabaseName, collectionName);
            try
            {
                if (cache.TryGetValue<JobCategory[]>(jobCategory.PartitionKey, out var jobCategories))
                {
                    var index = Array.FindIndex(jobCategories, q => q.Code.EqualsIgnoreCase(jobCategory.Code));

                    if (index == -1)
                    {
                        var x = jobCategories.ToList();
                        x.Add(jobCategory);
                        jobCategories = x.ToArray();
                    }
                    else
                    {
                        jobCategories[index] = jobCategory;
                    }
                    
                    cache.Set(jobCategory.PartitionKey, jobCategories, cacheExpiration);
                }
                
                await client.CreateDocumentAsync(uri, jobCategory);
            }
            catch
            {
                await client.UpsertDocumentAsync(uri, jobCategory);
            }
        }

        public async Task<JobCategory[]> GetJobCategories(string partitionKey = "job-categories")
        {
            try
            {
                if (!cache.TryGetValue<JobCategory[]>(partitionKey, out var jobCategories))
                {
                    var uri = UriFactory.CreateDocumentCollectionUri(cosmosSettings.DatabaseName, collectionName);
                    FeedOptions feedOptions = new FeedOptions() {PartitionKey = new PartitionKey(partitionKey)};
                    jobCategories = client.CreateDocumentQuery<JobCategory>(uri, feedOptions)
                        .AsEnumerable()
                        .ToArray();

                    cache.Set(partitionKey, jobCategories, cacheExpiration);
                }
                
                return jobCategories;
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

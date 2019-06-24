using System.Collections.Generic;
using System.Collections.ObjectModel;
using Dfc.DiscoverSkillsAndCareers.Models;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Microsoft.Extensions.Options;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace Dfc.DiscoverSkillsAndCareers.Repositories
{
    public class JobCategoryRepository : IJobCategoryRepository
    {
        readonly ICosmosSettings cosmosSettings;
        readonly string collectionName;
        readonly DocumentClient client;

        public JobCategoryRepository(DocumentClient client, IOptions<CosmosSettings> cosmosSettings)
        {
            this.cosmosSettings = cosmosSettings?.Value;
            this.collectionName = "QuestionSets";
            this.client = client;

        }
        
        public async Task<JobCategory> GetJobCategory(string jobCategoryCode, string partitionKey = "job-categories")
        {
            try
            {
                var uri = UriFactory.CreateDocumentUri(cosmosSettings.DatabaseName, collectionName, jobCategoryCode);
                var requestOptions = new RequestOptions { PartitionKey = new PartitionKey(partitionKey) };
                var document = await client.ReadDocumentAsync<JobCategory>(uri, requestOptions);
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

        public async Task CreateOrUpdateJobCategory(JobCategory jobCategory)
        {
            var uri = UriFactory.CreateDocumentCollectionUri(cosmosSettings.DatabaseName, collectionName);
            try
            {
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
                var uri = UriFactory.CreateDocumentCollectionUri(cosmosSettings.DatabaseName, collectionName);
                FeedOptions feedOptions = new FeedOptions() { PartitionKey = new PartitionKey(partitionKey) };
                var queryQuestions = client.CreateDocumentQuery<JobCategory>(uri, feedOptions)
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

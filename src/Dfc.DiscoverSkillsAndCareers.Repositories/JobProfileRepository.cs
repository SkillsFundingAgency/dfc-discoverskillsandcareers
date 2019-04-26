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
    public class JobProfileRepository : IJobProfileRepository
    {
        readonly ICosmosSettings cosmosSettings;
        readonly string collectionName;
        readonly DocumentClient client;

        public JobProfileRepository(DocumentClient client, IOptions<CosmosSettings> cosmosSettings)
        {
            this.cosmosSettings = cosmosSettings?.Value;
            this.collectionName = "QuestionSets";
            this.client = client;
        }

        public async Task<JobProfile> GetJobProfile(string socCode, string partitionKey)
        {
            try
            {
                var uri = UriFactory.CreateDocumentUri(cosmosSettings.DatabaseName, collectionName, socCode);
                var requestOptions = new RequestOptions { PartitionKey = new PartitionKey(partitionKey) };
                Document document = await client.ReadDocumentAsync(uri, requestOptions);
                return (JobProfile)(dynamic)document;
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

        public async Task<Document> CreateJobProfile(JobProfile jobProfile)
        {
            var uri = UriFactory.CreateDocumentCollectionUri(cosmosSettings.DatabaseName, collectionName);
            try
            {
                return await client.CreateDocumentAsync(uri, jobProfile);
            }
            catch
            {
                return await client.UpsertDocumentAsync(uri, jobProfile);
            }
        }

        public async Task<JobProfile[]> GetJobProfilesForJobFamily(string jobFamily)
        {
            try
            {
                var uri = UriFactory.CreateDocumentCollectionUri(cosmosSettings.DatabaseName, collectionName);
                FeedOptions feedOptions = new FeedOptions() { EnableCrossPartitionQuery = true };
                var queryQuestions = client.CreateDocumentQuery<JobProfile>(uri, feedOptions)
                                       .Where(x => x.JobProfileCategories.Contains(jobFamily))
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

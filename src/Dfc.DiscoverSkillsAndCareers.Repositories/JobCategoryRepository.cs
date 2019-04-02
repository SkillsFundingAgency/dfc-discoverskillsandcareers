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

        public async Task<JobFamily> GetJobCategory(string socCode, string partitionKey)
        {
            try
            {
                var uri = UriFactory.CreateDocumentUri(cosmosSettings.DatabaseName, collectionName, socCode);
                var requestOptions = new RequestOptions { PartitionKey = new PartitionKey(partitionKey) };
                Document document = await client.ReadDocumentAsync(uri, requestOptions);
                return (JobFamily)(dynamic)document;
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

        public async Task CreateJobCategory(JobFamily jobProfile)
        {
            var uri = UriFactory.CreateDocumentCollectionUri(cosmosSettings.DatabaseName, collectionName);
            try
            {
                await client.CreateDocumentAsync(uri, jobProfile);
            }
            catch
            {
                await client.UpsertDocumentAsync(uri, jobProfile);
            }
        }

        public async Task<JobFamily[]> GetJobCategories()
        {
            try
            {
                var uri = UriFactory.CreateDocumentCollectionUri(cosmosSettings.DatabaseName, collectionName);
                FeedOptions feedOptions = new FeedOptions() { EnableCrossPartitionQuery = true };
                var queryQuestions = client.CreateDocumentQuery<JobFamily>(uri, feedOptions)
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

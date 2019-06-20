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
    public class ShortTraitRepository : IShortTraitRepository
    {
        readonly ICosmosSettings cosmosSettings;
        readonly string collectionName;
        readonly DocumentClient client;

        public ShortTraitRepository(DocumentClient client, IOptions<CosmosSettings> cosmosSettings)
        {
            this.cosmosSettings = cosmosSettings?.Value;
            this.collectionName = "QuestionSets";
            this.client = client;
        }

        
        public async Task CreateTrait(Trait trait, string partitionKey = "traits")
        {
            trait.PartitionKey = partitionKey;
            var uri = UriFactory.CreateDocumentCollectionUri(cosmosSettings.DatabaseName, collectionName);
            try
            {
                await client.CreateDocumentAsync(uri, trait);
            }
            catch
            {
                await client.UpsertDocumentAsync(uri, trait);
            }
        }

        public async Task<Trait[]> GetTraits(string partitionKey = "tratis")
        {
            try
            {
                var uri = UriFactory.CreateDocumentCollectionUri(cosmosSettings.DatabaseName, collectionName);
                FeedOptions feedOptions = new FeedOptions() { EnableCrossPartitionQuery = true };
                var queryQuestions = client.CreateDocumentQuery<Trait>(uri, feedOptions)
                                       .Where(x => x.PartitionKey == partitionKey)
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

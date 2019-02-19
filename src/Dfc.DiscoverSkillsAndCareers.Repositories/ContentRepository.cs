using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Dfc.DiscoverSkillsAndCareers.Models;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Microsoft.Extensions.Options;

namespace Dfc.DiscoverSkillsAndCareers.Repositories
{
    public class ContentRepository : IContentRepository
    {
        readonly ICosmosSettings cosmosSettings;
        readonly string collectionName;
        readonly DocumentClient client;

        public ContentRepository(IOptions<CosmosSettings> cosmosSettings, string collectionName)
        {
            this.cosmosSettings = cosmosSettings?.Value;
            this.collectionName = collectionName ?? "Contents";
            client = new DocumentClient(new Uri(this.cosmosSettings.Endpoint), this.cosmosSettings.Key);
        }

        public async Task<Content> GetContent(string contentType)
        {
            try
            {
                var uri = UriFactory.CreateDocumentUri(cosmosSettings.DatabaseName, collectionName, contentType);
                var requestOptions = new RequestOptions { PartitionKey = new PartitionKey(contentType) };
                Document document = await client.ReadDocumentAsync(uri, requestOptions);
                return (Content)(dynamic)document;
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

        public async Task<Content> CreateContent(Content contentModel)
        {
            var uri = UriFactory.CreateDocumentCollectionUri(cosmosSettings.DatabaseName, collectionName);
            try
            {
                Document document = await client.CreateDocumentAsync(uri, contentModel);
                return (Content)(dynamic)document;
            }
            catch
            {
                Document document = await client.UpsertDocumentAsync(uri, contentModel);
                return (Content)(dynamic)document;
            }
            
        }
    }
}

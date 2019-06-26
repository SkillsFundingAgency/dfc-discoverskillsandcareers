using Dfc.DiscoverSkillsAndCareers.Models;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Microsoft.Extensions.Options;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Net;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace Dfc.DiscoverSkillsAndCareers.Repositories
{
    [ExcludeFromCodeCoverage]
    public class ContentRepository : IContentRepository
    {
        readonly ICosmosSettings cosmosSettings;
        readonly string collectionName;
        readonly DocumentClient client;

        public ContentRepository(ILogger<ContentRepository> logger, DocumentClient client, IOptions<CosmosSettings> cosmosSettings)
        {
            logger.LogInformation($"Config: {Newtonsoft.Json.JsonConvert.SerializeObject(cosmosSettings)}");

            this.cosmosSettings = cosmosSettings?.Value;
            this.collectionName = "Contents";
            this.client = client;
        }

        public async Task<Content> GetContent(string contentType)
        {
            try
            {
                var uri = UriFactory.CreateDocumentUri(cosmosSettings.DatabaseName, collectionName, contentType);
                var requestOptions = new RequestOptions { PartitionKey = new PartitionKey(contentType) };
                var document = await client.ReadDocumentAsync<Content>(uri, requestOptions);
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

        public async Task CreateContent(Content contentModel)
        {
            contentModel.Id = contentModel.ContentType.ToLower();
            var uri = UriFactory.CreateDocumentCollectionUri(cosmosSettings.DatabaseName, collectionName);
            try
            {
                await client.CreateDocumentAsync(uri, contentModel);
            
            }
            catch
            {
                await client.UpsertDocumentAsync(uri, contentModel);
                
            }
            
        }
    }
}

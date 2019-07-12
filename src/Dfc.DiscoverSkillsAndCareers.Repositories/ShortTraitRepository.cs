using Dfc.DiscoverSkillsAndCareers.Models;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Microsoft.Extensions.Options;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Dfc.DiscoverSkillsAndCareers.Models.Extensions;
using Microsoft.Extensions.Caching.Memory;

namespace Dfc.DiscoverSkillsAndCareers.Repositories
{
    [ExcludeFromCodeCoverage]
    public class ShortTraitRepository : IShortTraitRepository
    {
        readonly TimeSpan cacheExpiration = TimeSpan.FromMinutes(30);
        readonly ICosmosSettings cosmosSettings;
        readonly string collectionName;
        readonly DocumentClient client;
        private IMemoryCache cache;

        public ShortTraitRepository(DocumentClient client, IOptions<CosmosSettings> cosmosSettings, IMemoryCache cache)
        {
            this.cosmosSettings = cosmosSettings?.Value;
            this.collectionName = "QuestionSets";
            this.client = client;
            this.cache = cache;
        }

        
        public async Task CreateTrait(Trait trait, string partitionKey = "traits")
        {
            trait.PartitionKey = partitionKey;
            var uri = UriFactory.CreateDocumentCollectionUri(cosmosSettings.DatabaseName, collectionName);
            try
            {
                if (cache.TryGetValue<Trait[]>(partitionKey, out var traits))
                {
                    var index = Array.FindIndex(traits, q => q.TraitCode.EqualsIgnoreCase(trait.TraitCode));

                    if (index == -1)
                    {
                        var x = traits.ToList();
                        x.Add(trait);
                        traits = x.ToArray();
                    }
                    else
                    {
                        traits[index] = trait;
                    }
                    
                    cache.Set(partitionKey, traits, cacheExpiration);
                }
                await client.CreateDocumentAsync(uri, trait);
            }
            catch
            {
                await client.UpsertDocumentAsync(uri, trait);
            }
        }

        public async Task<Trait[]> GetTraits(string partitionKey = "traits")
        {
            try
            {
                var uri = UriFactory.CreateDocumentCollectionUri(cosmosSettings.DatabaseName, collectionName);
                FeedOptions feedOptions = new FeedOptions() { EnableCrossPartitionQuery = true };

                if (!cache.TryGetValue<Trait[]>(partitionKey, out var traits))
                {
                    traits = client.CreateDocumentQuery<Trait>(uri, feedOptions)
                        .Where(x => x.PartitionKey == partitionKey)
                        .AsEnumerable()
                        .ToArray();

                    cache.Set(partitionKey, traits, cacheExpiration);
                }

                return await Task.FromResult(traits);
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

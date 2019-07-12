using Dfc.DiscoverSkillsAndCareers.Models;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Microsoft.Extensions.Options;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Net;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;

namespace Dfc.DiscoverSkillsAndCareers.Repositories
{
    [ExcludeFromCodeCoverage]
    public class UserSessionRepository : IUserSessionRepository
    {
        readonly TimeSpan cacheExpiration = TimeSpan.FromMinutes(30);
        readonly ICosmosSettings cosmosSettings;
        readonly string collectionName;
        readonly DocumentClient client;
        private IMemoryCache cache;

        public UserSessionRepository(DocumentClient client, IOptions<CosmosSettings> cosmosSettings, IMemoryCache cache)
        {
            this.cosmosSettings = cosmosSettings?.Value;
            this.collectionName = "UserSessions";
            this.client = client;
            this.cache = cache;
        }

        public async Task<UserSession> GetUserSession(string primaryKey, bool useCache = true)
        {
            primaryKey = primaryKey.ToLower().Replace(" ", "");
            int pos = primaryKey.IndexOf('-');
            if (pos <= 0)
            {
                return null;
            }
            string partitionKey = primaryKey.Substring(0, pos);
            string userSessionId = primaryKey.Substring(pos + 1, primaryKey.Length - (pos + 1));
            return await GetUserSession(userSessionId, partitionKey, useCache);
        }

        private async Task<UserSession> GetUserSession(string userSessionId, string partitionKey, bool useCache = true)
        {
            try
            {
                if (!useCache || !cache.TryGetValue<UserSession>(userSessionId, out var session))
                {
                    var uri = UriFactory.CreateDocumentUri(cosmosSettings.DatabaseName, collectionName, userSessionId);
                    var requestOptions = new RequestOptions { PartitionKey = new PartitionKey(partitionKey) };
                    session = await client.ReadDocumentAsync<UserSession>(uri, requestOptions);
                    if (session != null)
                    {
                        cache.Set(session.UserSessionId, session, cacheExpiration);
                    }
                }

                return session;
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

        public async Task CreateUserSession(UserSession userSession)
        {
            var uri = UriFactory.CreateDocumentCollectionUri(cosmosSettings.DatabaseName, collectionName);
            cache.Set(userSession.UserSessionId, userSession, cacheExpiration);
            await client.CreateDocumentAsync(uri, userSession);
        }

        public async Task UpdateUserSession(UserSession userSession)
        {
            userSession.LastUpdatedDt = DateTime.UtcNow;
            var uri = UriFactory.CreateDocumentUri(cosmosSettings.DatabaseName, collectionName, userSession.UserSessionId);
            var requestOptions = new RequestOptions { PartitionKey = new PartitionKey(userSession.PartitionKey) };
            cache.Set(userSession.UserSessionId, userSession, cacheExpiration);
            await client.ReplaceDocumentAsync(uri, userSession, requestOptions);
        }
    }
}

using Dfc.DiscoverSkillsAndCareers.Models;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Microsoft.Extensions.Options;
using System;
using System.Net;
using System.Threading.Tasks;

namespace Dfc.DiscoverSkillsAndCareers.Repositories
{
    public class UserSessionRepository : IUserSessionRepository
    {
        readonly ICosmosSettings cosmosSettings;
        readonly string collectionName;
        readonly DocumentClient client;

        public UserSessionRepository(DocumentClient client, IOptions<CosmosSettings> cosmosSettings)
        {
            this.cosmosSettings = cosmosSettings?.Value;
            this.collectionName = "UserSessions";
            this.client = client;
        }

        public async Task<UserSession> GetUserSession(string primaryKey)
        {
            primaryKey = primaryKey.ToLower().Replace(" ", "");
            int pos = primaryKey.IndexOf('-');
            if (pos <= 0)
            {
                return null;
            }
            string partitionKey = primaryKey.Substring(0, pos);
            string userSessionId = primaryKey.Substring(pos + 1, primaryKey.Length - (pos + 1));
            return await GetUserSession(userSessionId, partitionKey);
        }

        public async Task<UserSession> GetUserSession(string userSessionId, string partitionKey)
        {
            try
            {
                var uri = UriFactory.CreateDocumentUri(cosmosSettings.DatabaseName, collectionName, userSessionId);
                var requestOptions = new RequestOptions { PartitionKey = new PartitionKey(partitionKey) };
                Document document = await client.ReadDocumentAsync(uri, requestOptions);
                return (UserSession)(dynamic)document;
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
            await client.CreateDocumentAsync(uri, userSession);
        }

        public async Task<UserSession> UpdateUserSession(UserSession userSession)
        {
            var uri = UriFactory.CreateDocumentUri(cosmosSettings.DatabaseName, collectionName, userSession.UserSessionId);
            var requestOptions = new RequestOptions { PartitionKey = new PartitionKey(userSession.PartitionKey) };
            var response = await client.ReplaceDocumentAsync(uri, userSession, requestOptions);
            var updated = response.Resource;
            return (UserSession)(dynamic)updated;
        }
    }
}

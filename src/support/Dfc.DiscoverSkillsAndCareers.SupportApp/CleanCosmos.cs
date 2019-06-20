using System;
using System.Threading.Tasks;
using CommandLine;
using Dfc.DiscoverSkillsAndCareers.CmsFunctionApp;
using Dfc.DiscoverSkillsAndCareers.Models;
using Dfc.DiscoverSkillsAndCareers.Repositories;
using Dfc.DiscoverSkillsAndCareers.SupportApp.Models;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Microsoft.Azure.Documents.Linq;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Dfc.DiscoverSkillsAndCareers.SupportApp
{
    public class CleanCosmos
    {
        [Verb("clean-cosmos", HelpText = "Cleans the cosmos DB DYSAC.")]
        public class Options : AppSettings
        {
            public CosmosSettings Cosmos { get; set; } = new CosmosSettings();

        }

        public static SuccessFailCode Execute(IServiceProvider services, Options opts)
        {
            var logger = services.GetService<ILogger<CleanCosmos>>();

            try
            {
                var configuration = services.GetService<IConfiguration>();
                configuration.Bind(opts);

                var client = new DocumentClient(new Uri(opts.Cosmos.Endpoint), opts.Cosmos.Key);

                DeleteAll<UserSession>(logger, client, opts.Cosmos.DatabaseName, "UserSessions", a => (a.UserSessionId, a.PartitionKey)).GetAwaiter().GetResult();
                DeleteAll<Question>(logger, client, opts.Cosmos.DatabaseName, "Questions", a => (a.QuestionId, a.PartitionKey)).GetAwaiter().GetResult();
                DeleteAll<QuestionSet>(logger, client, opts.Cosmos.DatabaseName, "QuestionSets", a => (a.QuestionSetVersion, a.PartitionKey)).GetAwaiter().GetResult();
                
                return SuccessFailCode.Succeed;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "An error occured ");
                return SuccessFailCode.Fail;
            }

        }

        public static async Task DeleteAll<T>(ILogger logger, DocumentClient client, string dbName, string collection, Func<T, (string,string)> idMapper)
        {
            var collectionQuery = client.CreateDocumentQuery<T>(
                UriFactory.CreateDocumentCollectionUri(dbName, collection),
                new FeedOptions
                {
                    EnableCrossPartitionQuery = true, 
                    MaxItemCount = 100
                }
            );

            using (var query = collectionQuery.AsDocumentQuery())
            {
                while (query.HasMoreResults)
                {
                    var docs = await query.ExecuteNextAsync<T>();
                    foreach (var doc in docs)
                    {
                        var (id, partitionKey) = idMapper(doc);
                        logger.LogInformation($"Deleting document {id}");
                        await client.DeleteDocumentAsync(UriFactory.CreateDocumentUri(dbName, collection, id), new RequestOptions
                        {
                            PartitionKey = new PartitionKey(partitionKey)
                        });
                    }
                }
            }
        }
    }
}
using Dfc.DiscoverSkillsAndCareers.ChangeFeed.Common.Blob;
using Dfc.DiscoverSkillsAndCareers.ChangeFeed.Common.Queue;
using DFC.Functions.DI.Standard.Attributes;
using Microsoft.Azure.Documents;
using Microsoft.Azure.ServiceBus;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Dfc.DiscoverSkillsAndCareers.ChangeFeed.Triggers
{
    public static class UserSessionChangeFeedTrigger
    {
        [FunctionName("UserSessionChangeFeedTrigger")]
        public static async Task RunAsync([CosmosDBTrigger(
            databaseName: "TestDatabase",
            collectionName: "UserSessions",
            ConnectionStringSetting = "AzureCosmosDBConnection",
            LeaseCollectionName = "leases",
            CreateLeaseCollectionIfNotExists = true)]IReadOnlyList<Document> input,
            ILogger log,
            [Inject]IBlobStorageService blobStorageService,
            [Inject]IOptions<ServiceBusSettings> serviceBusSettingsOptions)
        {
            var serviceBusSettings = serviceBusSettingsOptions.Value;
            foreach (var doc in input)
            {
                var userSession = (Dfc.DiscoverSkillsAndCareers.Models.UserSession)(dynamic)doc;
                log.LogInformation($"Handling usersession update id={userSession.UserSessionId}");

                // Create a blob
                var blobName = Guid.NewGuid().ToString();
                var blobContent = JsonConvert.SerializeObject(userSession);
                var blockBlob = await blobStorageService.CreateBlob(blobName, blobContent);
                log.LogInformation($"Added {userSession.UserSessionId} to blob {blobName}");


                // Add message to queue
                var messageContent = new ChangeFeedQueueItem()
                {
                    Type = "UserSession",
                    BlobName = blockBlob.Name
                };

                // Add to service bus queue
                var queueClient = new QueueClient(serviceBusSettings.ServiceBusConnectionString, serviceBusSettings.QueueName);
                var json = JsonConvert.SerializeObject(messageContent);
                Message message = new Message(System.Text.Encoding.ASCII.GetBytes(json));
                await queueClient.SendAsync(message);
                log.LogInformation($"Added {userSession.UserSessionId} to queue {serviceBusSettings.QueueName}");
            }
        }
    }
}


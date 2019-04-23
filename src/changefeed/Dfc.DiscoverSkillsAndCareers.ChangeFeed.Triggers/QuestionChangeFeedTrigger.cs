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
    public static class QuestionChangeFeedTrigger
    {
        [FunctionName("QuestionChangeFeedTrigger")]
        public static async Task RunAsync([CosmosDBTrigger(
            databaseName: "TestDatabase",
            collectionName: "Questions",
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
                var question = (Dfc.DiscoverSkillsAndCareers.Models.Question)(dynamic)doc;
                log.LogInformation($"Handling question update id={question.QuestionId}");

                // Create a blob
                var blobName = Guid.NewGuid().ToString();
                var blobContent = JsonConvert.SerializeObject(question);
                var blockBlob = await blobStorageService.CreateBlob(blobName, blobContent);
                log.LogInformation($"Added {question.QuestionId} to blob {blobName}");


                // Add message to queue
                var messageContent = new ChangeFeedQueueItem()
                {
                    Type = "Question",
                    BlobName = blockBlob.Name
                };

                // Add to service bus queue
                var queueClient = new QueueClient(serviceBusSettings.ServiceBusConnectionString, serviceBusSettings.QueueName);
                var json = JsonConvert.SerializeObject(messageContent);
                Message message = new Message(System.Text.Encoding.ASCII.GetBytes(json));
                await queueClient.SendAsync(message);
                log.LogInformation($"Added {question.QuestionId} to queue {serviceBusSettings.QueueName}");
            }
        }
    }
}


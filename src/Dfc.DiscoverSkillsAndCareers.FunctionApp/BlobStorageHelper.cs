using System;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;

namespace Dfc.DiscoverSkillsAndCareers.FunctionApp
{
    public class BlobStorageHelper
    {
        public static async Task<CloudBlockBlob> GetCloudBlockBlob(string blobName)
        {
            string containerReference = "mycontainer";
            string storageConnectionString = Environment.GetEnvironmentVariable("storageconnectionstring");

            // TODO: local - need to ensure config 
            storageConnectionString = @"DefaultEndpointsProtocol=http;AccountName=devstoreaccount1;AccountKey=Eby8vdM02xNOcqFlqUwJPLlmEtlCDXJ1OUzFT50uSRZ6IFsuFq2UVErCz4I6tq/K1SZFPTOtr/KBHBeksoGMGw==;BlobEndpoint=http://127.0.0.1:10000/devstoreaccount1;TableEndpoint=http://127.0.0.1:10002/devstoreaccount1;QueueEndpoint=http://127.0.0.1:10001/devstoreaccount1;";

            CloudStorageAccount cloudStorageAccount;
            if (CloudStorageAccount.TryParse(storageConnectionString, out cloudStorageAccount) == true)
            {
                var cloudBlobClient = cloudStorageAccount.CreateCloudBlobClient();
                var cloudBlobContainer = cloudBlobClient.GetContainerReference(containerReference);
                await cloudBlobContainer.CreateIfNotExistsAsync();

                // TODO: 
                Console.WriteLine("List blobs in container.");
                BlobContinuationToken blobContinuationToken = null;
                do
                {
                    var results = await cloudBlobContainer.ListBlobsSegmentedAsync(null, blobContinuationToken);
                    blobContinuationToken = results.ContinuationToken;
                    foreach (IListBlobItem item in results.Results)
                    {
                        Console.WriteLine(item.Uri);
                    }

                } while (blobContinuationToken != null);


                CloudBlockBlob blob = cloudBlobContainer.GetBlockBlobReference(blobName);
                return blob;
            }
            return null;
        }

        public static async Task<string> GetBlob(string blobName)
        {
            try
            {
                var cloudBlockBlob = await GetCloudBlockBlob(blobName);

                string html = await cloudBlockBlob.DownloadTextAsync();
                return html;
            }
            catch (Microsoft.WindowsAzure.Storage.StorageException)
            {
                return null;
            }
        }

        public static async Task CreateBlob(string localFileName, string blobName)
        {
            var cloudBlockBlob = await GetCloudBlockBlob(blobName);
            await cloudBlockBlob.UploadFromFileAsync(localFileName);
        }
    }
}

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
            string containerReference = Environment.GetEnvironmentVariable("ContainerName");
            string storageConnectionString = Environment.GetEnvironmentVariable("BlobStorage:StorageConnectionString");

            CloudStorageAccount cloudStorageAccount;
            if (CloudStorageAccount.TryParse(storageConnectionString, out cloudStorageAccount) == true)
            {
                var cloudBlobClient = cloudStorageAccount.CreateCloudBlobClient();
                var cloudBlobContainer = cloudBlobClient.GetContainerReference(containerReference);
                await cloudBlobContainer.CreateIfNotExistsAsync();

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
            catch (StorageException)
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

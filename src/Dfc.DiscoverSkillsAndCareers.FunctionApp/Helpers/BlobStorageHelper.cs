using System;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;

namespace Dfc.DiscoverSkillsAndCareers.FunctionApp.Helpers
{
    public class BlobStorageHelper
    {
        public static async Task<CloudBlockBlob> GetCloudBlockBlob(BlobStorageSettings settings, string blobName)
        {

            CloudStorageAccount cloudStorageAccount;
            if (CloudStorageAccount.TryParse(settings.StorageConnectionString, out cloudStorageAccount) == true)
            {
                var cloudBlobClient = cloudStorageAccount.CreateCloudBlobClient();
                var cloudBlobContainer = cloudBlobClient.GetContainerReference(settings.ContainerName);
                await cloudBlobContainer.CreateIfNotExistsAsync();

                CloudBlockBlob blob = cloudBlobContainer.GetBlockBlobReference(blobName);
                return blob;
            }
            return null;
        }

        public static async Task<string> GetBlob(BlobStorageSettings settings, string blobName)
        {
            try
            {
                var cloudBlockBlob = await GetCloudBlockBlob(settings, blobName);

                string html = await cloudBlockBlob.DownloadTextAsync();
                return html;
            }
            catch (StorageException)
            {
                return null;
            }
        }

        public static async Task CreateBlob(BlobStorageSettings settings, string localFileName, string blobName)
        {
            var cloudBlockBlob = await GetCloudBlockBlob(settings, blobName);
            await cloudBlockBlob.UploadFromFileAsync(localFileName);
        }
    }
}

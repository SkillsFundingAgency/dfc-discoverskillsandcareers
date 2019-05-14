using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using System.Threading.Tasks;

namespace Dfc.DiscoverSkillsAndCareers.ChangeFeed.Common.Blob
{
    public class BlobStorageService : IBlobStorageService
    {
        public BlobStorageSettings BlobStorageSettings { get; private set; }

        public BlobStorageService(BlobStorageSettings blobStorageSettings)
        {
            BlobStorageSettings = blobStorageSettings;
        }

        public async Task<CloudBlockBlob> GetCloudBlockBlob(string blobName)
        {

            CloudStorageAccount cloudStorageAccount;
            if (CloudStorageAccount.TryParse(BlobStorageSettings.StorageConnectionString, out cloudStorageAccount) == true)
            {
                var cloudBlobClient = cloudStorageAccount.CreateCloudBlobClient();
                var cloudBlobContainer = cloudBlobClient.GetContainerReference(BlobStorageSettings.ContainerName);
                await cloudBlobContainer.CreateIfNotExistsAsync();

                CloudBlockBlob blob = cloudBlobContainer.GetBlockBlobReference(blobName);
                return blob;
            }
            return null;
        }

        public async Task<string> GetBlobText(string blobName)
        {
            try
            {
                var cloudBlockBlob = await GetCloudBlockBlob(blobName);
                return await cloudBlockBlob.DownloadTextAsync();
            }
            catch (StorageException)
            {
                return null;
            }
        }

        public async Task<CloudBlockBlob> CreateBlob(string blobName, string content)
        {
            var cloudBlockBlob = await GetCloudBlockBlob(blobName);
            await cloudBlockBlob.UploadTextAsync(content);
            return cloudBlockBlob;
        }

        public async Task DeleteBlob(string blobName)
        {
            var cloudBlockBlob = await GetCloudBlockBlob(blobName);
            await cloudBlockBlob.DeleteAsync();
        }
    }
}

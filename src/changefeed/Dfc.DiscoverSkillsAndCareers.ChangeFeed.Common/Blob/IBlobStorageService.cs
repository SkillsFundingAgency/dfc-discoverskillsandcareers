using Microsoft.WindowsAzure.Storage.Blob;
using System.Threading.Tasks;

namespace Dfc.DiscoverSkillsAndCareers.ChangeFeed.Common.Blob
{
    public interface IBlobStorageService
    {
        BlobStorageSettings BlobStorageSettings { get; }
        Task<CloudBlockBlob> GetCloudBlockBlob(string blobName);
        Task<string> GetBlobText(string blobName);
        Task<CloudBlockBlob> CreateBlob(string blobName, string content);
        Task DeleteBlob(string blobName);
    }
}

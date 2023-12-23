using AzureBlobStorage.Models;
using System.Reflection.Metadata;

namespace AzureBlobStorage.Services
{
    public interface IBlobService
    {
        Task<List<string>> GetAllBlobs(string containerName);

        Task<string> GetBlob(string name, string containerName);

        Task<bool> UploadBlob(string name,IFormFile file,string containerName, Models.Blob blob);

        Task<bool> DeleteBlob(string name, string containerName);

        Task<List<Models.Blob>> GetAllBlobsWithUri(string containerName);
    }
}

using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;

namespace AzureBlobStorage.Services
{
    public class BlobService : IBlobService
    {
        private readonly BlobServiceClient _blobClient;
        public BlobService(BlobServiceClient blobClient) 
        { 
            _blobClient = blobClient;
        }
        public async Task<bool> DeleteBlob(string name, string containerName)
        {
            BlobContainerClient blobContainerItem = _blobClient.GetBlobContainerClient(containerName);
            var blobClient = blobContainerItem.GetBlobClient(name);
            return await blobClient.DeleteIfExistsAsync();
        }

        public async Task<List<string>> GetAllBlobs(string containerName)
        {
            BlobContainerClient blobContainerItem = _blobClient.GetBlobContainerClient(containerName);
            var blobs = blobContainerItem.GetBlobsAsync();
            var blobstring = new List<string>();
            await foreach(var blob in blobs)
            {
                blobstring.Add(blob.Name);
            }
            return blobstring;
        }

        public  async Task<string> GetBlob(string name, string containerName)
        {
            BlobContainerClient blobContainerItem = _blobClient.GetBlobContainerClient(containerName);
            var blobClient = blobContainerItem.GetBlobClient(name);
            return blobClient.Uri.AbsoluteUri;
        }

        public async Task<bool> UploadBlob(string name, IFormFile file, string containerName)
        {
            BlobContainerClient blobContainerItem = _blobClient.GetBlobContainerClient(containerName);
            var blobClient = blobContainerItem.GetBlobClient(name);
            var httpHeader = new BlobHttpHeaders()
            {
                ContentType = file.ContentType
            };

            var result = await blobClient.UploadAsync(file.OpenReadStream(),httpHeader);

            if(result != null)
            {
                return true;
            }

            return false;
        }
    }
}

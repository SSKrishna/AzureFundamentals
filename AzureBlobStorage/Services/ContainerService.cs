using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;

namespace AzureBlobStorage.Services
{
    public class ContainerService : IContainerService
    {
        private readonly BlobServiceClient _blobClient;

        public ContainerService(BlobServiceClient blobClient)
        {
            _blobClient = blobClient;
        }
        public async Task CreateContainer(string containerName)
        {
            BlobContainerClient blobContainerItem = _blobClient.GetBlobContainerClient(containerName);
            await blobContainerItem.CreateIfNotExistsAsync(PublicAccessType.BlobContainer);
        }

        public async Task DeleteContainer(string containerName)
        {
            BlobContainerClient blobContainerItem = _blobClient.GetBlobContainerClient(containerName);
            await blobContainerItem.DeleteIfExistsAsync();
        }

        public async Task<List<string>> GetAllContainer()
        {
            List<string> containerName = new();
            await foreach(BlobContainerItem blboContainerItem in _blobClient.GetBlobContainersAsync())
            {
                containerName.Add(blboContainerItem.Name);
            }
            return containerName;
        }

        public Task<List<string>> GetAllContainerAbdBlobs()
        {
            throw new NotImplementedException();
        }
    }
}

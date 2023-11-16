using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Microsoft.Extensions.Azure;

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

        public async Task<List<string>> GetAllContainerAndBlobs() 
        {
            List<string> containerAndBlobNames = new();
            containerAndBlobNames.Add("Accout Name:" + _blobClient.AccountName);
            containerAndBlobNames.Add("-------------------------------------------------------------------------------------");
            await foreach (BlobContainerItem blobContainerItem in _blobClient.GetBlobContainersAsync()) 
            {
                containerAndBlobNames.Add("---" + blobContainerItem.Name);
                BlobContainerClient _blobContainer = _blobClient.GetBlobContainerClient(blobContainerItem.Name);
                await foreach(BlobItem blobItem  in _blobContainer.GetBlobsAsync())
                {
                    containerAndBlobNames.Add("------" + blobItem.Name);
                }
                containerAndBlobNames.Add("-------------------------------------------------------------------------------------");
            }
            return containerAndBlobNames;
        }
    }
}

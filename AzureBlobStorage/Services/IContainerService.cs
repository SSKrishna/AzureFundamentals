namespace AzureBlobStorage.Services
{
    public interface IContainerService
    {
        Task<List<string>> GetAllContainerAbdBlobs();

        Task<List<string>> GetAllContainer();

        Task CreateContainer(string containerName);

        Task DeleteContainer(string containerName);

    }
}

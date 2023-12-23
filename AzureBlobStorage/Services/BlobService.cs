using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Azure.Storage.Blobs.Specialized;
using Azure.Storage.Sas;
using AzureBlobStorage.Models;

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

        public async Task<List<Blob>> GetAllBlobsWithUri(string containerName)
        {
            var blobList = new List<Blob>();
            try
            {
                BlobContainerClient blobContainerClient = _blobClient.GetBlobContainerClient(containerName);
                var blobs = blobContainerClient.GetBlobsAsync();
                await foreach (var item in blobs)
                {
                    var blobClient = blobContainerClient.GetBlobClient(item.Name);
                    Blob blobIndividual = new Blob()
                    {
                        Uri = blobClient.Uri.AbsoluteUri
                    };

                    //if (blobClient.CanGenerateSasUri)
                    //{
                    //    BlobSasBuilder sasBuilder = new()
                    //    {
                    //        BlobContainerName = blobClient.GetParentBlobContainerClient().Name,
                    //        BlobName = blobClient.Name,
                    //        Resource = "b",
                    //        ExpiresOn = DateTimeOffset.UtcNow.AddHours(1)
                    //    };

                    //    sasBuilder.SetPermissions(BlobSasPermissions.Read);

                    //    blobIndividual.Uri = blobClient.GenerateSasUri(sasBuilder).AbsoluteUri;
                    //}

                    // Creating token at container level

                    //if (blobClient.CanGenerateSasUri)
                    //{
                    //    BlobSasBuilder sasBuilder = new()
                    //    {
                    //        BlobContainerName = blobClient.Name,
                    //        Resource = "c",
                    //        ExpiresOn = DateTimeOffset.UtcNow.AddHours(1)
                    //    };

                    //    sasBuilder.SetPermissions(BlobSasPermissions.Read);

                    //    blobIndividual.Uri = blobClient.GenerateSasUri(sasBuilder).AbsoluteUri.Split('?')[1].ToString();
                    //}

                    BlobProperties blobProperties = await blobClient.GetPropertiesAsync();
                    if (blobProperties.Metadata.ContainsKey("title"))
                    {
                        blobIndividual.Title = blobProperties.Metadata["title"];
                    }
                    if (blobProperties.Metadata.ContainsKey("comment"))
                    {
                        blobIndividual.Comment = blobProperties.Metadata["comment"];
                    }
                    blobList.Add(blobIndividual);
                }
                return blobList;
            }
            catch (Exception ex)
            {

            }
            return blobList;

        }

        public  async Task<string> GetBlob(string name, string containerName)
        {
            BlobContainerClient blobContainerItem = _blobClient.GetBlobContainerClient(containerName);
            var blobClient = blobContainerItem.GetBlobClient(name);
            return blobClient.Uri.AbsoluteUri;
        }

        public async Task<bool> UploadBlob(string name, IFormFile file, string containerName, Blob blob)
        {
            BlobContainerClient blobContainerItem = _blobClient.GetBlobContainerClient(containerName);
            var blobClient = blobContainerItem.GetBlobClient(name);
            var httpHeader = new BlobHttpHeaders()
            {
                ContentType = file.ContentType
            };

            IDictionary<string, string> metadata = new Dictionary<string, string>();

            metadata.Add("title", blob.Title);
            metadata["comment"] = blob.Comment;


            var result = await blobClient.UploadAsync(file.OpenReadStream(),httpHeader,metadata );

            //metadata.Remove("title");
            //await blobClient.SetMetadataAsync(metadata);


            if(result != null)
            {
                return true;
            }

            return false;
        }
    }
}

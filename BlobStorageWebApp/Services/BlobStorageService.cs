using BlobStorageWebApp.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace BlobStorageWebApp.Services
{
    public class BlobStorageService : IBlobStorageService
    {
        private CloudBlobClient _blobClient;

        public BlobStorageService(IConfiguration configuration)
        {
            var storageConnectionString = configuration["AzureStorageAccount:ConnectionString"];

            var storageAccount = CloudStorageAccount.Parse(storageConnectionString);

            _blobClient = storageAccount.CreateCloudBlobClient();
        }

        public async Task<List<BlobContainerModel>> GetListBlobContainersAsync()
        {
            //If your storage account contains more than 5000 containers,
            //or if you have specified a page size such that the listing operation returns a subset of containers in the storage account,
            //then Azure Storage returns a continuation token with the list of containers.
            //A continuation token is an opaque value that you can use to retrieve the next set of results from Azure Storage.

            BlobContinuationToken blobContinuationToken = null;
            do
            {
                var results = await _blobClient.ListContainersSegmentedAsync(blobContinuationToken);
                blobContinuationToken = results.ContinuationToken;
                var output = results.Results.Select(
                    x => new BlobContainerModel
                    {
                        BlobContainerName = x.Name,
                        BlobContainerURL = x.Uri.AbsoluteUri,
                        PublicAccess = x.Properties.PublicAccess.ToString()
                    }).ToList();
                return output;
            } while (blobContinuationToken != null);
        }

        public async Task<bool> DeleteContainerIfExistAsync(string containerName)
        {
            //The Delete and DeleteAsync methods throw an exception if the container doesn't exist.

            //The DeleteIfExists and DeleteIfExistsAsync methods return a Boolean value indicating whether the container was deleted. I
            //If the specified container doesn't exist, then these methods return False to indicate that the container wasn't deleted.

            CloudBlobContainer container = _blobClient.GetContainerReference(containerName);
            return await container.DeleteIfExistsAsync();
        }

        public async Task<bool> CreateContainerAsync(string containerName)
        {
            CloudBlobContainer container = _blobClient.GetContainerReference(containerName);
            // Defaul public Access is Private
            return await container.CreateIfNotExistsAsync();
        }

        public async Task<List<BlobModel>> GetListBlobAsync(string containerName)
        {
            CloudBlobContainer container = _blobClient.GetContainerReference(containerName);
            if (container != null)
            {

                BlobContinuationToken blobContinuationToken = null;
                do
                {
                    BlobResultSegment results = await container.ListBlobsSegmentedAsync(blobContinuationToken);
                    blobContinuationToken = results.ContinuationToken;
                    var output = results.Results.OfType<CloudBlockBlob>().Select(
                        x => new BlobModel
                        {
                            BlobName = x.Name,
                            ContentType = x.Properties.ContentType,
                            BlobURL = x.Uri.ToString(),
                            CreatedDate =  x.Properties.Created.Value.LocalDateTime,
                        }).ToList();
                    return output;
                } while (blobContinuationToken != null);
            }
            return null;
        }

        public async Task<bool> DeleteBlobIfExistAsync(string containerName, string fileName)
        {
            CloudBlobContainer container = _blobClient.GetContainerReference(containerName);
            if (container != null)
            {
                var blockBlob = container.GetBlockBlobReference(fileName);
                return await blockBlob.DeleteIfExistsAsync();
            }
            return false;
        }

        public async Task UploadBlobAsync(Stream mediaBinaryStream, string fileName, string containerName, string contentType)
        {
            CloudBlobContainer container = _blobClient.GetContainerReference(containerName);
            var blockBlob = container.GetBlockBlobReference(fileName);
            blockBlob.Properties.ContentType = contentType;
            await blockBlob.UploadFromStreamAsync(mediaBinaryStream);
        }
    }
}

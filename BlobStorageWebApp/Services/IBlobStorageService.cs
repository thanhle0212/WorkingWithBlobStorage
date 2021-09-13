using BlobStorageWebApp.Models;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace BlobStorageWebApp.Services
{
    public interface IBlobStorageService
    {
        // Container methods
        Task<List<BlobContainerModel>> GetListBlobContainersAsync();
        Task<bool> DeleteContainerIfExistAsync(string containerName);
        Task<bool> CreateContainerAsync(string containerName);

        // Blob methods
        Task<List<BlobModel>> GetListBlobAsync(string containerName);
        Task<bool> DeleteBlobIfExistAsync(string containerName, string fileName);
        Task UploadBlobAsync(Stream mediaBinaryStream, string blobName, string containerName, string blobContentTpye);
    }
}

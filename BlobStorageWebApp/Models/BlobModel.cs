using System;

namespace BlobStorageWebApp.Models
{
    public class BlobModel
    {
        public string ContainerName { get; set; }
        public string BlobName { get; set; }
        public string BlobURL { get; set; }
        public string ContentType { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}

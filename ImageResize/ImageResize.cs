using System.IO;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using SixLabors.ImageSharp.Processing;

namespace ImageResize
{
    public static class ImageResize
    {
        [FunctionName("ImageResize")]
        public static void Run([BlobTrigger("images/{name}", Connection = "AzureWebJobsStorage")] Stream original, string name , ILogger log, [Blob("thumbnails/{name}", FileAccess.Write)] Stream resized)
        {
            log.LogInformation($"Processing Image \n Name: {name} \n Size: {original.Length} Bytes");
            //log.LogInformation($"Processing Image \n Size: {original.Length} Bytes");
            using (var image = SixLabors.ImageSharp.Image.Load(original))
            {
                image.Mutate(x => x.Resize(100, 100));
                image.Save(resized, new SixLabors.ImageSharp.Formats.Jpeg.JpegEncoder());
            }
        }
    }
}

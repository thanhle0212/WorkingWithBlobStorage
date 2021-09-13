using BlobStorageWebApp.Models;
using BlobStorageWebApp.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http.Headers;
using System.Threading.Tasks;


namespace BlobStorageWebApp.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private IBlobStorageService _blobStorageService;
        public HomeController(ILogger<HomeController> logger, IBlobStorageService blobStorageService)
        {
            _logger = logger;
            _blobStorageService = blobStorageService;
        }

        public async Task<IActionResult> Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        public async Task<IActionResult> Container()
        {
            var model = await _blobStorageService.GetListBlobContainersAsync();

            return View(model);
        }

        public IActionResult CreateContainer()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreateContainer(BlobContainerModel model)
        {
            var result = await _blobStorageService.CreateContainerAsync(model.BlobContainerName);
            if (result == true)
            {
                return RedirectToAction("Container");
            }
            else
            {
                return Error();
            }
        }

        public IActionResult DeleteContainer(string containerName)
        {
            var model = new BlobContainerModel { BlobContainerName = containerName };
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteContainer(BlobContainerModel model)
        {
            var result = await _blobStorageService.DeleteContainerIfExistAsync(model.BlobContainerName);
            if (result == true)
            {
                return RedirectToAction("Container");
            }
            else
            {
                return Error();
            }
        }

        public async Task<IActionResult> ListBlob(string containerName)
        {
            List<BlobModel> model = await _blobStorageService.GetListBlobAsync(containerName);
            ViewBag.ContainerName = containerName;
            return View(model);
        }


        public IActionResult UploadBlob(string containerName)
        {
            ViewBag.ContainerName = containerName;
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> UploadBlob(IFormFile file, string containerName)
        {
            if (file != null)
            {
                var contentType = file.ContentType;
                var originalFileName = ContentDispositionHeaderValue.Parse(file.ContentDisposition).FileName.Trim('"');
                var fileName = $"{Guid.NewGuid()}{Path.GetExtension(originalFileName)}";
                await _blobStorageService.UploadBlobAsync(file.OpenReadStream(), fileName, containerName ,contentType);
            }

            return RedirectToAction("ListBlob", new { containerName = containerName });
        }

        public IActionResult DeleteBlob(string containerName, string blobName)
        {
            var model = new BlobModel { BlobName = blobName, ContainerName = containerName };
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteBlob(BlobModel model)
        {
            var result = await _blobStorageService.DeleteBlobIfExistAsync(model.ContainerName, model.BlobName);
            if (result == true)
            {
                return RedirectToAction("ListBlob", new { containerName  = model.ContainerName } );
            }
            else
            {
                return Error();
            }
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}

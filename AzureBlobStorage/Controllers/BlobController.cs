using AzureBlobStorage.Services;
using Microsoft.AspNetCore.Mvc;

namespace AzureBlobStorage.Controllers
{
    public class BlobController : Controller
    {
        private readonly IBlobService _blobService;

        public BlobController(IBlobService blobService)
        {
                _blobService = blobService;
        }

        [HttpGet]
        public async  Task<IActionResult> Manage(string containerName)
        {
            var blobObj = await _blobService.GetAllBlobs(containerName);
            return View(blobObj);
        }

        [HttpGet]
        public IActionResult AddFile(string containerName)
        {
            return View();
        }


        [HttpPost]
        public async Task<IActionResult> AddFile(string containerName,IFormFile file)
        {
            if(file == null || file.Length < 1) { return View(); }
            var fileNmae = Path.GetFileNameWithoutExtension(file.Name) + "_" + Guid.NewGuid() + Path.GetExtension(file.FileName);
            var result = _blobService.UploadBlob(fileNmae, file, containerName);
            if (result.Result) return RedirectToAction("Index", "Container");
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> ViewFile(string name,string containerName)
        {
            return Redirect(await _blobService.GetBlob(name,containerName));
        }


        public async Task<IActionResult> DeleteFile(string name, string containerName)
        {
            await _blobService.DeleteBlob(name, containerName);
            return RedirectToAction("Index","Home");
        }
    }
}

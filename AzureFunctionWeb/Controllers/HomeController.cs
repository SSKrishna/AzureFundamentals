using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using AzureFunctionWeb.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Diagnostics;

namespace AzureFunctionWeb.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        static readonly HttpClient client = new HttpClient();
        private readonly BlobServiceClient _blobClient;
        public HomeController(ILogger<HomeController> logger,BlobServiceClient blobClient)
        {
            _logger = logger;
            _blobClient = blobClient;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]

        public async Task<IActionResult> Index(SalesRequest salesRequest,IFormFile file)
        {
            salesRequest.Id = Guid.NewGuid().ToString();
            using(var content = new StringContent(JsonConvert.SerializeObject(salesRequest),System.Text.Encoding.UTF8,"application/json"))
            {
                HttpResponseMessage response = await client.PostAsync("http://localhost:7210/api/OnSalesUploadWriteToQueue", content);
                string returnValue = response.Content.ReadAsStringAsync().Result;
            }
            if(file != null)
            {
                var filename = salesRequest.Id.ToString() + Path.GetExtension(file.FileName);
                BlobContainerClient blobServiceClient = _blobClient.GetBlobContainerClient("functionsalesrep");
                var blobClientName = blobServiceClient.GetBlobClient(filename);
                var httpHeaders = new BlobHttpHeaders
                {
                    ContentType = file.ContentType
                };

                await blobClientName.UploadAsync(file.OpenReadStream(),httpHeaders);
                return View();
            }
            return RedirectToAction(nameof(Index));
        }


        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
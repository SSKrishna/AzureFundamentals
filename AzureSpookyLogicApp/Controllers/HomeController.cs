﻿using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using AzureSpookyLogicApp.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace AzureSpookyLogicApp.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        static readonly HttpClient client = new HttpClient();
        private readonly BlobServiceClient _blobClient;

        public HomeController(ILogger<HomeController> logger, BlobServiceClient blobClient)
        {
            _logger = logger;
            _blobClient = blobClient;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Index(SpookyRequest spookyRequest,IFormFile file)
        {
            spookyRequest.Id = Guid.NewGuid().ToString();
            var JsonContent = JsonSerializer.Serialize(spookyRequest);
            using(var content = new StringContent(JsonContent,Encoding.UTF8,"application/json"))
            {
                HttpResponseMessage httpResponse = await client.PostAsync("https://prod-69.eastus.logic.azure.com:443/workflows/139489ff1d5e4e6484356b562639dd79/triggers/manual/paths/invoke?api-version=2016-10-01&sp=%2Ftriggers%2Fmanual%2Frun&sv=1.0&sig=AicGKZazhULZHt0pizJNYd8cvdzDmJE0jnz5rWiSRLQ", content);
            }
            if(file != null)
            {
                var fileName = spookyRequest.Id + Path.GetExtension(file.Name);
                BlobContainerClient blobContainerClient = _blobClient.GetBlobContainerClient("logicappholder");
                var blobClient = blobContainerClient.GetBlobClient(fileName);

                var httpHeaders = new BlobHttpHeaders()
                {
                    ContentType = file.ContentType
                };
                await blobClient.UploadAsync(file.OpenReadStream(),httpHeaders);
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
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

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Index(SpookyRequest spookyRequest)
        {
            spookyRequest.Id = Guid.NewGuid().ToString();
            var JsonContent = JsonSerializer.Serialize(spookyRequest);
            using(var content = new StringContent(JsonContent,Encoding.UTF8,"application/json"))
            {
                HttpResponseMessage httpResponse = await client.PostAsync("https://prod-69.eastus.logic.azure.com:443/workflows/139489ff1d5e4e6484356b562639dd79/triggers/manual/paths/invoke?api-version=2016-10-01&sp=%2Ftriggers%2Fmanual%2Frun&sv=1.0&sig=AicGKZazhULZHt0pizJNYd8cvdzDmJE0jnz5rWiSRLQ", content);
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
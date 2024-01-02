using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using AzureFunction.Data;
using AzureFunction.Models;
using System.Linq;

namespace AzureFunction
{
    public class GroceryAPI
    {

        private readonly AzureTangyDbContext _db;
        public GroceryAPI(AzureTangyDbContext db)
        {
            _db = db;
        }

        [FunctionName("CreateGrocery")]
        public async Task<IActionResult> CreateGrocery(
            [HttpTrigger(AuthorizationLevel.Function,"post", Route = "GroceryList")] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("Creating Grocery List Item.");

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            GroceryItem_Upsert data = JsonConvert.DeserializeObject<GroceryItem_Upsert>(requestBody);

            var groceryItem = new GroceryItem 
            { 
                Name = data.Name,
            };

            _db.GroceryItems.Add(groceryItem);
            _db.SaveChanges();
            return new OkObjectResult(groceryItem);
        }


        [FunctionName("GetGrocery")]
        public async Task<IActionResult> GetGrocery(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = "GroceryList")] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("Getting all Grocery List Item.");
            return new OkObjectResult(_db.GroceryItems.ToList());
        }



        [FunctionName("GetGroceryById")]
        public async Task<IActionResult> GetGroceryById(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = "GroceryList/{id}")] HttpRequest req,
            ILogger log,string id)
        {
            log.LogInformation("Getting Grocery List Item by ID");
            var item = _db.GroceryItems.FirstOrDefault(u => u.Id == id);
            if (item == null)
            {
                return new NotFoundResult();
            }
            return new OkObjectResult(item);
        }


        [FunctionName("UpdateGrocery")]
        public async Task<IActionResult> UpdateGrocery(
            [HttpTrigger(AuthorizationLevel.Function, "put", Route = "GroceryList/{id}")] HttpRequest req,
            ILogger log,string id)
        {
            log.LogInformation("Updated Grocery List Item.");
            var item = _db.GroceryItems.FirstOrDefault(u => u.Id == id);
            if (item == null)
            {
                return new NotFoundResult();
            }
            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            GroceryItem_Upsert data = JsonConvert.DeserializeObject<GroceryItem_Upsert>(requestBody);

            if(!string.IsNullOrEmpty(data.Name))
            {
                item.Name = data.Name;
            }
            _db.GroceryItems.Update(item);
            _db.SaveChanges();
            return new OkObjectResult(item);
        }

        [FunctionName("DeleteGrocery")]
        public async Task<IActionResult> DeleteGrocery(
            [HttpTrigger(AuthorizationLevel.Function, "delete", Route = "GroceryList/{id}")] HttpRequest req,
            ILogger log, string id)
        {
            log.LogInformation("Delete Grocery List Item.");
            var item = _db.GroceryItems.FirstOrDefault(u => u.Id == id);

            if (item == null)
            {
                return new NotFoundResult();
            }
            _db.GroceryItems.Remove(item);
            _db.SaveChanges();
            return new OkResult();
        }
    }
}

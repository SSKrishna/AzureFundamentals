using System;
using System.Collections.Generic;
using System.Linq;
using AzureFunction.Data;
using AzureFunction.Models;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;
using SendGrid.Helpers.Mail;

namespace AzureFunction
{
    public class UpdateStatusToCompletedSendEmail
    {
        private readonly AzureTangyDbContext _db;
        public UpdateStatusToCompletedSendEmail(AzureTangyDbContext db)
        {
            _db = db;
        }

        [FunctionName("UpdateStatusToCompletedSendEmail")]
        public void Run([TimerTrigger("0 */5 * * * *")]TimerInfo myTimer,
            [SendGrid(ApiKey = "CustomSendGridAppSettingKey")] IAsyncCollector<SendGridMessage> messageCollector,ILogger log)
        {
            log.LogInformation($"C# Timer trigger function executed at: {DateTime.Now}");

            IEnumerable<SalesRequest> salesRequests = _db.SalesRequests.Where(s => s.Status == "Image Processed").ToList();
            foreach (var request in salesRequests)
            {
                request.Status = "Completed";
            }
            _db.UpdateRange(salesRequests);
            _db.SaveChanges();
        }
    }
}

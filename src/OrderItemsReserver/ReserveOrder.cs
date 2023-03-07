using System;
using System.IO;
using System.Text;
using Azure.Storage.Blobs;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace OrderItemsReserver;

public class ReserveOrder
{
    private readonly string BlobConnection = Environment.GetEnvironmentVariable("AzureWebJobsStorage");
    private readonly string BlobContainerName = Environment.GetEnvironmentVariable("ContainerName");

    [FunctionName("ReserveOrder")]
    public void Run([ServiceBusTrigger("reserveorder", Connection = "SBConnectionString")] string myQueueItem, ILogger log)
    {
        string blobName = string.Empty;
        log.LogInformation($"C# ServiceBus queue trigger function processed message: {myQueueItem}");
        try
        {
            var blobClient = new BlobContainerClient(BlobConnection, BlobContainerName);
            dynamic data = JsonConvert.DeserializeObject(myQueueItem);
            blobName = $"{data.BuyerId} - {data.Id}";
            var blob = blobClient.GetBlobClient(blobName);
            using (MemoryStream ms = new MemoryStream(Encoding.UTF8.GetBytes(myQueueItem)))
            {
                blob.Upload(ms);
            }
        }
        catch (Exception ex)
        {
            log.LogError(ex, ex.Message);
            throw;
        }
        log.LogInformation($"Reservation {blobName} vhas been created successfully");
    }
}

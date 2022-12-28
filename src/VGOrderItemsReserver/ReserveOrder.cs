using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Azure.Storage.Blobs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.CSharp.RuntimeBinder;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace VGOrderItemsReserver
{
    public static class ReserveOrder
    {
        [FunctionName("ReserveOrder")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("ReserveOrder function processed a request.");
            string connection = Environment.GetEnvironmentVariable("AzureWebJobsStorage");
            string containerName = Environment.GetEnvironmentVariable("ContainerName");
            var blobClient = new BlobContainerClient(connection, containerName);
            string blobName;
            string requestBody = string.Empty;

            using (StreamReader reader = new StreamReader(req.Body))
            {
                requestBody = await reader.ReadToEndAsync();
                try
                {
                    dynamic data = JsonConvert.DeserializeObject(requestBody);
                    blobName = $"{data.BuyerId} - {data.Id}";
                    var blob = blobClient.GetBlobClient(blobName);
                    using (MemoryStream ms = new MemoryStream(Encoding.UTF8.GetBytes(requestBody)))
                    {
                        await blob.UploadAsync(ms);
                    }
                }
                catch (RuntimeBinderException ex)
                {
                    log.LogError(ex, ex.Message);
                    return new BadRequestObjectResult("Provide correct Order details");
                }
                catch (JsonException ex)
                {
                    log.LogError(ex, ex.Message);
                    return new BadRequestObjectResult("Provide correct JSON format");
                }
                log.LogInformation($"Reservation {blobName} vhas been created successfully");
                return new OkObjectResult("Reservation has been created successfully");
            }
        }
    }
}

using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace VGOrderItemsReserver;

public static class ReserveOrder
{
    private static readonly string EndpointUri = Environment.GetEnvironmentVariable("EndPointUri");
    private static readonly string PrimaryKey = Environment.GetEnvironmentVariable("PrimaryKey");
    private static readonly string DatabaseId = "ToDoList";
    private static readonly string ContainerId = "Items";

    [FunctionName("ReserveOrder")]
    public static async Task<IActionResult> Run(
        [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequest req,
        ILogger log)
    {
        CosmosClient cosmosClient = new CosmosClient(EndpointUri, PrimaryKey);
        Database database = await cosmosClient.CreateDatabaseIfNotExistsAsync(DatabaseId);
        Container container = await database.CreateContainerIfNotExistsAsync(ContainerId, "/BuyerId");

        log.LogInformation("ReserveOrder function processed a request.");
        try
        {
            using (StreamReader reader = new StreamReader(req.Body))
            {
                var requestBody = await reader.ReadToEndAsync();
                dynamic data = JsonConvert.DeserializeObject(requestBody);
                data.id = Guid.NewGuid().ToString();
                var response = await container.CreateItemAsync(data);
            }
        }
        catch (JsonException ex)
        {
            log.LogError(ex, ex.Message);
            return new BadRequestObjectResult("Provide correct JSON format");
        }
        log.LogInformation($"Reservation has been created successfully");
        return new OkObjectResult("Reservation has been created successfully");
    }
}

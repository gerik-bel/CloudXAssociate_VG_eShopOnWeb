using System.Text.Json;
using Azure.Messaging.ServiceBus;
using BlazorShared;
using Microsoft.eShopWeb.ApplicationCore.Entities.OrderAggregate;
using Microsoft.eShopWeb.Web.Interfaces;
using Microsoft.Extensions.Options;

namespace Microsoft.eShopWeb.Web.Services;

public class ReserveOrderService : IReserveOrderService
{
    private readonly BaseUrlConfiguration _baseUrlConfiguration;
    private readonly ILogger<CatalogViewModelService> _logger;

    public ReserveOrderService(IOptions<BaseUrlConfiguration> baseUrlConfiguration, ILogger<CatalogViewModelService> logger)
    {
        _baseUrlConfiguration = baseUrlConfiguration.Value;
        _logger = logger;
    }

    public async Task ReserveOrder(Order order)
    {
        try
        {
            await using var client = new ServiceBusClient(_baseUrlConfiguration.ReserveService);
            await using ServiceBusSender sender = client.CreateSender(_baseUrlConfiguration.ReserveServiceQueue);
            var message = new ServiceBusMessage(JsonSerializer.Serialize(order));
            await sender.SendMessageAsync(message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
        }
    }
}

using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using BlazorShared;
using Microsoft.eShopWeb.ApplicationCore.Entities.OrderAggregate;
using Microsoft.eShopWeb.Web.Interfaces;
using Microsoft.Extensions.Options;

namespace Microsoft.eShopWeb.Web.Services;

public class OrderProcessorService : IOrderProcessorService
{
    private readonly BaseUrlConfiguration _baseUrlConfiguration;
    private readonly ILogger<CatalogViewModelService> _logger;

    public OrderProcessorService(IOptions<BaseUrlConfiguration> baseUrlConfiguration, ILogger<CatalogViewModelService> logger)
    {
        _baseUrlConfiguration = baseUrlConfiguration.Value;
        _logger = logger;
    }

    public async Task ProcessOrder(Order order)
    {
        using (var client = new HttpClient())
        {
            using (var request = new HttpRequestMessage(HttpMethod.Post, _baseUrlConfiguration.ReserveService))
            {
                request.Content = new StringContent(JsonSerializer.Serialize(order), Encoding.UTF8, "application/json");
                request.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                try
                {
                    var response = await client.SendAsync(request, HttpCompletionOption.ResponseHeadersRead)
                        .ConfigureAwait(false);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, ex.Message);
                }
            }
        }
    }
}

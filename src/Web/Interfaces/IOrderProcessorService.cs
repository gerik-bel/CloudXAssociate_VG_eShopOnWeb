using Microsoft.AspNetCore.Mvc;
using Microsoft.eShopWeb.ApplicationCore.Entities.OrderAggregate;

namespace Microsoft.eShopWeb.Web.Interfaces;

public interface IOrderProcessorService
{
    Task ProcessOrder(Order order);
}

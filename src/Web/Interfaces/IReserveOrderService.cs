using Microsoft.AspNetCore.Mvc;
using Microsoft.eShopWeb.ApplicationCore.Entities.OrderAggregate;

namespace Microsoft.eShopWeb.Web.Interfaces;

public interface IReserveOrderService
{
    Task<IActionResult> ReserveOrder(Order order);
}

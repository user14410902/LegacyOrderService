using Microsoft.Extensions.Logging;
using OrderService.Entities;

namespace OrderService.Services.AddOrders.Display;

public class LoggerAddOrderDisplay : IAddOrderDisplay
{
  private readonly ILogger<LoggerAddOrderDisplay> _logger;

  public LoggerAddOrderDisplay(ILogger<LoggerAddOrderDisplay> logger)
  {
    _logger = logger;
  }
  public void DisplayOrder(Order order)
  {
    _logger.LogInformation($"Customer: {order.CustomerName} | Product: {order.Product.Name} | Quantity: {order.Quantity} | Total: {order.Total}");
  }
}
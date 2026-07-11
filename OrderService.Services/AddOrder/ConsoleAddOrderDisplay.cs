using Microsoft.Extensions.Logging;
using OrderService.Entities;

namespace OrderService.Services.AddOrder;

public class ConsoleAddOrderDisplay : IAddOrderDisplay
{
  private readonly ILogger<ConsoleAddOrderDisplay> _logger;

  public ConsoleAddOrderDisplay(ILogger<ConsoleAddOrderDisplay> logger)
  {
    _logger = logger;
  }
  public void DisplayOrder(Order order)
  {
    _logger.LogInformation("Customer: " + order.CustomerName);
    _logger.LogInformation("Product: " + order.Product.Name);
    _logger.LogInformation("Quantity: " + order.Quantity);
    _logger.LogInformation("Total: $" + order.Total);
  }
}
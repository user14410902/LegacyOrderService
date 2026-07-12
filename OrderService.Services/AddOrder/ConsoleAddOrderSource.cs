using Microsoft.Extensions.Logging;

namespace OrderService.Services.AddOrder;

public class ConsoleAddOrderSource : IAddOrderSource
{
  private readonly ILogger<ConsoleAddOrderSource> _logger;

  public ConsoleAddOrderSource(ILogger<ConsoleAddOrderSource> logger)
  {
    _logger = logger;
  }
  public string GetCustomerName()
  {
    return GetString("Enter customer name:", "Customer name is null or empty. A valid customer name is required.");
  }

  public string GetProductName()
  {
    return GetString("Enter product name:", "Product name is null or empty. A valid product name is required.");
  }

  public int GetQuantity()
  {
    int quantity = 0;
    bool quantityValid = false;
    while (!quantityValid)
    {
      _logger.LogInformation("Enter quantity greater zero:");
      string qtyInput = Console.ReadLine() ?? "0";
      quantityValid = Int32.TryParse(qtyInput, out quantity) && quantity > 0;
      if (!quantityValid)
      {
        _logger.LogError("Quantity is invalid. The quantity must be a whole number greater than 0.");
      }
    }
    return quantity;
  }

  private string GetString(string askMessage, string errorMessage)
  {
    string value = string.Empty;
    bool isValid = false;
    while (!isValid)
    {
      _logger.LogInformation(askMessage);
      value = (Console.ReadLine() ?? string.Empty).Trim();
      isValid = !string.IsNullOrWhiteSpace(value);
      if (!isValid)
      {
        _logger.LogError(errorMessage);
      }
    }
    return value;
  }
}
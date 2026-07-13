using Microsoft.Extensions.Logging;
using OrderService.Entities;

namespace OrderService.Services.AddOrders.Display;

public class ConsoleAddOrderDisplay : IAddOrderDisplay
{

  public void DisplayOrder(Order order)
  {
    Console.WriteLine("Customer: " + order.CustomerName);
    Console.WriteLine("Product: " + order.Product.Name);
    Console.WriteLine("Quantity: " + order.Quantity);
    Console.WriteLine("Total: $" + order.Total);
  }
}
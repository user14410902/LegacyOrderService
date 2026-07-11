using Microsoft.Extensions.Logging;

namespace OrderService.Services.AddOrder;

public class CommanLineAddOrderSources(string customerName, string productName, int quantity) : IAddOrderSources
{
  private readonly string customerName = customerName;
  private readonly string productName = productName;
  private readonly int quantity = quantity;

  public string GetCustomerName()
  {
    return customerName;
  }

  public string GetProductName()
  {
    return productName;
  }

  public int GetQuantity()
  {
    return quantity;
  }


}
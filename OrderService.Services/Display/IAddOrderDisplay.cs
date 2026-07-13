using OrderService.Entities;

namespace OrderService.Services.AddOrders.Display;

public interface IAddOrderDisplay
{
  public void DisplayOrder(Order order);
}
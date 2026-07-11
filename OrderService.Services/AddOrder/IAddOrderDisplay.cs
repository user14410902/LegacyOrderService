using OrderService.Entities;

namespace OrderService.Services.AddOrder;

public interface IAddOrderDisplay
{
  public void DisplayOrder(Order order);
}
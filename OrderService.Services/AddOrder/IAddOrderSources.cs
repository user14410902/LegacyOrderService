namespace OrderService.Services.AddOrder;

public interface IAddOrderSources
{
  public string GetCustomerName();
  public string GetProductName();
  public int GetQuantity();
}
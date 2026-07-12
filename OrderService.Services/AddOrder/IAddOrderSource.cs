namespace OrderService.Services.AddOrder;

public interface IAddOrderSource
{
  public string GetCustomerName();
  public string GetProductName();
  public int GetQuantity();
}
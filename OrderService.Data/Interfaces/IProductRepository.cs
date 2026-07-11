namespace OrderService.Data.Interfaces;

public interface IProductRepository
{
  public Task<OrderService.Entities.Product?> GetProductAsync(string productName, CancellationToken cancellationToken);

}
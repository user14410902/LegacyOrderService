using OrderService.Entities;

namespace OrderService.Data.Interfaces;

public interface IOrderRepository
{

  public Task<Guid> SaveAsync(Order order, CancellationToken cancellationToken);

}


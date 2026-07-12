using OrderService.Entities;

namespace OrderService.Data.Interfaces;

public interface IOrderRepository
{

  public void Add(Order entity);

  public Task SaveAsync(CancellationToken cancellationToken);

  public Task<Guid> AddAndSaveSingleAsync(Order order, CancellationToken cancellationToken);

}


using OrderService.Entities;

namespace OrderService.Data.Interfaces;

public interface IOrderRepository
{

  void Add(Order entity);

  Task SaveAsync(CancellationToken cancellationToken);

  Task<Guid> AddAndSaveSingleAsync(Order order, CancellationToken cancellationToken);

  Task<List<Order>> GetAllOrdersAsync(CancellationToken cancellationToken);
}


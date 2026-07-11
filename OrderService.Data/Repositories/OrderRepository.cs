using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using OrderService.Data;
using OrderService.Data.Interfaces;
using OrderService.Entities;

namespace OrderService.Data.Repositories;

public class OrderRepository : IOrderRepository
{
    private readonly OrderServiceDbContext _dbContext;

    public OrderRepository(OrderServiceDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Guid> SaveAsync(Order order, CancellationToken cancellationToken)
    {
        var modelOrder = new OrderService.Data.Models.Order
        {
            CustomerName = order.CustomerName,
            ProductId = order.Product.Id,
            Quantity = order.Quantity,
            Created = DateTime.UtcNow
        };
        _dbContext.Orders.Add(modelOrder);

        await _dbContext.SaveChangesAsync(cancellationToken);

        return modelOrder.Id;
    }
}


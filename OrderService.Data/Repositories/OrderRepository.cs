using Microsoft.EntityFrameworkCore;
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

    public async Task<List<Order>> GetAllOrdersAsync(CancellationToken cancellationToken)
    {
        return await _dbContext.Orders
        .Select(o => new Entities.Order(o.CustomerName,
        Product: new Entities.Product(Id: o.Product!.Id, Name: o.Product.Name, Price: o.Product.Price), o.Quantity))
        .ToListAsync(cancellationToken);
    }

    public void Add(Order entity)
    {
        _dbContext.Orders.Add(ModelFromEntity(entity));
    }

    public async Task SaveAsync(CancellationToken cancellationToken)
    {
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<Guid> AddAndSaveSingleAsync(Order entity, CancellationToken cancellationToken)
    {
        var model = ModelFromEntity(entity);
        _dbContext.Orders.Add(model);

        await _dbContext.SaveChangesAsync(cancellationToken);

        return model.Id;
    }

    //TODO Use Automapper?
    private Models.Order ModelFromEntity(Entities.Order entity)
    {
        return new OrderService.Data.Models.Order
        {
            CustomerName = entity.CustomerName,
            ProductId = entity.Product.Id,
            Quantity = entity.Quantity,
            Created = DateTime.UtcNow
        };
    }
}


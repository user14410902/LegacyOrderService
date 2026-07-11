using Microsoft.Extensions.DependencyInjection;
using OrderService.Data;
using OrderService.Entities;

namespace OrderService.Data
{
    public class OrderRepository
    {
        private readonly OrderServiceDbContext _dbContext;

        public OrderRepository(OrderServiceDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<Guid> Save(Order order, CancellationToken cancellationToken)
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
}

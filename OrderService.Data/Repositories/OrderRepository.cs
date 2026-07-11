using Microsoft.Extensions.DependencyInjection;
using OrderService.Data;
using OrderService.Entities;

namespace OrderService.Data
{
    public class OrderRepository
    {
        private ServiceProvider _serviceProvider;

        public OrderRepository(ServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public async Task<Guid> Save(Order order, CancellationToken cancellationToken)
        {
            using var scope = _serviceProvider.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<OrderServiceDbContext>();
            var modelOrder = new OrderService.Data.Models.Order
            {
                CustomerName = order.CustomerName,
                ProductId = order.Product.Id,
                Quantity = order.Quantity,
                Created = DateTime.UtcNow
            };
            db.Orders.Add(modelOrder);

            await db.SaveChangesAsync(cancellationToken);

            return modelOrder.Id;
        }
    }
}

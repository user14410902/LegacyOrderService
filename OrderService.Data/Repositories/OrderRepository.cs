using Microsoft.Extensions.DependencyInjection;
using OrderService.Data;
using OrderService.Entities;

namespace LegacyOrderService.Data
{
    public class OrderRepository
    {
        private ServiceProvider _serviceProvider;

        public OrderRepository(ServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public async Task Save(Order order)
        {
            using var scope = _serviceProvider.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<OrderServiceDbContext>();
            db.Orders.Add(new OrderService.Data.Models.Order
            {
                CustomerName = order.CustomerName,
                ProductId = order.Product.Id,
                Quantity = order.Quantity,
            });

            await db.SaveChangesAsync();
        }
    }
}

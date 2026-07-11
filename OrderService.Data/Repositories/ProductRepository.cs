using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using OrderService.Data;

namespace LegacyOrderService.Data
{
    public class ProductRepository
    {
        private ServiceProvider _serviceProvider;

        public ProductRepository(ServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public async Task<OrderService.Entities.Product> GetProduct(string productName)
        {
            using var scope = _serviceProvider.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<OrderServiceDbContext>();

            return await db.Products
            .Where(p => p.Name == productName)
            .AsNoTracking()
            .Select(p => new OrderService.Entities.Product(p.Id, p.Name, p.Price))
            .FirstAsync();
        }


    }
}

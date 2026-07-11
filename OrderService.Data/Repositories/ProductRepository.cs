using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using OrderService.Data;
using OrderService.Data.Interfaces;

namespace OrderService.Data
{
    public class ProductRepository : IProductRepository
    {
        private ServiceProvider _serviceProvider;

        public ProductRepository(ServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public async Task<OrderService.Entities.Product?> GetProductAsync(string productName, CancellationToken cancellationToken)
        {
            using var scope = _serviceProvider.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<OrderServiceDbContext>();

            return await db.Products
            .Where(p => p.Name == productName)
            .AsNoTracking()
            .Select(p => new OrderService.Entities.Product(p.Id, p.Name, p.Price))
            .FirstOrDefaultAsync(cancellationToken);
        }


    }
}

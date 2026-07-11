using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using OrderService.Data;
using OrderService.Data.Interfaces;

namespace OrderService.Data
{
    public class ProductRepository : IProductRepository
    {
        private readonly OrderServiceDbContext _dbContext;


        public ProductRepository(OrderServiceDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<OrderService.Entities.Product?> GetProductAsync(string productName, CancellationToken cancellationToken)
        {
            return await _dbContext.Products
            .Where(p => p.Name == productName && p.IsDeleted == false)
            .AsNoTracking()
            .Select(p => new OrderService.Entities.Product(p.Id, p.Name, p.Price))
            .FirstOrDefaultAsync(cancellationToken);
        }


    }
}

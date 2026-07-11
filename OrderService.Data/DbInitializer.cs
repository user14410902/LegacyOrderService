using OrderService.Data.Models;
namespace OrderService.Data;

public static class DbInitializer
{
  public static async Task Seed(OrderServiceDbContext context, CancellationToken cancellationToken)
  {
    context.Database.EnsureCreated();
    if (!context.Products.Any())
    {
      var defaultProducts = new List<Product>
            {
                new Product { Name = "Widget", Price = 12.99M },
                new Product { Name = "Gadget", Price = 15.49M },
                new Product { Name = "Doohickey", Price = 8.75M },
            };
      context.Products.AddRange(defaultProducts);
      await context.SaveChangesAsync(cancellationToken);
    }
  }
}